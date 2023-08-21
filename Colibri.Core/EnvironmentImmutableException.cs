namespace Colibri.Core;

public class EnvironmentImmutableException : Exception
{
    public EnvironmentImmutableException(string binding)
        : base($"Immutable binding: {binding}")
    {
        Binding = binding;
    }
    
    public string Binding { get; }
}