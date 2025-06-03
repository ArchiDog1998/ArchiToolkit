using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace ArchiToolkit.CppInteropGen;

public sealed class NativeFunctionLoader : IDisposable
{
    private static readonly ConcurrentDictionary<string, NativeFunctionLoader> Loaders = [];
    private readonly Lazy<IntPtr> _libHandle;
    private readonly ConcurrentDictionary<string, IntPtr> _exportCache = new(StringComparer.Ordinal);
    private bool _disposed;

    internal static NativeFunctionLoader GetLoader(string libName)
    {
        if (string.IsNullOrEmpty(libName)) throw new ArgumentException("The libName is null or empty.", nameof(libName));
        var loader = Loaders.GetOrAdd(libName, n => new NativeFunctionLoader(n));
        if (!loader._disposed) return loader;
        return Loaders[libName] = new NativeFunctionLoader(libName);
    }

    private NativeFunctionLoader(string libraryPath)
    {
        _libHandle = new Lazy<IntPtr>(() =>
        {
            var handle = NativeLibrary.Load(libraryPath);
            if (handle != IntPtr.Zero) return handle;
            throw new InvalidOperationException($"Failed to load native library: {libraryPath}");
        });
    }

    public IntPtr GetFunctionPointer(string name)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(NativeFunctionLoader));
        return _exportCache.GetOrAdd(name, n => NativeLibrary.GetExport(_libHandle.Value, n));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_libHandle.IsValueCreated) NativeLibrary.Free(_libHandle.Value);
    }
}