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
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_openlib(lua_State* L, [NativeTypeName("const char *")] byte* libname, [NativeTypeName("const luaL_Reg *")] luaL_Reg* l, int nup);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_register(lua_State* L, [NativeTypeName("const char *")] byte* libname, [NativeTypeName("const luaL_Reg *")] luaL_Reg* l);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_getmetafield(lua_State* L, int obj, [NativeTypeName("const char *")] byte* e);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_callmeta(lua_State* L, int obj, [NativeTypeName("const char *")] byte* e);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_typerror(lua_State* L, int narg, [NativeTypeName("const char *")] byte* tname);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_argerror(lua_State* L, int numarg, [NativeTypeName("const char *")] byte* extramsg);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_checklstring(lua_State* L, int numArg, [NativeTypeName("size_t *")] nuint* l);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_optlstring(lua_State* L, int numArg, [NativeTypeName("const char *")] byte* def, [NativeTypeName("size_t *")] nuint* l);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Number luaL_checknumber(lua_State* L, int numArg);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Number luaL_optnumber(lua_State* L, int nArg, lua_Number def);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Integer luaL_checkinteger(lua_State* L, int numArg);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Integer luaL_optinteger(lua_State* L, int nArg, lua_Integer def);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_checkstack(lua_State* L, int sz, [NativeTypeName("const char *")] byte* msg);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_checktype(lua_State* L, int narg, int t);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_checkany(lua_State* L, int narg);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_newmetatable(lua_State* L, [NativeTypeName("const char *")] byte* tname);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void* luaL_checkudata(lua_State* L, int ud, [NativeTypeName("const char *")] byte* tname);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_where(lua_State* L, int lvl);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_checkoption(lua_State* L, int narg, [NativeTypeName("const char *")] byte* def, [NativeTypeName("const char *const[]")] byte** lst);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_ref(lua_State* L, int t);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_unref(lua_State* L, int t, int @ref);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_loadfile(lua_State* L, [NativeTypeName("const char *")] byte* filename);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_loadbuffer(lua_State* L, [NativeTypeName("const char *")] byte* buff, [NativeTypeName("size_t")] nuint sz, [NativeTypeName("const char *")] byte* name);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_loadstring(lua_State* L, [NativeTypeName("const char *")] byte* s);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern lua_State* luaL_newstate();

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_gsub(lua_State* L, [NativeTypeName("const char *")] byte* s, [NativeTypeName("const char *")] byte* p, [NativeTypeName("const char *")] byte* r);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_findtable(lua_State* L, int idx, [NativeTypeName("const char *")] byte* fname, int szhint);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_fileresult(lua_State* L, int stat, [NativeTypeName("const char *")] byte* fname);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_execresult(lua_State* L, int stat);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_loadfilex(lua_State* L, [NativeTypeName("const char *")] byte* filename, [NativeTypeName("const char *")] byte* mode);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_loadbufferx(lua_State* L, [NativeTypeName("const char *")] byte* buff, [NativeTypeName("size_t")] nuint sz, [NativeTypeName("const char *")] byte* name, [NativeTypeName("const char *")] byte* mode);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaL_traceback(lua_State* L, lua_State* L1, [NativeTypeName("const char *")] byte* msg, int level);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_setfuncs(lua_State* L, [NativeTypeName("const luaL_Reg *")] luaL_Reg* l, int nup);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_pushmodule(lua_State* L, [NativeTypeName("const char *")] byte* modname, int sizehint);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void* luaL_testudata(lua_State* L, int ud, [NativeTypeName("const char *")] byte* tname);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_setmetatable(lua_State* L, [NativeTypeName("const char *")] byte* tname);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_buffinit(lua_State* L, luaL_Buffer* B);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_prepbuffer(luaL_Buffer* B);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_addlstring(luaL_Buffer* B, [NativeTypeName("const char *")] byte* s, [NativeTypeName("size_t")] nuint l);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_addstring(luaL_Buffer* B, [NativeTypeName("const char *")] byte* s);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_addvalue(luaL_Buffer* B);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_pushresult(luaL_Buffer* B);
    }
}
