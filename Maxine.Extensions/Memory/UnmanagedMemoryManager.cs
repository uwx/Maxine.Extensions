using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maxine.Extensions;

// https://github.com/mgravell/Pipelines.Sockets.Unofficial/blob/6703efc310131037ee55b3136ff0d2d9e0df1e3f/src/Pipelines.Sockets.Unofficial/UnsafeMemory.cs

// The MIT License (MIT)
// 
// Copyright (c) 2018 Marc Gravell
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 
// ===============================================
// 
// Third Party Licenses:
// 
// The Redis project (http://redis.io/) is independent of this client library, and
// is licensed separately under the three clause BSD license. The full license
// information can be viewed here: http://redis.io/topics/license
// 
// This tool makes use of the "redis-doc" library from http://redis.io/documentation
// in the intellisense comments, which is licensed under the
// Creative Commons Attribution-ShareAlike 4.0 International license; full
// details are available here:
// https://github.com/antirez/redis-doc/blob/master/COPYRIGHT
// 
// The development solution uses the Redis-64 package from nuget
// (https://www.nuget.org/packages/Redis-64) by Microsoft Open Technologies, inc.
// This is licensed under the BSD license; full details are available here:
// https://github.com/MSOpenTech/redis/blob/2.6/license.txt
// This tool is not used in the release binaries.
// 
// The development solution uses the BookSleeve package from nuget
// (https://code.google.com/p/booksleeve/) by Marc Gravell. This is licensed
// under the Apache 2.0 license; full details are available here:
// http://www.apache.org/licenses/LICENSE-2.0
// This tool is not used in the release binaries.

/// <summary>
/// A MemoryManager over a raw pointer
/// </summary>
/// <remarks>The pointer is assumed to be fully unmanaged, or externally pinned - no attempt will be made to pin this data</remarks>
public sealed unsafe class UnmanagedMemoryManager<T> : MemoryManager<T>
{
    // where T : unmanaged
    // is intended - can't enforce due to a: convincing compiler, and
    // b: runtime (AOT) limitations

    private readonly void* _pointer;
    private readonly int _length;

    static UnmanagedMemoryManager()
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            throw new InvalidOperationException($"{nameof(UnmanagedMemoryManager<T>)} can only be used with blittable types, and {typeof(T)} is non blittable");
        }
    }

    /// <summary>
    /// Create a new UnmanagedMemoryManager instance at the given pointer and size
    /// </summary>
    /// <remarks>It is assumed that the span provided is already unmanaged or externally pinned</remarks>
    public UnmanagedMemoryManager(Span<T> span)
    {
        _pointer = Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
        _length = span.Length;
    }

    /// <summary>
    /// Create a new UnmanagedMemoryManager instance at the given pointer and size
    /// </summary>
    [CLSCompliant(false)]
#pragma warning disable CS8500 // T* - would prefer void*, but can't change API
    public UnmanagedMemoryManager(T* pointer, int length) : this((void*)pointer, length) {}
#pragma warning restore CS8500

    /// <summary>
    /// Create a new UnmanagedMemoryManager instance at the given pointer and size
    /// </summary>
    public UnmanagedMemoryManager(IntPtr pointer, int length) : this(pointer.ToPointer(), length) { }

    private UnmanagedMemoryManager(void* pointer, int length)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(length, 0, nameof(length));
        _pointer = pointer;
        _length = length;
    }

    /// <summary>
    /// Obtains a span that represents the region
    /// </summary>
    public override Span<T> GetSpan() => new(_pointer, _length);

    /// <summary>
    /// Provides access to a pointer that represents the data (note: no actual pin occurs)
    /// </summary>
    public override MemoryHandle Pin(int elementIndex = 0)
    {
        if (elementIndex < 0 || elementIndex >= _length)
            throw new ArgumentOutOfRangeException(nameof(elementIndex), elementIndex, $"{nameof(elementIndex)} < 0 || {nameof(elementIndex)} >= length");

        return new MemoryHandle(Unsafe.Add<T>(_pointer, elementIndex));
    }

    /// <summary>
    /// Has no effect
    /// </summary>
    public override void Unpin()
    {
    }

    /// <summary>
    /// Releases all resources associated with this object
    /// </summary>
    protected override void Dispose(bool disposing)
    {
    }
}