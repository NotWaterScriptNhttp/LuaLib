﻿namespace LuaLib.Lua
{
    public class WriterOptions
    {
        public bool KeepOldMaxStacksize = true;
        public bool KeepLuaVersion = true;
        public LuaVersion NewLuaVersion = LuaVersion.LUA_VERSION_UNKNOWN;
    }
}
