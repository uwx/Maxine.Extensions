using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua50.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua50.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua50.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua50.lua_State*, Maxine.Extensions.Lua50.lua_Debug*, void>;

// ReSharper disable All

namespace Maxine.Extensions.Lua50
{
    public unsafe partial struct luaL_reg
    {
        [NativeTypeName("const char *")]
        public byte* name;

        public lua_CFunction func;
    }
}
