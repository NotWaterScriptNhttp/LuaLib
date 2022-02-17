using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LuaLib.Lua.Emit;
using LuaLib.Lua.LuaHelpers;

namespace LuaLib.Lua.LuaHelpers
{
    internal class LuaWriter
    {
        private class CustomWriter : BinaryWriter
        {
            private bool Is64Bit = true;

            public CustomWriter(Stream output) : base(output) {}

            public void SetArch(bool is64) => Is64Bit = is64;

            public void LWrite(string data)
            {
                if (data == null)
                {
                    if (Is64Bit)
                        Write((long)0);
                    else Write(0);
                    return;
                }

                if (!data.EndsWith("\0"))
                    data += '\0';

                if (Is64Bit)
                    Write((long)data.Length);
                else Write(data.Length);

                Write(Encoding.UTF8.GetBytes(data));
            }
        }

        private MemoryStream writerOutput;
        private CustomWriter writer;
        private bool Is64;
        private bool IsLittle;

        private string ToBase16(int num)
        {
            const string Base = "0123456789ABCDEF";
            string result = "";
            while (num != 0)
            {
                result += Base[num % 16];
                num /= 16;
            }

            if (result == "")
                result = "00";

            if (result.Length == 1)
                result = "0" + result;

            return result + " ";
        }
        private string BytesToString(byte[] bytes)
        {
            string outp = "";

            for (int i = 0; i < bytes.Length; i++)
                outp += ToBase16(bytes[i]);

            return outp;
        }

        //TODO: make this working
        private byte CalculateMaxStackSize(Function func)
        {
            return func.maxstacksize;
        }
        private byte[] DoEndian(byte[] data)
        {
            if (!IsLittle && BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return data;
        }

        private void DumpHeader(Chunk c, bool UseOld, LuaVersion ver)
        {
            // Write the luac file sig
            writer.Write(new byte[4]
            {
                0x1B,
                0x4C,
                0x75,
                0x61
            });

            LuaHeader header = c.Header;

            // Write the lua version the file is using
            if (UseOld)
                writer.Write((byte)header.Version);
            else writer.Write((byte)ver);

            // Write the info how the file should be read/interpreted
            writer.Write(new byte[]
            {
                header.Format,
                (byte)(BitConverter.IsLittleEndian ? 1 : 0),
                4, // sizeof(int)
                (byte)(header.Is64Bit ? 8 : 4),
                4, // Instruction size
                8, // luaNumber size (double)
                (byte)(header.IsIntegral ? 1 : 0)
            });
        }

        private void DumpCode(Function func, WriterOptions options)
        {
            writer.Write(func.InstructionCount);

            for (int i = 0; i < func.InstructionCount; i++)
            {
                writer.Write(func.Instructions[i].GetRawInstruction());
                Console.WriteLine($"[{i}]: {BytesToString(BitConverter.GetBytes(func.Instructions[i].GetRawInstruction()))}");
            }
        }
        private void DumpConstants(Function func, WriterOptions options)
        {
            writer.Write(func.ConstantCount);

            for (int i = 0; i < func.ConstantCount; i++)
            {
                Constant constant = func.Constants[i];

                writer.Write((byte)constant.Type);

                switch (constant.Type)
                {
                    case ConstantType.NIL:
                        break;
                    case ConstantType.BOOLEAN:
                        writer.Write((bool)constant.Value);
                        break;
                    case ConstantType.NUMBER:
                        if (Is64)
                            writer.Write((double)constant.Value);
                        else writer.Write((float)constant.Value);
                        break;
                    case ConstantType.STRING:
                        writer.LWrite(constant.Value);
                        break;
                    default:
                        throw new Exception($"Invalid constant type: '{constant.Type}', value: '{constant.Value}'");
                }
            }

            // i wouldn't count this is constants but lua does (SubFunctions)

            writer.Write(func.FunctionCount);

            for (int i = 0; i < func.FunctionCount; i++)
                DumpFunction(func.Functions[i], options);
        }
        private void DumpDebug(Function func, WriterOptions options)
        {
            // Dumping lineinfo
            if (func.lineinfo == null)
                func.lineinfo = new int[0];

            writer.Write(func.LineinfoSize);

            for (int i = 0; i < func.LineinfoSize; i++)
                writer.Write(func.lineinfo[i]);

            // Dumping locals
            writer.Write(func.LocalCount);

            for (int i = 0; i < func.LocalCount; i++)
            {
                Local l = func.Locals[i];

                writer.LWrite(l.Varname);
                writer.Write(l.StartPC);
                writer.Write(l.EndPC);
            }

            // Dumping upvalues
            writer.Write(func.UpValueCount);

            for (int i = 0; i < func.UpValueCount; i++)
                writer.LWrite(func.UpValues[i].Name);
        }
        private void DumpFunction(Function func, WriterOptions options)
        {
            // Write info about the function/chunk (this should be universal for every version of lua)
            {
                writer.LWrite(func.FuncName);
                writer.Write(func.LineDefined);
                writer.Write(func.LastLineDefined);
                writer.Write(func.nups);
                writer.Write(func.numparams);
                writer.Write(func.is_vararg);

                if (!options.KeepOldMaxStacksize)
                    func.maxstacksize = CalculateMaxStackSize(func);

                writer.Write(func.maxstacksize);
            }

            switch (options.NewLuaVersion)
            {
                case LuaVersion.LUA_VERSION_5_1:
                    DumpCode(func, options); // Dump the code/instructions
                    DumpConstants(func, options); // Dump the constants
                    DumpDebug(func, options); // Dump debug info
                    break;
                default:
                    throw new Exception($"Lua version {options.NewLuaVersion} is currently not supported");
            }
        }

        internal LuaWriter(Chunk chunk, WriterOptions options)
        {
            #region Setting up variables
            writerOutput = new MemoryStream();
            writer = new CustomWriter(writerOutput);

            Is64 = chunk.Header.Is64Bit;
            IsLittle = chunk.Header.IsLittleEndian;

            writer.SetArch(Is64);

            if (options.NewLuaVersion == LuaVersion.LUA_VERSION_UNKNOWN)
                options.NewLuaVersion = chunk.Header.Version;
            #endregion

            DumpHeader(chunk, options.KeepLuaVersion, options.NewLuaVersion); // Dump the header for the new file

            #region Name Checking
            // Adding the @ to the start cause lua expects it (i think)

            string name = chunk.MainFunction.FuncName;

            if (string.IsNullOrEmpty(name))
                name = "@Unnamed.lua";
            if (!name.StartsWith("@"))
                name = "@" + name;

            chunk.MainFunction.FuncName = name;
            #endregion

            DumpFunction(chunk.MainFunction, options); // Write the Chunk + all the other things stored in the chunk
        }

        public void Write(string outfile)
        {
            File.WriteAllBytes(outfile, writerOutput.ToArray());
        }
    }
}
