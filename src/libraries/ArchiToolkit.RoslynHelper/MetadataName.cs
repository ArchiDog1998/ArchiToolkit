using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;

namespace ArchiToolkit.RoslynHelper;

/// <summary>
/// The metadataName for the symbol
/// </summary>
[Obsolete]
public readonly struct MetadataName
{
    private readonly Lazy<string> _typeName, _safeName, _hashName;
    private readonly Lazy<bool> _hasTypeParameters;
    /// <summary>
    /// The typeName
    /// </summary>
    public string TypeName => _typeName.Value;

    /// <summary>
    /// Without special characters
    /// </summary>
    public string SafeName => _safeName.Value;

    /// <summary>
    /// The hashName
    /// </summary>
    public string HashName => _hashName.Value;

    /// <summary>
    /// Has type Parameter
    /// </summary>
    public bool HasTypeParameter => _hasTypeParameters.Value;

    internal MetadataName(ISymbol symbol, int hashCount)
    {
        _hasTypeParameters = new Lazy<bool>(() =>
        {
            symbol.GetFullMetadataName(out var has);
            return has;
        });

        _typeName = new Lazy<string>(() => symbol.GetFullMetadataName(out _, true));
        _safeName = new Lazy<string>(() => symbol.GetFullMetadataName(out _)
            .Replace('.', '_').Replace('[', '_').Replace(']', '_')
            .Replace('<', '_').Replace('>', '_')
            .Replace(',', '_').Replace(' ', '_'));
        _hashName = new Lazy<string>(() =>
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_1234567890";
            var input = symbol.GetFullMetadataName(out _);
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Concat(hashBytes.Take(hashCount).Select(b => chars[b % chars.Length]));
        });
    }
}