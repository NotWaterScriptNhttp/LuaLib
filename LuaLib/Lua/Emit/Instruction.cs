using System;
using System.Text;
using System.Collections.Generic;

namespace LuaLib.Lua.Emit
{
    public class Instruction
    {
        private class OpcodeMap
        {
            private static Dictionary<LuaVersion, OpCodes[]> Map = new Dictionary<LuaVersion, OpCodes[]>
            {
                {
                    LuaVersion.LUA_VERSION_5_1,
                    new OpCodes[]
                    {
                        OpCodes.MOVE,

                        OpCodes.LOADK,
                        OpCodes.LOADBOOL,
                        OpCodes.LOADNIL,

                        OpCodes.GETUPVAL,
                        OpCodes.GETGLOBAL,
                        OpCodes.GETTABLE,

                        OpCodes.SETGLOBAL,
                        OpCodes.SETUPVAL,
                        OpCodes.SETTABLE,

                        OpCodes.NEWTABLE,

                        OpCodes.SELF,

                        OpCodes.ADD,
                        OpCodes.SUB,
                        OpCodes.MUL,
                        OpCodes.DIV,
                        OpCodes.MOD,
                        OpCodes.POW,
                        OpCodes.UNM,
                        OpCodes.NOT,
                        OpCodes.LEN,

                        OpCodes.CONCAT,

                        OpCodes.JMP,

                        OpCodes.EQ,
                        OpCodes.LT,
                        OpCodes.LE,

                        OpCodes.TEST,
                        OpCodes.TESTSET,

                        OpCodes.CALL,
                        OpCodes.TAILCALL,
                        OpCodes.RETURN,

                        OpCodes.FORLOOP,
                        OpCodes.FORPREP,
                        OpCodes.TFORLOOP,

                        OpCodes.SETLIST,

                        OpCodes.CLOSE,
                        OpCodes.CLOSURE,

                        OpCodes.VARARG
                    }
                }, // Lua5.1
                {
                    LuaVersion.LUA_VERSION_5_2,
                    new OpCodes[]
                    {
                        OpCodes.MOVE, // 0

                        OpCodes.LOADK, // 1
                        OpCodes.LOADKX, // 2
                        OpCodes.LOADBOOL, // 3
                        OpCodes.LOADNIL, // 4

                        OpCodes.GETUPVAL, // 5
                        OpCodes.GETTABUP, // 6
                        OpCodes.GETTABLE, // 7

                        OpCodes.SETTABUP, // 8
                        OpCodes.SETUPVAL, // 9
                        OpCodes.SETTABLE, // 10

                        OpCodes.NEWTABLE, // 11

                        OpCodes.SELF, // 12

                        OpCodes.ADD, // 13
                        OpCodes.SUB, // 14
                        OpCodes.MUL, // 15
                        OpCodes.DIV, // 16
                        OpCodes.MOD, // 17
                        OpCodes.POW, // 18
                        OpCodes.UNM, // 19
                        OpCodes.NOT, // 20
                        OpCodes.LEN, // 21

                        OpCodes.CONCAT, // 22

                        OpCodes.JMP, // 23

                        OpCodes.EQ, // 24
                        OpCodes.LT, // 25
                        OpCodes.LE, // 26
                        
                        OpCodes.TEST, // 27
                        OpCodes.TESTSET, // 28

                        OpCodes.CALL, // 29
                        OpCodes.TAILCALL, // 30
                        OpCodes.RETURN, // 31

                        OpCodes.FORLOOP, // 32
                        OpCodes.FORPREP, // 33
                        OpCodes.TFORCALL, // 34
                        OpCodes.TFORLOOP, // 35

                        OpCodes.SETLIST, // 36

                        OpCodes.CLOSURE, // 37

                        OpCodes.VARARG, // 38
                        OpCodes.EXTRAARG // 39
                    }
                }, // Lua5.2
                {
                    LuaVersion.LUA_VERSION_5_3,
                    new OpCodes[]
                    {
                        OpCodes.MOVE,

                        OpCodes.LOADK,
                        OpCodes.LOADKX,
                        OpCodes.LOADBOOL,
                        OpCodes.LOADNIL,

                        OpCodes.GETUPVAL,
                        OpCodes.GETTABUP,
                        OpCodes.GETTABLE,

                        OpCodes.SETTABUP,
                        OpCodes.SETUPVAL,
                        OpCodes.SETTABLE,

                        OpCodes.NEWTABLE,

                        OpCodes.SELF,

                        OpCodes.ADD,
                        OpCodes.SUB,
                        OpCodes.MUL,
                        OpCodes.MOD,
                        OpCodes.POW,
                        OpCodes.DIV,
                        OpCodes.IDIV,
                        OpCodes.BAND,
                        OpCodes.BOR,
                        OpCodes.BXOR,
                        OpCodes.SHL,
                        OpCodes.SHR,
                        OpCodes.UNM,
                        OpCodes.BNOT,
                        OpCodes.NOT,
                        OpCodes.LEN,

                        OpCodes.CONCAT,

                        OpCodes.JMP,

                        OpCodes.EQ,
                        OpCodes.LT,
                        OpCodes.LE,

                        OpCodes.TEST,
                        OpCodes.TESTSET,

                        OpCodes.CALL,
                        OpCodes.TAILCALL,
                        OpCodes.RETURN,

                        OpCodes.FORLOOP,
                        OpCodes.FORPREP,
                        OpCodes.TFORCALL,
                        OpCodes.TFORLOOP,

                        OpCodes.SETLIST,

                        OpCodes.CLOSURE,

                        OpCodes.VARARG,
                        OpCodes.EXTRAARG
                    }
                }, // Lua5.3
                {
                    LuaVersion.LUA_VERSION_5_4,
                    new OpCodes[]
                    {
                        OpCodes.MOVE,

                        OpCodes.LOADI,
                        OpCodes.LOADF,
                        OpCodes.LOADK,
                        OpCodes.LOADKX,
                        OpCodes.LOADFALSE,
                        OpCodes.LFALSESKIP,
                        OpCodes.LOADTRUE,
                        OpCodes.LOADNIL,

                        OpCodes.GETUPVAL,
                        OpCodes.SETUPVAL,

                        OpCodes.GETTABUP,
                        OpCodes.GETTABLE,
                        OpCodes.GETI,
                        OpCodes.GETFIELD,

                        OpCodes.SETTABUP,
                        OpCodes.SETTABLE,
                        OpCodes.SETI,
                        OpCodes.SETFIELD,

                        OpCodes.NEWTABLE,

                        OpCodes.SELF,

                        OpCodes.ADDI,
                        OpCodes.ADDK,
                        OpCodes.SUBK,
                        OpCodes.MULK,
                        OpCodes.MODK,
                        OpCodes.POWK,
                        OpCodes.DIVK,
                        OpCodes.IDIVK,

                        OpCodes.BANDK,
                        OpCodes.BORK,
                        OpCodes.BXORK,
                        OpCodes.SHRI,
                        OpCodes.SHLI,

                        OpCodes.ADD,
                        OpCodes.SUB,
                        OpCodes.MUL,
                        OpCodes.MOD,
                        OpCodes.POW,
                        OpCodes.DIV,
                        OpCodes.IDIV,

                        OpCodes.BAND,
                        OpCodes.BOR,
                        OpCodes.BXOR,
                        OpCodes.SHL,
                        OpCodes.SHR,

                        OpCodes.MMBIN,
                        OpCodes.MMBINI,
                        OpCodes.MMBINK,

                        OpCodes.UNM,
                        OpCodes.BNOT,
                        OpCodes.NOT,
                        OpCodes.LEN,

                        OpCodes.CONCAT,

                        OpCodes.CLOSE,
                        OpCodes.TBC,
                        OpCodes.JMP,

                        OpCodes.EQ,
                        OpCodes.LT,
                        OpCodes.LE,

                        OpCodes.EQK,
                        OpCodes.EQI,
                        OpCodes.LTI,
                        OpCodes.LEI,
                        OpCodes.GTI,
                        OpCodes.GEI,

                        OpCodes.TEST,
                        OpCodes.TESTSET,

                        OpCodes.CALL,
                        OpCodes.TAILCALL,

                        OpCodes.RETURN,
                        OpCodes.RETURN0,
                        OpCodes.RETURN1,

                        OpCodes.FORLOOP,
                        OpCodes.FORPREP,
                        OpCodes.TFORPREP,
                        OpCodes.TFORCALL,
                        OpCodes.TFORLOOP,

                        OpCodes.SETLIST,

                        OpCodes.CLOSURE,

                        OpCodes.VARARG,
                        OpCodes.VARARGPREP,
                        OpCodes.EXTRAARG
                    }
                }, // Lua5.4
            };

            private OpcodeMap() {}

            public static OpCodes GetOpcodeFromMap(LuaVersion version, int position)
            {
                if (!Map.TryGetValue(version, out OpCodes[] opcodes))
                    throw new Exception($"This lua version do not have a map");

                if (opcodes.Length >= position)
                    return opcodes[position];

                throw new Exception($"Cannot find an opcode for '{position}'");
            }
            public static int GetOpcodeNumber(LuaVersion version, OpCodes opcode)
            {
                if (!Map.TryGetValue(version, out OpCodes[] opcodes))
                    throw new Exception($"This lua version do not have a map");

                for (int i = 0; i < opcodes.Length; i++)
                    if (opcode == opcodes[i])
                        return i;

                throw new Exception($"Cannot find number for '{opcode}'");
            }
        }
        private struct Registers
        {
            public OpCodes opcode;

            public bool A, B, C, k;
            public bool Ax, Bx;
            public bool sB, sC, sBx, sJ;
        }
        private class RegistersMap
        {
            private static Dictionary<LuaVersion, Registers[]> Map = new Dictionary<LuaVersion, Registers[]>
            {
                {
                    LuaVersion.LUA_VERSION_5_1,
                    new Registers[]
                    {
                        new Registers
                        {
                            opcode = OpCodes.MOVE,

                            A = true,
                            B = true
                        }, // MOVE

                        new Registers
                        {
                            opcode = OpCodes.LOADK,

                            A = true,
                            Bx = true
                        }, // LOADK
                        new Registers
                        {
                            opcode = OpCodes.LOADBOOL,

                            A = true,
                            B = true,
                            C = true
                        }, // LOADBOOL
                        new Registers
                        {
                            opcode = OpCodes.LOADNIL,

                            A = true,
                            B = true
                        }, // LOADNIL

                        new Registers
                        {
                            opcode = OpCodes.GETUPVAL,

                            A = true,
                            B = true
                        }, // GETUPVAL
                        new Registers
                        {
                            opcode = OpCodes.GETGLOBAL,

                            A = true,
                            Bx = true
                        }, // GETGLOBAL
                        new Registers
                        {
                            opcode = OpCodes.GETTABLE,

                            A = true,
                            B = true,
                            C = true
                        }, // GETTABLE

                        new Registers
                        {
                            opcode = OpCodes.SETGLOBAL,

                            A = true,
                            Bx = true
                        }, // SETGLOBAL
                        new Registers
                        {
                            opcode = OpCodes.SETUPVAL,

                            A = true,
                            B = true
                        }, // SETUPVAL
                        new Registers
                        {
                            opcode = OpCodes.SETTABLE,

                            A = true,
                            B = true,
                            C = true
                        }, // SETTABLE

                        new Registers
                        {
                            opcode = OpCodes.NEWTABLE,

                            A = true,
                            B = true,
                            C = true
                        }, // NEWTABLE
                        new Registers
                        {
                            opcode = OpCodes.SELF,

                            A = true,
                            B = true,
                            C = true
                        }, // SELF

                        new Registers
                        {
                            opcode = OpCodes.ADD,

                            A = true,
                            B = true,
                            C = true
                        }, // ADD
                        new Registers
                        {
                            opcode = OpCodes.SUB,

                            A = true,
                            B = true,
                            C = true
                        }, // SUB
                        new Registers
                        {
                            opcode = OpCodes.MUL,

                            A = true,
                            B = true,
                            C = true
                        }, // MUL
                        new Registers
                        {
                            opcode = OpCodes.DIV,

                            A = true,
                            B = true,
                            C = true
                        }, // DIV
                        new Registers
                        {
                            opcode = OpCodes.MOD,

                            A = true,
                            B = true,
                            C = true
                        }, // MOD
                        new Registers
                        {
                            opcode = OpCodes.POW,

                            A = true,
                            B = true,
                            C = true
                        }, // POW
                        new Registers
                        {
                            opcode = OpCodes.UNM,

                            A = true,
                            B = true
                        }, // UNM
                        new Registers
                        {
                            opcode = OpCodes.NOT,

                            A = true,
                            B = true
                        }, // NOT
                        new Registers
                        {
                            opcode = OpCodes.LEN,

                            A = true,
                            B = true
                        }, // LEN

                        new Registers
                        {
                            opcode = OpCodes.CONCAT,

                            A = true,
                            B = true,
                            C = true
                        }, // CONCAT

                        new Registers
                        {
                            opcode = OpCodes.JMP,

                            sBx = true
                        }, // JMP

                        new Registers
                        {
                            opcode = OpCodes.EQ,

                            A = true,
                            B = true,
                            C = true
                        }, // EQ
                        new Registers
                        {
                            opcode = OpCodes.LT,

                            A = true,
                            B = true,
                            C = true
                        }, // LT
                        new Registers
                        {
                            opcode = OpCodes.LE,

                            A = true,
                            B = true,
                            C = true
                        }, // LE

                        new Registers
                        {
                            opcode = OpCodes.TEST,

                            A = true,
                            C = true
                        }, // TEST
                        new Registers
                        {
                            opcode = OpCodes.TESTSET,

                            A = true,
                            B = true,
                            C = true
                        }, // TESTSET

                        new Registers
                        {
                            opcode = OpCodes.CALL,

                            A = true,
                            B = true,
                            C = true
                        }, // CALL
                        new Registers
                        {
                            opcode = OpCodes.TAILCALL,

                            A = true,
                            B = true,
                            C = true
                        }, // TAILCALL
                        new Registers
                        {
                            opcode = OpCodes.RETURN,

                            A = true,
                            B = true
                        }, // RETURN

                        new Registers
                        {
                            opcode = OpCodes.FORLOOP,

                            A = true,
                            sBx = true
                        }, // FORLOOP
                        new Registers
                        {
                            opcode = OpCodes.FORPREP,

                            A = true,
                            sBx = true
                        }, // FORPREP
                        new Registers
                        {
                            opcode = OpCodes.TFORLOOP,

                            A = true,
                            C = true
                        }, // TFORLOOP

                        new Registers
                        {
                            opcode = OpCodes.SETLIST,

                            A = true,
                            B = true,
                            C = true
                        }, // SETLIST

                        new Registers
                        {
                            opcode = OpCodes.CLOSE,

                            A = true
                        }, // CLOSE
                        new Registers
                        {
                            opcode = OpCodes.CLOSURE,

                            A = true,
                            Bx = true
                        }, // CLOSURE

                        new Registers
                        {
                            opcode = OpCodes.VARARG,

                            A = true,
                            B = true
                        } // VARARG
                    }
                }, // Lua5.1
                {
                    LuaVersion.LUA_VERSION_5_2,
                    new Registers[]
                    {
                        new Registers
                        {
                            opcode = OpCodes.MOVE,

                            A = true,
                            B = true
                        }, // MOVE

                        new Registers
                        {
                            opcode = OpCodes.LOADK,

                            A = true,
                            Bx = true
                        }, // LOADK
                        new Registers
                        {
                            opcode = OpCodes.LOADKX,

                            A = true
                        }, // LOADKX
                        new Registers
                        {
                            opcode = OpCodes.LOADBOOL,

                            A = true,
                            B = true,
                            C = true
                        }, // LOADBOOL
                        new Registers
                        {
                            opcode = OpCodes.LOADNIL,

                            A = true,
                            B = true
                        }, // LOADNIL

                        new Registers
                        {
                            opcode = OpCodes.GETUPVAL,

                            A = true,
                            B = true
                        }, // GETUPVAL
                        new Registers
                        {
                            opcode = OpCodes.GETTABUP,

                            A = true,
                            B = true,
                            C = true
                        }, // GETTABUP
                        new Registers
                        {
                            opcode = OpCodes.GETTABLE,

                            A = true,
                            B = true,
                            C = true
                        }, // GETTABLE

                        new Registers
                        {
                            opcode = OpCodes.SETTABUP,

                            A = true,
                            B = true,
                            C = true
                        }, // SETTABUP
                        new Registers
                        {
                            opcode = OpCodes.SETUPVAL,

                            A = true,
                            B = true
                        }, // SETUPVAL
                        new Registers
                        {
                            opcode = OpCodes.SETTABLE,

                            A = true,
                            B = true,
                            C = true
                        }, // SETTABLE

                        new Registers
                        {
                            opcode = OpCodes.NEWTABLE,

                            A = true,
                            B = true,
                            C = true
                        }, // NEWTABLE
                        new Registers
                        {
                            opcode = OpCodes.SELF,

                            A = true,
                            B = true,
                            C = true
                        }, // SELF

                        new Registers
                        {
                            opcode = OpCodes.ADD,

                            A = true,
                            B = true,
                            C = true
                        }, // ADD
                        new Registers
                        {
                            opcode = OpCodes.SUB,

                            A = true,
                            B = true,
                            C = true
                        }, // SUB
                        new Registers
                        {
                            opcode = OpCodes.MUL,

                            A = true,
                            B = true,
                            C = true
                        }, // MUL
                        new Registers
                        {
                            opcode = OpCodes.DIV,

                            A = true,
                            B = true,
                            C = true
                        }, // DIV
                        new Registers
                        {
                            opcode = OpCodes.MOD,

                            A = true,
                            B = true,
                            C = true
                        }, // MOD
                        new Registers
                        {
                            opcode = OpCodes.POW,

                            A = true,
                            B = true,
                            C = true
                        }, // POW
                        new Registers
                        {
                            opcode = OpCodes.UNM,

                            A = true,
                            B = true
                        }, // UNM
                        new Registers
                        {
                            opcode = OpCodes.NOT,

                            A = true,
                            B = true
                        }, // NOT
                        new Registers
                        {
                            opcode = OpCodes.LEN,

                            A = true,
                            B = true
                        }, // LEN

                        new Registers
                        {
                            opcode = OpCodes.CONCAT,

                            A = true,
                            B = true,
                            C = true
                        }, // CONCAT

                        new Registers
                        {
                            opcode = OpCodes.JMP,

                            A = true,
                            sBx = true
                        }, // JMP

                        new Registers
                        {
                            opcode = OpCodes.EQ,

                            A = true,
                            B = true,
                            C = true
                        }, // EQ
                        new Registers
                        {
                            opcode = OpCodes.LT,

                            A = true,
                            B = true,
                            C = true
                        }, // LT
                        new Registers
                        {
                            opcode = OpCodes.LE,

                            A = true,
                            B = true,
                            C = true
                        }, // LE

                        new Registers
                        {
                            opcode = OpCodes.TEST,

                            A = true,
                            C = true
                        }, // TEST
                        new Registers
                        {
                            opcode = OpCodes.TESTSET,

                            A = true,
                            B = true,
                            C = true
                        }, // TESTSET

                        new Registers
                        {
                            opcode = OpCodes.CALL,

                            A = true,
                            B = true,
                            C = true
                        }, // CALL
                        new Registers
                        {
                            opcode = OpCodes.TAILCALL,

                            A = true,
                            B = true,
                            C = true
                        }, // TAILCALL
                        new Registers
                        {
                            opcode = OpCodes.RETURN,

                            A = true,
                            B = true
                        }, // RETURN

                        new Registers
                        {
                            opcode = OpCodes.FORLOOP,

                            A = true,
                            sBx = true
                        }, // FORLOOP
                        new Registers
                        {
                            opcode = OpCodes.FORPREP,

                            A = true,
                            sBx = true
                        }, // FORPREP
                        new Registers
                        {
                            opcode = OpCodes.TFORCALL,

                            A = true,
                            C = true
                        }, // TFORCALL
                        new Registers
                        {
                            opcode = OpCodes.TFORLOOP,

                            A = true,
                            sBx = true
                        }, // TFORLOOP

                        new Registers
                        {
                            opcode = OpCodes.SETLIST,

                            A = true,
                            B = true,
                            C = true
                        }, // SETLIST

                        new Registers
                        {
                            opcode = OpCodes.CLOSURE,

                            A = true,
                            Bx = true
                        }, // CLOSURE

                        new Registers
                        {
                            opcode = OpCodes.VARARG,

                            A = true,
                            B = true
                        }, // VARARG
                        new Registers
                        {
                            opcode = OpCodes.EXTRAARG,

                            Ax = true
                        } // EXTRAARG
                    }
                }, // Lua5.2
                {
                    LuaVersion.LUA_VERSION_5_3,
                    new Registers[]
                    {
                        new Registers
                        {
                            opcode = OpCodes.MOVE,

                            A = true,
                            B = true
                        }, // MOVE

                        new Registers
                        {
                            opcode = OpCodes.LOADK,

                            A = true,
                            Bx = true
                        }, // LOADK
                        new Registers
                        {
                            opcode = OpCodes.LOADKX,

                            A = true
                        }, // LOADKX
                        new Registers
                        {
                            opcode = OpCodes.LOADBOOL,

                            A = true,
                            B = true,
                            C = true
                        }, // LOADBOOL
                        new Registers
                        {
                            opcode = OpCodes.LOADNIL,

                            A = true,
                            B = true
                        }, // LOADNIL

                        new Registers
                        {
                            opcode = OpCodes.GETUPVAL,

                            A = true,
                            B = true
                        }, // GETUPVAL
                        new Registers
                        {
                            opcode = OpCodes.GETTABUP,

                            A = true,
                            B = true,
                            C = true
                        }, // GETTABUP
                        new Registers
                        {
                            opcode = OpCodes.GETTABLE,

                            A = true,
                            B = true,
                            C = true
                        }, // GETTABLE

                        new Registers
                        {
                            opcode = OpCodes.SETTABUP,

                            A = true,
                            B = true,
                            C = true
                        }, // SETTABUP
                        new Registers
                        {
                            opcode = OpCodes.SETUPVAL,

                            A = true,
                            B = true
                        }, // SETUPVAL
                        new Registers
                        {
                            opcode = OpCodes.SETTABLE,

                            A = true,
                            B = true,
                            C = true
                        }, // SETTABLE

                        new Registers
                        {
                            opcode = OpCodes.NEWTABLE,

                            A = true,
                            B = true,
                            C = true
                        }, // NEWTABLE
                        new Registers
                        {
                            opcode = OpCodes.SELF,

                            A = true,
                            B = true,
                            C = true
                        }, // SELF

                        new Registers
                        {
                            opcode = OpCodes.ADD,

                            A = true,
                            B = true,
                            C = true
                        }, // ADD
                        new Registers
                        {
                            opcode = OpCodes.SUB,

                            A = true,
                            B = true,
                            C = true
                        }, // SUB
                        new Registers
                        {
                            opcode = OpCodes.MUL,

                            A = true,
                            B = true,
                            C = true
                        }, // MUL
                        new Registers
                        {
                            opcode = OpCodes.MOD,

                            A = true,
                            B = true,
                            C = true
                        }, // MOD
                        new Registers
                        {
                            opcode = OpCodes.POW,

                            A = true,
                            B = true,
                            C = true
                        }, // POW
                        new Registers
                        {
                            opcode = OpCodes.DIV,

                            A = true,
                            B = true,
                            C = true
                        }, // DIV
                        new Registers
                        {
                            opcode = OpCodes.IDIV,

                            A = true,
                            B = true,
                            C = true
                        }, // IDIV
                        new Registers
                        {
                            opcode = OpCodes.BAND,

                            A = true,
                            B = true,
                            C = true
                        }, // BAND
                        new Registers
                        {
                            opcode = OpCodes.BOR,

                            A = true,
                            B = true,
                            C = true
                        }, // BOR
                        new Registers
                        {
                            opcode = OpCodes.BXOR,

                            A = true,
                            B = true,
                            C = true
                        }, // BXOR
                        new Registers
                        {
                            opcode = OpCodes.SHL,

                            A = true,
                            B = true,
                            C = true
                        }, // SHL
                        new Registers
                        {
                            opcode = OpCodes.SHR,

                            A = true,
                            B = true,
                            C = true
                        }, // SHR
                        new Registers
                        {
                            opcode = OpCodes.UNM,

                            A = true,
                            B = true
                        }, // UNM
                        new Registers
                        {
                            opcode = OpCodes.BNOT,

                            A = true,
                            B = true
                        }, // BNOT
                        new Registers
                        {
                            opcode = OpCodes.NOT,

                            A = true,
                            B = true
                        }, // NOT
                        new Registers
                        {
                            opcode = OpCodes.LEN,

                            A = true,
                            B = true
                        }, // LEN

                        new Registers
                        {
                            opcode = OpCodes.CONCAT,

                            A = true,
                            B = true,
                            C = true
                        }, // CONCAT

                        new Registers
                        {
                            opcode = OpCodes.JMP,

                            A = true,
                            sBx = true
                        }, // JMP

                        new Registers
                        {
                            opcode = OpCodes.EQ,

                            A = true,
                            B = true,
                            C = true
                        }, // EQ
                        new Registers
                        {
                            opcode = OpCodes.LT,

                            A = true,
                            B = true,
                            C = true
                        }, // LT
                        new Registers
                        {
                            opcode = OpCodes.LE,

                            A = true,
                            B = true,
                            C = true
                        }, // LE

                        new Registers
                        {
                            opcode = OpCodes.TEST,

                            A = true,
                            C = true
                        }, // TEST
                        new Registers
                        {
                            opcode = OpCodes.TESTSET,

                            A = true,
                            B = true,
                            C = true
                        }, // TESTSET

                        new Registers
                        {
                            opcode = OpCodes.CALL,

                            A = true,
                            B = true,
                            C = true
                        }, // CALL
                        new Registers
                        {
                            opcode = OpCodes.TAILCALL,

                            A = true,
                            B = true,
                            C = true
                        }, // TAILCALL
                        new Registers
                        {
                            opcode = OpCodes.RETURN,

                            A = true,
                            B = true
                        }, // RETURN

                        new Registers
                        {
                            opcode = OpCodes.FORLOOP,

                            A = true,
                            sBx = true
                        }, // FORLOOP
                        new Registers
                        {
                            opcode = OpCodes.FORPREP,

                            A = true,
                            sBx = true
                        }, // FORPREP
                        new Registers
                        {
                            opcode = OpCodes.TFORCALL,

                            A = true,
                            C = true
                        }, // TFORCALL
                        new Registers
                        {
                            opcode = OpCodes.TFORLOOP,

                            A = true,
                            sBx = true
                        }, // TFORLOOP

                        new Registers
                        {
                            opcode = OpCodes.SETLIST,

                            A = true,
                            B = true,
                            C = true
                        }, // SETLIST

                        new Registers
                        {
                            opcode = OpCodes.CLOSURE,

                            A = true,
                            Bx = true
                        }, // CLOSURE

                        new Registers
                        {
                            opcode = OpCodes.VARARG,

                            A = true,
                            B = true
                        }, // VARARG
                        new Registers
                        {
                            opcode = OpCodes.EXTRAARG,

                            Ax = true
                        }, // EXTRAARG
                    }
                }, // Lua5.3
                {
                    LuaVersion.LUA_VERSION_5_4,
                    new Registers[]
                    {
                        new Registers
                        {
                            opcode = OpCodes.MOVE,

                            A = true,
                            B = true
                        }, // MOVE

                        new Registers
                        {
                            opcode = OpCodes.LOADI,

                            A = true,
                            sBx = true
                        }, // LOADI
                        new Registers
                        {
                            opcode = OpCodes.LOADF,

                            A = true,
                            sBx = true
                        }, // LOADF
                        new Registers
                        {
                            opcode = OpCodes.LOADK,

                            A = true,
                            Bx = true
                        }, // LOADK
                        new Registers
                        {
                            opcode = OpCodes.LOADKX,

                            A = true
                        }, // LOADKX
                        new Registers
                        {
                            opcode = OpCodes.LOADFALSE,

                            A = true
                        }, // LOADFALSE
                        new Registers
                        {
                            opcode = OpCodes.LFALSESKIP,

                            A = true
                        }, // LFALSESKIP
                        new Registers
                        {
                            opcode = OpCodes.LOADTRUE,

                            A = true
                        }, // LOADTRUE
                        new Registers
                        {
                            opcode = OpCodes.LOADNIL,

                            A = true,
                            B = true
                        }, // LOADNIL

                        new Registers
                        {
                            opcode = OpCodes.GETUPVAL,

                            A = true,
                            B = true
                        }, // GETUPVAL
                        new Registers
                        {
                            opcode = OpCodes.SETUPVAL,

                            A = true,
                            B = true
                        }, // SETUPVAL

                        new Registers
                        {
                            opcode = OpCodes.GETTABUP,

                            A = true,
                            B = true,
                            C = true
                        }, // GETTABUP
                        new Registers
                        {
                            opcode = OpCodes.GETTABLE,

                            A = true,
                            B = true,
                            C = true
                        }, // GETTABLE
                        new Registers
                        {
                            opcode = OpCodes.GETI,

                            A = true,
                            B = true,
                            C = true
                        }, // GETI
                        new Registers
                        {
                            opcode = OpCodes.GETFIELD,

                            A = true,
                            B = true,
                            C = true
                        }, // GETFIELD

                        new Registers
                        {
                            opcode = OpCodes.SETTABUP,

                            A = true,
                            B = true,
                            C = true
                        }, // SETTABUP
                        new Registers
                        {
                            opcode = OpCodes.SETTABLE,

                            A = true,
                            B = true,
                            C = true
                        }, // SETTABLE
                        new Registers
                        {
                            opcode = OpCodes.SETI,

                            A = true,
                            B = true,
                            C = true
                        }, // SETI
                        new Registers
                        {
                            opcode = OpCodes.SETFIELD,

                            A = true,
                            B = true,
                            C = true
                        }, // SETFIELD

                        new Registers
                        {
                            opcode = OpCodes.NEWTABLE,

                            A = true,
                            B = true,
                            C = true,
                            k = true
                        }, // NEWTABLE
                        new Registers
                        {
                            opcode = OpCodes.SELF,

                            A = true,
                            B = true,
                            C = true
                        }, // SELF

                        new Registers
                        {
                            opcode = OpCodes.ADDI,

                            A = true,
                            B = true,
                            sC = true
                        }, // ADDI
                        new Registers
                        {
                            opcode = OpCodes.ADDK,

                            A = true,
                            B = true,
                            C = true
                        }, // ADDK
                        new Registers
                        {
                            opcode = OpCodes.SUBK,

                            A = true,
                            B = true,
                            C = true
                        }, // SUBK
                        new Registers
                        {
                            opcode = OpCodes.MULK,

                            A = true,
                            B = true,
                            C = true
                        }, // MULK
                        new Registers
                        {
                            opcode = OpCodes.MODK,

                            A = true,
                            B = true,
                            C = true
                        }, // MODK
                        new Registers
                        {
                            opcode = OpCodes.POWK,

                            A = true,
                            B = true,
                            C = true
                        }, // POWK
                        new Registers
                        {
                            opcode = OpCodes.DIVK,

                            A = true,
                            B = true,
                            C = true
                        }, // DIVK
                        new Registers
                        {
                            opcode = OpCodes.IDIVK,

                            A = true,
                            B = true,
                            C = true
                        }, // IDIVK

                        new Registers
                        {
                            opcode = OpCodes.BANDK,

                            A = true,
                            B = true,
                            C = true
                        }, // BANDK
                        new Registers
                        {
                            opcode = OpCodes.BORK,

                            A = true,
                            B = true,
                            C = true
                        }, // BORK
                        new Registers
                        {
                            opcode = OpCodes.BXORK,

                            A = true,
                            B = true,
                            C = true
                        }, // BXORK
                        new Registers
                        {
                            opcode = OpCodes.SHRI,

                            A = true,
                            B = true,
                            sC = true
                        }, // SHRI
                        new Registers
                        {
                            opcode = OpCodes.SHLI,

                            A = true,
                            B = true,
                            sC = true
                        }, // SHLI

                        new Registers
                        {
                            opcode = OpCodes.ADD,

                            A = true,
                            B = true,
                            C = true
                        }, // ADD
                        new Registers
                        {
                            opcode = OpCodes.SUB,

                            A = true,
                            B = true,
                            C = true
                        }, // SUB
                        new Registers
                        {
                            opcode = OpCodes.MUL,

                            A = true,
                            B = true,
                            C = true
                        }, // MUL
                        new Registers
                        {
                            opcode = OpCodes.MOD,

                            A = true,
                            B = true,
                            C = true
                        }, // MOD
                        new Registers
                        {
                            opcode = OpCodes.POW,

                            A = true,
                            B = true,
                            C = true
                        }, // POW
                        new Registers
                        {
                            opcode = OpCodes.DIV,

                            A = true,
                            B = true,
                            C = true
                        }, // DIV
                        new Registers
                        {
                            opcode = OpCodes.IDIV,

                            A = true,
                            B = true,
                            C = true
                        }, // IDIV

                        new Registers
                        {
                            opcode = OpCodes.BAND,

                            A = true,
                            B = true,
                            C = true
                        }, // BAND
                        new Registers
                        {
                            opcode = OpCodes.BOR,

                            A = true,
                            B = true,
                            C = true
                        }, // BOR
                        new Registers
                        {
                            opcode = OpCodes.BXOR,

                            A = true,
                            B = true,
                            C = true
                        }, // BXOR
                        new Registers
                        {
                            opcode = OpCodes.SHL,

                            A = true,
                            B = true,
                            C = true
                        }, // SHL
                        new Registers
                        {
                            opcode = OpCodes.SHR,

                            A = true,
                            B = true,
                            C = true
                        }, // SHR

                        new Registers
                        {
                            opcode = OpCodes.MMBIN,

                            A = true,
                            B = true,
                            C = true
                        }, // MMBIN
                        new Registers
                        {
                            opcode = OpCodes.MMBINI,

                            A = true,
                            sB = true,
                            C = true,
                            k = true
                        }, // MMBINI
                        new Registers
                        {
                            opcode = OpCodes.MMBINK,

                            A = true,
                            B = true,
                            C = true,
                            k = true
                        }, // MMBINK

                        new Registers
                        {
                            opcode = OpCodes.UNM,

                            A = true,
                            B = true
                        }, // UNM
                        new Registers
                        {
                            opcode = OpCodes.BNOT,

                            A = true,
                            B = true
                        }, // BNOT
                        new Registers
                        {
                            opcode = OpCodes.NOT,

                            A = true,
                            B = true
                        }, // NOT
                        new Registers
                        {
                            opcode = OpCodes.LEN,

                            A = true,
                            B = true
                        }, // LEN

                        new Registers
                        {
                            opcode = OpCodes.CONCAT,

                            A = true,
                            B = true
                        }, // CONCAT

                        new Registers
                        {
                            opcode = OpCodes.CLOSE,

                            A = true
                        }, // CLOSE
                        new Registers
                        {
                            opcode = OpCodes.TBC,

                            A = true
                        }, // TBC
                        new Registers
                        {
                            opcode = OpCodes.JMP,

                            sJ = true
                        }, // JMP

                        new Registers
                        {
                            opcode = OpCodes.EQ,

                            A = true,
                            B = true,
                            k = true
                        }, // EQ
                        new Registers
                        {
                            opcode = OpCodes.LT,

                            A = true,
                            B = true,
                            k = true
                        }, // LT
                        new Registers
                        {
                            opcode = OpCodes.LE,

                            A = true,
                            B = true,
                            k = true
                        }, // LE

                        new Registers
                        {
                            opcode = OpCodes.EQK,

                            A = true,
                            B = true,
                            k = true
                        }, // EQK
                        new Registers
                        {
                            opcode = OpCodes.EQI,

                            A = true,
                            sB = true,
                            k = true
                        }, // EQI
                        new Registers
                        {
                            opcode = OpCodes.LTI,

                            A = true,
                            sB = true,
                            k = true
                        }, // LTI
                        new Registers
                        {
                            opcode = OpCodes.LEI,

                            A = true,
                            sB = true,
                            k = true
                        }, // LEI
                        new Registers
                        {
                            opcode = OpCodes.GTI,

                            A = true,
                            sB = true,
                            k = true
                        }, // GTI
                        new Registers
                        {
                            opcode = OpCodes.GEI,

                            A = true,
                            sB = true,
                            k = true
                        }, // GEI

                        new Registers
                        {
                            opcode = OpCodes.TEST,

                            A = true,
                            k = true
                        }, // TEST
                        new Registers
                        {
                            opcode = OpCodes.TESTSET,

                            A = true,
                            B = true,
                            k = true
                        }, // TESTSET

                        new Registers
                        {
                            opcode = OpCodes.CALL,

                            A = true,
                            B = true,
                            C = true
                        }, // CALL
                        new Registers
                        {
                            opcode = OpCodes.TAILCALL,

                            A = true,
                            B = true,
                            C = true,
                            k = true
                        }, // TAILCALL

                        new Registers
                        {
                            opcode = OpCodes.RETURN,

                            A = true,
                            B = true,
                            C = true,
                            k = true
                        }, // RETURN
                        new Registers
                        {
                            opcode = OpCodes.RETURN0,
                        }, // RETURN0
                        new Registers
                        {
                            opcode = OpCodes.RETURN1,

                            A = true
                        }, // RETURN1

                        new Registers
                        {
                            opcode = OpCodes.FORLOOP,

                            A = true,
                            Bx = true
                        }, // FORLOOP
                        new Registers
                        {
                            opcode = OpCodes.FORPREP,

                            A = true,
                            Bx = true
                        }, // FORPREP
                        new Registers
                        {
                            opcode = OpCodes.TFORPREP,

                            A = true,
                            Bx = true
                        }, // TFORPREP
                        new Registers
                        {
                            opcode = OpCodes.TFORCALL,

                            A = true,
                            C = true
                        }, // TFORCALL
                        new Registers
                        {
                            opcode = OpCodes.TFORLOOP,

                            A = true,
                            Bx = true
                        }, // TFORLOOP

                        new Registers
                        {
                            opcode = OpCodes.SETLIST,

                            A = true,
                            B = true,
                            C = true,
                            k = true
                        }, // SETLIST

                        new Registers
                        {
                            opcode = OpCodes.CLOSURE,

                            A = true,
                            Bx = true
                        }, // CLOSURE

                        new Registers
                        {
                            opcode = OpCodes.VARARG,

                            A = true,
                            C = true
                        }, // VARARG
                        new Registers
                        {
                            opcode = OpCodes.VARARGPREP,

                            A = true
                        }, // VARARGPREP
                        new Registers
                        {
                            opcode = OpCodes.EXTRAARG,

                            Ax = true
                        }, // EXTRAARG
                    }
                }, // Lua5.4
            };

            public static Registers GetRegister(LuaVersion version, OpCodes opcode, bool throwError = true)
            {
                if (!Map.TryGetValue(version, out Registers[] registers))
                    if (throwError)
                        throw new Exception($"No registers were found for version ({version})");
                    else return new Registers
                        {
                            opcode = OpCodes.INVALID
                        };

                foreach (Registers register in registers)
                    if (register.opcode == opcode)
                        return register;

                if (throwError)
                    throw new Exception($"Didn't find registers for '{opcode}'");

                return new Registers
                {
                    opcode = OpCodes.INVALID
                };
            }
        }

        #region Consts
        private abstract class ConstsBase
        {
            abstract public int SIZE_OP { get; protected set; }
            abstract public int SIZE_A { get; protected set; }
            abstract public int SIZE_B { get; protected set; }
            abstract public int SIZE_C { get; protected set; }
            abstract public int SIZE_Bx { get; protected set; }

            abstract public int POS_OP { get; protected set; }
            abstract public int POS_A { get; protected set; }
            abstract public int POS_B { get; protected set; }
            abstract public int POS_C { get; protected set; }
            abstract public int POS_Bx { get; protected set; }

            virtual public int SIZE_Ax { get; protected set; } = -1;
            virtual public int SIZE_sJ { get; protected set; } = -1;

            virtual public int POS_Ax { get; protected set; } = -1;
            virtual public int POS_sJ { get; protected set; } = -1;
            virtual public int POS_k { get; protected set; } = -1;

            public int MAXARG_A = -1;
            public int MAXARG_B = -1;
            public int MAXARG_C = -1;
            public int MAXARG_Ax = -1;
            public int MAXARG_Bx = -1;
            public int MAXARG_sBx = -1;
            public int MAXARG_sJ = -1;

            public int OFFSET_sBx;
            public int OFFSET_sJ;
            public int OFFSET_sC;

            public void CalcMaxargs()
            {
                MAXARG_A = (1 << SIZE_A) - 1;
                MAXARG_B = (1 << SIZE_B) - 1;
                MAXARG_C = (1 << SIZE_C) - 1;
                MAXARG_Bx = (1 << SIZE_Bx) - 1;
                MAXARG_sBx = (MAXARG_Bx >> 1);

                if (SIZE_Ax != -1)
                    MAXARG_Ax = (1 << SIZE_Ax) - 1;

                if (SIZE_sJ != -1)
                    MAXARG_sJ = (1 << SIZE_sJ) - 1;
            }
            public void CalcOffsets()
            {
                if (MAXARG_Bx != -1)
                    OFFSET_sBx = MAXARG_Bx >> 1;
                if (MAXARG_sJ != -1)
                    OFFSET_sJ = MAXARG_sJ >> 1;
                if (MAXARG_C != -1)
                    OFFSET_sC = MAXARG_C >> 1;
            }
        }

        private class Consts51 : ConstsBase
        {
            public override int SIZE_OP { get; protected set; } = 6;
            public override int SIZE_A { get; protected set; } = 8;
            public override int SIZE_B { get; protected set; } = 9;
            public override int SIZE_C { get; protected set; } = 9;
            public override int SIZE_Bx { get; protected set; } = 18;

            public override int POS_OP { get; protected set; } = 0;
            public override int POS_A { get; protected set; } = 6;
            public override int POS_B { get; protected set; } = 23;
            public override int POS_C { get; protected set; } = 14;
            public override int POS_Bx { get; protected set; } = 14;
        }
        private class Consts52 : ConstsBase
        {
            public override int SIZE_OP { get; protected set; } = 6;
            public override int SIZE_A { get; protected set; } = 8;
            public override int SIZE_B { get; protected set; } = 9;
            public override int SIZE_C { get; protected set; } = 9;
            public override int SIZE_Ax { get; protected set; } = 26;
            public override int SIZE_Bx { get; protected set; } = 18;

            public override int POS_OP { get; protected set; } = 0;
            public override int POS_A { get; protected set; } = 6;
            public override int POS_B { get; protected set; } = 23;
            public override int POS_C { get; protected set; } = 14;
            public override int POS_Ax { get; protected set; } = 6;
            public override int POS_Bx { get; protected set; } = 14;
        }
        private class Consts54 : ConstsBase
        {
            public override int SIZE_OP { get; protected set; } = 7;
            public override int SIZE_A { get; protected set; } = 8;
            public override int SIZE_B { get; protected set; } = 8;
            public override int SIZE_C { get; protected set; } = 8;
            public override int SIZE_Ax { get; protected set; } = 25;
            public override int SIZE_Bx { get; protected set; } = 17;
            public override int SIZE_sJ { get; protected set; } = 25;

            public override int POS_OP { get; protected set; } = 0;
            public override int POS_A { get; protected set; } = 7;
            public override int POS_B { get; protected set; } = 16;
            public override int POS_C { get; protected set; } = 24;
            public override int POS_k { get; protected set; } = 15;
            public override int POS_Ax { get; protected set; } = 7;
            public override int POS_Bx { get; protected set; } = 15;
            public override int POS_sJ { get; protected set; } = 7;
        }
        #endregion

        private LuaVersion version;
        private ConstsBase consts;

        public OpCodes Opcode { get; private set; }
        public int A, B, C, k;
        public int Ax, Bx;
        public int sB, sC, sBx, sJ;

        #region Main methods
        private bool IsNeg(int num) => num + num < num; // -10 + -10 = -20 thus doing Less than -10 will return true

        private int MASK1(int n, int p) => (~((~0) << n)) << p;
        private int MASK0(int n, int p) => (~MASK1(n, p));

        private int int2sC(int i) => i + consts.OFFSET_sC;
        private int sC2int(int i) => i - consts.OFFSET_sC;

        private long GetArg(uint inst, int pos, int size) => (inst >> pos) & MASK1(size, 0);
        private long SetArg(uint inst, int value, int pos, int size) => (inst & MASK0(size, pos)) | ((value << pos) & MASK1(size, pos));

        #region Getters
        private int GetOpcode(uint data) => (int)GetArg(data, consts.POS_OP, consts.SIZE_OP);
        private int GetA(uint data) => (int)GetArg(data, consts.POS_A, consts.SIZE_A);
        private int GetB(uint data) => (int)GetArg(data, consts.POS_B, consts.SIZE_B);
        private int GetsB(uint data) => sC2int(GetB(data));
        private int GetC(uint data) => (int)GetArg(data, consts.POS_C, consts.SIZE_C);
        private int GetsC(uint data) => sC2int(GetC(data));
        private int GetK(uint data) => (int)GetArg(data, consts.POS_k, 1);
        private int GetAx(uint data) => (int)GetArg(data, consts.POS_Ax, consts.SIZE_Ax);
        private int GetBx(uint data) => (int)GetArg(data, consts.POS_Bx, consts.SIZE_Bx);
        private int GetsBx(uint data) => GetBx(data) - consts.MAXARG_sBx;
        private int GetsJ(uint data) => (int)GetArg(data, consts.POS_sJ, consts.SIZE_sJ);
        #endregion
        #region Setters
        private uint SetOpcode(uint inst, OpCodes opcode) => (uint)SetArg(inst, OpcodeMap.GetOpcodeNumber(version, opcode), consts.POS_OP, consts.SIZE_OP);
        private uint SetA(uint inst, int A) => (uint)SetArg(inst, A, consts.POS_A, consts.SIZE_A);
        private uint SetB(uint inst, int B) => (uint)SetArg(inst, B, consts.POS_B, consts.SIZE_B);
        private uint SetC(uint inst, int C) => (uint)SetArg(inst, C, consts.POS_C, consts.SIZE_C);
        private uint SetK(uint inst, int k) => (uint)SetArg(inst, k, consts.POS_k, 1);
        private uint SetAx(uint inst, int Ax) => (uint)SetArg(inst, Ax, consts.POS_Ax, consts.SIZE_Ax);
        private uint SetBx(uint inst, int Bx) => (uint)SetArg(inst, Bx, consts.POS_Bx, consts.SIZE_Bx);
        private uint SetsBx(uint inst, int sBx) => SetBx(inst, sBx + consts.MAXARG_sBx);
        private uint SetsJ(uint inst, int sJ) => (uint)SetArg(inst, sJ, consts.POS_sJ, consts.SIZE_sJ);
        #endregion
        #endregion

        private void UpdateRegisters(uint data)
        {
            Registers register = RegistersMap.GetRegister(version, Opcode);

            if (register.A)
                A = GetA(data);

            if (register.B)
                B = GetB(data);

            if (register.sB)
                sB = GetsB(data);

            if (register.C)
                C = GetC(data);

            if (register.sC)
                sC = GetsC(data);

            if (register.k)
                k = GetK(data);

            if (register.Ax)
                Ax = GetAx(data);

            if (register.Bx)
                Bx = GetBx(data);

            if (register.sBx)
                sBx = GetsBx(data);

            if (register.sJ)
                sJ = GetsJ(data);
        }

        public Instruction(uint data, LuaVersion version)
        {
            this.version = version;

            // Get the constants for the given lua version
            switch (version)
            {
                case LuaVersion.LUA_VERSION_5_1:
                    consts = new Consts51();
                    break;
                case LuaVersion.LUA_VERSION_5_2:
                    consts = new Consts52();
                    break;
                case LuaVersion.LUA_VERSION_5_3:
                    consts = new Consts52(); // Lua5.3 has the same consts as Lua5.2
                    break;
                case LuaVersion.LUA_VERSION_5_4:
                    consts = new Consts54();
                    break;
                default:
                    throw new Exception($"Cannot decode any instructions without constants ({version})");
            }

            consts.CalcMaxargs();
            consts.CalcOffsets();

            Opcode = OpcodeMap.GetOpcodeFromMap(version, GetOpcode(data));

            UpdateRegisters(data);
        }

        internal uint GetRawInstruction()
        {
            uint inst = 0;

            Registers register = RegistersMap.GetRegister(version, Opcode);

            inst = SetOpcode(inst, Opcode);

            if (register.A)
                inst = SetA(inst, A);

            if (register.B)
                inst = SetB(inst, B);

            if (register.C)
                inst = SetC(inst, C);

            if (register.k)
                inst = SetK(inst, k);

            if (register.Ax)
                inst = SetAx(inst, Ax);

            if (register.Bx)
                inst = SetBx(inst, Bx);

            if (register.sBx)
                inst = SetsBx(inst, sBx);

            if (register.sJ)
                inst = SetsJ(inst, sJ);

            return inst;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            string RegistersUsed = "";
            string inQueue = "";

            builder.Append("Instruction: {\n");
            builder.Append($" Opcode: {Opcode},\n");

            Registers register = RegistersMap.GetRegister(version, Opcode, false);

            if (register.opcode != OpCodes.INVALID)
            {
                if (register.A)
                {
                    RegistersUsed += "A ";
                    inQueue += $" A: {A},\n";
                }
                if (register.B)
                {
                    RegistersUsed += "B ";
                    inQueue += $" B: {B},\n";
                }
                if (register.sB)
                {
                    RegistersUsed += "sB ";
                    inQueue += $" sB: {sB},\n";
                }
                if (register.C)
                {
                    RegistersUsed += "C ";
                    inQueue += $" C: {C},\n";
                }
                if (register.sC)
                {
                    RegistersUsed += "sC ";
                    inQueue += $" sC: {sC},\n";
                }
                if (register.k)
                {
                    RegistersUsed += "k ";
                    inQueue += $" k: {k},\n";
                }
                if (register.Ax)
                {
                    RegistersUsed += "Ax ";
                    inQueue += $" Ax: {Ax},\n";
                }
                if (register.Bx)
                {
                    RegistersUsed += "Bx ";
                    inQueue += $" Bx: {Bx},\n";
                }
                if (register.sBx)
                {
                    RegistersUsed += "sBx ";
                    inQueue += $" sBx: {sBx},\n";
                }
                if (register.sJ)
                {
                    RegistersUsed += "sJ ";
                    inQueue += $" sJ: {sJ},\n";
                }
            }
            else
            {
                if (!IsNeg(A))
                {
                    RegistersUsed += "A ";
                    inQueue += $" A: {A},\n";
                }
                if (!IsNeg(B))
                {
                    RegistersUsed += "B ";
                    inQueue += $" B: {B},\n";
                }
                if (!IsNeg(C))
                {
                    RegistersUsed += "C ";
                    inQueue += $" C: {C},\n";
                }
                if (!IsNeg(Bx))
                {
                    RegistersUsed += "Bx ";
                    inQueue += $" Bx: {Bx},\n";
                }
                if (!IsNeg(sBx))
                {
                    RegistersUsed += "sBx ";
                    inQueue += $" sBx: {sBx},\n";
                }
            }

            builder.Append($" UsedRegisters: {RegistersUsed.Substring(0, RegistersUsed.Length - 1)},\n");
            builder.Append(inQueue.Substring(0, inQueue.Length - 2) + "\n");
            builder.Append("} - Instruction");

            return builder.ToString();
        }
    }
}
