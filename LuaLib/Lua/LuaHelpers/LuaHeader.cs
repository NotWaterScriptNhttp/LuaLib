using System;
using System.Text;

namespace LuaLib.Lua.LuaHelpers
{
    public class LuaHeader
    {
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
            byte[] Tail = new byte[6]
            {
                0x19,
                0x93,
                0x0D,
                0x0A,
                0x1A,
                0x0A
            };

            void CheckTail()
            {
                for (int i = 0; i < Tail.Length; i++)
                    if (Tail[i] != reader.ReadByte())
                        throw new Exception("The lua tail does not match");
            }


            byte[] ReadSig = reader.ReadBytes(4); // Read the first 4 bytes in the file (aka the luac signature)

            for (int i = 0; i < FileSig.Length; i++)
                if (FileSig[i] != ReadSig[i])
                    throw new Exception("Invalid LuacHeader");

            Version = (LuaVersion)reader.ReadByte();
            Format = reader.ReadByte();
            IsLittleEndian = reader.ReadBoolean();

            if (Version >= LuaVersion.LUA_VERSION_5_3)
                CheckTail();

            reader.SkipBytes(); // Skip the size of int cause it will always be 04

            Is64Bit = reader.ReadByte() == 8 ? true : false;

            reader.SkipBytes(2); // Skip instruction size cause its always uint32 and luaNumber size cause luaNumber is always 08 (double)

            IsIntegral = reader.ReadBoolean();

            if (Version == LuaVersion.LUA_VERSION_5_2)
                CheckTail();
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
