using System.Text;

namespace Colibri.Core.Expressions;

public static class ProcessContextExpressions
{
    public static object? GetEnvironmentVariable(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("get-environment-variable requires one argument");
        }

        return args[0] switch
        {
            string s => Environment.GetEnvironmentVariable(s),
            StringBuilder sb => Environment.GetEnvironmentVariable(sb.ToString()),
            _ => throw new ArgumentException("name must be a string")
        };
    }

    public static object EmergencyExit(object?[] args)
    {
        var exitCode = ParseExitCode(args);

        Environment.Exit(exitCode);
        return null;
    }

    private static int ParseExitCode(object?[] args)
    {
        int exitCode;

        if (args.Length == 0)
        {
            exitCode = 0;
        }
        else if (args[0] is bool b)
        {
            exitCode = b ? 0 : -1;
        }
        else
        {
            try
            {
                exitCode = Convert.ToInt32(args[0]);
            }
            catch
            {
                exitCode = -1;
            }
        }

        return exitCode;
    }

    public static object Exit(object?[] args)
    {
        int code = ParseExitCode(args);
        throw new ExitException(code);
    }

    public static object GetEnvironmentVariables(object?[] args)
    {
        var list = new List<Pair>();

        var enumerator = Environment.GetEnvironmentVariables().GetEnumerator();

        while (enumerator.MoveNext())
        {
            list.Add(new Pair(enumerator.Key, enumerator.Value));
        }

        return List.FromNodes(list);
    }

    public static object CommandLine(object?[] args)
    {
        return List.FromNodes(Environment.GetCommandLineArgs().Cast<object>());
    }
}