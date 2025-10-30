using Microsoft.Extensions.Primitives;

namespace Maxine.Extensions.Collections;

public class UniquePrefixTrie
{
    public static IEnumerable<StringSegment> UniquePrefix(string[] a)
    {
        var size = a.Length;

        /* sort the array of strings */
        Array.Sort(a, StringComparer.Ordinal);

        /* compare the first string with its only right neighbor */
        var j = 0;
        while (j < Math.Min(a[0].Length - 1, a[1].Length - 1))
        {
            if (a[0][j] == a[1][j])
                j++;
            else
                break;
        }

        yield return new StringSegment(a[0], 0, j + 1);

        /* Store the unique prefix of a[1] from its left neighbor */
        var temp_prefix = new StringSegment(a[1], 0, j + 1);
        for (var i = 1; i < size - 1; i++)
        {
            /* compute common prefix of a[i] unique from its right neighbor */
            j = 0;
            while (j < Math.Min(a[i].Length - 1, a[i + 1].Length - 1))
            {
                if (a[i][j] == a[i + 1][j])
                    j++;
                else
                    break;
            }

            var new_prefix = new StringSegment(a[i], 0, j + 1);

            /* compare the new prefix with previous prefix */
            if (temp_prefix.Length > new_prefix.Length)
                yield return temp_prefix;
            else
                yield return new_prefix;

            /* store the prefix of a[i+1] unique from its left neighbour */
            temp_prefix = new StringSegment(a[i + 1], 0, j + 1);
        }

        /* compute the unique prefix for the last string in sorted array */
        j = 0;
        var sec_last = a[size - 2];

        var last = a[size - 1];

        while (j < Math.Min(sec_last.Length - 1, last.Length - 1))
        {
            if (sec_last[j] == last[j])
                j++;
            else
                break;
        }

        yield return new StringSegment(last, 0, j + 1);
    }

    // /* Driver code */
    // public static void Main(string[] args)
    // {
    //     var gfg = new Unique_Prefix_Trie();
    //
    //     string[] input = ["zebra", "dog", "duck", "dove"];
    //
    //     var output = gfg.UniquePrefix(input);
    //     Console.WriteLine( "The shortest unique prefixes" +
    //                         " in sorted order are :");
    //
    //     for (var i = 0; i < output.Length; i++)
    //         Console.Write( output[i] + " ");
    // }
}