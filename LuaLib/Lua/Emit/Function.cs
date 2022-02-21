﻿using System;
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
        public List<LineInfo> Lineinfo = new List<LineInfo>();


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
                return Lineinfo.Count;
            }
        }

        public bool IsMainChunkChild { get; private set; }

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
                            if (version >= LuaVersion.LUA_VERSION_5_4)
                                consts.Add(new Constant(ConstantType.FALSE, false));
                            else consts.Add(new Constant(ConstantType.BOOLEAN, reader.ReadBoolean()));
                            break;
                        case ConstantType.TRUE:
                            consts.Add(new Constant(ConstantType.TRUE, true));
                            break;
                        case ConstantType.NUMBER:
                            consts.Add(new Constant(ConstantType.NUMBER, reader.ReadFloat()));
                            break;
                        case ConstantType.NUMBER_INT:
                            consts.Add(new Constant(ConstantType.NUMBER, reader.ReadNumber64()));
                            break;
                        case ConstantType.STRING:
                            consts.Add(new Constant(ConstantType.STRING, reader.ReadString()));
                            break;
                        case ConstantType.LNGSTR:
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
                    functions.Add(GetFunction(reader, version));

                return functions;
            }
            List<UpValue> GetUpValues()
            {
                List<UpValue> upvalues = new List<UpValue>();

                if (version == LuaVersion.LUA_VERSION_5_1)
                    return new List<UpValue>();

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
            void GetDebug(Function func)
            {
                // Not using reader.GetVector cause things will break very easily

                // lineinfo
                {
                    int lineinfoSize = reader.ReadNumber32();

                    for (int i = 0; i < lineinfoSize; i++)
                    {
                        LineInfo li = new LineInfo();

                        if (version >= LuaVersion.LUA_VERSION_5_4)
                            li.pc = reader.ReadNumber32();

                        li.line = reader.ReadNumber32();
                    }
                }

                // locals
                {
                    int localCount = reader.ReadNumber32();

                    for (int i = 0; i < localCount; i++)
                        func.Locals.Add(new Local
                        {
                            Varname = reader.ReadString(),
                            StartPC = reader.ReadNumber32(),
                            EndPC = reader.ReadNumber32()
                        });
                }

                // upvalues
                {
                    int upvalueCount = reader.ReadNumber32();

                    if (version == LuaVersion.LUA_VERSION_5_1)
                        func.UpValues.AddRange(new UpValue[upvalueCount]);

                    for (int i = 0; i < upvalueCount; i++)
                        func.UpValues[i].Name = reader.ReadString();
                }
            }

            Function function = new Function();

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
            function.Constants = GetConstants();
            
            if (version <= LuaVersion.LUA_VERSION_5_2)
                function.Functions = GetFunctions();

            function.UpValues = GetUpValues();

            if (version >= LuaVersion.LUA_VERSION_5_3)
                function.Functions = GetFunctions();

            if (version == LuaVersion.LUA_VERSION_5_2)
                function.FuncName = reader.ReadString();

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
