using System;
using System.IO;
using System.Linq;

namespace LuaLib.Lua.LuaHelpers
{
    internal class LuaReader
    {
        private BinaryReader reader;
        private bool LittleEndian = true;
        private bool Is64Arch = true;

        internal LuaReader(MemoryStream ms) => reader = new BinaryReader(ms);
        internal LuaReader(string file)
        {
            if (!File.Exists(file))
                throw new Exception("File doesn't exits!"); // another check for file

            reader = new BinaryReader(new MemoryStream(File.ReadAllBytes(file)));
        }

        public void UpdateEndian(bool IsLittle) => LittleEndian = IsLittle;
        public void UpdateArch(bool Is64) => Is64Arch = Is64; // the luac file can be compiled on a 32 bit compiler even though i highly doubt

        public void SkipBytes(int count = 1)
        {
            reader.ReadBytes(count);
        }

        public byte ReadByte()
        {
            return reader.ReadByte();
        }
        public byte[] ReadBytes(int count)
        {
            byte[] bytes = reader.ReadBytes(count);

            if (!LittleEndian && BitConverter.IsLittleEndian) // Flip the bytes cause reading the big endian number on a little endian would create a very big number
                Array.Reverse(bytes);

            return bytes;
        }

        //Naming this a number cause lua
        public int ReadNumber32()
        {
            return BitConverter.ToInt32(ReadBytes(4), 0);
        }
        public uint ReadUNumber32()
        {
            return BitConverter.ToUInt32(ReadBytes(4), 0);
        }

        public long ReadNumber64()
        {
            return BitConverter.ToInt64(ReadBytes(8), 0);
        }
        public ulong ReadUNumber64()
        {
            return BitConverter.ToUInt64(ReadBytes(8), 0);
        }

        public double ReadFloat()
        {
            if (Is64Arch)
                return BitConverter.ToDouble(ReadBytes(8), 0);
            else return BitConverter.ToSingle(ReadBytes(4), 0);
        }

        public bool ReadBoolean()
        {
            return ReadByte() == 0 ? false : true;
        }
        public string ReadString()
        {
            int strLen;

            if (Is64Arch)
                strLen = (int)ReadNumber64();
            else strLen = ReadNumber32();

            if (strLen == 0)
                return "";

            string result = "";

            ReadBytes(strLen).ToList().ForEach(b => result += (char)b);

            return result.Replace("\0", "");
        }
    }
}
