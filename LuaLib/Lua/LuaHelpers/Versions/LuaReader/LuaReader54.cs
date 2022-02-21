using System.IO;
using System.Linq;

namespace LuaLib.Lua.LuaHelpers.Versions.LuaReader
{
    internal class LuaReader54 : LuaHelpers.LuaReader
    {
        public LuaReader54(BinaryReader br) : base(br) {}
        public LuaReader54(MemoryStream ms) : base(ms) {}
        public LuaReader54(string file) : base(file) {}

        public override string ReadString()
        {
            int Size = (int)ReadSize();

            if (Size == 0)
                return "";

            string Str = "";
            ReadBytes(Size).ToList().ForEach(b => Str += (char)b);

            return Str;
        }
    }
}
