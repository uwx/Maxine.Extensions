using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maxine.Extensions.Streams;

public ref struct SpanReader
{
#if NET8_0_OR_GREATER
	[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
	private static extern decimal CreateDecimal(int lo, int mid, int hi, int flags);

	private static decimal ToDecimal(ReadOnlySpan<byte> span)
	{
		Debug.Assert(span.Length >= 16, "span.Length >= 16");
		var lo = BinaryPrimitives.ReadInt32LittleEndian(span);
		var mid = BinaryPrimitives.ReadInt32LittleEndian(span[4..]);
		var hi = BinaryPrimitives.ReadInt32LittleEndian(span[8..]);
		var flags = BinaryPrimitives.ReadInt32LittleEndian(span[12..]);
		return CreateDecimal(lo, mid, hi, flags);
	}
#endif

	#region Read Primitive
	
	/// <summary>
	/// Reads a boolean value from the current binary stream and advances the current position within the stream by one byte.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ReadBoolean() => InternalReadByte() != 0;

	/// <summary>
	/// Reads the next byte from the current binary stream and advances the current position within the stream by one byte.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte ReadByte() => InternalReadByte();

	/// <summary>
	/// Reads a decimal value from the current binary stream and advances the current position within the stream by sixteen bytes.
	/// </summary>
	public unsafe decimal ReadDecimal()
	{
		var span = InternalReadSpan(16);
		try
		{
			if (BitConverter.IsLittleEndian)
			{
				
				return new decimal(MemoryMarshal.Cast<byte, int>(span[..16]));
				/*[
					Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span)),      // lo
					Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span[4..])), // mid
					Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span[8..])), // hi
					Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span[12..])) // flags
				]*/
			}
			else
			{
				return new decimal([
					BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span))),      // lo
					BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span[4..]))), // mid
					BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span[8..]))), // hi
					BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span[12..]))) // flags
				]);
			}
		}
		catch (ArgumentException e)
		{
			// ReadDecimal cannot leak out ArgumentException
			throw ExceptionHelper.DecimalReadingException(e);
		}
	}

	/// <summary>
	/// Reads single-precision floating-point number from the current binary stream and advances the current position within the stream by four bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float ReadSingle()
	{
		var span = InternalReadSpan(4);
		return MemoryMarshal.Read<float>(span);
	}

	/// <summary>
	/// Reads a double-precision floating-point number from the current binary stream and advances the current position within the stream by eight bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double ReadDouble()
	{
		var span = InternalReadSpan(8);
		return MemoryMarshal.Read<double>(span);
	}

	/// <summary>
	/// Reads a 16-bit signed integer from the current binary stream and advances the current position within the stream by two bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public short ReadInt16()
	{
		var span = InternalReadSpan(2);
		return MemoryMarshal.Read<short>(span);
	}

	/// <summary>
	/// Reads a 32-bit signed integer from the current binary stream and advances the current position within the stream by four bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ReadInt32()
	{
		var span = InternalReadSpan(4);
		return MemoryMarshal.Read<int>(span);
	}

	/// <summary>
	/// Reads a 64-bit signed integer from the current binary stream and advances the current position within the stream by eight bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public long ReadInt64()
	{
		var span = InternalReadSpan(8);
		return MemoryMarshal.Read<long>(span);
	}
	
	/// <summary>
	/// Reads a signed byte from the current binary stream and advances the current position within the stream by one byte.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sbyte ReadSByte() => (sbyte)InternalReadByte();

	/// <summary>
	/// Reads a 16-bit unsigned integer from the current binary stream and advances the current position within the stream by two bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ushort ReadUInt16()
	{
		var span = InternalReadSpan(2);
		return MemoryMarshal.Read<ushort>(span);
	}

	/// <summary>
	/// Reads a 32-bit unsigned integer from the current binary stream and advances the current position within the stream by four bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public uint ReadUInt32()
	{
		var span = InternalReadSpan(4);
		return MemoryMarshal.Read<uint>(span);
	}

	/// <summary>
	/// Reads 64-bit unsigned integer from the current binary stream and advances the current position within the stream by eight bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ulong ReadUInt64()
	{
		var span = InternalReadSpan(8);
		return MemoryMarshal.Read<ulong>(span);
	}
	
	#endregion

	#region Try Read Primitive
	
	/// <summary>
	/// Reads a boolean value from the current binary stream and advances the current position within the stream by one byte.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryReadBoolean(out bool result)
	{
		if (InternalTryReadByte(out var b))
		{
			result = b != 0;
			return true;
		}
		result = default;
		return false;
	}

	/// <summary>
	/// Reads the next byte from the current binary stream and advances the current position within the stream by one byte.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryReadByte(out byte result) => InternalTryReadByte(out result);

	/// <summary>
	/// Reads a decimal value from the current binary stream and advances the current position within the stream by sixteen bytes.
	/// </summary>
	public unsafe bool TryReadDecimal(out decimal result)
	{
		if (!InternalTryReadSpan(16, out var span))
		{
			result = default;
			return false;
		} 

		try
		{
			if (BitConverter.IsLittleEndian)
			{
				result = new decimal(MemoryMarshal.Cast<byte, int>(span[..16]));
				/*[
					Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span)),      // lo
					Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span[4..])), // mid
					Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span[8..])), // hi
					Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span[12..])) // flags
				]*/
				return true;
			}
			else
			{
				result = new decimal([
					BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span))),      // lo
					BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span[4..]))), // mid
					BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span[8..]))), // hi
					BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span[12..]))) // flags
				]);
				return true;
			}
		}
		catch (ArgumentException e)
		{
			// ReadDecimal cannot leak out ArgumentException
			throw ExceptionHelper.DecimalReadingException(e);
		}
	}

	/// <summary>
	/// Reads single-precision floating-point number from the current binary stream and advances the current position within the stream by four bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryReadSingle(out float result)
	{
		if (!InternalTryReadSpan(sizeof(float), out var span))
		{
			result = default;
			return false;
		}
		result = MemoryMarshal.Read<float>(span);
		return true;
	}

	/// <summary>
	/// Reads a double-precision floating-point number from the current binary stream and advances the current position within the stream by eight bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryReadDouble(out double result)
	{
		if (!InternalTryReadSpan(sizeof(double), out var span))
		{
			result = default;
			return false;
		}
		result = MemoryMarshal.Read<double>(span);
		return true;
	}

	/// <summary>
	/// Reads a 16-bit signed integer from the current binary stream and advances the current position within the stream by two bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryReadInt16(out short result)
	{
		if (!InternalTryReadSpan(sizeof(short), out var span))
		{
			result = default;
			return false;
		}
		result = MemoryMarshal.Read<short>(span);
		return true;
	}

	/// <summary>
	/// Reads a 32-bit signed integer from the current binary stream and advances the current position within the stream by four bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryReadInt32(out int result)
	{
		if (!InternalTryReadSpan(sizeof(int), out var span))
		{
			result = default;
			return false;
		}
		result = MemoryMarshal.Read<int>(span);
		return true;
	}

	/// <summary>
	/// Reads a 64-bit signed integer from the current binary stream and advances the current position within the stream by eight bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryReadInt64(out long result)
	{
		if (!InternalTryReadSpan(sizeof(long), out var span))
		{
			result = default;
			return false;
		}
		result = MemoryMarshal.Read<long>(span);
		return true;
	}
	
	/// <summary>
	/// Reads a signed byte from the current binary stream and advances the current position within the stream by one byte.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryReadSByte(out sbyte result)
	{
		if (InternalTryReadByte(out var b))
		{
			result = (sbyte)b;
			return true;
		}
		result = default;
		return false;
	}

	/// <summary>
	/// Reads a 16-bit unsigned integer from the current binary stream and advances the current position within the stream by two bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryReadUInt16(out ushort result)
	{
		if (!InternalTryReadSpan(sizeof(ushort), out var span))
		{
			result = default;
			return false;
		}
		result = MemoryMarshal.Read<ushort>(span);
		return true;
	}

	/// <summary>
	/// Reads a 32-bit unsigned integer from the current binary stream and advances the current position within the stream by four bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryReadUInt32(out uint result)
	{
		if (!InternalTryReadSpan(sizeof(uint), out var span))
		{
			result = default;
			return false;
		}
		result = MemoryMarshal.Read<uint>(span);
		return true;
	}

	/// <summary>
	/// Reads 64-bit unsigned integer from the current binary stream and advances the current position within the stream by eight bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryReadUInt64(out ulong result)
	{
		if (!InternalTryReadSpan(8, out var span))
		{
			result = default;
			return false;
		}
		result = MemoryMarshal.Read<ulong>(span);
		return true;
	}
	

	#endregion

	private readonly ReadOnlySpan<byte> _data;
	private int _position;

	/// <summary>
	/// Gets the offset into the underlying <see cref="ReadOnlySpan{T}"/> to start reading from.
	/// </summary>
	public int Offset => 0;

	/// <summary>
	/// Gets the effective length of the readable region of the underlying <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	public int Length => _data.Length;

	/// <summary>
	/// Gets or sets the current reading position within the underlying <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	public int Position
	{
		get => _position;
		set
		{
			if (value < 0) throw ExceptionHelper.PositionLessThanZeroException(nameof(value));
			if (value > _data.Length) throw ExceptionHelper.PositionGreaterThanLengthOfReadOnlyMemoryException(nameof(value));

			_position = value;
		}
	}

	/// <summary>
	/// Gets the amount of bytes remaining until end of the buffer.
	/// </summary>
	public int Remaining => _data.Length - _position;

	/// <summary>
	/// Initializes a new instance of <see cref="BinaryBufferReader"/> based on the specified <see cref="ReadOnlyMemory{T}"/>.
	/// </summary>
	/// <param name="data">The input <see cref="ReadOnlyMemory{T}"/>.</param>
	/// 
	public SpanReader(in ReadOnlySpan<byte> data)
	{
		_data = data;
		_position = 0;
	}

	/// <summary>
	/// Reads the specified number of bytes from the current binary stream into a byte array and advances the current position within the stream by that number of bytes.
	/// </summary>
	/// <param name="count">The number of bytes to read.</param>
	public byte[] ReadBytes(int count) => InternalReadSpan(count).ToArray();

	/// <summary>
	/// Reads a span of bytes from the current binary stream and advances the current position within the stream by the number of bytes read.
	/// </summary>
	/// <param name="count">The number of bytes to read.</param>
	public ReadOnlySpan<byte> ReadSpan(int count) => InternalReadSpan(count);
	
	public ReadOnlySpan<byte> GetRemainingBuffer() => _data[_position..];

	/// <summary>
	/// Reads the specified number of bytes from the current binary stream, starting from a specified point in the byte array.
	/// </summary>
	/// <returns>
	/// The number of bytes read into buffer. This might be less than the number of bytes requested if that many bytes are not available, or it might be zero if the end of the stream is reached.
	/// </returns>
	public int Read(byte[] buffer, int index, int count)
	{
		if (count <= 0)
			return 0;

		var relPos = _position + count;

		if ((uint)relPos > (uint)_data.Length)
		{
			count = relPos - _data.Length;
		}
		if (count <= 0)
			return 0;

		var span = InternalReadSpan(count);
		span.CopyTo(buffer.AsSpan(index, count));

		return count;
	}
	
	/// <summary>
	/// Reads the next byte from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by one byte.
	/// </summary>
	private byte InternalReadByte()
	{
		if (!InternalTryReadByte(out var result))
			ExceptionHelper.EndOfDataException();
		return result;
	}

	private bool InternalTryReadByte(out byte result)
	{
		var curPos = _position;
		var newPos = curPos + 1;

		if ((uint)newPos > (uint)_data.Length)
		{
			_position = _data.Length;
			result = default;
			return false;
		}

		_position = newPos;

		result = _data[curPos];
		return true;
	}

	/// <summary>
	/// Returns a read-only span over the specified number of bytes from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by that number of bytes.
	/// </summary>
	/// <param name="count">The size of the read-only span to return.</param>
	private ReadOnlySpan<byte> InternalReadSpan(int count)
	{
		if (!InternalTryReadSpan(count, out var span))
			ExceptionHelper.EndOfDataException();
		return span;
	}

	/// <summary>
	/// Returns a read-only span over the specified number of bytes from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by that number of bytes.
	/// </summary>
	/// <param name="count">The size of the read-only span to return.</param>
	private bool InternalTryReadSpan(int count, out ReadOnlySpan<byte> span)
	{
		if (count <= 0)
		{
			span = ReadOnlySpan<byte>.Empty;
			return true;
		}

		var curPos = _position;
		var newPos = curPos + count;

		if ((uint)newPos > (uint)_data.Length)
		{
			_position = _data.Length;
			span = default;
			return false;
		}

		_position = newPos;

		span = _data.Slice(curPos, count);
		return true;
	}

	public void Advance(int count)
	{
		var newPos = _position + count;
		if ((uint)newPos > (uint)_data.Length)
		{
			ExceptionHelper.EndOfDataException();
		}

		_position = newPos;
	}
}

file static class ExceptionHelper
{
	public static ArgumentOutOfRangeException PositionLessThanZeroException(string positionParameterName, string positionWord = "Position (zero-based)")
	{
		return new ArgumentOutOfRangeException(positionParameterName, $"{positionWord} must be greater than or equal to zero.");
	}

	public static ArgumentOutOfRangeException PositionGreaterThanLengthOfReadOnlyMemoryException(string positionParameterName)
	{
		return PositionGreaterThanDataStreamLengthException(positionParameterName, "Position (zero-based)", "read-only memory");
	}

	private static ArgumentOutOfRangeException PositionGreaterThanDataStreamLengthException(string positionParameterName, string positionWord, string dataStreamType)
	{
		return new ArgumentOutOfRangeException(positionParameterName, $"{positionWord} must be equal to or less than the size of the underlying {dataStreamType}.");
	}

	public static EndOfStreamException EndOfDataException()
	{
		return new EndOfStreamException("Reached to end of data");
	}

	public static IOException DecimalReadingException(ArgumentException argumentException)
	{
		return new IOException("Failed to read decimal value", argumentException);
	}
}