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
        /// <summary>
        /// [-0, +0, –]
        /// Creates a new independent state and returns its main thread. Returns NULL if it cannot create the state (due to lack of memory). The argument f is the allocator function; Lua will do all memory allocation for this state through this function (see lua_Alloc). The second argument, ud, is an opaque pointer that Lua passes to the allocator in every call.
        /// </summary>
        /// <code>
        /// lua_State *lua_newstate (lua_Alloc f, void *ud);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern lua_State* lua_newstate(lua_Alloc f, void* ud);

        /// <summary>
        /// [-0, +0, –]
        /// Close all active to-be-closed variables in the main thread, release all objects in the given Lua state (calling the corresponding garbage-collection metamethods, if any), and frees all dynamic memory used by this state.
        /// On several platforms, you may not need to call this function, because all resources are naturally released when the host program ends. On the other hand, long-running programs that create multiple states, such as daemons or web servers, will probably need to close states as soon as they are not needed.
        /// </summary>
        /// <code>
        /// void lua_close (lua_State *L);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_close(lua_State* L);

        /// <summary>
        /// [-0, +1, m]
        /// Creates a new thread, pushes it on the stack, and returns a pointer to a lua_State that represents this new thread. The new thread returned by this function shares with the original thread its global environment, but has an independent execution stack.
        /// Threads are subject to garbage collection, like any Lua object.
        /// </summary>
        /// <code>
        /// lua_State *lua_newthread (lua_State *L);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_State* lua_newthread(lua_State* L);

        /// <summary>
        /// [-0, +?, –]
        /// Resets a thread, cleaning its call stack and closing all pending to-be-closed variables. Returns a status code: LUA_OK for no errors in the thread (either the original error that stopped the thread or errors in closing methods), or an error status otherwise. In case of error, leaves the error object on the top of the stack.
        /// The parameter from represents the coroutine that is resetting L. If there is no such coroutine, this parameter can be NULL.
        /// (This function was introduced in release 5.4.6.)
        /// </summary>
        /// <code>
        /// int lua_closethread (lua_State *L, lua_State *from);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_closethread(lua_State* L, lua_State* from);

        /// <summary>
        /// [-0, +?, –]
        /// This function is deprecated; it is equivalent to lua_closethread with from being NULL.
        /// </summary>
        /// <code>
        /// int lua_resetthread (lua_State *L);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_resetthread(lua_State* L);

        /// <summary>
        /// [-0, +0, –]
        /// Sets a new panic function and returns the old one (see §4.4).
        /// </summary>
        /// <code>
        /// lua_CFunction lua_atpanic (lua_State *L, lua_CFunction panicf);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_CFunction lua_atpanic(lua_State* L, lua_CFunction panicf);

        /// <summary>
        /// [-0, +0, –]
        /// Returns the version number of this core.
        /// </summary>
        /// <code>
        /// lua_Number lua_version (lua_State *L);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_version", ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Number _lua_version(lua_State* L);

        /// <summary>
        /// [-0, +0, –]
        /// Converts the acceptable index idx into an equivalent absolute index (that is, one that does not depend on the stack size).
        /// </summary>
        /// <code>
        /// int lua_absindex (lua_State *L, int idx);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_absindex(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, –]
        /// Returns the index of the top element in the stack. Because indices start at 1, this result is equal to the number of elements in the stack; in particular, 0 means an empty stack.
        /// </summary>
        /// <code>
        /// int lua_gettop (lua_State *L);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_gettop(lua_State* L);

        /// <summary>
        /// [-?, +?, e]
        /// Accepts any index, or 0, and sets the stack top to this index. If the new top is greater than the old one, then the new elements are filled with nil. If index is 0, then all stack elements are removed.
        /// This function can run arbitrary code when removing an index marked as to-be-closed from the stack.
        /// </summary>
        /// <code>
        /// void lua_settop (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_settop(lua_State* L, int idx);

        /// <summary>
        /// [-0, +1, –]
        /// Pushes a copy of the element at the given index onto the stack.
        /// </summary>
        /// <code>
        /// void lua_pushvalue (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushvalue(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, –]
        /// Rotates the stack elements between the valid index idx and the top of the stack. The elements are rotated n positions in the direction of the top, for a positive n, or -n positions in the direction of the bottom, for a negative n. The absolute value of n must not be greater than the size of the slice being rotated. This function cannot be called with a pseudo-index, because a pseudo-index is not an actual stack position.
        /// </summary>
        /// <code>
        /// void lua_rotate (lua_State *L, int idx, int n);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_rotate(lua_State* L, int idx, int n);

        /// <summary>
        /// [-0, +0, –]
        /// Copies the element at index fromidx into the valid index toidx, replacing the value at that position. Values at other positions are not affected.
        /// </summary>
        /// <code>
        /// void lua_copy (lua_State *L, int fromidx, int toidx);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_copy(lua_State* L, int fromidx, int toidx);

        /// <summary>
        /// [-0, +0, –]
        /// Ensures that the stack has space for at least n extra elements, that is, that you can safely push up to n values into it. It returns false if it cannot fulfill the request, either because it would cause the stack to be greater than a fixed maximum size (typically at least several thousand elements) or because it cannot allocate memory for the extra space. This function never shrinks the stack; if the stack already has space for the extra elements, it is left unchanged.
        /// </summary>
        /// <code>
        /// int lua_checkstack (lua_State *L, int n);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_checkstack(lua_State* L, int n);

        /// <summary>
        /// [-?, +?, –]
        /// Exchange values between different threads of the same state.
        /// This function pops n values from the stack from, and pushes them onto the stack to.
        /// </summary>
        /// <code>
        /// void lua_xmove (lua_State *from, lua_State *to, int n);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_xmove(lua_State* from, lua_State* to, int n);

        /// <summary>
        /// [-0, +0, –]
        /// Returns 1 if the value at the given index is a number or a string convertible to a number, and 0 otherwise.
        /// </summary>
        /// <code>
        /// int lua_isnumber (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_isnumber(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, –]
        /// Returns 1 if the value at the given index is a string or a number (which is always convertible to a string), and 0 otherwise.
        /// </summary>
        /// <code>
        /// int lua_isstring (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_isstring(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, –]
        /// Returns 1 if the value at the given index is a C function, and 0 otherwise.
        /// </summary>
        /// <code>
        /// int lua_iscfunction (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_iscfunction(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, –]
        /// Returns 1 if the value at the given index is an integer (that is, the value is a number and is represented as an integer), and 0 otherwise.
        /// </summary>
        /// <code>
        /// int lua_isinteger (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_isinteger(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, –]
        /// Returns 1 if the value at the given index is a userdata (either full or light), and 0 otherwise.
        /// </summary>
        /// <code>
        /// int lua_isuserdata (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_isuserdata(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, –]
        /// Returns the type of the value in the given valid index, or LUA_TNONE for a non-valid but acceptable index. The types returned by lua_type are coded by the following constants defined in lua.h: LUA_TNIL, LUA_TNUMBER, LUA_TBOOLEAN, LUA_TSTRING, LUA_TTABLE, LUA_TFUNCTION, LUA_TUSERDATA, LUA_TTHREAD, and LUA_TLIGHTUSERDATA.
        /// </summary>
        /// <code>
        /// int lua_type (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_type(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, –]
        /// Returns the name of the type encoded by the value tp, which must be one the values returned by lua_type.
        /// </summary>
        /// <code>
        /// const char *lua_typename (lua_State *L, int tp);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_typename", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_typename(lua_State* L, int tp);

        /// <summary>
        /// [-0, +0, –]
        /// Converts the Lua value at the given index to the C type lua_Number (see lua_Number). The Lua value must be a number or a string convertible to a number (see §3.4.3); otherwise, lua_tonumberx returns 0.
        /// If isnum is not NULL, its referent is assigned a boolean value that indicates whether the operation succeeded.
        /// </summary>
        /// <code>
        /// lua_Number lua_tonumberx (lua_State *L, int index, int *isnum);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Number lua_tonumberx(lua_State* L, int idx, int* isnum);

        /// <summary>
        /// [-0, +0, –]
        /// Converts the Lua value at the given index to the signed integral type lua_Integer. The Lua value must be an integer, or a number or string convertible to an integer (see §3.4.3); otherwise, lua_tointegerx returns 0.
        /// If isnum is not NULL, its referent is assigned a boolean value that indicates whether the operation succeeded.
        /// </summary>
        /// <code>
        /// lua_Integer lua_tointegerx (lua_State *L, int index, int *isnum);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Integer lua_tointegerx(lua_State* L, int idx, int* isnum);

        /// <summary>
        /// [-0, +0, –]
        /// Converts the Lua value at the given index to a C boolean value (0 or 1). Like all tests in Lua, lua_toboolean returns true for any Lua value different from false and nil; otherwise it returns false. (If you want to accept only actual boolean values, use lua_isboolean to test the value's type.)
        /// </summary>
        /// <code>
        /// int lua_toboolean (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_toboolean(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, m]
        /// Converts the Lua value at the given index to a C string. If len is not NULL, it sets *len with the string length. The Lua value must be a string or a number; otherwise, the function returns NULL. If the value is a number, then lua_tolstring also changes the actual value in the stack to a string. (This change confuses lua_next when lua_tolstring is applied to keys during a table traversal.)
        /// lua_tolstring returns a pointer to a string inside the Lua state (see §4.1.3). This string always has a zero ('\0') after its last character (as in C), but can contain other zeros in its body.
        /// This function can raise memory errors only when converting a number to a string (as then it may create a new string).
        /// </summary>
        /// <code>
        /// const char *lua_tolstring (lua_State *L, int index, size_t *len);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        public static extern byte* lua_tolstring(lua_State* L, int idx, [NativeTypeName("size_t *")] nuint* len);

        /// <summary>
        /// [-0, +0, –]
        /// Returns the raw "length" of the value at the given index: for strings, this is the string length; for tables, this is the result of the length operator ('#') with no metamethods; for userdata, this is the size of the block of memory allocated for the userdata. For other values, this call returns 0.
        /// </summary>
        /// <code>
        /// lua_Unsigned lua_rawlen (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("lua_Unsigned")]
        public static extern ulong lua_rawlen(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, –]
        /// Converts a value at the given index to a C function. That value must be a C function; otherwise, returns NULL.
        /// </summary>
        /// <code>
        /// lua_CFunction lua_tocfunction (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_CFunction lua_tocfunction(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, –]
        /// If the value at the given index is a full userdata, returns its memory-block address. If the value is a light userdata, returns its value (a pointer). Otherwise, returns NULL.
        /// </summary>
        /// <code>
        /// void *lua_touserdata (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void* lua_touserdata(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, –]
        /// Converts the value at the given index to a Lua thread (represented as lua_State*). This value must be a thread; otherwise, the function returns NULL.
        /// </summary>
        /// <code>
        /// lua_State *lua_tothread (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_State* lua_tothread(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, –]
        /// Converts the value at the given index to a generic C pointer (void*). The value can be a userdata, a table, a thread, a string, or a function; otherwise, lua_topointer returns NULL. Different objects will give different pointers. There is no way to convert the pointer back to its original value.
        /// Typically this function is used only for hashing and debug information.
        /// </summary>
        /// <code>
        /// const void *lua_topointer (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const void *")]
        [SuppressGCTransition]
        public static extern void* lua_topointer(lua_State* L, int idx);

        /// <summary>
        /// [-(2|1), +1, e]
        /// Performs an arithmetic or bitwise operation over the two values (or one, in the case of negations) at the top of the stack, with the value on the top being the second operand, pops these values, and pushes the result of the operation. The function follows the semantics of the corresponding Lua operator (that is, it may call metamethods).
        /// The value of op must be one of the following constants:
        /// - LUA_OPADD: performs addition (+)
        /// - LUA_OPSUB: performs subtraction (-)
        /// - LUA_OPMUL: performs multiplication (*)
        /// - LUA_OPDIV: performs float division (/)
        /// - LUA_OPIDIV: performs floor division (//)
        /// - LUA_OPMOD: performs modulo (%)
        /// - LUA_OPPOW: performs exponentiation (^)
        /// - LUA_OPUNM: performs mathematical negation (unary -)
        /// - LUA_OPBNOT: performs bitwise NOT (~)
        /// - LUA_OPBAND: performs bitwise AND (&)
        /// - LUA_OPBOR: performs bitwise OR (|)
        /// - LUA_OPBXOR: performs bitwise exclusive OR (~)
        /// - LUA_OPSHL: performs left shift (<<)
        /// - LUA_OPSHR: performs right shift (>>)
        /// </summary>
        /// <code>
        /// void lua_arith (lua_State *L, int op);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_arith(lua_State* L, int op);

        /// <summary>
        /// [-0, +0, –]
        /// Returns 1 if the two values in indices index1 and index2 are primitively equal (that is, equal without calling the __eq metamethod). Otherwise returns 0. Also returns 0 if any of the indices are not valid.
        /// </summary>
        /// <code>
        /// int lua_rawequal (lua_State *L, int index1, int index2);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_rawequal(lua_State* L, int idx1, int idx2);

        /// <summary>
        /// [-0, +0, e]
        /// Compares two Lua values. Returns 1 if the value at index index1 satisfies op when compared with the value at index index2, following the semantics of the corresponding Lua operator (that is, it may call metamethods). Otherwise returns 0. Also returns 0 if any of the indices is not valid.
        /// The value of op must be one of the following constants:
        /// - LUA_OPEQ: compares for equality (==)
        /// - LUA_OPLT: compares for less than (<)
        /// - LUA_OPLE: compares for less or equal (<=)
        /// </summary>
        /// <code>
        /// int lua_compare (lua_State *L, int index1, int index2, int op);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_compare(lua_State* L, int idx1, int idx2, int op);

        /// <summary>
        /// [-0, +1, –]
        /// Pushes a nil value onto the stack.
        /// </summary>
        /// <code>
        /// void lua_pushnil (lua_State *L);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushnil(lua_State* L);

        /// <summary>
        /// [-0, +1, –]
        /// Pushes a float with value n onto the stack.
        /// </summary>
        /// <code>
        /// void lua_pushnumber (lua_State *L, lua_Number n);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushnumber(lua_State* L, lua_Number n);

        /// <summary>
        /// [-0, +1, –]
        /// Pushes an integer with value n onto the stack.
        /// </summary>
        /// <code>
        /// void lua_pushinteger (lua_State *L, lua_Integer n);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushinteger(lua_State* L, lua_Integer n);

        /// <summary>
        /// [-0, +1, m]
        /// Pushes the string pointed to by s with size len onto the stack. Lua will make or reuse an internal copy of the given string, so the memory at s can be freed or reused immediately after the function returns. The string can contain any binary data, including embedded zeros.
        /// Returns a pointer to the internal copy of the string (see §4.1.3).
        /// </summary>
        /// <code>
        /// const char *lua_pushlstring (lua_State *L, const char *s, size_t len);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* lua_pushlstring(lua_State* L, [NativeTypeName("const char *")] byte* s, [NativeTypeName("size_t")] nuint len);

        /// <summary>
        /// [-0, +1, m]
        /// Pushes the zero-terminated string pointed to by s onto the stack. Lua will make or reuse an internal copy of the given string, so the memory at s can be freed or reused immediately after the function returns.
        /// Returns a pointer to the internal copy of the string (see §4.1.3).
        /// If s is NULL, pushes nil and returns NULL.
        /// </summary>
        /// <code>
        /// const char *lua_pushstring (lua_State *L, const char *s);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* lua_pushstring(lua_State* L, [NativeTypeName("const char *")] byte* s);

        /// <summary>
        /// [-0, +1, v]
        /// Equivalent to lua_pushfstring, except that it receives a va_list instead of a variable number of arguments.
        /// </summary>
        /// <code>
        /// const char *lua_pushvfstring (lua_State *L,
        /// const char *fmt,
        /// va_list argp);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* lua_pushvfstring(lua_State* L, [NativeTypeName("const char *")] byte* fmt, [NativeTypeName("va_list")] byte* argp);

        /// <summary>
        /// [-n, +1, m]
        /// Pushes a new C closure onto the stack. This function receives a pointer to a C function and pushes onto the stack a Lua value of type function that, when called, invokes the corresponding C function. The parameter n tells how many upvalues this function will have (see §4.2).
        /// Any function to be callable by Lua must follow the correct protocol to receive its parameters and return its results (see lua_CFunction).
        /// When a C function is created, it is possible to associate some values with it, the so called upvalues; these upvalues are then accessible to the function whenever it is called. This association is called a C closure (see §4.2). To create a C closure, first the initial values for its upvalues must be pushed onto the stack. (When there are multiple upvalues, the first value is pushed first.) Then lua_pushcclosure is called to create and push the C function onto the stack, with the argument n telling how many values will be associated with the function. lua_pushcclosure also pops these values from the stack.
        /// The maximum value for n is 255.
        /// When n is zero, this function creates a light C function, which is just a pointer to the C function. In that case, it never raises a memory error.
        /// </summary>
        /// <code>
        /// void lua_pushcclosure (lua_State *L, lua_CFunction fn, int n);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushcclosure(lua_State* L, lua_CFunction fn, int n);

        /// <summary>
        /// [-0, +1, –]
        /// Pushes a boolean value with value b onto the stack.
        /// </summary>
        /// <code>
        /// void lua_pushboolean (lua_State *L, int b);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushboolean(lua_State* L, int b);

        /// <summary>
        /// [-0, +1, –]
        /// Pushes a light userdata onto the stack.
        /// Userdata represent C values in Lua. A light userdata represents a pointer, a void*. It is a value (like a number): you do not create it, it has no individual metatable, and it is not collected (as it was never created). A light userdata is equal to "any" light userdata with the same C address.
        /// </summary>
        /// <code>
        /// void lua_pushlightuserdata (lua_State *L, void *p);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_pushlightuserdata(lua_State* L, void* p);

        /// <summary>
        /// [-0, +1, –]
        /// Pushes the thread represented by L onto the stack. Returns 1 if this thread is the main thread of its state.
        /// </summary>
        /// <code>
        /// int lua_pushthread (lua_State *L);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_pushthread(lua_State* L);

        /// <summary>
        /// [-0, +1, e]
        /// Pushes onto the stack the value of the global name. Returns the type of that value.
        /// </summary>
        /// <code>
        /// int lua_getglobal (lua_State *L, const char *name);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_getglobal(lua_State* L, [NativeTypeName("const char *")] byte* name);

        /// <summary>
        /// [-1, +1, e]
        /// Pushes onto the stack the value t[k], where t is the value at the given index and k is the value on the top of the stack.
        /// This function pops the key from the stack, pushing the resulting value in its place. As in Lua, this function may trigger a metamethod for the "index" event (see §2.4).
        /// Returns the type of the pushed value.
        /// </summary>
        /// <code>
        /// int lua_gettable (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_gettable(lua_State* L, int idx);

        /// <summary>
        /// [-0, +1, e]
        /// Pushes onto the stack the value t[k], where t is the value at the given index. As in Lua, this function may trigger a metamethod for the "index" event (see §2.4).
        /// Returns the type of the pushed value.
        /// </summary>
        /// <code>
        /// int lua_getfield (lua_State *L, int index, const char *k);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_getfield(lua_State* L, int idx, [NativeTypeName("const char *")] byte* k);

        /// <summary>
        /// [-0, +1, e]
        /// Pushes onto the stack the value t[i], where t is the value at the given index. As in Lua, this function may trigger a metamethod for the "index" event (see §2.4).
        /// Returns the type of the pushed value.
        /// </summary>
        /// <code>
        /// int lua_geti (lua_State *L, int index, lua_Integer i);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_geti(lua_State* L, int idx, lua_Integer n);

        /// <summary>
        /// [-1, +1, –]
        /// Similar to lua_gettable, but does a raw access (i.e., without metamethods). The value at index must be a table.
        /// </summary>
        /// <code>
        /// int lua_rawget (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_rawget(lua_State* L, int idx);

        /// <summary>
        /// [-0, +1, –]
        /// Pushes onto the stack the value t[n], where t is the table at the given index. The access is raw, that is, it does not use the __index metavalue.
        /// Returns the type of the pushed value.
        /// </summary>
        /// <code>
        /// int lua_rawgeti (lua_State *L, int index, lua_Integer n);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_rawgeti(lua_State* L, int idx, lua_Integer n);

        /// <summary>
        /// [-0, +1, –]
        /// Pushes onto the stack the value t[k], where t is the table at the given index and k is the pointer p represented as a light userdata. The access is raw; that is, it does not use the __index metavalue.
        /// Returns the type of the pushed value.
        /// </summary>
        /// <code>
        /// int lua_rawgetp (lua_State *L, int index, const void *p);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_rawgetp(lua_State* L, int idx, [NativeTypeName("const void *")] void* p);

        /// <summary>
        /// [-0, +1, m]
        /// Creates a new empty table and pushes it onto the stack. Parameter narr is a hint for how many elements the table will have as a sequence; parameter nrec is a hint for how many other elements the table will have. Lua may use these hints to preallocate memory for the new table. This preallocation may help performance when you know in advance how many elements the table will have. Otherwise you can use the function lua_newtable.
        /// </summary>
        /// <code>
        /// void lua_createtable (lua_State *L, int narr, int nrec);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_createtable(lua_State* L, int narr, int nrec);

        /// <summary>
        /// [-0, +1, m]
        /// This function creates and pushes on the stack a new full userdata, with nuvalue associated Lua values, called user values, plus an associated block of raw memory with size bytes. (The user values can be set and read with the functions lua_setiuservalue and lua_getiuservalue.)
        /// The function returns the address of the block of memory. Lua ensures that this address is valid as long as the corresponding userdata is alive (see §2.5). Moreover, if the userdata is marked for finalization (see §2.5.3), its address is valid at least until the call to its finalizer.
        /// </summary>
        /// <code>
        /// void *lua_newuserdatauv (lua_State *L, size_t size, int nuvalue);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* lua_newuserdatauv(lua_State* L, [NativeTypeName("size_t")] nuint sz, int nuvalue);

        /// <summary>
        /// [-0, +(0|1), –]
        /// If the value at the given index has a metatable, the function pushes that metatable onto the stack and returns 1. Otherwise, the function returns 0 and pushes nothing on the stack.
        /// </summary>
        /// <code>
        /// int lua_getmetatable (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_getmetatable(lua_State* L, int objindex);

        /// <summary>
        /// [-0, +1, –]
        /// Pushes onto the stack the n-th user value associated with the full userdata at the given index and returns the type of the pushed value.
        /// If the userdata does not have that value, pushes nil and returns LUA_TNONE.
        /// </summary>
        /// <code>
        /// int lua_getiuservalue (lua_State *L, int index, int n);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_getiuservalue(lua_State* L, int idx, int n);

        /// <summary>
        /// [-1, +0, e]
        /// Pops a value from the stack and sets it as the new value of global name.
        /// </summary>
        /// <code>
        /// void lua_setglobal (lua_State *L, const char *name);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_setglobal(lua_State* L, [NativeTypeName("const char *")] byte* name);

        /// <summary>
        /// [-2, +0, e]
        /// Does the equivalent to t[k] = v, where t is the value at the given index, v is the value on the top of the stack, and k is the value just below the top.
        /// This function pops both the key and the value from the stack. As in Lua, this function may trigger a metamethod for the "newindex" event (see §2.4).
        /// </summary>
        /// <code>
        /// void lua_settable (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_settable(lua_State* L, int idx);

        /// <summary>
        /// [-1, +0, e]
        /// Does the equivalent to t[k] = v, where t is the value at the given index and v is the value on the top of the stack.
        /// This function pops the value from the stack. As in Lua, this function may trigger a metamethod for the "newindex" event (see §2.4).
        /// </summary>
        /// <code>
        /// void lua_setfield (lua_State *L, int index, const char *k);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_setfield(lua_State* L, int idx, [NativeTypeName("const char *")] byte* k);

        /// <summary>
        /// [-1, +0, e]
        /// Does the equivalent to t[n] = v, where t is the value at the given index and v is the value on the top of the stack.
        /// This function pops the value from the stack. As in Lua, this function may trigger a metamethod for the "newindex" event (see §2.4).
        /// </summary>
        /// <code>
        /// void lua_seti (lua_State *L, int index, lua_Integer n);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_seti(lua_State* L, int idx, lua_Integer n);

        /// <summary>
        /// [-2, +0, m]
        /// Similar to lua_settable, but does a raw assignment (i.e., without metamethods). The value at index must be a table.
        /// </summary>
        /// <code>
        /// void lua_rawset (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_rawset(lua_State* L, int idx);

        /// <summary>
        /// [-1, +0, m]
        /// Does the equivalent of t[i] = v, where t is the table at the given index and v is the value on the top of the stack.
        /// This function pops the value from the stack. The assignment is raw, that is, it does not use the __newindex metavalue.
        /// </summary>
        /// <code>
        /// void lua_rawseti (lua_State *L, int index, lua_Integer i);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_rawseti(lua_State* L, int idx, lua_Integer n);

        /// <summary>
        /// [-1, +0, m]
        /// Does the equivalent of t[p] = v, where t is the table at the given index, p is encoded as a light userdata, and v is the value on the top of the stack.
        /// This function pops the value from the stack. The assignment is raw, that is, it does not use the __newindex metavalue.
        /// </summary>
        /// <code>
        /// void lua_rawsetp (lua_State *L, int index, const void *p);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_rawsetp(lua_State* L, int idx, [NativeTypeName("const void *")] void* p);

        /// <summary>
        /// [-1, +0, –]
        /// Pops a table or nil from the stack and sets that value as the new metatable for the value at the given index. (nil means no metatable.)
        /// (For historical reasons, this function returns an int, which now is always 1.)
        /// </summary>
        /// <code>
        /// int lua_setmetatable (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_setmetatable(lua_State* L, int objindex);

        /// <summary>
        /// [-1, +0, –]
        /// Pops a value from the stack and sets it as the new n-th user value associated to the full userdata at the given index. Returns 0 if the userdata does not have that value.
        /// </summary>
        /// <code>
        /// int lua_setiuservalue (lua_State *L, int index, int n);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_setiuservalue(lua_State* L, int idx, int n);

        /// <summary>
        /// [-(nargs + 1), +nresults, e]
        /// This function behaves exactly like lua_call, but allows the called function to yield (see §4.5).
        /// </summary>
        /// <code>
        /// void lua_callk (lua_State *L,
        /// int nargs,
        /// int nresults,
        /// lua_KContext ctx,
        /// lua_KFunction k);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_callk(lua_State* L, int nargs, int nresults, [NativeTypeName("lua_KContext")] nint ctx, [NativeTypeName("lua_KFunction")] delegate* unmanaged[Cdecl]<lua_State*, int, nint, int> k);

        /// <summary>
        /// [-(nargs + 1), +(nresults|1), –]
        /// This function behaves exactly like lua_pcall, except that it allows the called function to yield (see §4.5).
        /// </summary>
        /// <code>
        /// int lua_pcallk (lua_State *L,
        /// int nargs,
        /// int nresults,
        /// int msgh,
        /// lua_KContext ctx,
        /// lua_KFunction k);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_pcallk(lua_State* L, int nargs, int nresults, int errfunc, [NativeTypeName("lua_KContext")] nint ctx, [NativeTypeName("lua_KFunction")] delegate* unmanaged[Cdecl]<lua_State*, int, nint, int> k);

        /// <summary>
        /// [-0, +1, –]
        /// Loads a Lua chunk without running it. If there are no errors, lua_load pushes the compiled chunk as a Lua function on top of the stack. Otherwise, it pushes an error message.
        /// The lua_load function uses a user-supplied reader function to read the chunk (see lua_Reader). The data argument is an opaque value passed to the reader function.
        /// The chunkname argument gives a name to the chunk, which is used for error messages and in debug information (see §4.7).
        /// lua_load automatically detects whether the chunk is text or binary and loads it accordingly (see program luac). The string mode works as in function load, with the addition that a NULL value is equivalent to the string "bt".
        /// lua_load uses the stack internally, so the reader function must always leave the stack unmodified when returning.
        /// lua_load can return LUA_OK, LUA_ERRSYNTAX, or LUA_ERRMEM. The function may also return other values corresponding to errors raised by the read function (see §4.4.1).
        /// If the resulting function has upvalues, its first upvalue is set to the value of the global environment stored at index LUA_RIDX_GLOBALS in the registry (see §4.3). When loading main chunks, this upvalue will be the _ENV variable (see §2.2). Other upvalues are initialized with nil.
        /// </summary>
        /// <code>
        /// int lua_load (lua_State *L,
        /// lua_Reader reader,
        /// void *data,
        /// const char *chunkname,
        /// const char *mode);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_load(lua_State* L, lua_Reader reader, void* dt, [NativeTypeName("const char *")] byte* chunkname, [NativeTypeName("const char *")] byte* mode);

        /// <summary>
        /// [-0, +0, –]
        /// Dumps a function as a binary chunk. Receives a Lua function on the top of the stack and produces a binary chunk that, if loaded again, results in a function equivalent to the one dumped. As it produces parts of the chunk, lua_dump calls function writer (see lua_Writer) with the given data to write them.
        /// If strip is true, the binary representation may not include all debug information about the function, to save space.
        /// The value returned is the error code returned by the last call to the writer; 0 means no errors.
        /// This function does not pop the Lua function from the stack.
        /// </summary>
        /// <code>
        /// int lua_dump (lua_State *L,
        /// lua_Writer writer,
        /// void *data,
        /// int strip);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_dump(lua_State* L, lua_Writer writer, void* data, int strip);

        /// <summary>
        /// [-?, +?, v]
        /// Yields a coroutine (thread).
        /// When a C function calls lua_yieldk, the running coroutine suspends its execution, and the call to lua_resume that started this coroutine returns. The parameter nresults is the number of values from the stack that will be passed as results to lua_resume.
        /// When the coroutine is resumed again, Lua calls the given continuation function k to continue the execution of the C function that yielded (see §4.5). This continuation function receives the same stack from the previous function, with the n results removed and replaced by the arguments passed to lua_resume. Moreover, the continuation function receives the value ctx that was passed to lua_yieldk.
        /// Usually, this function does not return; when the coroutine eventually resumes, it continues executing the continuation function. However, there is one special case, which is when this function is called from inside a line or a count hook (see §4.7). In that case, lua_yieldk should be called with no continuation (probably in the form of lua_yield) and no results, and the hook should return immediately after the call. Lua will yield and, when the coroutine resumes again, it will continue the normal execution of the (Lua) function that triggered the hook.
        /// This function can raise an error if it is called from a thread with a pending C call with no continuation function (what is called a C-call boundary), or it is called from a thread that is not running inside a resume (typically the main thread).
        /// </summary>
        /// <code>
        /// int lua_yieldk (lua_State *L,
        /// int nresults,
        /// lua_KContext ctx,
        /// lua_KFunction k);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_yieldk(lua_State* L, int nresults, [NativeTypeName("lua_KContext")] nint ctx, [NativeTypeName("lua_KFunction")] delegate* unmanaged[Cdecl]<lua_State*, int, nint, int> k);

        /// <summary>
        /// [-?, +?, –]
        /// Starts and resumes a coroutine in the given thread L.
        /// To start a coroutine, you push the main function plus any arguments onto the empty stack of the thread. then you call lua_resume, with nargs being the number of arguments. This call returns when the coroutine suspends or finishes its execution. When it returns, *nresults is updated and the top of the stack contains the *nresults values passed to lua_yield or returned by the body function. lua_resume returns LUA_YIELD if the coroutine yields, LUA_OK if the coroutine finishes its execution without errors, or an error code in case of errors (see §4.4.1). In case of errors, the error object is on the top of the stack.
        /// To resume a coroutine, you remove the *nresults yielded values from its stack, push the values to be passed as results from yield, and then call lua_resume.
        /// The parameter from represents the coroutine that is resuming L. If there is no such coroutine, this parameter can be NULL.
        /// </summary>
        /// <code>
        /// int lua_resume (lua_State *L, lua_State *from, int nargs,
        /// int *nresults);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_resume(lua_State* L, lua_State* from, int narg, int* nres);

        /// <summary>
        /// [-0, +0, –]
        /// Returns the status of the thread L.
        /// The status can be LUA_OK for a normal thread, an error code if the thread finished the execution of a lua_resume with an error, or LUA_YIELD if the thread is suspended.
        /// You can call functions only in threads with status LUA_OK. You can resume threads with status LUA_OK (to start a new coroutine) or LUA_YIELD (to resume a coroutine).
        /// </summary>
        /// <code>
        /// int lua_status (lua_State *L);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_status(lua_State* L);

        /// <summary>
        /// [-0, +0, –]
        /// Returns 1 if the given coroutine can yield, and 0 otherwise.
        /// </summary>
        /// <code>
        /// int lua_isyieldable (lua_State *L);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_isyieldable(lua_State* L);

        /// <summary>
        /// [-0, +0, –]
        /// Sets the warning function to be used by Lua to emit warnings (see lua_WarnFunction). The ud parameter sets the value ud passed to the warning function.
        /// </summary>
        /// <code>
        /// void lua_setwarnf (lua_State *L, lua_WarnFunction f, void *ud);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_setwarnf(lua_State* L, [NativeTypeName("lua_WarnFunction")] delegate* unmanaged[Cdecl]<void*, byte*, int, void> f, void* ud);

        /// <summary>
        /// [-0, +0, –]
        /// Emits a warning with the given message. A message in a call with tocont true should be continued in another call to this function.
        /// See warn for more details about warnings.
        /// </summary>
        /// <code>
        /// void lua_warning (lua_State *L, const char *msg, int tocont);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_warning(lua_State* L, [NativeTypeName("const char *")] byte* msg, int tocont);

        /// <summary>
        /// [-1, +0, v]
        /// Raises a Lua error, using the value on the top of the stack as the error object. This function does a long jump, and therefore never returns (see luaL_error).
        /// </summary>
        /// <code>
        /// int lua_error (lua_State *L);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_error(lua_State* L);

        /// <summary>
        /// [-1, +(2|0), v]
        /// Pops a key from the stack, and pushes a key–value pair from the table at the given index, the "next" pair after the given key. If there are no more elements in the table, then lua_next returns 0 and pushes nothing.
        /// A typical table traversal looks like this:
        /// While traversing a table, avoid calling lua_tolstring directly on a key, unless you know that the key is actually a string. Recall that lua_tolstring may change the value at the given index; this confuses the next call to lua_next.
        /// This function may raise an error if the given key is neither nil nor present in the table. See function next for the caveats of modifying the table during its traversal.
        /// </summary>
        /// <code>
        /// int lua_next (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_next(lua_State* L, int idx);

        /// <summary>
        /// [-n, +1, e]
        /// Concatenates the n values at the top of the stack, pops them, and leaves the result on the top. If n is 1, the result is the single value on the stack (that is, the function does nothing); if n is 0, the result is the empty string. Concatenation is performed following the usual semantics of Lua (see §3.4.6).
        /// </summary>
        /// <code>
        /// void lua_concat (lua_State *L, int n);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_concat(lua_State* L, int n);

        /// <summary>
        /// [-0, +1, e]
        /// Returns the length of the value at the given index. It is equivalent to the '#' operator in Lua (see §3.4.7) and may trigger a metamethod for the "length" event (see §2.4). The result is pushed on the stack.
        /// </summary>
        /// <code>
        /// void lua_len (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_len(lua_State* L, int idx);

        /// <summary>
        /// [-0, +1, –]
        /// Converts the zero-terminated string s to a number, pushes that number into the stack, and returns the total size of the string, that is, its length plus one. The conversion can result in an integer or a float, according to the lexical conventions of Lua (see §3.1). The string may have leading and trailing whitespaces and a sign. If the string is not a valid numeral, returns 0 and pushes nothing. (Note that the result can be used as a boolean, true if the conversion succeeds.)
        /// </summary>
        /// <code>
        /// size_t lua_stringtonumber (lua_State *L, const char *s);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint lua_stringtonumber(lua_State* L, [NativeTypeName("const char *")] byte* s);

        /// <summary>
        /// [-0, +0, –]
        /// Returns the memory-allocation function of a given state. If ud is not NULL, Lua stores in *ud the opaque pointer given when the memory-allocator function was set.
        /// </summary>
        /// <code>
        /// lua_Alloc lua_getallocf (lua_State *L, void **ud);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Alloc lua_getallocf(lua_State* L, void** ud);

        /// <summary>
        /// [-0, +0, –]
        /// Changes the allocator function of a given state to f with user data ud.
        /// </summary>
        /// <code>
        /// void lua_setallocf (lua_State *L, lua_Alloc f, void *ud);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_setallocf(lua_State* L, lua_Alloc f, void* ud);

        /// <summary>
        /// [-0, +0, v]
        /// Marks the given index in the stack as a to-be-closed slot (see §3.3.8). Like a to-be-closed variable in Lua, the value at that slot in the stack will be closed when it goes out of scope. Here, in the context of a C function, to go out of scope means that the running function returns to Lua, or there is an error, or the slot is removed from the stack through lua_settop or lua_pop, or there is a call to lua_closeslot. A slot marked as to-be-closed should not be removed from the stack by any other function in the API except lua_settop or lua_pop, unless previously deactivated by lua_closeslot.
        /// This function raises an error if the value at the given slot neither has a __close metamethod nor is a false value.
        /// This function should not be called for an index that is equal to or below an active to-be-closed slot.
        /// Note that, both in case of errors and of a regular return, by the time the __close metamethod runs, the C stack was already unwound, so that any automatic C variable declared in the calling function (e.g., a buffer) will be out of scope.
        /// </summary>
        /// <code>
        /// void lua_toclose (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_toclose(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, e]
        /// Close the to-be-closed slot at the given index and set its value to nil. The index must be the last index previously marked to be closed (see lua_toclose) that is still active (that is, not closed yet).
        /// A __close metamethod cannot yield when called through this function.
        /// (This function was introduced in release 5.4.3.)
        /// </summary>
        /// <code>
        /// void lua_closeslot (lua_State *L, int index);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void lua_closeslot(lua_State* L, int idx);

        /// <summary>
        /// [-0, +0, –]
        /// Gets information about the interpreter runtime stack.
        /// This function fills parts of a lua_Debug structure with an identification of the activation record of the function executing at a given level. Level 0 is the current running function, whereas level n+1 is the function that has called level n (except for tail calls, which do not count in the stack). When called with a level greater than the stack depth, lua_getstack returns 0; otherwise it returns 1.
        /// </summary>
        /// <code>
        /// int lua_getstack (lua_State *L, int level, lua_Debug *ar);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_getstack(lua_State* L, int level, lua_Debug* ar);

        /// <summary>
        /// [-(0|1), +(0|1|2), m]
        /// Gets information about a specific function or function invocation.
        /// To get information about a function invocation, the parameter ar must be a valid activation record that was filled by a previous call to lua_getstack or given as argument to a hook (see lua_Hook).
        /// To get information about a function, you push it onto the stack and start the what string with the character '>'. (In that case, lua_getinfo pops the function from the top of the stack.) For instance, to know in which line a function f was defined, you can write the following code:
        /// Each character in the string what selects some fields of the structure ar to be filled or a value to be pushed on the stack. (These characters are also documented in the declaration of the structure lua_Debug, between parentheses in the comments following each field.)
        /// - 'f': pushes onto the stack the function that is running at the given level;
        /// - 'l': fills in the field currentline;
        /// - 'n': fills in the fields name and namewhat;
        /// - 'r': fills in the fields ftransfer and ntransfer;
        /// - 'S': fills in the fields source, short_src, linedefined, lastlinedefined, and what;
        /// - 't': fills in the field istailcall;
        /// - 'u': fills in the fields nups, nparams, and isvararg;
        /// - 'L': pushes onto the stack a table whose indices are the lines on the function with some associated code, that is, the lines where you can put a break point. (Lines with no code include empty lines and comments.) If this option is given together with option 'f', its table is pushed after the function. This is the only option that can raise a memory error.
        /// This function returns 0 to signal an invalid option in what; even then the valid options are handled correctly.
        /// </summary>
        /// <code>
        /// int lua_getinfo (lua_State *L, const char *what, lua_Debug *ar);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_getinfo(lua_State* L, [NativeTypeName("const char *")] byte* what, lua_Debug* ar);

        /// <summary>
        /// [-0, +(0|1), –]
        /// Gets information about a local variable or a temporary value of a given activation record or a given function.
        /// In the first case, the parameter ar must be a valid activation record that was filled by a previous call to lua_getstack or given as argument to a hook (see lua_Hook). The index n selects which local variable to inspect; see debug.getlocal for details about variable indices and names.
        /// lua_getlocal pushes the variable's value onto the stack and returns its name.
        /// In the second case, ar must be NULL and the function to be inspected must be on the top of the stack. In this case, only parameters of Lua functions are visible (as there is no information about what variables are active) and no values are pushed onto the stack.
        /// Returns NULL (and pushes nothing) when the index is greater than the number of active local variables.
        /// </summary>
        /// <code>
        /// const char *lua_getlocal (lua_State *L, const lua_Debug *ar, int n);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_getlocal", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_getlocal(lua_State* L, [NativeTypeName("const lua_Debug *")] lua_Debug* ar, int n);

        /// <summary>
        /// [-(0|1), +0, –]
        /// Sets the value of a local variable of a given activation record. It assigns the value on the top of the stack to the variable and returns its name. It also pops the value from the stack.
        /// Returns NULL (and pops nothing) when the index is greater than the number of active local variables.
        /// Parameters ar and n are as in the function lua_getlocal.
        /// </summary>
        /// <code>
        /// const char *lua_setlocal (lua_State *L, const lua_Debug *ar, int n);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_setlocal", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_setlocal(lua_State* L, [NativeTypeName("const lua_Debug *")] lua_Debug* ar, int n);

        /// <summary>
        /// [-0, +(0|1), –]
        /// Gets information about the n-th upvalue of the closure at index funcindex. It pushes the upvalue's value onto the stack and returns its name. Returns NULL (and pushes nothing) when the index n is greater than the number of upvalues.
        /// See debug.getupvalue for more information about upvalues.
        /// </summary>
        /// <code>
        /// const char *lua_getupvalue (lua_State *L, int funcindex, int n);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_getupvalue", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_getupvalue(lua_State* L, int funcindex, int n);

        /// <summary>
        /// [-(0|1), +0, –]
        /// Sets the value of a closure's upvalue. It assigns the value on the top of the stack to the upvalue and returns its name. It also pops the value from the stack.
        /// Returns NULL (and pops nothing) when the index n is greater than the number of upvalues.
        /// Parameters funcindex and n are as in the function lua_getupvalue.
        /// </summary>
        /// <code>
        /// const char *lua_setupvalue (lua_State *L, int funcindex, int n);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_setupvalue", ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        [SuppressGCTransition]
        public static extern byte* _lua_setupvalue(lua_State* L, int funcindex, int n);

        /// <summary>
        /// [-0, +0, –]
        /// Returns a unique identifier for the upvalue numbered n from the closure at index funcindex.
        /// These unique identifiers allow a program to check whether different closures share upvalues. Lua closures that share an upvalue (that is, that access a same external local variable) will return identical ids for those upvalue indices.
        /// Parameters funcindex and n are as in the function lua_getupvalue, but n cannot be greater than the number of upvalues.
        /// </summary>
        /// <code>
        /// void *lua_upvalueid (lua_State *L, int funcindex, int n);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void* lua_upvalueid(lua_State* L, int fidx, int n);

        /// <summary>
        /// [-0, +0, –]
        /// Make the n1-th upvalue of the Lua closure at index funcindex1 refer to the n2-th upvalue of the Lua closure at index funcindex2.
        /// The auxiliary library provides several convenient functions to interface C with Lua. While the basic API provides the primitive functions for all interactions between C and Lua, the auxiliary library provides higher-level functions for some common tasks.
        /// All functions and types from the auxiliary library are defined in header file lauxlib.h and have a prefix luaL_.
        /// All functions in the auxiliary library are built on top of the basic API, and so they provide nothing that cannot be done with that API. Nevertheless, the use of the auxiliary library ensures more consistency to your code.
        /// Several functions in the auxiliary library use internally some extra stack slots. When a function in the auxiliary library uses less than five slots, it does not check the stack size; it simply assumes that there are enough slots.
        /// Several functions in the auxiliary library are used to check C function arguments. Because the error message is formatted for arguments (e.g., "bad argument #1"), you should not use these functions for other stack values.
        /// Functions called luaL_check* always raise an error if the check is not satisfied.
        /// </summary>
        /// <code>
        /// void lua_upvaluejoin (lua_State *L, int funcindex1, int n1,
        /// int funcindex2, int n2);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_upvaluejoin(lua_State* L, int fidx1, int n1, int fidx2, int n2);

        /// <summary>
        /// [-0, +0, –]
        /// Sets the debugging hook function.
        /// Argument f is the hook function. mask specifies on which events the hook will be called: it is formed by a bitwise OR of the constants LUA_MASKCALL, LUA_MASKRET, LUA_MASKLINE, and LUA_MASKCOUNT. The count argument is only meaningful when the mask includes LUA_MASKCOUNT. For each event, the hook is called as explained below:
        /// - The call hook: is called when the interpreter calls a function. The hook is called just after Lua enters the new function.
        /// - The return hook: is called when the interpreter returns from a function. The hook is called just before Lua leaves the function.
        /// - The line hook: is called when the interpreter is about to start the execution of a new line of code, or when it jumps back in the code (even to the same line). This event only happens while Lua is executing a Lua function.
        /// - The count hook: is called after the interpreter executes every count instructions. This event only happens while Lua is executing a Lua function.
        /// Hooks are disabled by setting mask to zero.
        /// </summary>
        /// <code>
        /// void lua_sethook (lua_State *L, lua_Hook f, int mask, int count);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern void lua_sethook(lua_State* L, lua_Hook func, int mask, int count);

        /// <summary>
        /// [-0, +0, –]
        /// Returns the current hook function.
        /// </summary>
        /// <code>
        /// lua_Hook lua_gethook (lua_State *L);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern lua_Hook lua_gethook(lua_State* L);

        /// <summary>
        /// [-0, +0, –]
        /// Returns the current hook mask.
        /// </summary>
        /// <code>
        /// int lua_gethookmask (lua_State *L);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_gethookmask(lua_State* L);

        /// <summary>
        /// [-0, +0, –]
        /// Returns the current hook count.
        /// </summary>
        /// <code>
        /// int lua_gethookcount (lua_State *L);
        /// </code>
        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressGCTransition]
        public static extern int lua_gethookcount(lua_State* L);

        [DllImport("lua548", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int lua_setcstacklimit(lua_State* L, [NativeTypeName("unsigned int")] uint limit);
    }
}
