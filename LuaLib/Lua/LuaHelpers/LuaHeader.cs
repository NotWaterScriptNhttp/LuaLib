using System;
using System.IO;
using System.Text;

using LuaLib.Lua.LuaHelpers.Versions.LuaHeader;

namespace LuaLib.Lua.LuaHelpers
{
    public class LuaHeader
    {
        internal void CheckTail(CustomBinaryReader br)
        {
            byte[] LuaTail = new byte[6]
            {
                0x19,
                0x93,
                0x0D,
                0x0A,
                0x1A,
                0x0A
            };
            for (int i = 0; i < LuaTail.Length; i++)
            {
                if (br.ReadByte() != LuaTail[i])
                    throw new Exception("The lua tail does not match");
            }
        }

        protected const int LUA_INT = 0x5678;
        protected const double LUA_NUM = 370.5;

        public LuaVersion Version { get; protected set; }
        public byte Format { get; protected set; }
        public bool IsLittleEndian { get; protected set; } = true;
        public bool Is64Bit { get; protected set; }
        public bool IsIntegral { get; protected set; } = false;

        internal LuaHeader() {}

        internal static LuaHeader GetHeader(CustomBinaryReader br)
        {
            byte[] FileSig = new byte[4]
            {
                0x1B, // 
                0x4C, // L
                0x75, // u
                0x61  // a
            }; // The character on the start of the file (never changing!)

            byte[] ReadSig = br.ReadBytes(4); // Read the first 4 bytes in the file (aka the luac signature)

            for (int i = 0; i < FileSig.Length; i++)
                if (FileSig[i] != ReadSig[i])
                    throw new Exception("Invalid LuacHeader");

            LuaVersion version = (LuaVersion)br.ReadByte();
            byte format = br.ReadByte();

            LuaHeader header = null;

            switch (version)
            {
                case LuaVersion.LUA_VERSION_5_1:
                    header = new LuaHeader51(br);
                    break;
                case LuaVersion.LUA_VERSION_5_2:
                    header = new LuaHeader52(br);
                    break;
                case LuaVersion.LUA_VERSION_5_3:
                    header = new LuaHeader53(br);
                    break;
                case LuaVersion.LUA_VERSION_5_4:
                    header = new LuaHeader54(br);
                    break;
                default:
                    throw new Exception($"Cannot create a header for ({version})");
            }

            header.Version = version;
            header.Format = format;

            return header;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("LuaHeader: {\n");
            builder.Append($" Version: {Version},\n");
            builder.Append($" Format: {Format},\n");
            builder.Append($" LittleEndian: {IsLittleEndian},\n");
            builder.Append($" 64Bit: {Is64Bit},\n");
            builder.Append($" Integral: {IsIntegral}\n");
            builder.Append("} - LuaHeader");

            return builder.ToString();
        }
    }
}
