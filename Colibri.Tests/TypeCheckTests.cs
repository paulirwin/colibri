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
    
    [Fact]
    public void BasicReturnTypeCheck_Nil()
    {
        const string input = "fn foo () -> () { }; (foo)";
        
        var runtime = new ColibriRuntime();
        
        // Implicitly asserting that this doesn't throw
        var result = runtime.EvaluateProgram(input);
        
        Assert.Equal(Nil.Value, result);
    }

    [Fact]
    public void VariableListReturnTypeCheck_Success()
    {
        const string input = "fn foo () -> (...) { list 1 2 }; (foo)";
        
        var runtime = new ColibriRuntime();
        
        // Implicitly asserting that this doesn't throw
        var result = runtime.EvaluateProgram(input);

        var pair = Assert.IsType<Pair>(result);
        var list = pair.ToList();
        
        Assert.Equal(2, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
    }
    
    [Fact]
    public void VariableListReturnTypeCheck_Failure()
    {
        const string input = "fn foo () -> (...) { 1 }; (foo)";
        
        var runtime = new ColibriRuntime();
        
        Assert.Throws<ReturnTypeCheckException>(() => runtime.EvaluateProgram(input));
    }
    
    [Fact]
    public void BasicArgumentTypeCheck_Success()
    {
        const string input = "fn square (a: i32) -> i32 { * a a }; square 4";
        
        var runtime = new ColibriRuntime();
        
        // Implicitly asserting that this doesn't throw
        var result = runtime.EvaluateProgram(input);
        
        Assert.Equal(16, result);
    }
    
    [Fact]
    public void BasicArgumentTypeCheck_Failure()
    {
        const string input = "fn square (a: i32) -> i32 { * a a }; square \"foo\"";
        
        var runtime = new ColibriRuntime();
        
        Assert.Throws<ArgumentTypeCheckException>(() => runtime.EvaluateProgram(input));
    }
    
    [Fact]
    public void BasicArgumentTypeCheck_Nil()
    {
        const string input = "fn square (a: i32) -> i32 { * a a }; square nil";
        
        var runtime = new ColibriRuntime();
        
        Assert.Throws<ArgumentTypeCheckException>(() => runtime.EvaluateProgram(input));
    }
    
    [Fact]
    public void BasicArgumentTypeCheck_Multiple_Success()
    {
        const string input = "fn add (a: i32 b: i32) -> i32 { + a b }; add 1 2";
        
        var runtime = new ColibriRuntime();
        
        // Implicitly asserting that this doesn't throw
        var result = runtime.EvaluateProgram(input);
        
        Assert.Equal(3, result);
    }
    
    [Fact]
    public void BasicArgumentTypeCheck_Multiple_Failure()
    {
        const string input = "fn add (a: i32 b: i32) -> i32 { + a b }; add 1 \"foo\"";
        
        var runtime = new ColibriRuntime();
        
        Assert.Throws<ArgumentTypeCheckException>(() => runtime.EvaluateProgram(input));
    }
}