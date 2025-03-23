// See https://aka.ms/new-console-template for more information
return;
if (args.Length < 1)
{
#if DEBUG
    //args = [@"C:\Users\ArchiTed\PartTime\Code\ArchiToolkit\tests\ArchiToolkit.Grasshopper.Instance"];
    args = [@"E:\PartTime\Code\ArchiToolkit\tests\ArchiToolkit.Grasshopper.Instance"];
#else
    return;
#endif
}

var folder =  args[0];
if (!Directory.Exists(folder)) return;

var directory = Path.Combine(Path.GetTempPath(), "ArchiToolkit.Grasshopper");
if (!Directory.Exists(directory)) return;
var resx = Path.Combine(directory, "ArchiToolkit.Resources.resx");
var icons = Path.Combine(directory, "icons.txt");

var resxTarget = new FileInfo(Path.Combine(folder, "l10n", "ArchiToolkit.Resources.resx"));
resxTarget.Directory?.Create();
if (File.Exists(resx))
{
    try
    {
        File.Copy(resx, resxTarget.FullName, true);
    }
    catch
    {
        // ignored
    }
}
var iconFolder = Path.Combine(folder, "Icons");
if (!Directory.Exists(iconFolder)) Directory.CreateDirectory(iconFolder);

if (File.Exists(icons))
{
    var assembly = typeof(Program).Assembly;

    foreach (var icon in File.ReadLines(icons))
    {
        var iconType = icon[0];
        var fileName = Path.Combine(iconFolder, string.Concat(icon.AsSpan(1), ".png"));
        if (File.Exists(fileName)) continue;
        using var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
        switch (iconType)
        {
            case 'P':
                assembly.GetManifestResourceStream("ArchiToolkit.Grasshopper.BeforeBuild.Icons.Red.png")
                    ?.CopyTo(fileStream);
                break;
            case 'C':
                assembly.GetManifestResourceStream("ArchiToolkit.Grasshopper.BeforeBuild.Icons.Blue.png")
                    ?.CopyTo(fileStream);
                break;
            default:
                assembly.GetManifestResourceStream("ArchiToolkit.Grasshopper.BeforeBuild.Icons.White.png")
                    ?.CopyTo(fileStream);
                break;
        }
    }
}
