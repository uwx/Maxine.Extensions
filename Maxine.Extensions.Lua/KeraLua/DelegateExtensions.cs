using System;
using System.Runtime.InteropServices;

namespace KeraLua
{
    static unsafe class DelegateExtensions
    {
        public static LuaFunction? ToLuaFunction(lua_CFunction ptr)
        {
            if ((IntPtr)ptr == IntPtr.Zero)
                return null;

            return Marshal.GetDelegateForFunctionPointer<LuaFunction>((IntPtr)ptr);
        }

        public static lua_CFunction ToFunctionPointer(this LuaFunction? d)
        {
            if (d == null)
                return (lua_CFunction)IntPtr.Zero;

            return (lua_CFunction)Marshal.GetFunctionPointerForDelegate<LuaFunction>(d);
        }

        public static LuaHookFunction? ToLuaHookFunction(lua_Hook ptr)
        {
            if (ptr == (void*)IntPtr.Zero)
                return null;

            return Marshal.GetDelegateForFunctionPointer<LuaHookFunction>((IntPtr)ptr);
        }

        public static lua_Hook ToFunctionPointer(this LuaHookFunction? d)
        {
            if (d == null)
                return (lua_Hook)IntPtr.Zero;

            return (lua_Hook)Marshal.GetFunctionPointerForDelegate<LuaHookFunction>(d);
        }

        public static LuaKFunction? ToLuaKFunction(this IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;

            return Marshal.GetDelegateForFunctionPointer<LuaKFunction>(ptr);
        }

        public static delegate* unmanaged[Cdecl]<lua_State*, int, nint, int> ToFunctionPointer(this LuaKFunction? d)
        {
            if (d == null)
                return (delegate* unmanaged[Cdecl]<lua_State*, int, IntPtr, int>)IntPtr.Zero;

            return (delegate* unmanaged[Cdecl]<lua_State*, int, IntPtr, int>)Marshal.GetFunctionPointerForDelegate<LuaKFunction>(d);
        }

        public static LuaReader? ToLuaReader(lua_Reader ptr)
        {
            if (ptr == (void*)IntPtr.Zero)
                return null;

            return Marshal.GetDelegateForFunctionPointer<LuaReader>((IntPtr)ptr);
        }

        public static lua_Reader ToFunctionPointer(this LuaReader? d)
        {
            if (d == null)
                return (lua_Reader)IntPtr.Zero;

            return (lua_Reader)Marshal.GetFunctionPointerForDelegate<LuaReader>(d);
        }

        public static LuaWriter? ToLuaWriter(this IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;

            return Marshal.GetDelegateForFunctionPointer<LuaWriter>(ptr);
        }

        public static lua_Writer ToFunctionPointer(this LuaWriter? d)
        {
            if (d == null)
                return (lua_Writer)IntPtr.Zero;

            return (lua_Writer)Marshal.GetFunctionPointerForDelegate<LuaWriter>(d);
        }

        public static LuaAlloc? ToLuaAlloc(lua_Alloc ptr)
        {
            if (ptr == (void*)IntPtr.Zero)
                return null;

            return Marshal.GetDelegateForFunctionPointer<LuaAlloc>((IntPtr)ptr);
        }

        public static unsafe delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, UIntPtr, UIntPtr, void*> ToFunctionPointer(this LuaAlloc? d)
        {
            if (d == null)
                return (delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, UIntPtr, UIntPtr, void*>)IntPtr.Zero;

            return (delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, UIntPtr, UIntPtr, void*>)Marshal.GetFunctionPointerForDelegate<LuaAlloc>(d);
        }

        public static LuaWarnFunction? ToLuaWarning(delegate* unmanaged[Cdecl]<void*, byte*, int, void> ptr)
        {
            if (ptr == (void*)IntPtr.Zero)
                return null;

            return Marshal.GetDelegateForFunctionPointer<LuaWarnFunction>((IntPtr)ptr);
        }

        public static delegate* unmanaged[Cdecl]<void*, byte*, int, void> ToFunctionPointer(this LuaWarnFunction? d)
        {
            if (d == null)
                return (delegate* unmanaged[Cdecl]<void*, byte*, int, void>)IntPtr.Zero;

            return (delegate* unmanaged[Cdecl]<void*, byte*, int, void>)Marshal.GetFunctionPointerForDelegate<LuaWarnFunction>(d);
        }
    }
}
