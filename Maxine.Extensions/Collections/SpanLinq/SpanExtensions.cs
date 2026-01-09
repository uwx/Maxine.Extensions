using System.Numerics;

namespace Maxine.Extensions.Collections.SpanLinq;

public static class SpanExtensions
{
    extension<TElement>(Span<TElement> span)
    {
        /// <summary>
        /// Executes the query and returns the results as a <see cref="List{T}"/>.
        /// </summary>
        public List<TElement> ToList()
        {
            var list = new List<TElement>(span.Length);
            list.AddRange(span);
            return list;
        }

        /// <summary>
        /// Executes the query and returns the results a <see cref="HashSet{T}"/>.
        /// </summary>
        public HashSet<TElement> ToHashSet(
            IEqualityComparer<TElement>? comparer = null)
        {
            var set = new HashSet<TElement>(comparer);
            foreach (var t in span)
            {
                set.Add(t);
            }
            return set;
        }

        /// <summary>
        /// Executes the query and returns the results a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        public Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            Func<TElement, TKey> keySelector,
            Func<TElement, TValue> valueSelector,
            IEqualityComparer<TKey>? comparer = null)
            where TKey : notnull
        {
            var dictionary = new Dictionary<TKey, TValue>(comparer ?? EqualityComparer<TKey>.Default);

            foreach (var element in span)
            {
                dictionary.Add(keySelector(element), valueSelector(element));
            }

            return dictionary;
        }
        
        /// <summary>
        /// Executes the query and returns the results a <see cref="Dictionary{TKey, TElement}"/>.
        /// </summary>
        public Dictionary<TKey, TElement> ToDictionary<TKey>(
            Func<TElement, TKey> keySelector,
            IEqualityComparer<TKey>? comparer = null)
            where TKey : notnull
        {
            return span.ToDictionary(keySelector, x => x, comparer);
        }

        /// <summary>
        /// Returns a query that produces the results of the original query with an additional element added to the end.
        /// </summary>
        public SpanQuery<TElement, TElement, AppendEnumerator<TElement, TElement, SpanEnumerator<TElement>>> Append(
            TElement element)
        {
            return span.AsSpanQuery().Append(element);
        }

        /// <summary>
        /// Returns a query that casts each element of the original query to the specified type.
        /// </summary>
        public SpanQuery<TElement, TResult, CastEnumerator<TElement, TElement, SpanEnumerator<TElement>, TResult>> Cast<TResult>()
        {
            return span.AsSpanQuery().Cast<TResult>();
        }

        /// <summary>
        /// Returns a query that breaks the elements of the original query into chunks.
        /// </summary>
        public SpanQuery<TElement, TElement[], ChunkToArrayEnumerator<TElement, TElement, SpanEnumerator<TElement>>> ChunkToArray(
            int size)
        {
            return span.AsSpanQuery().Chunk(size);
        }
        
        public ChunkToSpanEnumerator<TElement> ChunkToSpan(int size)
        {
            return new ChunkToSpanEnumerator<TElement>(size);
        }
        
        public SpanQuery<TElement, Range, ChunkToRangeEnumerator<TElement>> ChunkToRange(int size)
        {
            return span.AsSpanQuery().With<Range, ChunkToRangeEnumerator<TElement>>(
                new ChunkToRangeEnumerator<TElement>(size, span)
            );
        }

        /// <summary>
        /// Returns a query that produces the concatenation of the results of the original query 
        /// with the specified list of elements.
        /// </summary>
        public SpanQuery<TElement, TElement, ConcatEnumerator<TElement, TElement, SpanEnumerator<TElement>>> Concat(
            IEnumerable<TElement> elements)
        {
            return span.AsSpanQuery().Concat(elements);
        }

        /// <summary>
        /// Returns a new query the produces the results of the original query if the results are not empty,
        /// otherwise produces a single result of the default value.
        /// </summary>
        public SpanQuery<TElement, TElement, DefaultIfEmptyEnumerator<TElement, TElement, SpanEnumerator<TElement>>> DefaultIfEmpty(
            TElement defaultValue)
        {
            return span.AsSpanQuery().DefaultIfEmpty(defaultValue);
        }

        /// <summary>
        /// Returns a new query the produces the results of the original query if the results are not empty,
        /// otherwise produces a single result of the default value for the result type.
        /// </summary>
        public SpanQuery<TElement, TElement, DefaultIfEmptyEnumerator<TElement, TElement, SpanEnumerator<TElement>>> DefaultIfEmpty()
        {
            return span.AsSpanQuery().DefaultIfEmpty();
        }

        /// <summary>
        /// Returns a query that produces the distinct elements of the original query.
        /// </summary>
        public SpanQuery<TElement, TElement,
            DistinctByEnumerator<TElement, TElement, SpanEnumerator<TElement>, TElement>> Distinct(
            IEqualityComparer<TElement>? keyComparer = null)
        {
            return span.AsSpanQuery().DistinctBy(x => x, keyComparer);
        }

        /// <summary>
        /// Returns a query that procues the distinct elements of the original query as determined by the specified key selector.
        /// </summary>
        public SpanQuery<TElement, TElement, DistinctByEnumerator<TElement, TElement, SpanEnumerator<TElement>, TKey>>
            DistinctBy<TKey>(
                Func<TElement, TKey> keySelector,
                IEqualityComparer<TKey>? keyComparer = null)
        {
            return span.AsSpanQuery().DistinctBy(keySelector, keyComparer);
        }

        /// <summary>
        /// Returns a query that produces the elements of the original query, except for those elements in the specified list.
        /// </summary>
        public SpanQuery<TElement, TElement, ExceptByEnumerator<TElement, TElement, SpanEnumerator<TElement>, TElement>> Except(
            IEnumerable<TElement> elements,
            IEqualityComparer<TElement>? comparer = null)
        {
            return span.AsSpanQuery().Except(elements, comparer);
        }

        /// <summary>
        /// Returns a query that produces the elements of the original query, except for those elements with keys in the specified list.
        /// </summary>
        public SpanQuery<TElement, TElement, ExceptByEnumerator<TElement, TElement, SpanEnumerator<TElement>, TKey>> ExceptBy<TKey>(
            IEnumerable<TKey> keys,
            Func<TElement, TKey> keySelector,
            IEqualityComparer<TKey>? keyComparer = null)
        {
            return span.AsSpanQuery().ExceptBy(keys, keySelector, keyComparer);
        }

        public SpanQuery<TElement, IGrouping<TKey, TElement>, GroupByEnumerator<TElement, TElement, SpanEnumerator<TElement>, TKey>>
            GroupBy<TKey>(
                Func<TElement, TKey> keySelector,
                IEqualityComparer<TKey>? keyComparer = null)
        {
            return span.AsSpanQuery().GroupBy(keySelector, keyComparer);
        }

        public SpanQuery<TElement, TResult, GroupJoinEnumerator<TElement, TElement, SpanEnumerator<TElement>, TInner, TKey, TResult>>
            GroupJoin<TInner, TKey, TResult>(
                IEnumerable<TInner> inner,
                Func<TElement, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                Func<TElement, IEnumerable<TInner>, TResult> resultSelector,
                IEqualityComparer<TKey>? comparer = null)
        {
            return span.AsSpanQuery().GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
        }

        /// <summary>
        /// Returns a query that produces the elements of the original query that have keys also in the specified list.
        /// </summary>
        public SpanQuery<TElement, TElement, IntersectByEnumerator<TElement, TElement, SpanEnumerator<TElement>, TKey>> IntersectBy<TKey>(
            IEnumerable<TKey> keys,
            Func<TElement, TKey> keySelector,
            IEqualityComparer<TKey>? keyComparer = null)
        {
            return span.AsSpanQuery().IntersectBy(keys, keySelector, keyComparer);
        }

        /// <summary>
        /// Returns a query that produces the elements of the original query that is also in the specified list.
        /// </summary>
        public SpanQuery<TElement, TElement, IntersectByEnumerator<TElement, TElement, SpanEnumerator<TElement>, TElement>> Intersect(
            IEnumerable<TElement> elements,
            IEqualityComparer<TElement>? comparer = null)
        {
            return span.AsSpanQuery().Intersect(elements, comparer);
        }
        
        /// <summary>
        /// Produces the join of the result of the original query with the specified list of elements.
        /// </summary>
        public SpanQuery<TElement, TResult, JoinEnumerator<TElement, TElement, SpanEnumerator<TElement>, TInner, TKey, TResult>> Join<TInner, TKey, TResult>(
            IEnumerable<TInner> inner,
            Func<TElement, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TElement, TInner, TResult> resultSelector,
            IEqualityComparer<TKey>? keyComparer = null)
        {
            return span.AsSpanQuery().Join(inner, outerKeySelector, innerKeySelector, resultSelector, keyComparer);
        }

        /// <summary>
        /// Returns the elements of the original query that are of the specified type.
        /// </summary>
        public SpanQuery<TElement, TResult, OfTypeEnumerator<TElement, TElement, SpanEnumerator<TElement>, TResult>> OfType<TResult>()
        {
            return span.AsSpanQuery().OfType<TResult>();
        }

        /// <summary>
        /// Returns a query that produces the elements of the original query in order.
        /// </summary>
        public SpanQuery<TElement, TElement, OrderByEnumerator<TElement, TElement, SpanEnumerator<TElement>>> Order(
            IComparer<TElement>? comparer = null)
        {
            return span.AsSpanQuery().Order(comparer);
        }

        /// <summary>
        /// Returns a query that produces the elements of the original query in order of the specified key.
        /// </summary>
        public SpanQuery<TElement, TElement, OrderByEnumerator<TElement, TElement, SpanEnumerator<TElement>>> OrderBy<TKey>(
            Func<TElement, TKey> keySelector,
            IComparer<TKey>? comparer = null)
        {
            return span.AsSpanQuery().OrderBy(keySelector, comparer);
        }

        /// <summary>
        /// Returns a query that produces the elements of the original query in descending order of the specified key.
        /// </summary>
        public SpanQuery<TElement, TElement, OrderByEnumerator<TElement, TElement, SpanEnumerator<TElement>>> OrderByDescending<TKey>(
            Func<TElement, TKey> keySelector,
            IComparer<TKey>? comparer = null)
        {
            return span.AsSpanQuery().OrderByDescending(keySelector, comparer);
        }

        /// <summary>
        /// Returns a query that produces the elements of the original query in descending order.
        /// </summary>
        public SpanQuery<TElement, TElement, OrderByEnumerator<TElement, TElement, SpanEnumerator<TElement>>> OrderDescending(
            IComparer<TElement>? comparer = null)
        {
            return span.AsSpanQuery().OrderDescending(comparer);
        }

        /// <summary>
        /// Returns a query that produces the prepended element followed by the elements of the result of the original query.
        /// </summary>
        public SpanQuery<TElement, TElement, PrependSpanEnumerator<TElement, TElement, SpanEnumerator<TElement>>> Prepend(
            TElement element)
        {
            return span.AsSpanQuery().Prepend(element);
        }

        /// <summary>
        /// Returns a query that produces the result of the original query in reverse.
        /// </summary>
        public SpanQuery<TElement, TElement, ReverseEnumerator<TElement, TElement, SpanEnumerator<TElement>>> ToReversed()
        {
            return span.AsSpanQuery().Reverse();
        }

        /// <summary>
        /// Returns a query that produces the selected or mapped values of the original query results.
        /// </summary>
        public SpanQuery<TElement, TResult, SelectEnumerator<TElement, TElement, SpanEnumerator<TElement>, TResult>> Select<TResult>(
            Func<TElement, int, TResult> selector)
        {
            return span.AsSpanQuery().Select(selector);
        }

        /// <summary>
        /// Returns a query that produces the selected or mapped values of the original query results.
        /// </summary>
        public SpanQuery<TElement, TResult, SelectEnumerator<TElement, TElement, SpanEnumerator<TElement>, TResult>> Select<TResult>(
            Func<TElement, TResult> selector)
        {
            return span.AsSpanQuery().Select(selector);
        }

        /// <summary>
        /// Returns a query that produces the selected result of the original query results, flattend into a single sequence.
        /// </summary>
        public SpanQuery<TElement, TResult, SelectManyEnumerator<TElement, TElement, SpanEnumerator<TElement>, TResult, TResult>> SelectMany<TResult>(
            Func<TElement, int, IEnumerable<TResult>> selector)
        {
            return span.AsSpanQuery().SelectMany(selector);
        }

        /// <summary>
        /// Returns a query that produces the selected result of the original query results, flattend into a single sequence.
        /// </summary>
        public SpanQuery<TElement, TResult, SelectManyEnumerator<TElement, TElement, SpanEnumerator<TElement>, TResult, TResult>> SelectMany<TResult>(
            Func<TElement, IEnumerable<TResult>> selector)
        {
            return span.AsSpanQuery().SelectMany(selector);
        }

        /// <summary>
        /// Returns a query that produces the selected result of the original query results, flattend into a single sequence.
        /// </summary>
        public SpanQuery<TElement, TResult, SelectManyEnumerator<TElement, TElement, SpanEnumerator<TElement>, TCollection, TResult>> SelectMany<TCollection, TResult>(
            Func<TElement, int, IEnumerable<TCollection>> collectionSelector,
            Func<TElement, TCollection, TResult> resultSelector)
        {
            return span.AsSpanQuery().SelectMany(collectionSelector, resultSelector);
        }

        /// <summary>
        /// Returns a query that produces the selected result of the original query results, flattend into a single sequence.
        /// </summary>
        public SpanQuery<TElement, TResult, SelectManyEnumerator<TElement, TElement, SpanEnumerator<TElement>, TCollection, TResult>> SelectMany<TCollection, TResult>(
            Func<TElement, IEnumerable<TCollection>> collectionSelector,
            Func<TElement, TCollection, TResult> resultSelector)
        {
            return span.AsSpanQuery().SelectMany(collectionSelector, resultSelector);
        }

        /// <summary>
        /// Returns a query that produces the results of the original query, except for the first n elements.
        /// </summary>
        public SpanQuery<TElement, TElement, SpanEnumerator<TElement>> Skip(
            int count)
        {
            return span[count..].AsSpanQuery();
        }

        /// <summary>
        /// Returns a query that produces the results of the original query, except for the last n elements.
        /// </summary>
        public SpanQuery<TElement, TElement, SpanEnumerator<TElement>> SkipLast(
            int count)
        {
            return span[..^count].AsSpanQuery();
        }

        /// <summary>
        /// Returns a query that produces the results of the original query, except for the first elements that satisify the predicate.
        /// </summary>
        public SpanQuery<TElement, TElement, SkipWhileEnumerator<TElement, TElement, SpanEnumerator<TElement>>> SkipWhile(
            Func<TElement, int, bool> predicate)
        {
            return span.AsSpanQuery().SkipWhile(predicate);
        }

        /// <summary>
        /// Returns a query that produces the results of the original query, except for the first elements that satisify the predicate.
        /// </summary>
        public SpanQuery<TElement, TElement, SkipWhileEnumerator<TElement, TElement, SpanEnumerator<TElement>>> SkipWhile(
            Func<TElement, bool> predicate)
        {
            return span.AsSpanQuery().SkipWhile(predicate);
        }

        /// <summary>
        /// Returns a query that produces the first n elements of the original query results.
        /// </summary>
        public SpanQuery<TElement, TElement, SpanEnumerator<TElement>> Take(
            int count)
        {
            return span[..count].AsSpanQuery();
        }

        /// <summary>
        /// Returns a query that produces the last n elements of the original query results.
        /// </summary>
        public SpanQuery<TElement, TElement, SpanEnumerator<TElement>> TakeLast(
            int count)
        {
            return span[^count..].AsSpanQuery();
        }

        /// <summary>
        /// Returns a query that produces the first elements of the original query results that satisfy the predicate.
        /// </summary>
        public SpanQuery<TElement, TElement, TakeWhileEnumerator<TElement, TElement, SpanEnumerator<TElement>>> TakeWhile(
            Func<TElement, int, bool> predicate)
        {
            return span.AsSpanQuery().TakeWhile(predicate);
        }

        /// <summary>
        /// Returns a query that produces the first elements of the original query results that satisfy the predicate.
        /// </summary>
        public SpanQuery<TElement, TElement, TakeWhileEnumerator<TElement, TElement, SpanEnumerator<TElement>>> TakeWhile(
            Func<TElement, bool> predicate)
        {
            return span.AsSpanQuery().TakeWhile(predicate);
        }

        /// <summary>
        /// Returns a query that produces the distinct union of the original query results and the specified list of elements.
        /// </summary>
        public SpanQuery<TElement, TElement, UnionByEnumerator<TElement, TElement, SpanEnumerator<TElement>, TElement>> Union(
            IEnumerable<TElement> elements,
            IEqualityComparer<TElement>? comparer = null)
        {
            return span.AsSpanQuery().Union(elements, comparer);
        }

        /// <summary>
        /// Returns a query that produces the distinct union of the original query results and the specified list of elements,
        /// as determined by the corresponding keys.
        /// </summary>
        public SpanQuery<TElement, TElement, UnionByEnumerator<TElement, TElement, SpanEnumerator<TElement>, TKey>> UnionBy<TKey>(
            IEnumerable<TElement> elements,
            Func<TElement, TKey> keySelector,
            IEqualityComparer<TKey>? keyComparer = null)
        {
            return span.AsSpanQuery().UnionBy(elements, keySelector, keyComparer);
        }

        /// <summary>
        /// Returns a query that produces the result of the original query that satisfy the predicate.
        /// </summary>
        public SpanQuery<TElement, TElement, WhereEnumerator<TElement, TElement, SpanEnumerator<TElement>>> Where(
            Func<TElement, int, bool> predicate)
        {
            return span.AsSpanQuery().Where(predicate);
        }

        /// <summary>
        /// Returns a query that produces the result of the original query that satisfy the predicate.
        /// </summary>
        public SpanQuery<TElement, TElement, WhereEnumerator<TElement, TElement, SpanEnumerator<TElement>>> Where(
            Func<TElement, bool> predicate)
        {
            return span.AsSpanQuery().Where(predicate);
        }

    }

    extension<TElement>(ReadOnlySpan<TElement> span)
    {
        /// <summary>
        /// Executes the query and returns the results as a <see cref="List{T}"/>.
        /// </summary>
        public List<TElement> ToList()
        {
            var list = new List<TElement>(span.Length);
            list.AddRange(span);
            return list;
        }

        /// <summary>
        /// Executes the query and returns the results a <see cref="HashSet{T}"/>.
        /// </summary>
        public HashSet<TElement> ToHashSet(
            IEqualityComparer<TElement>? comparer = null)
        {
            var set = new HashSet<TElement>(comparer);
            foreach (var t in span)
            {
                set.Add(t);
            }
            return set;
        }

        /// <summary>
        /// Executes the query and returns the results a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        public Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            Func<TElement, TKey> keySelector,
            Func<TElement, TValue> valueSelector,
            IEqualityComparer<TKey>? comparer = null)
            where TKey : notnull
        {
            var dictionary = new Dictionary<TKey, TValue>(comparer ?? EqualityComparer<TKey>.Default);

            foreach (var element in span)
            {
                dictionary.Add(keySelector(element), valueSelector(element));
            }

            return dictionary;
        }
        
        /// <summary>
        /// Executes the query and returns the results a <see cref="Dictionary{TKey, TElement}"/>.
        /// </summary>
        public Dictionary<TKey, TElement> ToDictionary<TKey>(
            Func<TElement, TKey> keySelector,
            IEqualityComparer<TKey>? comparer = null)
            where TKey : notnull
        {
            return span.ToDictionary(keySelector, x => x, comparer);
        }

        /// <summary>
        /// Returns a query that produces the results of the original query with an additional element added to the end.
        /// </summary>
        public SpanQuery<TElement, TElement, AppendEnumerator<TElement, TElement, SpanEnumerator<TElement>>> Append(
            TElement element)
        {
            return span.AsSpanQuery().Append(element);
        }

        /// <summary>
        /// Returns a query that casts each element of the original query to the specified type.
        /// </summary>
        public SpanQuery<TElement, TResult, CastEnumerator<TElement, TElement, SpanEnumerator<TElement>, TResult>> Cast<TResult>()
        {
            return span.AsSpanQuery().Cast<TResult>();
        }

        /// <summary>
        /// Returns a query that breaks the elements of the original query into chunks.
        /// </summary>
        public SpanQuery<TElement, TElement[], ChunkToArrayEnumerator<TElement, TElement, SpanEnumerator<TElement>>> ChunkToArray(
            int size)
        {
            return span.AsSpanQuery().Chunk(size);
        }
        
        public ChunkToSpanEnumerator<TElement> ChunkToSpan(int size)
        {
            return new ChunkToSpanEnumerator<TElement>(size);
        }
        
        public SpanQuery<TElement, Range, ChunkToRangeEnumerator<TElement>> ChunkToRange(int size)
        {
            return span.AsSpanQuery().With<Range, ChunkToRangeEnumerator<TElement>>(
                new ChunkToRangeEnumerator<TElement>(size, span)
            );
        }

        /// <summary>
        /// Returns a query that produces the concatenation of the results of the original query 
        /// with the specified list of elements.
        /// </summary>
        public SpanQuery<TElement, TElement, ConcatEnumerator<TElement, TElement, SpanEnumerator<TElement>>> Concat(
            IEnumerable<TElement> elements)
        {
            return span.AsSpanQuery().Concat(elements);
        }

        /// <summary>
        /// Returns a new query the produces the results of the original query if the results are not empty,
        /// otherwise produces a single result of the default value.
        /// </summary>
        public SpanQuery<TElement, TElement, DefaultIfEmptyEnumerator<TElement, TElement, SpanEnumerator<TElement>>> DefaultIfEmpty(
            TElement defaultValue)
        {
            return span.AsSpanQuery().DefaultIfEmpty(defaultValue);
        }

        /// <summary>
        /// Returns a new query the produces the results of the original query if the results are not empty,
        /// otherwise produces a single result of the default value for the result type.
        /// </summary>
        public SpanQuery<TElement, TElement, DefaultIfEmptyEnumerator<TElement, TElement, SpanEnumerator<TElement>>> DefaultIfEmpty()
        {
            return span.AsSpanQuery().DefaultIfEmpty();
        }

        /// <summary>
        /// Returns a query that produces the distinct elements of the original query.
        /// </summary>
        public SpanQuery<TElement, TElement,
            DistinctByEnumerator<TElement, TElement, SpanEnumerator<TElement>, TElement>> Distinct(
            IEqualityComparer<TElement>? keyComparer = null)
        {
            return span.AsSpanQuery().DistinctBy(x => x, keyComparer);
        }

        /// <summary>
        /// Returns a query that procues the distinct elements of the original query as determined by the specified key selector.
        /// </summary>
        public SpanQuery<TElement, TElement, DistinctByEnumerator<TElement, TElement, SpanEnumerator<TElement>, TKey>>
            DistinctBy<TKey>(
                Func<TElement, TKey> keySelector,
                IEqualityComparer<TKey>? keyComparer = null)
        {
            return span.AsSpanQuery().DistinctBy(keySelector, keyComparer);
        }

        /// <summary>
        /// Returns a query that produces the elements of the original query, except for those elements in the specified list.
        /// </summary>
        public SpanQuery<TElement, TElement, ExceptByEnumerator<TElement, TElement, SpanEnumerator<TElement>, TElement>> Except(
            IEnumerable<TElement> elements,
            IEqualityComparer<TElement>? comparer = null)
        {
            return span.AsSpanQuery().Except(elements, comparer);
        }

        /// <summary>
        /// Returns a query that produces the elements of the original query, except for those elements with keys in the specified list.
        /// </summary>
        public SpanQuery<TElement, TElement, ExceptByEnumerator<TElement, TElement, SpanEnumerator<TElement>, TKey>> ExceptBy<TKey>(
            IEnumerable<TKey> keys,
            Func<TElement, TKey> keySelector,
            IEqualityComparer<TKey>? keyComparer = null)
        {
            return span.AsSpanQuery().ExceptBy(keys, keySelector, keyComparer);
        }

        public SpanQuery<TElement, IGrouping<TKey, TElement>, GroupByEnumerator<TElement, TElement, SpanEnumerator<TElement>, TKey>>
            GroupBy<TKey>(
                Func<TElement, TKey> keySelector,
                IEqualityComparer<TKey>? keyComparer = null)
        {
            return span.AsSpanQuery().GroupBy(keySelector, keyComparer);
        }

        public SpanQuery<TElement, TResult, GroupJoinEnumerator<TElement, TElement, SpanEnumerator<TElement>, TInner, TKey, TResult>>
            GroupJoin<TInner, TKey, TResult>(
                IEnumerable<TInner> inner,
                Func<TElement, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                Func<TElement, IEnumerable<TInner>, TResult> resultSelector,
                IEqualityComparer<TKey>? comparer = null)
        {
            return span.AsSpanQuery().GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
        }

        /// <summary>
        /// Returns a query that produces the elements of the original query that have keys also in the specified list.
        /// </summary>
        public SpanQuery<TElement, TElement, IntersectByEnumerator<TElement, TElement, SpanEnumerator<TElement>, TKey>> IntersectBy<TKey>(
            IEnumerable<TKey> keys,
            Func<TElement, TKey> keySelector,
            IEqualityComparer<TKey>? keyComparer = null)
        {
            return span.AsSpanQuery().IntersectBy(keys, keySelector, keyComparer);
        }

        /// <summary>
        /// Returns a query that produces the elements of the original query that is also in the specified list.
        /// </summary>
        public SpanQuery<TElement, TElement, IntersectByEnumerator<TElement, TElement, SpanEnumerator<TElement>, TElement>> Intersect(
            IEnumerable<TElement> elements,
            IEqualityComparer<TElement>? comparer = null)
        {
            return span.AsSpanQuery().Intersect(elements, comparer);
        }
        
        /// <summary>
        /// Produces the join of the result of the original query with the specified list of elements.
        /// </summary>
        public SpanQuery<TElement, TResult, JoinEnumerator<TElement, TElement, SpanEnumerator<TElement>, TInner, TKey, TResult>> Join<TInner, TKey, TResult>(
            IEnumerable<TInner> inner,
            Func<TElement, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TElement, TInner, TResult> resultSelector,
            IEqualityComparer<TKey>? keyComparer = null)
        {
            return span.AsSpanQuery().Join(inner, outerKeySelector, innerKeySelector, resultSelector, keyComparer);
        }

        /// <summary>
        /// Returns the elements of the original query that are of the specified type.
        /// </summary>
        public SpanQuery<TElement, TResult, OfTypeEnumerator<TElement, TElement, SpanEnumerator<TElement>, TResult>> OfType<TResult>()
        {
            return span.AsSpanQuery().OfType<TResult>();
        }

        /// <summary>
        /// Returns a query that produces the elements of the original query in order.
        /// </summary>
        public SpanQuery<TElement, TElement, OrderByEnumerator<TElement, TElement, SpanEnumerator<TElement>>> Order(
            IComparer<TElement>? comparer = null)
        {
            return span.AsSpanQuery().Order(comparer);
        }

        /// <summary>
        /// Returns a query that produces the elements of the original query in order of the specified key.
        /// </summary>
        public SpanQuery<TElement, TElement, OrderByEnumerator<TElement, TElement, SpanEnumerator<TElement>>> OrderBy<TKey>(
            Func<TElement, TKey> keySelector,
            IComparer<TKey>? comparer = null)
        {
            return span.AsSpanQuery().OrderBy(keySelector, comparer);
        }

        /// <summary>
        /// Returns a query that produces the elements of the original query in descending order of the specified key.
        /// </summary>
        public SpanQuery<TElement, TElement, OrderByEnumerator<TElement, TElement, SpanEnumerator<TElement>>> OrderByDescending<TKey>(
            Func<TElement, TKey> keySelector,
            IComparer<TKey>? comparer = null)
        {
            return span.AsSpanQuery().OrderByDescending(keySelector, comparer);
        }

        /// <summary>
        /// Returns a query that produces the elements of the original query in descending order.
        /// </summary>
        public SpanQuery<TElement, TElement, OrderByEnumerator<TElement, TElement, SpanEnumerator<TElement>>> OrderDescending(
            IComparer<TElement>? comparer = null)
        {
            return span.AsSpanQuery().OrderDescending(comparer);
        }

        /// <summary>
        /// Returns a query that produces the prepended element followed by the elements of the result of the original query.
        /// </summary>
        public SpanQuery<TElement, TElement, PrependSpanEnumerator<TElement, TElement, SpanEnumerator<TElement>>> Prepend(
            TElement element)
        {
            return span.AsSpanQuery().Prepend(element);
        }

        /// <summary>
        /// Returns a query that produces the result of the original query in reverse.
        /// </summary>
        public SpanQuery<TElement, TElement, ReverseEnumerator<TElement, TElement, SpanEnumerator<TElement>>> Reverse()
        {
            return span.AsSpanQuery().Reverse();
        }

        /// <summary>
        /// Returns a query that produces the selected or mapped values of the original query results.
        /// </summary>
        public SpanQuery<TElement, TResult, SelectEnumerator<TElement, TElement, SpanEnumerator<TElement>, TResult>> Select<TResult>(
            Func<TElement, int, TResult> selector)
        {
            return span.AsSpanQuery().Select(selector);
        }

        /// <summary>
        /// Returns a query that produces the selected or mapped values of the original query results.
        /// </summary>
        public SpanQuery<TElement, TResult, SelectEnumerator<TElement, TElement, SpanEnumerator<TElement>, TResult>> Select<TResult>(
            Func<TElement, TResult> selector)
        {
            return span.AsSpanQuery().Select(selector);
        }

        /// <summary>
        /// Returns a query that produces the selected result of the original query results, flattend into a single sequence.
        /// </summary>
        public SpanQuery<TElement, TResult, SelectManyEnumerator<TElement, TElement, SpanEnumerator<TElement>, TResult, TResult>> SelectMany<TResult>(
            Func<TElement, int, IEnumerable<TResult>> selector)
        {
            return span.AsSpanQuery().SelectMany(selector);
        }

        /// <summary>
        /// Returns a query that produces the selected result of the original query results, flattend into a single sequence.
        /// </summary>
        public SpanQuery<TElement, TResult, SelectManyEnumerator<TElement, TElement, SpanEnumerator<TElement>, TResult, TResult>> SelectMany<TResult>(
            Func<TElement, IEnumerable<TResult>> selector)
        {
            return span.AsSpanQuery().SelectMany(selector);
        }

        /// <summary>
        /// Returns a query that produces the selected result of the original query results, flattend into a single sequence.
        /// </summary>
        public SpanQuery<TElement, TResult, SelectManyEnumerator<TElement, TElement, SpanEnumerator<TElement>, TCollection, TResult>> SelectMany<TCollection, TResult>(
            Func<TElement, int, IEnumerable<TCollection>> collectionSelector,
            Func<TElement, TCollection, TResult> resultSelector)
        {
            return span.AsSpanQuery().SelectMany(collectionSelector, resultSelector);
        }

        /// <summary>
        /// Returns a query that produces the selected result of the original query results, flattend into a single sequence.
        /// </summary>
        public SpanQuery<TElement, TResult, SelectManyEnumerator<TElement, TElement, SpanEnumerator<TElement>, TCollection, TResult>> SelectMany<TCollection, TResult>(
            Func<TElement, IEnumerable<TCollection>> collectionSelector,
            Func<TElement, TCollection, TResult> resultSelector)
        {
            return span.AsSpanQuery().SelectMany(collectionSelector, resultSelector);
        }

        /// <summary>
        /// Returns a query that produces the results of the original query, except for the first n elements.
        /// </summary>
        public SpanQuery<TElement, TElement, SpanEnumerator<TElement>> Skip(
            int count)
        {
            return span[count..].AsSpanQuery();
        }

        /// <summary>
        /// Returns a query that produces the results of the original query, except for the last n elements.
        /// </summary>
        public SpanQuery<TElement, TElement, SpanEnumerator<TElement>> SkipLast(
            int count)
        {
            return span[..^count].AsSpanQuery();
        }

        /// <summary>
        /// Returns a query that produces the results of the original query, except for the first elements that satisify the predicate.
        /// </summary>
        public SpanQuery<TElement, TElement, SkipWhileEnumerator<TElement, TElement, SpanEnumerator<TElement>>> SkipWhile(
            Func<TElement, int, bool> predicate)
        {
            return span.AsSpanQuery().SkipWhile(predicate);
        }

        /// <summary>
        /// Returns a query that produces the results of the original query, except for the first elements that satisify the predicate.
        /// </summary>
        public SpanQuery<TElement, TElement, SkipWhileEnumerator<TElement, TElement, SpanEnumerator<TElement>>> SkipWhile(
            Func<TElement, bool> predicate)
        {
            return span.AsSpanQuery().SkipWhile(predicate);
        }

        /// <summary>
        /// Returns a query that produces the first n elements of the original query results.
        /// </summary>
        public SpanQuery<TElement, TElement, SpanEnumerator<TElement>> Take(
            int count)
        {
            return span[..count].AsSpanQuery();
        }

        /// <summary>
        /// Returns a query that produces the last n elements of the original query results.
        /// </summary>
        public SpanQuery<TElement, TElement, SpanEnumerator<TElement>> TakeLast(
            int count)
        {
            return span[^count..].AsSpanQuery();
        }

        /// <summary>
        /// Returns a query that produces the first elements of the original query results that satisfy the predicate.
        /// </summary>
        public SpanQuery<TElement, TElement, TakeWhileEnumerator<TElement, TElement, SpanEnumerator<TElement>>> TakeWhile(
            Func<TElement, int, bool> predicate)
        {
            return span.AsSpanQuery().TakeWhile(predicate);
        }

        /// <summary>
        /// Returns a query that produces the first elements of the original query results that satisfy the predicate.
        /// </summary>
        public SpanQuery<TElement, TElement, TakeWhileEnumerator<TElement, TElement, SpanEnumerator<TElement>>> TakeWhile(
            Func<TElement, bool> predicate)
        {
            return span.AsSpanQuery().TakeWhile(predicate);
        }

        /// <summary>
        /// Returns a query that produces the distinct union of the original query results and the specified list of elements.
        /// </summary>
        public SpanQuery<TElement, TElement, UnionByEnumerator<TElement, TElement, SpanEnumerator<TElement>, TElement>> Union(
            IEnumerable<TElement> elements,
            IEqualityComparer<TElement>? comparer = null)
        {
            return span.AsSpanQuery().Union(elements, comparer);
        }

        /// <summary>
        /// Returns a query that produces the distinct union of the original query results and the specified list of elements,
        /// as determined by the corresponding keys.
        /// </summary>
        public SpanQuery<TElement, TElement, UnionByEnumerator<TElement, TElement, SpanEnumerator<TElement>, TKey>> UnionBy<TKey>(
            IEnumerable<TElement> elements,
            Func<TElement, TKey> keySelector,
            IEqualityComparer<TKey>? keyComparer = null)
        {
            return span.AsSpanQuery().UnionBy(elements, keySelector, keyComparer);
        }

        /// <summary>
        /// Returns a query that produces the result of the original query that satisfy the predicate.
        /// </summary>
        public SpanQuery<TElement, TElement, WhereEnumerator<TElement, TElement, SpanEnumerator<TElement>>> Where(
            Func<TElement, int, bool> predicate)
        {
            return span.AsSpanQuery().Where(predicate);
        }

        /// <summary>
        /// Returns a query that produces the result of the original query that satisfy the predicate.
        /// </summary>
        public SpanQuery<TElement, TElement, WhereEnumerator<TElement, TElement, SpanEnumerator<TElement>>> Where(
            Func<TElement, bool> predicate)
        {
            return span.AsSpanQuery().Where(predicate);
        }

    }

    public ref struct ChunkToSpanEnumerator<T>(int size) : ISpanEnumerator<T, ReadOnlySpan<T>>
    {
        public ReadOnlySpan<T> Current { get; private set; }
        
        private int _currentIndex;
        
        public bool MoveNext(ReadOnlySpan<T> span)
        {
            if (span.Length == 0)
            {
                return false;
            }

            int chunkEnd = _currentIndex + Math.Min(size, span.Length);
            Current = span[_currentIndex..chunkEnd];
            _currentIndex = chunkEnd;
            return true;
        }
    }
    
    public struct ChunkToRangeEnumerator<T>(int size, ReadOnlySpan<T> span) : ISpanEnumerator<T, Range>
    {
        private int _currentIndex;
        private readonly int _spanLength = span.Length;

        public Range Current { get; private set; }

        public bool MoveNext(ReadOnlySpan<T> span)
        {
            if (_currentIndex >= _spanLength)
            {
                return false;
            }

            int chunkSize = Math.Min(size, _spanLength - _currentIndex);
            Current = _currentIndex..(_currentIndex + chunkSize);
            _currentIndex += chunkSize;
            return true;
        }
    }
}