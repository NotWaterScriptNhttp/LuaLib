using System;
using System.IO;

namespace LuaLib.LuaHelpers.Versions.LuaHeader
{
    internal class LuaHeader54 : LuaHelpers.LuaHeader
    {
        internal LuaHeader54(CustomBinaryReader br)
        {
            VersionNumber = 5.4f;
            CheckTail(br);

            br.ReadBytes(2); // Skip instruction and LuaInt (instr: 4 bytes, LuaInt: 4 or 8 bytes)

            // Get what platform it was compiled on cause LuaNumber will be 32 if its 32 bit compiler and 64 bit if its on a 64 bit compiler
            Is64Bit = br.ReadByte() == 8 ? true : false;

            if (br.ReadInt64() != LUA_INT)
                throw new Exception("Header int does not match");

            if (br.ReadDouble() != LUA_NUM)
                throw new Exception("Header number does not match");
        }
    }
}
