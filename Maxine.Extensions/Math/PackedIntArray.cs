/*
  SharpLab tools in Run mode:
    • value.Inspect()
    • Inspect.Heap(object)
    • Inspect.Stack(value)
    • Inspect.MemoryGraph(value1, value2, …)
using System;

var _bitsPerElement = 3;
byte[] bytes = [0b00100101, 0b10000000, 0b00000000, 0b00000000];

var index = 2;
var bitIndex = (index * _bitsPerElement);
var startingByteIndex = bitIndex / 8;
var headBits = bitIndex - (startingByteIndex * 8);
var tailBits = (headBits + _bitsPerElement) % 8;
var overlap = headBits + _bitsPerElement > 8;
Console.WriteLine($"bitIndex: {bitIndex}");
Console.WriteLine($"startingByteIndex: {startingByteIndex}");
Console.WriteLine($"headBits: {headBits}");
Console.WriteLine($"overlap: {overlap}");
Console.WriteLine($"tailBits: {tailBits}");

// read
{
    var v = bytes[startingByteIndex];
    v <<= headBits;
    v >>= headBits;

    if (tailBits != 0) {
        v <<= tailBits;

        var v2 = bytes[startingByteIndex + 1];
        v2 >>= 8 - tailBits;
        Console.WriteLine($"{v | v2}");
    } else {
        Console.WriteLine($"{v}");
    }
}

// write
var newValue = 7;

{
    var v = bytes[startingByteIndex];
    
    //zero the bits we want to replace in the existing bytes
    v &= (byte)~(newValue << headBits);
    v |= (newValue << headBits);
}

 */

#if UNFINISHED
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maxine.Extensions
{
    public unsafe struct PackedNibbleArray<T> where T : unmanaged, IBinaryInteger<T>
    {
        private byte[] _bytes;
        private int _len;
        private bool _isLittleEndian;

        private int _bitsPerElement;

        public T this[int index]
        {
            get
            {
                var bitIndex = (index * _bitsPerElement);
                var startingByteIndex = bitIndex / 8;
                var overlap = (bitIndex + 8) / 8;
            }
            set
            {
                
            }
        }

        public PackedNibbleArray(int bitsPerElement, int length)
        {
            _bitsPerElement = bitsPerElement;
            _len = length;
            var (div, rem) = Math.DivRem((length * bitsPerElement), 8);
            _bytes = new byte[div + (rem > 0 ? 1 : 0)]; // ? ((bitsPerElement / 8) + (bitsPerElement % 8 + 1))
        }
        
        private T Get(byte[] bytes, int index, byte bit_offset) {
            if (int_bits == 0) return 0;

            var bit_index = (index * int_bits) + bit_offset;
            var max_end_byte = (bit_index + max_io_bits) / 8;

            //using the larger container size will potentially read out of bounds
            if (max_end_byte > bytes.Length) return getBits(bytes, MinIo, bit_index);
            return getBits(bytes, MaxIo, bit_index);
        }

        private T getBits<TContainer>(byte[] bytes, int bit_index) where TContainer : unmanaged, IBinaryInteger<TContainer> {
            var container_bits = sizeof(TContainer) * 8;

            var start_byte = bit_index / 8;
            var head_keep_bits = bit_index - (start_byte * 8);
            var tail_keep_bits = container_bits - (int_bits + head_keep_bits);

            //read bytes as container
            var value_ptr = MemoryMarshal.Cast<byte, TContainer>(bytes.AsSpan(start_byte));
            var value = value_ptr[0];

            if (_isLittleEndian != BitConverter.IsLittleEndian) value = ReverseEndianness(value);

            switch (_isLittleEndian) {
                case false:
                    value <<= head_keep_bits;
                    value >>= head_keep_bits;
                    value >>= tail_keep_bits;
                    break;
                case true:
                    value <<= tail_keep_bits;
                    value >>= tail_keep_bits;
                    value >>= head_keep_bits;
                    break;
            }

            return T.CreateTruncating(value);
        }

        private void Set(int index, byte bitOffset, T value)
        {
            if (int_bits == 0) return;

            var bit_index = (index * int_bits) + bitOffset;
            var max_end_byte = (bit_index + max_io_bits) / 8;

            //using the larger container size will potentially write out of bounds
            if (max_end_byte > _bytes.Length) return setBits(_bytes, MinIo, bit_index, value);
            setBits(_bytes, MaxIo, bit_index, value);
        }
        
        private void setBits<TContainer>(byte[] bytes, int bit_index, int val) where TContainer : unmanaged, IBinaryInteger<TContainer>
        {
            var container_bits = sizeof(TContainer) * 8;
            var Shift = BitOperations.Log2((uint)sizeof(TContainer));

            var start_byte = bit_index / 8;
            var head_keep_bits = bit_index - (start_byte * 8);
            var tail_keep_bits = container_bits - (int_bits + head_keep_bits);
            var keep_shift = _isLittleEndian switch {
                false => tail_keep_bits,
                true => head_keep_bits,
            };

            //position the bits where they need to be in the container
            var value = TContainer.CreateTruncating(val) << keep_shift;

            //read existing bytes
            var target_ptr = MemoryMarshal.Cast<byte, TContainer>(bytes.AsSpan(start_byte));
            var target = target_ptr[0];

            if (_isLittleEndian != BitConverter.IsLittleEndian) target = ReverseEndianness(target);

            //zero the bits we want to replace in the existing bytes
            var inv_mask = TContainer.CreateTruncating(val) << keep_shift;
            var mask = ~inv_mask;
            target &= mask;

            //merge the new value
            target |= value;

            if (_isLittleEndian != BitConverter.IsLittleEndian) target = ReverseEndianness(target);

            //save it back
            target_ptr[0] = target;
        }

        private TContainer ReverseEndianness<TContainer>(TContainer target) where TContainer : unmanaged, IBinaryInteger<TContainer>
        {
            if (typeof(TContainer) == typeof(UIntPtr)) return (TContainer)(object)ReverseEndianness((UIntPtr)(object)target);
            if (typeof(TContainer) == typeof(long)) return (TContainer)(object)ReverseEndianness((long)(object)target);
            if (typeof(TContainer) == typeof(uint)) return (TContainer)(object)ReverseEndianness((uint)(object)target);
            if (typeof(TContainer) == typeof(ushort)) return (TContainer)(object)ReverseEndianness((ushort)(object)target);
            if (typeof(TContainer) == typeof(UInt128)) return (TContainer)(object)ReverseEndianness((UInt128)(object)target);
            if (typeof(TContainer) == typeof(sbyte)) return (TContainer)(object)ReverseEndianness((sbyte)(object)target);
            if (typeof(TContainer) == typeof(IntPtr)) return (TContainer)(object)ReverseEndianness((IntPtr)(object)target);
            if (typeof(TContainer) == typeof(ulong)) return (TContainer)(object)ReverseEndianness((ulong)(object)target);
            if (typeof(TContainer) == typeof(int)) return (TContainer)(object)ReverseEndianness((int)(object)target);
            if (typeof(TContainer) == typeof(short)) return (TContainer)(object)ReverseEndianness((short)(object)target);
            if (typeof(TContainer) == typeof(Int128)) return (TContainer)(object)ReverseEndianness((Int128)(object)target);
            if (typeof(TContainer) == typeof(byte)) return (TContainer)(object)ReverseEndianness((byte)(object)target);
            throw new InvalidOperationException();
        }
    }
}
#endif