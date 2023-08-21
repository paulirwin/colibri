namespace Colibri.Core;

public class ExitEventArgs : EventArgs
{
    public ExitEventArgs(int exitCode)
    {
        ExitCode = exitCode;
    }

    public int ExitCode { get; }
}