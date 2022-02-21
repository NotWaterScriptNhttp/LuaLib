using System;
using System.IO;
using System.ComponentModel;

using LuaLib.Lua.Emit;
using LuaLib.Lua.LuaHelpers;

using LuaLib.Lua.LuaHelpers.Versions.LuaReader;

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

        public static Chunk Load(string file, bool ignoreReadError = false)
        {
            if (!File.Exists(file))
                throw new Exception("File does not exist!");

            Chunk chunk = new Chunk();

            BinaryReader br = new BinaryReader(new MemoryStream(File.ReadAllBytes(file)));

            LuaHeader header = LuaHeader.GetHeader(br);
            chunk.Header = header;

            LuaReader lr = null;

            switch (header.Version)
            {
                case LuaVersion.LUA_VERSION_5_1:
                    lr = new LuaReader51(br);
                    break;
                case LuaVersion.LUA_VERSION_5_2:
                    lr = new LuaReader52(br);
                    break;
                case LuaVersion.LUA_VERSION_5_3:
                    lr = new LuaReader53(br);
                    break;
                default:
                    throw new Exception($"No reader for {header.Version}");
            }

            if (header.Version >= LuaVersion.LUA_VERSION_5_3)
                lr.ReadByte(); // UpvalueCount 

            lr.UpdateEndian(header.IsLittleEndian);
            lr.UpdateArch(header.Is64Bit);

            chunk.MainFunction = Function.GetFunction(lr, header.Version);

            if (!lr.IsFullyRead() && !ignoreReadError)
                throw new Exception($"The file {Path.GetFileName(file)} was not fully read!");

            return chunk;
        }

        public static bool Write(Chunk chunk, string file, WriterOptions options = null) => chunk.Write(file, options);
    }
}
