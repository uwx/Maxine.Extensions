using System.Collections;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Maxine.Extensions;

public readonly struct ArrayOfVector2 : IEnumerable<Vector2>
{
    private readonly byte[] _data;
    public unsafe int Length => _data.Length / sizeof(Vector2);
    
    public Span<float> X => MemoryMarshal.Cast<byte, float>(_data.AsSpan())[..Length];
    public Span<float> Y => MemoryMarshal.Cast<byte, float>(_data.AsSpan())[Length..];

    public Vector2 this[int index]
    {
        get
        {
            var floats = MemoryMarshal.Cast<byte, float>(_data.AsSpan());
            return new Vector2(floats[index], floats[index + Length]);
        }
        set
        {
            var floats = MemoryMarshal.Cast<byte, float>(_data.AsSpan());
            floats[index] = value.X;
            floats[index + Length] = value.Y;
        }
    }

    public unsafe ArrayOfVector2(int length)
    {
        _data = new byte[length * sizeof(Vector2)];
    }

    public ArrayOfVector2Enumerator GetEnumerator() => new(this);
    IEnumerator<Vector2> IEnumerable<Vector2>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct ArrayOfVector2Enumerator : IEnumerator<Vector2>
    {
        private readonly ArrayOfVector2 _arr;
        private int _index;

        internal ArrayOfVector2Enumerator(ArrayOfVector2 arr)
        {
            _arr = arr;
            _index = -1;
        }

        public bool MoveNext()
        {
            _index++;
            return _index < _arr.Length;
        }

        public void Reset()
        {
            _index = 0;
        }

        public readonly Vector2 Current => _arr[_index];

        readonly object IEnumerator.Current => Current;
        
        public readonly void Dispose()
        {
        }
    }
}