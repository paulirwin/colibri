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

    [Fact]
    public void ImportExceptSuccessfulTest()
    {
        const string program = @"
(import (except (scheme base) -))
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
    public void ImportExceptFailedTest()
    {
        const string program = @"
(import (except (scheme base) -))
(- 1 2)
";

        var runtime = new ColibriRuntime(new RuntimeOptions
        {
            ImportStandardLibrary = false,
        });

        Assert.ThrowsAny<Exception>(() => runtime.EvaluateProgram(program));
    }

    [Fact]
    public void ImportCombinedOnlyAndExceptTest()
    {
        const string program = @"
(import (only (except (scheme base) -) +))
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
    public void ImportRenameSuccessfulTest()
    {
        const string program = @"
(import (rename (scheme base) (+ plus)))
(plus 1 2)
";
        
        var runtime = new ColibriRuntime(new RuntimeOptions
        {
            ImportStandardLibrary = false,
        });
        
        var result = runtime.EvaluateProgram(program);
        
        Assert.Equal(3, result);
    }
    
    [Fact]
    public void ImportRenameFailedTest()
    {
        const string program = @"
(import (rename (scheme base) (+ plus)))
(+ 1 2)
";
        
        var runtime = new ColibriRuntime(new RuntimeOptions
        {
            ImportStandardLibrary = false,
        });
        
        Assert.ThrowsAny<Exception>(() => runtime.EvaluateProgram(program));
    }
    
    [Fact]
    public void ImportRenameMultipleNamesTest()
    {
        const string program = @"
(import (rename (scheme base) (+ plus) (- minus)))
(plus 1 (minus 4 2))
";
        
        var runtime = new ColibriRuntime(new RuntimeOptions
        {
            ImportStandardLibrary = false,
        });
        
        var result = runtime.EvaluateProgram(program);
        
        Assert.Equal(3, result);
    }
    
    [Fact]
    public void ImportRenameMultipleNamesFailedTest()
    {
        const string program = @"
(import (rename (scheme base) (+ plus) (- minus)))
(+ 1 (- 4 2))
";
        
        var runtime = new ColibriRuntime(new RuntimeOptions
        {
            ImportStandardLibrary = false,
        });
        
        Assert.ThrowsAny<Exception>(() => runtime.EvaluateProgram(program));
    }
    
    [Fact]
    public void ImportRenameMultipleNamesAndOnlyTest()
    {
        const string program = @"
(import (rename (only (scheme base) + -) (+ plus) (- minus)))
(plus 1 (minus 4 2))
";
        
        var runtime = new ColibriRuntime(new RuntimeOptions
        {
            ImportStandardLibrary = false,
        });
        
        var result = runtime.EvaluateProgram(program);
        
        Assert.Equal(3, result);
    }
    
    [Fact]
    public void ImportPrefixSuccessfulTest()
    {
        const string program = @"
(import (prefix (scheme base) foo:))
(foo:+ 1 2)
";
        
        var runtime = new ColibriRuntime(new RuntimeOptions
        {
            ImportStandardLibrary = false,
        });
        
        var result = runtime.EvaluateProgram(program);
        
        Assert.Equal(3, result);
    }
    
    [Fact]
    public void ImportPrefixFailedTest()
    {
        const string program = @"
(import (prefix (scheme base) foo:))
(+ 1 2)
";
        
        var runtime = new ColibriRuntime(new RuntimeOptions
        {
            ImportStandardLibrary = false,
        });
        
        Assert.ThrowsAny<Exception>(() => runtime.EvaluateProgram(program));
    }
    
    [Fact]
    public void MultipleImportsTest()
    {
        const string program = @"
(import (scheme base) (scheme cxr))
(define mylist '(1 2 3 4))
(caddr mylist)
";
        
        var runtime = new ColibriRuntime(new RuntimeOptions
        {
            ImportStandardLibrary = false,
        });
        
        var result = runtime.EvaluateProgram(program);
        
        Assert.Equal(3, result);
    }
}