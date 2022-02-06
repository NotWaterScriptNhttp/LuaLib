using System.Text;

namespace LuaLib.Lua.Emit
{
    public enum ConstantType
    {
        NIL = 0,
        BOOLEAN = 1,
        LIGHTUSERDATA = 2,
        NUMBER = 3,
        STRING = 4,
        TABLE = 5,
        FUNCTION = 6,
        USERDATA = 7,
        THREAD = 8
    }

    public struct Constant
    {
        public ConstantType Type;
        public dynamic Value;

        public Constant(ConstantType type, dynamic value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Constant: {\n");
            sb.Append($" Type: {Type},\n");
            sb.Append($" Value: {Value}\n");
            sb.Append("} - Constant");

            return sb.ToString();
        }
    }
}
