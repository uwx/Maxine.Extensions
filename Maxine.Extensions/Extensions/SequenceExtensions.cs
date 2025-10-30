using System.Buffers;
using System.Runtime.CompilerServices;

namespace Maxine.Extensions
{
    public static class SequenceExtensions
    {
        public static SequencePosition? PositionOf<T>(in this ReadOnlySequence<T> sequence, ReadOnlySpan<T> search) where T : unmanaged, IEquatable<T>
        {
            if (search.Length == 0) return new SequencePosition(sequence.First, 0);
            if (search.Length > sequence.Length) return null;
            if (sequence.Length == 0) return null;
            
            if (sequence.IsSingleSegment)
            {
                var idx = sequence.First.Span.IndexOf(search);
                return idx != -1 ? sequence.GetPosition(idx) : null;
            }

            if (search.Length == 1)
            {
                return sequence.PositionOf(search[0]);
            }

            // var position = sequence.Start;
            // var oldPosition = position;
            // while (sequence.TryGet(ref position, out var memory))
            // {
            //     var span = memory.Span;
            //     var startIdx = 0;
            //
            //     while (startIdx < span.Length)
            //     {
            //         var idx = span[startIdx..].IndexOf(search[0]);
            //         if (idx == -1)
            //         {
            //             break;
            //         }
            //
            //         if (IsEqual(sequence, span[(startIdx + idx + 1)..], position, search[1..]))
            //         {
            //             return sequence.GetPosition(startIdx + idx, oldPosition);
            //         }
            //             
            //         startIdx += idx + 1;
            //     }
            //
            //     oldPosition = position;
            //     if (position.GetObject() == null)
            //     {
            //         break;
            //     }
            // }
            //
            // return null;
            //
            // static bool IsEqual(in ReadOnlySequence<T> sequence, ReadOnlySpan<T> currentMemory, SequencePosition position, ReadOnlySpan<T> search)
            // {
            //     // try to match the remainder of the current span to the rest of search 
            //     if (search.Length <= currentMemory.Length)
            //     {
            //         return currentMemory.StartsWith(search);
            //     }
            //
            //     if (!currentMemory.SequenceEqual(search[..currentMemory.Length]))
            //     {
            //         return false;
            //     }
            //
            //     // if there are no more entries, give up
            //     if (position.GetObject() == null)
            //     {
            //         return false;
            //     }
            //     
            //     // trim away the part of the span we've found so far
            //     search = search[currentMemory.Length..];
            //     
            //     // keep going in the sequence
            //     while (sequence.TryGet(ref position, out var memory))
            //     {
            //         var span = memory.Span;
            //         
            //         // try to match the remainder of the current span to the rest of search 
            //         if (search.Length <= span.Length)
            //         {
            //             return currentMemory.StartsWith(search);
            //         }
            //
            //         if (!span.SequenceEqual(search[..span.Length]))
            //         {
            //             return false;
            //         }
            //
            //         // if there are no more entries, give up
            //         if (position.GetObject() == null)
            //         {
            //             return false;
            //         }
            //         
            //         // trim away the part of the span we've found so far
            //         search = search[span.Length..];
            //     }
            //
            //     return false;
            // }

            // old impl:
            var reader = new SequenceReader<T>(sequence);
            
            // Span-based fast path
            unsafe
            {
                if (search.Length * sizeof(T) < 1024)
                {
                    Span<T> temp = stackalloc T[search.Length];
                    
                    while (reader.TryAdvanceTo(search[0], false))
                    {
                        var readerClone = reader;
            
                        if (readerClone.TryReadExact(search.Length, out var seq))
                        {
                            seq.CopyTo(temp);
                            if (temp.SequenceEqual(search))
                            {
                                return reader.Position;
                            }
                        }
                        else
                        {
                            return null; // Reached the end of the stream :(
                        }
            
                        reader.Advance(1);
                    }
                }
            }
            
            // Element-by-element slow path
            {
                while (reader.TryAdvanceTo(search[0], false))
                {
                    for (var i = 1; i < search.Length; i++)
                    {
                        if (!reader.TryPeek(i, out var peek)) goto nextIteration;
                        if (!peek.Equals(search[i])) goto nextIteration;
                    }
            
                    return reader.Position;
            
                    nextIteration:
                    reader.Advance(1);
                    ;
                }
            }
            
            return null;
        }
    }
}