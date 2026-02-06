using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua50.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua50.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua50.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua50.lua_State*, Maxine.Extensions.Lua50.lua_Debug*, void>;

// ReSharper disable All

using System.Runtime.InteropServices;

namespace Maxine.Extensions.Lua50
{
    public static unsafe partial class Lua50
    {
        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern lua_State* lua_open();

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_close(lua_State* L);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_State* lua_newthread(lua_State* L);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_CFunction lua_atpanic(lua_State* L, lua_CFunction panicf);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_gettop(lua_State* L);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_settop(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushvalue(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_remove(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_insert(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_replace(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_checkstack(lua_State* L, int sz);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_xmove(lua_State* from, lua_State* to, int n);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_isnumber(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_isstring(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_iscfunction(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_isuserdata(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_type(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_typename", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_typename(lua_State* L, int tp);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_equal(lua_State* L, int idx1, int idx2);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_rawequal(lua_State* L, int idx1, int idx2);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_lessthan(lua_State* L, int idx1, int idx2);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Number lua_tonumber(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_toboolean(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        public static extern byte* lua_tostring(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint lua_strlen(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_CFunction lua_tocfunction(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void* lua_touserdata(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_State* lua_tothread(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const void *")]
        [SuppressGCTransition]
        public static extern void* lua_topointer(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushnil(lua_State* L);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushnumber(lua_State* L, lua_Number n);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushlstring(lua_State* L, [NativeTypeName("const char *")] byte* s, [NativeTypeName("size_t")] nuint l);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushstring(lua_State* L, [NativeTypeName("const char *")] byte* s);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* lua_pushvfstring(lua_State* L, [NativeTypeName("const char *")] byte* fmt, [NativeTypeName("va_list")] byte* argp);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushcclosure(lua_State* L, lua_CFunction fn, int n);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushboolean(lua_State* L, int b);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushlightuserdata(lua_State* L, void* p);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_gettable(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_rawget(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_rawgeti(lua_State* L, int idx, int n);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_newtable(lua_State* L);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void* lua_newuserdata(lua_State* L, [NativeTypeName("size_t")] nuint sz);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_getmetatable(lua_State* L, int objindex);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_getfenv(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_settable(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_rawset(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_rawseti(lua_State* L, int idx, int n);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_setmetatable(lua_State* L, int objindex);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_setfenv(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_call(lua_State* L, int nargs, int nresults);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_pcall(lua_State* L, int nargs, int nresults, int errfunc);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_cpcall(lua_State* L, lua_CFunction func, void* ud);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_load(lua_State* L, [NativeTypeName("lua_Chunkreader")] delegate* unmanaged[Cdecl]<lua_State*, void*, nuint*, byte*> reader, void* dt, [NativeTypeName("const char *")] byte* chunkname);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_dump(lua_State* L, [NativeTypeName("lua_Chunkwriter")] delegate* unmanaged[Cdecl]<lua_State*, void*, nuint, void*, int> writer, void* data);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_yield(lua_State* L, int nresults);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_resume(lua_State* L, int narg);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_getgcthreshold(lua_State* L);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_getgccount(lua_State* L);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_setgcthreshold(lua_State* L, int newthreshold);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_version", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_version();

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_error(lua_State* L);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_next(lua_State* L, int idx);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_concat(lua_State* L, int n);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_pushupvalues(lua_State* L);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_getstack(lua_State* L, int level, lua_Debug* ar);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_getinfo(lua_State* L, [NativeTypeName("const char *")] byte* what, lua_Debug* ar);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_getlocal", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_getlocal(lua_State* L, [NativeTypeName("const lua_Debug *")] lua_Debug* ar, int n);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_setlocal", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_setlocal(lua_State* L, [NativeTypeName("const lua_Debug *")] lua_Debug* ar, int n);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_getupvalue", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_getupvalue(lua_State* L, int funcindex, int n);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_setupvalue", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_setupvalue(lua_State* L, int funcindex, int n);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_sethook(lua_State* L, lua_Hook func, int mask, int count);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Hook lua_gethook(lua_State* L);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_gethookmask(lua_State* L);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_gethookcount(lua_State* L);
    }
}
