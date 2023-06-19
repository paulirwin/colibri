namespace Colibri.Core;

public interface IInvokable
{
    object? Invoke(ColibriRuntime runtime, Scope scope, object?[] args);
}