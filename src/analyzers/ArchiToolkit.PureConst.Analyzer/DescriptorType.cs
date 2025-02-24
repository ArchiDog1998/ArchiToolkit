using Microsoft.CodeAnalysis;

namespace ArchiToolkit.PureConst.Analyzer;

[AttributeUsage(AttributeTargets.Field)]
internal class DescriptorAttribute(string id, DiagnosticSeverity severity, string category) : Attribute
{
    public string Id { get; set; } = id;
    public DiagnosticSeverity Severity { get; set; } = severity;
    public string Category { get; set; } = category;
}

public enum DescriptorType : byte
{
    [Descriptor("PC0001", DiagnosticSeverity.Error, "Usage")]
    CantUseOnAccessor,
}