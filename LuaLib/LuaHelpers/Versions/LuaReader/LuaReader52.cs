using System.IO;
using System.Linq;

namespace LuaLib.LuaHelpers.Versions.LuaReader
{
    internal class LuaReader52 : LuaHelpers.LuaReader
    {
        public LuaReader52(CustomBinaryReader br) : base(br) {}
        public LuaReader52(MemoryStream ms) : base(ms) {}
        public LuaReader52(string file) : base(file) {}

        public override string ReadString()
        {
            int strLen;

            if (Is64Arch)
                strLen = (int)ReadNumber64();
            else strLen = ReadNumber32();

            if (strLen == 0)
                return null;

            string result = "";

            ReadBytes(strLen).ToList().ForEach(b => result += (char)b);

            return result.Replace("\0", "");
        }
    }
}
