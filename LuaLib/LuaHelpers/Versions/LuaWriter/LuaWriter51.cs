using System;
using System.Text;

using LuaLib.Emit;

namespace LuaLib.LuaHelpers.Versions.LuaWriter
{
    internal class LuaWriter51 : LuaHelpers.LuaWriter
    {
        internal LuaWriter51() : base() {}

        internal override void DumpString(string str)
        {
            void WriteLen(long len)
            {
                if (Is64Bit)
                    DumpInt64(len);
                else DumpInt((int)len);
            }

            if (str == null || str == "")
            {
                WriteLen(0);
                return;
            }

            WriteLen(str.Length + 1);
            writer.Write(Encoding.UTF8.GetBytes(str += "\0"));
        }

        internal override void DumpHeader(LuaHelpers.LuaHeader header)
        {
            writer.Write(LuaSig); // Write the signature of luac

            writer.Write((byte)0x51); // Version of luac

            writer.Write(header.Format);
            writer.Write(BitConverter.IsLittleEndian); // endianness
            writer.Write(new byte[]
            {
                4, // int32
                (byte)(header.Is64Bit ? 8 : 4), // size_t
                4, // instruction (uint32)
                (byte)(header.Is64Bit ? 8 : 4) // float/double
            });
            writer.Write(header.IsIntegral);
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

                writer.Write((byte)con.Type);

                switch (con.Type)
                {
                    case ConstantType.NIL:
                        break;
                    case ConstantType.BOOLEAN:
                        writer.Write((bool)con.Value);
                        break;
                    case ConstantType.NUMBER:
                        if (Is64Bit)
                            writer.Write((double)con.Value);
                        else writer.Write((float)con.Value);
                        break;
                    case ConstantType.STRING:
                        DumpString(con.Value);
                        break;
                    default:
                        throw new Exception($"Constant ({con.Type}) is not supported by lua 5.1");
                }
            }
        }

        internal override void DumpUpValues(Emit.Function func, WriterOptions options)
        {
            throw new NotImplementedException();
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
            if (func.IsMainChunk == true)
                DumpString(func.FuncName);
            else DumpString(null);

            writer.Write(func.LineDefined);
            writer.Write(func.LastLineDefined);
            writer.Write(func.nups);
            writer.Write(func.numparams);
            writer.Write(func.is_vararg);

            if (options.KeepOldMaxStacksize)
                writer.Write(func.maxstacksize);
            else writer.Write(CalculateMaxStackSize(func));

            DumpCode(func, options);
            DumpConstants(func, options);

            writer.Write(func.FunctionCount);
            for (int i = 0; i < func.FunctionCount; i++)
                DumpFunction(func.Functions[i], options);

            DumpDebug(func, options);
        }
    }
}
