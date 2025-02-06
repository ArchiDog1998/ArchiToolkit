// ReSharper disable LocalizableElement

using ArchiToolkit.Assertions.Assertions.Extensions;
using ArchiToolkit.Assertions.Execution;

List<int> a = [1, 2, 3];
using (new AssertionScope("Nice scope"))
{
    a.Must().Contain(1).AndIt.Must.Not.Contain(2);
}
Console.WriteLine("Hello, World!");