using Colibri.Core;

namespace Colibri.Tests;

public class ProcessContextTests
{
    [Fact]
    public void GetEnvironmentVariable_BasicTest()
    {
        var runtime = new ColibriRuntime();

        Environment.SetEnvironmentVariable("foo", "bar");

        var result = runtime.EvaluateProgram("(get-environment-variable \"foo\")");

        Assert.Equal("bar", result);
    }

    [Fact]
    public void CommandLine_BasicTest()
    {
        var runtime = new ColibriRuntime();

        var result = runtime.EvaluateProgram("(command-line)") as Pair;

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void ExitTest()
    {
        const string program = @"
(define out-called #f)
(dynamic-wind
    (lambda () #f)
    (lambda () (exit))
    (lambda () (set! out-called #t)))
";
            
        var runtime = new ColibriRuntime();

        var e = Assert.Raises<ExitEventArgs>(
            handler => runtime.Exit += handler,
            handler => runtime.Exit -= handler,
            () => runtime.EvaluateProgram(program)
        );
        
        Assert.Equal(0, e.Arguments.ExitCode);
        Assert.Equal(true, runtime.EvaluateProgram("out-called"));
    }
}