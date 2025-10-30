/*
https://github.com/BlueRaja/Weighted-Item-Randomizer-for-C-Sharp

The MIT License (MIT)

Copyright (c) 2013 Daniel "BlueRaja" Pflughoeft

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFWARE.

 */

using System.Collections;
using JetBrains.Annotations;

namespace Poki.Shared;

public interface IWeightedRandomizer<out T> : IReadOnlyCollection<T> where T : notnull
{
    /// <summary>
    /// The total weight of all the items added to the randomizer.
    /// </summary>
    long TotalWeight { get; }

    /// <summary>
    /// Returns an item chosen randomly by weight (higher weights are more likely).
    /// </summary>
    T Next();
}

/// <summary>
/// A weighted randomizer implementation which uses Vose's alias method. It is very fast when doing many contiguous
/// calls to <see cref="Next"/>.
/// </summary>
/// <typeparam name="T">The type of the objects to choose at random</typeparam>
public class WeightedRandomizer<T> : IWeightedRandomizer<T> where T : notnull
{
    private readonly Random _random;
    private readonly Dictionary<T, int> _weights;
    private readonly List<ProbabilityBox> _probabilityBoxes;
    private long _heightPerBox;

    public int Count => _weights.Keys.Count;
    public IEnumerator<T> GetEnumerator() => _weights.Keys.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// The discrete boxes used to hold the keys/aliases in Vose's alias method.  Since we're using integers rather than
    /// floating-point probabilities, I've chosen the word "balls" for the value of the coin-flip used to determine
    /// whether to choose the key or the alias from the box.  If the number of balls chosen (taken from 1 to
    /// <see cref="_heightPerBox"/>) is &lt;= <see cref="NumBallsInBox"/>, we choose the Key; otherwise, we choose the
    /// Alias. Thus, there is exactly a NumBallsInBox/_heightPerBox probability of choosing the Key.
    /// </summary>
    /// <param name="Key">
    /// The parameter to be returned when the generated value is &lt;= <see cref="NumBallsInBox"/>
    /// </param>
    /// <param name="Alias">
    /// The parameter to be returned when the generated value is &gt; <see cref="NumBallsInBox"/>
    /// </param>
    /// <param name="NumBallsInBox">The probability of the generated value</param>
    private readonly record struct ProbabilityBox(
        T Key,
        T Alias,
        long NumBallsInBox
    );

    public WeightedRandomizer(IEnumerable<(T Item, int Weight)> entries, Random? random = null)
        : this(entries.Select(static e => KeyValuePair.Create(e.Item, e.Weight)), random)
    {
    }

    public WeightedRandomizer(IEnumerable<T> inputs, [RequireStaticDelegate] Func<T, int> getWeight)
        : this(inputs.Select(e => KeyValuePair.Create(e, getWeight(e))))
    {
    }

    public WeightedRandomizer(IEnumerable<KeyValuePair<T, int>> entries, Random? random = null)
    {
        _random = random ?? Random.Shared;

        _weights = new Dictionary<T, int>(entries);
        TotalWeight = _weights.Sum(static kvp => kvp.Value <= 0 ? throw new InvalidOperationException("Weight cannot be negative") : kvp.Value);

        _probabilityBoxes = [];
        _heightPerBox = 0;

        VerifyHaveItemsToChooseFrom();
        RebuildProbabilityList();

        _weights.TrimExcess();
        _probabilityBoxes.TrimExcess();
    }

    #region IWeightedRandomizer<T> stuff
    /// <inheritdoc/>
    public long TotalWeight { get; }

    /// <inheritdoc/>
    public T Next()
    {
        // Choose a random box, then flip a biased coin (represented by choosing a number of balls within the box)
        var randomIndex = _random.Next(_probabilityBoxes.Count);
        var randomNumBalls = _random.NextInt64(_heightPerBox) + 1;

        return randomNumBalls <= _probabilityBoxes[randomIndex].NumBallsInBox
            ? _probabilityBoxes[randomIndex].Key
            : _probabilityBoxes[randomIndex].Alias;
    }

    private readonly record struct KeyBallsPair(
        T Key,
        long NumBalls
    );

    private void RebuildProbabilityList()
    {
        var gcd = GreatestCommonDenominator(Count, TotalWeight);
        var weightMultiplier = Count / gcd;
        _heightPerBox = TotalWeight / gcd;

        var smallStack = new Stack<KeyBallsPair>();
        var largeStack = new Stack<KeyBallsPair>();

        DistributeKeysIntoStacks(weightMultiplier, largeStack, smallStack);
        CreateSplitProbabilityBoxes(largeStack, smallStack);
        AddRemainingProbabilityBoxes(smallStack);
    }

    /// <summary>
    /// Step one:  Load the small list with all items whose total weight is less than _heightPerBox (after scaling)
    /// the large list with those that are greater.
    /// </summary>
    private void DistributeKeysIntoStacks(long weightMultiplier, Stack<KeyBallsPair> largeStack, Stack<KeyBallsPair> smallStack)
    {
        _probabilityBoxes.Clear();
        foreach (var (item, weight) in _weights)
        {
            var newWeight = weight*weightMultiplier;
            if (newWeight > _heightPerBox)
            {
                largeStack.Push(new KeyBallsPair {Key = item, NumBalls = newWeight});
            }
            else
            {
                smallStack.Push(new KeyBallsPair {Key = item, NumBalls = newWeight});
            }
        }
    }

    /// <summary>
    /// Step two:  Pair up each item in the large/small lists and create a probability box for them
    /// </summary>
    private void CreateSplitProbabilityBoxes(Stack<KeyBallsPair> largeStack, Stack<KeyBallsPair> smallStack)
    {
        while (largeStack.Count != 0)
        {
            var largeItem = largeStack.Pop();
            var smallItem = smallStack.Pop();
            _probabilityBoxes.Add(new ProbabilityBox(smallItem.Key, largeItem.Key, smallItem.NumBalls));

            // Set the new weight for the largeList item, and move it to smallList if necessary
            var difference = _heightPerBox - smallItem.NumBalls;
            largeItem = largeItem with { NumBalls = largeItem.NumBalls - difference };
            if (largeItem.NumBalls > _heightPerBox)
            {
                largeStack.Push(largeItem);
            }
            else
            {
                smallStack.Push(largeItem);
            }
        }
    }

    /// <summary>
    /// Step three:  All the remining items in smallList necessarily have probability of 100%
    /// </summary>
    private void AddRemainingProbabilityBoxes(Stack<KeyBallsPair> smallStack)
    {
        while (smallStack.Count != 0)
        {
            var smallItem = smallStack.Pop();
            _probabilityBoxes.Add(new ProbabilityBox(smallItem.Key, smallItem.Key, _heightPerBox));
        }
    }

    private static long GreatestCommonDenominator(long a, long b)
    {
        // https://en.wikipedia.org/wiki/Euclidean_algorithm#Implementations

        while (b != 0)
        {
            var temp = a % b;
            a = b;
            b = temp;
        }
        
        return a;
    }

    /// <summary>
    /// Throws an exception if the Count or TotalWeight are 0, meaning that are no items to choose from.
    /// </summary>
    private void VerifyHaveItemsToChooseFrom()
    {
        if (Count <= 0)
            throw new InvalidOperationException("There are no items in the StaticWeightedRandomizer");
        if (TotalWeight <= 0)
            throw new InvalidOperationException("There are no items with positive weight in the StaticWeightedRandomizer");
    }
    #endregion
}