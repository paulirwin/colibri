namespace Colibri.Core;

public class AuxiliarySyntax
{
    public AuxiliarySyntax(string value) => Value = value;

    public string Value { get; }

    public override string ToString() => Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override bool Equals(object? obj)
        => obj is AuxiliarySyntax other
           && string.Equals(Value, other.Value);
}