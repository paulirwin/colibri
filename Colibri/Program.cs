using System.CommandLine;
using Colibri.Core;

namespace Colibri;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var fileOption = new Option<FileInfo>("--file", "A Colibri Lisp file to execute.");
        
        var rootCommand = new RootCommand("A modern Lisp-based language in .NET.")
        {
            fileOption,
        };

        rootCommand.SetHandler(FileHandler, fileOption);

        return await rootCommand.InvokeAsync(args);
    }

    private static async Task FileHandler(FileInfo? file)
    {
        if (file == null)
        {
            await new Repl(new ReplOptions()).RunRepl();
        }
        else
        {
            string text = await File.ReadAllTextAsync(file.FullName);

            var runtime = new ColibriRuntime();

            runtime.EvaluateProgram(text);
        }
    }
}