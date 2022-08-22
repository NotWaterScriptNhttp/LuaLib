using System.IO;

using LuaLib.Emit;
using LuaLib.LuaHelpers;

namespace LuaLib
{
    public class Writer
    {
        private LuaVersion version;
        private Function function;

        private LuaWriter writer;

        public Writer(Chunk chunk)
        {
            version = chunk.Header.Version;
            function = chunk.MainFunction;
        }
        public Writer(LuaVersion version, Function func)
        {
            this.version = version;
            function = func;
        }

        public byte[] GetBytes(WriterOptions options = null)
        {
            if (writer == null)
                writer = LuaWriter.GetWriter(version, function, options);

            return writer.GetWritenBytes();
        }

        public void Write(string file, WriterOptions options = null) => File.WriteAllBytes(file, GetBytes(options));
    }
}
