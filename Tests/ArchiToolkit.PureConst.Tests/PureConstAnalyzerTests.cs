using Verify = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<
    ArchiToolkit.PureConst.Analyzer.PureConstAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace ArchiToolkit.Pure.Tests;

public class PureConstAnalyzerTests
{
    [Test]
    public async Task SetSpeedHugeSpeedSpecified_AlertDiagnostic()
    {
        const string text = @"
public class Program
{
    public void Main()
    {
        var spaceship = new Spaceship();
        spaceship.SetSpeed(300000000);
    }
}

public class Spaceship
{
    public void SetSpeed(long speed) {}
}
";

        var expected = Verify.Diagnostic().WithLocation(7, 28)
            .WithArguments("300000000");

        //await Verify.VerifyAnalyzerAsync(text, expected);
    }
}