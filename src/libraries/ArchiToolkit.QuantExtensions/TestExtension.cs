using System.CodeDom.Compiler;
using UnitsNet;

namespace ArchiToolkit.QuantExtensions;

/// <summary>
/// 
/// </summary>
public static class TestExtension
{
    extension(int value)
    {
        public Length A => Length.FromMillimeters(value);
    }
}