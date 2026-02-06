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
        /// <summary>
        /// [-0, +0, -]
        /// Creates a new, independent state. Returns NULL if cannot create the state (due to lack of memory). The argument f is the allocator function; Lua does all memory allocation for this state through this function. The second argument, ud, is an opaque pointer that Lua simply passes to the allocator in every call.
        /// </summary>
        /// <code>
        /// lua_State *lua_newstate (lua_Alloc f, void *ud);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern lua_State* lua_newstate(lua_Alloc f, void* ud);

        /// <summary>
        /// [-0, +0, -]
        /// Destroys all objects in the given Lua state (calling the corresponding garbage-collection metamethods, if any) and frees all dynamic memory used by this state. On several platforms, you may not need to call this function, because all resources are naturally released when the host program ends. On the other hand, long-running programs, such as a daemon or a web server, might need to release states as soon as they are not needed, to avoid growing too large.
        /// </summary>
        /// <code>
        /// void lua_close (lua_State *L);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_close(lua_State* L);

        /// <summary>
        /// [-0, +1, m]
        /// Creates a new thread, pushes it on the stack, and returns a pointer to a lua_State that represents this new thread. The new state returned by this function shares with the original state all global objects (such as tables), but has an independent execution stack.
        /// There is no explicit function to close or to destroy a thread. Threads are subject to garbage collection, like any Lua object.
        /// </summary>
        /// <code>
        /// lua_State *lua_newthread (lua_State *L);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_State* lua_newthread(lua_State* L);

        /// <summary>
        /// [-0, +0, -]
        /// Sets a new panic function and returns the old one.
        /// If an error happens outside any protected environment, Lua calls a panic function and then calls exit(EXIT_FAILURE), thus exiting the host application. Your panic function can avoid this exit by never returning (e.g., doing a long jump).
        /// The panic function can access the error message at the top of the stack.
        /// </summary>
        /// <code>
        /// lua_CFunction lua_atpanic (lua_State *L, lua_CFunction panicf);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_CFunction lua_atpanic(lua_State* L, lua_CFunction panicf);

        /// <summary>
        /// [-0, +0, -]
        /// Returns the index of the top element in the stack. Because indices start at 1, this result is equal to the number of elements in the stack (and so 0 means an empty stack).
        /// </summary>
        /// <code>
        /// int lua_gettop (lua_State *L);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_gettop(lua_State* L);

        /// <summary>
        /// [-?, +?, -]
        /// Accepts any acceptable index, or 0, and sets the stack top to this index. If the new top is larger than the old one, then the new elements are filled with nil. If index is 0, then all stack elements are removed.
        /// </summary>
        /// <code>
        /// void lua_settop (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_settop(lua_State* L, int idx);

        /// <summary>
        /// [-0, +1, -]
        /// Pushes a copy of the element at the given valid index onto the stack.
        /// </summary>
        /// <code>
        /// void lua_pushvalue (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushvalue(lua_State* L, int idx);

        /// <summary>
        /// [-1, +0, -]
        /// Removes the element at the given valid index, shifting down the elements above this index to fill the gap. Cannot be called with a pseudo-index, because a pseudo-index is not an actual stack position.
        /// </summary>
        /// <code>
        /// void lua_remove (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_remove(lua_State* L, int idx);

        /// <summary>
        /// [-1, +1, -]
        /// Moves the top element into the given valid index, shifting up the elements above this index to open space. Cannot be called with a pseudo-index, because a pseudo-index is not an actual stack position.
        /// </summary>
        /// <code>
        /// void lua_insert (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_insert(lua_State* L, int idx);

        /// <summary>
        /// [-1, +0, -]
        /// Moves the top element into the given position (and pops it), without shifting any element (therefore replacing the value at the given position).
        /// </summary>
        /// <code>
        /// void lua_replace (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_replace(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, m]
        /// Ensures that there are at least extra free stack slots in the stack. It returns false if it cannot grow the stack to that size. This function never shrinks the stack; if the stack is already larger than the new size, it is left unchanged.
        /// </summary>
        /// <code>
        /// int lua_checkstack (lua_State *L, int extra);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_checkstack(lua_State* L, int sz);

        /// <summary>
        /// [-?, +?, -]
        /// Exchange values between different threads of the same global state.
        /// This function pops n values from the stack from, and pushes them onto the stack to.
        /// </summary>
        /// <code>
        /// void lua_xmove (lua_State *from, lua_State *to, int n);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_xmove(lua_State* from, lua_State* to, int n);

        /// <summary>
        /// [-0, +0, -]
        /// Returns 1 if the value at the given acceptable index is a number or a string convertible to a number, and 0 otherwise.
        /// </summary>
        /// <code>
        /// int lua_isnumber (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_isnumber(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, -]
        /// Returns 1 if the value at the given acceptable index is a string or a number (which is always convertible to a string), and 0 otherwise.
        /// </summary>
        /// <code>
        /// int lua_isstring (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_isstring(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, -]
        /// Returns 1 if the value at the given acceptable index is a C function, and 0 otherwise.
        /// </summary>
        /// <code>
        /// int lua_iscfunction (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_iscfunction(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, -]
        /// Returns 1 if the value at the given acceptable index is a userdata (either full or light), and 0 otherwise.
        /// </summary>
        /// <code>
        /// int lua_isuserdata (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_isuserdata(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, -]
        /// Returns the type of the value in the given acceptable index, or LUA_TNONE for a non-valid index (that is, an index to an "empty" stack position). The types returned by lua_type are coded by the following constants defined in lua.h: LUA_TNIL, LUA_TNUMBER, LUA_TBOOLEAN, LUA_TSTRING, LUA_TTABLE, LUA_TFUNCTION, LUA_TUSERDATA, LUA_TTHREAD, and LUA_TLIGHTUSERDATA.
        /// </summary>
        /// <code>
        /// int lua_type (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_type(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, -]
        /// Returns the name of the type encoded by the value tp, which must be one the values returned by lua_type.
        /// </summary>
        /// <code>
        /// const char *lua_typename  (lua_State *L, int tp);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_typename", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_typename(lua_State* L, int tp);

        /// <summary>
        /// [-0, +0, e]
        /// Returns 1 if the two values in acceptable indices index1 and index2 are equal, following the semantics of the Lua == operator (that is, may call metamethods). Otherwise returns 0. Also returns 0 if any of the indices is non valid.
        /// </summary>
        /// <code>
        /// int lua_equal (lua_State *L, int index1, int index2);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_equal(lua_State* L, int idx1, int idx2);

        /// <summary>
        /// [-0, +0, -]
        /// Returns 1 if the two values in acceptable indices index1 and index2 are primitively equal (that is, without calling metamethods). Otherwise returns 0. Also returns 0 if any of the indices are non valid.
        /// </summary>
        /// <code>
        /// int lua_rawequal (lua_State *L, int index1, int index2);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_rawequal(lua_State* L, int idx1, int idx2);

        /// <summary>
        /// [-0, +0, e]
        /// Returns 1 if the value at acceptable index index1 is smaller than the value at acceptable index index2, following the semantics of the Lua < operator (that is, may call metamethods). Otherwise returns 0. Also returns 0 if any of the indices is non valid.
        /// </summary>
        /// <code>
        /// int lua_lessthan (lua_State *L, int index1, int index2);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_lessthan(lua_State* L, int idx1, int idx2);

        /// <summary>
        /// [-0, +0, -]
        /// Converts the Lua value at the given acceptable index to the C type lua_Number (see lua_Number). The Lua value must be a number or a string convertible to a number (see §2.2.1); otherwise, lua_tonumber returns 0.
        /// </summary>
        /// <code>
        /// lua_Number lua_tonumber (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Number lua_tonumber(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, -]
        /// Converts the Lua value at the given acceptable index to the signed integral type lua_Integer. The Lua value must be a number or a string convertible to a number (see §2.2.1); otherwise, lua_tointeger returns 0.
        /// If the number is not an integer, it is truncated in some non-specified way.
        /// </summary>
        /// <code>
        /// lua_Integer lua_tointeger (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Integer lua_tointeger(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, -]
        /// Converts the Lua value at the given acceptable index to a C boolean value (0 or 1). Like all tests in Lua, lua_toboolean returns 1 for any Lua value different from false and nil; otherwise it returns 0. It also returns 0 when called with a non-valid index. (If you want to accept only actual boolean values, use lua_isboolean to test the value's type.)
        /// </summary>
        /// <code>
        /// int lua_toboolean (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_toboolean(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, m]
        /// Converts the Lua value at the given acceptable index to a C string. If len is not NULL, it also sets *len with the string length. The Lua value must be a string or a number; otherwise, the function returns NULL. If the value is a number, then lua_tolstring also changes the actual value in the stack to a string. (This change confuses lua_next when lua_tolstring is applied to keys during a table traversal.)
        /// lua_tolstring returns a fully aligned pointer to a string inside the Lua state. This string always has a zero ('\0') after its last character (as in C), but can contain other zeros in its body. Because Lua has garbage collection, there is no guarantee that the pointer returned by lua_tolstring will be valid after the corresponding value is removed from the stack.
        /// </summary>
        /// <code>
        /// const char *lua_tolstring (lua_State *L, int index, size_t *len);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        public static extern byte* lua_tolstring(lua_State* L, int idx, [NativeTypeName("size_t *")] nuint* len);

        /// <summary>
        /// [-0, +0, -]
        /// Returns the "length" of the value at the given acceptable index: for strings, this is the string length; for tables, this is the result of the length operator ('#'); for userdata, this is the size of the block of memory allocated for the userdata; for other values, it is 0.
        /// </summary>
        /// <code>
        /// size_t lua_objlen (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        [SuppressGCTransition]
        public static extern nuint lua_objlen(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, -]
        /// Converts a value at the given acceptable index to a C function. That value must be a C function; otherwise, returns NULL.
        /// </summary>
        /// <code>
        /// lua_CFunction lua_tocfunction (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_CFunction lua_tocfunction(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, -]
        /// If the value at the given acceptable index is a full userdata, returns its block address. If the value is a light userdata, returns its pointer. Otherwise, returns NULL.
        /// </summary>
        /// <code>
        /// void *lua_touserdata (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void* lua_touserdata(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, -]
        /// Converts the value at the given acceptable index to a Lua thread (represented as lua_State*). This value must be a thread; otherwise, the function returns NULL.
        /// </summary>
        /// <code>
        /// lua_State *lua_tothread (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_State* lua_tothread(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, -]
        /// Converts the value at the given acceptable index to a generic C pointer (void*). The value can be a userdata, a table, a thread, or a function; otherwise, lua_topointer returns NULL. Different objects will give different pointers. There is no way to convert the pointer back to its original value.
        /// Typically this function is used only for debug information.
        /// </summary>
        /// <code>
        /// const void *lua_topointer (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const void *")]
        [SuppressGCTransition]
        public static extern void* lua_topointer(lua_State* L, int idx);

        /// <summary>
        /// [-0, +1, -]
        /// Pushes a nil value onto the stack.
        /// </summary>
        /// <code>
        /// void lua_pushnil (lua_State *L);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushnil(lua_State* L);

        /// <summary>
        /// [-0, +1, -]
        /// Pushes a number with value n onto the stack.
        /// </summary>
        /// <code>
        /// void lua_pushnumber (lua_State *L, lua_Number n);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushnumber(lua_State* L, lua_Number n);

        /// <summary>
        /// [-0, +1, -]
        /// Pushes a number with value n onto the stack.
        /// </summary>
        /// <code>
        /// void lua_pushinteger (lua_State *L, lua_Integer n);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushinteger(lua_State* L, lua_Integer n);

        /// <summary>
        /// [-0, +1, m]
        /// Pushes the string pointed to by s with size len onto the stack. Lua makes (or reuses) an internal copy of the given string, so the memory at s can be freed or reused immediately after the function returns. The string can contain embedded zeros.
        /// </summary>
        /// <code>
        /// void lua_pushlstring (lua_State *L, const char *s, size_t len);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushlstring(lua_State* L, [NativeTypeName("const char *")] byte* s, [NativeTypeName("size_t")] nuint l);

        /// <summary>
        /// [-0, +1, m]
        /// Pushes the zero-terminated string pointed to by s onto the stack. Lua makes (or reuses) an internal copy of the given string, so the memory at s can be freed or reused immediately after the function returns. The string cannot contain embedded zeros; it is assumed to end at the first zero.
        /// </summary>
        /// <code>
        /// void lua_pushstring (lua_State *L, const char *s);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushstring(lua_State* L, [NativeTypeName("const char *")] byte* s);

        /// <summary>
        /// [-0, +1, m]
        /// Equivalent to lua_pushfstring, except that it receives a va_list instead of a variable number of arguments.
        /// </summary>
        /// <code>
        /// const char *lua_pushvfstring (lua_State *L,
        /// const char *fmt,
        /// va_list argp);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* lua_pushvfstring(lua_State* L, [NativeTypeName("const char *")] byte* fmt, [NativeTypeName("va_list")] byte* argp);

        /// <summary>
        /// [-n, +1, m]
        /// Pushes a new C closure onto the stack.
        /// When a C function is created, it is possible to associate some values with it, thus creating a C closure (see §3.4); these values are then accessible to the function whenever it is called. To associate values with a C function, first these values should be pushed onto the stack (when there are multiple values, the first value is pushed first). Then lua_pushcclosure is called to create and push the C function onto the stack, with the argument n telling how many values should be associated with the function. lua_pushcclosure also pops these values from the stack.
        /// The maximum value for n is 255.
        /// </summary>
        /// <code>
        /// void lua_pushcclosure (lua_State *L, lua_CFunction fn, int n);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushcclosure(lua_State* L, lua_CFunction fn, int n);

        /// <summary>
        /// [-0, +1, -]
        /// Pushes a boolean value with value b onto the stack.
        /// </summary>
        /// <code>
        /// void lua_pushboolean (lua_State *L, int b);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushboolean(lua_State* L, int b);

        /// <summary>
        /// [-0, +1, -]
        /// Pushes a light userdata onto the stack.
        /// Userdata represent C values in Lua. A light userdata represents a pointer. It is a value (like a number): you do not create it, it has no individual metatable, and it is not collected (as it was never created). A light userdata is equal to "any" light userdata with the same C address.
        /// </summary>
        /// <code>
        /// void lua_pushlightuserdata (lua_State *L, void *p);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushlightuserdata(lua_State* L, void* p);

        /// <summary>
        /// [-0, +1, -]
        /// Pushes the thread represented by L onto the stack. Returns 1 if this thread is the main thread of its state.
        /// </summary>
        /// <code>
        /// int lua_pushthread (lua_State *L);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_pushthread(lua_State* L);

        /// <summary>
        /// [-1, +1, e]
        /// Pushes onto the stack the value t[k], where t is the value at the given valid index and k is the value at the top of the stack.
        /// This function pops the key from the stack (putting the resulting value in its place). As in Lua, this function may trigger a metamethod for the "index" event (see §2.8).
        /// </summary>
        /// <code>
        /// void lua_gettable (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_gettable(lua_State* L, int idx);

        /// <summary>
        /// [-0, +1, e]
        /// Pushes onto the stack the value t[k], where t is the value at the given valid index. As in Lua, this function may trigger a metamethod for the "index" event (see §2.8).
        /// </summary>
        /// <code>
        /// void lua_getfield (lua_State *L, int index, const char *k);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_getfield(lua_State* L, int idx, [NativeTypeName("const char *")] byte* k);

        /// <summary>
        /// [-1, +1, -]
        /// Similar to lua_gettable, but does a raw access (i.e., without metamethods).
        /// </summary>
        /// <code>
        /// void lua_rawget (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_rawget(lua_State* L, int idx);

        /// <summary>
        /// [-0, +1, -]
        /// Pushes onto the stack the value t[n], where t is the value at the given valid index. The access is raw; that is, it does not invoke metamethods.
        /// </summary>
        /// <code>
        /// void lua_rawgeti (lua_State *L, int index, int n);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_rawgeti(lua_State* L, int idx, int n);

        /// <summary>
        /// [-0, +1, m]
        /// Creates a new empty table and pushes it onto the stack. The new table has space pre-allocated for narr array elements and nrec non-array elements. This pre-allocation is useful when you know exactly how many elements the table will have. Otherwise you can use the function lua_newtable.
        /// </summary>
        /// <code>
        /// void lua_createtable (lua_State *L, int narr, int nrec);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_createtable(lua_State* L, int narr, int nrec);

        /// <summary>
        /// [-0, +1, m]
        /// This function allocates a new block of memory with the given size, pushes onto the stack a new full userdata with the block address, and returns this address.
        /// Userdata represent C values in Lua. A full userdata represents a block of memory. It is an object (like a table): you must create it, it can have its own metatable, and you can detect when it is being collected. A full userdata is only equal to itself (under raw equality).
        /// When Lua collects a full userdata with a gc metamethod, Lua calls the metamethod and marks the userdata as finalized. When this userdata is collected again then Lua frees its corresponding memory.
        /// </summary>
        /// <code>
        /// void *lua_newuserdata (lua_State *L, size_t size);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void* lua_newuserdata(lua_State* L, [NativeTypeName("size_t")] nuint sz);

        /// <summary>
        /// [-0, +(0|1), -]
        /// Pushes onto the stack the metatable of the value at the given acceptable index. If the index is not valid, or if the value does not have a metatable, the function returns 0 and pushes nothing on the stack.
        /// </summary>
        /// <code>
        /// int lua_getmetatable (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_getmetatable(lua_State* L, int objindex);

        /// <summary>
        /// [-0, +1, -]
        /// Pushes onto the stack the environment table of the value at the given index.
        /// </summary>
        /// <code>
        /// void lua_getfenv (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_getfenv(lua_State* L, int idx);

        /// <summary>
        /// [-2, +0, e]
        /// Does the equivalent to t[k] = v, where t is the value at the given valid index, v is the value at the top of the stack, and k is the value just below the top.
        /// This function pops both the key and the value from the stack. As in Lua, this function may trigger a metamethod for the "newindex" event (see §2.8).
        /// </summary>
        /// <code>
        /// void lua_settable (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_settable(lua_State* L, int idx);

        /// <summary>
        /// [-1, +0, e]
        /// Does the equivalent to t[k] = v, where t is the value at the given valid index and v is the value at the top of the stack.
        /// This function pops the value from the stack. As in Lua, this function may trigger a metamethod for the "newindex" event (see §2.8).
        /// </summary>
        /// <code>
        /// void lua_setfield (lua_State *L, int index, const char *k);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_setfield(lua_State* L, int idx, [NativeTypeName("const char *")] byte* k);

        /// <summary>
        /// [-2, +0, m]
        /// Similar to lua_settable, but does a raw assignment (i.e., without metamethods).
        /// </summary>
        /// <code>
        /// void lua_rawset (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_rawset(lua_State* L, int idx);

        /// <summary>
        /// [-1, +0, m]
        /// Does the equivalent of t[n] = v, where t is the value at the given valid index and v is the value at the top of the stack.
        /// This function pops the value from the stack. The assignment is raw; that is, it does not invoke metamethods.
        /// </summary>
        /// <code>
        /// void lua_rawseti (lua_State *L, int index, int n);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_rawseti(lua_State* L, int idx, int n);

        /// <summary>
        /// [-1, +0, -]
        /// Pops a table from the stack and sets it as the new metatable for the value at the given acceptable index.
        /// </summary>
        /// <code>
        /// int lua_setmetatable (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_setmetatable(lua_State* L, int objindex);

        /// <summary>
        /// [-1, +0, -]
        /// Pops a table from the stack and sets it as the new environment for the value at the given index. If the value at the given index is neither a function nor a thread nor a userdata, lua_setfenv returns 0. Otherwise it returns 1.
        /// </summary>
        /// <code>
        /// int lua_setfenv (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_setfenv(lua_State* L, int idx);

        /// <summary>
        /// [-(nargs + 1), +nresults, e]
        /// Calls a function.
        /// To call a function you must use the following protocol: first, the function to be called is pushed onto the stack; then, the arguments to the function are pushed in direct order; that is, the first argument is pushed first. Finally you call lua_call; nargs is the number of arguments that you pushed onto the stack. All arguments and the function value are popped from the stack when the function is called. The function results are pushed onto the stack when the function returns. The number of results is adjusted to nresults, unless nresults is LUA_MULTRET. In this case, all results from the function are pushed. Lua takes care that the returned values fit into the stack space. The function results are pushed onto the stack in direct order (the first result is pushed first), so that after the call the last result is on the top of the stack.
        /// Any error inside the called function is propagated upwards (with a longjmp).
        /// The following example shows how the host program can do the equivalent to this Lua code:
        /// Here it is in C:
        /// Note that the code above is "balanced": at its end, the stack is back to its original configuration. This is considered good programming practice.
        /// </summary>
        /// <code>
        /// void lua_call (lua_State *L, int nargs, int nresults);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_call(lua_State* L, int nargs, int nresults);

        /// <summary>
        /// [-(nargs + 1), +(nresults|1), -]
        /// Calls a function in protected mode.
        /// Both nargs and nresults have the same meaning as in lua_call. If there are no errors during the call, lua_pcall behaves exactly like lua_call. However, if there is any error, lua_pcall catches it, pushes a single value on the stack (the error message), and returns an error code. Like lua_call, lua_pcall always removes the function and its arguments from the stack.
        /// If errfunc is 0, then the error message returned on the stack is exactly the original error message. Otherwise, errfunc is the stack index of an error handler function. (In the current implementation, this index cannot be a pseudo-index.) In case of runtime errors, this function will be called with the error message and its return value will be the message returned on the stack by lua_pcall.
        /// Typically, the error handler function is used to add more debug information to the error message, such as a stack traceback. Such information cannot be gathered after the return of lua_pcall, since by then the stack has unwound.
        /// The lua_pcall function returns 0 in case of success or one of the following error codes (defined in lua.h):
        /// - LUA_ERRRUN: a runtime error.
        /// - LUA_ERRMEM: memory allocation error. For such errors, Lua does not call the error handler function.
        /// - LUA_ERRERR: error while running the error handler function.
        /// </summary>
        /// <code>
        /// int lua_pcall (lua_State *L, int nargs, int nresults, int errfunc);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_pcall(lua_State* L, int nargs, int nresults, int errfunc);

        /// <summary>
        /// [-0, +(0|1), -]
        /// Calls the C function func in protected mode. func starts with only one element in its stack, a light userdata containing ud. In case of errors, lua_cpcall returns the same error codes as lua_pcall, plus the error object on the top of the stack; otherwise, it returns zero, and does not change the stack. All values returned by func are discarded.
        /// </summary>
        /// <code>
        /// int lua_cpcall (lua_State *L, lua_CFunction func, void *ud);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_cpcall(lua_State* L, lua_CFunction func, void* ud);

        /// <summary>
        /// [-0, +1, -]
        /// Loads a Lua chunk. If there are no errors, lua_load pushes the compiled chunk as a Lua function on top of the stack. Otherwise, it pushes an error message. The return values of lua_load are:
        /// - 0: no errors;
        /// - LUA_ERRSYNTAX: syntax error during pre-compilation;
        /// - LUA_ERRMEM: memory allocation error.
        /// This function only loads a chunk; it does not run it.
        /// lua_load automatically detects whether the chunk is text or binary, and loads it accordingly (see program luac).
        /// The lua_load function uses a user-supplied reader function to read the chunk (see lua_Reader). The data argument is an opaque value passed to the reader function.
        /// The chunkname argument gives a name to the chunk, which is used for error messages and in debug information (see §3.8).
        /// </summary>
        /// <code>
        /// int lua_load (lua_State *L,
        /// lua_Reader reader,
        /// void *data,
        /// const char *chunkname);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_load(lua_State* L, lua_Reader reader, void* dt, [NativeTypeName("const char *")] byte* chunkname);

        /// <summary>
        /// [-0, +0, m]
        /// Dumps a function as a binary chunk. Receives a Lua function on the top of the stack and produces a binary chunk that, if loaded again, results in a function equivalent to the one dumped. As it produces parts of the chunk, lua_dump calls function writer (see lua_Writer) with the given data to write them.
        /// The value returned is the error code returned by the last call to the writer; 0 means no errors.
        /// This function does not pop the Lua function from the stack.
        /// </summary>
        /// <code>
        /// int lua_dump (lua_State *L, lua_Writer writer, void *data);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_dump(lua_State* L, lua_Writer writer, void* data);

        /// <summary>
        /// [-?, +?, -]
        /// Yields a coroutine.
        /// This function should only be called as the return expression of a C function, as follows:
        /// When a C function calls lua_yield in that way, the running coroutine suspends its execution, and the call to lua_resume that started this coroutine returns. The parameter nresults is the number of values from the stack that are passed as results to lua_resume.
        /// </summary>
        /// <code>
        /// int lua_yield  (lua_State *L, int nresults);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_yield(lua_State* L, int nresults);

        /// <summary>
        /// [-?, +?, -]
        /// Starts and resumes a coroutine in a given thread.
        /// To start a coroutine, you first create a new thread (see lua_newthread); then you push onto its stack the main function plus any arguments; then you call lua_resume, with narg being the number of arguments. This call returns when the coroutine suspends or finishes its execution. When it returns, the stack contains all values passed to lua_yield, or all values returned by the body function. lua_resume returns LUA_YIELD if the coroutine yields, 0 if the coroutine finishes its execution without errors, or an error code in case of errors (see lua_pcall). In case of errors, the stack is not unwound, so you can use the debug API over it. The error message is on the top of the stack. To restart a coroutine, you put on its stack only the values to be passed as results from yield, and then call lua_resume.
        /// </summary>
        /// <code>
        /// int lua_resume (lua_State *L, int narg);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_resume(lua_State* L, int narg);

        /// <summary>
        /// [-0, +0, -]
        /// Returns the status of the thread L.
        /// The status can be 0 for a normal thread, an error code if the thread finished its execution with an error, or LUA_YIELD if the thread is suspended.
        /// </summary>
        /// <code>
        /// int lua_status (lua_State *L);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_status(lua_State* L);

        /// <summary>
        /// [-0, +0, e]
        /// Controls the garbage collector.
        /// This function performs several tasks, according to the value of the parameter what:
        /// - LUA_GCSTOP: stops the garbage collector.
        /// - LUA_GCRESTART: restarts the garbage collector.
        /// - LUA_GCCOLLECT: performs a full garbage-collection cycle.
        /// - LUA_GCCOUNT: returns the current amount of memory (in Kbytes) in use by Lua.
        /// - LUA_GCCOUNTB: returns the remainder of dividing the current amount of bytes of memory in use by Lua by 1024.
        /// - LUA_GCSTEP: performs an incremental step of garbage collection. The step "size" is controlled by data (larger values mean more steps) in a non-specified way. If you want to control the step size you must experimentally tune the value of data. The function returns 1 if the step finished a garbage-collection cycle.
        /// - LUA_GCSETPAUSE: sets data as the new value for the pause of the collector (see §2.10). The function returns the previous value of the pause.
        /// - LUA_GCSETSTEPMUL: sets data as the new value for the step multiplier of the collector (see §2.10). The function returns the previous value of the step multiplier.
        /// </summary>
        /// <code>
        /// int lua_gc (lua_State *L, int what, int data);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_gc(lua_State* L, int what, int data);

        /// <summary>
        /// [-1, +0, v]
        /// Generates a Lua error. The error message (which can actually be a Lua value of any type) must be on the stack top. This function does a long jump, and therefore never returns. (see luaL_error).
        /// </summary>
        /// <code>
        /// int lua_error (lua_State *L);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_error(lua_State* L);

        /// <summary>
        /// [-1, +(2|0), e]
        /// Pops a key from the stack, and pushes a key-value pair from the table at the given index (the "next" pair after the given key). If there are no more elements in the table, then lua_next returns 0 (and pushes nothing).
        /// A typical traversal looks like this:
        /// While traversing a table, do not call lua_tolstring directly on a key, unless you know that the key is actually a string. Recall that lua_tolstring changes the value at the given index; this confuses the next call to lua_next.
        /// </summary>
        /// <code>
        /// int lua_next (lua_State *L, int index);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_next(lua_State* L, int idx);

        /// <summary>
        /// [-n, +1, e]
        /// Concatenates the n values at the top of the stack, pops them, and leaves the result at the top. If n is 1, the result is the single value on the stack (that is, the function does nothing); if n is 0, the result is the empty string. Concatenation is performed following the usual semantics of Lua (see §2.5.4).
        /// </summary>
        /// <code>
        /// void lua_concat (lua_State *L, int n);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_concat(lua_State* L, int n);

        /// <summary>
        /// [-0, +0, -]
        /// Returns the memory-allocation function of a given state. If ud is not NULL, Lua stores in *ud the opaque pointer passed to lua_newstate.
        /// </summary>
        /// <code>
        /// lua_Alloc lua_getallocf (lua_State *L, void **ud);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Alloc lua_getallocf(lua_State* L, void** ud);

        /// <summary>
        /// [-0, +0, -]
        /// Changes the allocator function of a given state to f with user data ud.
        /// </summary>
        /// <code>
        /// void lua_setallocf (lua_State *L, lua_Alloc f, void *ud);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_setallocf(lua_State* L, lua_Alloc f, void* ud);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_setlevel(lua_State* from, lua_State* to);

        /// <summary>
        /// [-0, +0, -]
        /// Get information about the interpreter runtime stack.
        /// This function fills parts of a lua_Debug structure with an identification of the activation record of the function executing at a given level. Level 0 is the current running function, whereas level n+1 is the function that has called level n. When there are no errors, lua_getstack returns 1; when called with a level greater than the stack depth, it returns 0.
        /// </summary>
        /// <code>
        /// int lua_getstack (lua_State *L, int level, lua_Debug *ar);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_getstack(lua_State* L, int level, lua_Debug* ar);

        /// <summary>
        /// [-(0|1), +(0|1|2), m]
        /// Returns information about a specific function or function invocation.
        /// To get information about a function invocation, the parameter ar must be a valid activation record that was filled by a previous call to lua_getstack or given as argument to a hook (see lua_Hook).
        /// To get information about a function you push it onto the stack and start the what string with the character '>'. (In that case, lua_getinfo pops the function in the top of the stack.) For instance, to know in which line a function f was defined, you can write the following code:
        /// Each character in the string what selects some fields of the structure ar to be filled or a value to be pushed on the stack:
        /// - 'n': fills in the field name and namewhat;
        /// - 'S': fills in the fields source, short_src, linedefined, lastlinedefined, and what;
        /// - 'l': fills in the field currentline;
        /// - 'u': fills in the field nups;
        /// - 'f': pushes onto the stack the function that is running at the given level;
        /// - 'L': pushes onto the stack a table whose indices are the numbers of the lines that are valid on the function. (A valid line is a line with some associated code, that is, a line where you can put a break point. Non-valid lines include empty lines and comments.)
        /// This function returns 0 on error (for instance, an invalid option in what).
        /// </summary>
        /// <code>
        /// int lua_getinfo (lua_State *L, const char *what, lua_Debug *ar);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_getinfo(lua_State* L, [NativeTypeName("const char *")] byte* what, lua_Debug* ar);

        /// <summary>
        /// [-0, +(0|1), -]
        /// Gets information about a local variable of a given activation record. The parameter ar must be a valid activation record that was filled by a previous call to lua_getstack or given as argument to a hook (see lua_Hook). The index n selects which local variable to inspect (1 is the first parameter or active local variable, and so on, until the last active local variable). lua_getlocal pushes the variable's value onto the stack and returns its name.
        /// Variable names starting with '(' (open parentheses) represent internal variables (loop control variables, temporaries, and C function locals).
        /// Returns NULL (and pushes nothing) when the index is greater than the number of active local variables.
        /// </summary>
        /// <code>
        /// const char *lua_getlocal (lua_State *L, lua_Debug *ar, int n);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_getlocal", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_getlocal(lua_State* L, [NativeTypeName("const lua_Debug *")] lua_Debug* ar, int n);

        /// <summary>
        /// [-(0|1), +0, -]
        /// Sets the value of a local variable of a given activation record. Parameters ar and n are as in lua_getlocal (see lua_getlocal). lua_setlocal assigns the value at the top of the stack to the variable and returns its name. It also pops the value from the stack.
        /// Returns NULL (and pops nothing) when the index is greater than the number of active local variables.
        /// </summary>
        /// <code>
        /// const char *lua_setlocal (lua_State *L, lua_Debug *ar, int n);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_setlocal", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_setlocal(lua_State* L, [NativeTypeName("const lua_Debug *")] lua_Debug* ar, int n);

        /// <summary>
        /// [-0, +(0|1), -]
        /// Gets information about a closure's upvalue. (For Lua functions, upvalues are the external local variables that the function uses, and that are consequently included in its closure.) lua_getupvalue gets the index n of an upvalue, pushes the upvalue's value onto the stack, and returns its name. funcindex points to the closure in the stack. (Upvalues have no particular order, as they are active through the whole function. So, they are numbered in an arbitrary order.)
        /// Returns NULL (and pushes nothing) when the index is greater than the number of upvalues. For C functions, this function uses the empty string "" as a name for all upvalues.
        /// </summary>
        /// <code>
        /// const char *lua_getupvalue (lua_State *L, int funcindex, int n);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_getupvalue", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_getupvalue(lua_State* L, int funcindex, int n);

        /// <summary>
        /// [-(0|1), +0, -]
        /// Sets the value of a closure's upvalue. It assigns the value at the top of the stack to the upvalue and returns its name. It also pops the value from the stack. Parameters funcindex and n are as in the lua_getupvalue (see lua_getupvalue).
        /// Returns NULL (and pops nothing) when the index is greater than the number of upvalues.
        /// The auxiliary library provides several convenient functions to interface C with Lua. While the basic API provides the primitive functions for all interactions between C and Lua, the auxiliary library provides higher-level functions for some common tasks.
        /// All functions from the auxiliary library are defined in header file lauxlib.h and have a prefix luaL_.
        /// All functions in the auxiliary library are built on top of the basic API, and so they provide nothing that cannot be done with this API.
        /// Several functions in the auxiliary library are used to check C function arguments. Their names are always luaL_check* or luaL_opt*. All of these functions throw an error if the check is not satisfied. Because the error message is formatted for arguments (e.g., "bad argument #1"), you should not use these functions for other stack values.
        /// </summary>
        /// <code>
        /// const char *lua_setupvalue (lua_State *L, int funcindex, int n);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_setupvalue", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_setupvalue(lua_State* L, int funcindex, int n);

        /// <summary>
        /// [-0, +0, -]
        /// Sets the debugging hook function.
        /// Argument f is the hook function. mask specifies on which events the hook will be called: it is formed by a bitwise or of the constants LUA_MASKCALL, LUA_MASKRET, LUA_MASKLINE, and LUA_MASKCOUNT. The count argument is only meaningful when the mask includes LUA_MASKCOUNT. For each event, the hook is called as explained below:
        /// - The call hook: is called when the interpreter calls a function. The hook is called just after Lua enters the new function, before the function gets its arguments.
        /// - The return hook: is called when the interpreter returns from a function. The hook is called just before Lua leaves the function. You have no access to the values to be returned by the function.
        /// - The line hook: is called when the interpreter is about to start the execution of a new line of code, or when it jumps back in the code (even to the same line). (This event only happens while Lua is executing a Lua function.)
        /// - The count hook: is called after the interpreter executes every count instructions. (This event only happens while Lua is executing a Lua function.)
        /// A hook is disabled by setting mask to zero.
        /// </summary>
        /// <code>
        /// int lua_sethook (lua_State *L, lua_Hook f, int mask, int count);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_sethook(lua_State* L, lua_Hook func, int mask, int count);

        /// <summary>
        /// [-0, +0, -]
        /// Returns the current hook function.
        /// </summary>
        /// <code>
        /// lua_Hook lua_gethook (lua_State *L);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Hook lua_gethook(lua_State* L);

        /// <summary>
        /// [-0, +0, -]
        /// Returns the current hook mask.
        /// </summary>
        /// <code>
        /// int lua_gethookmask (lua_State *L);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_gethookmask(lua_State* L);

        /// <summary>
        /// [-0, +0, -]
        /// Returns the current hook count.
        /// </summary>
        /// <code>
        /// int lua_gethookcount (lua_State *L);
        /// </code>
        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_gethookcount(lua_State* L);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void* lua_upvalueid(lua_State* L, int idx, int n);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_upvaluejoin(lua_State* L, int idx1, int n1, int idx2, int n2);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_loadx(lua_State* L, lua_Reader reader, void* dt, [NativeTypeName("const char *")] byte* chunkname, [NativeTypeName("const char *")] byte* mode);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_version", ExactSpelling = true)]
        [return: NativeTypeName("const lua_Number *")]
        [SuppressGCTransition]
        public static extern lua_Number* _lua_version(lua_State* L);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_copy(lua_State* L, int fromidx, int toidx);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Number lua_tonumberx(lua_State* L, int idx, int* isnum);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Integer lua_tointegerx(lua_State* L, int idx, int* isnum);

        [DllImport("luajit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_isyieldable(lua_State* L);
    }
}
