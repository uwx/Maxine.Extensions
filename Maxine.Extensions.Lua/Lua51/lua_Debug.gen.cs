using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua51.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua51.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua51.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua51.lua_State*, Maxine.Extensions.Lua51.lua_Debug*, void>;

// ReSharper disable All

using System.Runtime.CompilerServices;

namespace Maxine.Extensions.Lua51
{
    /// <summary>
    /// A structure used to carry different pieces of information about an active function. lua_getstack fills only the private part of this structure, for later use. To fill the other fields of lua_Debug with useful information, call lua_getinfo.
    /// The fields of lua_Debug have the following meaning:
    /// - source: If the function was defined in a string, then source is that string. If the function was defined in a file, then source starts with a '@' followed by the file name.
    /// - short_src: a "printable" version of source, to be used in error messages.
    /// - linedefined: the line number where the definition of the function starts.
    /// - lastlinedefined: the line number where the definition of the function ends.
    /// - what: the string "Lua" if the function is a Lua function, "C" if it is a C function, "main" if it is the main part of a chunk, and "tail" if it was a function that did a tail call. In the latter case, Lua has no other information about the function.
    /// - currentline: the current line where the given function is executing. When no line information is available, currentline is set to -1.
    /// - name: a reasonable name for the given function. Because functions in Lua are first-class values, they do not have a fixed name: some functions can be the value of multiple global variables, while others can be stored only in a table field. The lua_getinfo function checks how the function was called to find a suitable name. If it cannot find a name, then name is set to NULL.
    /// - namewhat: explains the name field. The value of namewhat can be "global", "local", "method", "field", "upvalue", or "" (the empty string), according to how the function was called. (Lua uses the empty string when no other option seems to apply.)
    /// - nups: the number of upvalues of the function.
    /// </summary>
    /// <code>
    /// typedef struct lua_Debug {
    /// int event;
    /// const char *name;           /* (n) */
    /// const char *namewhat;       /* (n) */
    /// const char *what;           /* (S) */
    /// const char *source;         /* (S) */
    /// int currentline;            /* (l) */
    /// int nups;                   /* (u) number of upvalues */
    /// int linedefined;            /* (S) */
    /// int lastlinedefined;        /* (S) */
    /// char short_src[LUA_IDSIZE]; /* (S) */
    /// /* private part */
    /// other fields
    /// } lua_Debug;
    /// </code>
    public unsafe partial struct lua_Debug
    {
        public int @event;

        [NativeTypeName("const char *")]
        public byte* name;

        [NativeTypeName("const char *")]
        public byte* namewhat;

        [NativeTypeName("const char *")]
        public byte* what;

        [NativeTypeName("const char *")]
        public byte* source;

        public int currentline;

        public int nups;

        public int linedefined;

        public int lastlinedefined;

        [NativeTypeName("char[60]")]
        public _short_src_e__FixedBuffer short_src;

        public int i_ci;

        [InlineArray(60)]
        public partial struct _short_src_e__FixedBuffer
        {
            public byte e0;
        }
    }
}
