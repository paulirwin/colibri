﻿using System.Collections;

namespace Colibri.Core;

public sealed class Nil : Pair, IList<object>
{
    public static readonly Nil Value = new();

    private static readonly object[] _empty = Array.Empty<object>();

    private Nil()
    {
    }

    // ReSharper disable once UnusedParameter.Global
    public static implicit operator object[](Nil _) => _empty;

    // ReSharper disable once UnusedParameter.Global
    public static implicit operator bool(Nil _) => false;

    public override int GetHashCode() => _empty.GetHashCode();

    public override bool Equals(object? obj) 
        => obj switch
        {
            Nil => true,
            object[] {Length: 0} => true,
            null => true,
            false => true,
            _ => false
        };

    public IEnumerator<object> GetEnumerator() => ((IEnumerable<object>)_empty).GetEnumerator();

    public override string ToString() => "()";

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(object item) => throw new NotImplementedException();

    public void Clear() => throw new NotImplementedException();

    public bool Contains(object item) => false;

    public void CopyTo(object[] array, int arrayIndex)
    {
    }

    public bool Remove(object item) => throw new NotImplementedException();

    public int Count => 0;

    public bool IsReadOnly => true;
        
    public int IndexOf(object item) => -1;

    public void Insert(int index, object item) => throw new NotImplementedException();

    public void RemoveAt(int index) => throw new NotImplementedException();

    public object this[int index]
    {
        get => _empty[index];
        set => throw new NotImplementedException();
    }
}