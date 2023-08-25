using Colibri.Core;

namespace Colibri.Tests;

public class R5RSTests
{
    [Fact]
    public void R5RSLibraryTest()
    {
        const string program = @"
(import (scheme r5rs))
(define x 8)
(define y 5)
(define z 2)
(define result (+ (* x y) z))
result
";
        
        var runtime = new ColibriRuntime(new RuntimeOptions
        {
            ImportStandardLibrary = false,
        });
        
        var result = runtime.EvaluateProgram(program);
        
        Assert.Equal(42, result);
    }
}