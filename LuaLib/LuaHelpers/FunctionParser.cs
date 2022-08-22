using System.Collections.Generic;

using LuaLib.Emit;

namespace LuaLib.LuaHelpers
{
    internal abstract class FunctionParser
    {
        protected LuaReader lr;

        internal FunctionParser(LuaReader lr) => this.lr = lr;

        public abstract List<Constant> GetConstants();
        public abstract void GetDebug(Function func);
    }
}
