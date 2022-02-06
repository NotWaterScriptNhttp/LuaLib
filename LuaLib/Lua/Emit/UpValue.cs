namespace LuaLib.Lua.Emit
{
    public class UpValue
    {
        public string Name;

        public override string ToString()
        {
            // This isn't pretty lol
            return 
$@"UpValue: {"{"}
 Name: {Name},
{"}"} - Constant";
        }
    }
}
