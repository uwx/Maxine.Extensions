using System.Numerics;

namespace Maxine.Extensions.Test;

[TestClass]
public class VarIntTests
{
    private static IEnumerable<T> Data<T>() where T : unmanaged, INumber<T>, IMinMaxValue<T>
        => Range(T.MinValue, T.MaxValue, T.MaxValue / T.CreateTruncating(100000));

    private static IEnumerable<T> Range<T>(T start, T count, T? increment = null) where T : unmanaged, INumber<T>
    {
        increment ??= T.One;
        var end = start + count;
        for (var current = start; current < end; current += increment.Value)
        {
            yield return current;
        }
    }
    
    [TestMethod]
    public void TestRoundTrip()
    {
        Span<byte> buffer = stackalloc byte[10];
        foreach (var data in Data<int>())
        {
            var tryGetSignedBytes = VarInt.TryGetSignedBytes(data, buffer, out var bytesWritten);
            Assert.IsTrue(tryGetSignedBytes);
            var roundTrip = VarInt.ToSigned<int, ulong>(buffer[..bytesWritten]);
            
            Assert.AreEqual(data, roundTrip);
        }
    }
}
