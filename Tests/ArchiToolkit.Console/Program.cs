// ReSharper disable LocalizableElement

using ArchiToolkit.Assertions;
using ArchiToolkit.Assertions.Assertions.Extensions;
using ArchiToolkit.Assertions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using var services = new ServiceCollection()
    .AddLogging(builder =>
    {
        builder.AddSeq();
        builder.AddConsole();
        builder.AddArchiToolkitAssertion();
    })
    .BuildServiceProvider();

var logger = services.GetRequiredService<ILogger<Program>>();

var a = "Hello, World!";
var b = new List<int> { 1, 2, 3 };
using (logger.CreateScope("Nice Scope"))
{
    b.Should().HaveCount(2, "You are good").And.ContainSingle(3).Which.Could.Be(2);
}

Console.WriteLine("Hello, World!");