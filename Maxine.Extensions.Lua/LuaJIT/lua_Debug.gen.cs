using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.LuaJIT.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.LuaJIT.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.LuaJIT.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.LuaJIT.lua_State*, Maxine.Extensions.LuaJIT.lua_Debug*, void>;

// ReSharper disable All

using System.Runtime.CompilerServices;

namespace Maxine.Extensions.LuaJIT
{
    public unsafe partial struct lua_Debug
    {
        public int @event;

        [NativeTypeName("const char *")]
        public byte* name;

        [NativeTypeName("const char *")]
        public byte* namewhat;

        [NativeTypeName("const char *")]
        public byte* what;

        [NativeTypeName("const char *")]
        public byte* source;

        public int currentline;

        public int nups;

        public int linedefined;

        public int lastlinedefined;

        [NativeTypeName("char[60]")]
        public _short_src_e__FixedBuffer short_src;

        public int i_ci;

        [InlineArray(60)]
        public partial struct _short_src_e__FixedBuffer
        {
            public byte e0;
        }
    }
}
