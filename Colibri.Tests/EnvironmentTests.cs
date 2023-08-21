using Colibri.Core;

namespace Colibri.Tests;

public class EnvironmentTests
{
    [Fact]
    public void InteractionEnvironmentTest()
    {
        const string program = @"
define x 42
eval 'x (interaction-environment)
";
        
        var runtime = new ColibriRuntime();
        
        var result = runtime.EvaluateProgram(program);
        
        Assert.Equal(42, result);
        
        var env = runtime.EvaluateProgram("(interaction-environment)");
        
        Assert.Same(env, runtime.UserScope);
    }
}