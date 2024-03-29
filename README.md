# LuaLib
Lua binary chunk reader/writer library.

NOTE: using the LuaHelpers will add nothing. It is just for internal classes

Loading a binary chunk
----------------------

```C#
    using LuaLib.Lua;

    Chunk chunk = Chunk.Load("C:/your/file.luac"); // Loads the compiled lua file
    // The chunk contains LuaHeader and MainFunction (The function that holds everything)
```

Saving a binary chunk
---------------------

```C#
    using LuaLib.Lua;

    Chunk chunk = Chunk.Load("C:/your/file.luac"); // Loads the compiled lua file

    WriterOptions options = new WriterOptions {
        KeepOldMaxStacksize = true,
        KeepLuaVersion = true,
        NewLuaVersion = LuaVersion.LUA_VERSION_X_Y
    };

    // Options do not need to be supplied
    chunk.Write("C:/your/file_out.luac", [options]); // Writes all the functions, constants, etc... out in the form of bytecode
```
