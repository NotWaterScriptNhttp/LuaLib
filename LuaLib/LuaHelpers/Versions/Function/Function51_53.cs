using System;
using System.Collections.Generic;

using LuaLib.Emit;

namespace LuaLib.LuaHelpers.Versions.Function
{
    internal class Function51_53 : FunctionParser
    {
        public Function51_53(LuaHelpers.LuaReader lr) : base(lr) {}

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
                    case ConstantType.BOOLEAN:
                        consts.Add(new Constant(ConstantType.BOOLEAN, lr.ReadBoolean()));
                        break;
                    case ConstantType.NUMBER:
                        consts.Add(new Constant(ConstantType.NUMBER, lr.ReadFloat()));
                        break;
                    case ConstantType.NUMBER_INT:
                        consts.Add(new Constant(ConstantType.NUMBER, lr.ReadNumber64(), ConstantType.NUMBER_INT));
                        break;
                    case ConstantType.STRING:
                    case ConstantType.LNGSTR:
                        consts.Add(new Constant(ConstantType.STRING, lr.ReadString()));
                        break;
                    default:
                        //throw new Exception($"This constant is not valid '{ttype} -> ({(ConstantType)ttype})'");
                        break;
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
                    func.lineinfo[i] = lr.ReadNumber32();
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

                if (func.UpValueCount == 0 || func.UpValues == null)
                {
                    func.UpValues = new List<UpValue>();

                    for (int i = 0; i < upvalueCount; i++)
                        func.UpValues.Add(new UpValue() { Name = null });
                }

                for (int i = 0; i < upvalueCount; i++)
                    func.UpValues[i].Name = lr.ReadString();
            }
        }
    }
}
