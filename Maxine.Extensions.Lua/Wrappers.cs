using System.Runtime.InteropServices;
using System.Text;

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
    #region luaL_* Library Functions
    
#if LUA_5_1_OR_LATER && !LUA_5_2_OR_LATER
    /// <inheritdoc cref="luaL_openlib(lua_State*, byte*, luaL_Reg*, int)"/>
    public static void luaL_openlib(lua_State* L, string? libname, luaL_Reg* l, int nup)
    {
        var libnamePtr = libname == null ? null : (byte*)Marshal.StringToHGlobalAnsi(libname);
        try
        {
            luaL_openlib(L, libnamePtr, l, nup);
        }
        finally
        {
            if (libnamePtr != null) Marshal.FreeHGlobal((nint)libnamePtr);
        }
    }

    /// <inheritdoc cref="luaL_openlib(lua_State*, byte*, luaL_Reg*, int)"/>
    public static void luaL_openlib(lua_State* L, ReadOnlySpan<byte> libname, luaL_Reg* l, int nup)
    {
        fixed (byte* libnamePtr = libname)
        {
            luaL_openlib(L, libnamePtr, l, nup);
        }
    }

    /// <inheritdoc cref="luaL_register(lua_State*, byte*, luaL_Reg*)"/>
    public static void luaL_register(lua_State* L, string? libname, luaL_Reg* l)
    {
        var libnamePtr = libname == null ? null : (byte*)Marshal.StringToHGlobalAnsi(libname);
        try
        {
            luaL_register(L, libnamePtr, l);
        }
        finally
        {
            if (libnamePtr != null) Marshal.FreeHGlobal((nint)libnamePtr);
        }
    }

    /// <inheritdoc cref="luaL_register(lua_State*, byte*, luaL_Reg*)"/>
    public static void luaL_register(lua_State* L, ReadOnlySpan<byte> libname, luaL_Reg* l)
    {
        fixed (byte* libnamePtr = libname)
        {
            luaL_register(L, libnamePtr, l);
        }
    }
#endif
    
    /// <inheritdoc cref="luaL_getmetafield(lua_State*, int, byte*)"/>
    public static int luaL_getmetafield(lua_State* L, int obj, string e)
    {
        var ePtr = e == null ? null : (byte*)Marshal.StringToHGlobalAnsi(e);
        try
        {
            return luaL_getmetafield(L, obj, ePtr);
        }
        finally
        {
            if (ePtr != null) Marshal.FreeHGlobal((nint)ePtr);
        }
    }

    /// <inheritdoc cref="luaL_getmetafield(lua_State*, int, byte*)"/>
    public static int luaL_getmetafield(lua_State* L, int obj, ReadOnlySpan<byte> e)
    {
        fixed (byte* ePtr = e)
        {
            return luaL_getmetafield(L, obj, ePtr);
        }
    }

    /// <inheritdoc cref="luaL_callmeta(lua_State*, int, byte*)"/>
    public static int luaL_callmeta(lua_State* L, int obj, string e)
    {
        var ePtr = e == null ? null : (byte*)Marshal.StringToHGlobalAnsi(e);
        try
        {
            return luaL_callmeta(L, obj, ePtr);
        }
        finally
        {
            if (ePtr != null) Marshal.FreeHGlobal((nint)ePtr);
        }
    }

    /// <inheritdoc cref="luaL_callmeta(lua_State*, int, byte*)"/>
    public static int luaL_callmeta(lua_State* L, int obj, ReadOnlySpan<byte> e)
    {
        fixed (byte* ePtr = e)
        {
            return luaL_callmeta(L, obj, ePtr);
        }
    }

#if !LUA_5_2_OR_LATER
    /// <inheritdoc cref="luaL_typerror(lua_State*, int, byte*)"/>
    public static int luaL_typerror(lua_State* L, int narg, string tname)
    {
        var tnamePtr = tname == null ? null : (byte*)Marshal.StringToHGlobalAnsi(tname);
        try
        {
            return luaL_typerror(L, narg, tnamePtr);
        }
        finally
        {
            if (tnamePtr != null) Marshal.FreeHGlobal((nint)tnamePtr);
        }
    }

    /// <inheritdoc cref="luaL_typerror(lua_State*, int, byte*)"/>
    public static int luaL_typerror(lua_State* L, int narg, ReadOnlySpan<byte> tname)
    {
        fixed (byte* tnamePtr = tname)
        {
            return luaL_typerror(L, narg, tnamePtr);
        }
    }
#endif

    /// <inheritdoc cref="luaL_argerror(lua_State*, int, byte*)"/>
    public static int luaL_argerror(lua_State* L, int numarg, string extramsg)
    {
        var extramsgPtr = extramsg == null ? null : (byte*)Marshal.StringToHGlobalAnsi(extramsg);
        try
        {
            return luaL_argerror(L, numarg, extramsgPtr);
        }
        finally
        {
            if (extramsgPtr != null) Marshal.FreeHGlobal((nint)extramsgPtr);
        }
    }

    /// <inheritdoc cref="luaL_argerror(lua_State*, int, byte*)"/>
    public static int luaL_argerror(lua_State* L, int numarg, ReadOnlySpan<byte> extramsg)
    {
        fixed (byte* extramsgPtr = extramsg)
        {
            return luaL_argerror(L, numarg, extramsgPtr);
        }
    }

    /// <inheritdoc cref="luaL_checklstring(lua_State*, int, nuint*)"/>
    public static string? luaL_checklstring(lua_State* L, int numArg, out nuint length)
    {
        nuint l;
        var result = luaL_checklstring(L, numArg, &l);
        length = l;
        return result == null ? null : Marshal.PtrToStringAnsi((nint)result);
    }

    /// <inheritdoc cref="luaL_checklstring(lua_State*, int, nuint*)"/>
    public static string? luaL_checklstring(lua_State* L, int numArg)
    {
        return luaL_checklstring(L, numArg, out _);
    }

    /// <inheritdoc cref="luaL_optlstring(lua_State*, int, byte*, nuint*)"/>
    public static string? luaL_optlstring(lua_State* L, int numArg, string? def, out nuint length)
    {
        var defPtr = def == null ? null : (byte*)Marshal.StringToHGlobalAnsi(def);
        try
        {
            nuint l;
            var result = luaL_optlstring(L, numArg, defPtr, &l);
            length = l;
            return result == null ? null : Marshal.PtrToStringAnsi((nint)result);
        }
        finally
        {
            if (defPtr != null) Marshal.FreeHGlobal((nint)defPtr);
        }
    }

    /// <inheritdoc cref="luaL_optlstring(lua_State*, int, byte*, nuint*)"/>
    public static string? luaL_optlstring(lua_State* L, int numArg, ReadOnlySpan<byte> def, out nuint length)
    {
        fixed (byte* defPtr = def)
        {
            nuint l;
            var result = luaL_optlstring(L, numArg, defPtr, &l);
            length = l;
            return result == null ? null : Marshal.PtrToStringAnsi((nint)result);
        }
    }

    /// <inheritdoc cref="luaL_checkstack(lua_State*, int, byte*)"/>
    public static void luaL_checkstack(lua_State* L, int sz, string msg)
    {
        var msgPtr = msg == null ? null : (byte*)Marshal.StringToHGlobalAnsi(msg);
        try
        {
            luaL_checkstack(L, sz, msgPtr);
        }
        finally
        {
            if (msgPtr != null) Marshal.FreeHGlobal((nint)msgPtr);
        }
    }

    /// <inheritdoc cref="luaL_checkstack(lua_State*, int, byte*)"/>
    public static void luaL_checkstack(lua_State* L, int sz, ReadOnlySpan<byte> msg)
    {
        fixed (byte* msgPtr = msg)
        {
            luaL_checkstack(L, sz, msgPtr);
        }
    }

    /// <inheritdoc cref="luaL_newmetatable(lua_State*, byte*)"/>
    public static int luaL_newmetatable(lua_State* L, string tname)
    {
        var tnamePtr = tname == null ? null : (byte*)Marshal.StringToHGlobalAnsi(tname);
        try
        {
            return luaL_newmetatable(L, tnamePtr);
        }
        finally
        {
            if (tnamePtr != null) Marshal.FreeHGlobal((nint)tnamePtr);
        }
    }

    /// <inheritdoc cref="luaL_newmetatable(lua_State*, byte*)"/>
    public static int luaL_newmetatable(lua_State* L, ReadOnlySpan<byte> tname)
    {
        fixed (byte* tnamePtr = tname)
        {
            return luaL_newmetatable(L, tnamePtr);
        }
    }

    /// <inheritdoc cref="luaL_checkudata(lua_State*, int, byte*)"/>
    public static void* luaL_checkudata(lua_State* L, int ud, string tname)
    {
        var tnamePtr = tname == null ? null : (byte*)Marshal.StringToHGlobalAnsi(tname);
        try
        {
            return luaL_checkudata(L, ud, tnamePtr);
        }
        finally
        {
            if (tnamePtr != null) Marshal.FreeHGlobal((nint)tnamePtr);
        }
    }

    /// <inheritdoc cref="luaL_checkudata(lua_State*, int, byte*)"/>
    public static void* luaL_checkudata(lua_State* L, int ud, ReadOnlySpan<byte> tname)
    {
        fixed (byte* tnamePtr = tname)
        {
            return luaL_checkudata(L, ud, tnamePtr);
        }
    }

    /// <summary>
    /// [-0, +0, v]<br/>
    /// <br/>
    /// Raises an error. The error message format is given by fmt. It also adds at the beginning of the message the file name and the line number where the error occurred, if this information is available.<br/>
    /// <br/>
    /// This function never returns, but it is an idiom to use it in C functions as return luaL_error(args).<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int luaL_error (lua_State *L, const char *fmt, ...);
    /// </code>
    public static int luaL_error(lua_State* L, string fmt)
    {
        var fmtPtr = fmt == null ? null : (byte*)Marshal.StringToHGlobalAnsi(fmt);
        try
        {
            return luaL_error(L, fmtPtr, __arglist());
        }
        finally
        {
            if (fmtPtr != null) Marshal.FreeHGlobal((nint)fmtPtr);
        }
    }

    /// <summary>
    /// [-0, +0, v]<br/>
    /// <br/>
    /// Raises an error. The error message format is given by fmt. It also adds at the beginning of the message the file name and the line number where the error occurred, if this information is available.<br/>
    /// <br/>
    /// This function never returns, but it is an idiom to use it in C functions as return luaL_error(args).<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int luaL_error (lua_State *L, const char *fmt, ...);
    /// </code>
    public static int luaL_error(lua_State* L, ReadOnlySpan<byte> fmt)
    {
        fixed (byte* fmtPtr = fmt)
        {
            return luaL_error(L, fmtPtr, __arglist());
        }
    }
    
#if LUA_5_2_OR_LATER
    /// <summary>
    /// Equivalent to luaL_loadfilex with mode equal to NULL.
    /// </summary>
    /// <code>
    /// int luaL_loadfile (lua_State *L, const char *filename);
    /// </code>
    public static int luaL_loadfile(lua_State* L, byte* filename)
    {
        return luaL_loadfilex(L, filename, null);
    }
#endif

    /// <inheritdoc cref="luaL_loadfile(lua_State*, byte*)"/>
    public static int luaL_loadfile(lua_State* L, string filename)
    {
        var filenamePtr = filename == null ? null : (byte*)Marshal.StringToHGlobalAnsi(filename);
        try
        {
            return luaL_loadfile(L, filenamePtr);
        }
        finally
        {
            if (filenamePtr != null) Marshal.FreeHGlobal((nint)filenamePtr);
        }
    }

    /// <inheritdoc cref="luaL_loadfile(lua_State*, byte*)"/>
    public static int luaL_loadfile(lua_State* L, ReadOnlySpan<byte> filename)
    {
        fixed (byte* filenamePtr = filename)
        {
            return luaL_loadfile(L, filenamePtr);
        }
    }
    
#if LUA_5_2_OR_LATER
    /// <summary>
    /// Equivalent to luaL_loadbufferx with mode equal to NULL.
    /// </summary>
    /// <code>
    /// int luaL_loadbuffer (lua_State *L,
    ///     const char *buff,
    ///     size_t sz,
    ///     const char *name);
    /// </code>
    public static int luaL_loadbuffer(lua_State* L, byte* buff, nuint sz, byte* name)
    {
        return luaL_loadbufferx(L, buff, sz, name, null);
    }
#endif

    /// <inheritdoc cref="luaL_loadbuffer(lua_State*, byte*, nuint, byte*)"/>
    public static int luaL_loadbuffer(lua_State* L, string buff, string name)
    {
        var buffPtr = buff == null ? null : (byte*)Marshal.StringToHGlobalAnsi(buff);
        var namePtr = name == null ? null : (byte*)Marshal.StringToHGlobalAnsi(name);
        try
        {
            var sz = buff != null ? (nuint)Encoding.ASCII.GetByteCount(buff) : 0;
            return luaL_loadbuffer(L, buffPtr, sz, namePtr);
        }
        finally
        {
            if (buffPtr != null) Marshal.FreeHGlobal((nint)buffPtr);
            if (namePtr != null) Marshal.FreeHGlobal((nint)namePtr);
        }
    }

    /// <inheritdoc cref="luaL_loadbuffer(lua_State*, byte*, nuint, byte*)"/>
    public static int luaL_loadbuffer(lua_State* L, ReadOnlySpan<byte> buff, ReadOnlySpan<byte> name)
    {
        fixed (byte* buffPtr = buff)
        fixed (byte* namePtr = name)
        {
            return luaL_loadbuffer(L, buffPtr, (nuint)buff.Length, namePtr);
        }
    }

#if LUA_5_1_OR_LATER
    /// <inheritdoc cref="luaL_loadstring(lua_State*, byte*)"/>
    public static int luaL_loadstring(lua_State* L, string s)
    {
        var sPtr = s == null ? null : (byte*)Marshal.StringToHGlobalAnsi(s);
        try
        {
            return luaL_loadstring(L, sPtr);
        }
        finally
        {
            if (sPtr != null) Marshal.FreeHGlobal((nint)sPtr);
        }
    }

    /// <inheritdoc cref="luaL_loadstring(lua_State*, byte*)"/>
    public static int luaL_loadstring(lua_State* L, ReadOnlySpan<byte> s)
    {
        fixed (byte* sPtr = s)
        {
            return luaL_loadstring(L, sPtr);
        }
    }

    /// <inheritdoc cref="luaL_gsub(lua_State*, byte*, byte*, byte*)"/>
    public static string? luaL_gsub(lua_State* L, string s, string p, string r)
    {
        var sPtr = s == null ? null : (byte*)Marshal.StringToHGlobalAnsi(s);
        var pPtr = p == null ? null : (byte*)Marshal.StringToHGlobalAnsi(p);
        var rPtr = r == null ? null : (byte*)Marshal.StringToHGlobalAnsi(r);
        try
        {
            var result = luaL_gsub(L, sPtr, pPtr, rPtr);
            return result == null ? null : Marshal.PtrToStringAnsi((nint)result);
        }
        finally
        {
            if (sPtr != null) Marshal.FreeHGlobal((nint)sPtr);
            if (pPtr != null) Marshal.FreeHGlobal((nint)pPtr);
            if (rPtr != null) Marshal.FreeHGlobal((nint)rPtr);
        }
    }

    /// <inheritdoc cref="luaL_gsub(lua_State*, byte*, byte*, byte*)"/>
    public static string? luaL_gsub(lua_State* L, ReadOnlySpan<byte> s, ReadOnlySpan<byte> p, ReadOnlySpan<byte> r)
    {
        fixed (byte* sPtr = s)
        fixed (byte* pPtr = p)
        fixed (byte* rPtr = r)
        {
            var result = luaL_gsub(L, sPtr, pPtr, rPtr);
            return result == null ? null : Marshal.PtrToStringAnsi((nint)result);
        }
    }

#if !LUA_5_2_OR_LATER
    /// <inheritdoc cref="luaL_findtable(lua_State*, int, byte*, int)"/>
    public static string? luaL_findtable(lua_State* L, int idx, string fname, int szhint)
    {
        var fnamePtr = fname == null ? null : (byte*)Marshal.StringToHGlobalAnsi(fname);
        try
        {
            var result = luaL_findtable(L, idx, fnamePtr, szhint);
            return result == null ? null : Marshal.PtrToStringAnsi((nint)result);
        }
        finally
        {
            if (fnamePtr != null) Marshal.FreeHGlobal((nint)fnamePtr);
        }
    }

    /// <inheritdoc cref="luaL_findtable(lua_State*, int, byte*, int)"/>
    public static string? luaL_findtable(lua_State* L, int idx, ReadOnlySpan<byte> fname, int szhint)
    {
        fixed (byte* fnamePtr = fname)
        {
            var result = luaL_findtable(L, idx, fnamePtr, szhint);
            return result == null ? null : Marshal.PtrToStringAnsi((nint)result);
        }
    }
#endif
#endif

#if LUA_5_2_OR_LATER
    /// <inheritdoc cref="luaL_fileresult(lua_State*, int, byte*)"/>
    public static int luaL_fileresult(lua_State* L, int stat, string? fname)
    {
        var fnamePtr = fname == null ? null : (byte*)Marshal.StringToHGlobalAnsi(fname);
        try
        {
            return luaL_fileresult(L, stat, fnamePtr);
        }
        finally
        {
            if (fnamePtr != null) Marshal.FreeHGlobal((nint)fnamePtr);
        }
    }

    /// <inheritdoc cref="luaL_fileresult(lua_State*, int, byte*)"/>
    public static int luaL_fileresult(lua_State* L, int stat, ReadOnlySpan<byte> fname)
    {
        fixed (byte* fnamePtr = fname)
        {
            return luaL_fileresult(L, stat, fnamePtr);
        }
    }

    /// <inheritdoc cref="luaL_loadfilex(lua_State*, byte*, byte*)"/>
    public static int luaL_loadfilex(lua_State* L, string filename, string? mode)
    {
        var filenamePtr = filename == null ? null : (byte*)Marshal.StringToHGlobalAnsi(filename);
        var modePtr = mode == null ? null : (byte*)Marshal.StringToHGlobalAnsi(mode);
        try
        {
            return luaL_loadfilex(L, filenamePtr, modePtr);
        }
        finally
        {
            if (filenamePtr != null) Marshal.FreeHGlobal((nint)filenamePtr);
            if (modePtr != null) Marshal.FreeHGlobal((nint)modePtr);
        }
    }

    /// <inheritdoc cref="luaL_loadfilex(lua_State*, byte*, byte*)"/>
    public static int luaL_loadfilex(lua_State* L, ReadOnlySpan<byte> filename, ReadOnlySpan<byte> mode)
    {
        fixed (byte* filenamePtr = filename)
        fixed (byte* modePtr = mode)
        {
            return luaL_loadfilex(L, filenamePtr, modePtr);
        }
    }

    /// <inheritdoc cref="luaL_loadbufferx(lua_State*, byte*, nuint, byte*, byte*)"/>
    public static int luaL_loadbufferx(lua_State* L, string buff, string name, string? mode)
    {
        var buffPtr = buff == null ? null : (byte*)Marshal.StringToHGlobalAnsi(buff);
        var namePtr = name == null ? null : (byte*)Marshal.StringToHGlobalAnsi(name);
        var modePtr = mode == null ? null : (byte*)Marshal.StringToHGlobalAnsi(mode);
        try
        {
            var sz = buff != null ? (nuint)Encoding.ASCII.GetByteCount(buff) : 0;
            return luaL_loadbufferx(L, buffPtr, sz, namePtr, modePtr);
        }
        finally
        {
            if (buffPtr != null) Marshal.FreeHGlobal((nint)buffPtr);
            if (namePtr != null) Marshal.FreeHGlobal((nint)namePtr);
            if (modePtr != null) Marshal.FreeHGlobal((nint)modePtr);
        }
    }

    /// <inheritdoc cref="luaL_loadbufferx(lua_State*, byte*, nuint, byte*, byte*)"/>
    public static int luaL_loadbufferx(lua_State* L, ReadOnlySpan<byte> buff, ReadOnlySpan<byte> name, ReadOnlySpan<byte> mode)
    {
        fixed (byte* buffPtr = buff)
        fixed (byte* namePtr = name)
        fixed (byte* modePtr = mode)
        {
            return luaL_loadbufferx(L, buffPtr, (nuint)buff.Length, namePtr, modePtr);
        }
    }

    /// <inheritdoc cref="luaL_traceback(lua_State*, lua_State*, byte*, int)"/>
    public static void luaL_traceback(lua_State* L, lua_State* L1, string? msg, int level)
    {
        var msgPtr = msg == null ? null : (byte*)Marshal.StringToHGlobalAnsi(msg);
        try
        {
            luaL_traceback(L, L1, msgPtr, level);
        }
        finally
        {
            if (msgPtr != null) Marshal.FreeHGlobal((nint)msgPtr);
        }
    }

    /// <inheritdoc cref="luaL_traceback(lua_State*, lua_State*, byte*, int)"/>
    public static void luaL_traceback(lua_State* L, lua_State* L1, ReadOnlySpan<byte> msg, int level)
    {
        fixed (byte* msgPtr = msg)
        {
            luaL_traceback(L, L1, msgPtr, level);
        }
    }

#if !LUA_5_2_OR_LATER
    /// <inheritdoc cref="luaL_pushmodule(lua_State*, byte*, int)"/>
    public static void luaL_pushmodule(lua_State* L, string modname, int sizehint)
    {
        var modnamePtr = modname == null ? null : (byte*)Marshal.StringToHGlobalAnsi(modname);
        try
        {
            luaL_pushmodule(L, modnamePtr, sizehint);
        }
        finally
        {
            if (modnamePtr != null) Marshal.FreeHGlobal((nint)modnamePtr);
        }
    }

    /// <inheritdoc cref="luaL_pushmodule(lua_State*, byte*, int)"/>
    public static void luaL_pushmodule(lua_State* L, ReadOnlySpan<byte> modname, int sizehint)
    {
        fixed (byte* modnamePtr = modname)
        {
            luaL_pushmodule(L, modnamePtr, sizehint);
        }
    }
#endif

    /// <inheritdoc cref="luaL_testudata(lua_State*, int, byte*)"/>
    public static void* luaL_testudata(lua_State* L, int ud, string tname)
    {
        var tnamePtr = tname == null ? null : (byte*)Marshal.StringToHGlobalAnsi(tname);
        try
        {
            return luaL_testudata(L, ud, tnamePtr);
        }
        finally
        {
            if (tnamePtr != null) Marshal.FreeHGlobal((nint)tnamePtr);
        }
    }

    /// <inheritdoc cref="luaL_testudata(lua_State*, int, byte*)"/>
    public static void* luaL_testudata(lua_State* L, int ud, ReadOnlySpan<byte> tname)
    {
        fixed (byte* tnamePtr = tname)
        {
            return luaL_testudata(L, ud, tnamePtr);
        }
    }

    /// <inheritdoc cref="luaL_setmetatable(lua_State*, byte*)"/>
    public static void luaL_setmetatable(lua_State* L, string tname)
    {
        var tnamePtr = tname == null ? null : (byte*)Marshal.StringToHGlobalAnsi(tname);
        try
        {
            luaL_setmetatable(L, tnamePtr);
        }
        finally
        {
            if (tnamePtr != null) Marshal.FreeHGlobal((nint)tnamePtr);
        }
    }

    /// <inheritdoc cref="luaL_setmetatable(lua_State*, byte*)"/>
    public static void luaL_setmetatable(lua_State* L, ReadOnlySpan<byte> tname)
    {
        fixed (byte* tnamePtr = tname)
        {
            luaL_setmetatable(L, tnamePtr);
        }
    }
#endif

    /// <inheritdoc cref="luaL_addlstring(luaL_Buffer*, byte*, nuint)"/>
    public static void luaL_addlstring(luaL_Buffer* B, string s)
    {
        var sPtr = s == null ? null : (byte*)Marshal.StringToHGlobalAnsi(s);
        try
        {
            var len = s != null ? (nuint)Encoding.ASCII.GetByteCount(s) : 0;
            luaL_addlstring(B, sPtr, len);
        }
        finally
        {
            if (sPtr != null) Marshal.FreeHGlobal((nint)sPtr);
        }
    }

    /// <inheritdoc cref="luaL_addlstring(luaL_Buffer*, byte*, nuint)"/>
    public static void luaL_addlstring(luaL_Buffer* B, ReadOnlySpan<byte> s)
    {
        fixed (byte* sPtr = s)
        {
            luaL_addlstring(B, sPtr, (nuint)s.Length);
        }
    }

    /// <inheritdoc cref="luaL_addstring(luaL_Buffer*, byte*)"/>
    public static void luaL_addstring(luaL_Buffer* B, string s)
    {
        var sPtr = s == null ? null : (byte*)Marshal.StringToHGlobalAnsi(s);
        try
        {
            luaL_addstring(B, sPtr);
        }
        finally
        {
            if (sPtr != null) Marshal.FreeHGlobal((nint)sPtr);
        }
    }

    /// <inheritdoc cref="luaL_addstring(luaL_Buffer*, byte*)"/>
    public static void luaL_addstring(luaL_Buffer* B, ReadOnlySpan<byte> s)
    {
        fixed (byte* sPtr = s)
        {
            luaL_addstring(B, sPtr);
        }
    }

    #endregion

    #region lua_* Core Functions

    /// <inheritdoc cref="lua_typename(lua_State*, int)"/>
    public static string? lua_typename(lua_State* L, int tp)
    {
        var result = _lua_typename(L, tp);
        return result == null ? null : Marshal.PtrToStringAnsi((nint)result);
    }

#if LUA_5_1_OR_LATER
    /// <inheritdoc cref="lua_tolstring(lua_State*, int, nuint*)"/>
    public static string? lua_tolstring(lua_State* L, int idx, out nuint len)
    {
        nuint length;
        var result = lua_tolstring(L, idx, &length);
        len = length;
        return result == null ? null : Marshal.PtrToStringAnsi((nint)result);
    }

    /// <inheritdoc cref="lua_tolstring(lua_State*, int, nuint*)"/>
    public static string? lua_tolstring(lua_State* L, int idx)
    {
        return lua_tolstring(L, idx, out _);
    }
#endif

    /// <summary>
    /// Convenience method equivalent to lua_tolstring with length ignored.
    /// </summary>
    public static string? lua_tostring(lua_State* L, int idx)
    {
#if LUA_5_0
        var result = _lua_tostring(L, idx);
        return result == null ? null : Marshal.PtrToStringAnsi((nint)result);
#else
        return lua_tolstring(L, idx, out _);
#endif
    }
    
    public static int lua_tostringintobuffer(lua_State* L, int idx, Span<byte> buffer)
    {
#if LUA_5_0
        var strPtr = _lua_tostring(L, idx);
        var strPtr1 = strPtr;
        while (*strPtr1 != 0) strPtr1++;
        var len = (nuint)(strPtr1 - strPtr);
#else
        nuint len;
        var strPtr = lua_tolstring(L, idx, &len);
#endif
        if (strPtr == null) return 0;

        var bytesToCopy = (int)Math.Min(len, (nuint)buffer.Length);
        for (int i = 0; i < bytesToCopy; i++)
        {
            buffer[i] = strPtr[i];
        }
        return bytesToCopy;
    }

    /// <inheritdoc cref="lua_pushlstring(lua_State*, byte*, nuint)"/>
    public static void lua_pushlstring(lua_State* L, string s)
    {
        var sPtr = s == null ? null : (byte*)Marshal.StringToHGlobalAnsi(s);
        try
        {
            var len = s != null ? (nuint)Encoding.ASCII.GetByteCount(s) : 0;
            lua_pushlstring(L, sPtr, len);
        }
        finally
        {
            if (sPtr != null) Marshal.FreeHGlobal((nint)sPtr);
        }
    }

    /// <inheritdoc cref="lua_pushlstring(lua_State*, byte*, nuint)"/>
    public static void lua_pushlstring(lua_State* L, ReadOnlySpan<byte> s)
    {
        fixed (byte* sPtr = s)
        {
            lua_pushlstring(L, sPtr, (nuint)s.Length);
        }
    }

    /// <inheritdoc cref="lua_pushstring(lua_State*, byte*)"/>
    public static void lua_pushstring(lua_State* L, string s)
    {
        var sPtr = s == null ? null : (byte*)Marshal.StringToHGlobalAnsi(s);
        try
        {
            lua_pushstring(L, sPtr);
        }
        finally
        {
            if (sPtr != null) Marshal.FreeHGlobal((nint)sPtr);
        }
    }

    /// <inheritdoc cref="lua_pushstring(lua_State*, byte*)"/>
    public static void lua_pushstring(lua_State* L, ReadOnlySpan<byte> s)
    {
        fixed (byte* sPtr = s)
        {
            lua_pushstring(L, sPtr);
        }
    }

#if LUA_5_1_OR_LATER
#if LUA_5_3_OR_LATER
    /// <inheritdoc cref="lua_getfield(lua_State*, int, byte*)"/>
    public static int lua_getfield(lua_State* L, int idx, string k)
    {
        var kPtr = k == null ? null : (byte*)Marshal.StringToHGlobalAnsi(k);
        try
        {
            return lua_getfield(L, idx, kPtr);
        }
        finally
        {
            if (kPtr != null) Marshal.FreeHGlobal((nint)kPtr);
        }
    }

    /// <inheritdoc cref="lua_getfield(lua_State*, int, byte*)"/>
    public static int lua_getfield(lua_State* L, int idx, ReadOnlySpan<byte> k)
    {
        fixed (byte* kPtr = k)
        {
            return lua_getfield(L, idx, kPtr);
        }
    }
#else
    /// <inheritdoc cref="lua_getfield(lua_State*, int, byte*)"/>
    public static void lua_getfield(lua_State* L, int idx, string k)
    {
        var kPtr = k == null ? null : (byte*)Marshal.StringToHGlobalAnsi(k);
        try
        {
            lua_getfield(L, idx, kPtr);
        }
        finally
        {
            if (kPtr != null) Marshal.FreeHGlobal((nint)kPtr);
        }
    }

    /// <inheritdoc cref="lua_getfield(lua_State*, int, byte*)"/>
    public static void lua_getfield(lua_State* L, int idx, ReadOnlySpan<byte> k)
    {
        fixed (byte* kPtr = k)
        {
            lua_getfield(L, idx, kPtr);
        }
    }
#endif

    /// <inheritdoc cref="lua_setfield(lua_State*, int, byte*)"/>
    public static void lua_setfield(lua_State* L, int idx, string k)
    {
        var kPtr = k == null ? null : (byte*)Marshal.StringToHGlobalAnsi(k);
        try
        {
            lua_setfield(L, idx, kPtr);
        }
        finally
        {
            if (kPtr != null) Marshal.FreeHGlobal((nint)kPtr);
        }
    }

    /// <inheritdoc cref="lua_setfield(lua_State*, int, byte*)"/>
    public static void lua_setfield(lua_State* L, int idx, ReadOnlySpan<byte> k)
    {
        fixed (byte* kPtr = k)
        {
            lua_setfield(L, idx, kPtr);
        }
    }
#endif

#if !LUA_5_2_OR_LATER
    /// <summary>
    /// [-0, +1, -]<br/>
    /// <br/>
    /// Loads a Lua chunk. If there are no errors, lua_load pushes the compiled chunk as a Lua function on top of the stack. Otherwise, it pushes an error message. The return values of lua_load are:<br/>
    /// <br/>
    /// - 0: no errors;<br/>
    /// - LUA_ERRSYNTAX: syntax error during pre-compilation;<br/>
    /// - LUA_ERRMEM: memory allocation error.<br/>
    /// <br/>
    /// This function only loads a chunk; it does not run it.<br/>
    /// <br/>
    /// lua_load automatically detects whether the chunk is text or binary, and loads it accordingly (see program luac).<br/>
    /// <br/>
    /// The lua_load function uses a user-supplied reader function to read the chunk (see lua_Reader). The data argument is an opaque value passed to the reader function.<br/>
    /// <br/>
    /// The chunkname argument gives a name to the chunk, which is used for error messages and in debug information (see §3.8).<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int lua_load (lua_State *L,
    /// lua_Reader reader,
    /// void *data,
    /// const char *chunkname);
    /// </code>
    public static int lua_load(lua_State* L, delegate* unmanaged[Cdecl]<lua_State*, void*, nuint*, byte*> reader, void* dt, string chunkname)
    {
        var chunknamePtr = chunkname == null ? null : (byte*)Marshal.StringToHGlobalAnsi(chunkname);
        try
        {
            return lua_load(L, reader, dt, chunknamePtr);
        }
        finally
        {
            if (chunknamePtr != null) Marshal.FreeHGlobal((nint)chunknamePtr);
        }
    }

    /// <summary>
    /// [-0, +1, -]<br/>
    /// <br/>
    /// Loads a Lua chunk. If there are no errors, lua_load pushes the compiled chunk as a Lua function on top of the stack. Otherwise, it pushes an error message. The return values of lua_load are:<br/>
    /// <br/>
    /// - 0: no errors;<br/>
    /// - LUA_ERRSYNTAX: syntax error during pre-compilation;<br/>
    /// - LUA_ERRMEM: memory allocation error.<br/>
    /// <br/>
    /// This function only loads a chunk; it does not run it.<br/>
    /// <br/>
    /// lua_load automatically detects whether the chunk is text or binary, and loads it accordingly (see program luac).<br/>
    /// <br/>
    /// The lua_load function uses a user-supplied reader function to read the chunk (see lua_Reader). The data argument is an opaque value passed to the reader function.<br/>
    /// <br/>
    /// The chunkname argument gives a name to the chunk, which is used for error messages and in debug information (see §3.8).<br/>
    /// <br/>
    /// </summary>
    /// <code>
    /// int lua_load (lua_State *L,
    /// lua_Reader reader,
    /// void *data,
    /// const char *chunkname);
    /// </code>
    public static int lua_load(lua_State* L, delegate* unmanaged[Cdecl]<lua_State*, void*, nuint*, byte*> reader, void* dt, ReadOnlySpan<byte> chunkname)
    {
        fixed (byte* chunknamePtr = chunkname)
        {
            return lua_load(L, reader, dt, chunknamePtr);
        }
    }
#else
    /// <summary>
    /// [-0, +1, -]<br/>
    /// <br/>
    /// Loads a Lua chunk. If there are no errors, lua_load pushes the compiled chunk as a Lua function on top of the stack. Otherwise, it pushes an error message. The return values of lua_load are:<br/>
    /// <br/>
    /// - 0: no errors;<br/>
    /// - LUA_ERRSYNTAX: syntax error during pre-compilation;<br/>
    /// - LUA_ERRMEM: memory allocation error.<br/>
    /// - LUA_ERRGCMM: error while running a __gc metamethod. (This error has no relation with the chunk being loaded. It is generated by the garbage collector.)<br/>
    /// <br/>
    /// The lua_load function uses a user-supplied reader function to read the chunk (see lua_Reader). The data argument is an opaque value passed to the reader function.
    /// <br/> 
    /// The source argument gives a name to the chunk, which is used for error messages and in debug information (see §4.9). <br/>
    /// <br/> 
    /// lua_load automatically detects whether the chunk is text or binary and loads it accordingly (see program luac). The string mode works as in function load, with the addition that a NULL value is equivalent to the string "bt". <br/>
    /// <br/> 
    /// lua_load uses the stack internally, so the reader function should always leave the stack unmodified when returning. <br/>
    /// <br/> 
    /// If the resulting function has one upvalue, this upvalue is set to the value of the global environment stored at index LUA_RIDX_GLOBALS in the registry (see §4.5). When loading main chunks, this upvalue will be the _ENV variable (see §2.2). <br/>
    /// </summary>
    /// <code>
    /// int lua_load (lua_State *L,
    /// lua_Reader reader,
    /// void *data,
    /// const char *source,
    /// const char *mode);
    /// </code>
    public static int lua_load(lua_State* L, delegate* unmanaged[Cdecl]<lua_State*, void*, nuint*, byte*> reader, void* dt, string chunkname, string mode)
    {
        var chunknamePtr = chunkname == null ? null : (byte*)Marshal.StringToHGlobalAnsi(chunkname);
        var modePtr = mode == null ? null : (byte*)Marshal.StringToHGlobalAnsi(mode);
        try
        {
            return lua_load(L, reader, dt, chunknamePtr, modePtr);
        }
        finally
        {
            if (chunknamePtr != null) Marshal.FreeHGlobal((nint)chunknamePtr);
            if (modePtr != null) Marshal.FreeHGlobal((nint)modePtr);
        }
    }
    
    /// <summary>
    /// [-0, +1, -]<br/>
    /// <br/>
    /// Loads a Lua chunk. If there are no errors, lua_load pushes the compiled chunk as a Lua function on top of the stack. Otherwise, it pushes an error message. The return values of lua_load are:<br/>
    /// <br/>
    /// - 0: no errors;<br/>
    /// - LUA_ERRSYNTAX: syntax error during pre-compilation;<br/>
    /// - LUA_ERRMEM: memory allocation error.<br/>
    /// - LUA_ERRGCMM: error while running a __gc metamethod. (This error has no relation with the chunk being loaded. It is generated by the garbage collector.)<br/>
    /// <br/>
    /// The lua_load function uses a user-supplied reader function to read the chunk (see lua_Reader). The data argument is an opaque value passed to the reader function.
    /// <br/> 
    /// The source argument gives a name to the chunk, which is used for error messages and in debug information (see §4.9). <br/>
    /// <br/> 
    /// lua_load automatically detects whether the chunk is text or binary and loads it accordingly (see program luac). The string mode works as in function load, with the addition that a NULL value is equivalent to the string "bt". <br/>
    /// <br/> 
    /// lua_load uses the stack internally, so the reader function should always leave the stack unmodified when returning. <br/>
    /// <br/> 
    /// If the resulting function has one upvalue, this upvalue is set to the value of the global environment stored at index LUA_RIDX_GLOBALS in the registry (see §4.5). When loading main chunks, this upvalue will be the _ENV variable (see §2.2). <br/>
    /// </summary>
    /// <code>
    /// int lua_load (lua_State *L,
    /// lua_Reader reader,
    /// void *data,
    /// const char *source,
    /// const char *mode);
    /// </code>
    public static int lua_load(lua_State* L, delegate* unmanaged[Cdecl]<lua_State*, void*, nuint*, byte*> reader, void* dt, ReadOnlySpan<byte> source, ReadOnlySpan<byte> mode)
    {
        fixed (byte* chunknamePtr = source)
        fixed (byte* modePtr = mode)
        {
            return lua_load(L, reader, dt, chunknamePtr, modePtr);
        }
    }
#endif

    /// <inheritdoc cref="_lua_getlocal"/>
    public static string? lua_getlocal(lua_State* L, lua_Debug* ar, int n)
    {
        var result = _lua_getlocal(L, ar, n);
        return result == null ? null : Marshal.PtrToStringAnsi((nint)result);
    }
    
    /// <inheritdoc cref="_lua_setlocal"/>
    public static string? lua_setlocal(lua_State* L, lua_Debug* ar, int n)
    {
        var result = _lua_setlocal(L, ar, n);
        return result == null ? null : Marshal.PtrToStringAnsi((nint)result);
    }

    /// <inheritdoc cref="_lua_getupvalue"/>
    public static string? lua_getupvalue(lua_State* L, int funcindex, int n)
    {
        var result = _lua_getupvalue(L, funcindex, n);
        return result == null ? null : Marshal.PtrToStringAnsi((nint)result);
    }

    /// <inheritdoc cref="_lua_setupvalue"/>
    public static string? lua_setupvalue(lua_State* L, int funcindex, int n)
    {
        var result = _lua_setupvalue(L, funcindex, n);
        return result == null ? null : Marshal.PtrToStringAnsi((nint)result);
    }

#if LUA_5_2_OR_LATER && LUAJIT
    public static int lua_loadx(lua_State* L, delegate* unmanaged[Cdecl]<lua_State*, void*, nuint*, byte*> reader, void* dt, string chunkname, string? mode)
    {
        var chunknamePtr = chunkname == null ? null : (byte*)Marshal.StringToHGlobalAnsi(chunkname);
        var modePtr = mode == null ? null : (byte*)Marshal.StringToHGlobalAnsi(mode);
        try
        {
            return lua_loadx(L, reader, dt, chunknamePtr, modePtr);
        }
        finally
        {
            if (chunknamePtr != null) Marshal.FreeHGlobal((nint)chunknamePtr);
            if (modePtr != null) Marshal.FreeHGlobal((nint)modePtr);
        }
    }

    public static int lua_loadx(lua_State* L, delegate* unmanaged[Cdecl]<lua_State*, void*, nuint*, byte*> reader, void* dt, ReadOnlySpan<byte> chunkname, ReadOnlySpan<byte> mode)
    {
        fixed (byte* chunknamePtr = chunkname)
        fixed (byte* modePtr = mode)
        {
            return lua_loadx(L, reader, dt, chunknamePtr, modePtr);
        }
    }
#endif

    #endregion

#if LUAJIT
    #region LuaJIT Profiling Functions

    public static void luaJIT_profile_start(lua_State* L, string mode, delegate* unmanaged[Cdecl]<void*, lua_State*, int, int, void> cb, void* data)
    {
        var modePtr = mode == null ? null : (byte*)Marshal.StringToHGlobalAnsi(mode);
        try
        {
            luaJIT_profile_start(L, modePtr, cb, data);
        }
        finally
        {
            if (modePtr != null) Marshal.FreeHGlobal((nint)modePtr);
        }
    }

    public static void luaJIT_profile_start(lua_State* L, ReadOnlySpan<byte> mode, delegate* unmanaged[Cdecl]<void*, lua_State*, int, int, void> cb, void* data)
    {
        fixed (byte* modePtr = mode)
        {
            luaJIT_profile_start(L, modePtr, cb, data);
        }
    }

    public static string? luaJIT_profile_dumpstack(lua_State* L, string fmt, int depth, out nuint len)
    {
        var fmtPtr = fmt == null ? null : (byte*)Marshal.StringToHGlobalAnsi(fmt);
        try
        {
            nuint length;
            var result = luaJIT_profile_dumpstack(L, fmtPtr, depth, &length);
            len = length;
            return result == null ? null : Marshal.PtrToStringAnsi((nint)result);
        }
        finally
        {
            if (fmtPtr != null) Marshal.FreeHGlobal((nint)fmtPtr);
        }
    }

    public static string? luaJIT_profile_dumpstack(lua_State* L, ReadOnlySpan<byte> fmt, int depth, out nuint len)
    {
        fixed (byte* fmtPtr = fmt)
        {
            nuint length;
            var result = luaJIT_profile_dumpstack(L, fmtPtr, depth, &length);
            len = length;
            return result == null ? null : Marshal.PtrToStringAnsi((nint)result);
        }
    }

    #endregion
#endif

    /// <inheritdoc cref="lua_getinfo(lua_State*, byte*, lua_Debug*)"/>
    public static int lua_getinfo(lua_State* L, ReadOnlySpan<byte> what, lua_Debug* ar)
    {
        fixed (byte* whatPtr = what)
        {
            return lua_getinfo(L, whatPtr, ar);
        }
    }

    /// <inheritdoc cref="lua_getinfo(lua_State*, byte*, lua_Debug*)"/>
    public static int lua_getinfo(lua_State* L, string what, lua_Debug* ar)
    {
        var whatPtr = what == null ? null : (byte*)Marshal.StringToHGlobalAnsi(what);
        try
        {
            return lua_getinfo(L, whatPtr, ar);
        }
        finally
        {
            if (whatPtr != null) Marshal.FreeHGlobal((nint)whatPtr);
        }
    }
    
#if LUA_5_3_OR_LATER
    /// <inheritdoc cref="lua_stringtonumber(lua_State*, byte*)"/>
    public static nuint lua_stringtonumber(lua_State* L, ReadOnlySpan<byte> s)
    {
        fixed (byte* sPtr = s)
        {
            return lua_stringtonumber(L, sPtr);
        }
    }
    
    /// <inheritdoc cref="lua_stringtonumber(lua_State*, byte*)"/>
    public static nuint lua_stringtonumber(lua_State* L, string s)
    {
        var sPtr = s == null ? null : (byte*)Marshal.StringToHGlobalAnsi(s);
        try
        {
            return lua_stringtonumber(L, sPtr);
        }
        finally
        {
            if (sPtr != null) Marshal.FreeHGlobal((nint)sPtr);
        }
    }
#endif
    
#if LUA_5_1_OR_LATER
    /// <inheritdoc cref="luaL_checkoption(lua_State*, int, byte*, byte**)"/>
    public static int luaL_checkoption(lua_State* L, int arg, string def, ReadOnlySpan<string> lst)
    {
        Span<IntPtr> ptrs = stackalloc IntPtr[lst.Length];
        for (int i = 0; i < lst.Length; i++)
        {
            ptrs[i] = Marshal.StringToHGlobalAnsi(lst[i]);
        }

        var ptrDef = Marshal.StringToHGlobalAnsi(def);
        try
        {
            fixed (void* pList = ptrs)
            {
                return luaL_checkoption(L, arg, (byte*)ptrDef, (byte**)pList);
            }
        }
        finally
        {
            foreach (var ptr in ptrs)
                Marshal.FreeHGlobal(ptr);
            Marshal.FreeHGlobal(ptrDef);
        }
    }
#endif
    
#if LUA_5_2_OR_LATER
    /// <inheritdoc cref="luaL_getsubtable(lua_State*, int, byte*)"/>
    public static int luaL_getsubtable(lua_State* L, int idx, ReadOnlySpan<byte> fname)
    {
        fixed (byte* fnamePtr = fname)
        {
            return luaL_getsubtable(L, idx, fnamePtr);
        }
    }
    
    /// <inheritdoc cref="luaL_getsubtable(lua_State*, int, byte*)"/>
    public static int luaL_getsubtable(lua_State* L, int idx, string fname)
    {
        var fnamePtr = fname == null ? null : (byte*)Marshal.StringToHGlobalAnsi(fname);
        try
        {
            return luaL_getsubtable(L, idx, fnamePtr);
        }
        finally
        {
            if (fnamePtr != null) Marshal.FreeHGlobal((nint)fnamePtr);
        }
    }

    /// <inheritdoc cref="luaL_loadbufferx(lua_State*, byte*, nuint, byte*, byte*)"/>
    public static int luaL_loadbufferx(lua_State* L, ReadOnlySpan<byte> buff, size_t sz, ReadOnlySpan<byte> name, ReadOnlySpan<byte> mode)
    {
        fixed (byte* buffPtr = buff)
        fixed (byte* namePtr = name)
        fixed (byte* modePtr = mode)
        {
            return luaL_loadbufferx(L, buffPtr, sz, namePtr, modePtr);
        }
    }

    /// <inheritdoc cref="luaL_loadbufferx(lua_State*, byte*, nuint, byte*, byte*)"/>
    public static int luaL_loadbufferx(lua_State* L, ReadOnlySpan<byte> buff, size_t sz, string name, string mode)
    {
        var namePtr = name == null ? null : (byte*)Marshal.StringToHGlobalAnsi(name);
        var modePtr = mode == null ? null : (byte*)Marshal.StringToHGlobalAnsi(mode);
        try
        {
            fixed (byte* buffPtr = buff)
                return luaL_loadbufferx(L, buffPtr, sz, namePtr, modePtr);
        }
        finally
        {
            if (namePtr != null) Marshal.FreeHGlobal((nint)namePtr);
            if (modePtr != null) Marshal.FreeHGlobal((nint)modePtr);
        }
    }

    /// <inheritdoc cref="luaL_loadbufferx(lua_State*, byte*, nuint, byte*, byte*)"/>
    public static int luaL_loadbufferx(lua_State* L, string buff, size_t sz, string name, string mode)
    {
        var buffPtr = buff == null ? null : (byte*)Marshal.StringToHGlobalAnsi(buff);
        var namePtr = name == null ? null : (byte*)Marshal.StringToHGlobalAnsi(name);
        var modePtr = mode == null ? null : (byte*)Marshal.StringToHGlobalAnsi(mode);
        try
        {
            return luaL_loadbufferx(L, buffPtr, sz, namePtr, modePtr);
        }
        finally
        {
            if (buffPtr != null) Marshal.FreeHGlobal((nint)buffPtr);
            if (namePtr != null) Marshal.FreeHGlobal((nint)namePtr);
            if (modePtr != null) Marshal.FreeHGlobal((nint)modePtr);
        }
    }
#endif
    
#if LUA_5_2_OR_LATER
    /// <inheritdoc cref="luaL_requiref(lua_State*, byte*, lua_CFunction, int)"/>
    public static void luaL_requiref(lua_State* L, ReadOnlySpan<byte> modname, lua_CFunction openf, int glb)
    {
        fixed (byte* modnamePtr = modname)
        {
            luaL_requiref(L, modnamePtr, openf, glb);
        }
    }
    
    /// <inheritdoc cref="luaL_requiref(lua_State*, byte*, lua_CFunction, int)"/>
    public static void luaL_requiref(lua_State* L, string modname, lua_CFunction openf, int glb)
    {
        var modnamePtr = modname == null ? null : (byte*)Marshal.StringToHGlobalAnsi(modname);
        try
        {
            luaL_requiref(L, modnamePtr, openf, glb);
        }
        finally
        {
            if (modnamePtr != null) Marshal.FreeHGlobal((nint)modnamePtr);
        }
    }
#endif
    
#if LUA_5_4_OR_LATER
    public static void lua_warning(lua_State* L, ReadOnlySpan<byte> msg, int tocont)
    {
        fixed (byte* msgPtr = msg)
        {
            lua_warning(L, msgPtr, tocont);
        }
    }
    
    public static void lua_warning(lua_State* L, string msg, int tocont)
    {
        var msgPtr = msg == null ? null : (byte*)Marshal.StringToHGlobalAnsi(msg);
        try
        {
            lua_warning(L, msgPtr, tocont);
        }
        finally
        {
            if (msgPtr != null) Marshal.FreeHGlobal((nint)msgPtr);
        }
    }
#endif
}