namespace ArchiToolkit.Fluent;
/// <summary>
/// What it is.
/// </summary>
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
    public int Check(in int abc)
    {
        Console.WriteLine("Invoke the Check " + abc);
        return Data;
    }

    public void VoidCheck(in int abc, out int cde, ref int x)
    {
        cde = 2;
        Console.WriteLine("Invoke the Check " + abc);
    }
}

public static class FluentObjectsExtensions
{
    /// <summary>
    /// Invoke the method <see cref="Test.Check"/> in <see cref="Test"/>
    /// <para><inheritdoc cref="Test.Check"/></para>
    /// </summary>
    /// <param name="fluent">Self</param>
    /// <param name="abc"><inheritdoc cref="Test.Check"/></param>
    /// <returns>
    /// <list type="table">
    /// <listheader>
    ///   <term>Parameter</term>
    ///   <description>Description</description>
    /// </listheader>
    /// <item>
    ///   <term><c>abc</c></term>
    ///   <description>The input value to process.</description>
    /// </item>
    /// <item>
    ///   <term><c>mode</c></term>
    ///   <description>The processing mode (e.g., Fast, Safe).</description>
    /// </item>
    /// <item>
    ///   <term><c>output</c></term>
    ///   <description>The computed result.</description>
    /// </item>
    /// </list>
    /// </returns>
    public static DoResult<Test, (int cde, int x)> DoVoidCheck(this Fluent<Test> fluent, int abc, int x)
    {
        return fluent.InvokeMethod(Invoke);
        (int cde, int x) Invoke(ref Test data)
        {
            data.VoidCheck(abc, out var cde, ref x);
            return (cde, x);
        }
    }
}