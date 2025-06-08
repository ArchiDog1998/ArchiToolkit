// ReSharper disable LocalizableElement

using System.Collections;
using ArchiToolkit.Assertions;
using ArchiToolkit.Assertions.Execution;
using ArchiToolkit.Assertions.FluentValidation;
using ArchiToolkit.Console;
using ArchiToolkit.InterpolatedParser;
using ArchiToolkit.ValidResults;

var validator = new Validator();
new Item().Must().BeValidBy(validator);

var task = Wait();
Console.WriteLine("Starting task");
await task;
return;

async Task Wait()
{
    Console.WriteLine("Waiting...");
    await Task.Delay(1000);
}