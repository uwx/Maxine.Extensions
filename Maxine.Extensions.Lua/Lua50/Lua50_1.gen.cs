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
        [SuppressGCTransition]
        public static extern void luaL_openlib(lua_State* L, [NativeTypeName("const char *")] byte* libname, [NativeTypeName("const luaL_reg *")] luaL_reg* l, int nup);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_getmetafield(lua_State* L, int obj, [NativeTypeName("const char *")] byte* e);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_callmeta(lua_State* L, int obj, [NativeTypeName("const char *")] byte* e);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_typerror(lua_State* L, int narg, [NativeTypeName("const char *")] byte* tname);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_argerror(lua_State* L, int numarg, [NativeTypeName("const char *")] byte* extramsg);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_checklstring(lua_State* L, int numArg, [NativeTypeName("size_t *")] nuint* l);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_optlstring(lua_State* L, int numArg, [NativeTypeName("const char *")] byte* def, [NativeTypeName("size_t *")] nuint* l);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Number luaL_checknumber(lua_State* L, int numArg);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Number luaL_optnumber(lua_State* L, int nArg, lua_Number def);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_checkstack(lua_State* L, int sz, [NativeTypeName("const char *")] byte* msg);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_checktype(lua_State* L, int narg, int t);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_checkany(lua_State* L, int narg);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_newmetatable(lua_State* L, [NativeTypeName("const char *")] byte* tname);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaL_getmetatable(lua_State* L, [NativeTypeName("const char *")] byte* tname);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void* luaL_checkudata(lua_State* L, int ud, [NativeTypeName("const char *")] byte* tname);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_where(lua_State* L, int lvl);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_findstring([NativeTypeName("const char *")] byte* st, [NativeTypeName("const char *const[]")] byte** lst);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_ref(lua_State* L, int t);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_unref(lua_State* L, int t, int @ref);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_getn(lua_State* L, int t);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaL_setn(lua_State* L, int t, int n);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_loadfile(lua_State* L, [NativeTypeName("const char *")] byte* filename);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_loadbuffer(lua_State* L, [NativeTypeName("const char *")] byte* buff, [NativeTypeName("size_t")] nuint sz, [NativeTypeName("const char *")] byte* name);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_buffinit(lua_State* L, luaL_Buffer* B);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_prepbuffer(luaL_Buffer* B);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_addlstring(luaL_Buffer* B, [NativeTypeName("const char *")] byte* s, [NativeTypeName("size_t")] nuint l);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_addstring(luaL_Buffer* B, [NativeTypeName("const char *")] byte* s);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_addvalue(luaL_Buffer* B);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_pushresult(luaL_Buffer* B);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_dofile(lua_State* L, [NativeTypeName("const char *")] byte* filename);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_dostring(lua_State* L, [NativeTypeName("const char *")] byte* str);

        [DllImport("lua503", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_dobuffer(lua_State* L, [NativeTypeName("const char *")] byte* buff, [NativeTypeName("size_t")] nuint sz, [NativeTypeName("const char *")] byte* n);
    }
}
