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
}