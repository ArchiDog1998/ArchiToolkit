using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ArchiToolkit.CppInteropGen;


public abstract class CppObject(bool disposed = false) : IDisposable
{
    protected abstract string DllName { get; }

    private bool _disposed = disposed;

    void IDisposable.Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        GC.SuppressFinalize(this);
        Dispose();
        Delete();
    }

    [DebuggerStepThrough]
    protected void SafeRun(Func<NativeFunctionLoader, IntPtr> func)
    {
        var loader = NativeFunctionLoader.GetLoader(DllName);
        var errorPtr = func(loader);
        if (errorPtr == IntPtr.Zero) return;
        throw new CppException(GetStringFromIntPtr(errorPtr, loader));

        static unsafe string GetStringFromIntPtr(IntPtr errorPtr, NativeFunctionLoader loader)
        {
            var message = Marshal.PtrToStringAnsi(errorPtr) ?? "Unknown error";
            ((delegate* unmanaged[Cdecl]<IntPtr, void> )loader.GetFunctionPointer("free_error"))(errorPtr);
            return message;
        }
    }

    protected virtual void Dispose()
    {

    }

    protected abstract void Delete();

    public sealed class CppException(string message) : Exception(message)
    {
        private static readonly Regex Matcher = new (@"\[(.*?)\]");

        public string ExceptionType
        {
            get
            {
                var match = Matcher.Match(Message);
                return match.Success ? match.Groups[1].Value : "Unknown";
            }
        }
    }
}