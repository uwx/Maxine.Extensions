using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.Lua55.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua55.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua55.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.Lua55.lua_State*, Maxine.Extensions.Lua55.lua_Debug*, void>;

// ReSharper disable All

using System.Runtime.InteropServices;

namespace Maxine.Extensions.Lua55
{
    public static unsafe partial class Lua55
    {
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaL_checkversion_(lua_State* L, lua_Number ver, [NativeTypeName("size_t")] nuint sz);

        /// <summary>
        /// [-0, +(0|1), m]<br/>
        /// <br/>
        /// Pushes onto the stack the field e from the metatable of the object at index obj and returns the type of the pushed value. If the object does not have a metatable, or if the metatable does not have this field, pushes nothing and returns LUA_TNIL.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_getmetafield (lua_State *L, int obj, const char *e);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_getmetafield(lua_State* L, int obj, [NativeTypeName("const char *")] byte* e);

        /// <summary>
        /// [-0, +(0|1), e]<br/>
        /// <br/>
        /// Calls a metamethod.<br/>
        /// <br/>
        /// If the object at index obj has a metatable and this metatable has a field e, this function calls this field passing the object as its only argument. In this case this function returns true and pushes onto the stack the value returned by the call. If there is no metatable or no metamethod, this function returns false without pushing any value on the stack.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_callmeta (lua_State *L, int obj, const char *e);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_callmeta(lua_State* L, int obj, [NativeTypeName("const char *")] byte* e);

        /// <summary>
        /// [-0, +1, e]<br/>
        /// <br/>
        /// Converts any Lua value at the given index to a C string in a reasonable format. The resulting string is pushed onto the stack and also returned by the function (see §4.1.3). If len is not NULL, the function also sets *len with the string length.<br/>
        /// <br/>
        /// If the value has a metatable with a __tostring field, then luaL_tolstring calls the corresponding metamethod with the value as argument, and uses the result of the call as its result.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// const char *luaL_tolstring (lua_State *L, int idx, size_t *len);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        public static extern byte* luaL_tolstring(lua_State* L, int idx, [NativeTypeName("size_t *")] nuint* len);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Raises an error reporting a problem with argument arg of the C function that called it, using a standard message that includes extramsg as a comment:<br/>
        /// <br/>
        /// This function never returns.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_argerror (lua_State *L, int arg, const char *extramsg);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_argerror(lua_State* L, int arg, [NativeTypeName("const char *")] byte* extramsg);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Raises a type error for the argument arg of the C function that called it, using a standard message; tname is a "name" for the expected type. This function never returns.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_typeerror (lua_State *L, int arg, const char *tname);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_typeerror(lua_State* L, int arg, [NativeTypeName("const char *")] byte* tname);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Checks whether the function argument arg is a string and returns this string; if l is not NULL fills its referent with the string's length.<br/>
        /// <br/>
        /// This function uses lua_tolstring to get its result, so all conversions and caveats of that function apply here.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// const char *luaL_checklstring (lua_State *L, int arg, size_t *l);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_checklstring(lua_State* L, int arg, [NativeTypeName("size_t *")] nuint* l);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// If the function argument arg is a string, returns this string. If this argument is absent or is nil, returns d. Otherwise, raises an error.<br/>
        /// <br/>
        /// If l is not NULL, fills its referent with the result's length. If the result is NULL (only possible when returning d and d == NULL), its length is considered zero.<br/>
        /// <br/>
        /// This function uses lua_tolstring to get its result, so all conversions and caveats of that function apply here.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// const char *luaL_optlstring (lua_State *L,
        /// int arg,
        /// const char *d,
        /// size_t *l);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_optlstring(lua_State* L, int arg, [NativeTypeName("const char *")] byte* def, [NativeTypeName("size_t *")] nuint* l);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Checks whether the function argument arg is a number and returns this number converted to a lua_Number.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// lua_Number luaL_checknumber (lua_State *L, int arg);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Number luaL_checknumber(lua_State* L, int arg);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// If the function argument arg is a number, returns this number as a lua_Number. If this argument is absent or is nil, returns d. Otherwise, raises an error.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// lua_Number luaL_optnumber (lua_State *L, int arg, lua_Number d);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Number luaL_optnumber(lua_State* L, int arg, lua_Number def);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Checks whether the function argument arg is an integer (or can be converted to an integer) and returns this integer.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// lua_Integer luaL_checkinteger (lua_State *L, int arg);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Integer luaL_checkinteger(lua_State* L, int arg);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// If the function argument arg is an integer (or it is convertible to an integer), returns this integer. If this argument is absent or is nil, returns d. Otherwise, raises an error.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// lua_Integer luaL_optinteger (lua_State *L,
        /// int arg,
        /// lua_Integer d);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Integer luaL_optinteger(lua_State* L, int arg, lua_Integer def);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Grows the stack size to top + sz elements, raising an error if the stack cannot grow to that size. msg is an additional text to go into the error message (or NULL for no additional text).<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_checkstack (lua_State *L, int sz, const char *msg);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_checkstack(lua_State* L, int sz, [NativeTypeName("const char *")] byte* msg);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Checks whether the function argument arg has type t. See lua_type for the encoding of types for t.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_checktype (lua_State *L, int arg, int t);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_checktype(lua_State* L, int arg, int t);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Checks whether the function has an argument of any type (including nil) at position arg.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_checkany (lua_State *L, int arg);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_checkany(lua_State* L, int arg);

        /// <summary>
        /// [-0, +1, m]<br/>
        /// <br/>
        /// If the registry already has the key tname, returns 0. Otherwise, creates a new table to be used as a metatable for userdata, adds to this new table the pair __name = tname, adds to the registry the pair [tname] = new table, and returns 1.<br/>
        /// <br/>
        /// In both cases, the function pushes onto the stack the final value associated with tname in the registry.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_newmetatable (lua_State *L, const char *tname);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_newmetatable(lua_State* L, [NativeTypeName("const char *")] byte* tname);

        /// <summary>
        /// [-0, +0, –]<br/>
        /// <br/>
        /// Sets the metatable of the object on the top of the stack as the metatable associated with name tname in the registry (see luaL_newmetatable).<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_setmetatable (lua_State *L, const char *tname);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_setmetatable(lua_State* L, [NativeTypeName("const char *")] byte* tname);

        /// <summary>
        /// [-0, +0, m]<br/>
        /// <br/>
        /// This function works like luaL_checkudata, except that, when the test fails, it returns NULL instead of raising an error.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void *luaL_testudata (lua_State *L, int arg, const char *tname);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void* luaL_testudata(lua_State* L, int ud, [NativeTypeName("const char *")] byte* tname);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Checks whether the function argument arg is a userdata of the type tname (see luaL_newmetatable) and returns the userdata's memory-block address (see lua_touserdata).<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void *luaL_checkudata (lua_State *L, int arg, const char *tname);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void* luaL_checkudata(lua_State* L, int ud, [NativeTypeName("const char *")] byte* tname);

        /// <summary>
        /// [-0, +1, m]<br/>
        /// <br/>
        /// Pushes onto the stack a string identifying the current position of the control at level lvl in the call stack. Typically this string has the following format:<br/>
        /// <br/>
        /// Level 0 is the running function, level 1 is the function that called the running function, etc.<br/>
        /// <br/>
        /// This function is used to build a prefix for error messages.<br/>
        /// <br/>
        /// The standard Lua libraries provide useful functions that are implemented in C through the C API. Some of these functions provide essential services to the language (e.g., type and getmetatable); others provide access to outside services (e.g., I/O); and others could be implemented in Lua itself, but that for different reasons deserve an implementation in C (e.g., table.sort).<br/>
        /// <br/>
        /// All libraries are implemented through the official C API and are provided as separate C modules. Unless otherwise noted, these library functions do not adjust its number of arguments to its expected parameters. For instance, a function documented as foo(arg) should not be called without an argument.<br/>
        /// <br/>
        /// The notation fail means a false value representing some kind of failure. (Currently, fail is equal to nil, but that may change in future versions. The recommendation is to always test the success of these functions with (not status), instead of (status == nil).)<br/>
        /// <br/>
        /// Currently, Lua has the following standard libraries:<br/>
        /// <br/>
        /// - basic library (§6.2);<br/>
        /// - coroutine library (§6.3);<br/>
        /// - package library (§6.4);<br/>
        /// - string manipulation (§6.5);<br/>
        /// - basic UTF-8 support (§6.6);<br/>
        /// - table manipulation (§6.7);<br/>
        /// - mathematical functions (§6.8) (sin, log, etc.);<br/>
        /// - input and output (§6.9);<br/>
        /// - operating system facilities (§6.10);<br/>
        /// - debug facilities (§6.11).<br/>
        /// <br/>
        /// Except for the basic and the package libraries, each library provides all its functions as fields of a global table or as methods of its objects.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_where (lua_State *L, int lvl);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_where(lua_State* L, int lvl);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Raises an error. The error message format is given by fmt plus any extra arguments, following the same rules of lua_pushfstring. It also adds at the beginning of the message the file name and the line number where the error occurred, if this information is available.<br/>
        /// <br/>
        /// This function never returns, but it is an idiom to use it in C functions as return luaL_error(args).<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_error (lua_State *L, const char *fmt, ...);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_error(lua_State* L, [NativeTypeName("const char *")] byte* fmt, __arglist);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Checks whether the function argument arg is a string and searches for this string in the array lst (which must be NULL-terminated). Returns the index in the array where the string was found. Raises an error if the argument is not a string or if the string cannot be found.<br/>
        /// <br/>
        /// If def is not NULL, the function uses def as a default value when there is no argument arg or when this argument is nil.<br/>
        /// <br/>
        /// This is a useful function for mapping strings to C enums. (The usual convention in Lua libraries is to use strings instead of numbers to select options.)<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_checkoption (lua_State *L,
        /// int arg,
        /// const char *def,
        /// const char *const lst[]);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_checkoption(lua_State* L, int arg, [NativeTypeName("const char *")] byte* def, [NativeTypeName("const char *const[]")] byte** lst);

        /// <summary>
        /// [-0, +(1|3), m]<br/>
        /// <br/>
        /// This function produces the return values for file-related functions in the standard library (io.open, os.rename, file:seek, etc.).<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_fileresult (lua_State *L, int stat, const char *fname);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_fileresult(lua_State* L, int stat, [NativeTypeName("const char *")] byte* fname);

        /// <summary>
        /// [-0, +3, m]<br/>
        /// <br/>
        /// This function produces the return values for process-related functions in the standard library (os.execute and io.close).<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_execresult (lua_State *L, int stat);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_execresult(lua_State* L, int stat);

        /// <summary>
        /// A standard allocator function for Lua (see lua_Alloc), built on top of the C functions realloc and free.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void *luaL_alloc (void *ud, void *ptr, size_t osize, size_t nsize);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* luaL_alloc(void* ud, void* ptr, [NativeTypeName("size_t")] nuint osize, [NativeTypeName("size_t")] nuint nsize);

        /// <summary>
        /// [-1, +0, m]<br/>
        /// <br/>
        /// Creates and returns a reference, in the table at index t, for the object on the top of the stack (and pops the object).<br/>
        /// <br/>
        /// The reference system uses the integer keys of the table. A reference is a unique integer key; luaL_ref ensures the uniqueness of the keys it returns. The entry 1 is reserved for internal use. Before the first use of luaL_ref, the integer keys of the table should form a proper sequence (no holes), and the value at entry 1 should be false: nil if the sequence is empty, false otherwise. You should not manually set integer keys in the table after the first use of luaL_ref.<br/>
        /// <br/>
        /// You can retrieve an object referred by the reference r by calling lua_rawgeti(L,t,r) or lua_geti(L,t,r). The function luaL_unref frees a reference.<br/>
        /// <br/>
        /// If the object on the top of the stack is nil, luaL_ref returns the constant LUA_REFNIL. The constant LUA_NOREF is guaranteed to be different from any reference returned by luaL_ref.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_ref (lua_State *L, int t);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_ref(lua_State* L, int t);

        /// <summary>
        /// [-0, +0, –]<br/>
        /// <br/>
        /// Releases a reference (see luaL_ref). The integer ref must be either LUA_NOREF, LUA_REFNIL, or a reference previously returned by luaL_ref and not already released. If ref is either LUA_NOREF or LUA_REFNIL this function does nothing. Otherwise, the entry is removed from the table, so that the referred object can be collected and the reference ref can be used again by luaL_ref.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_unref (lua_State *L, int t, int ref);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_unref(lua_State* L, int t, int @ref);

        /// <summary>
        /// [-0, +1, m]<br/>
        /// <br/>
        /// Loads a file as a Lua chunk. This function uses lua_load to load the chunk in the file named filename. If filename is NULL, then it loads from the standard input. The first line in the file is ignored if it starts with a #.<br/>
        /// <br/>
        /// The string mode works as in the function lua_load.<br/>
        /// <br/>
        /// This function returns the same results as lua_load, or LUA_ERRFILE for file-related errors.<br/>
        /// <br/>
        /// As lua_load, this function only loads the chunk; it does not run it.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_loadfilex (lua_State *L, const char *filename,
        /// const char *mode);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_loadfilex(lua_State* L, [NativeTypeName("const char *")] byte* filename, [NativeTypeName("const char *")] byte* mode);

        /// <summary>
        /// [-0, +1, –]<br/>
        /// <br/>
        /// Loads a buffer as a Lua chunk. This function uses lua_load to load the chunk in the buffer pointed to by buff with size sz.<br/>
        /// <br/>
        /// This function returns the same results as lua_load. name is the chunk name, used for debug information and error messages. The string mode works as in the function lua_load. In particular, this function supports mode 'B' for fixed buffers.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_loadbufferx (lua_State *L,
        /// const char *buff,
        /// size_t sz,
        /// const char *name,
        /// const char *mode);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_loadbufferx(lua_State* L, [NativeTypeName("const char *")] byte* buff, [NativeTypeName("size_t")] nuint sz, [NativeTypeName("const char *")] byte* name, [NativeTypeName("const char *")] byte* mode);

        /// <summary>
        /// [-0, +1, –]<br/>
        /// <br/>
        /// Loads a string as a Lua chunk. This function uses lua_load to load the chunk in the zero-terminated string s.<br/>
        /// <br/>
        /// This function returns the same results as lua_load.<br/>
        /// <br/>
        /// Also as lua_load, this function only loads the chunk; it does not run it.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_loadstring (lua_State *L, const char *s);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_loadstring(lua_State* L, [NativeTypeName("const char *")] byte* s);

        /// <summary>
        /// [-0, +0, –]<br/>
        /// <br/>
        /// Creates a new Lua state. It calls lua_newstate with luaL_alloc as the allocator function and the result of luaL_makeseed(NULL) as the seed, and then sets a warning function and a panic function (see §4.4) that print messages to the standard error output.<br/>
        /// <br/>
        /// Returns the new state, or NULL if there is a memory allocation error.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// lua_State *luaL_newstate (void);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern lua_State* luaL_newstate();

        /// <summary>
        /// [-0, +0, –]<br/>
        /// <br/>
        /// Returns a value with a weak attempt for randomness. The parameter L can be NULL if there is no Lua state available.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// unsigned int luaL_makeseed (lua_State *L);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint luaL_makeseed(lua_State* L);

        /// <summary>
        /// [-0, +0, e]<br/>
        /// <br/>
        /// Returns the "length" of the value at the given index as a number; it is equivalent to the '#' operator in Lua (see §3.4.7). Raises an error if the result of the operation is not an integer. (This case can only happen through metamethods.)<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// lua_Integer luaL_len (lua_State *L, int index);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern lua_Integer luaL_len(lua_State* L, int idx);

        /// <summary>
        /// [-?, +?, m]<br/>
        /// <br/>
        /// Adds a copy of the string s to the buffer B (see luaL_Buffer), replacing any occurrence of the string p with the string r.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// const void luaL_addgsub (luaL_Buffer *B, const char *s,
        /// const char *p, const char *r);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaL_addgsub(luaL_Buffer* b, [NativeTypeName("const char *")] byte* s, [NativeTypeName("const char *")] byte* p, [NativeTypeName("const char *")] byte* r);

        /// <summary>
        /// [-0, +1, m]<br/>
        /// <br/>
        /// Creates a copy of string s, replacing any occurrence of the string p with the string r. Pushes the resulting string on the stack and returns it.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// const char *luaL_gsub (lua_State *L,
        /// const char *s,
        /// const char *p,
        /// const char *r);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_gsub(lua_State* L, [NativeTypeName("const char *")] byte* s, [NativeTypeName("const char *")] byte* p, [NativeTypeName("const char *")] byte* r);

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
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_setfuncs(lua_State* L, [NativeTypeName("const luaL_Reg *")] luaL_Reg* l, int nup);

        /// <summary>
        /// [-0, +1, e]<br/>
        /// <br/>
        /// Ensures that the value t[fname], where t is the value at index idx, is a table, and pushes that table onto the stack. Returns true if it finds a previous table there and false if it creates a new table.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_getsubtable (lua_State *L, int idx, const char *fname);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_getsubtable(lua_State* L, int idx, [NativeTypeName("const char *")] byte* fname);

        /// <summary>
        /// [-0, +1, m]<br/>
        /// <br/>
        /// Creates and pushes a traceback of the stack L1. If msg is not NULL, it is appended at the beginning of the traceback. The level parameter tells at which level to start the traceback.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_traceback (lua_State *L, lua_State *L1, const char *msg,
        /// int level);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaL_traceback(lua_State* L, lua_State* L1, [NativeTypeName("const char *")] byte* msg, int level);

        /// <summary>
        /// [-0, +1, e]<br/>
        /// <br/>
        /// If package.loaded[modname] is not true, calls the function openf with the string modname as an argument and sets the call result to package.loaded[modname], as if that function has been called through require.<br/>
        /// <br/>
        /// If glb is true, also stores the module into the global variable modname.<br/>
        /// <br/>
        /// Leaves a copy of the module on the stack.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_requiref (lua_State *L, const char *modname,
        /// lua_CFunction openf, int glb);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaL_requiref(lua_State* L, [NativeTypeName("const char *")] byte* modname, lua_CFunction openf, int glb);

        /// <summary>
        /// [-0, +?, –]<br/>
        /// <br/>
        /// Initializes a buffer B (see luaL_Buffer). This function does not allocate any space; the buffer must be declared as a variable.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_buffinit (lua_State *L, luaL_Buffer *B);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_buffinit(lua_State* L, luaL_Buffer* B);

        /// <summary>
        /// [-?, +?, m]<br/>
        /// <br/>
        /// Returns an address to a space of size sz where you can copy a string to be added to buffer B (see luaL_Buffer). After copying the string into this space you must call luaL_addsize with the size of the string to actually add it to the buffer.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// char *luaL_prepbuffsize (luaL_Buffer *B, size_t sz);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("char *")]
        public static extern byte* luaL_prepbuffsize(luaL_Buffer* B, [NativeTypeName("size_t")] nuint sz);

        /// <summary>
        /// [-?, +?, m]<br/>
        /// <br/>
        /// Adds the string pointed to by s with length l to the buffer B (see luaL_Buffer). The string can contain embedded zeros.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_addlstring (luaL_Buffer *B, const char *s, size_t l);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_addlstring(luaL_Buffer* B, [NativeTypeName("const char *")] byte* s, [NativeTypeName("size_t")] nuint l);

        /// <summary>
        /// [-?, +?, m]<br/>
        /// <br/>
        /// Adds the zero-terminated string pointed to by s to the buffer B (see luaL_Buffer).<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_addstring (luaL_Buffer *B, const char *s);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_addstring(luaL_Buffer* B, [NativeTypeName("const char *")] byte* s);

        /// <summary>
        /// [-?, +?, m]<br/>
        /// <br/>
        /// Adds the value on the top of the stack to the buffer B (see luaL_Buffer). Pops the value.<br/>
        /// <br/>
        /// This is the only function on string buffers that can (and must) be called with an extra element on the stack, which is the value to be added to the buffer.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_addvalue (luaL_Buffer *B);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_addvalue(luaL_Buffer* B);

        /// <summary>
        /// [-?, +1, m]<br/>
        /// <br/>
        /// Finishes the use of buffer B leaving the final string on the top of the stack.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_pushresult (luaL_Buffer *B);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_pushresult(luaL_Buffer* B);

        /// <summary>
        /// [-?, +1, m]<br/>
        /// <br/>
        /// Equivalent to the sequence luaL_addsize, luaL_pushresult.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_pushresultsize (luaL_Buffer *B, size_t sz);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaL_pushresultsize(luaL_Buffer* B, [NativeTypeName("size_t")] nuint sz);

        /// <summary>
        /// [-?, +?, m]<br/>
        /// <br/>
        /// Equivalent to the sequence luaL_buffinit, luaL_prepbuffsize.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// char *luaL_buffinitsize (lua_State *L, luaL_Buffer *B, size_t sz);
        /// </code>
        [DllImport("lua550", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("char *")]
        public static extern byte* luaL_buffinitsize(lua_State* L, luaL_Buffer* B, [NativeTypeName("size_t")] nuint sz);
    }
}
