using System;
using System.IO;

using LuaLib.Emit;
using LuaLib.LuaHelpers.Versions;
using LuaLib.LuaHelpers.Versions.LuaWriter;

namespace LuaLib.LuaHelpers
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

        //TODO: make this a real max stack size calculator
        protected byte CalculateMaxStackSize(Function func)
        {
            // Temporary fix for this thing

            byte biggestA = 0;

            foreach (Instruction instr in func.Instructions)
                if (instr.A > biggestA)
                    biggestA = (byte)instr.A;

            return (byte)(biggestA + 1); // even bigger bypass would be returning 255
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
        protected void DumpInt32(int i32) => writer.Write(DoEndian(BitConverter.GetBytes(i32))); // Copy of the above cause Lua5.4 overrides it
        protected void DumpInt64(long i64) => writer.Write(DoEndian(BitConverter.GetBytes(i64)));
        protected void DumpBool(bool b) => writer.Write(b);

        internal abstract void DumpCode(Function func, WriterOptions options);
        internal abstract void DumpConstants(Function func, WriterOptions options);
        internal abstract void DumpUpValues(Function func, WriterOptions options);
        internal abstract void DumpDebug(Function func, WriterOptions options);
        internal abstract void DumpFunction(Function func, WriterOptions options);

        internal abstract void DumpHeader(LuaHeader header);
        internal void DumpHeader(Chunk chunk) => DumpHeader(chunk.Header);

        internal abstract void DumpString(string str);

        internal LuaWriter()
        {
            #region Setting up variables
            writerOutput = new MemoryStream();
            writer = new BinaryWriter(writerOutput);
            #endregion
        }

        public byte[] GetWritenBytes() => writerOutput.ToArray();
        public void Write(string outfile) => File.WriteAllBytes(outfile, GetWritenBytes());

        public static LuaWriter GetWriter(LuaVersion version, Function func, WriterOptions options)
        {
            if (options == null)
                options = new WriterOptions();

            LuaWriter writer = null;

            #region Creating the right writer
            LuaVersion verToUse;

            if (options.KeepLuaVersion)
                verToUse = version;
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
                case LuaVersion.LUA_VERSION_5_4:
                    writer = new LuaWriter54();
                    break;
                default:
                    throw new Exception($"Didn't find any writer for ({verToUse})");
            }

            writer.IsLittle = options.IsChunk ? options.Header.IsLittleEndian : true;
            writer.Is64Bit = options.IsChunk ? options.Header.Is64Bit : true;
            #endregion

            writer.DumpHeader(options.IsChunk ? options.Header : new Versions.LuaHeader.CustomLuaHeader(verToUse)); // Dump the header for the new file

            #region Name Checking
            // Adding the @ to the start cause lua expects it (i think)

            string name = func.FuncName;

            if (string.IsNullOrEmpty(name))
                name = "@Unnamed.lua";
            if (!name.StartsWith("@"))
                name = "@" + name;

            func.FuncName = name;
            #endregion

            writer.DumpFunction(func, options); // Write the Chunk + all the other things stored in the chunk

            return writer;
        }
        public static LuaWriter GetWriter(Chunk chunk, WriterOptions options)
        {
            WriterOptions opts = options; // doing this cause we don't want to set the real options vars
            opts.IsChunk = true;
            opts.Header = chunk.Header;

            return GetWriter(chunk.Header.Version, chunk.MainFunction, opts);
        }
    }
}
