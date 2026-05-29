using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maxine.Extensions.UnionGen
{
    [StructLayout(LayoutKind.Explicit)]
    internal partial struct IntOrString : IUnion
    {
        [FieldOffset(0)] private readonly sbyte _kind;
        
        [FieldOffset(8)] private readonly int _value1;
        
        [FieldOffset(8)] private readonly string _value2;

        public IntOrString(int value)
        {
            _kind = 1;
            _value1 = value;
        }

        public IntOrString(string value)
        {
            _kind = 2;
            _value2 = value;
        }

        // still needs to exist for IUnion
        public object? Value => _kind switch { 1 => _value1, 2 => _value2, _ => null };

        // access pattern that avoids boxing.
        public bool HasValue => _kind != 0;

        public bool TryGetValue(out int value)
        {
            if (_kind == 1)
            {
                value = _value1;
                return true;
            }

            value = -1;
            return false;
        }

        public bool TryGetValue(out string value)
        {
            if (_kind == 2)
            {
                value = _value2;
                return true;
            }

            value = "";
            return false;
        }
    }
}

#if !NET10_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class UnionAttribute : Attribute;

    public interface IUnion
    {
        object? Value { get; }
    }
}
#endif