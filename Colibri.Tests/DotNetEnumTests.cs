using Colibri.Core;

namespace Colibri.Tests;

public class DotNetEnumTests
{
    [Fact]
    public void BasicEnumTest()
    {
        var runtime = new ColibriRuntime();

        var program = "(defenum MyEnum Foo Bar Baz)\n(cast MyEnum/Bar Int32)";

        var result = runtime.EvaluateProgram(program);

        Assert.NotNull(result);
        Assert.Equal(1, result);
    }
}