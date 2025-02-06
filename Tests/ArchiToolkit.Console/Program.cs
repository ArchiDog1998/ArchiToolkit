// ReSharper disable LocalizableElement

using ArchiToolkit.Assertions;
using ArchiToolkit.Assertions.Assertions;
using ArchiToolkit.Assertions.Assertions.Extensions;
using ArchiToolkit.Assertions.Execution;

var a = "Hello, World!";
var b = new List<int> { 1, 2, 3 };
using (new AssertionScope("Nice scope"))
{
    b.Must().HaveCountLessThanOrEqualTo(5);
}

Console.WriteLine("Hello, World!");