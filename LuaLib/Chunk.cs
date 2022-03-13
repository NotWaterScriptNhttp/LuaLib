using System;
using System.IO;

using LuaLib.Emit;
using LuaLib.LuaHelpers;

using LuaLib.LuaHelpers.Versions.LuaReader;

namespace LuaLib
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

            LuaWriter.GetWriter(this, options).Write(out_file);

            return true;
        }

        public static Chunk Load(string file, bool IsLuau = false, bool ignoreReadError = false)
        {
            if (!File.Exists(file))
                throw new Exception("File does not exist!");

            Chunk chunk = new Chunk();

            CustomBinaryReader br = new CustomBinaryReader(new MemoryStream(File.ReadAllBytes(file)));

            //NOTE: Luau does not have a header
            LuaHeader header = null;

            if (IsLuau)
                header = new LuaHeader(LuaVersion.LUA_VERSION_U);
            else header = LuaHeader.GetHeader(br);

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
                case LuaVersion.LUA_VERSION_5_4:
                    lr = new LuaReader54(br);
                    break;
                case LuaVersion.LUA_VERSION_U:
                    break;
                default:
                    throw new Exception($"No reader for {header.Version}");
            }

            if (header.Version >= LuaVersion.LUA_VERSION_5_3)
                lr.ReadByte(); // UpvalueCount 

            lr.UpdateEndian(header.IsLittleEndian);
            lr.UpdateArch(header.Is64Bit);

            chunk.MainFunction = Function.GetFunction(lr, header.Version);
            chunk.MainFunction.IsMainChunk = true;

            for (int i = 0; i < chunk.MainFunction.FunctionCount; i++)
                chunk.MainFunction.Functions[i].IsMainChunkChild = true;

            if (!lr.IsFullyRead() && !ignoreReadError)
                throw new Exception($"The file {Path.GetFileName(file)} was not fully read!");

            return chunk;
        }

        public static bool Write(Chunk chunk, string file, WriterOptions options = null) => chunk.Write(file, options);
    }
}
