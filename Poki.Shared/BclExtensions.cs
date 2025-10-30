namespace Poki.Shared;

public static class BclExtensions
{
    /// <summary>
    /// Removes a suffix from a given string if it is present in the string; returns the unmodified string otherwise.
    /// </summary>
    /// <param name="str">The input string</param>
    /// <param name="suffix">The substring representing the suffix to remove</param>
    /// <returns>
    /// A string without the suffix <paramref name="suffix"/>, or the unmodified string if it didn't have one
    /// </returns>
    public static string RemoveFromEnd(this string str, string suffix)
    {
        return str.EndsWith(suffix) ? str[..^suffix.Length] : str;
    }
    
    /// <summary>
    /// Removes a prefix from a given string if it is present in the string; returns the unmodified string otherwise.
    /// </summary>
    /// <param name="str">The input string</param>
    /// <param name="prefix">The substring representing the prefix to remove</param>
    /// <returns>
    /// A string without the prefix <paramref name="prefix"/>, or the unmodified string if it didn't have one
    /// </returns>
    public static string RemoveFromStart(this string str, string prefix)
    {
        return str.StartsWith(prefix) ? str[prefix.Length..] : str;
    }
}