namespace KeraLua
{
    /// <summary>
    /// Enum for pseudo-index used by registry table
    /// </summary>
#pragma warning disable CA1008 // Enums should have zero value
    public enum LuaRegistry
#pragma warning restore CA1008 // Enums should have zero value
    {
        /* LUAI_MAXSTACK		1000000 */
        /// <summary>
        /// pseudo-index used by registry table
        /// </summary>
#if LUA_5_5_OR_LATER
        Index = (-(int.MaxValue/2 + 1000))
#else
        Index = -1000000 - 1000
#endif
    }

    /// <summary>
    /// Registry index 
    /// </summary>
#pragma warning disable CA1008 // Enums should have zero value
    public enum LuaRegistryIndex
#pragma warning restore CA1008 // Enums should have zero value
    {
        /// <summary>
        ///  At this index the registry has the main thread of the state.
        /// </summary>
        MainThread = 1,
        /// <summary>
        /// At this index the registry has the global environment. 
        /// </summary>
        Globals = 2,
    }
}
