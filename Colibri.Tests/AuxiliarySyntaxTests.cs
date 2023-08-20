using Colibri.Core;

namespace Colibri.Tests;

public class AuxiliarySyntaxTests
{
    [InlineData("...")]
    [InlineData("=>")]
    [InlineData("else")]
    [Theory]
    public void AuxiliarySyntaxShouldNotBeEvaluated(string value)
    {
        var runtime = new ColibriRuntime();

        Assert.Throws<InvalidOperationException>(() => runtime.EvaluateProgram(value));
    }
}