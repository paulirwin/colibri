using Colibri.Core;

namespace Colibri.Tests;

public class TypeCheckTests
{
    [Fact]
    public void BasicReturnTypeCheck_Success()
    {
        const string input = "fn foo () -> i32 { 1 }; (foo)";
        
        var runtime = new ColibriRuntime();
        
        // Implicitly asserting that this doesn't throw
        var result = runtime.EvaluateProgram(input);
        
        Assert.Equal(1, result);
    }
    
    [Fact]
    public void BasicReturnTypeCheck_Failure()
    {
        const string input = "fn foo () -> i32 { \"foo\" }; (foo)";
        
        var runtime = new ColibriRuntime();
        
        Assert.Throws<ReturnTypeCheckException>(() => runtime.EvaluateProgram(input));
    }
}