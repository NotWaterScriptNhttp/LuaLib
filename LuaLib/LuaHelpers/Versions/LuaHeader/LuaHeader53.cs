using System;
using System.IO;

namespace LuaLib.LuaHelpers.Versions.LuaHeader
{
    internal class LuaHeader53 : LuaHelpers.LuaHeader
    {
        internal LuaHeader53(CustomBinaryReader br)
        {
            VersionNumber = 5.3f;
            CheckTail(br);

            br.ReadByte(); // Skip the size of int cause it will always be 4

            // Get what platform it was compiled on cause size_t will be 32 if its 32 bit compiler and 64 bit if its on a 64 bit compiler
            Is64Bit = br.ReadByte() == 8 ? true : false;

            br.ReadBytes(3); // Skip instruction, int and LuaNumber sizes (instr: 4 bytes, int: 4 bytes, LuaNumber: 8 bytes)

            if (br.ReadInt64() != LUA_INT)
                throw new Exception("Header int does not match");

            if (br.ReadDouble() != LUA_NUM)
                throw new Exception("Header number does not match");
        }
    }
}
