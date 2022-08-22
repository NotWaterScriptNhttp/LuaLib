using LuaLib.LuaHelpers;

namespace LuaLib
{
    public class WriterOptions
    {
        public bool KeepOldMaxStacksize = true;
        public bool KeepLuaVersion = true;
        public LuaVersion NewLuaVersion = LuaVersion.LUA_VERSION_UNKNOWN;

        internal bool IsChunk;
        internal LuaHeader Header;
    }
}
