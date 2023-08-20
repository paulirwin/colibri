using Microsoft.CodeAnalysis.CSharp;

namespace Colibri.Core;

public class Symbol : Node
{
    public Symbol(string value, bool escaped = false)
    {
        Value = string.Intern(value);
        Escaped = escaped;
    }

    public string Value { get; }

    public bool Escaped { get; }

    public override string ToString() => Escaped ? $"|{SymbolDisplay.FormatLiteral(Value, false)}|" : Value;

    protected bool Equals(Symbol other) => Value == other.Value;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Symbol) obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator Symbol(string s) => new(s);
}