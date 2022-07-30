using System;
using System.Text;
using System.Collections.Generic;

using LuaLib.LuaHelpers;
using LuaLib.LuaHelpers.Versions.Function;

namespace LuaLib.Emit
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
        public List<string> Strings = new List<string>();
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
        public int StringCount
        {
            get
            {
                return Strings.Count;
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

        public int LineDefined;
        public int LastLineDefined;

        public byte nups;
        public byte numparams;
        public byte is_vararg;
        public byte maxstacksize;

        public string FuncName = "";
        public uint MainId = 0;

        public bool IsMainChunk { get; internal set; }
        public bool IsMainChunkChild { get; internal set; }

        public Instruction DefingInstruction { get; internal set; } = null;
        public Function Parent { get; internal set; } = null;

        internal static Function GetClassicLuaFunction(LuaReader reader, LuaVersion version)
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
                    functions.Add(GetClassicLuaFunction(reader, version));

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
                case LuaVersion.LUA_VERSION_U:
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
        internal static Function GetLuaUFunction(LuaReader reader)
        {
            Function func = new Function();

            string ReadString()
            {
                uint id = (uint)reader.ReadSize();

                return id == 0 ? null : func.Strings[(int)(id - 1)];
            }

            uint strcount = (uint)reader.ReadVarInt();
            for (uint i = 0; i < strcount; i++)
            {
                uint len = (uint)reader.ReadVarInt();

                func.Strings.Add(Encoding.UTF8.GetString(reader.ReadBytes((int)len)));
            }

            uint funccount = (uint)reader.ReadVarInt();
            for (uint i = 0; i < funccount; i++)
            {
                Function p = new Function();
                p.maxstacksize = reader.ReadByte();
                p.numparams = reader.ReadByte();
                p.nups = reader.ReadByte();
                p.is_vararg = reader.ReadByte();

                uint instrcount = (uint)reader.ReadVarInt();
                for (uint j = 0; j < instrcount; j++)
                    p.Instructions.Add(new Instruction(reader.ReadUNumber32(), LuaVersion.LUA_VERSION_U));

                uint constcount = (uint)reader.ReadVarInt();
                for (uint j = 0; j < constcount; j++)
                {
                    byte t;
                    switch ((ConstantType)(t = reader.ReadByte()))
                    {
                        case ConstantType.NIL:
                            p.Constants.Add(new Constant(ConstantType.NIL, null));
                            break;
                        case ConstantType.BOOLEAN:
                            p.Constants.Add(new Constant(ConstantType.BOOLEAN, reader.ReadBoolean()));
                            break;
                        case ConstantType.LU_NUMBER:
                            p.Constants.Add(new Constant(ConstantType.NUMBER, reader.ReadFloat(), ConstantType.LU_NUMBER));
                            break;
                        case ConstantType.LU_STRING:
                            p.Constants.Add(new Constant(ConstantType.STRING, ReadString(), ConstantType.LU_STRING));
                            break;
                        case ConstantType.LU_IMPORT:
                            //p.Constants.Add(new Constant(ConstantType.LU_IMPORT, )); looks like this won't be supported cause it requires some run time bullshit
                            throw new ApplicationException("Luau import constant is not supported");
                            break;
                        case ConstantType.LU_TABLE:
                            int keys = (int)reader.ReadVarInt();
                            float[] data = new float[keys];

                            for (int k = 0; k < keys; k++)
                                data[k] = 0.0f;

                            p.Constants.Add(new Constant(ConstantType.LU_TABLE, data));
                            break;
                        case ConstantType.LU_CLOSURE:
                            uint fid = (uint)reader.ReadVarInt();
                            p.Constants.Add(new Constant(ConstantType.LU_CLOSURE, func.Functions[(int)fid].Copy()));
                            break;
                        default:
                            throw new ApplicationException($"Unknown constant type: {t}");
                    }
                }

                uint pcount = (uint)reader.ReadVarInt();
                for (uint j = 0; j < pcount; j++)
                    reader.ReadVarInt(); // We already added functions to the list

                p.LineDefined = (int)reader.ReadVarInt();
                p.FuncName = ReadString();

                if (reader.ReadBoolean())
                {
                    byte gaplog2 = reader.ReadByte();

                    int interval = ((p.InstructionCount - 1) >> gaplog2) + 1;
                    int absoffset = (p.InstructionCount + 1) & ~3;

                    int lineinfosize = absoffset + interval * sizeof(int);
                    p.lineinfo = new int[lineinfosize];

                    byte lastoffset = 0;
                    for (int j = 0; j < p.InstructionCount; j++)
                        p.lineinfo[j] = (lastoffset += reader.ReadByte());

                    int lastline = 0;
                    for (int j = 0; j < interval; j++)
                        p.AbsLineinfo.Add(new AbsLineInfo() { line = lastline += reader.ReadNumber32() });
                }
                if (reader.ReadBoolean())
                {
                    int localcount = (int)reader.ReadVarInt();

                    for (int j = 0; j < localcount; j++)
                    {
                        Local loc = new Local();

                        loc.Varname = ReadString();
                        loc.StartPC = (int)reader.ReadVarInt();
                        loc.EndPC = (int)reader.ReadVarInt();
                        loc.Register = reader.ReadByte();

                        p.Locals.Add(loc);
                    }

                    int upvalcount = (int)reader.ReadVarInt();
                    for (int j = 0; j < upvalcount; j++)
                        p.UpValues.Add(new UpValue() { Name = ReadString() });
                }

                func.Functions.Add(p);
            }

            func.MainId = (uint)reader.ReadVarInt();

            return func;
        }
        internal static Function GetFunction(LuaReader reader, LuaVersion version, ChunkSettings settings)
        {
            switch (version)
            {
                case LuaVersion.LUA_VERSION_5_1:
                case LuaVersion.LUA_VERSION_5_2:
                case LuaVersion.LUA_VERSION_5_3:
                case LuaVersion.LUA_VERSION_5_4:
                    return GetClassicLuaFunction(reader, version);
                case LuaVersion.LUA_VERSION_U:
                    return GetLuaUFunction(reader);
                default:
                    throw new ApplicationException("Unknown lua version, can't get function data");
            }
        }

        //This thing currently only works for local names
        public bool TryGetName()
        {
            if (IsMainChunk)
                return false; // return cause main chunk always has a name
            if (!string.IsNullOrEmpty(FuncName))
                return false;

            string newName = null;


            if (Parent != null && DefingInstruction != null && Parent.LocalCount >= DefingInstruction.A)
                newName = Parent.Locals[DefingInstruction.A - 1].Varname;

            if (newName == null)
                return false;

            FuncName = newName;

            return true;
        }
        public Function Copy()
        {
            Constant[] consts = new Constant[ConstantCount];
            Constants.CopyTo(consts);

            Instruction[] instrs = new Instruction[InstructionCount];
            Instructions.CopyTo(instrs);

            Function[] funcs = new Function[FunctionCount];
            for (int i = 0; i < FunctionCount; i++)
                funcs[i] = Functions[i].Copy();

            Local[] locals = new Local[LocalCount];
            Locals.CopyTo(locals);

            UpValue[] upvalues = new UpValue[UpValueCount];
            UpValues.CopyTo(upvalues);

            AbsLineInfo[] abslineinfo = new AbsLineInfo[AbsLineinfoCount];
            AbsLineinfo.CopyTo(abslineinfo);

            return new Function
            {
                Constants = new List<Constant>(consts),
                Instructions = new List<Instruction>(instrs),
                Functions = new List<Function>(funcs),
                Locals = new List<Local>(locals),
                UpValues = new List<UpValue>(upvalues),
                AbsLineinfo = new List<AbsLineInfo>(abslineinfo),

                lineinfo = lineinfo,

                IsMainChunk = IsMainChunk,
                IsMainChunkChild = IsMainChunkChild,

                LineDefined = LineDefined,
                LastLineDefined = LastLineDefined,

                nups = nups,
                numparams = numparams,
                is_vararg = is_vararg,
                maxstacksize = maxstacksize,

                FuncName = FuncName
            };
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
