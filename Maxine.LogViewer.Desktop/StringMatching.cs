using System;
using System.Text.RegularExpressions;

namespace Maxine.LogViewer.Desktop;

public readonly record struct CompiledFilter(string Original, int Start, int Length);

public readonly record struct CompiledFilters(CompiledFilter[] Positive, CompiledFilter[] Negative)
{
    public bool TestPositive(string str)
    {
        var strSpan = str.AsSpan();
        var positive = Positive;

        foreach (var (original, start, length) in positive)
        {
            if (strSpan.Contains(original.AsSpan(start, length), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        
        return false;
    }
    
    public bool TestNegative(string str)
    {
        var strSpan = str.AsSpan();
        var negative = Negative;

        foreach (var (original, start, length) in negative)
        {
            if (!strSpan.Contains(original.AsSpan(start, length), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }
        
        return true;
    }

    public bool HasPositiveFilters => Positive.Length != 0;
}

public static class StringMatching
{
    private static readonly Regex FiltersRegex = new(@"(?<Negate>-?)(?:""(?<Text>.*?)""|(?<Text>\S+))(?:\s|$)", RegexOptions.Compiled);

    public static CompiledFilters CompileFilters(string filterText)
    {
        var matches = FiltersRegex.Matches(filterText);

        var array = new CompiledFilter[matches.Count];
        var start = 0;
        var end = array.Length - 1;

        foreach (Match match in matches)
        {
            var negate = !match.Groups["Negate"].ValueSpan.IsEmpty;
            var text = match.Groups["Text"];

            var filter = new CompiledFilter(filterText, text.Index, text.Length);
            if (negate)
                array[end--] = filter;
            else
                array[start++] = filter;
        }

        return new CompiledFilters(array[..start], array[start..]);
    }
}