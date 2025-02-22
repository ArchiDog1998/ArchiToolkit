namespace ArchiToolkit.Fluent;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = true)]
public class FluentApiAttribute(params Type[] types) : Attribute;