using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua55.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua55.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua55.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua55.lua_State*, Maxine.Extensions.Lua55.lua_Debug*, void>;

// ReSharper disable All

using System.Runtime.CompilerServices;

namespace Maxine.Extensions.Lua55
{
    /// <summary>
    /// A structure used to carry different pieces of information about a function or an activation record. lua_getstack fills only the private part of this structure, for later use. To fill the other fields of lua_Debug with useful information, you must call lua_getinfo with an appropriate parameter. (Specifically, to get a field, you must add the letter between parentheses in the field's comment to the parameter what of lua_getinfo.)
    /// The fields of lua_Debug have the following meaning:
    /// - source: the source of the chunk that created the function. If source starts with a '@', it means that the function was defined in a file where the file name follows the '@'. If source starts with a '=', the remainder of its contents describes the source in a user-dependent manner. Otherwise, the function was defined in a string where source is that string.
    /// - srclen: The length of the string source.
    /// - short_src: a "printable" version of source, to be used in error messages.
    /// - linedefined: the line number where the definition of the function starts.
    /// - lastlinedefined: the line number where the definition of the function ends.
    /// - what: the string "Lua" if the function is a Lua function, "C" if it is a C function, "main" if it is the main part of a chunk.
    /// - currentline: the current line where the given function is executing. When no line information is available, currentline is set to -1.
    /// - name: a reasonable name for the given function. Because functions in Lua are first-class values, they do not have a fixed name: some functions can be the value of multiple global variables, while others can be stored only in a table field. The lua_getinfo function checks how the function was called to find a suitable name. If it cannot find a name, then name is set to NULL.
    /// - namewhat: explains the name field. The value of namewhat can be "global", "local", "upvalue", "field", "" (the empty string), plus some other options, according to how the function was called. (Lua uses the empty string when no other option seems to apply.)
    /// - istailcall: true if this function invocation was called by a tail call. In this case, the caller of this level is not in the stack.
    /// - extraargs: The number of extra arguments added by the call to functions called through __call metamethods. (Each __call metavalue adds a single extra argument, the object being called, but there may be a chain of __call metavalues.)
    /// - nups: the number of upvalues of the function.
    /// - nparams: the number of parameters of the function (always 0 for C functions).
    /// - isvararg: true if the function is a variadic function (always true for C functions).
    /// - ftransfer: the index in the stack of the first value being "transferred", that is, parameters in a call or return values in a return. (The other values are in consecutive indices.) Using this index, you can access and modify these values through lua_getlocal and lua_setlocal. This field is only meaningful during a call hook, denoting the first parameter, or a return hook, denoting the first value being returned. (For call hooks, this value is always 1.)
    /// - ntransfer: The number of values being transferred (see previous item). (For calls of Lua functions, this value is always equal to nparams.)
    /// </summary>
    /// <code>
    /// typedef struct lua_Debug {
    /// int event;
    /// const char *name;           /* (n) */
    /// const char *namewhat;       /* (n) */
    /// const char *what;           /* (S) */
    /// const char *source;         /* (S) */
    /// size_t srclen;              /* (S) */
    /// int currentline;            /* (l) */
    /// int linedefined;            /* (S) */
    /// int lastlinedefined;        /* (S) */
    /// unsigned char nups;         /* (u) number of upvalues */
    /// unsigned char nparams;      /* (u) number of parameters */
    /// char isvararg;              /* (u) */
    /// unsigned char extraargs;    /* (t) number of extra arguments */
    /// char istailcall;            /* (t) */
    /// int ftransfer;              /* (r) index of first value transferred */
    /// int ntransfer;              /* (r) number of transferred values */
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

        [NativeTypeName("size_t")]
        public nuint srclen;

        public int currentline;

        public int linedefined;

        public int lastlinedefined;

        [NativeTypeName("unsigned char")]
        public byte nups;

        [NativeTypeName("unsigned char")]
        public byte nparams;

        [NativeTypeName("char")]
        public byte isvararg;

        [NativeTypeName("unsigned char")]
        public byte extraargs;

        [NativeTypeName("char")]
        public byte istailcall;

        public int ftransfer;

        public int ntransfer;

        [NativeTypeName("char[60]")]
        public _short_src_e__FixedBuffer short_src;

        [NativeTypeName("struct CallInfo *")]
        public CallInfo* i_ci;

        public partial struct CallInfo
        {
        }

        [InlineArray(60)]
        public partial struct _short_src_e__FixedBuffer
        {
            public byte e0;
        }
    }
}
