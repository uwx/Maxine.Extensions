using System.Runtime.CompilerServices;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Maxine.Extensions.Test;

[TestClass]
public class UnitTest2
{
    public static IEnumerable<object[]> Data =>
    [
        [(1, 2)],
        [(1, 2, 3)],
        [(1, 2, 3, 4)],
        [(1, 2, 3, 4, 5)],
        [(1, 2, 3, 4, 5, 6)],
        [(1, 2, 3, 4, 5, 6, 7)],
        [(1, 2, 3, 4, 5, 6, 7, 8)],
        [(1, 2, 3, 4, 5, 6, 7, 8, 9)],
        [(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)],
        [(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)],
        [(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12)],
        [(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13)],
        [(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14)],
        [(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15)],
        [Tuple.Create(1, 2)],
        [Tuple.Create(1, 2, 3)],
        [Tuple.Create(1, 2, 3, 4)],
        [Tuple.Create(1, 2, 3, 4, 5)],
        [Tuple.Create(1, 2, 3, 4, 5, 6)],
        [Tuple.Create(1, 2, 3, 4, 5, 6, 7)],
        [Tuple.Create(1, 2, 3, 4, 5, 6, 7, 8)],
        [Tuple.Create(1, 2, 3, 4, 5, 6, 7, Tuple.Create(8, 9))],
        [Tuple.Create(1, 2, 3, 4, 5, 6, 7, Tuple.Create(8, 9, 10))],
        [Tuple.Create(1, 2, 3, 4, 5, 6, 7, Tuple.Create(8, 9, 10, 11))],
        [Tuple.Create(1, 2, 3, 4, 5, 6, 7, Tuple.Create(8, 9, 10, 11, 12))],
        [Tuple.Create(1, 2, 3, 4, 5, 6, 7, Tuple.Create(8, 9, 10, 11, 12, 13))],
        [Tuple.Create(1, 2, 3, 4, 5, 6, 7, Tuple.Create(8, 9, 10, 11, 12, 13, 14))],
        [Tuple.Create(1, 2, 3, 4, 5, 6, 7, Tuple.Create(8, 9, 10, 11, 12, 13, 14, 15))],
    ];

    [TestMethod]
    [DynamicData(nameof(Data))]
    public void TestMethod1(ITuple t)
    {
        var opts = new JsonSerializerOptions
        {
            Converters = { new TupleAsArrayFactory() }
        };

        var serialized = JsonSerializer.Serialize(t, t.GetType(), opts);
        Console.WriteLine(t.GetType() + ";" + serialized);
        
        Assert.StartsWith("[", serialized);
        Assert.EndsWith("]", serialized);
        var deserialized = JsonSerializer.Deserialize(serialized, t.GetType(), opts);

        Assert.AreEqual(t, deserialized);
    }
}