#if LUAJIT
global using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.LuaJIT.lua_State*, int>;
global using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
global using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.LuaJIT.lua_State*, void*, nuint*, byte*>;
global using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.LuaJIT.lua_State*, void*, nuint, void*, int>;
global using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.LuaJIT.lua_State*, Maxine.Extensions.LuaJIT.lua_Debug*, void>;
global using NativeMethods = Maxine.Extensions.LuaJIT.LuaJIT;
global using Maxine.Extensions.LuaJIT;
#else
#if LUA_5_0
global using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua50.lua_State*, int>;
global using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
global using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua50.lua_State*, void*, nuint*, byte*>;
global using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua50.lua_State*, void*, nuint, void*, int>;
global using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua50.lua_State*, Maxine.Extensions.Lua50.lua_Debug*, void>;
global using NativeMethods = Maxine.Extensions.Lua50.Lua50;
global using Maxine.Extensions.Lua50;
#else
#if LUA_5_1
global using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua51.lua_State*, int>;
global using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
global using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua51.lua_State*, void*, nuint*, byte*>;
global using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua51.lua_State*, void*, nuint, void*, int>;
global using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua51.lua_State*, Maxine.Extensions.Lua51.lua_Debug*, void>;
global using NativeMethods = Maxine.Extensions.Lua51.Lua51;
global using Maxine.Extensions.Lua51;
#else
#if LUA_5_2
global using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua52.lua_State*, int>;
global using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
global using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua52.lua_State*, void*, nuint*, byte*>;
global using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua52.lua_State*, void*, nuint, void*, int>;
global using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua52.lua_State*, Maxine.Extensions.Lua52.lua_Debug*, void>;
global using NativeMethods = Maxine.Extensions.Lua52.Lua52;
global using Maxine.Extensions.Lua52;
#else
#if LUA_5_3
global using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua53.lua_State*, int>;
global using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
global using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua53.lua_State*, void*, nuint*, byte*>;
global using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua53.lua_State*, void*, nuint, void*, int>;
global using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua53.lua_State*, Maxine.Extensions.Lua53.lua_Debug*, void>;
global using NativeMethods = Maxine.Extensions.Lua53.Lua53;
global using Maxine.Extensions.Lua53;
#else
#if LUA_5_4
global using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua54.lua_State*, int>;
global using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
global using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua54.lua_State*, void*, nuint*, byte*>;
global using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua54.lua_State*, void*, nuint, void*, int>;
global using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua54.lua_State*, Maxine.Extensions.Lua54.lua_Debug*, void>;
global using NativeMethods = Maxine.Extensions.Lua54.Lua54;
global using Maxine.Extensions.Lua54;
#else
#if LUA_5_5
global using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua55.lua_State*, int>;
global using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
global using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua55.lua_State*, void*, nuint*, byte*>;
global using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua55.lua_State*, void*, nuint, void*, int>;
global using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua55.lua_State*, Maxine.Extensions.Lua55.lua_Debug*, void>;
global using NativeMethods = Maxine.Extensions.Lua55.Lua55;
global using Maxine.Extensions.Lua55;
#else
#error Unsupported Lua version. Define one of LUAJIT, LUA_5_0, LUA_5_1, LUA_5_2, LUA_5_3, LUA_5_4, or LUA_5_5.
#endif
#endif
#endif
#endif
#endif
#endif
#endif