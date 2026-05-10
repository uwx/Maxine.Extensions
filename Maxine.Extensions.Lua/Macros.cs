using System.Runtime.InteropServices;
using JetBrains.Annotations;

#if LUAJIT
using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.LuaJIT.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.LuaJIT.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.LuaJIT.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.LuaJIT.lua_State*, Maxine.Extensions.LuaJIT.lua_Debug*, void>;
using static Maxine.Extensions.LuaJIT.LuaJIT;
namespace Maxine.Extensions.LuaJIT;
#else
#if LUA_5_0
using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua50.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua50.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua50.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua50.lua_State*, Maxine.Extensions.Lua50.lua_Debug*, void>;
using static Maxine.Extensions.Lua50.Lua50;
namespace Maxine.Extensions.Lua50;
#else
#if LUA_5_1
using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua51.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua51.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua51.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua51.lua_State*, Maxine.Extensions.Lua51.lua_Debug*, void>;
using static Maxine.Extensions.Lua51.Lua51;
namespace Maxine.Extensions.Lua51;
#else
#if LUA_5_2
using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua52.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua52.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua52.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua52.lua_State*, Maxine.Extensions.Lua52.lua_Debug*, void>;
using static Maxine.Extensions.Lua52.Lua52;
namespace Maxine.Extensions.Lua52;
#else
#if LUA_5_3
using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua53.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua53.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua53.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua53.lua_State*, Maxine.Extensions.Lua53.lua_Debug*, void>;
using static Maxine.Extensions.Lua53.Lua53;
namespace Maxine.Extensions.Lua53;
#else
#if LUA_5_4
using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua54.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua54.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua54.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua54.lua_State*, Maxine.Extensions.Lua54.lua_Debug*, void>;
using static Maxine.Extensions.Lua54.Lua54;
namespace Maxine.Extensions.Lua54;
#else
#if LUA_5_5
using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua55.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua55.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua55.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua55.lua_State*, Maxine.Extensions.Lua55.lua_Debug*, void>;
using static Maxine.Extensions.Lua55.Lua55;
namespace Maxine.Extensions.Lua55;
#else
#error Unsupported Lua version. Define one of LUAJIT, LUA_5_0, LUA_5_1, LUA_5_2, LUA_5_3, LUA_5_4, or LUA_5_5.
#endif
#endif
#endif
#endif
#endif
#endif
#endif

// ReSharper disable InconsistentNaming
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

/// <summary>
/// Type for arrays of functions to be registered by luaL_register. name is the function name and func is a pointer to the function. Any array of luaL_Reg must end with an sentinel entry in which both name and func are NULL.<br/>
/// <br/>
/// </summary>
/// <code>
/// typedef struct luaL_Reg {
/// const char *name;
/// lua_CFunction func;
/// } luaL_Reg;
/// </code>
public unsafe partial struct luaL_RegManaged
{
    public string name;
    public lua_CFunction func;
}


#if LUAJIT
public static unsafe partial class LuaJIT
#else
#if LUA_5_0
public static unsafe partial class Lua50
#else
#if LUA_5_1
public static unsafe partial class Lua51
#else
#if LUA_5_2
public static unsafe partial class Lua52
#else
#if LUA_5_3
public static unsafe partial class Lua53
#else
#if LUA_5_4
public static unsafe partial class Lua54
#else
#if LUA_5_5
public static unsafe partial class Lua55
#else
#error Unsupported Lua version. Define one of LUAJIT, LUA_5_0, LUA_5_1, LUA_5_2, LUA_5_3, LUA_5_4, or LUA_5_5.
#endif
#endif
#endif
#endif
#endif
#endif
#endif
{
    /// <summary>
    /// [-n, +0, e]<br/>
    /// <br/>
    /// Pops n elements from the stack. It is implemented as a macro over lua_settop.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// void lua_pop (lua_State *L, int n);
    /// </code>
    public static void lua_pop(lua_State* L, int n)
    {
        lua_settop(L, -n - 1);
    }
    
#if LUA_5_1_OR_LATER
    /// <summary>
    /// [-0, +1, m]<br/>
    /// <br/>
    /// Creates a new empty table and pushes it onto the stack. It is equivalent to lua_createtable(L,0,0).
    /// </summary>
    /// <code>
    /// void lua_newtable (lua_State *L);
    /// </code>
    public static void lua_newtable(lua_State* L)
    {
        lua_createtable(L, 0, 0);
    }
#endif

    /// <summary>
    /// [-0, +0, e]<br/>
    /// Sets the C function f as the new value of global name. It is defined as a macro:
    /// <code>
    /// #define lua_register(L,n,f) \
    ///     (lua_pushcfunction(L, f), lua_setglobal(L, n))
    /// </code>
    /// </summary>
    /// <code>
    /// void lua_register (lua_State *L, const char *name, lua_CFunction f);
    /// </code>
    public static void lua_register(lua_State* L, string n,
        [NativeTypeName("lua_CFunction")] lua_CFunction f)
    {
        lua_pushcfunction(L, f);
        lua_setglobal(L, n);
    }
    
#if LUA_5_0
    public static void lua_setglobal(lua_State* L, string s)
    {
        lua_pushstring(L, s);
        lua_insert(L, -2);
        lua_settable(L, LUA_GLOBALSINDEX);
    }

    public static void lua_setglobal(lua_State* L, ReadOnlySpan<byte> s)
    {
        lua_pushstring(L, s);
        lua_insert(L, -2);
        lua_settable(L, LUA_GLOBALSINDEX);
    }

    public static void lua_setglobal(lua_State* L, byte* s)
    {
        lua_pushstring(L, s);
        lua_insert(L, -2);
        lua_settable(L, LUA_GLOBALSINDEX);
    }
#endif

#if LUA_5_1_OR_LATER
    public static size_t lua_strlen(lua_State* L, int i)
    {
#if LUA_5_2_OR_LATER
        return lua_rawlen(L, i);
#else
        return lua_objlen(L, i);
#endif
    }
#endif

    /// <summary>
    /// [-0, +0, -]<br/>
    /// <br/>
    /// Returns 1 if the value at the given acceptable index is a function (either C or Lua), and 0 otherwise.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int lua_isfunction (lua_State *L, int index);
    /// </code>
    public static int lua_isfunction(lua_State* L, int n)
    {
        return (lua_type(L, n) == LUA_TFUNCTION) ? 1 : 0;
    }

    /// <summary>
    /// [-0, +0, -]<br/>
    /// <br/>
    /// Returns 1 if the value at the given acceptable index is a table, and 0 otherwise.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int lua_istable (lua_State *L, int index);
    /// </code>
    public static int lua_istable(lua_State* L, int n)
    {
        return (lua_type(L, n) == LUA_TTABLE) ? 1 : 0;
    }

    /// <summary>
    /// [-0, +0, -]<br/>
    /// <br/>
    /// Returns 1 if the value at the given acceptable index is a light userdata, and 0 otherwise.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int lua_islightuserdata (lua_State *L, int index);
    /// </code>
    public static int lua_islightuserdata(lua_State* L, int n)
    {
        return (lua_type(L, n) == LUA_TLIGHTUSERDATA) ? 1 : 0;
    }

    /// <summary>
    /// [-0, +0, -]<br/>
    /// <br/>
    /// Returns 1 if the value at the given acceptable index is nil, and 0 otherwise.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int lua_isnil (lua_State *L, int index);
    /// </code>
    public static int lua_isnil(lua_State* L, int n)
    {
        return (lua_type(L, n) == LUA_TNIL) ? 1 : 0;
    }

    /// <summary>
    /// [-0, +0, -]<br/>
    /// <br/>
    /// Returns 1 if the value at the given acceptable index has type boolean, and 0 otherwise.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int lua_isboolean (lua_State *L, int index);
    /// </code>
    public static int lua_isboolean(lua_State* L, int n)
    {
        return (lua_type(L, n) == LUA_TBOOLEAN) ? 1 : 0;
    }

    /// <summary>
    /// [-0, +0, -]<br/>
    /// <br/>
    /// Returns 1 if the value at the given acceptable index is a thread, and 0 otherwise.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int lua_isthread (lua_State *L, int index);
    /// </code>
    public static int lua_isthread(lua_State* L, int n)
    {
        return (lua_type(L, n) == LUA_TTHREAD) ? 1 : 0;
    }

    /// <summary>
    /// [-0, +0, -]<br/>
    /// <br/>
    /// Returns 1 if the given acceptable index is not valid (that is, it refers to an element outside the current stack), and 0 otherwise.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int lua_isnone (lua_State *L, int index);
    /// </code>
    public static int lua_isnone(lua_State* L, int n)
    {
        return (lua_type(L, n) == LUA_TNONE) ? 1 : 0;
    }

    /// <summary>
    /// [-0, +0, -]<br/>
    /// <br/>
    /// Returns 1 if the given acceptable index is not valid (that is, it refers to an element outside the current stack) or if the value at this index is nil, and 0 otherwise.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int lua_isnoneornil (lua_State *L, int index);
    /// </code>
    public static int lua_isnoneornil(lua_State* L, int n)
    {
        return (lua_type(L, n) <= 0) ? 1 : 0;
    }

    /// <summary>
    /// [-0, +1, m]<br/>
    /// <br/>
    /// This macro is equivalent to lua_pushlstring, but can be used only when s is a literal string. In these cases, it automatically provides the string length.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// void lua_pushliteral (lua_State *L, const char *s);
    /// </code>
    public static void lua_pushliteral(lua_State* L, string s)
    {
        lua_pushlstring(L, s);
    }

    /// <summary>
    /// [-0, +1, m]<br/>
    /// <br/>
    /// This macro is equivalent to lua_pushlstring, but can be used only when s is a literal string. In these cases, it automatically provides the string length.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// void lua_pushliteral (lua_State *L, const char *s);
    /// </code>
    public static void lua_pushliteral(lua_State* L, ReadOnlySpan<byte> s)
    {
        lua_pushlstring(L, s);
    }

#if LUA_5_1_OR_LATER
    /// <summary>
    /// [-1, +0, e]<br/>
    /// <br/>
    /// Pops a value from the stack and sets it as the new value of global name. It is defined as a macro:<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// void lua_setglobal (lua_State *L, const char *name);
    /// </code>
    public static void lua_setglobal(lua_State* L, string s)
    {
#if !LUA_5_2_OR_LATER
        lua_setfield(L, LUA_GLOBALSINDEX, s);
#else
        var ptr = (byte*)Marshal.StringToHGlobalAnsi(s);
        try
        {
            lua_setglobal(L, ptr);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
#endif
    }

#if LUA_5_3_OR_LATER
    /// <summary>
    /// [-0, +1, e]<br/>
    /// <br/>
    /// Pushes onto the stack the value of the global name. It is defined as a macro:<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// void lua_getglobal (lua_State *L, const char *name);
    /// </code>
    public static int lua_getglobal(lua_State* L, string s)
    {
#if !LUA_5_2_OR_LATER
        return lua_getfield(L, LUA_GLOBALSINDEX, s);
#else
        var ptr = (byte*)Marshal.StringToHGlobalAnsi(s);
        try
        {
            return lua_getglobal(L, ptr);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
#endif
    }
#else
    /// <summary>
    /// [-0, +1, e]<br/>
    /// <br/>
    /// Pushes onto the stack the value of the global name. It is defined as a macro:<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// void lua_getglobal (lua_State *L, const char *name);
    /// </code>
    public static void lua_getglobal(lua_State* L, string s)
    {
#if !LUA_5_2_OR_LATER
        lua_getfield(L, LUA_GLOBALSINDEX, s);
#else
        var ptr = (byte*)Marshal.StringToHGlobalAnsi(s);
        try
        {
            lua_getglobal(L, ptr);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
#endif
    }
#endif

    public static lua_State* lua_open()
    {
        return luaL_newstate();
    }
#endif

    public static void lua_getregistry(lua_State* L)
    {
        lua_pushvalue(L, LUA_REGISTRYINDEX);
    }

#if LUA_5_1_OR_LATER
    public static int lua_getgccount(lua_State* L)
    {
#if LUA_5_4_OR_LATER
        return lua_gc(L, LUA_GCCOUNT, __arglist(0));
#else
        return lua_gc(L, LUA_GCCOUNT, 0);
#endif
    }
#endif

    #region Common Convenience Helpers
    
#if LUA_5_2_OR_LATER
    /// <summary>
    /// [-(nargs + 1), +(nresults|1), -]<br/>
    /// <br/>
    /// Calls a function in protected mode.<br/>
    /// <br/>
    /// Both nargs and nresults have the same meaning as in lua_call. If there are no errors during the call, lua_pcall behaves exactly like lua_call. However, if there is any error, lua_pcall catches it, pushes a single value on the stack (the error message), and returns an error code. Like lua_call, lua_pcall always removes the function and its arguments from the stack.<br/>
    /// <br/>
    /// If errfunc is 0, then the error message returned on the stack is exactly the original error message. Otherwise, errfunc is the stack index of an error handler function. (In the current implementation, this index cannot be a pseudo-index.) In case of runtime errors, this function will be called with the error message and its return value will be the message returned on the stack by lua_pcall.<br/>
    /// <br/>
    /// Typically, the error handler function is used to add more debug information to the error message, such as a stack traceback. Such information cannot be gathered after the return of lua_pcall, since by then the stack has unwound.<br/>
    /// <br/>
    /// The lua_pcall function returns 0 in case of success or one of the following error codes (defined in lua.h):<br/>
    /// <br/>
    /// - LUA_ERRRUN: a runtime error.<br/>
    /// - LUA_ERRMEM: memory allocation error. For such errors, Lua does not call the error handler function.<br/>
    /// - LUA_ERRERR: error while running the error handler function.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int lua_pcall (lua_State *L, int nargs, int nresults, int errfunc);
    /// </code>
    public static int lua_pcall(lua_State* L, int nargs, int nresults, int errfunc)
    {
        return lua_pcallk(L, nargs, nresults, errfunc, 0, null);
    }
#endif

#if LUA_5_1_OR_LATER
    /// <summary>
    /// [-0, +?, m]<br/>
    /// <br/>
    /// Loads and runs the given string. It is defined as the following macro:<br/>
    /// <br/>
    /// It returns 0 if there are no errors or 1 in case of errors.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int luaL_dostring (lua_State *L, const char *str);
    /// </code>
    public static int luaL_dostring(lua_State* L, [LanguageInjection("Lua")] string str)
    {
        var result = luaL_loadstring(L, str);
        if (result != 0) return result;
        return lua_pcall(L, 0, -1, 0);
    }

    /// <summary>
    /// [-0, +?, m]<br/>
    /// <br/>
    /// Loads and runs the given string. It is defined as the following macro:<br/>
    /// <br/>
    /// It returns 0 if there are no errors or 1 in case of errors.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int luaL_dostring (lua_State *L, const char *str);
    /// </code>
    public static int luaL_dostring(lua_State* L, [LanguageInjection("Lua")] ReadOnlySpan<byte> str)
    {
        var result = luaL_loadstring(L, str);
        if (result != 0) return result;
        return lua_pcall(L, 0, -1, 0);
    }
#endif

    /// <summary>
    /// [-0, +?, m]<br/>
    /// <br/>
    /// Loads and runs the given file. It is defined as the following macro:<br/>
    /// <br/>
    /// It returns 0 (LUA_OK) if there are no errors, or 1 in case of errors.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int luaL_dofile (lua_State *L, const char *filename);
    /// </code>
    public static int luaL_dofile(lua_State* L, string filename)
    {
        var result = luaL_loadfile(L, filename);
        if (result != 0) return result;
        return lua_pcall(L, 0, -1, 0);
    }

    /// <summary>
    /// [-0, +?, m]<br/>
    /// <br/>
    /// Loads and runs the given file. It is defined as the following macro:<br/>
    /// <br/>
    /// It returns 0 if there are no errors or 1 in case of errors.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int luaL_dofile (lua_State *L, const char *filename);
    /// </code>
    public static int luaL_dofile(lua_State* L, ReadOnlySpan<byte> filename)
    {
        var result = luaL_loadfile(L, filename);
        if (result != 0) return result;
        return lua_pcall(L, 0, -1, 0);
    }

#if LUA_5_1_OR_LATER
    /// <summary>
    /// [-0, +1, e]<br/>
    /// <br/>
    /// Pushes onto the stack the value of the global name. It is defined as a macro:<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// void lua_getglobal (lua_State *L, const char *name);
    /// </code>
    public static void lua_getglobal(lua_State* L, ReadOnlySpan<byte> name)
    {
#if !LUA_5_2_OR_LATER
        lua_getfield(L, LUA_GLOBALSINDEX, name);
#else
        fixed (byte* ptr = name)
        {
            lua_getglobal(L, ptr);
        }
#endif
    }

    /// <summary>
    /// [-1, +0, e]<br/>
    /// <br/>
    /// Pops a value from the stack and sets it as the new value of global name. It is defined as a macro:<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// void lua_setglobal (lua_State *L, const char *name);
    /// </code>
    public static void lua_setglobal(lua_State* L, ReadOnlySpan<byte> name)
    {
#if !LUA_5_2_OR_LATER
        lua_setfield(L, LUA_GLOBALSINDEX, name);
#else
        fixed (byte* ptr = name)
        {
            lua_setglobal(L, ptr);
        }
#endif
    }
#endif

    /// <summary>
    /// [-0, +1, -]<br/>
    /// <br/>
    /// Pushes a C function onto the stack. This function is equivalent to lua_pushcclosure with no upvalues.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// void lua_pushcfunction (lua_State *L, lua_CFunction f);
    /// </code>
    public static void lua_pushcfunction(lua_State* L, lua_CFunction f)
    {
        lua_pushcclosure(L, f, 0);
    }

    #endregion

#if LUAJIT || LUA_5_2_OR_LATER
    /// <summary>
    /// [-0, +0, -]<br/>
    /// <br/>
    /// Returns the version number of this core.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// lua_Number lua_version (lua_State *L);
    /// </code>
    public static double lua_version(lua_State* L)
    {
#if LUA_5_4_OR_LATER
        return _lua_version(L);
#else
        var mem = _lua_version(L);
        if (mem == null)
            return 0;
        return *mem;
#endif
    }
#endif
	
    /// <summary>
    /// [-0, +0, v]<br/>
    /// <br/>
    /// Checks whether cond is true. If it is not, raises an error with a standard message (see luaL_argerror).<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// void luaL_argcheck (lua_State *L,
    /// int cond,
    /// int arg,
    /// const char *extramsg);
    /// </code>
    public static void luaL_argcheck(lua_State* L, bool cond, int numarg, string extramsg)
    {
        if (!cond)
            luaL_argerror(L, numarg, extramsg);
    }
	
    /// <summary>
    /// [-0, +0, v]<br/>
    /// <br/>
    /// Checks whether the function argument arg is a string and returns this string.<br/>
    /// <br/>
    /// This function uses lua_tolstring to get its result, so all conversions and caveats of that function apply here.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// const char *luaL_checkstring (lua_State *L, int arg);
    /// </code>
    public static string? luaL_checkstring(lua_State* L, int n)
    {
        return luaL_checklstring(L, n, out _);
    }
	
    /// <summary>
    /// [-0, +0, v]<br/>
    /// <br/>
    /// If the function argument arg is a string, returns this string. If this argument is absent or is nil, returns d. Otherwise, raises an error.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// const char *luaL_optstring (lua_State *L,
    /// int arg,
    /// const char *d);
    /// </code>
    public static string? luaL_optstring(lua_State* L, int n, string d)
    {
        return luaL_optlstring(L, n, d, out _);
    }
	
#if LUA_5_1_OR_LATER
    /// <summary>
    /// [-0, +0, v]<br/>
    /// <br/>
    /// Checks whether the function argument narg is a number and returns this number cast to an int.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int luaL_checkint (lua_State *L, int narg);
    /// </code>
    public static int luaL_checkint(lua_State* L, int n)
    {
        return (int) luaL_checkinteger(L, n);
    }

    /// <summary>
    /// [-0, +0, v]<br/>
    /// <br/>
    /// If the function argument narg is a number, returns this number cast to an int. If this argument is absent or is nil, returns d. Otherwise, raises an error.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int luaL_optint (lua_State *L, int narg, int d);
    /// </code>
    public static int luaL_optint(lua_State* L, int n, lua_Integer d)
    {
        return (int) luaL_optinteger(L, n, d);
    }
	
    /// <summary>
    /// [-0, +0, v]<br/>
    /// <br/>
    /// Checks whether the function argument narg is a number and returns this number cast to a long.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// long luaL_checklong (lua_State *L, int narg);
    /// </code>
    public static long luaL_checklong(lua_State* L, int n)
    {
        return luaL_checkinteger(L, n);
    }
	
    /// <summary>
    /// [-0, +0, v]<br/>
    /// <br/>
    /// If the function argument narg is a number, returns this number cast to a long. If this argument is absent or is nil, returns d. Otherwise, raises an error.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// long luaL_optlong (lua_State *L, int narg, long d);
    /// </code>
    public static long luaL_optlong(lua_State* L, int n, lua_Integer d)
    {
        return luaL_optinteger(L, n, d);
    }
#endif
    
    /// <summary>
    /// [-0, +0, -]<br/>
    /// <br/>
    /// Returns the name of the type of the value at the given index.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// const char *luaL_typename (lua_State *L, int index);
    /// </code>
    public static string? luaL_typename(lua_State* L, int i)
    {
        return lua_typename(L, lua_type(L, i));
    }
	
#if LUA_5_1_OR_LATER
    /// <summary>
    /// [-0, +1, m]<br/>
    /// <br/>
    /// Pushes onto the stack the metatable associated with the name tname in the registry (see luaL_newmetatable), or nil if there is no metatable associated with that name. Returns the type of the pushed value.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int luaL_getmetatable (lua_State *L, const char *tname);
    /// </code>
    public static void luaL_getmetatable(lua_State* L, string n)
    {
        lua_getfield(L, LUA_REGISTRYINDEX, n);
    }
	
    /// <summary>
    /// [-0, +1, m]<br/>
    /// <br/>
    /// Pushes onto the stack the metatable associated with the name tname in the registry (see luaL_newmetatable), or nil if there is no metatable associated with that name. Returns the type of the pushed value.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int luaL_getmetatable (lua_State *L, const char *tname);
    /// </code>
    public static void luaL_getmetatable(lua_State* L, ReadOnlySpan<byte> n)
    {
        lua_getfield(L, LUA_REGISTRYINDEX, n);
    }
#endif
    
    public delegate T luaL_Function<out T>(lua_State* L, int n);
	
    /// <summary>
    /// [-0, +0, -]<br/>
    /// <br/>
    /// This macro is defined as follows:<br/>
    /// <br/>
    /// In words, if the argument arg is nil or absent, the macro results in the default dflt. Otherwise, it results in the result of calling func with the state L and the argument index arg as arguments. Note that it evaluates the expression dflt only if needed.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// T luaL_opt (L, func, arg, dflt);
    /// </code>
    public static T luaL_opt<T>(lua_State* L, luaL_Function<T> f, int n, T d)
    {
        return lua_isnoneornil(L, n) > 0 ? d : f(L, n);
    }
	
#if LUA_5_2_OR_LATER
    /// <summary>
    /// [-0, +1, m]<br/>
    /// <br/>
    /// Creates a new table with a size optimized to store all entries in the array l (but does not actually store them). It is intended to be used in conjunction with luaL_setfuncs (see luaL_newlib).<br/>
    /// <br/>
    /// It is implemented as a macro. The array l must be the actual array, not a pointer to it.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// void luaL_newlibtable (lua_State *L, const luaL_Reg l[]);
    /// </code>
    public static void luaL_newlibtable(lua_State* L, luaL_Reg* l)
    {
        int n = 0;
        for (luaL_Reg* curr = l; curr->name != null; curr++)
            n++;
        lua_createtable(L, 0, n);
    }
	
    /// <summary>
    /// [-0, +1, m]<br/>
    /// <br/>
    /// Creates a new table and registers there the functions in the list l.<br/>
    /// <br/>
    /// It is implemented as the following macro:<br/>
    /// <br/>
    /// The array l must be the actual array, not a pointer to it.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// void luaL_newlib (lua_State *L, const luaL_Reg l[]);
    /// </code>
    public static void luaL_newlib(lua_State* L, luaL_Reg* l)
    {
        luaL_newlibtable(L, l);
        luaL_setfuncs(L, l, 0);
    }

    /// <summary>
    /// [-0, +1, m]<br/>
    /// <br/>
    /// Creates a new table and registers there the functions in the list l.<br/>
    /// <br/>
    /// It is implemented as the following macro:<br/>
    /// <br/>
    /// The array l must be the actual array, not a pointer to it.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// void luaL_newlib (lua_State *L, const luaL_Reg l[]);
    /// </code>
    public static void luaL_newlib(lua_State* L, ReadOnlySpan<luaL_RegManaged> l)
    {
        lua_createtable(L, 0, l.Length);
        luaL_setfuncs(L, l, 0);
    }

    /// <summary>
    /// [-nup, +0, m]<br/>
    /// <br/>
    /// Registers all functions in the array l (see luaL_Reg) into the table on the top of the stack (below optional upvalues, see next).<br/>
    /// <br/>
    /// When nup is not zero, all functions are created with nup upvalues, initialized with copies of the nup values previously pushed on the stack on top of the library table. These values are popped from the stack after the registration.<br/>
    /// <br/>
    /// A function with a NULL value represents a placeholder, which is filled with false.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// void luaL_setfuncs (lua_State *L, const luaL_Reg *l, int nup);
    /// </code>
    public static void luaL_setfuncs(lua_State* L, ReadOnlySpan<luaL_RegManaged> l, int nup)
    {
        Span<luaL_Reg> regs = stackalloc luaL_Reg[l.Length + 1];
        try
        {
            for (int i = 0; i < l.Length; i++)
            {
                regs[i].name = (byte*)Marshal.StringToHGlobalAnsi(l[i].name);
                regs[i].func = l[i].func;
            }

            regs[l.Length].name = null;
            regs[l.Length].func = null;
            fixed (luaL_Reg* p = &regs[0])
            {
                luaL_setfuncs(L, p, nup);
            }
        }
        finally
        {
            for (int i = 0; i < l.Length; i++)
            {
                Marshal.FreeHGlobal((nint)regs[i].name);
            }
        }
    }

    public static void luaL_openlib(lua_State* L, string libname, ReadOnlySpan<luaL_RegManaged> l, int nup)
    {
        if (libname != null)
        {
            lua_getglobal(L, libname);
            if (lua_isnil(L, -1) == 1)
            {
                lua_pop(L, 1);
                lua_createtable(L, 0, l.Length);
                lua_pushvalue(L, -1);
                lua_setglobal(L, libname);
            }
        }
        luaL_setfuncs(L, l, nup);
    }
#endif
	
#if !LUA_5_2_OR_LATER
    /// <summary>
    /// [-0, +0, m]<br/>
    /// <br/>
    /// Adds the character c to the buffer B (see luaL_Buffer).<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// void luaL_addchar (luaL_Buffer *B, char c);
    /// </code>
    public static void luaL_addchar(luaL_Buffer* B, byte c)
    {
        if (B->p >= &B->buffer + LUAL_BUFFERSIZE)
            luaL_prepbuffer(B);
        *(B->p) = c;
        B->p++;
    }
	
    public static void luaL_putchar(luaL_Buffer* B, byte c)
    {
        luaL_addchar(B, c);
    }
#endif
	
    /// <summary>
    /// [-0, +0, m]<br/>
    /// <br/>
    /// Adds to the buffer B (see luaL_Buffer) a string of length n previously copied to the buffer area (see luaL_prepbuffer).<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// void luaL_addsize (luaL_Buffer *B, size_t n);
    /// </code>
    public static void luaL_addsize(luaL_Buffer* B, nint n)
    {
#if LUA_5_2_OR_LATER
        B->b += n;
#else
        B->p += n;
#endif
    }
    
    #region Lua 5.2/5.3 Compatibility Helpers
    // https://github.com/lunarmodules/lua-compat-5.3

#if !LUA_5_2_OR_LATER
    /// <summary>
    /// Converts a possibly negative stack index into an absolute index. Implemented as abs_index(L, i) macro in
    /// lauxlib.c.
    /// </summary>
    /// <param name="L"></param>
    /// <param name="i"></param>
    /// <returns></returns>
    public static int lua_absindex(lua_State* L, int i)
    {
        return i is > 0 or <= LUA_REGISTRYINDEX ? i : lua_gettop(L) + i + 1;
    }

#if LUA_5_1_OR_LATER
    /// <summary>
    /// [-0, +0, -]<br/>
    /// <br/>
    /// Returns the raw "length" of the value at the given index: for strings, this is the string length; for tables, this is the result of the length operator ('#') with no metamethods; for userdata, this is the size of the block of memory allocated for the userdata. For other values, this call returns 0.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// lua_Unsigned lua_rawlen (lua_State *L, int index);
    /// </code>
    public static ulong lua_rawlen(lua_State* L, int i)
    {
        return lua_objlen(L, i);
    }
#endif
#endif
    
    public static void* lua_tolightuserdata(lua_State* L, int n)
    {
        return lua_touserdata(L, n);
    }
    
#if !LUA_5_2_OR_LATER
#if LUA_5_1_OR_LATER
    public static void lua_pushunsigned(lua_State* L, nuint n)
    {
        lua_pushinteger(L, (lua_Integer)n);
    }
    
#if !LUA_5_1
    public static lua_Unsigned lua_tounsigned(lua_State* L, int n)
    {
        return lua_tounsignedx(L, n, null);
    }
    
    public static lua_Unsigned lua_tounsignedx(lua_State* L, int n, int* isnum)
    {
        return (lua_Unsigned) lua_tointegerx(L, n, isnum);
    }
#endif
    
    public static lua_Unsigned luaL_checkunsigned(lua_State* L, int n)
    {
        return (lua_Unsigned) luaL_checkinteger(L, n);
    }
    
    public static lua_Unsigned luaL_optunsigned(lua_State* L, int n, lua_Unsigned d)
    {
        return (lua_Unsigned) luaL_optinteger(L, n, (lua_Integer)d);
    }
#endif
    
    public static void lua_getuservalue(lua_State* L, int i)
    {
        lua_getfenv(L, i);
        lua_type(L, -1);
    }
    
    public static void lua_setuservalue(lua_State* L, int i)
    {
        luaL_checktype(L, -1, LUA_TTABLE);
        lua_setfenv(L, i);
    }
#endif
    
    /// <summary>
    /// [-0, +1, -]<br/>
    /// <br/>
    /// Pushes the global environment onto the stack.<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// void lua_pushglobaltable (lua_State *L);
    /// </code>
    public static void lua_pushglobaltable(lua_State* L)
    {
#if !LUA_5_2_OR_LATER
        lua_pushvalue(L, LUA_GLOBALSINDEX);
#else
        lua_rawgeti(L, LUA_REGISTRYINDEX, LUA_RIDX_GLOBALS);
#endif
    }
    
    #endregion
    
#if LUA_5_0
    public static void lua_getfield(lua_State* L, int index, string k)
    {
        lua_pushstring(L, k);
        lua_gettable(L, index - 1);
    }
    
    public static void lua_getfield(lua_State* L, int index, ReadOnlySpan<byte> k)
    {
        lua_pushstring(L, k);
        lua_gettable(L, index - 1);
    }
    
    public static void lua_getfield(lua_State* L, int index, byte* k)
    {
        lua_pushstring(L, k);
        lua_gettable(L, index - 1);
    }
    
    public static void lua_setfield(lua_State* L, int index, string k)
    {
        lua_pushstring(L, k);
        lua_insert(L, -2);
        lua_settable(L, index - 1);
    }
    
    public static void lua_setfield(lua_State* L, int index, ReadOnlySpan<byte> k)
    {
        lua_pushstring(L, k);
        lua_insert(L, -2);
        lua_settable(L, index - 1);
    }
    
    public static void lua_setfield(lua_State* L, int index, byte* k)
    {
        lua_pushstring(L, k);
        lua_insert(L, -2);
        lua_settable(L, index - 1);
    }
#endif
    
#if LUA_5_2_OR_LATER
    public static nuint lua_objlen(lua_State* L, int i)
    {
        return lua_rawlen(L, i);
    }
    
    public static lua_Number lua_tonumber(lua_State* L, int i)
    {
        return lua_tonumberx(L, i, null);
    }
    
    public static lua_Integer lua_tointeger(lua_State* L, int i)
    {
        return lua_tointegerx(L, i, null);
    }
    
#if !LUA_5_3_OR_LATER
    public static lua_Unsigned lua_tounsigned(lua_State* L, int i)
    {
        return lua_tounsignedx(L, i, null);
    }
#endif
#endif
    
#if LUA_5_5_OR_LATER
    public static int lua_resetthread(lua_State* L)
    {
        return lua_closethread(L, null);
    }

    public static void luaL_openlibs(lua_State* L)
    {
        luaL_openselectedlibs(L, ~0, 0);
    }
#endif
}