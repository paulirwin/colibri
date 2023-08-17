namespace Colibri.Core;

public class SyntaxLiteral : Node
{
    public SyntaxLiteral(Symbol symbol)
    {
        Symbol = symbol;
    }

    public Symbol Symbol { get; }

    public override string ToString() => Symbol.ToString();

    protected bool Equals(SyntaxLiteral other) => Symbol.Equals(other.Symbol);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((SyntaxLiteral)obj);
    }

    public override int GetHashCode() => Symbol.GetHashCode();
}