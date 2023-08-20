using System.CommandLine;
using Colibri.Core;

namespace Colibri;

public class Program
{
    public static int Main(string[] args)
    {
        var fileOption = new Option<FileInfo>("--file", "A Colibri Lisp file to execute.");
        
        var rootCommand = new RootCommand("A modern Lisp-based language in .NET.")
        {
            fileOption,
        };

        rootCommand.SetHandler(FileHandler, fileOption);

        return rootCommand.Invoke(args);
    }

    private static void FileHandler(FileInfo? file)
    {
        if (file == null)
        {
            Repl.RunRepl();
        }
        else
        {
            string text = File.ReadAllText(file.FullName);

            var runtime = new ColibriRuntime();

            runtime.EvaluateProgram(text);
        }
    }
}