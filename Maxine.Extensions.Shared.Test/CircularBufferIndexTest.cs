namespace Maxine.Extensions.Shared.Test;

[TestClass]
public class CircularBufferIndexTest
{
    [TestMethod]
    [DataRow(10, 20)]
    [DataRow(5, 10)]
    [DataRow(20, 10)]
    [DynamicData(nameof(GetData))]
    public void TestExternalIndexConversionCorrect(int capacity, int added)
    {
        var buf = new ObservableCircularBuffer<int>(capacity);

        for (var i = 0; i <= added; i++)
        {
            buf.PushBack(i);
        }

        for (var i = 0; i < capacity; i++)
        {
            var internalIndex = buf.InternalIndex(i);
            var externalIndex = buf.ExternalIndex(internalIndex);
            
            Console.WriteLine(i + ", " + internalIndex + ", " + externalIndex);
            
            Assert.AreEqual(i, externalIndex);
        }
    }

    public static IEnumerable<object[]> GetData()
    {
        for (var i = 1; i < 100; i++)
        {
            for (var j = 0; j < 100; j++)
            {
                yield return new object[] { i, j };
            }
        }
    }
}