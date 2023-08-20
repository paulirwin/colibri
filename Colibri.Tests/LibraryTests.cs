using Colibri.Core;

namespace Colibri.Tests;

public class LibraryTests
{
    [Fact]
    public void NegativeTestWithNoStandardLibrariesImported()
    {
        const string program = "(+ 1 2)";
        
        var runtime = new ColibriRuntime(new RuntimeOptions
        {
            ImportStandardLibrary = false,
        });
        
        Assert.ThrowsAny<Exception>(() => runtime.EvaluateProgram(program));
    }
    
    [Fact]
    public void BasicStandardLibraryImportTest()
    {
        const string program = @"
(import (scheme base))
(+ 1 2)
";

        var runtime = new ColibriRuntime(new RuntimeOptions
        {
            ImportStandardLibrary = false,
        });
        
        var result = runtime.EvaluateProgram(program);
        
        Assert.Equal(3, result);
    }
    
    [Fact]
    public void ReimportingStandardLibraryTest()
    {
        const string program = @"
(import (scheme base))
(import (scheme base))
(+ 1 2)
";
        
        var runtime = new ColibriRuntime(new RuntimeOptions
        {
            ImportStandardLibrary = false,
        });
        
        var result = runtime.EvaluateProgram(program);
        
        Assert.Equal(3, result);
    }
    
    [Fact]
    public void ImportingNonExistentLibraryTest()
    {
        const string program = @"
(import (scheme foo))
";
        
        var runtime = new ColibriRuntime();
        
        Assert.ThrowsAny<Exception>(() => runtime.EvaluateProgram(program));
    }
    
    [Fact]
    public void BasicImportOnlySuccessfulTest()
    {
        const string program = @"
(import (only (scheme base) +))
(+ 1 2)
";
        
        var runtime = new ColibriRuntime(new RuntimeOptions
        {
            ImportStandardLibrary = false,
        });
        
        runtime.EvaluateProgram(program);
    }
        
    [Fact]
    public void BasicImportOnlyFailedTest()
    {
        const string program = @"
(import (only (scheme base) +))
(- 1 2)
";
        
        var runtime = new ColibriRuntime(new RuntimeOptions
        {
            ImportStandardLibrary = false,
        });
        
        Assert.ThrowsAny<Exception>(() => runtime.EvaluateProgram(program));
    }
    
    [Fact]
    public void ImportOnlyMultipleNamesTest()
    {
        const string program = @"
(import (only (scheme base) + -))
(+ 1 (- 4 2))
";
        
        var runtime = new ColibriRuntime(new RuntimeOptions
        {
            ImportStandardLibrary = false,
        });
        
        var result = runtime.EvaluateProgram(program);
        
        Assert.Equal(3, result);
    }
}