namespace ArchiToolkit.Console;

/// <summary>
/// Test summary.
/// </summary>
/// <typeparam name="T"></typeparam>
public class TestClass<T> where T :  struct
{
    /// <summary>
    /// What the hell.
    /// </summary>
    public T Data { get; set; } = default;


    public class TestSub
    {
        public int A { get; set; }
        public Task<int> TestMethod() => Task.FromResult(1);
    }


    private void NonConstMethod()
    {

    }

    public void TestMethod(TestSub t)
    {
        var a = t.TestMethod().Result;
        t.A = 5;
        var c = t.A;
        var b = t;
        var b2 = b;
        b2.A = 10;

        var i = 0;
        i = 5;
        var j = 0;
        j = i;
        NonConstMethod();
        // var d = (10, "");
        // int a = t.A, b;
        // NestMethod();
        // a = b = 1;
        // var c = t.TestMethod().Result.Equals(1);
        // return;
        // //
        // [Const]
        // void NestMethod()
        // {
        //
        // }
    }
}

