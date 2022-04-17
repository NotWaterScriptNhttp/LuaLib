using System;
using System.Text;

using LuaLib.Emit;

namespace LuaLib.LuaHelpers.Versions.LuaWriter
{
    internal class LuaWriter53 : LuaHelpers.LuaWriter
    {
        internal override void DumpString(string str)
        {
            if (str == null || str == "")
                writer.Write((byte)0);
            else
            {
                int size = str.Length + 1;
                if (size < 0xFF)
                    writer.Write((byte)size);
                else
                {
                    writer.Write((byte)0xFF);
                    writer.Write(size);
                }

                writer.Write(Encoding.UTF8.GetBytes(str));
            }
        }

        internal override void DumpHeader(LuaHelpers.LuaHeader header)
        {
            writer.Write(LuaSig); // Write the signature of luac

            writer.Write((byte)0x53); // Version of luac

            writer.Write(header.Format);
            writer.Write(LuaTail);

            writer.Write(new byte[]
            {
                4, // int32
                (byte)(header.Is64Bit ? 8 : 4), // size_t
                4, // instruction (uint32)
                (byte)(header.Is64Bit ? 8 : 4), // int / long
                (byte)(header.Is64Bit ? 8 : 4) // float / double
            });
            writer.Write((long)LuaInt);
            writer.Write(LuaNum);
        }

        internal override void DumpCode(Emit.Function func, WriterOptions options)
        {
            writer.Write(func.InstructionCount);

            for (int i = 0; i < func.InstructionCount; i++)
                writer.Write(func.Instructions[i].GetRawInstruction());
        }

        internal override void DumpConstants(Emit.Function func, WriterOptions options)
        {
            writer.Write(func.ConstantCount);

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
                    case ConstantType.BOOLEAN:
                        writer.Write((bool)con.Value);
                        break;
                    case ConstantType.NUMBER:
                        if (con.AltType == ConstantType.NUMBER_INT)
                        {
                            if (Is64Bit)
                                DumpInt64(con.Value);
                            else DumpInt(con.Value);
                        }
                        else
                        {
                            if (Is64Bit)
                                writer.Write(DoEndian(BitConverter.GetBytes((double)con.Value)));
                            else writer.Write(DoEndian(BitConverter.GetBytes((float)con.Value)));
                        }
                        break;
                    case ConstantType.STRING:
                        DumpString(con.Value);
                        break;
                    default:
                        throw new Exception($"Constant ({con.Type}{(con.AltType.HasValue ? $" / {con.AltType}" : "")}) is not supported by lua 5.3");
                }
            }
        }

        internal override void DumpUpValues(Emit.Function func, WriterOptions options)
        {
            writer.Write(func.UpValueCount);

            for (int i = 0; i < func.UpValueCount; i++)
            {
                UpValue uv = func.UpValues[i];

                writer.Write(uv.InStack);
                writer.Write(uv.Idx);
            }
        }

        internal override void DumpDebug(Emit.Function func, WriterOptions options)
        {
            writer.Write(func.LineinfoSize);
            for (int i = 0; i < func.LineinfoSize; i++)
                writer.Write(func.lineinfo[i]);

            writer.Write(func.LocalCount);
            for (int i = 0; i < func.LocalCount; i++)
            {
                Local loc = func.Locals[i];

                DumpString(loc.Varname);
                writer.Write(loc.StartPC);
                writer.Write(loc.EndPC);
            }

            writer.Write(func.UpValueCount);
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

            writer.Write(func.LineDefined);
            writer.Write(func.LastLineDefined);
            writer.Write(func.numparams);
            writer.Write(func.is_vararg);

            if (options.KeepOldMaxStacksize)
                writer.Write(func.maxstacksize);
            else writer.Write(CalculateMaxStackSize(func));

            DumpCode(func, options);
            DumpConstants(func, options);
            DumpUpValues(func, options);

            #region DumpProtos

            writer.Write(func.FunctionCount);

            for (int i = 0; i < func.FunctionCount; i++)
                DumpFunction(func.Functions[i], options);

            #endregion

            DumpDebug(func, options);
        }
    }
}
