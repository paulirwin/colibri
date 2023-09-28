using System.Collections;

namespace Colibri.Core;

public class AdaptiveList : Node, 
    IList<object?>,
    IReadOnlyList<object?>
{
    private readonly List<object?> _elements;

    public AdaptiveList()
    {
        _elements = new List<object?>();
    }

    public AdaptiveList(IEnumerable<object?> elements)
    {
        _elements = new List<object?>(elements);
    }

    public IEnumerator<object?> GetEnumerator() => _elements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(object? item) => _elements.Add(item);

    public void Clear() => _elements.Clear();

    public bool Contains(object? item) => _elements.Contains(item);

    public void CopyTo(object?[] array, int arrayIndex) => _elements.CopyTo(array, arrayIndex);

    public bool Remove(object? item) => _elements.Remove(item);

    public int Count => _elements.Count;

    public bool IsReadOnly => ((ICollection<object?>)_elements).IsReadOnly;

    public int IndexOf(object? item) => _elements.IndexOf(item);

    public void Insert(int index, object? item) => _elements.Insert(index, item);

    public void RemoveAt(int index) => _elements.RemoveAt(index);

    public object? this[int index]
    {
        get => _elements[index];
        set => _elements[index] = value;
    }

    public static implicit operator Pair(AdaptiveList list) => List.FromNodes(list._elements);
    
    public static implicit operator Vector(AdaptiveList list) => new(list._elements);

    public static implicit operator List<object?>(AdaptiveList list) => new(list._elements);

    public static implicit operator object?[](AdaptiveList list) => list._elements.ToArray();

    public static implicit operator HashSet<object?>(AdaptiveList list) => new(list._elements);
}