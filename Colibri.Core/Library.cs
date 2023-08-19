namespace Colibri.Core;

public class Library
{
    public Library(IReadOnlyDictionary<string, object?> definitions)
    {
        Definitions = definitions;
    }

    public IReadOnlyDictionary<string, object?> Definitions { get; }
    
    public string? EmbeddedResourceName { get; set; }
}