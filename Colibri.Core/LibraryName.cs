namespace Colibri.Core;

public class LibraryName
{
    public LibraryName(params object[] identifiers) 
        : this((IEnumerable<object>) identifiers)
    {
    }
    
    public LibraryName(IEnumerable<object> identifiers)
    {
        var identifierList = identifiers.ToList();
        
        if (identifierList.Any(identifier => identifier is not string && identifier is not >= 0))
        {
            throw new ArgumentException("Library name must be a list of identifiers or exact, non-negative integers.");
        }

        Identifiers = identifierList.AsReadOnly();
    }
    
    public IReadOnlyList<object> Identifiers { get; }

    public Node ToList() => List.FromNodes(Identifiers);

    public override string ToString() => $"({string.Join(" ", Identifiers)})";

    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var identifier in Identifiers)
        {
            hash.Add(identifier);
        }

        return hash.ToHashCode();
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is not LibraryName other)
        {
            return false;
        }

        if (other.Identifiers.Count != Identifiers.Count)
        {
            return false;
        }

        for (var i = 0; i < Identifiers.Count; i++)
        {
            if (!Equals(Identifiers[i], other.Identifiers[i]))
            {
                return false;
            }
        }

        return true;
    }

    public static LibraryName Parse(Pair pair)
    {
        var identifiers = pair
            .Select(i => i switch
            {
                Atom { AtomType: AtomType.Number, Value: >= 0 } atom => atom.Value,
                Symbol symbol => symbol.Value,
                string identifier => identifier,
                _ => throw new ArgumentException("Library name must be a list of identifiers or exact, non-negative integers.")
            });

        return new LibraryName(identifiers);
    }
}