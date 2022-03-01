using System;
using System.IO;
using System.Text;

using System.Collections.Generic;

namespace LuaLib.Lua.LuaHelpers
{
    // This class exists just cause of debug things
    internal class CustomBinaryReader
    {
        private static bool UseDebugBytes = false;

        private struct Char
        {
            public byte b;
            public bool ShouldBeSelected;
        }

        private BinaryReader br;
        private byte[] bytes;
        public Stream BaseStream => br.BaseStream;

#if DEBUG
        private void UpdateBytes(int size)
        {
            if (!UseDebugBytes)
                return;

            List<Char> chars = new List<Char>();

            byte FilterByte(byte b)
            {
                if (b <= 32)
                    return (byte)'.';

                if (b >= 127)
                    return (byte)'.';

                return b;
            }
            void WriteCharVisuals()
            {
                Console.ResetColor();

                Console.Write("| ");

                chars.ForEach(c =>
                {
                    if (c.ShouldBeSelected)
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                    else Console.ResetColor();

                    Console.Write((char)c.b);
                });

                chars.Clear();
                Console.Write("\n");
            }

            Console.Clear();

            for (int i = 0; i < bytes.Length; i++)
            {
                Console.ResetColor();

                Char c = default;

                if (i >= BaseStream.Position && i <= BaseStream.Position + (size - 1))
                {
                    c.ShouldBeSelected = true;
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                }

                Console.Write(BitConverter.ToString(new byte[] { bytes[i] }) + " ");
                c.b = FilterByte(bytes[i]);
                chars.Add(c);

                if (((i + 1) % 24) == 0)
                {
                    WriteCharVisuals();
                }
            }

            WriteCharVisuals();

            Console.ResetColor();
        }
#else
        private void UpdateBytes(int size) {}
#endif
        public static void EnableDebugBytes(bool enable) => UseDebugBytes = enable;

        internal CustomBinaryReader(MemoryStream ms)
        {
            br = new BinaryReader(ms);

            bytes = ms.ToArray();

            UpdateBytes(0);
        }

        public byte ReadByte()
        {
            UpdateBytes(1);
            return br.ReadByte();  
        }
        public byte[] ReadBytes(int bytes)
        {
            UpdateBytes(bytes);
            return br.ReadBytes(bytes);
        }

        public int ReadInt32()
        {
            UpdateBytes(4);
            return br.ReadInt32();
        }
        public uint ReadUInt32()
        {
            UpdateBytes(4);
            return br.ReadUInt32();
        }

        public long ReadInt64()
        {
            UpdateBytes(8);
            return br.ReadInt64();
        }
        public ulong ReadUInt64()
        {
            UpdateBytes(8);
            return br.ReadUInt64();
        }

        public bool ReadBoolean()
        {
            UpdateBytes(1);
            return br.ReadBoolean();
        }

        public double ReadDouble()
        {
            UpdateBytes(8);
            return br.ReadDouble();
        }
        public float ReadSingle()
        {
            UpdateBytes(4);
            return br.ReadSingle();
        }
    }
}
