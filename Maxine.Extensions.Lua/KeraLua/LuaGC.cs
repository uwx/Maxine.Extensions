using System;
using System.Collections.Generic;
using System.Text;

namespace KeraLua
{
    /// <summary>
    /// Garbage Collector operations
    /// </summary>
    public enum LuaGC
    {
        /// <summary>
        ///  Stops the garbage collector. 
        /// </summary>
        Stop = 0,
        /// <summary>
        /// Restarts the garbage collector. 
        /// </summary>
        Restart = 1,
        /// <summary>
        /// Performs a full garbage-collection cycle. 
        /// </summary>
        Collect = 2,
        /// <summary>
        ///  Returns the current amount of memory (in Kbytes) in use by Lua. 
        /// </summary>
        Count = 3,
        /// <summary>
        ///  Returns the remainder of dividing the current amount of bytes of memory in use by Lua by 1024
        /// </summary>
        Countb = 4,
        /// <summary>
        ///  Performs an incremental step of garbage collection. 
        /// </summary>
        Step = 5,
#if !LUA_5_5_OR_LATER
        /// <summary>
        /// The options LUA_GCSETPAUSE and LUA_GCSETSTEPMUL of the function lua_gc are deprecated. You should use the new option LUA_GCINC to set them. 
        /// </summary>
        [Obsolete("Deprecatad since Lua 5.4, Use Incremental instead")]
        SetPause = 6,
        /// <summary>
        /// The options LUA_GCSETPAUSE and LUA_GCSETSTEPMUL of the function lua_gc are deprecated. You should use the new option LUA_GCINC to set them. 
        /// </summary>
        [Obsolete("Deprecatad since Lua 5.4, Use Incremental instead")]
        SetStepMultiplier = 7,
        /// <summary>
        ///  returns a boolean that tells whether the collector is running
        /// </summary>
        IsRunning = 9,
        /// <summary>
        ///  Changes the collector to generational mode with the given parameters (see §2.5.2). Returns the previous mode (LUA_GCGEN or LUA_GCINC). 
        /// </summary>
        Generational = 10,
        /// <summary>
        /// Changes the collector to incremental mode with the given parameters (see §2.5.1). Returns the previous mode (LUA_GCGEN or LUA_GCINC). 
        /// </summary>
        Incremental = 11,
#else
        // #define LUA_GCSTOP		0
        // #define LUA_GCRESTART		1
        // #define LUA_GCCOLLECT		2
        // #define LUA_GCCOUNT		3
        // #define LUA_GCCOUNTB		4
        // #define LUA_GCSTEP		5
        // #define LUA_GCISRUNNING		6
        // #define LUA_GCGEN		7
        // #define LUA_GCINC		8
        // #define LUA_GCPARAM		9

        /// <summary>
        ///  returns a boolean that tells whether the collector is running
        /// </summary>
        IsRunning = 6,
         /// <summary>
        ///  Changes the collector to generational mode with the given parameters (see §2.5.2). Returns the previous mode (LUA_GCGEN or LUA_GCINC). 
        /// </summary>
        Generational = 7,
        /// <summary>
        /// Changes the collector to incremental mode with the given parameters (see §2.5.1). Returns the previous mode (LUA_GCGEN or LUA_GCINC). 
        /// </summary>
        Incremental = 8,
        GcParam = 9
#endif
    }
}
