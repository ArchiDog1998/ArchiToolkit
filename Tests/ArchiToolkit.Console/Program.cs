// ReSharper disable LocalizableElement

using ArchiToolkit.Assertions;
using ArchiToolkit.Assertions.Assertions.Extensions;
using ArchiToolkit.Assertions.Execution;
using ArchiToolkit.Assertions.FluentValidation;
using ArchiToolkit.Console;using ArchiToolkit.Console.Wrapper;
using ArchiToolkit.ValidResults;

ValidResultsConfig.AddValidator(new DoubleValidator(), (methodName, argumentName) =>
{
    var result = methodName is "AddDays" && argumentName is "value";
    return result;
});
var datetimeResult = DateTime.Now.ToValidResult();
var result = datetimeResult.AddDays(-10);

Console.WriteLine(result);
var validator = new Validator();
using (new AssertionScope())
{
    new Item().Must().BeValidBy(validator);
}

var task = Wait();
Console.WriteLine("Starting task");
await task;
return;

async Task Wait()
{
    Console.WriteLine("Waiting...");
    await Task.Delay(1000);
}