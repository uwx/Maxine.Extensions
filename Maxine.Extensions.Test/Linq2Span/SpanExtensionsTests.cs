using Maxine.Extensions.Collections.SpanLinq;

namespace Maxine.Extensions.Test.Linq2Span;


[TestClass]
public class SpanExtensionsTests : TestBase
{
    protected static int[] _OneToTen = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
    protected static long[] _OneToTen_long = OneToTen.Select(x => (long)x).ToArray();
    protected static double[] _OneToTen_double = OneToTen.Select(x => (double)x).ToArray();
    protected static float[] _OneToTen_float = OneToTen.Select(x => (float)x).ToArray();
    protected static decimal[] _OneToTen_decimal = OneToTen.Select(x => (decimal)x).ToArray();

    protected static Span<int> OneToTen => _OneToTen;
    protected static Span<long> OneToTen_long => _OneToTen_long;
    protected static Span<double> OneToTen_double => _OneToTen_double;
    protected static Span<float> OneToTen_float => _OneToTen_float;
    protected static Span<decimal> OneToTen_decimal => _OneToTen_decimal;

    [TestMethod]
    public void Test_All()
    {
        Assert.IsTrue(OneToTen.All(x => x > 0));
        Assert.IsFalse(OneToTen.All(x => x > 1));
    }

    [TestMethod]
    public void Test_Aggregate()
    {
        Assert.AreEqual(55, OneToTen.Aggregate(0, (total, value) => total + value));
        Assert.AreEqual(3628800, OneToTen.Aggregate(1, (total, value) => total * value));
    }

    [TestMethod]
    public void Test_Any()
    {
        int[] none = [];
        Assert.IsTrue(OneToTen.Any());
        Assert.IsFalse(none.Any());

        Assert.IsTrue(OneToTen.Where(x => x > 0).Any());
        Assert.IsFalse(OneToTen.Where(x => x < 0).Any());
    }

    [TestMethod]
    public void Test_Any_Predicate()
    {
        Assert.IsTrue(OneToTen.Any(x => x > 0));
        Assert.IsFalse(OneToTen.Any(x => x < 0));
    }

    [TestMethod]
    public void Test_Append()
    {
        AssertAreEquivalent(
            OneToTen.Append(11).ToArray(),
            OneToTen.Append(11).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.Append(11).Select((x, i) => i).ToArray(),
            OneToTen.Append(11).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_Average()
    {
        Assert.AreEqual(5.5, OneToTen_double.Average());
    }

    [TestMethod]
    public void Test_Cast()
    {
        AssertAreEquivalent(
            OneToTen.Cast<int, object>().ToArray(),
            OneToTen.Cast<int, object>().ToArray()
            );
    }

    [TestMethod]
    public void Test_Chunk()
    {
        AssertAreEquivalent(
            OneToTen.ChunkToArray(3).ToArray(),
            OneToTen.ChunkToArray(3).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.ChunkToArray(3).Select((x, i) => i).ToArray(),
            OneToTen.ChunkToArray(3).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_Concat()
    {
        AssertAreEquivalent(
            OneToTen.Concat([..OneToTen]).ToArray(),
            OneToTen.Concat([..OneToTen]).ToArray()
            );

        // post concat indices
        AssertAreEquivalent(
            OneToTen.Concat([..OneToTen]).Select((x, i) => i).ToArray(),
            OneToTen.Concat([..OneToTen]).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_Contains()
    {
        Assert.IsTrue(OneToTen.Contains(10));
        Assert.IsTrue(OneToTen.Contains(1));
        Assert.IsFalse(OneToTen.Contains(0));
    }

    [TestMethod]
    public void Test_Count()
    {
        Assert.AreEqual(10, OneToTen.Count());
    }

    [TestMethod]
    public void Test_Count_Predicate()
    {
        Assert.AreEqual(5, OneToTen.Count(x => x > 5));
    }

    [TestMethod]
    public void Test_DefaultIfEmpty()
    {
        int[] none = [];
        int[] one = [1];
        AssertAreEquivalent(
            none.DefaultIfEmpty().ToArray(),
            none.DefaultIfEmpty().ToArray()
            );
        AssertAreEquivalent(
            one.DefaultIfEmpty().ToArray(),
            one.DefaultIfEmpty().ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            none.DefaultIfEmpty().Select((x, i) => i).ToArray(),
            none.DefaultIfEmpty().Select((x, i) => i).ToArray()
            );

        AssertAreEquivalent(
            one.DefaultIfEmpty().Select((x, i) => i).ToArray(),
            one.DefaultIfEmpty().Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_DefaultIfEmpty_Default()
    {
        int[] none = [];
        int[] one = [1];

        AssertAreEquivalent(
            none.DefaultIfEmpty(2).ToArray(),
            none.DefaultIfEmpty(2).ToArray()
            );
        AssertAreEquivalent(
            one.DefaultIfEmpty(2).ToArray(),
            one.DefaultIfEmpty(2).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            none.DefaultIfEmpty(2).Select((x, i) => i).ToArray(),
            none.DefaultIfEmpty(2).Select((x, i) => i).ToArray()
            );
        AssertAreEquivalent(
            one.DefaultIfEmpty(2).Select((x, i) => i).ToArray(),
            one.DefaultIfEmpty(2).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_Distinct()
    {
        int[] nondistinct = [1, 2, 3, 4, 5, 1, 2, 3, 4, 5];
        string[] nondistinct2 = ["one", "One", "Two", "TWO"];
        
        AssertAreEquivalent(
            nondistinct.Distinct().ToArray(),
            nondistinct.Distinct().ToArray()
            );
        AssertAreEquivalent(
            nondistinct2.Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
            nondistinct2.Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            nondistinct.Distinct().Select((x, i) => i).ToArray(),
            nondistinct.Distinct().Select((x, i) => i).ToArray()
            );
        AssertAreEquivalent(
            nondistinct2.Distinct(StringComparer.OrdinalIgnoreCase).Select((x, i) => i).ToArray(),
            nondistinct2.Distinct(StringComparer.OrdinalIgnoreCase).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_DistinctBy()
    {
        string[] nondistinct = ["one", "two", "three", "four", "five"];
        AssertAreEquivalent(
            nondistinct.DistinctBy(x => x.Length).ToArray(),
            nondistinct.DistinctBy(x => x.Length).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            nondistinct.DistinctBy(x => x.Length).Select((x, i) => i).ToArray(),
            nondistinct.DistinctBy(x => x.Length).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_ElementAt()
    {
        Assert.AreEqual(3, OneToTen.ElementAt(2));
        Assert.AreEqual(1, OneToTen.ElementAt(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => OneToTen.ElementAt(10));
    }

    [TestMethod]
    public void Test_ElementAt_Index()
    {
        Assert.AreEqual(3, OneToTen.ElementAt((Index)2));
        Assert.AreEqual(1, OneToTen.ElementAt((Index)0));
        Assert.Throws<ArgumentOutOfRangeException>(() => OneToTen.ElementAt((Index)10));
        Assert.AreEqual(10, OneToTen.ElementAt(^1));
        Assert.AreEqual(1, OneToTen.ElementAt(^10));
        Assert.Throws<ArgumentOutOfRangeException>(() => OneToTen.ElementAt(^0));
        Assert.Throws<ArgumentOutOfRangeException>(() => OneToTen.ElementAt(^11));
    }

    [TestMethod]
    public void Test_ElementAtOrDefault()
    {
        Assert.AreEqual(3, OneToTen.ElementAtOrDefault(2));
        Assert.AreEqual(1, OneToTen.ElementAtOrDefault(0));
        Assert.AreEqual(0, OneToTen.ElementAtOrDefault(10));
    }

    [TestMethod]
    public void Test_ElementAtOrDefault_Index()
    {
        Assert.AreEqual(3, OneToTen.ElementAtOrDefault((Index)2));
        Assert.AreEqual(1, OneToTen.ElementAtOrDefault((Index)0));
        Assert.AreEqual(0, OneToTen.ElementAtOrDefault((Index)10));
        Assert.AreEqual(10, OneToTen.ElementAtOrDefault(^1));
        Assert.AreEqual(1, OneToTen.ElementAtOrDefault(^10));
        Assert.AreEqual(0, OneToTen.ElementAtOrDefault(^0));
        Assert.AreEqual(0, OneToTen.ElementAtOrDefault(^11));
    }

    [TestMethod]
    public void Test_Except()
    {
        AssertAreEquivalent(
            OneToTen.Except([2, 4, 6]).ToArray(),
            OneToTen.Except([2, 4, 6]).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.Except([2, 4, 6]).Select((x, i) => i).ToArray(),
            OneToTen.Except([2, 4, 6]).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_ExceptBy()
    {
        AssertAreEquivalent(
            OneToTen.ExceptBy([2, 4, 6], x => x * 2).ToArray(),
            OneToTen.ExceptBy([2, 4, 6], x => x * 2).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.ExceptBy([2, 4, 6], x => x * 2).Select((x, i) => i).ToArray(),
            OneToTen.ExceptBy([2, 4, 6], x => x * 2).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_First()
    {
        int[] none = [];
        Assert.AreEqual(OneToTen.First(), 1);
        Assert.Throws<InvalidOperationException>(() => none.First());
    }

    [TestMethod]
    public void Test_First_Predicate()
    {
        Assert.AreEqual(OneToTen.First(x => x > 5), 6);
        Assert.Throws<InvalidOperationException>(() => OneToTen.First(x => x > 10));
    }

    [TestMethod]
    public void Test_FirstOrDefault()
    {
        int[] none = [];
        Assert.AreEqual(OneToTen.FirstOrDefault(), 1);
        Assert.AreEqual(none.FirstOrDefault(), 0);
    }

    [TestMethod]
    public void Test_FirstOrDefault_Predicate()
    {
        Assert.AreEqual(OneToTen.FirstOrDefault(x => x > 5), 6);
        Assert.AreEqual(OneToTen.FirstOrDefault(x => x > 10), 0);
    }

    [TestMethod]
    public void Test_foreach()
    {
        var items = new List<int>();

        foreach (var x in OneToTen.Where(x => x < 5))
        {
            items.Add(x);
        }

        AssertAreEquivalent(
            OneToTen.Where(x => x < 5).ToArray(),
            items.ToArray()
            );
    }

    [TestMethod]
    public void Test_ForEach()
    {
        var items = new List<int>();

        OneToTen.ForEach(x =>
        {
            items.Add(x);
            if (items.Count > 10)
                throw new Exception("Runaway query");
        });

        AssertAreEquivalent(
            OneToTen.ToArray(),
            items.ToArray()
            );
    }

    [TestMethod]
    public void Test_ForEach_Index()
    {
        var items = new List<int>();

        OneToTen.ForEach((x, i) =>
        {
            items.Add(x);
            if (items.Count > 10)
                throw new Exception("Runaway query");
        });

        AssertAreEquivalent(
            OneToTen.ToArray(),
            items.ToArray()
            );
    }

    [TestMethod]
    public void Test_GroupBy()
    {
        AssertAreEquivalent(
            OneToTen.GroupBy(x => x / 2).ToArray(),
            OneToTen.GroupBy(x => x / 2).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.GroupBy(x => x / 2).Select((x, i) => i).ToArray(),
            OneToTen.GroupBy(x => x / 2).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_GroupJoin()
    {
        AssertAreEquivalent(
            OneToTen.GroupJoin(OneToTen.ToArray(), x => x, y => y, (x, ygroup) => x + Enumerable.Sum(ygroup)).ToArray(),
            OneToTen.GroupJoin(OneToTen.ToArray(), x => x, y => y, (x, ygroup) => x + Enumerable.Sum(ygroup)).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.GroupJoin(OneToTen.ToArray(), x => x, y => y, (x, ygroup) => x + Enumerable.Sum(ygroup)).Select((x, i) => i).ToArray(),
            OneToTen.GroupJoin(OneToTen.ToArray(), x => x, y => y, (x, ygroup) => x + Enumerable.Sum(ygroup)).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_Intersect()
    {
        AssertAreEquivalent(
            OneToTen.Intersect([2, 4, 6]).ToArray(),
            OneToTen.Intersect([2, 4, 6]).ToArray()
            );

        // results are distinct
        var duplicates = OneToTen.Concat(OneToTen.ToArray()).ToArray();
        AssertAreEquivalent(
            duplicates.Intersect([2, 4, 6]).ToArray(),
            duplicates.Intersect([2, 4, 6]).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.Intersect([2, 4, 6]).Select((x, i) => i).ToArray(),
            OneToTen.Intersect([2, 4, 6]).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_IntersectBy()
    {
        AssertAreEquivalent(
            OneToTen.IntersectBy([2, 4, 6], x => x * 2).ToArray(),
            OneToTen.IntersectBy([2, 4, 6], x => x * 2).ToArray()
            );

        // results are distinct
        var duplicates = OneToTen.Concat(OneToTen.ToArray()).ToArray();
        AssertAreEquivalent(
            duplicates.IntersectBy([2, 4, 6], x => x * 2).ToArray(),
            duplicates.IntersectBy([2, 4, 6], x => x * 2).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.IntersectBy([2, 4, 6], x => x * 2).Select((x, i) => i).ToArray(),
            OneToTen.IntersectBy([2, 4, 6], x => x * 2).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_Join()
    {
        AssertAreEquivalent(
            OneToTen.Join(OneToTen.ToArray(), x => x, y => y, (x, y) => x = y).ToArray(),
            OneToTen.Join(OneToTen.ToArray(), x => x, y => y, (x, y) => x = y).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.Join(OneToTen.ToArray(), x => x, y => y, (x, y) => x = y).Select((x, i) => i).ToArray(),
            OneToTen.Join(OneToTen.ToArray(), x => x, y => y, (x, y) => x = y).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_Last()
    {
        int[] none = [];
        Assert.AreEqual(OneToTen.Last(), 10);
        Assert.Throws<InvalidOperationException>(() => none.Last());
    }

    [TestMethod]
    public void Test_LastOrDefault()
    {
        int[] none = [];
        Assert.AreEqual(OneToTen.LastOrDefault(), 10);
        Assert.AreEqual(none.LastOrDefault(), 0);
    }

    [TestMethod]
    public void Test_Max()
    {
        Assert.AreEqual(10, OneToTen.Max());
    }

    [TestMethod]
    public void Test_Max_Selector()
    {
        Assert.AreEqual(20, OneToTen.Max(x => x * 2));
    }

    [TestMethod]
    public void Test_MaxBy()
    {
        Assert.AreEqual(1, OneToTen.MaxBy(x => -x));
    }

    [TestMethod]
    public void Test_Min()
    {
        Assert.AreEqual(1, OneToTen.Min());
    }

    [TestMethod]
    public void Test_Min_Selector()
    {
        Assert.AreEqual(2, OneToTen.Min(x => x * 2));
    }

    [TestMethod]
    public void Test_MinBy()
    {
        Assert.AreEqual(10, OneToTen.MinBy(x => -x));
    }

    [TestMethod]
    public void Test_OfType()
    {
        object[] values = [1, 1.0, 1m, "one"];

        AssertAreEquivalent(
            values.OfType<int>().ToArray(),
            values.OfType<int>().ToArray()
            );

        AssertAreEquivalent(
            values.OfType<double>().ToArray(),
            values.OfType<double>().ToArray()
            );

        AssertAreEquivalent(
            values.OfType<decimal>().ToArray(),
            values.OfType<decimal>().ToArray()
            );

        AssertAreEquivalent(
            values.OfType<string>().ToArray(),
            values.OfType<string>().ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            values.OfType<double>().Select((x, i) => i).ToArray(),
            values.OfType<double>().Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_Order()
    {
        AssertAreEquivalent(
            OneToTen.Order().ToArray(),
            OneToTen.Order().ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.Order().Select((x, i) => i).ToArray(),
            OneToTen.Order().Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_OrderDescending()
    {
        AssertAreEquivalent(
            OneToTen.OrderDescending().ToArray(),
            OneToTen.OrderDescending().ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.OrderDescending().Select((x, i) => i).ToArray(),
            OneToTen.OrderDescending().Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_OrderBy()
    {
        AssertAreEquivalent(
            OneToTen.OrderBy(x => -x).ToArray(),
            OneToTen.OrderBy(x => -x).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.OrderBy(x => -x).Select((x, i) => i).ToArray(),
            OneToTen.OrderBy(x => -x).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_OrderByDescending()
    {
        AssertAreEquivalent(
            OneToTen.OrderBy(x => x).ToArray(),
            OneToTen.OrderBy(x => x).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.OrderBy(x => x).Select((x, i) => i).ToArray(),
            OneToTen.OrderBy(x => x).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_Prepend()
    {
        AssertAreEquivalent(
            OneToTen.Prepend(0).ToArray(),
            OneToTen.Prepend(0).ToArray()
            );

        AssertAreEquivalent(
            OneToTen.Prepend(0).Prepend(-1).ToArray(),
            OneToTen.Prepend(0).Prepend(-1).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.Prepend(0).Select((x, i) => i).ToArray(),
            OneToTen.Prepend(0).Select((x, i) => i).ToArray()
            );

        AssertAreEquivalent(
            OneToTen.Prepend(0).Prepend(-1).Select((x, i) => i).ToArray(),
            OneToTen.Prepend(0).Prepend(-1).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_Reverse()
    {
        AssertAreEquivalent(
            OneToTen.ToReversed().ToArray(),
            OneToTen.ToReversed().ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.ToReversed().Select((x, i) => i).ToArray(),
            OneToTen.ToReversed().Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_Select()
    {
        AssertAreEquivalent(
            OneToTen.Select(x => x * 2).ToArray(),
            OneToTen.Select(x => x * 2).ToArray()
            );
    }

    [TestMethod]
    public void Test_Select_Index()
    {
        AssertAreEquivalent(
            Enumerable.Range(0, 10).ToArray(),
            OneToTen.Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_SelectMany()
    {
        AssertAreEquivalent(
            OneToTen.SelectMany(x => new[] { x, x }).ToArray(),
            OneToTen.SelectMany(x => new[] { x, x }).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.SelectMany(x => new[] { x, x }).Select((x, i) => i).ToArray(),
            OneToTen.SelectMany(x => new[] { x, x }).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_SelectMany_Index()
    {
        AssertAreEquivalent(
            OneToTen.SelectMany((x, i) => new[] { x, i }).ToArray(),
            OneToTen.SelectMany((x, i) => new[] { x, i }).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.SelectMany((x, i) => new[] { x, i }).Select((x, i) => i).ToArray(),
            OneToTen.SelectMany((x, i) => new[] { x, i }).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_SelectMany_Result()
    {
        AssertAreEquivalent(
            OneToTen.SelectMany(x => new[] { x, x }, (x, y) => x + y).ToArray(),
            OneToTen.SelectMany(x => new[] { x, x }, (x, y) => x + y).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.SelectMany(x => new[] { x, x }, (x, y) => x + y).Select((x, i) => i).ToArray(),
            OneToTen.SelectMany(x => new[] { x, x }, (x, y) => x + y).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_SelectMany_Index_Result()
    {
        AssertAreEquivalent(
            OneToTen.SelectMany((x, i) => new[] { x, i }, (x, y) => x + y).ToArray(),
            OneToTen.SelectMany((x, i) => new[] { x, i }, (x, y) => x + y).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.SelectMany((x, i) => new[] { x, i }, (x, y) => x + y).Select((x, i) => i).ToArray(),
            OneToTen.SelectMany((x, i) => new[] { x, i }, (x, y) => x + y).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_Single()
    {
        int[] one = [1];
        int[] none = [];
        int[] two = [1, 2];
        Assert.AreEqual(one.Single(), 1);
        Assert.Throws<InvalidOperationException>(() => none.Single());
        Assert.Throws<InvalidOperationException>(() => two.Single());
    }

    [TestMethod]
    public void Test_Single_Predicate()
    {
        Assert.AreEqual(OneToTen.Single(x => x > 9), 10);
        Assert.Throws<InvalidOperationException>(() => OneToTen.Single(x => x > 10));
        Assert.Throws<InvalidOperationException>(() => OneToTen.Single(x => x > 8));
    }

    [TestMethod]
    public void Test_SingleOrDefault()
    {
        int[] one = [1];
        int[] none = [];
        int[] two = [1, 2];
        Assert.AreEqual(one.SingleOrDefault(), 1);
        Assert.AreEqual(none.SingleOrDefault(), 0);
        Assert.Throws<InvalidOperationException>(() => two.SingleOrDefault());
    }

    [TestMethod]
    public void Test_SingleOrDefault_Predicate()
    {
        Assert.AreEqual(OneToTen.SingleOrDefault(x => x > 9), 10);
        Assert.AreEqual(OneToTen.SingleOrDefault(x => x > 10), 0);
        Assert.Throws<InvalidOperationException>(() => OneToTen.SingleOrDefault(x => x > 8));
    }

    [TestMethod]
    public void Test_Skip()
    {
        AssertAreEquivalent(
            OneToTen.Skip(5).ToArray(),
            OneToTen.Skip(5).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.Skip(5).Select((x, i) => i).ToArray(),
            OneToTen.Skip(5).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_SkipLast()
    {
        AssertAreEquivalent(
            OneToTen.SkipLast(5).ToArray(),
            OneToTen.SkipLast(5).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.SkipLast(5).Select((x, i) => i).ToArray(),
            OneToTen.SkipLast(5).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_SkipWhile()
    {
        AssertAreEquivalent(
            OneToTen.SkipWhile(x => x < 6).ToArray(),
            OneToTen.SkipWhile(x => x < 6).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.SkipWhile(x => x < 6).Select((x, i) => i).ToArray(),
            OneToTen.SkipWhile(x => x < 6).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_SkipWhile_Index()
    {
        AssertAreEquivalent(
            OneToTen.SkipWhile((x, i) => i < 6).ToArray(),
            OneToTen.SkipWhile((x, i) => i < 6).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.SkipWhile((x, i) => i < 6).Select((x, i) => i).ToArray(),
            OneToTen.SkipWhile((x, i) => i < 6).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_Sum()
    {
        Assert.AreEqual(55, OneToTen.Sum());
    }

    [TestMethod]
    public void Test_Sum_Selector()
    {
        Assert.AreEqual(110, OneToTen.Sum(x => x * 2));
    }

    [TestMethod]
    public void Test_Take()
    {
        AssertAreEquivalent(
            OneToTen.Take(5).ToArray(),
            OneToTen.Take(5).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.Take(5).Select((x, i) => i).ToArray(),
            OneToTen.Take(5).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_TakeLast()
    {
        AssertAreEquivalent(
            OneToTen.TakeLast(5).ToArray(),
            OneToTen.TakeLast(5).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.TakeLast(5).Select((x, i) => i).ToArray(),
            OneToTen.TakeLast(5).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_TakeWhile()
    {
        AssertAreEquivalent(
            OneToTen.TakeWhile(x => x < 6).ToArray(),
            OneToTen.TakeWhile(x => x < 6).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.TakeWhile(x => x < 6).Select((x, i) => i).ToArray(),
            OneToTen.TakeWhile(x => x < 6).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_TakeWhile_Index()
    {
        AssertAreEquivalent(
            OneToTen.TakeWhile((x, i) => i < 6).ToArray(),
            OneToTen.TakeWhile((x, i) => i < 6).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.TakeWhile((x, i) => i < 6).Select((x, i) => i).ToArray(),
            OneToTen.TakeWhile((x, i) => i < 6).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_ThenBy()
    {
        AssertAreEquivalent(
            OneToTen.OrderBy(x => x & 2).ThenBy(x => x & 1).ToArray(),
            OneToTen.OrderBy(x => x & 2).ThenBy(x => x & 1).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.OrderBy(x => x & 2).ThenBy(x => x & 1).Select((x, i) => i).ToArray(),
            OneToTen.OrderBy(x => x & 2).ThenBy(x => x & 1).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_ThenByDescending()
    {
        AssertAreEquivalent(
            OneToTen.OrderBy(x => x & 2).ThenByDescending(x => x & 1).ToArray(),
            OneToTen.OrderBy(x => x & 2).ThenByDescending(x => x & 1).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.OrderBy(x => x & 2).ThenByDescending(x => x & 1).Select((x, i) => i).ToArray(),
            OneToTen.OrderBy(x => x & 2).ThenByDescending(x => x & 1).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_Union()
    {
        AssertAreEquivalent(
            OneToTen.Union([2, 11, 15]).ToArray(),
            OneToTen.Union([2, 11, 15]).ToArray()
            );

        // results are distinct
        var duplicates = OneToTen.Concat(OneToTen.ToArray()).ToArray();
        AssertAreEquivalent(
            duplicates.Union([2, 11, 15]).ToArray(),
            duplicates.Union([2, 11, 15]).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.Union([2, 11, 15]).Select((x, i) => i).ToArray(),
            OneToTen.Union([2, 11, 15]).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_UnionBy()
    {
        AssertAreEquivalent(
            OneToTen.UnionBy([2, 11, 15], x => x % 2).ToArray(),
            OneToTen.UnionBy([2, 11, 15], x => x % 2).ToArray()
            );

        // results are distinct
        var duplicates = OneToTen.Concat(OneToTen.ToArray()).ToArray();
        AssertAreEquivalent(
            duplicates.UnionBy([2, 11, 15], x => x * 2).ToArray(),
            duplicates.UnionBy([2, 11, 15], x => x * 2).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.UnionBy([2, 11, 15], x => x % 2).Select((x, i) => i).ToArray(),
            OneToTen.UnionBy([2, 11, 15], x => x % 2).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_Where()
    {
        AssertAreEquivalent(
            OneToTen.Where(x => x > 5).ToArray(),
            OneToTen.Where(x => x > 5).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.Where(x => x > 5).Select((x, i) => i).ToArray(),
            OneToTen.Where(x => x > 5).Select((x, i) => i).ToArray()
            );
    }

    [TestMethod]
    public void Test_Where_Index()
    {
        AssertAreEquivalent(
            OneToTen.Where((x, i) => i > 5).ToArray(),
            OneToTen.Where((x, i) => i > 5).ToArray()
            );

        // post op indices
        AssertAreEquivalent(
            OneToTen.Where((x, i) => i > 5).ToArray(),
            OneToTen.Where((x, i) => i > 5).ToArray()
            );
    }
}