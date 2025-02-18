// ReSharper disable LocalizableElement

using System.Text.RegularExpressions;using ArchiToolkit.Assertions;
using ArchiToolkit.Assertions.Assertions;
using ArchiToolkit.Assertions.Assertions.Extensions;
using ArchiToolkit.Assertions.Logging;
using ArchiToolkit.Console;
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

var x = new List<int>();
var name = "";
var item = new MyTestClass();
"x is 69,12! And My name is ArchiTed".Parse($"x is {x}! And M[y] name is {name}");
Console.WriteLine(string.Join(", ", x));
Console.WriteLine(name);