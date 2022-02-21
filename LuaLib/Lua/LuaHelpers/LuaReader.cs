using System;
using System.IO;

namespace LuaLib.Lua.LuaHelpers
{
    internal abstract class LuaReader
    {
        protected CustomBinaryReader reader;
        protected bool LittleEndian = true;
        protected bool Is64Arch = true;

        internal LuaReader(CustomBinaryReader br) => reader = br;
        internal LuaReader(MemoryStream ms) => reader = new CustomBinaryReader(ms);
        internal LuaReader(string file)
        {
            if (!File.Exists(file))
                throw new Exception("File doesn't exits!"); // another check for file

            reader = new CustomBinaryReader(new MemoryStream(File.ReadAllBytes(file)));
        }

        public long GetPosition() => reader.BaseStream.Position;
        public long GetLength() => reader.BaseStream.Length;
        public bool IsFullyRead() => reader.BaseStream.Position == reader.BaseStream.Length;

        public void UpdateEndian(bool IsLittle) => LittleEndian = IsLittle;
        public void UpdateArch(bool Is64) => Is64Arch = Is64; // the luac file can be compiled on a 32 bit compiler even though i highly doubt

        public void SkipBytes(int count = 1) => reader.ReadBytes(count);

        public byte ReadByte() => reader.ReadByte();
        public byte[] ReadBytes(int count)
        {
            byte[] bytes = reader.ReadBytes(count);

            if (!LittleEndian && BitConverter.IsLittleEndian) // Flip the bytes cause reading the big endian number on a little endian would create a very big number
                Array.Reverse(bytes);

            return bytes;
        }

        //Naming this a number cause lua
        virtual public int ReadNumber32() => reader.ReadInt32();
        public uint ReadUNumber32() => reader.ReadUInt32();

        public long ReadNumber64() => reader.ReadInt64();
        public ulong ReadUNumber64() => reader.ReadUInt64();

        // This function is stollen straight from source https://www.lua.org/source/5.4/lundump.c.html#loadUnsigned
        protected ulong ReadUnsigned(ulong limit)
        {
            ulong x = 0;
            int b;

            limit >>= 7;

            do
            {
                b = ReadByte();

                if (x >= limit)
                    throw new Exception("X can't be bigger than limit");

                x = (x << 7) | (ulong)(b & 0x7f);
            } while ((b & 0x80) == 0);

            return x;
        }
        virtual public ulong ReadSize()
        {
            return ReadUnsigned(~(ulong)0);
        }

        public double ReadFloat()
        {
            if (Is64Arch)
                return reader.ReadDouble();
            else return reader.ReadSingle();
        }

        public bool ReadBoolean() => reader.ReadBoolean();
        public abstract string ReadString();
    }
}
