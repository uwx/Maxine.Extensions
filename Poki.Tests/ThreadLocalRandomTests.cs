using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Poki.Shared;
using Poki.Utilities;

namespace Poki.Tests;

[TestClass]
public class ThreadLocalRandomTests
{
    [TestMethod]
    public void TestUpperBoundExclusive()
    {
        for (var i = 0; i < 100000; i++)
        {
            for (var j = 0; j < 10; j++)
            {
                const int lowerBound = 1;
                const int upperBound = 100;

                var result = ThreadLocalRandom.NextTriangle(lowerBound, upperBound, j / 10D);
                Assert.AreNotEqual(result, upperBound, $"At iteration step {i}:{j}");
            }
        }
    }

    [TestMethod]
    public void TestUpperBoundExclusive2()
    {
        for (var i = 0; i < 1000000; i++)
        {
            var result = ThreadLocalRandom.NextTriangle(1);
            Assert.AreNotEqual(result, 1D, $"At iteration step {i}");
        }
    }

    [TestMethod]
    public void Histogram()
    {
        var dict = new Dictionary<int, int>();

        for (var i = 0; i < 5000; i++)
        {
            var result = ThreadLocalRandom.NextTriangle(1, 100, 0.2);
            if (!dict.TryAdd(result, 1))
            {
                dict[result]++;
            }
        }

        Console.WriteLine(string.Join("\n", dict.OrderBy(e => e.Key).Select(e => $"{e.Key:000} {new string('*', e.Value)}")));
    }

}