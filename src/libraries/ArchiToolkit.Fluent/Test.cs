namespace ArchiToolkit.Fluent;

public struct Test
{
    /// <summary>
    /// This looks good
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
    /// Check the things.
    /// </summary>
    /// <param name="abc">looks good</param>
    /// <returns>OK</returns>
    public int Check(int abc)
    {
        Console.WriteLine("Invoke the Check " + abc);
        return Data;
    }
}

public static partial class FluentExtensions
{
    public static FluentTest AsFluentImmediate(this in Test value)
    {
        return new (value, FluentType.Immediate);
    }

    public static FluentTest AsFluentLazy(this in Test value)
    {
        return new(value, FluentType.Lazy);
    }
}

public class FluentTest(in Test test, FluentType type) : Fluent<Test>(test, type)
{
    /// <summary>
    /// Set the value <see cref="Test.Data"/> in <see cref="Test"/>
    /// <para><inheritdoc cref="Test.Data"/></para>
    /// </summary>
    /// <param name="value">The value to input</param>
    /// <returns>Self</returns>
    public FluentTest WithData(int value)
    {
        AddAction(() => Target.Data = value);
        return this;
    }

    /// <summary>
    /// <inheritdoc cref="WithData(System.Int32)"/>
    /// </summary>
    /// <param name="modifyValue">The method to modify it</param>
    /// <returns>Self</returns>
    public FluentTest WithData(ModifyDelegate<int> modifyValue)
    {
        AddAction(() => Target.Data = modifyValue(Target.Data));
        return this;
    }

    /// <summary>
    /// Invoke the method <see cref="Test.Check"/> in <see cref="Test"/>
    /// <para><inheritdoc cref="Test.Check"/></para>
    /// </summary>
    /// <param name="abc"><inheritdoc cref="Test.Check"/></param>
    /// <returns><inheritdoc cref="Test.Check"/></returns>
    public DoResult<FluentTest, Test, int> DoCheck(int abc)
    {
        return new(this, () => Target.Check(abc));
    }
}