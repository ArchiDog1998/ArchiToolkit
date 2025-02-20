using Microsoft.CodeAnalysis;

namespace ArchiToolkit.RoslynHelper.Names;

/// <summary>
///
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IName<out T> where T : ISymbol
{
    /// <summary>
    /// Symbol
    /// </summary>
    public T Symbol { get; }

    /// <summary>
    /// Full Name
    /// </summary>
    string FullName { get; }

    /// <summary>
    /// Name
    /// </summary>
    string Name { get; }
}