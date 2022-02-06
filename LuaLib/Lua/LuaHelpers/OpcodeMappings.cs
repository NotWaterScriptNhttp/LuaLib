using System.Collections.Generic;

using LuaLib.Lua.Emit;

namespace LuaLib.Lua.LuaHelpers
{
    internal struct OpcodeMapping
    {
        public bool UsesA;
        public bool UsesB;
        public bool UsesC;
        public bool UsesBx;
        public bool UsessBx;

        public OpcodeMapping(bool A = true, bool B = true, bool C = true, bool Bx = false, bool sBx = false)
        {
            UsesA = A;
            UsesB = B;
            UsesC = C;
            UsesBx = Bx;
            UsessBx = sBx;
        }
    }

    internal static class OpcodeMappings
    {
        public static Dictionary<OpCodes, OpcodeMapping> Mappings = new Dictionary<OpCodes, OpcodeMapping>
        {
            { OpCodes.MOVE, new OpcodeMapping(true, true, false) },

            { OpCodes.LOADK, new OpcodeMapping(true, false, false, true) },
            { OpCodes.LOADBOOL, new OpcodeMapping(true) },
            { OpCodes.LOADNIL, new OpcodeMapping(true, true, false) },

            { OpCodes.GETUPVAL, new OpcodeMapping(true, true, false) },
            { OpCodes.GETGLOBAL, new OpcodeMapping(true, false, false, true) },
            { OpCodes.GETTABLE, new OpcodeMapping(true) },

            { OpCodes.SETGLOBAL, new OpcodeMapping(true, false, false, true) },
            { OpCodes.SETUPVAL, new OpcodeMapping(true, true, false) },
            { OpCodes.SETTABLE, new OpcodeMapping(true) },

            { OpCodes.NEWTABLE, new OpcodeMapping(true) },

            { OpCodes.SELF, new OpcodeMapping(true) },

            { OpCodes.ADD, new OpcodeMapping(true) },
            { OpCodes.SUB, new OpcodeMapping(true) },
            { OpCodes.MUL, new OpcodeMapping(true) },
            { OpCodes.DIV, new OpcodeMapping(true) },
            { OpCodes.MOD, new OpcodeMapping(true) },
            { OpCodes.POW, new OpcodeMapping(true) },
            { OpCodes.UNM, new OpcodeMapping(true, true, false) },
            { OpCodes.NOT, new OpcodeMapping(true, true, false) },
            { OpCodes.LEN, new OpcodeMapping(true, true, false) },

            { OpCodes.CONCAT, new OpcodeMapping(true) },

            { OpCodes.JMP, new OpcodeMapping(false, false, false, false, true) },

            { OpCodes.EQ, new OpcodeMapping(true) },
            { OpCodes.LT, new OpcodeMapping(true) },
            { OpCodes.LE, new OpcodeMapping(true) },

            { OpCodes.TEST, new OpcodeMapping(true, false, true) },
            { OpCodes.TESTSET, new OpcodeMapping(true) },

            { OpCodes.CALL, new OpcodeMapping(true) },
            { OpCodes.TAILCALL, new OpcodeMapping(true) },
            { OpCodes.RETURN, new OpcodeMapping(true, true, false) },

            { OpCodes.FORLOOP, new OpcodeMapping(true, false, false, false, true) },
            { OpCodes.FORPREP, new OpcodeMapping(true, false, false, false, true) },
            { OpCodes.TFORLOOP, new OpcodeMapping(true, false, true) },

            { OpCodes.SETLIST, new OpcodeMapping(true) },

            { OpCodes.CLOSE, new OpcodeMapping(true, false, false) },
            { OpCodes.CLOSURE, new OpcodeMapping(true, false, false , true) },

            { OpCodes.VARARG, new OpcodeMapping(true, true, false) }
        };
    }
}
