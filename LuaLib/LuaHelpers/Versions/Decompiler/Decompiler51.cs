using System;
using System.Text;

using LuaLib.Emit;


// This decompiler is not made to be seriously used, and is subject to change


namespace LuaLib.LuaHelpers.Versions.Decompiler
{
    internal class Decompiler51 : DecompilerBase
    {
        public override string Decompile(Emit.Function function, bool IsFunction = false)
        {
            func = function;

            Emit.Function copy = function.Copy();

            StringBuilder sb = new StringBuilder();
            dynamic[] stack = new dynamic[function.maxstacksize];

            if (IsFunction)
            {
                string tmp = "function func" + currentFuncsBx + "(";
                for (int i = 0; i < function.numparams; i++)
                    tmp += "v" + i + (function.numparams - 1 == i ? "" : ", ");
                tmp += ")\n";
                sb.Append(tmp);
            }

            for (int i = 0; i < function.InstructionCount; i++)
            {
                Instruction instruction = function.Instructions[i];
                Registers regs = Instruction.RegistersMap.GetRegister(LuaVersion.LUA_VERSION_5_1, instruction.Opcode);

                int A = instruction.A, B = 0, C = instruction.C;

                #region Getting B

                if (regs.B)
                    B = instruction.B;
                if (regs.Bx)
                    B = instruction.Bx;
                if (regs.sBx)
                    B = instruction.sBx;

                #endregion

                switch (instruction.Opcode)
                {
                    case OpCodes.MOVE:
                        stack[A] = stack[B];

                        SetNop(copy, i);
                        break;

                    case OpCodes.LOADK:
                        stack[A] = GetConstant(B);

                        SetNop(copy, i);
                        break;
                    case OpCodes.LOADBOOL:
                        stack[A] = B == 1;

                        if (C != 0)
                            i++;

                        SetNop(copy, i);
                        break;
                    case OpCodes.LOADNIL:
                        for (int k = A; k < B; k++)
                            stack[k] = "nil";

                        SetNop(copy, i);
                        break;

                    case OpCodes.GETUPVAL:
                        stack[A] = "uv" + B; // Get the upvalue from Function.Upvalues

                        SetNop(copy, i);
                        break;
                    case OpCodes.GETGLOBAL:
                        stack[A] = GetConstant(B);

                        SetNop(copy, i);
                        break;
                    case OpCodes.GETTABLE:
                        stack[A] += "." + RK(C, stack);

                        SetNop(copy, i);
                        break;

                    case OpCodes.SETGLOBAL:
                    case OpCodes.SETUPVAL:
                        break;
                    case OpCodes.SETTABLE:
                        stack[A][RK(B, stack)] = RK(C, stack);

                        SetNop(copy, i);
                        break;

                    case OpCodes.NEWTABLE:
                        stack[A] = new dynamic[C];

                        SetNop(copy, i);
                        break;

                    case OpCodes.SELF:
                        stack[A + 1] = stack[B];
                        stack[A] += ":" + RK(C, stack);

                        SetNop(copy, i);
                        break;

                    case OpCodes.ADD:
                        stack[A] = RK(B, stack) + RK(C, stack);

                        SetNop(copy, i);
                        break;
                    case OpCodes.SUB:
                        stack[A] = RK(B, stack) - RK(C, stack);

                        SetNop(copy, i);
                        break;
                    case OpCodes.MUL:
                        stack[A] = RK(B, stack) * RK(C, stack);

                        SetNop(copy, i);
                        break;
                    case OpCodes.DIV:
                        stack[A] = RK(B, stack) / RK(C, stack);

                        SetNop(copy, i);
                        break;
                    case OpCodes.MOD:
                        stack[A] = RK(B, stack) % RK(C, stack);

                        SetNop(copy, i);
                        break;
                    case OpCodes.POW:
                        stack[A] = Math.Pow(RK(B, stack), RK(C, stack));

                        SetNop(copy, i);
                        break;
                    case OpCodes.UNM:
                        stack[A] = -stack[B];

                        SetNop(copy, i);
                        break;
                    case OpCodes.NOT:
                        stack[A] = $"not {stack[B]}";

                        SetNop(copy, i);
                        break;
                    case OpCodes.LEN:
                        stack[A] = $"#{stack[B]}";

                        SetNop(copy, i);
                        break;

                    case OpCodes.CONCAT:
                        break;

                    case OpCodes.JMP:
                        i += B;
                        break;

                    case OpCodes.EQ:
                    case OpCodes.LT:
                    case OpCodes.LE:

                    case OpCodes.TEST:
                    case OpCodes.TESTSET:

                    case OpCodes.CALL:
                    case OpCodes.TAILCALL:
                    case OpCodes.RETURN:

                    case OpCodes.FORLOOP:
                    case OpCodes.FORPREP:
                    case OpCodes.TFORLOOP:

                    case OpCodes.SETLIST:
                        break;

                    case OpCodes.CLOSE:
                        for (int k = 0; i < stack.Length; i++)
                            if (k >= A)
                                stack[A] = "nil";
                        break;
                    case OpCodes.CLOSURE:

                    case OpCodes.VARARG:
                        break;
                }
            }

            string hold = "";
            for (int i = 0; i < function.InstructionCount; i++)
            {
                Instruction instruction = copy.Instructions[i];

                if (instruction.Opcode == OpCodes.NOP)
                    continue;

                Registers regs = Instruction.RegistersMap.GetRegister(LuaVersion.LUA_VERSION_5_1, instruction.Opcode, false);

                int A = instruction.A, B = 0, C = instruction.C;

                #region Getting B

                if (regs.B)
                    B = instruction.B;
                if (regs.Bx)
                    B = instruction.Bx;
                if (regs.sBx)
                    B = instruction.sBx;

                #endregion

                if (instruction.Opcode != OpCodes.CALL)
                    if (instruction.Opcode != OpCodes.TAILCALL)
                        sb.Append(hold + "\n");

                switch (instruction.Opcode)
                {
                    case OpCodes.NOP:
                        break;

                    case OpCodes.SETGLOBAL: // most likely useless in this decompiler
                        break;
                    case OpCodes.SETUPVAL:
                        sb.Append($"uv{B} = {stack[A]}");
                        break;

                    case OpCodes.NOT:
                    case OpCodes.LEN:
                        break;

                    case OpCodes.CONCAT:
                        {
                            string full = "";

                            for (int k = B; i < C; i++)
                                full += stack[k] + (C - 1 == k ? "" : " .. ");

                            sb.Append(full);
                        }
                        break;

                    case OpCodes.EQ:
                    case OpCodes.LT:
                    case OpCodes.LE:
                        break;

                    case OpCodes.TEST:
                    case OpCodes.TESTSET:
                        break;

                    case OpCodes.CALL:
                    case OpCodes.TAILCALL:
                        {
                            string args = hold;

                            if (B > 0 && args.Length != 0)
                                args += ", ";

                            for (int k = 1; k < B; k++)
                                args += stack[A + k] + (B - 1 == k ? "" : ", ");

                            hold = $"{(instruction.Opcode == OpCodes.TAILCALL ? "return " : "")}{((string)stack[A]).Replace("\"", "")}({args})";
                        }
                        break;
                    case OpCodes.RETURN:
                        {
                            sb.Append("return");
                        }
                        break;

                    case OpCodes.CLOSURE:
                        currentFuncsBx = B;
                        sb.Append(Decompile(function.Functions[B], true));
                        func = function;
                        break;
                }
            }

            return sb.ToString() + (IsFunction ? "\nend" : "");
        }
    }
}
