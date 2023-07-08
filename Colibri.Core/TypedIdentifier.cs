namespace Colibri.Core;

public class TypedIdentifier : Node
{
    public TypedIdentifier(Symbol identifier, Node type)
    {
        Identifier = identifier;
        Type = type;
    }

    public Symbol Identifier { get; }
    
    public Node Type { get; }
    
    public override string ToString() => $"{Identifier}: {Type}";
}