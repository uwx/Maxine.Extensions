using System.Text;

namespace Maxine.Extensions;

public static class RomanUtilities
{
    // https://stackoverflow.com/a/11749642, https://stackoverflow.com/a/23303475
    
    /// <summary>
    /// Converts a number to roman numeral.
    /// </summary>
    /// <param name="number">The number to convert</param>
    /// <returns>A roman numeral representing the input number</returns>
    public static string ToRomanNumeral(int number)
    {
        if (number < 1) return string.Empty;

        var sb = new StringBuilder();

        while (number > 0)
        {
            switch (number)
            {
                case >= 1000: sb.Append('M'); number -= 1000; break;
                case >= 900: sb.Append("CM"); number -= 900; break;
                case >= 500: sb.Append('D'); number -= 500; break;
                case >= 400: sb.Append("CD"); number -= 400; break;
                case >= 100: sb.Append('C'); number -= 100; break;
                case >= 90: sb.Append("XC"); number -= 90; break;
                case >= 50: sb.Append('L'); number -= 50; break;
                case >= 40: sb.Append("XL"); number -= 40; break;
                case >= 10: sb.Append('X'); number -= 10; break;
                case >= 9: sb.Append("IX"); number -= 9; break;
                case >= 5: sb.Append('V'); number -= 5; break;
                case >= 4: sb.Append("IV"); number -= 4; break;
                default: sb.Append('I'); number -= 1; break;
            }
        }

        return sb.ToString();
    }
}