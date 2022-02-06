# LuaLib
Lua binary chunk reader/writer library.
Currently supports only lua 5.1.5 but more lua versions will be supported later.

Loading a binary chunk
----------------------

```C#
    using LuaLib.Lua;

    Chunk chunk = Chunk.Load("C:/your/file.luac"); // Loads the compiled lua file
    // The chunk contains LuaHeader and MainFunction (The function that holds everything)
```

Saving a binary chunk
---------------------

Not implemented yet