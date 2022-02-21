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
        THREAD = 8,

        #region Lua 5.3 (Totally useless constant types)
        NUMBER_INT = 19,

        LNGSTR = 20,
        #endregion
        #region Lua 5.4 (Not as useless as the above)
        FALSE = 1,
        TRUE = 17,
        #endregion
    }

    public class Constant
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
