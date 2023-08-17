namespace Colibri.Core;

public record RecordTypeDefinition(Symbol Name, Symbol ConstructorName, Symbol PredicateName)
{
    public IList<Symbol> ConstructorParameters { get; } = new List<Symbol>();

    public IList<RecordFieldDefinition> Fields { get; } = new List<RecordFieldDefinition>();
}