using System.Diagnostics;

namespace Maxine.Extensions.Test;

[TestClass]
public class Base64Tests
{
    [DataTestMethod]
    [DynamicData(nameof(GetData), DynamicDataSourceType.Method)]
    public void TestIntToB64AndBack(int value)
    {
        var displayString = ArbitraryBase.B93.ToBase(value);

        var converted = ArbitraryBase.B93.FromBase(displayString);
        
        Console.WriteLine("Input: " + value + ", DisplayString: " + displayString + ", Converted: " + converted);
        Console.WriteLine();
        Console.WriteLine("Input: " + Convert.ToString(value, 2).PadLeft(32, '0'));
        Console.WriteLine("Cvrtd: " + Convert.ToString(converted, 2).PadLeft(32, '0'));
        
        Assert.AreEqual(value, converted);
    }

    [TestMethod]
    public void TestPerfAndUniqueness()
    {
        var stopwatch = Stopwatch.StartNew();
        var set = new HashSet<string>();
        foreach (var objects in GetData())
        {
            int value = (int) objects[0];
            var displayString = HashUtilities.GetDisplayString(value);
            if (!set.Add(displayString))
            {
                throw new InvalidOperationException($"Value conflict: {value} ({displayString})");
            }
        }
        Console.WriteLine(stopwatch.ElapsedMilliseconds + "ms, " + (stopwatch.ElapsedMilliseconds / (double) set.Count) + "ms each");
    }

    public static IEnumerable<object[]> GetData()
    {
        for (var i = 0; i < 1000; i++)
        {
            yield return [i];
        }
        
        for (var i = int.MaxValue; i > int.MaxValue - 1000; i++)
        {
            yield return [i];
        }
        
        for (var i = int.MinValue; i <= int.MinValue + 1000; i++)
        {
            yield return [i];
        }

        var random = new Random(1394276316);
        
        yield return [1394276316];

        for (var i = 0; i < 1000; i++)
        {
            yield return [random.Next()];
        }
    }
}