using System;
using System.IO;

namespace LuaLib.LuaHelpers.Versions.LuaHeader
{
    internal class LuaHeader52 : LuaHelpers.LuaHeader
    {
        internal LuaHeader52(CustomBinaryReader br)
        {
            IsLittleEndian = br.ReadBoolean();

            br.ReadByte(); // Skip the size of int cause it will always be 4

            // Get what platform it was compiled on cause size_t will be 32 if its 32 bit compiler and 64 bit if its on a 64 bit compiler
            Is64Bit = br.ReadByte() == 8 ? true : false;

            br.ReadBytes(2); // Skip instruction and LuaNumber sizes (instr: 4 bytes, LuaNumber: 8 bytes)

            IsIntegral = br.ReadBoolean();

            CheckTail(br);
        }
    }
}
