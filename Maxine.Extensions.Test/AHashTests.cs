using System.Buffers;
using System.Runtime.InteropServices;
using Maxine.Extensions.Collections;

namespace Maxine.Extensions.Test;

[TestClass]
public class AHashTests
{
    [DllImport(@"H:\GitHub\Rust\aHash\smhasher\ahash-cbindings\target\release\ahash_c.dll")]
    public static extern unsafe ulong ahash64(byte* buf, int len, ulong key1_1, ulong key1_2, ulong key2_1, ulong key2_2);

    [TestMethod]
    public unsafe void TestMethod1()
    {
        var r = new Random(6);

        for (var i = 0; i < 10000; i++)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(i);
            try
            {
                r.NextBytes(bytes);

                var key1_1 = (ulong)r.NextInt64();
                var key2_1 = (ulong)r.NextInt64();
                var key1_2 = (ulong)r.NextInt64();
                var key2_2 = (ulong)r.NextInt64();

                var key1 = (UInt128)key1_1 | ((UInt128)key1_2 << 8);
                var key2 = (UInt128)key2_1 | ((UInt128)key2_2 << 8);
                
                var hash = new AHash(key1, key2);
                hash.Write(bytes.AsSpan()[..i]);
                var result1 = hash.Finish();

                ulong result2;
                fixed (byte* buf = bytes)
                {
                    result2 = ahash64(buf, i, key1_1, key1_2, key2_1, key2_2);
                }
                
                Assert.AreEqual(result2, result1, $"Mismatch at size {i}");
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }
    }
}