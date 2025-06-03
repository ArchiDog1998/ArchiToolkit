# CppInteropGen
> [!WARNING]  
> This tool is still work in progress.

## Brief
This is an easy tool to wrap the cpp file to c#.

## CPP
You should include the file `csharp_interop.h` to your cpp project.
And Define the Macro `WRAP_CALL_CUSTOM_CATCH` before include the `csharp_interop.h`.
Use the Macro `CSHARP_WRAPPER` To define the method you like.

use `//DLL_NAME:` to define the dll name.
ust `//STRUCT:` to define the struct layout.
## C#
In C#, it'll generate the Wrappers for you. You can use `partial` keyword to add more things.