using System.Text;

namespace LuaLib.Emit
{
    public class AbsLineInfo
    {
        public int line;
        public int pc;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("LineInfo: {\n");
            sb.Append($" Line: {line},\n");
            sb.Append($" PC: {pc}\n");
            sb.Append("} - LineInfo");

            return sb.ToString();
        }
    }
}
