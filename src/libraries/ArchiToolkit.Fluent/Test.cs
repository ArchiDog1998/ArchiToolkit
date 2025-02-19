namespace ArchiToolkit.Fluent;

public struct Test
{
    public string Name;
    /// <summary>
    ///     This looks good
    /// </summary>
    public int Data
    {
        get;
        set
        {
            field = value;
            Console.WriteLine("Set the data " + value);
        }
    }

    /// <summary>
    ///     Check the things.
    /// </summary>
    /// <param name="abc">looks good</param>
    /// <returns>OK</returns>
    public int Check(int abc)
    {
        Console.WriteLine("Invoke the Check " + abc);
        return Data;
    }
}

public static partial class FluentObjectsExtensions
{
    /// <summary>
    /// Invoke the method <see cref="Test.Check"/> in <see cref="Test"/>
    /// <para><inheritdoc cref="Test.Check"/></para>
    /// </summary>
    /// <param name="fluent">Self</param>
    /// <param name="abc"><inheritdoc cref="Test.Check"/></param>
    /// <returns>Self</returns>
    public static DoResult<Test, int> DoCheck(this Fluent<Test> fluent, int abc)
    {
        return fluent.InvokeMethod(Invoke);
        int Invoke(ref Test data)
        {
            return data.Check(abc);
        }
    }
}