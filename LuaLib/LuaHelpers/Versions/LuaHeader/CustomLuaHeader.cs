using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaLib.LuaHelpers.Versions.LuaHeader
{
    internal class CustomLuaHeader : LuaHelpers.LuaHeader
    {
        public CustomLuaHeader(LuaVersion version, byte format = 0, bool little = true, bool is64 = true, bool isInt = false)
        {
            Version = version;
            Format = format;
            IsLittleEndian = little;
            Is64Bit = is64;
            IsIntegral = isInt;
        }

        public CustomLuaHeader SetVersion(LuaVersion version)
        {
            Version = version;

            return this;
        }

        public CustomLuaHeader SetFormat(byte fmt)
        {
            Format = fmt;

            return this;
        }
        public CustomLuaHeader SetEndian(bool IsLittle)
        {
            IsLittleEndian = IsLittle;

            return this;
        }
        public CustomLuaHeader SetPlatform(bool is64)
        {
            Is64Bit = is64;

            return this;
        }
        public CustomLuaHeader SetIntegral(bool isIntegral)
        {
            IsIntegral = IsIntegral;

            return this;
        }
    }
}
