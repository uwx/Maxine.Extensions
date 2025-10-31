using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;
using System.Buffers;

namespace Maxine.Extensions.Test;

[TestClass]
public class SequenceExtensionsAdditionalTests
{
    [TestMethod]
    public void PositionOf_SingleSegment_FindsMatch()
    {
        var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var sequence = new ReadOnlySequence<byte>(data);
        var search = new byte[] { 4, 5, 6 };

        var position = sequence.PositionOf(search);

        Assert.IsNotNull(position);
        Assert.AreEqual(3L, sequence.GetOffset(position.Value));
    }

    [TestMethod]
    public void PositionOf_SingleSegment_NoMatch_ReturnsNull()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        var sequence = new ReadOnlySequence<byte>(data);
        var search = new byte[] { 9, 10 };

        var position = sequence.PositionOf(search);

        Assert.IsNull(position);
    }

    [TestMethod]
    public void PositionOf_EmptySearch_ReturnsStart()
    {
        var data = new byte[] { 1, 2, 3 };
        var sequence = new ReadOnlySequence<byte>(data);
        var search = Array.Empty<byte>();

        var position = sequence.PositionOf(search);

        Assert.IsNotNull(position);
        Assert.AreEqual(0L, sequence.GetOffset(position.Value));
    }

    [TestMethod]
    public void PositionOf_SearchLongerThanSequence_ReturnsNull()
    {
        var data = new byte[] { 1, 2, 3 };
        var sequence = new ReadOnlySequence<byte>(data);
        var search = new byte[] { 1, 2, 3, 4, 5 };

        var position = sequence.PositionOf(search);

        Assert.IsNull(position);
    }

    [TestMethod]
    public void PositionOf_EmptySequence_ReturnsNull()
    {
        var sequence = ReadOnlySequence<byte>.Empty;
        var search = new byte[] { 1, 2 };

        var position = sequence.PositionOf(search);

        Assert.IsNull(position);
    }

    [TestMethod]
    public void PositionOf_SingleElement_FindsMatch()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        var sequence = new ReadOnlySequence<byte>(data);
        var search = new byte[] { 3 };

        var position = sequence.PositionOf(search);

        Assert.IsNotNull(position);
        Assert.AreEqual(2L, sequence.GetOffset(position.Value));
    }

    [TestMethod]
    public void PositionOf_MatchAtStart_FindsMatch()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        var sequence = new ReadOnlySequence<byte>(data);
        var search = new byte[] { 1, 2 };

        var position = sequence.PositionOf(search);

        Assert.IsNotNull(position);
        Assert.AreEqual(0L, sequence.GetOffset(position.Value));
    }

    [TestMethod]
    public void PositionOf_MatchAtEnd_FindsMatch()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        var sequence = new ReadOnlySequence<byte>(data);
        var search = new byte[] { 4, 5 };

        var position = sequence.PositionOf(search);

        Assert.IsNotNull(position);
        Assert.AreEqual(3L, sequence.GetOffset(position.Value));
    }

    [TestMethod]
    public void PositionOf_MultipleMatches_ReturnsFirst()
    {
        var data = new byte[] { 1, 2, 3, 2, 3, 4 };
        var sequence = new ReadOnlySequence<byte>(data);
        var search = new byte[] { 2, 3 };

        var position = sequence.PositionOf(search);

        Assert.IsNotNull(position);
        Assert.AreEqual(1L, sequence.GetOffset(position.Value));
    }

    [TestMethod]
    public void PositionOf_ExactMatch_FindsMatch()
    {
        var data = new byte[] { 1, 2, 3 };
        var sequence = new ReadOnlySequence<byte>(data);
        var search = new byte[] { 1, 2, 3 };

        var position = sequence.PositionOf(search);

        Assert.IsNotNull(position);
        Assert.AreEqual(0L, sequence.GetOffset(position.Value));
    }

    [TestMethod]
    public void PositionOf_PartialMatchNotFound_ReturnsNull()
    {
        var data = new byte[] { 1, 2, 4, 5 };
        var sequence = new ReadOnlySequence<byte>(data);
        var search = new byte[] { 2, 3 };

        var position = sequence.PositionOf(search);

        Assert.IsNull(position);
    }

    [TestMethod]
    public void PositionOf_WithIntegers_FindsMatch()
    {
        var data = new int[] { 10, 20, 30, 40, 50 };
        var sequence = new ReadOnlySequence<int>(data);
        var search = new int[] { 30, 40 };

        var position = sequence.PositionOf(search);

        Assert.IsNotNull(position);
        Assert.AreEqual(2L, sequence.GetOffset(position.Value));
    }

    [TestMethod]
    public void PositionOf_SingleSegment_LargeData_FindsMatch()
    {
        var data = new byte[1000];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (byte)(i % 256);
        }
        data[500] = 99;
        data[501] = 88;
        data[502] = 77;

        var sequence = new ReadOnlySequence<byte>(data);
        var search = new byte[] { 99, 88, 77 };

        var position = sequence.PositionOf(search);

        Assert.IsNotNull(position);
        Assert.AreEqual(500L, sequence.GetOffset(position.Value));
    }
}
