namespace Colibri.Core;

public class ArgumentTypeCheckException : RuntimeTypeCheckException
{
    public ArgumentTypeCheckException(string argName, Type expected, Type? actual)
        : base($"Expected type {expected} for argument {argName} but got {actual?.ToString() ?? "null"}.")
    {
    }
}