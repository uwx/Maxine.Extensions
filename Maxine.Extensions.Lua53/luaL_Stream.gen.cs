using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua53.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua53.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua53.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua53.lua_State*, Maxine.Extensions.Lua53.lua_Debug*, void>;

// ReSharper disable All

namespace Maxine.Extensions.Lua53
{
    /// <summary>
    /// The standard representation for file handles, which is used by the standard I/O library.<br/>
    /// <br/>
    /// A file handle is implemented as a full userdata, with a metatable called LUA_FILEHANDLE (where LUA_FILEHANDLE is a macro with the actual metatable's name). The metatable is created by the I/O library (see luaL_newmetatable).<br/>
    /// <br/>
    /// This userdata must start with the structure luaL_Stream; it can contain other data after this initial structure. Field f points to the corresponding C stream (or it can be NULL to indicate an incompletely created handle). Field closef points to a Lua function that will be called to close the stream when the handle is closed or collected; this function receives the file handle as its sole argument and must return either true (in case of success) or nil plus an error message (in case of error). Once Lua calls this field, it changes the field value to NULL to signal that the handle is closed.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// typedef struct luaL_Stream {
    /// FILE *f;
    /// lua_CFunction closef;
    /// } luaL_Stream;
    /// </code>
    public unsafe partial struct luaL_Stream
    {
        [NativeTypeName("FILE *")]
        public void* f;

        public lua_CFunction closef;
    }
}
