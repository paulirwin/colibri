using Colibri.Core;

namespace Colibri.Tests;

public class LoadTests
{
    [Fact]
    public void LoadDefaultEnvironmentTest()
    {
        const string program = @"
(with-output-to-file ""test.txt""
    (lambda ()
        (display ""(define xyz 42)"")
        (newline)))

(load ""test.txt"")
(define result xyz)
(delete-file ""test.txt"")
result
";

        var runtime = new ColibriRuntime();

        var result = runtime.EvaluateProgram(program);

        Assert.Equal(42, result);
    }

    [Fact]
    public void LoadSpecifiedEnvironment()
    {
        // HACK.PI: uses (colibri base) method (mutable-environment)
        const string program = @"
(with-output-to-file ""test.txt""
    (lambda ()
        (display ""(define xyz 42)"")
        (newline)))

(define e (mutable-environment '(scheme base)))
(load ""test.txt"" e)
(define result (eval 'xyz e))
(delete-file ""test.txt"")
result
";

        var runtime = new ColibriRuntime();

        var result = runtime.EvaluateProgram(program);

        Assert.Equal(42, result);
    }
}