using System.Collections.Frozen;
using System.Globalization;
using System.IO;
using System.Text;

namespace Maxine.TU.Sims4Exporter;

public class StringHelpers
{
    private static readonly FrozenSet<char> InvalidFileNameCharsSet = Path.GetInvalidFileNameChars().ToFrozenSet();
    public static string NormalizeFilename(string name)
    {
        var normalizedString = name.Normalize(NormalizationForm.FormD);
        /*using*/ var stringBuilder = name.Length <= ValueStringBuilder.StackallocCharBufferSizeLimit
            ? new ValueStringBuilder(stackalloc char[name.Length])
            : new ValueStringBuilder(name.Length);

        foreach (var c in normalizedString)
        {
            switch (CharUnicodeInfo.GetUnicodeCategory(c))
            {
                // case UnicodeCategory.LowercaseLetter:
                // case UnicodeCategory.UppercaseLetter:
                // case UnicodeCategory.DecimalDigitNumber:
                //     stringBuilder.Append(c);
                //     break;
                case UnicodeCategory.ParagraphSeparator:
                case UnicodeCategory.SpaceSeparator:
                    if (stringBuilder.Peek() != ' ')
                    {
                        stringBuilder.Append(' ');
                    }
                    break;
                case UnicodeCategory.ConnectorPunctuation:
                    if (stringBuilder.Peek() != '_')
                    {
                        stringBuilder.Append('_');
                    }
                    break;
                case UnicodeCategory.DashPunctuation:
                    stringBuilder.Append('-');
                    break;
                case UnicodeCategory.InitialQuotePunctuation:
                case UnicodeCategory.FinalQuotePunctuation:
                    stringBuilder.Append('\'');
                    break;
                case UnicodeCategory.Control:
                case UnicodeCategory.Surrogate:
                case UnicodeCategory.PrivateUse:
                    break;
                default:
                    if (char.IsAscii(c) && !InvalidFileNameCharsSet.Contains(c))
                    {
                        stringBuilder.Append(c);
                    }
                    else if (stringBuilder.Peek() != '_')
                    {
                        stringBuilder.Append('_');
                    }

                    break;
            }
        }

        var str = new string(stringBuilder.AsSpan().TrimEnd('.', ' ').TrimStart(' '));
        stringBuilder.Dispose();
        return str;
    }
}