using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.LuaJIT.lua_State*, int>;
using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.LuaJIT.lua_State*, void*, nuint*, byte*>;
using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.LuaJIT.lua_State*, void*, nuint, void*, int>;
using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.LuaJIT.lua_State*, Maxine.Extensions.LuaJIT.lua_Debug*, void>;

// ReSharper disable All

using System.Runtime.InteropServices;

namespace Maxine.Extensions.LuaJIT
{
    public static unsafe partial class LuaJIT
    {
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_openlib(lua_State* L, [NativeTypeName("const char *")] byte* libname, [NativeTypeName("const luaL_Reg *")] luaL_Reg* l, int nup);

        /// <summary>
        /// [-(0|1), +1, m]<br/>
        /// <br/>
        /// Opens a library.<br/>
        /// <br/>
        /// When called with libname equal to NULL, it simply registers all functions in the list l (see luaL_Reg) into the table on the top of the stack.<br/>
        /// <br/>
        /// When called with a non-null libname, luaL_register creates a new table t, sets it as the value of the global variable libname, sets it as the value of package.loaded[libname], and registers on it all functions in the list l. If there is a table in package.loaded[libname] or in variable libname, reuses this table instead of creating a new one.<br/>
        /// <br/>
        /// In any case the function leaves the table on the top of the stack.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_register (lua_State *L,
        /// const char *libname,
        /// const luaL_Reg *l);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_register(lua_State* L, [NativeTypeName("const char *")] byte* libname, [NativeTypeName("const luaL_Reg *")] luaL_Reg* l);

        /// <summary>
        /// [-0, +(0|1), m]<br/>
        /// <br/>
        /// Pushes onto the stack the field e from the metatable of the object at index obj. If the object does not have a metatable, or if the metatable does not have this field, returns 0 and pushes nothing.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_getmetafield (lua_State *L, int obj, const char *e);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_getmetafield(lua_State* L, int obj, [NativeTypeName("const char *")] byte* e);

        /// <summary>
        /// [-0, +(0|1), e]<br/>
        /// <br/>
        /// Calls a metamethod.<br/>
        /// <br/>
        /// If the object at index obj has a metatable and this metatable has a field e, this function calls this field and passes the object as its only argument. In this case this function returns 1 and pushes onto the stack the value returned by the call. If there is no metatable or no metamethod, this function returns 0 (without pushing any value on the stack).<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_callmeta (lua_State *L, int obj, const char *e);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_callmeta(lua_State* L, int obj, [NativeTypeName("const char *")] byte* e);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Generates an error with a message like the following:<br/>
        /// <br/>
        /// where location is produced by luaL_where, func is the name of the current function, and rt is the type name of the actual argument.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_typerror (lua_State *L, int narg, const char *tname);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_typerror(lua_State* L, int narg, [NativeTypeName("const char *")] byte* tname);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Raises an error with the following message, where func is retrieved from the call stack:<br/>
        /// <br/>
        /// This function never returns, but it is an idiom to use it in C functions as return luaL_argerror(args).<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_argerror (lua_State *L, int narg, const char *extramsg);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_argerror(lua_State* L, int numarg, [NativeTypeName("const char *")] byte* extramsg);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Checks whether the function argument narg is a string and returns this string; if l is not NULL fills *l with the string's length.<br/>
        /// <br/>
        /// This function uses lua_tolstring to get its result, so all conversions and caveats of that function apply here.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// const char *luaL_checklstring (lua_State *L, int narg, size_t *l);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_checklstring(lua_State* L, int numArg, [NativeTypeName("size_t *")] nuint* l);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// If the function argument narg is a string, returns this string. If this argument is absent or is nil, returns d. Otherwise, raises an error.<br/>
        /// <br/>
        /// If l is not NULL, fills the position *l with the results's length.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// const char *luaL_optlstring (lua_State *L,
        /// int narg,
        /// const char *d,
        /// size_t *l);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_optlstring(lua_State* L, int numArg, [NativeTypeName("const char *")] byte* def, [NativeTypeName("size_t *")] nuint* l);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Checks whether the function argument narg is a number and returns this number.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// lua_Number luaL_checknumber (lua_State *L, int narg);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Number luaL_checknumber(lua_State* L, int numArg);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// If the function argument narg is a number, returns this number. If this argument is absent or is nil, returns d. Otherwise, raises an error.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// lua_Number luaL_optnumber (lua_State *L, int narg, lua_Number d);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Number luaL_optnumber(lua_State* L, int nArg, lua_Number def);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Checks whether the function argument narg is a number and returns this number cast to a lua_Integer.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// lua_Integer luaL_checkinteger (lua_State *L, int narg);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Integer luaL_checkinteger(lua_State* L, int numArg);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// If the function argument narg is a number, returns this number cast to a lua_Integer. If this argument is absent or is nil, returns d. Otherwise, raises an error.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// lua_Integer luaL_optinteger (lua_State *L,
        /// int narg,
        /// lua_Integer d);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Integer luaL_optinteger(lua_State* L, int nArg, lua_Integer def);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Grows the stack size to top + sz elements, raising an error if the stack cannot grow to that size. msg is an additional text to go into the error message.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_checkstack (lua_State *L, int sz, const char *msg);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_checkstack(lua_State* L, int sz, [NativeTypeName("const char *")] byte* msg);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Checks whether the function argument narg has type t. See lua_type for the encoding of types for t.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_checktype (lua_State *L, int narg, int t);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_checktype(lua_State* L, int narg, int t);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Checks whether the function has an argument of any type (including nil) at position narg.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_checkany (lua_State *L, int narg);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_checkany(lua_State* L, int narg);

        /// <summary>
        /// [-0, +1, m]<br/>
        /// <br/>
        /// If the registry already has the key tname, returns 0. Otherwise, creates a new table to be used as a metatable for userdata, adds it to the registry with key tname, and returns 1.<br/>
        /// <br/>
        /// In both cases pushes onto the stack the final value associated with tname in the registry.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_newmetatable (lua_State *L, const char *tname);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_newmetatable(lua_State* L, [NativeTypeName("const char *")] byte* tname);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Checks whether the function argument narg is a userdata of the type tname (see luaL_newmetatable).<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void *luaL_checkudata (lua_State *L, int narg, const char *tname);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
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
        /// The standard Lua libraries provide useful functions that are implemented directly through the C API. Some of these functions provide essential services to the language (e.g., type and getmetatable); others provide access to "outside" services (e.g., I/O); and others could be implemented in Lua itself, but are quite useful or have critical performance requirements that deserve an implementation in C (e.g., table.sort).<br/>
        /// <br/>
        /// All libraries are implemented through the official C API and are provided as separate C modules. Currently, Lua has the following standard libraries:<br/>
        /// <br/>
        /// - basic library, which includes the coroutine sub-library;<br/>
        /// - package library;<br/>
        /// - string manipulation;<br/>
        /// - table manipulation;<br/>
        /// - mathematical functions (sin, log, etc.);<br/>
        /// - input and output;<br/>
        /// - operating system facilities;<br/>
        /// - debug facilities.<br/>
        /// <br/>
        /// Except for the basic and package libraries, each library provides all its functions as fields of a global table or as methods of its objects.<br/>
        /// <br/>
        /// To have access to these libraries, the C host program should call the luaL_openlibs function, which opens all standard libraries. Alternatively, it can open them individually by calling luaopen_base (for the basic library), luaopen_package (for the package library), luaopen_string (for the string library), luaopen_table (for the table library), luaopen_math (for the mathematical library), luaopen_io (for the I/O library), luaopen_os (for the Operating System library), and luaopen_debug (for the debug library). These functions are declared in lualib.h and should not be called directly: you must call them like any other Lua C function, e.g., by using lua_call.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_where (lua_State *L, int lvl);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
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
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_error(lua_State* L, [NativeTypeName("const char *")] byte* fmt, __arglist);

        /// <summary>
        /// [-0, +0, v]<br/>
        /// <br/>
        /// Checks whether the function argument narg is a string and searches for this string in the array lst (which must be NULL-terminated). Returns the index in the array where the string was found. Raises an error if the argument is not a string or if the string cannot be found.<br/>
        /// <br/>
        /// If def is not NULL, the function uses def as a default value when there is no argument narg or if this argument is nil.<br/>
        /// <br/>
        /// This is a useful function for mapping strings to C enums. (The usual convention in Lua libraries is to use strings instead of numbers to select options.)<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_checkoption (lua_State *L,
        /// int narg,
        /// const char *def,
        /// const char *const lst[]);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_checkoption(lua_State* L, int narg, [NativeTypeName("const char *")] byte* def, [NativeTypeName("const char *const[]")] byte** lst);

        /// <summary>
        /// [-1, +0, m]<br/>
        /// <br/>
        /// Creates and returns a reference, in the table at index t, for the object at the top of the stack (and pops the object).<br/>
        /// <br/>
        /// A reference is a unique integer key. As long as you do not manually add integer keys into table t, luaL_ref ensures the uniqueness of the key it returns. You can retrieve an object referred by reference r by calling lua_rawgeti(L, t, r). Function luaL_unref frees a reference and its associated object.<br/>
        /// <br/>
        /// If the object at the top of the stack is nil, luaL_ref returns the constant LUA_REFNIL. The constant LUA_NOREF is guaranteed to be different from any reference returned by luaL_ref.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_ref (lua_State *L, int t);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_ref(lua_State* L, int t);

        /// <summary>
        /// [-0, +0, -]<br/>
        /// <br/>
        /// Releases reference ref from the table at index t (see luaL_ref). The entry is removed from the table, so that the referred object can be collected. The reference ref is also freed to be used again.<br/>
        /// <br/>
        /// If ref is LUA_NOREF or LUA_REFNIL, luaL_unref does nothing.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_unref (lua_State *L, int t, int ref);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_unref(lua_State* L, int t, int @ref);

        /// <summary>
        /// [-0, +1, m]<br/>
        /// <br/>
        /// Loads a file as a Lua chunk. This function uses lua_load to load the chunk in the file named filename. If filename is NULL, then it loads from the standard input. The first line in the file is ignored if it starts with a #.<br/>
        /// <br/>
        /// This function returns the same results as lua_load, but it has an extra error code LUA_ERRFILE if it cannot open/read the file.<br/>
        /// <br/>
        /// As lua_load, this function only loads the chunk; it does not run it.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_loadfile (lua_State *L, const char *filename);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_loadfile(lua_State* L, [NativeTypeName("const char *")] byte* filename);

        /// <summary>
        /// [-0, +1, m]<br/>
        /// <br/>
        /// Loads a buffer as a Lua chunk. This function uses lua_load to load the chunk in the buffer pointed to by buff with size sz.<br/>
        /// <br/>
        /// This function returns the same results as lua_load. name is the chunk name, used for debug information and error messages.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// int luaL_loadbuffer (lua_State *L,
        /// const char *buff,
        /// size_t sz,
        /// const char *name);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_loadbuffer(lua_State* L, [NativeTypeName("const char *")] byte* buff, [NativeTypeName("size_t")] nuint sz, [NativeTypeName("const char *")] byte* name);

        /// <summary>
        /// [-0, +1, m]<br/>
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
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_loadstring(lua_State* L, [NativeTypeName("const char *")] byte* s);

        /// <summary>
        /// [-0, +0, -]<br/>
        /// <br/>
        /// Creates a new Lua state. It calls lua_newstate with an allocator based on the standard C realloc function and then sets a panic function (see lua_atpanic) that prints an error message to the standard error output in case of fatal errors.<br/>
        /// <br/>
        /// Returns the new state, or NULL if there is a memory allocation error.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// lua_State *luaL_newstate (void);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern lua_State* luaL_newstate();

        /// <summary>
        /// [-0, +1, m]<br/>
        /// <br/>
        /// Creates a copy of string s by replacing any occurrence of the string p with the string r. Pushes the resulting string on the stack and returns it.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// const char *luaL_gsub (lua_State *L,
        /// const char *s,
        /// const char *p,
        /// const char *r);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_gsub(lua_State* L, [NativeTypeName("const char *")] byte* s, [NativeTypeName("const char *")] byte* p, [NativeTypeName("const char *")] byte* r);

        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_findtable(lua_State* L, int idx, [NativeTypeName("const char *")] byte* fname, int szhint);

        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_fileresult(lua_State* L, int stat, [NativeTypeName("const char *")] byte* fname);

        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int luaL_execresult(lua_State* L, int stat);

        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_loadfilex(lua_State* L, [NativeTypeName("const char *")] byte* filename, [NativeTypeName("const char *")] byte* mode);

        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int luaL_loadbufferx(lua_State* L, [NativeTypeName("const char *")] byte* buff, [NativeTypeName("size_t")] nuint sz, [NativeTypeName("const char *")] byte* name, [NativeTypeName("const char *")] byte* mode);

        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void luaL_traceback(lua_State* L, lua_State* L1, [NativeTypeName("const char *")] byte* msg, int level);

        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_setfuncs(lua_State* L, [NativeTypeName("const luaL_Reg *")] luaL_Reg* l, int nup);

        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_pushmodule(lua_State* L, [NativeTypeName("const char *")] byte* modname, int sizehint);

        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void* luaL_testudata(lua_State* L, int ud, [NativeTypeName("const char *")] byte* tname);

        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_setmetatable(lua_State* L, [NativeTypeName("const char *")] byte* tname);

        /// <summary>
        /// [-0, +0, -]<br/>
        /// <br/>
        /// Initializes a buffer B. This function does not allocate any space; the buffer must be declared as a variable (see luaL_Buffer).<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_buffinit (lua_State *L, luaL_Buffer *B);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_buffinit(lua_State* L, luaL_Buffer* B);

        /// <summary>
        /// [-0, +0, -]<br/>
        /// <br/>
        /// Returns an address to a space of size LUAL_BUFFERSIZE where you can copy a string to be added to buffer B (see luaL_Buffer). After copying the string into this space you must call luaL_addsize with the size of the string to actually add it to the buffer.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// char *luaL_prepbuffer (luaL_Buffer *B);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("char *")]
        [SuppressGCTransition]
        public static extern byte* luaL_prepbuffer(luaL_Buffer* B);

        /// <summary>
        /// [-0, +0, m]<br/>
        /// <br/>
        /// Adds the string pointed to by s with length l to the buffer B (see luaL_Buffer). The string may contain embedded zeros.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_addlstring (luaL_Buffer *B, const char *s, size_t l);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_addlstring(luaL_Buffer* B, [NativeTypeName("const char *")] byte* s, [NativeTypeName("size_t")] nuint l);

        /// <summary>
        /// [-0, +0, m]<br/>
        /// <br/>
        /// Adds the zero-terminated string pointed to by s to the buffer B (see luaL_Buffer). The string may not contain embedded zeros.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_addstring (luaL_Buffer *B, const char *s);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_addstring(luaL_Buffer* B, [NativeTypeName("const char *")] byte* s);

        /// <summary>
        /// [-1, +0, m]<br/>
        /// <br/>
        /// Adds the value at the top of the stack to the buffer B (see luaL_Buffer). Pops the value.<br/>
        /// <br/>
        /// This is the only function on string buffers that can (and must) be called with an extra element on the stack, which is the value to be added to the buffer.<br/>
        /// <br/>
        /// </summary>
        /// <code>
        /// void luaL_addvalue (luaL_Buffer *B);
        /// </code>
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
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
        [DllImport("lua51", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void luaL_pushresult(luaL_Buffer* B);
    }
}
