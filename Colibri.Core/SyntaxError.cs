namespace Colibri.Core;

public class SyntaxError : Exception
{
    public SyntaxError(string message, IList<object?>? args = null)
        : base(message)
    {
        Args = args ?? new List<object?>();
    }

    public IList<object?> Args { get; }
}