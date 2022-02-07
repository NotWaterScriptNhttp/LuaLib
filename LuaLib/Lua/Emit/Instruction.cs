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

        #region Getters
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
        #region Setters
        private static long SetOpcode(uint inst, OpCodes opcode)
        {
            uint op = (uint)opcode;

            return (inst & MASK0(SIZE_OP, POS_OP)) | ((op << POS_OP) & MASK1(SIZE_OP, POS_OP));
        }
        private static long SetA(uint inst, uint A)
        {
            return (inst & MASK0(SIZE_A, POS_A)) | (A << POS_A) & MASK1(SIZE_A, POS_A);
        }
        private static long SetB(uint inst, uint B)
        {
            return (inst & MASK0(SIZE_B, POS_B)) | (B << POS_B) & MASK1(SIZE_B, POS_B);
        }
        private static long SetC(uint inst, uint C)
        {
            return (inst & MASK0(SIZE_C, POS_C)) | (C << POS_C) & MASK1(SIZE_C, POS_C);
        }
        private static long SetBx(uint inst, uint Bx)
        {
            return (inst & MASK0(SIZE_Bx, POS_Bx)) | (Bx << POS_Bx) & MASK1(SIZE_Bx, POS_Bx);
        }
        private static long SetsBx(uint inst, uint sBx)
        {
            return SetBx(inst, sBx + MAXARG_sBx);
        }
        #endregion
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

        internal byte[] GetInstructionBytes()
        {
            uint inst = 0;

            if (!OpcodeMappings.Mappings.TryGetValue(Opcode, out OpcodeMapping mapping))
                throw new Exception($"Mapping for {Opcode} was not found please add it");

            inst = (uint)SetOpcode(inst, Opcode);

            if (mapping.UsesA)
                inst = (uint)SetA(inst, (uint)A);

            if (mapping.UsesB)
                inst = (uint)SetB(inst, (uint)B);

            if (mapping.UsesC)
                inst = (uint)SetC(inst, (uint)C);

            if (mapping.UsesBx)
                inst = (uint)SetBx(inst, (uint)Bx);

            if (mapping.UsessBx)
                inst = (uint)SetsBx(inst, (uint)sBx);

            return BitConverter.GetBytes(inst);
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

            builder.Append($" UsedRegisters: {registersUsed.Substring(0, registersUsed.Length - 1)},\n");
            builder.Append(inQueue.Substring(0, inQueue.Length - 2) + "\n");
            builder.Append("} - Instruction");

            return builder.ToString();
        }
    }
}
