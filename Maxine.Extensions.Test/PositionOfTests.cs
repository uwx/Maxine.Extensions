using System.Buffers;
using Maxine.Extensions.Collections;

namespace Maxine.Extensions.Test;

[TestClass]
public class PositionOfTests
{
    [TestMethod]
    public void TestWorksForSingleSegment()
    {
        var bytes = new byte[1024];
        new Random(63).NextBytes(bytes);

        var search = bytes[800..900];
        
        var seq = new ReadOnlySequence<byte>(bytes);
        
        CollectionAssert.AreEqual(seq.Slice(seq.GetPosition(800)).ToArray(), seq.Slice(seq.PositionOf(search)!.Value).ToArray());
    }
    
    [TestMethod]
    public void TestWorksForMultiSegmentsSmall()
    {
        var bytes = new byte[1024*4];
        var rand = new Random(63);
        rand.NextBytes(bytes);

        var chunkedSequence = new ChunkedSequence<byte>();
        
        for (var i = 0; i < 4; i++)
        {
            chunkedSequence.Append(bytes.AsMemory(i * 1024, 1024));
        }

        ReadOnlySequence<byte> seq = chunkedSequence;
        
        var search = seq.Slice(1500, 256).ToArray();

        var expected = seq.GetPosition(1500);
        var actual = seq.PositionOf(search);
        CollectionAssert.AreEqual(seq.Slice(expected).ToArray(), seq.Slice(actual!.Value).ToArray(), $"{Convert.ToBase64String(seq.Slice(expected).ToArray())}\n\n\n{Convert.ToBase64String(seq.Slice(actual!.Value).ToArray())}");
    }

    [TestMethod]
    public void TestWorksForMultiSegmentsLarge()
    {
        var bytes = new byte[1024];

        var chunkedSequence = new ChunkedSequence<byte>();
        
        var rand = new Random(63);
        for (var i = 0; i < 128; i++)
        {
            rand.NextBytes(bytes);
            chunkedSequence.Append(bytes.AsMemory(0, bytes.Length / rand.Next(1, 12)));
        }

        ReadOnlySequence<byte> seq = chunkedSequence;
        
        var search = seq.Slice(1653, 2048).ToArray();

        var expected = seq.GetPosition(1653);
        var actual = seq.PositionOf(search);
        CollectionAssert.AreEqual(seq.Slice(expected).ToArray(), seq.Slice(actual!.Value).ToArray());
    }
}