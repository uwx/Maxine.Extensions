using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using ArmAes = System.Runtime.Intrinsics.Arm.Aes;
using X86Aes = System.Runtime.Intrinsics.X86.Aes;

namespace Maxine.Extensions.Collections;

//
// NB: Why can't we use UInt128?
// There is no guarantee (afaik) that UInt128 will be enregistered into vector registers. Which means vector ops will be
// much, much slower than expected as they will only enregister during the op itself.
// So we must be careful to only use UInt128 when we do not expect to be able to enregister a vec.
//
using u64 = ulong;
using u128 = Vector128<byte>;

public struct AHash
{
    private u128 _enc;
    private u128 _sum;
    private readonly u128 _key;

    private static ReadOnlySpan<u64> Pi => [ // TODO should this be reordered for endianness?
        0x243f_6a88_85a3_08d3UL,
        0x1319_8a2e_0370_7344UL,
        0xa409_3822_299f_31d0UL,
        0x082e_fa98_ec4e_6c89UL,
        
        // 0x082e_fa98_ec4e_6c89UL,
        // 0xa409_3822_299f_31d0UL,
        // 0x1319_8a2e_0370_7344UL,
        // 0x243f_6a88_85a3_08d3UL,
    ];

    private static ReadOnlySpan<u64> Pi2 => [
        0x4528_21e6_38d0_1377UL,
        0xbe54_66cf_34e9_0c6cUL,
        0xc0ac_29b7_c97c_50ddUL,
        0x3f84_d5b5_b547_0917UL,
    ];

    private static ReadOnlySpan<u128> PiTransposed => MemoryMarshal.Cast<u64, u128>(Pi);

    private static u128 ToU128(UInt128 uInt128)
    {
        return Unsafe.BitCast<UInt128, u128>(uInt128);
    }


    public AHash(UInt128 key1, UInt128 key2) : this(ToU128(key1), ToU128(key2))
    {
        
    }

    public AHash(u128 key1, u128 key2)
    {
        Unsafe.SkipInit(out _enc);
        Unsafe.SkipInit(out _sum);
        Unsafe.SkipInit(out _key);

        key1 ^= PiTransposed[0];
        key2 ^= PiTransposed[1];
        _enc = key1;
        _sum = key2;
        _key = key1 ^ key2;
    }

    public AHash(Random random)
    {
        Unsafe.SkipInit(out _enc);
        Unsafe.SkipInit(out _sum);
        Unsafe.SkipInit(out _key);
        
        Span<u128> keys = stackalloc u128[2];
        
        random.NextBytes(MemoryMarshal.Cast<u128, byte>(keys));
        _enc = keys[0];
        _sum = keys[1];
        _key = keys[0] ^ keys[1];
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static u128 AesEncrypt(u128 value, u128 xor)
    {
        if (ArmAes.IsSupported)
        {
            return xor ^ ArmAes.MixColumns(ArmAes.Encrypt(value, Vector128.Create<byte>(0)));
        }

        if (X86Aes.IsSupported)
        {
            return X86Aes.Encrypt(value, xor);
        }

        return ThrowNotSupported();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static u128 AesDecrypt(u128 value, u128 xor)
    {
        if (ArmAes.IsSupported)
        {
            return xor ^ ArmAes.InverseMixColumns(ArmAes.Decrypt(value, Vector128.Create<byte>(0)));
        }

        if (X86Aes.IsSupported)
        {
            return X86Aes.Decrypt(value, xor);
        }
        
        return ThrowNotSupported();
    }
    
    [DoesNotReturn]
    private static u128 ThrowNotSupported()
    {
        throw new NotSupportedException();
    }

    private static ReadOnlySpan<int> ShuffleMask1 => [0x020a0700, 0x0c01030e, 0x050f0d08, 0x06090b04]; // TODO is endianness wrong here?

    private static ReadOnlySpan<int> ShuffleMask => [0x06090b04, 0x050f0d08, 0x0c01030e, 0x020a0700]; // TODO is endianness wrong here?

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static u128 Shuffle(u128 a)
    {
        if (Vector128.IsHardwareAccelerated)
        {
            return Vector128.Shuffle(a, Vector128.Create(ShuffleMask).AsByte());
        }

        // "swap_bytes"
        return Unsafe.BitCast<UInt128, u128>(BinaryPrimitives.ReverseEndianness(Unsafe.BitCast<u128, UInt128>(a)));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static u128 shuffle_and_add(u128 b, u128 toAdd)
    {
        var shuffled = Shuffle(b).AsUInt64();
        return add_by_64s(shuffled, toAdd.AsUInt64()).AsByte();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector128<u64> add_by_64s(Vector128<u64> a, Vector128<u64> b)
    {
        // assuming wrapping_add is the same as this......
        // also the intrinsic is probably faster than our own scalar impl, so use it.
        return a + b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void hash_in(u128 newValue) {
        _enc = AesDecrypt(_enc, newValue);
        _sum = shuffle_and_add(_sum, newValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void hash_in_2(u128 v1, u128 v2) {
        _enc = AesDecrypt(_enc, v1);
        _sum = shuffle_and_add(_sum, v1);
        _enc = AesDecrypt(_enc, v2);
        _sum = shuffle_and_add(_sum, v2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly u64 short_finish() {
        var combined = AesEncrypt(_sum, _enc);
        var reference = AesDecrypt(combined, combined);
        var result = reference.AsUInt64();
        return result[0];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void add_in_length(ref u128 enc, u64 len)
    {
        if (Vector128.IsHardwareAccelerated)
        {
            enc = (enc.AsUInt64() + Vector128.CreateScalar(len)).AsByte();
        }

        var len_ = Vector128.CreateScalar(len);
        enc = (enc.AsUInt64() + len_).AsByte();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private u128 Makevec(UInt128 u) => Unsafe.BitCast<UInt128, u128>(u);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Write(ReadOnlySpan<byte> input) {
        var data = input;
        var length = data.Length;
        add_in_length(ref _enc, (u64)length);

        //A 'binary search' on sizes reduces the number of comparisons.
        switch (data.Length)
        {
            case > 64:
            {
                // var tails = MemoryMarshal.Cast<byte, u128>(data)[^4..];

                // TODO: these may confuse the JIT 
                var tail0 = Vector128.Create(data[^64..^48]);
                var tail1 = Vector128.Create(data[^48..^32]);
                var tail2 = Vector128.Create(data[^32..^16]);
                var tail3 = Vector128.Create(data[^16..]);

                var current0 = _key;
                var current1 = _key;
                var current2 = _key;
                var current3 = _key;
                current0 = AesEncrypt(current0, tail0);
                current1 = AesDecrypt(current1, tail1);
                current2 = AesEncrypt(current2, tail2);
                current3 = AesDecrypt(current3, tail3);
                var sum0 = _key;
                var sum1 = Vector128.Negate(_key);
                sum0 = add_by_64s(sum0.AsUInt64(), tail0.AsUInt64()).AsByte();
                sum1 = add_by_64s(sum1.AsUInt64(), tail1.AsUInt64()).AsByte();
                sum0 = shuffle_and_add(sum0, tail2);
                sum1 = shuffle_and_add(sum1, tail3);
                while (data.Length > 64)
                {
                    // var blocks = MemoryMarshal.Cast<byte, u128>(data)[..4];
                    var blocks0 = Vector128.Create(data[..16]);
                    var blocks1 = Vector128.Create(data[16..32]);
                    var blocks2 = Vector128.Create(data[32..48]);
                    var blocks3 = Vector128.Create(data[48..64]);

                    var rest = data[64..];
                    current0 = AesDecrypt(current0, blocks0);
                    current1 = AesDecrypt(current1, blocks1);
                    current2 = AesDecrypt(current2, blocks2);
                    current3 = AesDecrypt(current3, blocks3);
                    sum0 = shuffle_and_add(sum0, blocks0);
                    sum1 = shuffle_and_add(sum1, blocks1);
                    sum0 = shuffle_and_add(sum0, blocks2);
                    sum1 = shuffle_and_add(sum1, blocks3);
                    data = rest;
                }
                hash_in_2(current0, current1);
                hash_in_2(current2, current3);
                hash_in_2(sum0, sum1);
                break;
            }
            case > 32:
            {
                //len 33-64
                var head = MemoryMarshal.Cast<byte, u128>(data)[..2];
                var tail = MemoryMarshal.Cast<byte, u128>(data)[^2..];
                hash_in_2(head[0], head[1]);
                hash_in_2(tail[0], tail[1]);
                break;
            }
            case > 16:
            {
                //len 17-32
                var head = BinaryPrimitives.ReadUInt128LittleEndian(data);
                var tail = BinaryPrimitives.ReadUInt128LittleEndian(data[^16..]);
                hash_in_2(Makevec(head), Makevec(tail));
                break;
            }
            case > 8:
            {
                //len 9-16
                var head = BinaryPrimitives.ReadUInt64LittleEndian(data);
                var tail = BinaryPrimitives.ReadUInt64LittleEndian(data[^sizeof(u64)..]);
                hash_in(Vector128.Create([head, tail]).AsByte());
                break;
            }
            case >= 4:
            {
                //len 4-7
                var value = Vector128.Create<u64>([
                    BinaryPrimitives.ReadUInt32LittleEndian(data),
                    BinaryPrimitives.ReadUInt32LittleEndian(data[^sizeof(uint)..])
                ]);
                hash_in(value.AsByte());
                break;
            }
            case >= 2:
            {
                //len 2-3
                var value = Vector128.Create<u64>([BinaryPrimitives.ReadUInt16LittleEndian(data), data[^1]]);
                hash_in(value.AsByte());
                break;
            }
            case > 0:
            {
                // len 1
                hash_in(Vector128.Create<u64>(data[0]).AsByte());
                break;
            }
            default:
                hash_in(u128.Zero);
                break;
        }
    }
    
    public readonly u64 Finish()
    {
        var combined = AesEncrypt(_sum, _enc);
        var result = AesDecrypt(AesDecrypt(combined, _key), combined).AsUInt64();
        return result[0];
    }
}