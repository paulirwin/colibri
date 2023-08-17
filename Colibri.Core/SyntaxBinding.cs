namespace Colibri.Core;

public class SyntaxBinding : Symbol
{
    public SyntaxBinding(Symbol symbol, Scope scope)
        : base(symbol.Value)
    {
        Scope = scope;
    }

    public Scope Scope { get; }

    protected bool Equals(SyntaxBinding other) => base.Equals(other) && Scope.Equals(other.Scope);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((SyntaxBinding)obj);
    }

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Scope);
}