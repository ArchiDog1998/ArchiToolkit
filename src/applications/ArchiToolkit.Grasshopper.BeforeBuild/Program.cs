// See https://aka.ms/new-console-template for more information

using ArchiToolkit.Grasshopper.BeforeBuild;

if (args.Length < 1)
{
#if DEBUG
    args = [@"C:\Users\ArchiTed\PartTime\Code\ArchiToolkit\tests\ArchiToolkit.Grasshopper.Instance"];
#else
    return;
#endif
}

var folder =  args[0];
var resxFile = new FileInfo(Path.Combine(folder, "l10n", "ArchiToolkit.Resources.resx"));
resxFile.Directory?.Create();
var iconFolder = new DirectoryInfo(Path.Combine(folder, "Icons"));
iconFolder.Create();
ResxManager.Generate(resxFile.FullName, new Dictionary<string, string>()
{
    ["test"] = "Hello World!",
});