using System.Collections;
using System.Dynamic;

namespace Colibri.Core;

public class AssociativeArray : Node,
    IList<AssociativeArray.Element>,
    IReadOnlyList<AssociativeArray.Element>,
    IDynamicMetaObjectProvider,
    IDictionary<object, object?>
{
    public class Element : Node
    {
        public Element(object key, object? value)
        {
            Key = key;
            Value = value;
        }
        
        public object Key { get; }
        
        public object? Value { get; }

        public override string ToString() => $"{Key} => {Value}";

        protected bool Equals(Element other) => Key.Equals(other.Key) && Equals(Value, other.Value);

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Element)obj);
        }

        public override int GetHashCode() => HashCode.Combine(Key, Value);

        public KeyValuePair<object, object?> ToKeyValuePair() => new(Key, Value);

        public static implicit operator KeyValuePair<object, object?>(Element element)
            => element.ToKeyValuePair();

        public static explicit operator Element(KeyValuePair<object, object?> kvp)
            => new(kvp.Key, kvp.Value);

        public Pair ToPair() => List.FromNodes(Key, Value);
    }
    
    private readonly List<Element> _elements;
    private readonly Dictionary<object, int> _indexMap = new();

    public AssociativeArray()
    {
        _elements = new List<Element>();
    }

    public AssociativeArray(IEnumerable<Element> elements)
    {
        _elements = new List<Element>(elements);
    }
    
    public DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
    {
        throw new NotImplementedException("TODO");
    }

    IEnumerator<KeyValuePair<object, object?>> IEnumerable<KeyValuePair<object, object?>>.GetEnumerator() 
        => _elements.Select(i => i.ToKeyValuePair()).GetEnumerator();

    public IEnumerator<Element> GetEnumerator() => _elements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _elements.GetEnumerator();

    public void Add(Element item)
    {
        _elements.Add(item);
        int index = _elements.Count - 1;

        _indexMap[item.Key] = index;
    }

    public void Add(KeyValuePair<object, object?> item) => Add((Element)item);

    public void Clear()
    {
        _elements.Clear();
        _indexMap.Clear();
    }

    public bool Contains(KeyValuePair<object, object?> item) => Contains((Element)item);

    public void CopyTo(KeyValuePair<object, object?>[] array, int arrayIndex)
    {
        throw new NotSupportedException();
    }

    public bool Remove(KeyValuePair<object, object?> item) => Remove((Element)item);

    public bool Contains(Element item) => _elements.Contains(item);

    public void CopyTo(Element[] array, int arrayIndex) => _elements.CopyTo(array, arrayIndex);

    public bool Remove(Element item)
    {
        _indexMap.Remove(item.Key);
        return _elements.Remove(item);
    }

    public int Count => _elements.Count;

    public bool IsReadOnly => ((ICollection<Element>)_elements).IsReadOnly;

    public int IndexOf(Element item) => _elements.IndexOf(item);

    public void Insert(int index, Element item)
    {
        _elements.Insert(index, item);
        _indexMap[item.Key] = index;
    }

    public void RemoveAt(int index)
    {
        var item = _elements[index];
        _elements.RemoveAt(index);
        _indexMap.Remove(item.Key);
    }

    public Element this[int index]
    {
        get => _elements[index];
        set => _elements[index] = value;
    }

    public void Add(object key, object? value) => Add(new Element(key, value));

    public bool ContainsKey(object key) => _indexMap.ContainsKey(key);

    public bool Remove(object key)
    {
        if (!_indexMap.TryGetValue(key, out var index))
        {
            return false;
        }

        RemoveAt(index);
        return true;
    }

    public bool TryGetValue(object key, out object? value)
    {
        if (!_indexMap.TryGetValue(key, out var index))
        {
            value = null;
            return false;
        }

        var element = _elements[index];
        value = element.Value;
        return true;
    }

    public object? this[object key]
    {
        get
        {
            if (!TryGetValue(key, out var value))
            {
                throw new KeyNotFoundException();
            }

            return value;
        }
        set
        {
            if (_indexMap.TryGetValue(key, out var index))
            {
                _elements[index] = new Element(key, value);
            }
            else
            {
                Add(new Element(key, value));
            }
        }
    }

    public ICollection<object> Keys => _elements.Select(i => i.Key).ToList();

    public ICollection<object?> Values => _elements.Select(i => i.Value).ToList();

    public Pair ToPair() => List.FromNodes(_elements.Select(i => i.ToPair()));
    
    public static implicit operator Pair(AssociativeArray arr) => arr.ToPair();
}