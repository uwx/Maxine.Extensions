using System.Runtime.InteropServices;

namespace Maxine.Extensions;

public static class UrlUtil
{
    private const char UrlSeparator = '/';

    public static string Combine(string uri1, string uri2)
    {
        var seg1 = uri1.AsSpan().TrimEnd(UrlSeparator);
        var seg2 = uri2.AsSpan().TrimStart(UrlSeparator);

        Span<char> chars = [UrlSeparator];

        return string.Concat(seg1, chars, seg2);
    }

    // Loosely from Path.JoinInternal
    public static unsafe string Combine(string uri1, string uri2, string uri3)
    {
        var seg1 = uri1.AsSpan().TrimEnd(UrlSeparator);
        var seg2 = uri2.AsSpan().Trim(UrlSeparator);
        var seg3 = uri3.AsSpan().TrimStart(UrlSeparator);

        fixed (
            char*
            f = &MemoryMarshal.GetReference(seg1),
            s = &MemoryMarshal.GetReference(seg2),
            t = &MemoryMarshal.GetReference(seg3)
        )
        {
            return string.Create(
                seg1.Length + 1 + seg2.Length + 1 + seg3.Length,
                new Combine3State(f, seg1.Length, s, seg2.Length, t, seg3.Length),
                static (destination, state) =>
                {
                    new Span<char>(state.First, state.FirstLength).CopyTo(destination);
                    destination[state.FirstLength] = UrlSeparator;
                    new Span<char>(state.Second, state.SecondLength).CopyTo(destination[(state.FirstLength + 1)..]);
                    destination[state.SecondLength] = UrlSeparator;
                    new Span<char>(state.Third, state.ThirdLength).CopyTo(destination[^state.ThirdLength..]);
                }
            );
        }
    }

    private readonly unsafe struct Combine3State
    {
        public Combine3State(char* first, int firstLength, char* second, int secondLength, char* third, int thirdLength)
        {
            First = first;
            FirstLength = firstLength;
            Second = second;
            SecondLength = secondLength;
            Third = third;
            ThirdLength = thirdLength;
        }

        public readonly char* First;
        public readonly int FirstLength;
        public readonly char* Second;
        public readonly int SecondLength;
        public readonly char* Third;
        public readonly int ThirdLength;
    }

    // Implemented from scratch
    public static unsafe string Combine(params string[] uris)
    {
        switch (uris.Length)
        {
            case 0:
                return string.Empty;
            case 1:
                return uris[0];
            case 2:
                return Combine(uris[0], uris[1]);
            case 3:
                return Combine(uris[0], uris[1], uris[2]);
        }

        Span<UnsafeSegment> segs = stackalloc UnsafeSegment[uris.Length];

        var totalStringLength = uris.Length - 1; // start off with the amount of / symbols in the result
        
        var len = uris.Length;
        for (var i = 0; i < len; i++)
        {
            var span = uris[i].AsSpan();

            var trimStart = 0;
            var trimLength = span.Length;

            if (i != 0)
            {
                // trim start

                for (trimStart = 0; i < span.Length; i++)
                {
                    if (span[i] != UrlSeparator)
                    {
                        break;
                    }
                }

            }

            if (i != len - 1)
            {
                // trim end
                
                for (trimLength = span.Length - 1; i >= 0; i--)
                {
                    if (span[i] != UrlSeparator)
                    {
                        break;
                    }
                }
            }

            segs[i] = new UnsafeSegment(trimStart, trimLength);

            totalStringLength += trimLength;
        }

        fixed (UnsafeSegment* segs1 = segs)
        {
            return string.Create(
                totalStringLength,
                new CombineNState(uris, segs1, segs.Length),
                static (destination, state) =>
                {
                    for (var i = 0; i < state.SegsLength; i++)
                    {
                        var seg = state.Segs[i];
                        var span = state.Uris[i].AsSpan(seg.Start, seg.Length);
                        
                        span.CopyTo(destination);

                        if (i != state.SegsLength - 1)
                        {
                            destination[span.Length] = UrlSeparator;
                            destination = destination[(span.Length+1)..];
                        }
                    }
                }
            );
        }
    }
    
    private readonly struct UnsafeSegment
    {
        public readonly int Start;
        public readonly int Length;

        public UnsafeSegment(int start, int length)
        {
            Start = start;
            Length = length;
        }
    }

    private readonly unsafe struct CombineNState
    {
        public readonly string[] Uris;
        public readonly UnsafeSegment* Segs;
        public readonly int SegsLength;

        public CombineNState(string[] uris, UnsafeSegment* segs, int segsLength)
        {
            Uris = uris;
            Segs = segs;
            SegsLength = segsLength;
        }
    }
}