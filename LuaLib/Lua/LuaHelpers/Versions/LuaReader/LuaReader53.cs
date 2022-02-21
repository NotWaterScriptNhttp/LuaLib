using System.IO;
using System.Linq;

namespace LuaLib.Lua.LuaHelpers.Versions.LuaReader
{
    internal class LuaReader53 : LuaHelpers.LuaReader
    {
        public LuaReader53(CustomBinaryReader br) : base(br) {}
        public LuaReader53(MemoryStream ms) : base(ms) {}
        public LuaReader53(string file) : base(file) {}

        public override string ReadString()
        {
            byte bSize = ReadByte();

            if (bSize == 0)
                return "";

            int iSize = bSize;

            if (bSize == byte.MaxValue)
                iSize = ReadNumber32();

            string str = "";
            ReadBytes(iSize - 1).ToList().ForEach(b => str += (char)b);

            return str;
        }
    }
}
