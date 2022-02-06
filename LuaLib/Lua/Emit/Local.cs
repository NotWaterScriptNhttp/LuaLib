using System.Text;

namespace LuaLib.Lua.Emit
{
    public class Local
    {
        public string Varname;
        public int StartPC;
        public int EndPC;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Local: {\n");
            sb.Append($" VarName: {Varname},\n");
            sb.Append($" StartPC: {StartPC},\n");
            sb.Append($" EndPC: {EndPC}\n");
            sb.Append("} - Local");

            return sb.ToString();
        }
    }
}
