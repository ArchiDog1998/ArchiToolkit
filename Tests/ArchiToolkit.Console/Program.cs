// ReSharper disable LocalizableElement

using System.Collections;
using ArchiToolkit.InterpolatedParser;

var task = Wait();
Console.WriteLine("Starting task");
await task;
return;

async Task Wait()
{
    Console.WriteLine("Waiting...");
    await Task.Delay(1000);
}