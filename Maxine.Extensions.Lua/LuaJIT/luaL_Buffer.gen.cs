using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.LuaJIT.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.LuaJIT.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.LuaJIT.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.LuaJIT.lua_State*, Maxine.Extensions.LuaJIT.lua_Debug*, void>;

// ReSharper disable All

using System.Runtime.CompilerServices;

namespace Maxine.Extensions.LuaJIT
{
    /// <summary>
    /// Type for a string buffer.
    /// A string buffer allows C code to build Lua strings piecemeal. Its pattern of use is as follows:
    /// - First you declare a variable b of type luaL_Buffer.
    /// - Then you initialize it with a call luaL_buffinit(L, &b).
    /// - Then you add string pieces to the buffer calling any of the luaL_add* functions.
    /// - You finish by calling luaL_pushresult(&b). This call leaves the final string on the top of the stack.
    /// During its normal operation, a string buffer uses a variable number of stack slots. So, while using a buffer, you cannot assume that you know where the top of the stack is. You can use the stack between successive calls to buffer operations as long as that use is balanced; that is, when you call a buffer operation, the stack is at the same level it was immediately after the previous buffer operation. (The only exception to this rule is luaL_addvalue.) After calling luaL_pushresult the stack is back to its level when the buffer was initialized, plus the final string on its top.
    /// </summary>
    /// <code>
    /// typedef struct luaL_Buffer luaL_Buffer;
    /// </code>
    public unsafe partial struct luaL_Buffer
    {
        [NativeTypeName("char *")]
        public byte* p;

        public int lvl;

        public lua_State* L;

        [NativeTypeName("char[512]")]
        public _buffer_e__FixedBuffer buffer;

        [InlineArray(512)]
        public partial struct _buffer_e__FixedBuffer
        {
            public byte e0;
        }
    }
}
