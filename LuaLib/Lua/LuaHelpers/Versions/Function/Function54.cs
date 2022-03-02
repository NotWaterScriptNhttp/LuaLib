using LuaLib.Lua.Emit;
using System;
using System.Collections.Generic;

namespace LuaLib.Lua.LuaHelpers.Versions.Function
{
    internal class Function54 : FunctionParser
    {
        public Function54(LuaHelpers.LuaReader lr) : base(lr) {}

        public override List<Constant> GetConstants()
        {
            List<Constant> consts = new List<Constant>();

            int constCount = lr.ReadNumber32();

            for (int i = 0; i < constCount; i++)
            {
                byte ttype = lr.ReadByte();

                switch ((ConstantType)ttype)
                {
                    case ConstantType.NIL:
                        consts.Add(new Constant(ConstantType.NIL, null));
                        break;
                    case ConstantType.FALSE:
                        consts.Add(new Constant(ConstantType.BOOLEAN, false, ConstantType.FALSE));
                        break;
                    case ConstantType.TRUE:
                        consts.Add(new Constant(ConstantType.BOOLEAN, true, ConstantType.TRUE));
                        break;
                    case ConstantType.INT54:
                        consts.Add(new Constant(ConstantType.NUMBER, lr.ReadNumber64(), ConstantType.INT54));
                        break;
                    case ConstantType.NUMBER54:
                        consts.Add(new Constant(ConstantType.NUMBER, lr.ReadFloat(), ConstantType.NUMBER54));
                        break;
                    case ConstantType.STRING:
                        consts.Add(new Constant(ConstantType.STRING, lr.ReadString()));
                        break;
                    case ConstantType.LNGSTR:
                        consts.Add(new Constant(ConstantType.STRING, lr.ReadString(), ConstantType.LNGSTR));
                        break;
                    default:
                        throw new Exception($"This constant is not valid '{ttype} -> ({(ConstantType)ttype})'");
                }
            }

            return consts;
        }

        public override void GetDebug(Emit.Function func)
        {
            // lineinfo
            {
                int lineinfoSize = lr.ReadNumber32();

                func.lineinfo = new int[lineinfoSize];

                for (int i = 0; i < lineinfoSize; i++)
                    func.lineinfo[i] = lr.ReadByte();
            }

            // absLineInfo
            {
                int abslineinfoSize = lr.ReadNumber32();

                for (int i = 0; i < abslineinfoSize; i++)
                {
                    AbsLineInfo li = new AbsLineInfo();

                    li.pc = lr.ReadNumber32();
                    li.line = lr.ReadNumber32();

                    func.AbsLineinfo.Add(li);
                }
            }

            // locals
            {
                int localCount = lr.ReadNumber32();

                for (int i = 0; i < localCount; i++)
                    func.Locals.Add(new Local
                    {
                        Varname = lr.ReadString(),
                        StartPC = lr.ReadNumber32(),
                        EndPC = lr.ReadNumber32()
                    });
            }

            // upvalues
            {
                int upvalueCount = lr.ReadNumber32();

                for (int i = 0; i < upvalueCount; i++)
                    func.UpValues[i].Name = lr.ReadString();
            }
        }
    }
}
