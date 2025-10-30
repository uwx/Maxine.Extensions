using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Maxine.Extensions.Shared;

public static class BclExtensions
{
    /// <summary>
    /// Converts an array of one type to an array of another type.
    /// </summary>
    /// <param name="array">The one-dimensional, zero-based <see cref="Array"/> to convert to a target type.</param>
    /// <param name="converter">
    /// A <see cref="Converter{TInput,TOutput}"/> that converts each element from one type to another type.
    /// </param>
    /// <typeparam name="TInput">The type of the elements of the source array.</typeparam>
    /// <typeparam name="TOutput">The type of the elements of the target array.</typeparam>
    /// <returns>An array of the target type containing the converted elements from the source array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array, Converter<TInput, TOutput> converter)
    {
        return Array.ConvertAll(array, converter);
    }

    /// <summary>
    /// Casts an array of one type to an array of a base type of the input array.
    /// </summary>
    /// <param name="array">The one-dimensional, zero-based <see cref="Array"/> to convert to a target type.</param>
    /// <typeparam name="TInput">The type of the elements of the source array.</typeparam>
    /// <typeparam name="TOutput">The type of the elements of the target array.</typeparam>
    /// <returns>An array of the target type containing the converted elements from the source array.</returns>
    public static TOutput[] Upcast<TInput, TOutput>(this TInput[] array)
        where TInput : TOutput?
        where TOutput : class?
    {
        var newArray = new TOutput[array.Length];
        for (var i = 0; i < array.Length; i++)
        {
            newArray[i] = array[i]!;
        }
        return newArray;
    }
    
    /// <summary>
    /// Assigns the given value of type T to each element of the specified array, and returns the modified array.
    /// </summary>
    /// <param name="array">The array to be filled.</param>
    /// <param name="value">The value to assign to each array element.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>The existing, but newly-modified array.</returns>
    public static T[] Fill<T>(this T[] array, T value)
    {
        Array.Fill(array, value);
        return array;
    }

    public static IEnumerable<T> PadAndTruncateToLength<T>(this IEnumerable<T> enumerable, int length, T value)
    {
        return enumerable
            .Concat(Enumerable.Repeat(value, length))
            .Take(length);
    }

    public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, T value, IEqualityComparer<T>? comparer = null)
    {
        comparer ??= EqualityComparer<T>.Default;

        return enumerable.Where(element => !comparer.Equals(element, value));
    }

    [StackTraceHidden]
    public static Exception SetCurrentStackTrace(this Exception ex) => ExceptionDispatchInfo.SetCurrentStackTrace(ex);

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowWithExistingStackTrace(this Exception ex) => ExceptionDispatchInfo.Capture(ex).Throw();
}
