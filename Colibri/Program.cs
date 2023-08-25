using System.CommandLine;
using Colibri.Core;

namespace Colibri;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var fileOption = new Option<FileInfo>("--file", "A Colibri Lisp file to execute");
        
        var r5rsOption = new Option<bool>("--r5rs", "Import the R5RS library instead of R7RS");
        
        var rootCommand = new RootCommand("A modern Lisp language in .NET")
        {
            fileOption,
            r5rsOption,
        };

        rootCommand.SetHandler(FileHandler, fileOption, r5rsOption);

        return await rootCommand.InvokeAsync(args);
    }

    private static async Task FileHandler(FileInfo? file, bool useR5RS)
    {
        if (file == null)
        {
            await new Repl(new ReplOptions
            {
                R5RS = useR5RS,
            }).RunRepl();
        }
        else
        {
            string text = await File.ReadAllTextAsync(file.FullName);

            var runtime = new ColibriRuntime(new RuntimeOptions
            {
                ImportStandardLibrary = !useR5RS,
            });
            
            if (useR5RS)
            {
                var r5rs = StandardLibraries.R5RS;
                runtime.ImportLibrary(runtime.GlobalScope, new ImportSet(r5rs));
            }

            runtime.EvaluateProgram(text);
        }
    }
}