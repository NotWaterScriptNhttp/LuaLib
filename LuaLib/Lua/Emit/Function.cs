using System;
using System.Text;
using System.Collections.Generic;

using LuaLib.Lua.LuaHelpers;
using LuaLib.Lua.LuaHelpers.Versions.Function;

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
        public List<AbsLineInfo> AbsLineinfo = new List<AbsLineInfo>();

        public int[] lineinfo;


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
        public int AbsLineinfoCount
        {
            get
            {
                return AbsLineinfo.Count;
            }
        }
        public int LineinfoSize
        {
            get
            {
                if (lineinfo == null)
                    lineinfo = new int[0];

                return lineinfo.Length;
            }
        }

        public bool IsMainChunk { get; internal set; }
        public bool IsMainChunkChild { get; internal set; }

        public int LineDefined;
        public int LastLineDefined;

        public byte nups;
        public byte numparams;
        public byte is_vararg;
        public byte maxstacksize;

        public string FuncName { get; internal set; } = "";

        internal Function() {}

        internal static Function GetFunction(LuaReader reader, LuaVersion version)
        {
            List<Instruction> GetInstructions()
            {
                List<Instruction> instrs = new List<Instruction>();

                int instrCount = reader.ReadNumber32();

                for (int i = 0; i < instrCount; i++)
                    instrs.Add(new Instruction(reader.ReadUNumber32(), version));

                return instrs;
            }
            List<Function> GetFunctions()
            {
                List<Function> functions = new List<Function>();

                int funcCount = reader.ReadNumber32();

                for (int i = 0; i < funcCount; i++)
                    functions.Add(GetFunction(reader, version));

                return functions;
            }
            List<UpValue> GetUpValues()
            {
                List<UpValue> upvalues = new List<UpValue>();

                if (version == LuaVersion.LUA_VERSION_5_1)
                    return upvalues;

                int upCount = reader.ReadNumber32();

                for (int i = 0; i < upCount; i++)
                {
                    UpValue upval = new UpValue();

                    upval.InStack = reader.ReadByte();
                    upval.Idx = reader.ReadByte();

                    if (version >= LuaVersion.LUA_VERSION_5_4)
                        upval.Kind = reader.ReadByte();

                    upvalues.Add(upval);
                }

                return upvalues;
            }

            Function function = new Function();
            FunctionParser parser = null;

            switch (version)
            {
                case LuaVersion.LUA_VERSION_5_1:
                case LuaVersion.LUA_VERSION_5_2:
                case LuaVersion.LUA_VERSION_5_3:
                    parser = new Function51_53(reader);
                    break;
                case LuaVersion.LUA_VERSION_5_4:
                    parser = new Function54(reader);
                    break;
                default:
                    throw new Exception($"No function parser for ({version})");
            }

            if (version != LuaVersion.LUA_VERSION_5_2)
                function.FuncName = reader.ReadString();

            function.LineDefined = reader.ReadNumber32();
            function.LastLineDefined = reader.ReadNumber32();
            
            if (version == LuaVersion.LUA_VERSION_5_1)
                function.nups = reader.ReadByte();

            function.numparams = reader.ReadByte();
            function.is_vararg = reader.ReadByte();
            function.maxstacksize = reader.ReadByte();

            function.Instructions = GetInstructions();
            function.Constants = parser.GetConstants();
            
            if (version <= LuaVersion.LUA_VERSION_5_2)
                function.Functions = GetFunctions();

            function.UpValues = GetUpValues();

            if (version >= LuaVersion.LUA_VERSION_5_3)
                function.Functions = GetFunctions();

            if (version == LuaVersion.LUA_VERSION_5_2)
                function.FuncName = reader.ReadString();

            parser.GetDebug(function);

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
