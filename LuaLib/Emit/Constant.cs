using System.Text;

namespace LuaLib.Emit
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
        INT54 = 3,
        NUMBER54 = 19,

        FALSE = 1,
        TRUE = 17,
        #endregion
    }

    public class Constant
    {
        public ConstantType Type;
        public ConstantType? AltType;
        public dynamic Value;

        public Constant(ConstantType type, dynamic value, ConstantType? altType = null)
        {
            Type = type;
            Value = value;
            AltType = altType;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Constant: {\n");
            sb.Append($" Type: {Type},\n");
            sb.Append($" AlternativeType: {AltType},\n");
            sb.Append($" Value: {Value},\n");
            sb.Append("} - Constant");

            return sb.ToString();
        }
    }
}
