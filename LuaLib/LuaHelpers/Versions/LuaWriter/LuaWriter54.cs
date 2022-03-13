using System;
using System.Text;

using LuaLib.Emit;

namespace LuaLib.LuaHelpers.Versions.LuaWriter
{
    internal class LuaWriter54 : LuaHelpers.LuaWriter
    {
        protected override void DumpInt(int i32)
        {
            if (i32 < 0)
                throw new Exception("Int must be positive");

            DumpSize((ulong)i32);
        }

        internal override void DumpString(string str)
        {
            if (str == null || str == "")
                DumpSize(0);
            else
            {
                DumpSize((ulong)str.Length + 1);
                writer.Write(Encoding.UTF8.GetBytes(str));
            }
        }

        internal override void DumpHeader(Chunk chunk)
        {
            LuaHelpers.LuaHeader header = chunk.Header;

            writer.Write(LuaSig); // Write the signature of luac

            writer.Write((byte)0x54); // Version of luac
            writer.Write(header.Format);

            writer.Write(LuaTail);

            writer.Write(new byte[]
            {
                4, // instruction (uint32)
                (byte)(header.Is64Bit ? 8 : 4), // int / long
                (byte)(header.Is64Bit ? 8 : 4) // float / double
            });
            writer.Write((long)LuaInt);
            writer.Write(LuaNum);
        }

        internal override void DumpCode(Emit.Function func, WriterOptions options)
        {
            DumpInt(func.InstructionCount);

            for (int i = 0; i < func.InstructionCount; i++)
                writer.Write(func.Instructions[i].GetRawInstruction());
        }

        internal override void DumpConstants(Emit.Function func, WriterOptions options)
        {
            DumpInt(func.ConstantCount);

            for (int i = 0; i < func.ConstantCount; i++)
            {
                Constant con = func.Constants[i];

                if (con.AltType.HasValue)
                    writer.Write((byte)con.AltType);
                else writer.Write((byte)con.Type);

                switch (con.Type)
                {
                    case ConstantType.NIL:
                        break;
                    case ConstantType.BOOLEAN: // Type boolean always has AltType
                        break;
                    case ConstantType.NUMBER:
                        {
                            if (con.AltType == ConstantType.INT54)
                            {
                                if (Is64Bit)
                                    DumpInt32(con.Value);
                                else DumpInt64(con.Value);
                            }
                            else
                            {
                                if (Is64Bit)
                                    writer.Write(DoEndian(BitConverter.GetBytes((double)con.Value)));
                                else writer.Write(DoEndian(BitConverter.GetBytes((float)con.Value)));
                            }
                        }
                        break;
                    case ConstantType.STRING:
                        DumpString(con.Value);
                        break;
                    default:
                        throw new Exception($"Constant ({con.Type}{(con.AltType.HasValue ? $" / {con.AltType}" : "")}) is not supported by lua 5.4");
                }
            }
        }

        internal override void DumpUpValues(Emit.Function func, WriterOptions options)
        {
            DumpInt(func.UpValueCount);

            for (int i = 0; i < func.UpValueCount; i++)
            {
                UpValue uv = func.UpValues[i];

                writer.Write(uv.InStack);
                writer.Write(uv.Idx);
                writer.Write(uv.Kind);
            }
        }

        internal override void DumpDebug(Emit.Function func, WriterOptions options)
        {
            DumpInt(func.LineinfoSize);
            for (int i = 0; i < func.LineinfoSize; i++)
                writer.Write((byte)func.lineinfo[i]);

            DumpInt(func.AbsLineinfoCount);
            for (int i = 0; i < func.AbsLineinfoCount; i++)
            {
                AbsLineInfo ali = func.AbsLineinfo[i];

                DumpInt(ali.pc);
                DumpInt(ali.line);
            }

            DumpInt(func.LocalCount);
            for (int i = 0; i < func.LocalCount; i++)
            {
                Local loc = func.Locals[i];

                DumpString(loc.Varname);
                DumpInt(loc.StartPC);
                DumpInt(loc.EndPC);
            }

            DumpInt(func.UpValueCount);
            for (int i = 0; i < func.UpValueCount; i++)
                DumpString(func.UpValues[i].Name);
        }

        internal override void DumpFunction(Emit.Function func, WriterOptions options)
        {
            if (func.IsMainChunk)
            {
                writer.Write((byte)func.UpValueCount);
                DumpString(func.FuncName);
            }
            else DumpString(null);

            DumpInt(func.LineDefined);
            DumpInt(func.LastLineDefined);
            writer.Write(func.numparams);
            writer.Write(func.is_vararg);

            if (options.KeepOldMaxStacksize)
                writer.Write(func.maxstacksize);
            else writer.Write(CalculateMaxStackSize(func));

            DumpCode(func, options);
            DumpConstants(func, options);
            DumpUpValues(func, options);

            #region DumpProtos

            DumpInt(func.FunctionCount);

            for (int i = 0; i < func.FunctionCount; i++)
                DumpFunction(func.Functions[i], options);

            #endregion

            DumpDebug(func, options);
        }
    }
}
