namespace ArchiToolkit.CppInteropGenerator.Models;

public readonly struct ConvertTypeModel(ConvertType type)
{
    public ConvertType Type => type;
    public override string ToString() => type switch
    {
        ConvertType.PInvoke => "P/Invoke",
        ConvertType.NativeLibrary => "Native Library",
        _ => "Unknown Type",
    };
}