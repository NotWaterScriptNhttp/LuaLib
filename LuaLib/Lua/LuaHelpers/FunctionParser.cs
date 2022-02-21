using System.Collections.Generic;

using LuaLib.Lua.Emit;

namespace LuaLib.Lua.LuaHelpers
{
    internal abstract class FunctionParser
    {
        protected LuaReader lr;

        internal FunctionParser(LuaReader lr) => this.lr = lr;

        public abstract List<Constant> GetConstants();
        public abstract void GetDebug(Function func);
    }
}
