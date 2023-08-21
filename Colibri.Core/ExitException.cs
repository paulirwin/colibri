namespace Colibri.Core;

public class ExitException : Exception
{
    public ExitException(int exitCode)
    {
        ExitCode = exitCode;
    }
    
    public int ExitCode { get; }
}