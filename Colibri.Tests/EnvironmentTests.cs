using Colibri.Core;

namespace Colibri.Tests;

public class EnvironmentTests
{
    [Fact]
    public void InteractionEnvironmentTest()
    {
        const string program = @"
define x 42
eval 'x (interaction-environment)
";
        
        var runtime = new ColibriRuntime();
        
        var result = runtime.EvaluateProgram(program);
        
        Assert.Equal(42, result);
        
        var env = runtime.EvaluateProgram("(interaction-environment)");
        
        Assert.Same(env, runtime.UserScope);
    }

    // R7RS 6.12
    [InlineData("(eval '(* 7 3) (environment '(scheme base)))", 21)]
    [InlineData("(eval '(pre-* 7 3) (environment '(prefix (scheme base) pre-)))", 21)]
    [Theory]
    public void NewEnvironmentSuccessfulTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [Fact]
    public void NewEnvironmentImmutableTest()
    {
        const string program = "(eval '(define foo 32) (environment '(scheme base)))";

        var runtime = new ColibriRuntime();

        Assert.Throws<EnvironmentImmutableException>(() => runtime.EvaluateProgram(program));
    }
}