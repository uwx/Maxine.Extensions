using Microsoft.Extensions.Primitives;

namespace Poki.Utilities;

public static class StringSegments
{
    public static string Concat(StringSegment str0, StringSegment str1)
        => string.Concat(str0.AsSpan(), str1.AsSpan());
    
    public static string Concat(StringSegment str0, StringSegment str1, StringSegment str2)
        => string.Concat(str0.AsSpan(), str1.AsSpan(), str2.AsSpan());
    
    public static string Concat(StringSegment str0, StringSegment str1, StringSegment str2, StringSegment str3)
        => string.Concat(str0.AsSpan(), str1.AsSpan(), str2.AsSpan(), str3.AsSpan());
}