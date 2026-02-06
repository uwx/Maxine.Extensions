using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.LuaJIT.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.LuaJIT.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.LuaJIT.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.LuaJIT.lua_State*, Maxine.Extensions.LuaJIT.lua_Debug*, void>;

// ReSharper disable All

using System.Runtime.InteropServices;

namespace Maxine.Extensions.LuaJIT
{
    public static unsafe partial class LuaJIT
    {
        public const int LUAJIT_MODE_ENGINE = 0;
        public const int LUAJIT_MODE_DEBUG = 1;
        public const int LUAJIT_MODE_FUNC = 2;
        public const int LUAJIT_MODE_ALLFUNC = 3;
        public const int LUAJIT_MODE_ALLSUBFUNC = 4;
        public const int LUAJIT_MODE_TRACE = 5;
        public const int LUAJIT_MODE_WRAPCFUNC = 0x10;
        public const int LUAJIT_MODE_MAX = 17;

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaJIT_setmode(lua_State* L, int idx, int mode);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaJIT_profile_start(lua_State* L, [NativeTypeName("const char *")] byte* mode, [NativeTypeName("luaJIT_profile_callback")] delegate* unmanaged[Cdecl]<void*, lua_State*, int, int, void> cb, void* data);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaJIT_profile_stop(lua_State* L);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        public static extern byte* luaJIT_profile_dumpstack(lua_State* L, [NativeTypeName("const char *")] byte* fmt, int depth, [NativeTypeName("size_t *")] nuint* len);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaJIT_version_2_1_1739213504();
    }
}
