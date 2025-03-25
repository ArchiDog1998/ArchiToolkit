using System.Diagnostics.CodeAnalysis;
using System.Drawing;

// ReSharper disable ArrangeTypeModifiers
// ReSharper disable PartialTypeWithSinglePart

namespace ArchiToolkit.Grasshopper;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal static partial class ArchiToolkitResources
{
    public static Bitmap? GetIcon(string resourceName)
    {
        using var stream = typeof(ArchiToolkitResources).Assembly.GetManifestResourceStream(resourceName);
        if (stream is null) return null;
        var bitmap = new Bitmap(stream);
        if (bitmap.Width < 2 || bitmap.Height < 2) return null!;
        return bitmap;
    }
}