using System.Text;

namespace LuaLib.Lua.Emit
{
    public class UpValue
    {
        public string Name;
        public byte InStack;
        public byte Idx;
        public byte Kind;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("UpValue: {\n");
            sb.Append($"Name: {Name},\n");
            sb.Append($"InStack: {InStack},\n");
            sb.Append($"Index: {Idx},\n");
            sb.Append($"Kind: {Kind}\n");
            sb.Append("} - UpValue");

            return sb.ToString();
        }
    }
}
