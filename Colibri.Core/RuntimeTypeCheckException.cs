namespace Colibri.Core;

public abstract class RuntimeTypeCheckException : Exception
{
    protected RuntimeTypeCheckException(string message)
        : base(message)
    {
    }
}