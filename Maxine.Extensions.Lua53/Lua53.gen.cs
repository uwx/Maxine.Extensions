using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua53.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua53.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua53.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua53.lua_State*, Maxine.Extensions.Lua53.lua_Debug*, void>;

// ReSharper disable All

using System.Runtime.InteropServices;

namespace Maxine.Extensions.Lua53
{
    public static unsafe partial class Lua53
    {
        [DllImport("lua536", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaopen_base(lua_State* L);

        [DllImport("lua536", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaopen_coroutine(lua_State* L);

        [DllImport("lua536", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaopen_table(lua_State* L);

        [DllImport("lua536", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaopen_io(lua_State* L);

        [DllImport("lua536", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaopen_os(lua_State* L);

        [DllImport("lua536", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaopen_string(lua_State* L);

        [DllImport("lua536", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaopen_utf8(lua_State* L);

        [DllImport("lua536", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaopen_bit32(lua_State* L);

        [DllImport("lua536", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaopen_math(lua_State* L);

        [DllImport("lua536", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaopen_debug(lua_State* L);

        [DllImport("lua536", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaopen_package(lua_State* L);

        /// <summary>
        /// [-0, +0, e]<br/>
        /// <br/>
        /// Opens all standard Lua libraries into the given state.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_openlibs (lua_State *L);
        /// </code>
        [DllImport("lua536", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaL_openlibs(lua_State* L);
    }
}
