// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;

namespace Maxine.Extensions.MemoryPools;

internal static class PinnedBlockMemoryPoolFactory
{
    public static MemoryPool<byte> Create()
    {
#if DEBUG
        return new DiagnosticMemoryPool(CreatePinnedBlockMemoryPool());
#else
        return CreatePinnedBlockMemoryPool();
#endif
    }

    public static MemoryPool<byte> CreatePinnedBlockMemoryPool()
    {
        return new PinnedBlockMemoryPool();
    }
}