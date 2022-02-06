using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaLib.Lua.LuaHelpers
{
    public class LuaHeader
    {
        public enum LuaVersion
        {
            LUA_VERSION_5_1 = 0x51,
            LUA_VERSION_5_2 = 0x52,
            LUA_VERSION_5_3 = 0x53,
            LUA_VERSION_5_4 = 0x54,
        }

        public LuaVersion Version { get; private set; }
        public byte Format { get; private set; }
        public bool IsLittleEndian { get; private set; }
        public bool Is64Bit { get; private set; }
        public bool IsIntegral { get; private set; }

        internal LuaHeader(LuaReader reader)
        {
            byte[] FileSig = new byte[4]
            {
                0x1B, // 
                0x4C, // L
                0x75, // u
                0x61  // a
            }; // The character on the start of the file (never changing!)
            byte[] ReadSig = reader.ReadBytes(4); // Read the first 4 bytes in the file (aka the luac signature)

            for (int i = 0; i < FileSig.Length; i++)
                if (FileSig[i] != ReadSig[i])
                    throw new Exception("Invalid LuacHeader");

            Version = (LuaVersion)reader.ReadByte();
            Format = reader.ReadByte();
            IsLittleEndian = reader.ReadBoolean();

            reader.SkipBytes(); // Skip the size of int cause it will always be 04

            Is64Bit = reader.ReadByte() == 8 ? true : false;

            reader.SkipBytes(2); // Skip instruction size cause its always uint32 and luaNumber size cause luaNumber is always 08 (double)

            IsIntegral = reader.ReadBoolean();
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
