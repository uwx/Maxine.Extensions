using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua51.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua51.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua51.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua51.lua_State*, Maxine.Extensions.Lua51.lua_Debug*, void>;

// ReSharper disable All

namespace Maxine.Extensions.Lua51
{
    /// <summary>
    /// Type for arrays of functions to be registered by luaL_register. name is the function name and func is a pointer to the function. Any array of luaL_Reg must end with an sentinel entry in which both name and func are NULL.
    /// </summary>
    /// <code>
    /// typedef struct luaL_Reg {
    /// const char *name;
    /// lua_CFunction func;
    /// } luaL_Reg;
    /// </code>
    public unsafe partial struct luaL_Reg
    {
        [NativeTypeName("const char *")]
        public byte* name;

        public lua_CFunction func;
    }
}
