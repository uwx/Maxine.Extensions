using System.Buffers;
using Maxine.Extensions.Io;

namespace Maxine.Extensions;

public static class StreamExtensions
{
    extension(Stream stream)
    {
        public IBufferWriter<T> AsBufferWriter<T>() where T : struct
        {
            return new StreamBufferWriter<T>(stream);
        }

        public StreamArrayPooledReadOnlySequenceProvider AsPooledReadOnlySequence()
        {
            return new StreamArrayPooledReadOnlySequenceProvider(stream);
        }
    }
}