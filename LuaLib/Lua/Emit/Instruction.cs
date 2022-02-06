using System;
using System.Text;

using LuaLib.Lua.LuaHelpers;

namespace LuaLib.Lua.Emit
{
    public class Instruction
    {
        /*Masks
             bits 0-1: op mode
             bits 2-3: C arg mode
             bits 4-5: B arg mode
             bit 6: instruction set register A
             bit 7: operator is a test
        */
        #region Consts
        private const int SIZE_A = 8;
        private const int SIZE_B = 9;
        private const int SIZE_C = 9;

        private const int SIZE_OP = 6;
        private const int POS_OP = 0;

        private const int POS_A = (POS_OP + SIZE_OP);
        private const int POS_C = (POS_A + SIZE_A);
        private const int POS_B = (POS_C + SIZE_C);
        private const int POS_Bx = POS_C;

        private const int SIZE_Bx = (SIZE_C + SIZE_B);

        private const int MAXARG_Bx = ((1 << SIZE_Bx) - 1);
        private const int MAXARG_sBx = (MAXARG_Bx >> 1);
        #endregion

        public OpCodes Opcode { get; private set; }
        public int A, B, C;
        public int Bx, sBx;

        #region Static methods
        private static bool IsNeg(int num)
        {
            return num + num < num; // -10 + -10 = -20 thus doing Less than -10 will return true
        }

        private static int MASK1(int n, int p)
        {
            return (~((~0) << n)) << p;
        }
        private static int MASK0(int n, int p)
        {
            return (~MASK1(n, p));
        }

        private static long GetOpcode(uint data)
        {
            return (data >> POS_OP) & MASK1(SIZE_OP, 0);
        }
        private static long GetA(uint data)
        {
            return (data >> POS_A) & MASK1(SIZE_A, 0);
        }
        private static long GetB(uint data)
        {
            return (data >> POS_B) & MASK1(SIZE_B, 0);
        }
        private static long GetC(uint data)
        {
            return (data >> POS_C) & MASK1(SIZE_C, 0);
        }
        private static long GetBx(uint data)
        {
            return (data >> POS_Bx) & MASK1(SIZE_Bx, 0);
        }
        private static long GetsBx(uint data)
        {
            return (GetBx(data) - MAXARG_sBx);
        }
        #endregion

        private void UpdateRegisters(uint data)
        {
            if (!OpcodeMappings.Mappings.TryGetValue(Opcode, out OpcodeMapping mapping))
                throw new Exception($"Can't find mapping for {Opcode}");

            if (mapping.UsesA)
                A = (int)GetA(data);

            if (mapping.UsesB)
                B = (int)GetB(data);

            if (mapping.UsesC)
                C = (int)GetC(data);

            if (mapping.UsesBx)
                Bx = (int)GetBx(data);

            if (mapping.UsessBx)
                sBx = (int)GetsBx(data);
        }

        public Instruction(uint data)
        {
            Opcode = (OpCodes)GetOpcode(data);

            UpdateRegisters(data);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            string registersUsed = "";
            string inQueue = "";

            builder.Append("Instruction: {\n");
            builder.Append($" Opcode: {Opcode},\n");

            if (OpcodeMappings.Mappings.TryGetValue(Opcode, out OpcodeMapping mapping))
            {
                if (mapping.UsesA)
                {
                    registersUsed += "A ";
                    inQueue += $" A: {A},\n";
                }
                if (mapping.UsesB)
                {
                    registersUsed += "B ";
                    inQueue += $" B: {B},\n";
                }
                if (mapping.UsesC)
                {
                    registersUsed += "C ";
                    inQueue += $" C: {C},\n";
                }
                if (mapping.UsesBx)
                {
                    registersUsed += "Bx ";
                    inQueue += $" Bx: {Bx},\n";
                }
                if (mapping.UsessBx)
                {
                    registersUsed += "sBx ";
                    inQueue += $" sBx: {sBx},\n";
                }
            }
            else
            {
                if (!IsNeg(A))
                {
                    registersUsed += "A ";
                    inQueue += $" A: {A},\n";
                }
                if (!IsNeg(B))
                {
                    registersUsed += "B ";
                    inQueue += $" B: {B},\n";
                }
                if (!IsNeg(C))
                {
                    registersUsed += "C ";
                    inQueue += $" C: {C},\n";
                }
                if (!IsNeg(Bx))
                {
                    registersUsed += "Bx ";
                    inQueue += $" Bx: {Bx},\n";
                }
                if (!IsNeg(sBx))
                {
                    registersUsed += "sBx ";
                    inQueue += $" sBx: {sBx},\n";
                }
            }

            builder.Append($" UsedRegisters: {registersUsed},\n");
            builder.Append(inQueue.Substring(0, inQueue.Length - 2));
            builder.Append("} - Instruction");

            return builder.ToString();
        }
    }
}
