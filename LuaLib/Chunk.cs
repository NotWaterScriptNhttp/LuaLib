using System;
using System.IO;

using LuaLib.Emit;
using LuaLib.LuaHelpers;

using LuaLib.LuaHelpers.Versions.LuaReader;

namespace LuaLib
{
    public struct ChunkSettings
    {
        public bool IsLuau;
        public bool IgnoreReadError;
        public bool AddRefs = true;
        public bool AutoFunctionNames = true; // Gets the function name automaticaly
    }

    public class Chunk
    {
        public LuaHeader Header { get; private set; }
        public Function MainFunction { get; private set; }

        private Chunk() {}

        public static Chunk Load(string file, ChunkSettings settings = default)
        {
            if (!File.Exists(file))
                throw new Exception("File does not exist!");

            Chunk chunk = new Chunk();

            CustomBinaryReader br = new CustomBinaryReader(new MemoryStream(File.ReadAllBytes(file)));
            
            LuaHeader header = null;

            //NOTE: Luau does not have a header
            if (settings.IsLuau)
            {
                header = new LuaHeader(LuaVersion.LUA_VERSION_U);
                byte ver = br.ReadByte();

                if (ver == 0)
                    throw new ApplicationException("Some error happened durring the compile time");

                if (ver != 2)
                    throw new ApplicationException($"Bytecode version mismatch (expected: 2, got: {ver})");

                header.VersionNumber = ver;
            }
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
                    lr = new LuaReader53(br); // looks like that luau ver 2 has the same data storage
                    break;
                default:
                    throw new Exception($"No reader for {header.Version}");
            }

            if (!settings.IsLuau)
                if (header.Version >= LuaVersion.LUA_VERSION_5_3)
                    lr.ReadByte(); // UpvalueCount 

            lr.UpdateEndian(header.IsLittleEndian);
            lr.UpdateArch(header.Is64Bit);

            chunk.MainFunction = Function.GetFunction(lr, header.Version, settings);
            chunk.MainFunction.IsMainChunk = true;

            #region Some final non important things

            for (int i = 0; i < chunk.MainFunction.FunctionCount; i++)
                chunk.MainFunction.Functions[i].IsMainChunkChild = true;

            void Addrefs(Function func)
            {
                if (func.FunctionCount == 0)
                    return;

                foreach (Instruction inst in func.Instructions)
                    if (inst.Opcode == OpCodes.CLOSURE)
                    {
                        Function func2 = func.Functions[inst.Bx];

                        func2.DefingInstruction = inst;
                        func2.Parent = func;

                        Addrefs(func2);
                    }
            }
            void GetFuncName(Function func)
            {
                func.TryGetName();

                func.Functions.ForEach(GetFuncName);
            }

            if (settings.AddRefs)
                Addrefs(chunk.MainFunction);
            if (settings.AutoFunctionNames) // Doesn't work (needs some debuging on why it doesn't work)
                GetFuncName(chunk.MainFunction);

            #endregion

            if (!lr.IsFullyRead() && !settings.IgnoreReadError)
                throw new Exception($"The file {Path.GetFileName(file)} was not fully read!");

            return chunk;
        }

        public bool Write(string out_file, WriterOptions options = default)
        {
            if (options == null)
                options = new WriterOptions();

            LuaWriter.GetWriter(this, options).Write(out_file);

            return true;
        }
        public static bool Write(Chunk chunk, string file, WriterOptions options = null) => chunk.Write(file, options);
        public static bool Write(Function func, string file, LuaVersion targetVer, WriterOptions options = null)
        {
            if (options == null)
                options = new WriterOptions();

            options.Header = new LuaHeader(targetVer);

            LuaWriter.GetWriter(targetVer, func, options).Write(file);

            return true;
        }    
    }
}
