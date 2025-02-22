namespace ArchiToolkit.Fluent.Tests;

// public class BasicFluentTest
// {
//     [Test]
//     public async Task PropertyTest()
//     {
//         var obj = new LoggingType<Random, int>();
//         await Assert.That(obj.DataA).IsNull();
//
//         obj = obj.AsFluent()
//             .WithDataA(new Random())
//             .Result;
//
//         await Assert.That(obj.DataA).IsNotNull();
//     }
//
//     [Test]
//     public async Task MethodTest()
//     {
//         var obj = new LoggingType<Random, int>();
//         await Assert.That(obj.Name).IsEmpty();
//
//         obj = obj.AsFluent()
//             .DoAMethod(1)
//             .Result;
//
//         await Assert.That(obj.Name).IsEqualTo(nameof(Int32));
//     }
// }