using System.Numerics;

namespace Maxine.Extensions;

public static class FileSizeUtils
{
    private static string[] DecimalSuffixes { get; } = ["B", "KB", "MB", "GB", "TB", "PB"];
    private static string[] BinarySuffixes { get; } = ["B", "KiB", "MiB", "GiB", "TiB", "PiB"];

    // https://stackoverflow.com/a/281679
    public static string HumanizeFileSize<T>(T sizeInBytes, bool isDecimal = false) where T : INumber<T>
    {
        ReadOnlySpan<string> sizes = isDecimal ? DecimalSuffixes : BinarySuffixes;

        var divisor = isDecimal ? T.CreateTruncating(1000) : T.CreateTruncating(1024);
        
        var order = 0;
        while (sizeInBytes >= divisor && order < sizes.Length - 1) {
            order++;
            sizeInBytes /= divisor;
        }

        return $"{sizeInBytes:0.##} {sizes[order]}";
    }
}