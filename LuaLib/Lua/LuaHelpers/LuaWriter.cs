using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LuaLib.Lua.Emit;
using LuaLib.Lua.LuaHelpers;

namespace LuaLib.Lua.LuaHelpers
{
    internal class LuaWriter
    {
        private List<byte> dataToWrite;
        private bool Is64;
        private bool IsLittle;

        //TODO: make this working
        private byte CalculateMaxStackSize(Function func)
        {
            return func.maxstacksize;
        }
        private byte[] DoEndian(byte[] data)
        {
            if (!IsLittle && BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return data;
        }

        private byte[] CreateHeader(Chunk c, bool UseOld, LuaVersion ver)
        {
            List<byte> holder = new List<byte>();

            holder.AddRange(new byte[4]
            {
                0x1B,
                0x4C,
                0x75,
                0x61
            });

            LuaHeader header = c.Header;

            if (UseOld)
                holder.Add((byte)header.Version);
            else holder.Add((byte)ver);

            holder.AddRange(new byte[]
            {
                header.Format,
                (byte)(header.IsLittleEndian ? 1 : 0),
                4, // sizeof(int)
                (byte)(header.Is64Bit ? 8 : 4),
                4, // Instruction size
                8, // luaNumber size (double)
                (byte)(header.IsIntegral ? 1 : 0)
            });

            return holder.ToArray();
        }

        private byte[] DumpInt(int i)
        {
            return DoEndian(BitConverter.GetBytes(i));
        }
        private byte[] DumpString(string str)
        {
            List<byte> holder = new List<byte>();

            str = str.Replace("\0", "");

            if (str.Length != 0)
                str += "\0";

            if (Is64)
                holder.AddRange(DoEndian(BitConverter.GetBytes((long)str.Length)));
            else holder.AddRange(DumpInt(str.Length));

            holder.AddRange(Encoding.UTF8.GetBytes(str));

            return holder.ToArray();
        }
        private byte[] DumpNumber(double number)
        {
            if (Is64)
                return DoEndian(BitConverter.GetBytes(number));
            else return DoEndian(BitConverter.GetBytes((float)number));
        } 


        private byte[] DumpFunction(Function func, WriterOptions options)
        {
            List<byte> holder = new List<byte>();

            holder.AddRange(DumpString(func.FuncName));
            holder.AddRange(DumpInt(func.LineDefined));
            holder.AddRange(DumpInt(func.LastLineDefined));
            holder.Add(func.nups);
            holder.Add(func.numparams);
            holder.Add(func.is_vararg);

            if (options.KeepOldMaxStacksize)
                holder.Add(func.maxstacksize);
            else holder.Add(CalculateMaxStackSize(func));

            holder.AddRange(DumpInt(func.InstructionCount));
            for (int i = 0; i < func.InstructionCount; i++)
                holder.AddRange(func.Instructions[i].GetInstructionBytes());

            holder.AddRange(DumpInt(func.ConstantCount));
            for (int i = 0; i < func.ConstantCount; i++)
            {
                Constant con = func.Constants[i];

                holder.Add((byte)con.Type);

                switch (con.Type)
                {
                    case ConstantType.BOOLEAN:
                        holder.Add((byte)(con.Value == true ? 1 : 0));
                        break;
                    case ConstantType.NUMBER:
                        holder.AddRange(DumpNumber(con.Value));
                        break;
                    case ConstantType.STRING:
                        holder.AddRange(DumpString(con.Value));
                        break;
                    default:
                        throw new Exception($"This constant is not valid '{con.Type}'");
                }
            }

            holder.AddRange(DumpInt(func.FunctionCount));
            for (int i = 0; i < func.FunctionCount; i++)
                holder.AddRange(DumpFunction(func.Functions[i], options));

            if (func.lineinfo == null)
                func.lineinfo = new int[0];

            holder.AddRange(DumpInt(func.LineinfoSize));
            for (int i = 0; i < func.LineinfoSize; i++)
                holder.AddRange(DumpInt(func.lineinfo[i]));

            holder.AddRange(DumpInt(func.LocalCount));
            for (int i = 0; i < func.LocalCount; i++)
            {
                Local local = func.Locals[i];

                holder.AddRange(DumpString(local.Varname));
                holder.AddRange(DumpInt(local.StartPC));
                holder.AddRange(DumpInt(local.EndPC));
            }

            holder.AddRange(DumpInt(func.UpValueCount));
            for (int i = 0; i < func.UpValueCount; i++)
                holder.AddRange(DumpString(func.UpValues[i].Name));

            return holder.ToArray();
        }

        internal LuaWriter(Chunk chunk, WriterOptions options)
        {
            #region Setting up variables
            dataToWrite = new List<byte>();

            Is64 = chunk.Header.Is64Bit;
            IsLittle = chunk.Header.IsLittleEndian;
            #endregion

            dataToWrite.AddRange(CreateHeader(chunk, options.KeepLuaVersion, options.NewLuaVersion)); // Add the header to the file

            #region Name Checking
            string name = chunk.MainFunction.FuncName;

            if (string.IsNullOrEmpty(name))
                name = "@Unnamed.lua";
            if (!name.StartsWith("@"))
                name = "@" + name;

            chunk.MainFunction.FuncName = name;
            #endregion

            dataToWrite.AddRange(DumpFunction(chunk.MainFunction, options)); // Write the Chunk + all the other things stored in the chunk
        }

        public void Write(string outfile)
        {
            File.WriteAllBytes(outfile, dataToWrite.ToArray());
        }
    }
}
