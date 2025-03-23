// See https://aka.ms/new-console-template for more information

using ArchiToolkit.Grasshopper.BeforeBuild;
using Microsoft.CodeAnalysis;

return;
// if (args.Length < 1)
// {
// #if DEBUG
//     args = [@"C:\Users\ArchiTed\PartTime\Code\ArchiToolkit\tests\ArchiToolkit.Grasshopper.Instance"];
// #else
//     return;
// #endif
// }
//
// var folder =  args[0];
// if (!Directory.Exists(folder)) return;
// var resxFile = new FileInfo(Path.Combine(folder, "l10n", "ArchiToolkit.Resources.resx"));
// resxFile.Directory?.Create();
// var iconFolder = new DirectoryInfo(Path.Combine(folder, "Icons"));
// iconFolder.Create();
//
// var projectPath = "Project";
// using var workspace = MSBuildWorkspace.Create();
// var project = await workspace.OpenProjectAsync(projectPath);
//
// Compilation compilation = await project.GetCompilationAsync();
// foreach (var syntaxTree in compilation.SyntaxTrees)
// {
//     var semanticModel = compilation.GetSemanticModel(syntaxTree);
//     var root = await syntaxTree.GetRootAsync();
//
//     var symbols = root
//         .DescendantNodes()
//         .Select(node => semanticModel.GetSymbolInfo(node).Symbol)
//         .Where(symbol => symbol != null);
//
//     foreach (var symbol in symbols)
//     {
//         Console.WriteLine(symbol); // Print symbol information
//     }
// }
//
//
// ResxManager.Generate(resxFile.FullName, new Dictionary<string, string>()
// {
//     ["test"] = "Hello World!",
// });