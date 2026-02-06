using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua55.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua55.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua55.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua55.lua_State*, Maxine.Extensions.Lua55.lua_Debug*, void>;

// ReSharper disable All

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maxine.Extensions.Lua55
{
    /// <summary>
    /// Type for a string buffer.
    /// A string buffer allows C code to build Lua strings piecemeal. Its pattern of use is as follows:
    /// - First declare a variable b of type luaL_Buffer.
    /// - Then initialize it with a call luaL_buffinit(L,&b).
    /// - Then add string pieces to the buffer calling any of the luaL_add* functions.
    /// - Finish by calling luaL_pushresult(&b). This call leaves the final string on the top of the stack.
    /// If you know beforehand the maximum size of the resulting string, you can use the buffer like this:
    /// - First declare a variable b of type luaL_Buffer.
    /// - Then initialize it and preallocate a space of size sz with a call luaL_buffinitsize(L,&b,sz).
    /// - Then produce the string into that space.
    /// - Finish by calling luaL_pushresultsize(&b,sz), where sz is the total size of the resulting string copied into that space (which may be less than or equal to the preallocated size).
    /// During its normal operation, a string buffer uses a variable number of stack slots. So, while using a buffer, you cannot assume that you know where the top of the stack is. You can use the stack between successive calls to buffer operations as long as that use is balanced; that is, when you call a buffer operation, the stack is at the same level it was immediately after the previous buffer operation. (The only exception to this rule is luaL_addvalue.) After calling luaL_pushresult, the stack is back to its level when the buffer was initialized, plus the final string on its top.
    /// </summary>
    /// <code>
    /// typedef struct luaL_Buffer luaL_Buffer;
    /// </code>
    public unsafe partial struct luaL_Buffer
    {
        [NativeTypeName("char *")]
        public byte* b;

        [NativeTypeName("size_t")]
        public nuint size;

        [NativeTypeName("size_t")]
        public nuint n;

        public lua_State* L;

        [NativeTypeName("__AnonymousRecord_lauxlib_L190_C3")]
        public _init_e__Union init;

        [StructLayout(LayoutKind.Explicit)]
        public unsafe partial struct _init_e__Union
        {
            [FieldOffset(0)]
            public lua_Number n;

            [FieldOffset(0)]
            public double u;

            [FieldOffset(0)]
            public void* s;

            [FieldOffset(0)]
            public lua_Integer i;

            [FieldOffset(0)]
            [NativeTypeName("long")]
            public nint l;

            [FieldOffset(0)]
            [NativeTypeName("char[1024]")]
            public _b_e__FixedBuffer b;

            [InlineArray(1024)]
            public partial struct _b_e__FixedBuffer
            {
                public byte e0;
            }
        }
    }
}
