// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

#pragma warning disable CA1810 // Initialize reference type static fields inline

namespace Maxine.Extensions.Collections;

// Probe sequence based on triangular numbers, which is guaranteed (since our
// table size is a power of two) to visit every group of elements exactly once.
//
// A triangular probe has us jump by 1 more group every time. So first we
// jump by 1 group (meaning we just continue our linear scan), then 2 groups
// (skipping over 1 group), then 3 groups (skipping over 2 groups), and so on.
//
// The proof is a simple number theory question: i*(i+1)/2 can walk through the complete residue system of 2n
// to prove this, we could prove when "0 <= i <= j < 2n", "i * (i + 1) / 2 mod 2n == j * (j + 1) / 2" iff "i == j"
// sufficient: we could have `(i-j)(i+j+1)=4n*k`, k is integer. It is obvious that if i!=j, the left part is odd, but right is always even.
// So, the the only chance is i==j
// necessary: obvious
// Q.E.D.
internal struct ProbeSeq
{
    internal int pos;
    private int _stride;
    private readonly int _bucket_mask;

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ProbeSeq(int hash, int bucket_mask)
    {
        _bucket_mask = bucket_mask;
        pos = SwissTableHelper.h1(hash) & bucket_mask;
        _stride = 0;
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void move_next()
    {
        // We should have found an empty bucket by now and ended the probe.
        Debug.Assert(_stride <= _bucket_mask, "Went past end of probe sequence");
        _stride += SwissTableHelper.GROUP_WIDTH;
        pos += _stride;
        pos &= _bucket_mask;
    }
}

internal static unsafe class SwissTableHelper
{
    private interface IIndirectComparer<in TSelf, in T> where TSelf : IIndirectComparer<TSelf, T>
    {
        static abstract bool Equals(TSelf? self, T? x, T? y);
        static abstract int GetHashCode(TSelf? self, [DisallowNull] T obj);
    }

    // TODO: unknown if this devirts?
    // https://github.com/dotnet/runtime/issues/10050
    private readonly struct DefaultValueTypeComparer<T> : IIndirectComparer<DefaultValueTypeComparer<T>,T>
    {
        public static bool Equals(DefaultValueTypeComparer<T> self, T? x, T? y) => EqualityComparer<T>.Default.Equals(x, y);
        public static int GetHashCode(DefaultValueTypeComparer<T> self, T obj) => EqualityComparer<T>.Default.GetHashCode(obj!);
    }

    private readonly struct DefaultRefTypeComparer<T> : IIndirectComparer<DefaultRefTypeComparer<T>,T>
    {
        public static bool Equals(DefaultRefTypeComparer<T> self, T? x, T? y) => object.Equals(x, y);
        public static int GetHashCode(DefaultRefTypeComparer<T> self, T obj) => obj?.GetHashCode() ?? 0;
    }

    private readonly struct CustomComparer<T>(IEqualityComparer<T> comparer) : IIndirectComparer<CustomComparer<T>,T>
    {
        private readonly IEqualityComparer<T> _comparer = comparer;

        public static bool Equals(CustomComparer<T> self, T? x, T? y) => self._comparer.Equals(x, y);
        public static int GetHashCode(CustomComparer<T> self, T obj) => self._comparer.GetHashCode(obj);
    }
    
    public static readonly int GROUP_WIDTH = InitialGroupWidth();

    public static int InitialGroupWidth()
    {
        if (Avx2.IsSupported)
        {
            return Avx2Group.WIDTH;
        }

        if (Sse2.IsSupported)
        {
            return Sse2Group.WIDTH;
        }

        return FallbackGroup.WIDTH;
    }

    /// Control byte value for an empty bucket.
    public const byte EMPTY = 0b1111_1111;

    /// Control byte value for a deleted bucket.
    public const byte DELETED = 0b1000_0000;

    /// Checks whether a control byte represents a full bucket (top bit is clear).
    public static bool is_full(byte ctrl) => (ctrl & 0x80) == 0;

    /// Checks whether a control byte represents a special value (top bit is set).
    public static bool is_special(byte ctrl) => (ctrl & 0x80) != 0;

    /// Checks whether a special control value is EMPTY (just check 1 bit).
    public static bool special_is_empty(byte ctrl)
    {
        Debug.Assert(is_special(ctrl));
        return (ctrl & 0x01) != 0;
    }

    /// Checks whether a special control value is EMPTY.
    // optimise: return 1 as true, 0 as false
    public static int special_is_empty_with_int_return(byte ctrl)
    {
        Debug.Assert(is_special(ctrl));
        return ctrl & 0x01;
    }

    /// Primary hash function, used to select the initial bucket to probe from.
    public static int h1(int hash)
    {
        return hash;
    }

    /// Secondary hash function, saved in the low 7 bits of the control byte.
    public static byte h2(int hash)
    {
        // Grab the top 7 bits of the hash.
        // cast to uint to use `shr` rahther than `sar`, which makes sure the top bit of returned byte is 0.
        var top7 = (uint)hash >> 25;
        return (byte)top7;
    }

    // DISPATHCH METHODS

    // Generally we do not want to duplicate code, but for performance(use struct and inline), we have to do so.
    // The difference between mirror implmentations should only be `_dummyGroup` except `MoveNext`, in which we use C++ union trick

    // For enumerator, which need record the current state
    [StructLayout(LayoutKind.Explicit)]
    internal struct BitMaskUnion
    {
        [FieldOffset(0)]
        internal Avx2BitMask avx2BitMask;
        [FieldOffset(0)]
        internal Sse2BitMask sse2BitMask;
        [FieldOffset(0)]
        internal FallbackBitMask fallbackBitMask;
    }

    // maybe we should just pass bucket_mask in as parater rather than calculate
    private static int GetBucketMaskFromControlsLength(int controlsLength)
    {
        Debug.Assert(controlsLength >= GROUP_WIDTH);
        if (controlsLength == GROUP_WIDTH)
            return 0;
        return controlsLength - GROUP_WIDTH - 1;
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] DispatchGetEmptyControls()
    {
        if (Avx2.IsSupported)
        {
            return Avx2Group.StaticEmpty;
        }

        if (Sse2.IsSupported)
        {
            return Sse2Group.StaticEmpty;
        }

        return FallbackGroup.StaticEmpty;
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitMaskUnion DispatchGetMatchFullBitMask(byte[] controls, int index)
    {
        if (Avx2.IsSupported)
        {
            var v = GetMatchFullBitMaskForGeneric<Avx2Group, Avx2BitMask>(controls, index);
            return Unsafe.As<Avx2BitMask, BitMaskUnion>(ref v);
        }

        if (Sse2.IsSupported)
        {
            var v = GetMatchFullBitMaskForGeneric<Sse2Group, Sse2BitMask>(controls, index);
            return Unsafe.As<Sse2BitMask, BitMaskUnion>(ref v);
        }

        {
            var v = GetMatchFullBitMaskForGeneric<FallbackGroup, FallbackBitMask>(controls, index);
            return Unsafe.As<FallbackBitMask, BitMaskUnion>(ref v);
        }
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static TBitmask GetMatchFullBitMaskForGeneric<TGroup, TBitmask>(byte[] controls, int index)
        where TGroup : struct, IGroup<TBitmask, TGroup>
        where TBitmask : struct, IBitMask<TBitmask>
    {
        TBitmask result = default;
        fixed (byte* ctrl = &controls[index])
        {
            result = TGroup.Load(ctrl).MatchFull();
        }
        return result;
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref SwissTable<TKey, TValue>.Entry DispatchMoveNextDictionary<TKey, TValue>(
        int version,
        int tolerantVersion,
        in SwissTable<TKey, TValue> dictionary,
        ref int currentCtrlOffset,
        ref BitMaskUnion currentBitMask
    )
        where TKey : notnull
    {
        if (Avx2.IsSupported)
        {
            return ref MoveNextDictionaryForGeneric<Avx2Group, Avx2BitMask, TKey, TValue>(version, tolerantVersion, in dictionary, ref currentCtrlOffset, ref currentBitMask);
        }

        if (Sse2.IsSupported)
        {
            return ref MoveNextDictionaryForGeneric<Sse2Group, Sse2BitMask, TKey, TValue>(version, tolerantVersion, in dictionary, ref currentCtrlOffset, ref currentBitMask);
        }
        
        return ref MoveNextDictionaryForGeneric<FallbackGroup, FallbackBitMask, TKey, TValue>(version, tolerantVersion, in dictionary, ref currentCtrlOffset, ref currentBitMask);
    }


    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref SwissTable<TKey, TValue>.Entry MoveNextDictionaryForGeneric<TGroup, TBitmask, TKey, TValue>(
        int version,
        int tolerantVersion,
        in SwissTable<TKey, TValue> dictionary,
        ref int currentCtrlOffset,
        ref BitMaskUnion currentBitMask
    )
        where TGroup : struct, IGroup<TBitmask, TGroup>
        where TBitmask : struct, IBitMask<TBitmask>
        where TKey : notnull
    {
        var controls = dictionary.rawTable._controls;
        var entries = dictionary.rawTable._entries;

        ref var realBitMask = ref Unsafe.As<BitMaskUnion, TBitmask>(ref currentBitMask);

        if (version != dictionary._version)
        {
            ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
        }
        if (tolerantVersion != dictionary._tolerantVersion)
        {
            var newBitMask = GetMatchFullBitMaskForGeneric<TGroup, TBitmask>(controls, currentCtrlOffset);
            realBitMask = realBitMask.And(newBitMask);
        }

        while (true)
        {
            var lowest_set_bit = realBitMask.LowestSetBit();
            if (lowest_set_bit >= 0)
            {
                Debug.Assert(entries != null);
                realBitMask = realBitMask.RemoveLowestBit();
                ref var entry = ref entries[currentCtrlOffset + lowest_set_bit];
                return ref entry;
            }
            currentCtrlOffset += GROUP_WIDTH;
            if (currentCtrlOffset >= dictionary._buckets)
            {
                return ref Unsafe.NullRef<SwissTable<TKey, TValue>.Entry>();
            }
            realBitMask = GetMatchFullBitMaskForGeneric<TGroup, TBitmask>(controls, currentCtrlOffset);
        }
    }
    
    // If we are inside a continuous block of Group::WIDTH full or deleted
    // cells then a probe window may have seen a full block when trying to
    // insert. We therefore need to keep that block non-empty so that
    // lookups will continue searching to the next probe window.
    //
    // Note that in this context `leading_zeros` refers to the bytes at the
    // end of a group, while `trailing_zeros` refers to the bytes at the
    // begining of a group.
    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool DispatchIsEraseSafeToSetEmptyControlFlag(int bucketMask, byte[] controls, int index)
    {
        if (Avx2.IsSupported)
        {
            return IsEraseSafeToSetEmptyControlFlagForGeneric<Avx2Group, Avx2BitMask>(bucketMask, controls, index);
        }

        if (Sse2.IsSupported)
        {
            return IsEraseSafeToSetEmptyControlFlagForGeneric<Sse2Group, Sse2BitMask>(bucketMask, controls, index);
        }

        return IsEraseSafeToSetEmptyControlFlagForGeneric<FallbackGroup, FallbackBitMask>(bucketMask, controls, index);
        
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool IsEraseSafeToSetEmptyControlFlagForGeneric<TGroup, TBitmask>(int bucketMask, byte[] controls, int index)
            where TGroup : struct, IGroup<TBitmask, TGroup>
            where TBitmask : struct, IBitMask<TBitmask>
        {
            Debug.Assert(bucketMask == GetBucketMaskFromControlsLength(controls.Length));
            var indexBefore = unchecked((index - GROUP_WIDTH) & bucketMask);
            fixed (byte* ptr_before = &controls[indexBefore])
            fixed (byte* ptr = &controls[index])
            {
                var empty_before = TGroup.Load(ptr_before).MatchEmpty();
                var empty_after = TGroup.Load(ptr).MatchEmpty();
                return empty_before.LeadingZeros() + empty_after.TrailingZeros() < GROUP_WIDTH;
            }
        }

    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref SwissTable<TKey, TValue>.Entry DispatchFindBucketOfDictionary<TKey, TValue>(SwissTable<TKey, TValue> dictionary, TKey key, int hashOfKey)
        where TKey : notnull
    {
        if (Avx2.IsSupported)
        {
            return ref FindBucketOfDictionaryForGeneric<Avx2Group, Avx2BitMask>(dictionary, key, hashOfKey);
        }

        if (Sse2.IsSupported)
        {
            return ref FindBucketOfDictionaryForGeneric<Sse2Group, Sse2BitMask>(dictionary, key, hashOfKey);
        }

        return ref FindBucketOfDictionaryForGeneric<FallbackGroup, FallbackBitMask>(dictionary, key, hashOfKey);
            
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ref SwissTable<TKey, TValue>.Entry FindBucketOfDictionaryForGeneric<TGroup, TBitmask>(SwissTable<TKey, TValue> dictionary, TKey key, int hash)
            where TGroup : struct, IGroup<TBitmask, TGroup>
            where TBitmask : struct, IBitMask<TBitmask>
        {
            var hashComparer = dictionary._comparer;

            if (hashComparer == null)
            {
                if (typeof(TKey).IsValueType)
                {
                    return ref RunWithComparer(default(DefaultValueTypeComparer<TKey>), dictionary, key, hash);
                }
                
                return ref RunWithComparer(default(DefaultRefTypeComparer<TKey>), dictionary, key, hash);
            }

            return ref RunWithComparer(new CustomComparer<TKey>(hashComparer), dictionary, key, hash);

            [SkipLocalsInit]
            static ref SwissTable<TKey, TValue>.Entry RunWithComparer<TComparer>(TComparer comparerSelf, SwissTable<TKey, TValue> dictionary, TKey key, int hash)
                where TComparer : IIndirectComparer<TComparer, TKey>
            {
                var controls = dictionary.rawTable._controls;
                var entries = dictionary.rawTable._entries;
                var bucketMask = dictionary.rawTable._bucket_mask;

                Debug.Assert(controls != null);

                var h2_hash = h2(hash);
                var targetGroup = TGroup.Create(h2_hash);
                var probeSeq = new ProbeSeq(hash, bucketMask);

                fixed (byte* ptr = &controls[0])
                {
                    while (true)
                    {
                        var group = TGroup.Load(ptr + probeSeq.pos);
                        var bitmask = group.MatchGroup(targetGroup);
                        // TODO: Iterator and performance, if not influence, iterator would be clearer.
                        while (bitmask.AnyBitSet())
                        {
                            // there must be set bit
                            Debug.Assert(entries != null);
                            var bit = bitmask.LowestSetBitNonzero();
                            bitmask = bitmask.RemoveLowestBit();
                            var index = (probeSeq.pos + bit) & bucketMask;
                            ref var entry = ref entries[index];
                            if (TComparer.Equals(comparerSelf, key, entry.Key))
                            {
                                return ref entry;
                            }
                        }
                        if (group.MatchEmpty().AnyBitSet())
                        {
                            return ref Unsafe.NullRef<SwissTable<TKey, TValue>.Entry>();
                        }
                        probeSeq.move_next();
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Find the index of given key, negative means not found.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <returns>
    /// negative return value means not found
    /// </returns>
    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int DispatchFindBucketIndexOfDictionary<TKey, TValue>(SwissTable<TKey, TValue> dictionary, TKey key)
        where TKey : notnull
    {
        if (Avx2.IsSupported)
        {
            return FindBucketIndexOfDictionaryForGeneric<Avx2Group, Avx2BitMask>(dictionary, key);
        }

        if (Sse2.IsSupported)
        {
            return FindBucketIndexOfDictionaryForGeneric<Sse2Group, Sse2BitMask>(dictionary, key);
        }

        return FindBucketIndexOfDictionaryForGeneric<FallbackGroup, FallbackBitMask>(dictionary, key);
        
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int FindBucketIndexOfDictionaryForGeneric<TGroup, TBitmask>(SwissTable<TKey, TValue> dictionary, TKey key)
            where TGroup : struct, IGroup<TBitmask, TGroup>
            where TBitmask : struct, IBitMask<TBitmask>
        {
            var hashComparer = dictionary._comparer;

            if (hashComparer == null)
            {
                if (typeof(TKey).IsValueType)
                {
                    return RunWithComparer(default(DefaultValueTypeComparer<TKey>), dictionary, key);
                }
                
                return RunWithComparer(default(DefaultRefTypeComparer<TKey>), dictionary, key);
            }

            return RunWithComparer(new CustomComparer<TKey>(hashComparer), dictionary, key);

            [SkipLocalsInit]
            static int RunWithComparer<TComparer>(TComparer comparerSelf, SwissTable<TKey,TValue> dictionary, TKey key)
                where TComparer : IIndirectComparer<TComparer, TKey>
            {
                var controls = dictionary.rawTable._controls;
                var entries = dictionary.rawTable._entries;
                var bucketMask = dictionary.rawTable._bucket_mask;

                Debug.Assert(controls != null);

                var hash = TComparer.GetHashCode(comparerSelf, key);
                var h2_hash = h2(hash);
                var targetGroup = TGroup.Create(h2_hash);
                var probeSeq = new ProbeSeq(hash, bucketMask);

                fixed (byte* ptr = &controls[0])
                {
                    while (true)
                    {
                        var group = TGroup.Load(ptr + probeSeq.pos);
                        var bitmask = group.MatchGroup(targetGroup);
                        // TODO: Iterator and performance, if not influence, iterator would be clearer.
                        while (bitmask.AnyBitSet())
                        {
                            // there must be set bit
                            Debug.Assert(entries != null);
                            var bit = bitmask.LowestSetBitNonzero();
                            bitmask = bitmask.RemoveLowestBit();
                            var index = (probeSeq.pos + bit) & bucketMask;
                            ref var entry = ref entries[index];
                            if (TComparer.Equals(comparerSelf, key, entry.Key))
                            {
                                return index;
                            }
                        }
                        if (group.MatchEmpty().AnyBitSet())
                        {
                            return -1;
                        }
                        probeSeq.move_next();
                    }
                }
            }
        }
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DispatchCopyToArrayFromDictionaryWorker<TKey, TValue>(SwissTable<TKey, TValue> dictionary, KeyValuePair<TKey, TValue>[] destArray, int index)
        where TKey : notnull
    {
        if (Avx2.IsSupported)
        {
            CopyToArrayFromDictionaryWorkerForGeneric<Avx2Group, Avx2BitMask>(dictionary, destArray, index);
        }
        else if (Sse2.IsSupported)
        {
            CopyToArrayFromDictionaryWorkerForGeneric<Sse2Group, Sse2BitMask>(dictionary, destArray, index);
        }
        else
        {
            CopyToArrayFromDictionaryWorkerForGeneric<FallbackGroup, FallbackBitMask>(dictionary, destArray, index);
        }

        return;

        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void CopyToArrayFromDictionaryWorkerForGeneric<TGroup, TBitmask>(SwissTable<TKey, TValue> dictionary, KeyValuePair<TKey, TValue>[] destArray, int index)
            where TGroup : struct, IGroup<TBitmask, TGroup>
            where TBitmask : struct, IBitMask<TBitmask>
        {
            var offset = 0;
            var controls = dictionary.rawTable._controls;
            var entries = dictionary.rawTable._entries;
            var buckets = entries?.Length ?? 0;

            Debug.Assert(controls != null);

            fixed (byte* ptr = &controls[0])
            {
                var bitMask = TGroup.Load(ptr).MatchFull();
                while (true)
                {
                    var lowestSetBit = bitMask.LowestSetBit();
                    if (lowestSetBit >= 0)
                    {
                        Debug.Assert(entries != null);
                        bitMask = bitMask.RemoveLowestBit();
                        ref var entry = ref entries[offset + lowestSetBit];
                        destArray[index++] = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
                        continue;
                    }
                    offset += GROUP_WIDTH;
                    if (offset >= buckets)
                    {
                        break;
                    }
                    bitMask = TGroup.Load(ptr + offset).MatchFull();
                }
            }
        }
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int DispatchFindInsertSlot(int hash, byte[] contorls, int bucketMask)
    {
        if (Avx2.IsSupported)
        {
            return FindInsertSlotForGeneric<Avx2Group, Avx2BitMask>(hash, contorls, bucketMask);
        }

        if (Sse2.IsSupported)
        {
            return FindInsertSlotForGeneric<Sse2Group, Sse2BitMask>(hash, contorls, bucketMask);
        }
        return FindInsertSlotForGeneric<FallbackGroup, FallbackBitMask>(hash, contorls, bucketMask);

        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int FindInsertSlotForGeneric<TGroup, TBitmask>(int hash, byte[] contorls, int bucketMask)
            where TGroup : struct, IGroup<TBitmask, TGroup>
            where TBitmask : struct, IBitMask<TBitmask>
        {
            Debug.Assert(bucketMask == GetBucketMaskFromControlsLength(contorls.Length));
            var probeSeq = new ProbeSeq(hash, bucketMask);
            fixed (byte* ptr = &contorls[0])
            {
                while (true)
                {
                    // TODO: maybe we should lock even fix the whole loop.
                    // I am not sure which would be faster.
                    var bit = TGroup.Load(ptr + probeSeq.pos)
                        .MatchEmptyOrDeleted()
                        .LowestSetBit();
                    if (bit >= 0)
                    {
                        var result = (probeSeq.pos + bit) & bucketMask;

                        // In tables smaller than the group width, trailing control
                        // bytes outside the range of the table are filled with
                        // EMPTY entries. These will unfortunately trigger a
                        // match, but once masked may point to a full bucket that
                        // is already occupied. We detect this situation here and
                        // perform a second scan starting at the begining of the
                        // table. This second scan is guaranteed to find an empty
                        // slot (due to the load factor) before hitting the trailing
                        // control bytes (containing EMPTY).
                        if (!is_full(*(ptr + result)))
                        {
                            return result;
                        }
                        Debug.Assert(bucketMask < GROUP_WIDTH);
                        Debug.Assert(probeSeq.pos != 0);
                        return TGroup.Load(ptr)
                            .MatchEmptyOrDeleted()
                            .LowestSetBitNonzero();
                    }
                    probeSeq.move_next();
                }
            }
        }
    }

}