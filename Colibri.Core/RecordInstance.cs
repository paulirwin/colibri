namespace Colibri.Core;

public record RecordInstance(RecordTypeDefinition RecordType)
{
    public IDictionary<Symbol, object?> Fields { get; } = new Dictionary<Symbol, object?>();
}