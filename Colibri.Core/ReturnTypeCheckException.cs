namespace Colibri.Core;

public class ReturnTypeCheckException : RuntimeTypeCheckException
{
    public ReturnTypeCheckException(Type expected, Type? actual)
        : base($"Expected return type {expected} but got {actual?.ToString() ?? "null"}. Note that the code in this method has already been executed.")
    {
    }
}