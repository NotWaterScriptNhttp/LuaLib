using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LuaLib.Lua.Emit;
using LuaLib.Lua.LuaHelpers.Versions.LuaWriter;

namespace LuaLib.Lua.LuaHelpers
{
    internal abstract class LuaWriter
    {
        private MemoryStream writerOutput;
        protected bool Is64Bit;
        protected bool IsLittle;
        protected BinaryWriter writer;

        protected readonly byte[] LuaSig = new byte[4]
        {
            0x1B,
            0x4C,
            0x75,
            0x61
        };
        protected readonly byte[] LuaTail = new byte[6]
        {
            0x19,
            0x93,
            0x0D,
            0x0A,
            0x1A,
            0x0A
        };
        protected readonly int LuaInt = 0x5678;
        protected readonly double LuaNum = 370.5d;

        //TODO: make this working
        protected byte CalculateMaxStackSize(Function func)
        {
            return func.maxstacksize;
        }
        protected byte[] DoEndian(byte[] data)
        {
            if (!IsLittle && BitConverter.IsLittleEndian)
                Array.Reverse(data);
            else if (IsLittle && !BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return data;
        }

        virtual protected void DumpSize(ulong size)
        {
            int dibs = ((Is64Bit ? 8 : 4) * 8 / 7) + 1;

            byte[] buff = new byte[dibs];
            int n = 0;

            do
            {
                buff[dibs - (++n)] = (byte)(size & 0x7f);
                size >>= 7;
            } while (size != 0);
            buff[dibs - 1] |= 0x80;

            for (int i = 0;i < n; i++)
                writer.Write(buff[i + dibs - n]);
        }
        virtual protected void DumpInt(int i32) => writer.Write(DoEndian(BitConverter.GetBytes(i32)));
        protected void DumpInt64(long i64) => writer.Write(DoEndian(BitConverter.GetBytes(i64)));
        protected void DumpBool(bool b) => writer.Write(b);

        internal abstract void DumpCode(Function func, WriterOptions options);
        internal abstract void DumpConstants(Function func, WriterOptions options);
        internal abstract void DumpUpValues(Function func, WriterOptions options);
        internal abstract void DumpDebug(Function func, WriterOptions options);
        internal abstract void DumpFunction(Function func, WriterOptions options);

        internal abstract void DumpHeader(Chunk chunk);

        internal abstract void DumpString(string str);

        internal LuaWriter()
        {
            #region Setting up variables
            writerOutput = new MemoryStream();
            writer = new BinaryWriter(writerOutput);
            #endregion
        }

        public void Write(string outfile)
        {
            File.WriteAllBytes(outfile, writerOutput.ToArray());
        }

        public static LuaWriter GetWriter(Chunk chunk, WriterOptions options)
        {
            LuaWriter writer = null;

            #region Creating the right writer
            LuaVersion verToUse;

            if (options.KeepLuaVersion)
                verToUse = chunk.Header.Version;
            else verToUse = options.NewLuaVersion;

            switch (verToUse)
            {
                case LuaVersion.LUA_VERSION_5_1:
                    writer = new LuaWriter51();
                    break;
                case LuaVersion.LUA_VERSION_5_2:
                    writer = new LuaWriter52();
                    break;
                case LuaVersion.LUA_VERSION_5_3:
                    writer = new LuaWriter53();
                    break;
                default:
                    throw new Exception($"Didn't find any writer for ({verToUse})");
            }

            writer.IsLittle = chunk.Header.IsLittleEndian;
            writer.Is64Bit = chunk.Header.Is64Bit;
            #endregion

            writer.DumpHeader(chunk); // Dump the header for the new file

            #region Name Checking
            // Adding the @ to the start cause lua expects it (i think)

            string name = chunk.MainFunction.FuncName;

            if (string.IsNullOrEmpty(name))
                name = "@Unnamed.lua";
            if (!name.StartsWith("@"))
                name = "@" + name;

            chunk.MainFunction.FuncName = name;
            #endregion

            writer.DumpFunction(chunk.MainFunction, options); // Write the Chunk + all the other things stored in the chunk

            return writer;
        }
    }
}
