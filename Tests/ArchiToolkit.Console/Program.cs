// ReSharper disable LocalizableElement

using System.Text.RegularExpressions;using ArchiToolkit.Assertions;
using ArchiToolkit.Assertions.Assertions;
using ArchiToolkit.Assertions.Assertions.Extensions;
using ArchiToolkit.Assertions.Logging;
using ArchiToolkit.InterpolatedParser;
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
var x = 0;
var name = "";
"x is 69! And My name is ArchiTed".Parse(["abc"], $"x is {x:1232sa}! And M[y] name is {name}");
Console.WriteLine(x);
Console.WriteLine(name);
