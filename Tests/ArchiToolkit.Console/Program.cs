// ReSharper disable LocalizableElement

using ArchiToolkit.Assertions;
using ArchiToolkit.Assertions.Assertions.Extensions;
using ArchiToolkit.Assertions.Execution;

var a = "Hello, World!";
var b = new List<int> { 1, 2, 3 };
using (new AssertionScope("Nice scope"))
{
    b.Must().HaveCount(2).And.ContainSingle(3).Which.Must.Be(2);
}

Console.WriteLine("Hello, World!");