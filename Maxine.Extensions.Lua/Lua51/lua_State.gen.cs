using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua51.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua51.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua51.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua51.lua_State*, Maxine.Extensions.Lua51.lua_Debug*, void>;

// ReSharper disable All

namespace Maxine.Extensions.Lua51
{
    /// <summary>
    /// Opaque structure that keeps the whole state of a Lua interpreter. The Lua library is fully reentrant: it has no global variables. All information about a state is kept in this structure.
    /// A pointer to this state must be passed as the first argument to every function in the library, except to lua_newstate, which creates a Lua state from scratch.
    /// </summary>
    /// <code>
    /// typedef struct lua_State lua_State;
    /// </code>
    public partial struct lua_State
    {
    }
}
