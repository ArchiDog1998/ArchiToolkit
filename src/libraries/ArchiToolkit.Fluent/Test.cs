namespace ArchiToolkit.Fluent;

public struct Test
{
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

public static class FluentObjectsExtensions
{
    /// <summary>
    ///     Set the value <see cref="Test.Data" /> in <see cref="Test" />
    ///     <para>
    ///         <inheritdoc cref="Test.Data" />
    ///     </para>
    /// </summary>
    /// <param name="fluent">Self</param>
    /// <param name="value">The value to input</param>
    /// <returns>Self</returns>
    public static Fluent<Test> WithData(this Fluent<Test> fluent, int value)
    {
        return fluent.AddProperty(Modify);

        void Modify(ref Test data)
        {
            data.Data = value;
        }
    }

    /// <summary>
    ///     <inheritdoc cref="WithData(ArchiToolkit.Fluent.Fluent{ArchiToolkit.Fluent.Test},int)" />
    /// </summary>
    /// <param name="fluent">Self</param>
    /// <param name="modifyValue">The method to modify it</param>
    /// <returns>Self</returns>
    public static Fluent<Test> WithData(this Fluent<Test> fluent, ModifyDelegate<int> modifyValue)
    {
        return fluent.AddProperty(Modify);

        void Modify(ref Test data)
        {
            data.Data = modifyValue(data.Data);
        }
    }

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