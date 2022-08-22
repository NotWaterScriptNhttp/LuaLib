using System.IO;
using System.Linq;

namespace LuaLib.LuaHelpers.Versions.LuaReader
{
    internal class LuaReader54 : LuaHelpers.LuaReader
    {
        public LuaReader54(CustomBinaryReader br) : base(br) {}
        public LuaReader54(MemoryStream ms) : base(ms) {}
        public LuaReader54(string file) : base(file) {}


        public override int ReadNumber32()
        {
            return (int)ReadUnsigned(int.MaxValue);
        }
        public override string ReadString()
        {
            int Size = (int)ReadSize();

            if (Size == 0)
                return "";

            string Str = "";
            ReadBytes(Size - 1).ToList().ForEach(b => Str += (char)b);

            return Str;
        }
    }
}
