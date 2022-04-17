using System.Collections.Generic;

using LuaLib.Emit;

namespace LuaLib.LuaHelpers
{
    internal abstract class DecompilerBase
    {
        protected Function func;
        protected int currentFuncsBx;

        protected void SetNop(Function func, int idx) => func.Instructions[idx].Opcode = OpCodes.NOP;
        protected dynamic GetConstant(int idx)
        {
            if (idx > func.ConstantCount)
                return null;

            Constant con = func.Constants[idx];

            switch (con.Type)
            {
                case ConstantType.NIL:
                    return "nil";
                case ConstantType.BOOLEAN:
                    return con.Value == true ? "true" : "false";
                case ConstantType.NUMBER:
                    return con.Value;
                case ConstantType.STRING:
                    return $"\"{con.Value}\"";
                default:
                    return "\"unknown constant!!!\"";
            }
        }
        protected dynamic RK(int i, dynamic[] stack)
        {
            if (((i >> 8) & 1) == 1)
                return GetConstant(i - 256);

            return stack[i];
        }

        abstract public string Decompile(Function func, bool IsFunction = false);
    }
}
