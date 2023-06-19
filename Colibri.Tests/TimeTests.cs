using Colibri.Core;

namespace Colibri.Tests;

public class TimeTests
{
    [Fact]
    public void CurrentJiffy_BasicTest()
    {
        var runtime = new ColibriRuntime();

        var result = runtime.EvaluateProgram("(current-jiffy)");

        Assert.IsType<long>(result);
        Assert.NotEqual(0L, result);
    }

    [Fact]
    public void JiffiesPerSecond_BasicTest()
    {
        var runtime = new ColibriRuntime();

        var result = runtime.EvaluateProgram("(jiffies-per-second)");

        Assert.IsType<long>(result);
        Assert.Equal(10_000_000L, result);
    }

    [Fact]
    public void CurrentSecond_BasicTest()
    {
        var runtime = new ColibriRuntime();

        var result = runtime.EvaluateProgram("(current-second)");

        Assert.IsType<long>(result);
        Assert.NotEqual(0L, result);
    }
}