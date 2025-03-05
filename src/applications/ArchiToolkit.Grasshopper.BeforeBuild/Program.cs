// See https://aka.ms/new-console-template for more information
using System.Drawing;
using System.Resources.NetStandard;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CSharp;

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
var csFile =  new FileInfo(Path.Combine(folder, "l10n", "ArchiToolkit.Resources.Designer.cs"));
if(!csFile.Exists) csFile.Create();
using (var writer = new ResXResourceWriter(resxFile.FullName))
{
    writer.AddResource("example", "Test");
}
var doc = XDocument.Load(resxFile.FullName);
doc.Root!
    .Elements()
    .Where(attr => attr.Name.NamespaceName == "http://www.w3.org/2001/XMLSchema" ||
                   attr.Name.LocalName.Contains("xsd"))
    .ToList()
    .ForEach(attr => attr.Remove());
doc.Save(resxFile.FullName);

Console.WriteLine(resxFile);
Console.ReadLine();