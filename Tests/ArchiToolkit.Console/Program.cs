// ReSharper disable LocalizableElement

using System.Text.RegularExpressions;
using ArchiToolkit.Assertions;
using ArchiToolkit.Assertions.Assertions;
using ArchiToolkit.Assertions.Assertions.Extensions;
using ArchiToolkit.Assertions.Logging;
using ArchiToolkit.Console;
using ArchiToolkit.Fluent;
using ArchiToolkit.InterpolatedParser;
using ArchiToolkit.InterpolatedParser.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// using var services = new ServiceCollection()
//     .AddLogging(builder =>
//     {
//         builder.AddSeq();
//         builder.AddConsole();
//         builder.AddArchiToolkitAssertion();
//     })
//     .BuildServiceProvider();
//
// var logger = services.GetRequiredService<ILogger<Program>>();
//
// var a = "Hello, My World!";
// var b = new List<int> { 1, 2, 3 };
//
// using (logger.BeginAssertionScope("Nice Scope{Cool}", a))
// {
//     b.Should().HaveCount(2, new AssertionParams()
//     {
//         ReasonFormat = "You are good",
//         Tag = new EventId(15, "Fucking event"),
//     }).And.ContainSingle(3).Which.Could.Be(2);
// }

var a = "";
"I am cool!!".Parse($"I am {a}");
Console.WriteLine(a);

