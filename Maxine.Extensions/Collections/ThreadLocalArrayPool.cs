using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Maxine.Extensions.Collections;

public sealed class ThreadLocalArrayPool<T> : ArrayPool<T>, IDisposable
{
    /// <summary>The default maximum length of each array in the pool (2^20).</summary>
    private const int DefaultMaxArrayLength = 1024 * 1024;
    /// <summary>The default maximum number of arrays per bucket that are available for rent.</summary>
    private const int DefaultMaxNumberOfArraysPerBucket = 50;

    private readonly Bucket[] _buckets;

    public ThreadLocalArrayPool() : this(DefaultMaxArrayLength, DefaultMaxNumberOfArraysPerBucket)
    {
    }

    public ThreadLocalArrayPool(int maxArrayLength, int maxArraysPerBucket)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxArrayLength);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxArraysPerBucket);

        // Our bucketing algorithm has a min length of 2^4 and a max length of 2^30.
        // Constrain the actual max used to those values.
        const int MinimumArrayLength = 0x10, MaximumArrayLength = 0x40000000;
        if (maxArrayLength > MaximumArrayLength)
        {
            maxArrayLength = MaximumArrayLength;
        }
        else if (maxArrayLength < MinimumArrayLength)
        {
            maxArrayLength = MinimumArrayLength;
        }

        // Create the buckets.
        int maxBuckets = Utilities.SelectBucketIndex(maxArrayLength);
        var buckets = new Bucket[maxBuckets + 1];
        for (int i = 0; i < buckets.Length; i++)
        {
            buckets[i] = new Bucket(Utilities.GetMaxSizeForBucket(i), maxArraysPerBucket);
        }
        _buckets = buckets;
    }

    public override T[] Rent(int minimumLength)
    {
        // Arrays can't be smaller than zero.  We allow requesting zero-length arrays (even though
        // pooling such an array isn't valuable) as it's a valid length array, and we want the pool
        // to be usable in general instead of using `new`, even for computed lengths.
        ArgumentOutOfRangeException.ThrowIfNegative(minimumLength);
        if (minimumLength == 0)
        {
            // No need for events with the empty array.  Our pool is effectively infinite
            // and we'll never allocate for rents and never store for returns.
            return [];
        }

        T[]? buffer;

        int index = Utilities.SelectBucketIndex(minimumLength);
        if (index < _buckets.Length)
        {
            // Search for an array starting at the 'index' bucket. If the bucket is empty, bump up to the
            // next higher bucket and try that one, but only try at most a few buckets.
            const int MaxBucketsToTry = 2;
            int i = index;
            do
            {
                // Attempt to rent from the bucket.  If we get a buffer from it, return it.
                buffer = _buckets[i].Rent();
                if (buffer != null)
                {
                    return buffer;
                }
            }
            while (++i < _buckets.Length && i != index + MaxBucketsToTry);

            // The pool was exhausted for this buffer size.  Allocate a new buffer with a size corresponding
            // to the appropriate bucket.
            buffer = new T[_buckets[index].BufferLength];
        }
        else
        {
            // The request was for a size too large for the pool.  Allocate an array of exactly the requested length.
            // When it's returned to the pool, we'll simply throw it away.
            buffer = new T[minimumLength];
        }

        return buffer;
    }

    public override void Return(T[] array, bool clearArray = false)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (array.Length == 0)
        {
            // Ignore empty arrays.  When a zero-length array is rented, we return a singleton
            // rather than actually taking a buffer out of the lowest bucket.
            return;
        }

        // Determine with what bucket this array length is associated
        int bucket = Utilities.SelectBucketIndex(array.Length);

        // If we can tell that the buffer was allocated, drop it. Otherwise, check if we have space in the pool
        bool haveBucket = bucket < _buckets.Length;
        if (haveBucket)
        {
            // Clear the array if the user requests
            if (clearArray)
            {
                Array.Clear(array);
            }

            // Return the buffer to its bucket.  In the future, we might consider having Return return false
            // instead of dropping a bucket, in which case we could try to return to a lower-sized bucket,
            // just as how in Rent we allow renting from a higher-sized bucket.
            _buckets[bucket].Return(array);
        }
    }

    /// <summary>Provides a thread-safe bucket containing buffers that can be Rent'd and Return'd.</summary>
    private readonly struct Bucket : IDisposable
    {
        private class ThreadBucket(int numberOfBuffers)
        {
            public readonly T[]?[] Buffers = new T[numberOfBuffers][];
            public int Index;
        }
        
        internal readonly int BufferLength;
        private readonly ThreadLocal<ThreadBucket> _buckets;

        /// <summary>
        /// Creates the pool with numberOfBuffers arrays where each buffer is of bufferLength length.
        /// </summary>
        internal Bucket(int bufferLength, int numberOfBuffers)
        {
            _buckets = new ThreadLocal<ThreadBucket>(() => new ThreadBucket(numberOfBuffers));
            BufferLength = bufferLength;
        }

        /// <summary>Takes an array from the bucket.  If the bucket is empty, returns null.</summary>
        internal T[]? Rent()
        {
            var bucket = _buckets.Value!;
            var buffers = bucket.Buffers;
            T[]? buffer = null;

            // Grab whatever is at the next available index and update the index.
            bool allocateBuffer = false;

            if (bucket.Index < buffers.Length)
            {
                buffer = buffers[bucket.Index];
                buffers[bucket.Index++] = null;
                allocateBuffer = buffer == null;
            }

            // While we were holding the lock, we grabbed whatever was at the next available index, if
            // there was one.  If we tried and if we got back null, that means we hadn't yet allocated
            // for that slot, in which case we should do so now.
            if (allocateBuffer)
            {
                buffer = new T[BufferLength];
            }

            return buffer;
        }

        /// <summary>
        /// Attempts to return the buffer to the bucket.  If successful, the buffer will be stored
        /// in the bucket and true will be returned; otherwise, the buffer won't be stored, and false
        /// will be returned.
        /// </summary>
        internal void Return(T[] array)
        {
            // Check to see if the buffer is the correct size for this bucket
            if (array.Length != BufferLength)
            {
                throw new ArgumentException("Buffer was not from this ArrayPool.", nameof(array));
            }

            // If there's room available in the bucket, put the buffer into the next available slot.
            // Otherwise, we just drop it.
            var bucket = _buckets.Value!;
            var returned = bucket.Index != 0;
            if (returned)
            {
                bucket.Buffers[--bucket.Index] = array;
            }
        }

        public void Dispose()
        {
            _buckets.Dispose();
        }
    }

    public void Dispose()
    {
        foreach (var bucket in _buckets)
        {
            bucket.Dispose();
        }
    }
}

file static class Utilities
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int SelectBucketIndex(int bufferSize)
    {
        // Buffers are bucketed so that a request between 2^(n-1) + 1 and 2^n is given a buffer of 2^n
        // Bucket index is log2(bufferSize - 1) with the exception that buffers between 1 and 16 bytes
        // are combined, and the index is slid down by 3 to compensate.
        // Zero is a valid bufferSize, and it is assigned the highest bucket index so that zero-length
        // buffers are not retained by the pool. The pool will return the Array.Empty singleton for these.
        return BitOperations.Log2((uint)bufferSize - 1 | 15) - 3;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetMaxSizeForBucket(int binIndex)
    {
        int maxSize = 16 << binIndex;
        Debug.Assert(maxSize >= 0);
        return maxSize;
    }

    internal enum MemoryPressure
    {
        Low,
        Medium,
        High
    }

    internal static MemoryPressure GetMemoryPressure()
    {
        const double HighPressureThreshold = .90;       // Percent of GC memory pressure threshold we consider "high"
        const double MediumPressureThreshold = .70;     // Percent of GC memory pressure threshold we consider "medium"

        GCMemoryInfo memoryInfo = GC.GetGCMemoryInfo();

        if (memoryInfo.MemoryLoadBytes >= memoryInfo.HighMemoryLoadThresholdBytes * HighPressureThreshold)
        {
            return MemoryPressure.High;
        }

        if (memoryInfo.MemoryLoadBytes >= memoryInfo.HighMemoryLoadThresholdBytes * MediumPressureThreshold)
        {
            return MemoryPressure.Medium;
        }

        return MemoryPressure.Low;
    }
}