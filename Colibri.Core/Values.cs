using System.Collections;

namespace Colibri.Core;

public class Values : IEnumerable<object?>
{
    private readonly IEnumerable<object?> _values;

    // ReSharper disable once UnusedMember.Global - Preserved for public API
    public Values(IEnumerable<object?> values)
    {
        _values = values;
    }

    public Values(params object?[] values)
    {
        _values = values;
    }

    public IEnumerator<object?> GetEnumerator() => _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}