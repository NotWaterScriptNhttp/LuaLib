using System;
using System.Text;
using System.Collections.Generic;

using LuaLib.Lua.LuaHelpers;

namespace LuaLib.Lua.Emit
{
    public class Function
    {
        public enum VARARG : byte
        {
            HASARG = 1,
            ISVARARG = 2,
            NEEDSVARARG = 4,
            UNKNOWN
        }

        public List<Constant> Constants = new List<Constant>();
        public List<Instruction> Instructions = new List<Instruction>();
        public List<Function> Functions = new List<Function>();

        public List<Local> Locals = new List<Local>();
        public List<UpValue> UpValues = new List<UpValue>();


        public int ConstantCount
        {
            get
            {
                return Constants.Count;
            }
        }
        public int InstructionCount
        {
            get
            {
                return Instructions.Count;
            }
        }
        public int FunctionCount
        {
            get
            {
                return Functions.Count;
            }
        }
        public int LocalCount
        {
            get
            {
                return Locals.Count;
            }
        }
        public int UpValueCount
        {
            get
            {
                return UpValues.Count;
            }
        }
        public int LineinfoSize
        {
            get
            {
                return lineinfo.Length;
            }
        }

        public bool IsMainChunkChild { get; private set; }

        public int LineDefined;
        public int LastLineDefined;

        public byte nups;
        public byte numparams;
        public byte is_vararg;
        public byte maxstacksize;

        public int[] lineinfo;

        public string FuncName { get; internal set; } = "";

        private Function() {}

        internal static Function GetFunction(LuaReader reader)
        {
            List<Instruction> GetInstructions()
            {
                List<Instruction> instrs = new List<Instruction>();

                int instrCount = reader.ReadNumber32();

                for (int i = 0; i < instrCount; i++)
                    instrs.Add(new Instruction(reader.ReadUNumber32()));

                return instrs;
            }
            List<Constant> GetConstants()
            {
                List<Constant> consts = new List<Constant>();

                int constCount = reader.ReadNumber32();

                for (int i = 0; i< constCount; i++)
                {
                    byte ttype = reader.ReadByte();

                    switch ((ConstantType)ttype)
                    {
                        case ConstantType.NIL:
                            consts.Add(new Constant(ConstantType.NIL, null));
                            break;
                        case ConstantType.BOOLEAN:
                            consts.Add(new Constant(ConstantType.BOOLEAN, reader.ReadBoolean()));
                            break;
                        case ConstantType.NUMBER:
                            consts.Add(new Constant(ConstantType.NUMBER, reader.ReadFloat()));
                            break;
                        case ConstantType.STRING:
                            consts.Add(new Constant(ConstantType.STRING, reader.ReadString()));
                            break;
                        default:
                            throw new Exception($"This constant is not valid '{ttype} -> ({(ConstantType)ttype})'");
                    }
                }

                return consts;
            }
            List<Function> GetFunctions()
            {
                List<Function> functions = new List<Function>();

                int funcCount = reader.ReadNumber32();

                for (int i = 0; i < funcCount; i++)
                    functions.Add(GetFunction(reader));

                return functions;
            }
            void GetDebug(Function func)
            {
                // Not using reader.GetVector cause things will break very easily

                int lineinfoSize = reader.ReadNumber32();

                if (lineinfoSize != 0)
                {
                    func.lineinfo = new int[lineinfoSize];

                    for (int i = 0; i < lineinfoSize; i++)
                        func.lineinfo[i] = reader.ReadNumber32();
                }

                int localCount = reader.ReadNumber32();

                if (localCount != 0)
                {
                    for (int i = 0; i < localCount; i++)
                        func.Locals.Add(new Local
                        {
                            Varname = reader.ReadString(),
                            StartPC = reader.ReadNumber32(),
                            EndPC = reader.ReadNumber32()
                        });
                }

                int upvalueCount = reader.ReadNumber32();

                if (upvalueCount != 0)
                {
                    for (int i = 0; i < upvalueCount; i++)
                        func.UpValues.Add(new UpValue() { Name = reader.ReadString() });
                }
            }

            Function function = new Function();

            function.FuncName = reader.ReadString();
            function.LineDefined = reader.ReadNumber32();
            function.LastLineDefined = reader.ReadNumber32();
            function.nups = reader.ReadByte();
            function.numparams = reader.ReadByte();
            function.is_vararg = reader.ReadByte();
            function.maxstacksize = reader.ReadByte();

            function.Instructions = GetInstructions();
            function.Constants = GetConstants();
            function.Functions = GetFunctions();
            GetDebug(function);

            return function;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Function: {\n");
            sb.Append($" Name: {FuncName},\n");
            sb.Append($" Constants: {ConstantCount},\n");
            sb.Append($" Instructions: {InstructionCount},\n");
            sb.Append($" SubFunctions: {FunctionCount},\n");
            sb.Append($" Locals: {LocalCount},\n");
            sb.Append($" UpValues: {UpValueCount},\n");
            sb.Append($" Vararg: {(VARARG)is_vararg},\n");
            sb.Append($" MaxStacksize: {maxstacksize}\n");
            sb.Append("} - Function");

            return sb.ToString();
        }
    }
}
