// ReSharper disable LocalizableElement

using System.Text.RegularExpressions;using ArchiToolkit.Assertions;
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

//var a = 0;
//"abc10".Parse($"abc{a}");

var obj = new Test();

var t = obj.AsFluent(FluentType.Immediate)
    .WithData(1)
    .DoCheck(123)
    .ContinueWhen(t => t == 1)
    .WithData(2);

Console.WriteLine("Wait for the result");
var r1 = t.Result;
Console.WriteLine(r1.Data);
Console.WriteLine(typeof(Type).GetFulTypeName());

