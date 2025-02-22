using ArchiToolkit.Fluent;
using ArchiToolkit.Fluent.Tests;

[assembly: FluentApi(typeof(BasicType<,>), typeof(BasicType<Random,int>))]
[assembly: FluentApi(typeof(int))]
[assembly: FluentApi]