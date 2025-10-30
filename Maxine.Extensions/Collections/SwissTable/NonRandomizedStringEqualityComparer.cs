using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Maxine.Extensions.Collections;


// NonRandomizedStringEqualityComparer is the comparer used by default with the SwissTable<string,...>
// We use NonRandomizedStringEqualityComparer as default comparer as it doesnt use the randomized string hashing which
// keeps the performance not affected till we hit collision threshold and then we switch to the comparer which is using
// randomized string hashing.
[Serializable] // Required for compatibility with .NET Core 2.0 as we exposed the NonRandomizedStringEqualityComparer inside the serialization blob
// Needs to be public to support binary serialization compatibility
public class NonRandomizedStringEqualityComparer : IEqualityComparer<string?>
{
    // SwissTable<...>.Comparer and similar methods need to return the original IEqualityComparer
    // that was passed in to the ctor. The caller chooses one of these singletons so that the
    // GetUnderlyingEqualityComparer method can return the correct value.

    private static readonly NonRandomizedStringEqualityComparer WrappedAroundDefaultComparer = new OrdinalComparer(EqualityComparer<string?>.Default);
    private static readonly NonRandomizedStringEqualityComparer WrappedAroundStringComparerOrdinal = new OrdinalComparer(StringComparer.Ordinal);
    private static readonly NonRandomizedStringEqualityComparer WrappedAroundStringComparerOrdinalIgnoreCase = new OrdinalIgnoreCaseComparer(StringComparer.OrdinalIgnoreCase);

    private readonly IEqualityComparer<string?> _underlyingComparer;

    private NonRandomizedStringEqualityComparer(IEqualityComparer<string?> underlyingComparer)
    {
        Debug.Assert(underlyingComparer != null);
        _underlyingComparer = underlyingComparer;
    }

    // This is used by the serialization engine.
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected NonRandomizedStringEqualityComparer(SerializationInfo information, StreamingContext context)
        : this(EqualityComparer<string?>.Default)
    {
    }

    public virtual bool Equals(string? x, string? y)
    {
        // This instance may have been deserialized into a class that doesn't guarantee
        // these parameters are non-null. Can't short-circuit the null checks.

        return string.Equals(x, y);
    }

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "GetNonRandomizedHashCode")]
    private static extern int GetNonRandomizedHashCode(string? obj);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "GetNonRandomizedHashCodeOrdinalIgnoreCase")]
    private static extern int GetNonRandomizedHashCodeOrdinalIgnoreCase(string? obj);

    public virtual int GetHashCode(string? obj)
    {
        // This instance may have been deserialized into a class that doesn't guarantee
        // these parameters are non-null. Can't short-circuit the null checks.

        return obj != null ? GetNonRandomizedHashCode(obj) : 0;
    }

    // internal virtual RandomizedStringEqualityComparer GetRandomizedEqualityComparer()
    // {
    //     return RandomizedStringEqualityComparer.Create(_underlyingComparer, ignoreCase: false);
    // }

    // Gets the comparer that should be returned back to the caller when querying the
    // ICollection.Comparer property. Also used for serialization purposes.
    public virtual IEqualityComparer<string?> GetUnderlyingEqualityComparer() => _underlyingComparer;

    private sealed class OrdinalComparer : NonRandomizedStringEqualityComparer
    {
        internal OrdinalComparer(IEqualityComparer<string?> wrappedComparer)
            : base(wrappedComparer)
        {
        }

        public override bool Equals(string? x, string? y) => string.Equals(x, y);

        public override int GetHashCode(string? obj)
        {
            Debug.Assert(obj != null, "This implementation is only called from first-party collection types that guarantee non-null parameters.");
            return GetNonRandomizedHashCode(obj);
        }

    }

    private sealed class OrdinalIgnoreCaseComparer : NonRandomizedStringEqualityComparer
    {
        internal OrdinalIgnoreCaseComparer(IEqualityComparer<string?> wrappedComparer)
            : base(wrappedComparer)
        {
        }

        public override bool Equals(string? x, string? y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode(string? obj)
        {
            Debug.Assert(obj != null, "This implementation is only called from first-party collection types that guarantee non-null parameters.");
            return GetNonRandomizedHashCodeOrdinalIgnoreCase(obj);
        }
    }

    public static IEqualityComparer<string>? GetStringComparer(object? comparer)
    {
        // Special-case EqualityComparer<string>.Default, StringComparer.Ordinal, and StringComparer.OrdinalIgnoreCase.
        // We use a non-randomized comparer for improved perf, falling back to a randomized comparer if the
        // hash buckets become unbalanced.

        if (ReferenceEquals(comparer, EqualityComparer<string>.Default))
        {
            return WrappedAroundDefaultComparer;
        }

        if (ReferenceEquals(comparer, StringComparer.Ordinal))
        {
            return WrappedAroundStringComparerOrdinal;
        }

        if (ReferenceEquals(comparer, StringComparer.OrdinalIgnoreCase))
        {
            return WrappedAroundStringComparerOrdinalIgnoreCase;
        }

        return null;
    }
}