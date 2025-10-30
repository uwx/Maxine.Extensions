using System.Collections;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Maxine.Extensions.Collections;

public readonly struct ArrayOfVector3 : IEnumerable<Vector3>
{
    private readonly byte[] _data;
    public unsafe int Length => _data.Length / sizeof(Vector3);
    
    public Span<float> X => MemoryMarshal.Cast<byte, float>(_data.AsSpan())[..Length];
    public Span<float> Y => MemoryMarshal.Cast<byte, float>(_data.AsSpan())[Length..(Length * 2)];
    public Span<float> Z => MemoryMarshal.Cast<byte, float>(_data.AsSpan())[(Length * 2)..];

    public Vector3 this[int index]
    {
        get
        {
            var floats = MemoryMarshal.Cast<byte, float>(_data.AsSpan());
            return new Vector3(floats[index], floats[index + Length], floats[index + (Length * 2)]);
        }
        set
        {
            var floats = MemoryMarshal.Cast<byte, float>(_data.AsSpan());
            floats[index] = value.X;
            floats[index + Length] = value.Y;
            floats[index + (Length * 2)] = value.Z;
        }
    }

    public unsafe ArrayOfVector3(int length)
    {
        _data = new byte[length * sizeof(Vector3)];
    }

    public ArrayOfVector3Enumerator GetEnumerator() => new(this);
    IEnumerator<Vector3> IEnumerable<Vector3>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct ArrayOfVector3Enumerator : IEnumerator<Vector3>
    {
        private readonly ArrayOfVector3 _arr;
        private int _index;

        internal ArrayOfVector3Enumerator(ArrayOfVector3 arr)
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

        public readonly Vector3 Current => _arr[_index];

        readonly object IEnumerator.Current => Current;
        
        public readonly void Dispose()
        {
        }
    }
}