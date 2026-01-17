using System.Buffers;
using System.Text;

namespace Maxine.Extensions;

public static class StringBuilderExtensions
{
    extension(StringBuilder builder)
    {
        public unsafe StringBuilder Append(byte* bytes, int byteCount, Encoding encoding)
        {
            var span = new ReadOnlySpan<byte>(bytes, byteCount);
            return builder.Append(span, encoding);
        }

        public unsafe StringBuilder Append(byte* bytes, int byteCount)
        {
            return builder.Append(bytes, byteCount, Encoding.UTF8);
        }

        public StringBuilder Append(ReadOnlySpan<byte> bytes, Encoding encoding)
        {
            var charCount = encoding.GetCharCount(bytes);
            char[]? arrayToReturn = null;
            try
            {
                var chars = charCount > 512
                    ? arrayToReturn = ArrayPool<char>.Shared.Rent(charCount)
                    : stackalloc char[charCount];
                var charsWritten = encoding.GetChars(bytes, chars[..charCount]);
                builder.Append(chars[..charsWritten]);
                return builder;
            }
            finally
            {
                if (arrayToReturn != null)
                {
                    ArrayPool<char>.Shared.Return(arrayToReturn);
                }
            }
        }

        public StringBuilder Append(ReadOnlySpan<byte> bytes)
        {
            return builder.Append(bytes, Encoding.UTF8);
        }
    }
}