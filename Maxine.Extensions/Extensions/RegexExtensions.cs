using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.Extensions.Primitives;

namespace Maxine.Extensions;

[PublicAPI]
public static class RegexExtensions
{
    public static ValueMatchSpan ValueMatch(this Regex regex, ReadOnlySpan<char> input, int? startat = null)
    {
        foreach (var match in regex.EnumerateMatches(input, startat ?? (regex.RightToLeft ? input.Length : 0)))
            return new ValueMatchSpan(input, match);

        return new ValueMatchSpan();
    }
    
    public static ValueMatchSpan ValueMatch(ReadOnlySpan<char> input, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
    {
        foreach (var match in Regex.EnumerateMatches(input, pattern))
            return new ValueMatchSpan(input, match);

        return new ValueMatchSpan();
    }
    
    public static ValueMatchSpan ValueMatch(ReadOnlySpan<char> input, [StringSyntax(StringSyntaxAttribute.Regex, nameof(options))] string pattern, RegexOptions options)
    {
        foreach (var match in Regex.EnumerateMatches(input, pattern, options))
            return new ValueMatchSpan(input, match);

        return new ValueMatchSpan();
    }
    
    public static ValueMatchSpan ValueMatch(ReadOnlySpan<char> input, [StringSyntax(StringSyntaxAttribute.Regex, nameof(options))] string pattern, RegexOptions options, TimeSpan matchTimeout)
    {
        foreach (var match in Regex.EnumerateMatches(input, pattern, options, matchTimeout))
            return new ValueMatchSpan(input, match);

        return new ValueMatchSpan();
    }
    
    public static ValueMatchSegment ValueMatch(this Regex regex, string input, int? startat = null)
    {
        foreach (var match in regex.EnumerateMatches(input, startat ?? (regex.RightToLeft ? input.Length : 0)))
            return new ValueMatchSegment(input, match);

        return new ValueMatchSegment();
    }
    
    public static ValueMatchSegment ValueMatch(string input, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
    {
        foreach (var match in Regex.EnumerateMatches(input, pattern))
            return new ValueMatchSegment(input, match);

        return new ValueMatchSegment();
    }
    
    public static ValueMatchSegment ValueMatch(string input, [StringSyntax(StringSyntaxAttribute.Regex, nameof(options))] string pattern, RegexOptions options)
    {
        foreach (var match in Regex.EnumerateMatches(input, pattern, options))
            return new ValueMatchSegment(input, match);

        return new ValueMatchSegment();
    }
    
    public static ValueMatchSegment ValueMatch(string input, [StringSyntax(StringSyntaxAttribute.Regex, nameof(options))] string pattern, RegexOptions options, TimeSpan matchTimeout)
    {
        foreach (var match in Regex.EnumerateMatches(input, pattern, options, matchTimeout))
            return new ValueMatchSegment(input, match);

        return new ValueMatchSegment();
    }
}

[PublicAPI]
public ref struct ValueMatchSpan
{
    public ReadOnlySpan<char> Match { get; }
    public int Index { get; }
    public int Length => Match.Length;
    public bool IsMatch { get; } = true;

    public ValueMatchSpan(ReadOnlySpan<char> input, ValueMatch valueMatch)
    {
        Match = input.Slice(valueMatch.Index, valueMatch.Length);
        Index = valueMatch.Index;
        IsMatch = true;
    }

    public ValueMatchSpan()
    {
        Match = default;
        Index = 0;
        IsMatch = false;
    }

    public static implicit operator ReadOnlySpan<char>(ValueMatchSpan valueMatchSpan) => valueMatchSpan.Match;
}

[PublicAPI]
public readonly struct ValueMatchSegment
{
    public ReadOnlySpan<char> Match => MatchSegment;
    public StringSegment MatchSegment { get; }
    public int Index => MatchSegment.Offset;
    public int Length => MatchSegment.Length;
    public bool IsMatch { get; } = true;

    public ValueMatchSegment(string input, ValueMatch valueMatch)
    {
        MatchSegment = new StringSegment(input, valueMatch.Index, valueMatch.Length);
        IsMatch = true;
    }

    public ValueMatchSegment()
    {
        MatchSegment = string.Empty;
        IsMatch = false;
    }
    
    public static implicit operator ReadOnlySpan<char>(ValueMatchSegment valueMatchSegment) => valueMatchSegment.MatchSegment;
    public static implicit operator ReadOnlyMemory<char>(ValueMatchSegment valueMatchSegment) => valueMatchSegment.MatchSegment;
    public static implicit operator StringSegment(ValueMatchSegment valueMatchSegment) => valueMatchSegment.MatchSegment;
}