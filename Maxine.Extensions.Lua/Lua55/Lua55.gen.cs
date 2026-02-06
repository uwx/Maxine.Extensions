using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua55.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua55.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua55.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua55.lua_State*, Maxine.Extensions.Lua55.lua_Debug*, void>;

// ReSharper disable All

using System.Runtime.InteropServices;

namespace Maxine.Extensions.Lua55
{
    public static unsafe partial class Lua55
    {
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaopen_base(lua_State* L);

        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaopen_package(lua_State* L);

        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaopen_coroutine(lua_State* L);

        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaopen_debug(lua_State* L);

        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaopen_io(lua_State* L);

        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaopen_math(lua_State* L);

        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaopen_os(lua_State* L);

        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaopen_string(lua_State* L);

        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaopen_table(lua_State* L);

        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaopen_utf8(lua_State* L);

        /// <summary>
        /// [-0, +0, e]
        /// Opens (loads) and preloads selected standard libraries into the state L. (To preload means to add the library loader into the table package.preload, so that the library can be required later by the program. Keep in mind that require itself is provided by the package library. If a program does not load that library, it will be unable to require anything.)
        /// The integer load selects which libraries to load; the integer preload selects which to preload, among those not loaded. Both are masks formed by a bitwise OR of the following constants:
        /// - LUA_GLIBK : the basic library.
        /// - LUA_LOADLIBK : the package library.
        /// - LUA_COLIBK : the coroutine library.
        /// - LUA_STRLIBK : the string library.
        /// - LUA_UTF8LIBK : the UTF-8 library.
        /// - LUA_TABLIBK : the table library.
        /// - LUA_MATHLIBK : the mathematical library.
        /// - LUA_IOLIBK : the I/O library.
        /// - LUA_OSLIBK : the operating system library.
        /// - LUA_DBLIBK : the debug library.
        /// </summary>
        /// <code>
        /// void luaL_openselectedlibs (lua_State *L, int load, int preload);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaL_openselectedlibs(lua_State* L, int load, int preload);
    }
}
