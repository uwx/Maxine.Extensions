using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua54.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua54.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua54.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua54.lua_State*, Maxine.Extensions.Lua54.lua_Debug*, void>;

// ReSharper disable All

using System.Runtime.InteropServices;

namespace Maxine.Extensions.Lua54
{
    public static unsafe partial class Lua54
    {
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaL_checkversion_(lua_State* L, lua_Number ver, [NativeTypeName("size_t")] nuint sz);

        /// <summary>
        /// [-0, +(0|1), m]
        /// Pushes onto the stack the field e from the metatable of the object at index obj and returns the type of the pushed value. If the object does not have a metatable, or if the metatable does not have this field, pushes nothing and returns LUA_TNIL.
        /// </summary>
        /// <code>
        /// int luaL_getmetafield (lua_State *L, int obj, const char *e);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_getmetafield(lua_State* L, int obj, [NativeTypeName("const char *")] byte* e);

        /// <summary>
        /// [-0, +(0|1), e]
        /// Calls a metamethod.
        /// If the object at index obj has a metatable and this metatable has a field e, this function calls this field passing the object as its only argument. In this case this function returns true and pushes onto the stack the value returned by the call. If there is no metatable or no metamethod, this function returns false without pushing any value on the stack.
        /// </summary>
        /// <code>
        /// int luaL_callmeta (lua_State *L, int obj, const char *e);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_callmeta(lua_State* L, int obj, [NativeTypeName("const char *")] byte* e);

        /// <summary>
        /// [-0, +1, e]
        /// Converts any Lua value at the given index to a C string in a reasonable format. The resulting string is pushed onto the stack and also returned by the function (see §4.1.3). If len is not NULL, the function also sets *len with the string length.
        /// If the value has a metatable with a __tostring field, then luaL_tolstring calls the corresponding metamethod with the value as argument, and uses the result of the call as its result.
        /// </summary>
        /// <code>
        /// const char *luaL_tolstring (lua_State *L, int idx, size_t *len);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        public static extern byte* luaL_tolstring(lua_State* L, int idx, [NativeTypeName("size_t *")] nuint* len);

        /// <summary>
        /// [-0, +0, v]
        /// Raises an error reporting a problem with argument arg of the C function that called it, using a standard message that includes extramsg as a comment:
        /// This function never returns.
        /// </summary>
        /// <code>
        /// int luaL_argerror (lua_State *L, int arg, const char *extramsg);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_argerror(lua_State* L, int arg, [NativeTypeName("const char *")] byte* extramsg);

        /// <summary>
        /// [-0, +0, v]
        /// Raises a type error for the argument arg of the C function that called it, using a standard message; tname is a "name" for the expected type. This function never returns.
        /// </summary>
        /// <code>
        /// int luaL_typeerror (lua_State *L, int arg, const char *tname);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_typeerror(lua_State* L, int arg, [NativeTypeName("const char *")] byte* tname);

        /// <summary>
        /// [-0, +0, v]
        /// Checks whether the function argument arg is a string and returns this string; if l is not NULL fills its referent with the string's length.
        /// This function uses lua_tolstring to get its result, so all conversions and caveats of that function apply here.
        /// </summary>
        /// <code>
        /// const char *luaL_checklstring (lua_State *L, int arg, size_t *l);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_checklstring(lua_State* L, int arg, [NativeTypeName("size_t *")] nuint* l);

        /// <summary>
        /// [-0, +0, v]
        /// If the function argument arg is a string, returns this string. If this argument is absent or is nil, returns d. Otherwise, raises an error.
        /// If l is not NULL, fills its referent with the result's length. If the result is NULL (only possible when returning d and d == NULL), its length is considered zero.
        /// This function uses lua_tolstring to get its result, so all conversions and caveats of that function apply here.
        /// </summary>
        /// <code>
        /// const char *luaL_optlstring (lua_State *L,
        /// int arg,
        /// const char *d,
        /// size_t *l);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_optlstring(lua_State* L, int arg, [NativeTypeName("const char *")] byte* def, [NativeTypeName("size_t *")] nuint* l);

        /// <summary>
        /// [-0, +0, v]
        /// Checks whether the function argument arg is a number and returns this number converted to a lua_Number.
        /// </summary>
        /// <code>
        /// lua_Number luaL_checknumber (lua_State *L, int arg);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Number luaL_checknumber(lua_State* L, int arg);

        /// <summary>
        /// [-0, +0, v]
        /// If the function argument arg is a number, returns this number as a lua_Number. If this argument is absent or is nil, returns d. Otherwise, raises an error.
        /// </summary>
        /// <code>
        /// lua_Number luaL_optnumber (lua_State *L, int arg, lua_Number d);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Number luaL_optnumber(lua_State* L, int arg, lua_Number def);

        /// <summary>
        /// [-0, +0, v]
        /// Checks whether the function argument arg is an integer (or can be converted to an integer) and returns this integer.
        /// </summary>
        /// <code>
        /// lua_Integer luaL_checkinteger (lua_State *L, int arg);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Integer luaL_checkinteger(lua_State* L, int arg);

        /// <summary>
        /// [-0, +0, v]
        /// If the function argument arg is an integer (or it is convertible to an integer), returns this integer. If this argument is absent or is nil, returns d. Otherwise, raises an error.
        /// </summary>
        /// <code>
        /// lua_Integer luaL_optinteger (lua_State *L,
        /// int arg,
        /// lua_Integer d);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Integer luaL_optinteger(lua_State* L, int arg, lua_Integer def);

        /// <summary>
        /// [-0, +0, v]
        /// Grows the stack size to top + sz elements, raising an error if the stack cannot grow to that size. msg is an additional text to go into the error message (or NULL for no additional text).
        /// </summary>
        /// <code>
        /// void luaL_checkstack (lua_State *L, int sz, const char *msg);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_checkstack(lua_State* L, int sz, [NativeTypeName("const char *")] byte* msg);

        /// <summary>
        /// [-0, +0, v]
        /// Checks whether the function argument arg has type t. See lua_type for the encoding of types for t.
        /// </summary>
        /// <code>
        /// void luaL_checktype (lua_State *L, int arg, int t);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_checktype(lua_State* L, int arg, int t);

        /// <summary>
        /// [-0, +0, v]
        /// Checks whether the function has an argument of any type (including nil) at position arg.
        /// </summary>
        /// <code>
        /// void luaL_checkany (lua_State *L, int arg);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_checkany(lua_State* L, int arg);

        /// <summary>
        /// [-0, +1, m]
        /// If the registry already has the key tname, returns 0. Otherwise, creates a new table to be used as a metatable for userdata, adds to this new table the pair __name = tname, adds to the registry the pair [tname] = new table, and returns 1.
        /// In both cases, the function pushes onto the stack the final value associated with tname in the registry.
        /// </summary>
        /// <code>
        /// int luaL_newmetatable (lua_State *L, const char *tname);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_newmetatable(lua_State* L, [NativeTypeName("const char *")] byte* tname);

        /// <summary>
        /// [-0, +0, –]
        /// Sets the metatable of the object on the top of the stack as the metatable associated with name tname in the registry (see luaL_newmetatable).
        /// </summary>
        /// <code>
        /// void luaL_setmetatable (lua_State *L, const char *tname);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_setmetatable(lua_State* L, [NativeTypeName("const char *")] byte* tname);

        /// <summary>
        /// [-0, +0, m]
        /// This function works like luaL_checkudata, except that, when the test fails, it returns NULL instead of raising an error.
        /// </summary>
        /// <code>
        /// void *luaL_testudata (lua_State *L, int arg, const char *tname);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void* luaL_testudata(lua_State* L, int ud, [NativeTypeName("const char *")] byte* tname);

        /// <summary>
        /// [-0, +0, v]
        /// Checks whether the function argument arg is a userdata of the type tname (see luaL_newmetatable) and returns the userdata's memory-block address (see lua_touserdata).
        /// </summary>
        /// <code>
        /// void *luaL_checkudata (lua_State *L, int arg, const char *tname);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void* luaL_checkudata(lua_State* L, int ud, [NativeTypeName("const char *")] byte* tname);

        /// <summary>
        /// [-0, +1, m]
        /// Pushes onto the stack a string identifying the current position of the control at level lvl in the call stack. Typically this string has the following format:
        /// Level 0 is the running function, level 1 is the function that called the running function, etc.
        /// This function is used to build a prefix for error messages.
        /// The standard Lua libraries provide useful functions that are implemented in C through the C API. Some of these functions provide essential services to the language (e.g., type and getmetatable); others provide access to outside services (e.g., I/O); and others could be implemented in Lua itself, but that for different reasons deserve an implementation in C (e.g., table.sort).
        /// All libraries are implemented through the official C API and are provided as separate C modules. Unless otherwise noted, these library functions do not adjust its number of arguments to its expected parameters. For instance, a function documented as foo(arg) should not be called without an argument.
        /// The notation fail means a false value representing some kind of failure. (Currently, fail is equal to nil, but that may change in future versions. The recommendation is to always test the success of these functions with (not status), instead of (status == nil).)
        /// Currently, Lua has the following standard libraries:
        /// - basic library (§6.1);
        /// - coroutine library (§6.2);
        /// - package library (§6.3);
        /// - string manipulation (§6.4);
        /// - basic UTF-8 support (§6.5);
        /// - table manipulation (§6.6);
        /// - mathematical functions (§6.7) (sin, log, etc.);
        /// - input and output (§6.8);
        /// - operating system facilities (§6.9);
        /// - debug facilities (§6.10).
        /// Except for the basic and the package libraries, each library provides all its functions as fields of a global table or as methods of its objects.
        /// To have access to these libraries, the C host program should call the luaL_openlibs function, which opens all standard libraries. Alternatively, the host program can open them individually by using luaL_requiref to call luaopen_base (for the basic library), luaopen_package (for the package library), luaopen_coroutine (for the coroutine library), luaopen_string (for the string library), luaopen_utf8 (for the UTF-8 library), luaopen_table (for the table library), luaopen_math (for the mathematical library), luaopen_io (for the I/O library), luaopen_os (for the operating system library), and luaopen_debug (for the debug library). These functions are declared in lualib.h.
        /// </summary>
        /// <code>
        /// void luaL_where (lua_State *L, int lvl);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_where(lua_State* L, int lvl);

        /// <summary>
        /// [-0, +0, v]
        /// Checks whether the function argument arg is a string and searches for this string in the array lst (which must be NULL-terminated). Returns the index in the array where the string was found. Raises an error if the argument is not a string or if the string cannot be found.
        /// If def is not NULL, the function uses def as a default value when there is no argument arg or when this argument is nil.
        /// This is a useful function for mapping strings to C enums. (The usual convention in Lua libraries is to use strings instead of numbers to select options.)
        /// </summary>
        /// <code>
        /// int luaL_checkoption (lua_State *L,
        /// int arg,
        /// const char *def,
        /// const char *const lst[]);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_checkoption(lua_State* L, int arg, [NativeTypeName("const char *")] byte* def, [NativeTypeName("const char *const[]")] byte** lst);

        /// <summary>
        /// [-0, +(1|3), m]
        /// This function produces the return values for file-related functions in the standard library (io.open, os.rename, file:seek, etc.).
        /// </summary>
        /// <code>
        /// int luaL_fileresult (lua_State *L, int stat, const char *fname);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_fileresult(lua_State* L, int stat, [NativeTypeName("const char *")] byte* fname);

        /// <summary>
        /// [-0, +3, m]
        /// This function produces the return values for process-related functions in the standard library (os.execute and io.close).
        /// </summary>
        /// <code>
        /// int luaL_execresult (lua_State *L, int stat);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_execresult(lua_State* L, int stat);

        /// <summary>
        /// [-1, +0, m]
        /// Creates and returns a reference, in the table at index t, for the object on the top of the stack (and pops the object).
        /// A reference is a unique integer key. As long as you do not manually add integer keys into the table t, luaL_ref ensures the uniqueness of the key it returns. You can retrieve an object referred by the reference r by calling lua_rawgeti(L, t, r). The function luaL_unref frees a reference.
        /// If the object on the top of the stack is nil, luaL_ref returns the constant LUA_REFNIL. The constant LUA_NOREF is guaranteed to be different from any reference returned by luaL_ref.
        /// </summary>
        /// <code>
        /// int luaL_ref (lua_State *L, int t);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_ref(lua_State* L, int t);

        /// <summary>
        /// [-0, +0, –]
        /// Releases the reference ref from the table at index t (see luaL_ref). The entry is removed from the table, so that the referred object can be collected. The reference ref is also freed to be used again.
        /// If ref is LUA_NOREF or LUA_REFNIL, luaL_unref does nothing.
        /// </summary>
        /// <code>
        /// void luaL_unref (lua_State *L, int t, int ref);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_unref(lua_State* L, int t, int @ref);

        /// <summary>
        /// [-0, +1, m]
        /// Loads a file as a Lua chunk. This function uses lua_load to load the chunk in the file named filename. If filename is NULL, then it loads from the standard input. The first line in the file is ignored if it starts with a #.
        /// The string mode works as in the function lua_load.
        /// This function returns the same results as lua_load or LUA_ERRFILE for file-related errors.
        /// As lua_load, this function only loads the chunk; it does not run it.
        /// </summary>
        /// <code>
        /// int luaL_loadfilex (lua_State *L, const char *filename,
        /// const char *mode);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_loadfilex(lua_State* L, [NativeTypeName("const char *")] byte* filename, [NativeTypeName("const char *")] byte* mode);

        /// <summary>
        /// [-0, +1, –]
        /// Loads a buffer as a Lua chunk. This function uses lua_load to load the chunk in the buffer pointed to by buff with size sz.
        /// This function returns the same results as lua_load. name is the chunk name, used for debug information and error messages. The string mode works as in the function lua_load.
        /// </summary>
        /// <code>
        /// int luaL_loadbufferx (lua_State *L,
        /// const char *buff,
        /// size_t sz,
        /// const char *name,
        /// const char *mode);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_loadbufferx(lua_State* L, [NativeTypeName("const char *")] byte* buff, [NativeTypeName("size_t")] nuint sz, [NativeTypeName("const char *")] byte* name, [NativeTypeName("const char *")] byte* mode);

        /// <summary>
        /// [-0, +1, –]
        /// Loads a string as a Lua chunk. This function uses lua_load to load the chunk in the zero-terminated string s.
        /// This function returns the same results as lua_load.
        /// Also as lua_load, this function only loads the chunk; it does not run it.
        /// </summary>
        /// <code>
        /// int luaL_loadstring (lua_State *L, const char *s);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_loadstring(lua_State* L, [NativeTypeName("const char *")] byte* s);

        /// <summary>
        /// [-0, +0, –]
        /// Creates a new Lua state. It calls lua_newstate with an allocator based on the ISO C allocation functions and then sets a warning function and a panic function (see §4.4) that print messages to the standard error output.
        /// Returns the new state, or NULL if there is a memory allocation error.
        /// </summary>
        /// <code>
        /// lua_State *luaL_newstate (void);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern lua_State* luaL_newstate();

        /// <summary>
        /// [-0, +0, e]
        /// Returns the "length" of the value at the given index as a number; it is equivalent to the '#' operator in Lua (see §3.4.7). Raises an error if the result of the operation is not an integer. (This case can only happen through metamethods.)
        /// </summary>
        /// <code>
        /// lua_Integer luaL_len (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern lua_Integer luaL_len(lua_State* L, int idx);

        /// <summary>
        /// [-?, +?, m]
        /// Adds a copy of the string s to the buffer B (see luaL_Buffer), replacing any occurrence of the string p with the string r.
        /// </summary>
        /// <code>
        /// const void luaL_addgsub (luaL_Buffer *B, const char *s,
        /// const char *p, const char *r);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaL_addgsub(luaL_Buffer* b, [NativeTypeName("const char *")] byte* s, [NativeTypeName("const char *")] byte* p, [NativeTypeName("const char *")] byte* r);

        /// <summary>
        /// [-0, +1, m]
        /// Creates a copy of string s, replacing any occurrence of the string p with the string r. Pushes the resulting string on the stack and returns it.
        /// </summary>
        /// <code>
        /// const char *luaL_gsub (lua_State *L,
        /// const char *s,
        /// const char *p,
        /// const char *r);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_gsub(lua_State* L, [NativeTypeName("const char *")] byte* s, [NativeTypeName("const char *")] byte* p, [NativeTypeName("const char *")] byte* r);

        /// <summary>
        /// [-nup, +0, m]
        /// Registers all functions in the array l (see luaL_Reg) into the table on the top of the stack (below optional upvalues, see next).
        /// When nup is not zero, all functions are created with nup upvalues, initialized with copies of the nup values previously pushed on the stack on top of the library table. These values are popped from the stack after the registration.
        /// A function with a NULL value represents a placeholder, which is filled with false.
        /// </summary>
        /// <code>
        /// void luaL_setfuncs (lua_State *L, const luaL_Reg *l, int nup);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_setfuncs(lua_State* L, [NativeTypeName("const luaL_Reg *")] luaL_Reg* l, int nup);

        /// <summary>
        /// [-0, +1, e]
        /// Ensures that the value t[fname], where t is the value at index idx, is a table, and pushes that table onto the stack. Returns true if it finds a previous table there and false if it creates a new table.
        /// </summary>
        /// <code>
        /// int luaL_getsubtable (lua_State *L, int idx, const char *fname);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_getsubtable(lua_State* L, int idx, [NativeTypeName("const char *")] byte* fname);

        /// <summary>
        /// [-0, +1, m]
        /// Creates and pushes a traceback of the stack L1. If msg is not NULL, it is appended at the beginning of the traceback. The level parameter tells at which level to start the traceback.
        /// </summary>
        /// <code>
        /// void luaL_traceback (lua_State *L, lua_State *L1, const char *msg,
        /// int level);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaL_traceback(lua_State* L, lua_State* L1, [NativeTypeName("const char *")] byte* msg, int level);

        /// <summary>
        /// [-0, +1, e]
        /// If package.loaded[modname] is not true, calls the function openf with the string modname as an argument and sets the call result to package.loaded[modname], as if that function has been called through require.
        /// If glb is true, also stores the module into the global modname.
        /// Leaves a copy of the module on the stack.
        /// </summary>
        /// <code>
        /// void luaL_requiref (lua_State *L, const char *modname,
        /// lua_CFunction openf, int glb);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaL_requiref(lua_State* L, [NativeTypeName("const char *")] byte* modname, lua_CFunction openf, int glb);

        /// <summary>
        /// [-0, +?, –]
        /// Initializes a buffer B (see luaL_Buffer). This function does not allocate any space; the buffer must be declared as a variable.
        /// </summary>
        /// <code>
        /// void luaL_buffinit (lua_State *L, luaL_Buffer *B);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_buffinit(lua_State* L, luaL_Buffer* B);

        /// <summary>
        /// [-?, +?, m]
        /// Returns an address to a space of size sz where you can copy a string to be added to buffer B (see luaL_Buffer). After copying the string into this space you must call luaL_addsize with the size of the string to actually add it to the buffer.
        /// </summary>
        /// <code>
        /// char *luaL_prepbuffsize (luaL_Buffer *B, size_t sz);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("char *")]
        public static extern byte* luaL_prepbuffsize(luaL_Buffer* B, [NativeTypeName("size_t")] nuint sz);

        /// <summary>
        /// [-?, +?, m]
        /// Adds the string pointed to by s with length l to the buffer B (see luaL_Buffer). The string can contain embedded zeros.
        /// </summary>
        /// <code>
        /// void luaL_addlstring (luaL_Buffer *B, const char *s, size_t l);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_addlstring(luaL_Buffer* B, [NativeTypeName("const char *")] byte* s, [NativeTypeName("size_t")] nuint l);

        /// <summary>
        /// [-?, +?, m]
        /// Adds the zero-terminated string pointed to by s to the buffer B (see luaL_Buffer).
        /// </summary>
        /// <code>
        /// void luaL_addstring (luaL_Buffer *B, const char *s);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_addstring(luaL_Buffer* B, [NativeTypeName("const char *")] byte* s);

        /// <summary>
        /// [-?, +?, m]
        /// Adds the value on the top of the stack to the buffer B (see luaL_Buffer). Pops the value.
        /// This is the only function on string buffers that can (and must) be called with an extra element on the stack, which is the value to be added to the buffer.
        /// </summary>
        /// <code>
        /// void luaL_addvalue (luaL_Buffer *B);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_addvalue(luaL_Buffer* B);

        /// <summary>
        /// [-?, +1, m]
        /// Finishes the use of buffer B leaving the final string on the top of the stack.
        /// </summary>
        /// <code>
        /// void luaL_pushresult (luaL_Buffer *B);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_pushresult(luaL_Buffer* B);

        /// <summary>
        /// [-?, +1, m]
        /// Equivalent to the sequence luaL_addsize, luaL_pushresult.
        /// </summary>
        /// <code>
        /// void luaL_pushresultsize (luaL_Buffer *B, size_t sz);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaL_pushresultsize(luaL_Buffer* B, [NativeTypeName("size_t")] nuint sz);

        /// <summary>
        /// [-?, +?, m]
        /// Equivalent to the sequence luaL_buffinit, luaL_prepbuffsize.
        /// </summary>
        /// <code>
        /// char *luaL_buffinitsize (lua_State *L, luaL_Buffer *B, size_t sz);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("char *")]
        public static extern byte* luaL_buffinitsize(lua_State* L, luaL_Buffer* B, [NativeTypeName("size_t")] nuint sz);
    }
}
