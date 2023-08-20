namespace Colibri.Core;

public class Library
{
    public Library(IReadOnlyDictionary<string, object?> runtimeDefinitions)
    {
        RuntimeDefinitions = runtimeDefinitions;
        Exports = new HashSet<string>(runtimeDefinitions.Keys);
    }
    
    public Library(
        IReadOnlyDictionary<string, object?> runtimeDefinitions,
        IEnumerable<string> additionalExports)
    {
        RuntimeDefinitions = runtimeDefinitions;
        
        var exports = new HashSet<string>(runtimeDefinitions.Keys);
        exports.UnionWith(additionalExports);
        Exports = exports;
    }

    public IReadOnlyDictionary<string, object?> RuntimeDefinitions { get; }
    
    public IReadOnlySet<string> Exports { get; }
    
    public string? EmbeddedResourceName { get; init; }
}