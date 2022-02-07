using System;
using System.IO;

using LuaLib.Lua.Emit;
using LuaLib.Lua.LuaHelpers;

namespace LuaLib.Lua
{
    public class Chunk
    {
        public LuaHeader Header { get; private set; }
        public Function MainFunction { get; private set; }

        private Chunk() {}

        public bool Write(string out_file, WriterOptions options = null)
        {
            if (options == null)
                options = new WriterOptions();

            new LuaWriter(this, options).Write(out_file);

            return true;
        }

        public static Chunk Load(string file)
        {
            if (!File.Exists(file))
                throw new Exception("File does not exist!");

            LuaReader lr = new LuaReader(file);

            Chunk chunk = new Chunk();

            chunk.Header = new LuaHeader(lr);

            if (chunk.Header.Version != LuaVersion.LUA_VERSION_5_1)
                throw new Exception($"Only 5.1 is supported at this moment");

            lr.UpdateEndian(chunk.Header.IsLittleEndian);
            lr.UpdateArch(chunk.Header.Is64Bit);

            chunk.MainFunction = Function.GetFunction(lr);

            return chunk;
        }

        public static bool Write(Chunk chunk, string file, WriterOptions options = null) => chunk.Write(file, options);
    }
}
