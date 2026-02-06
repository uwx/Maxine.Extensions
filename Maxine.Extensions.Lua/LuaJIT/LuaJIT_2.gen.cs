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
        public static extern lua_State* lua_newstate(lua_Alloc f, void* ud);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_close(lua_State* L);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_State* lua_newthread(lua_State* L);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_CFunction lua_atpanic(lua_State* L, lua_CFunction panicf);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_gettop(lua_State* L);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_settop(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushvalue(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_remove(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_insert(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_replace(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_checkstack(lua_State* L, int sz);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_xmove(lua_State* from, lua_State* to, int n);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_isnumber(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_isstring(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_iscfunction(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_isuserdata(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_type(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_typename", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_typename(lua_State* L, int tp);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_equal(lua_State* L, int idx1, int idx2);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_rawequal(lua_State* L, int idx1, int idx2);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_lessthan(lua_State* L, int idx1, int idx2);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Number lua_tonumber(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Integer lua_tointeger(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_toboolean(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        public static extern byte* lua_tolstring(lua_State* L, int idx, [NativeTypeName("size_t *")] nuint* len);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        [SuppressGCTransition]
        public static extern nuint lua_objlen(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_CFunction lua_tocfunction(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void* lua_touserdata(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_State* lua_tothread(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const void *")]
        [SuppressGCTransition]
        public static extern void* lua_topointer(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushnil(lua_State* L);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushnumber(lua_State* L, lua_Number n);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushinteger(lua_State* L, lua_Integer n);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushlstring(lua_State* L, [NativeTypeName("const char *")] byte* s, [NativeTypeName("size_t")] nuint l);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushstring(lua_State* L, [NativeTypeName("const char *")] byte* s);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* lua_pushvfstring(lua_State* L, [NativeTypeName("const char *")] byte* fmt, [NativeTypeName("va_list")] byte* argp);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushcclosure(lua_State* L, lua_CFunction fn, int n);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushboolean(lua_State* L, int b);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushlightuserdata(lua_State* L, void* p);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_pushthread(lua_State* L);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_gettable(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_getfield(lua_State* L, int idx, [NativeTypeName("const char *")] byte* k);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_rawget(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_rawgeti(lua_State* L, int idx, int n);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_createtable(lua_State* L, int narr, int nrec);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void* lua_newuserdata(lua_State* L, [NativeTypeName("size_t")] nuint sz);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_getmetatable(lua_State* L, int objindex);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_getfenv(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_settable(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_setfield(lua_State* L, int idx, [NativeTypeName("const char *")] byte* k);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_rawset(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_rawseti(lua_State* L, int idx, int n);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_setmetatable(lua_State* L, int objindex);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_setfenv(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_call(lua_State* L, int nargs, int nresults);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_pcall(lua_State* L, int nargs, int nresults, int errfunc);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_cpcall(lua_State* L, lua_CFunction func, void* ud);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_load(lua_State* L, lua_Reader reader, void* dt, [NativeTypeName("const char *")] byte* chunkname);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_dump(lua_State* L, lua_Writer writer, void* data);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_yield(lua_State* L, int nresults);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_resume(lua_State* L, int narg);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_status(lua_State* L);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_gc(lua_State* L, int what, int data);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_error(lua_State* L);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_next(lua_State* L, int idx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_concat(lua_State* L, int n);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Alloc lua_getallocf(lua_State* L, void** ud);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_setallocf(lua_State* L, lua_Alloc f, void* ud);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_setlevel(lua_State* from, lua_State* to);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_getstack(lua_State* L, int level, lua_Debug* ar);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_getinfo(lua_State* L, [NativeTypeName("const char *")] byte* what, lua_Debug* ar);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_getlocal", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_getlocal(lua_State* L, [NativeTypeName("const lua_Debug *")] lua_Debug* ar, int n);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_setlocal", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_setlocal(lua_State* L, [NativeTypeName("const lua_Debug *")] lua_Debug* ar, int n);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_getupvalue", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_getupvalue(lua_State* L, int funcindex, int n);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_setupvalue", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_setupvalue(lua_State* L, int funcindex, int n);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_sethook(lua_State* L, lua_Hook func, int mask, int count);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Hook lua_gethook(lua_State* L);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_gethookmask(lua_State* L);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_gethookcount(lua_State* L);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void* lua_upvalueid(lua_State* L, int idx, int n);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_upvaluejoin(lua_State* L, int idx1, int n1, int idx2, int n2);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_loadx(lua_State* L, lua_Reader reader, void* dt, [NativeTypeName("const char *")] byte* chunkname, [NativeTypeName("const char *")] byte* mode);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_version", ExactSpelling = true)]
        [return: NativeTypeName("const lua_Number *")]
        [SuppressGCTransition]
        public static extern lua_Number* _lua_version(lua_State* L);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_copy(lua_State* L, int fromidx, int toidx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Number lua_tonumberx(lua_State* L, int idx, int* isnum);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Integer lua_tointegerx(lua_State* L, int idx, int* isnum);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_isyieldable(lua_State* L);
    }
}
