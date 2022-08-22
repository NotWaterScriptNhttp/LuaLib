using LuaLib.Emit;
using LuaLib.LuaHelpers.Versions.Decompiler;

namespace LuaLib
{
    public class Decompiler
    {
        private string decomp(Function func, LuaVersion ver)
        {
            switch (ver)
            {
                case LuaVersion.LUA_VERSION_5_1:
                    return new Decompiler51().Decompile(func);
                default:
                    return $"No decompiler for {ver}";
            }
        }

        private Decompiler() {}

        public static string Decompile(Function func, LuaVersion ver) => new Decompiler().decomp(func, ver);
        public static string Decompile(Chunk c) => Decompile(c.MainFunction, c.Header.Version);
        public static string Decompile(string file) => Decompile(Chunk.Load(file));
    }
}
