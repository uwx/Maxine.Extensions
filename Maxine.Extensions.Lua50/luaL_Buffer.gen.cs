using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua50.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua50.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua50.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua50.lua_State*, Maxine.Extensions.Lua50.lua_Debug*, void>;

// ReSharper disable All

using System.Runtime.CompilerServices;

namespace Maxine.Extensions.Lua50
{
    public unsafe partial struct luaL_Buffer
    {
        [NativeTypeName("char *")]
        public byte* p;

        public int lvl;

        public lua_State* L;

        [NativeTypeName("char[512]")]
        public _buffer_e__FixedBuffer buffer;

        [InlineArray(512)]
        public partial struct _buffer_e__FixedBuffer
        {
            public byte e0;
        }
    }
}
