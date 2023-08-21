namespace Colibri.Core.Macros;

public static class EnvironmentMacros
{
    public static object InteractionEnvironment(ColibriRuntime runtime, Scope scope, object?[] args) 
        => runtime.UserScope;
}