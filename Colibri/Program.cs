using System.CommandLine;
using System.CommandLine.Invocation;
using System.Reflection;
using Antlr4.Runtime;
using Colibri.Core;

namespace Colibri;

public class Program
{
    public static int Main(string[] args)
    {
        var rootCommand = new RootCommand("A modern Lisp-based language in .NET.")
        {
            new Option<FileInfo>("--file", "A Colibri Lisp file to execute."),
        };

        rootCommand.Handler = CommandHandler.Create<FileInfo?>(FileHandler);

        return rootCommand.Invoke(args);
    }

    private static void FileHandler(FileInfo? file)
    {
        if (file == null)
        {
            RunRepl();
        }
        else
        {
            string text = File.ReadAllText(file.FullName);

            var runtime = new ColibriRuntime();

            runtime.EvaluateProgram(text);
        }
    }

    private static void RunRepl()
    {
        PrintSystemInfo();

        var options = new ReplOptions();

        var runtime = CreateRuntime(options);

        var visitor = new ColibriVisitor();

        string? programText = null;

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(programText == null ? ">>> " : "... ");
            Console.ForegroundColor = ConsoleColor.White;

            string? input = Console.ReadLine();

            if (input == null)
            {
                break; // happens with ctrl+c
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            if (programText == null)
            {
                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)
                    || input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (input.Equals("clear", StringComparison.OrdinalIgnoreCase)
                    || input.Equals("cls", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Clear();
                    continue;
                }

                if (input.Equals("reset", StringComparison.OrdinalIgnoreCase))
                {
                    runtime = CreateRuntime(options);
                    Console.WriteLine("Runtime environment reset to defaults.");
                    continue;
                }

                programText = input;
            }
            else
            {
                programText += " " + input;
            }

            Node? programNode = null;
            
            try
            {
                var lexer = new ColibriLexer(new AntlrInputStream(programText));
                var parser = new ColibriParser(new CommonTokenStream(lexer));

                programNode = visitor.Visit(parser.prog());
            }
            catch (IncompleteParseException)
            {
            }
            catch (Exception ex)
            {
                PrintException(ex);
                continue;
            }

            if (programNode != null)
            {
                EvaluateAndPrint(runtime, options, programNode);
                programText = null;
            }
        }
    }

    private static void EvaluateAndPrint(ColibriRuntime runtime, ReplOptions options, Node programNode)
    {
        if (options.ShowAst)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("AST: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(programNode);
        }

        try
        {
            object? result = runtime.EvaluateProgram(programNode);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("-> ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(OutputFormatter.FormatRepl(result) ?? "null");
        }
        catch (Exception ex)
        {
            PrintException(ex);
        }
    }

    private static void PrintException(Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write("ERROR: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(ex.Message);
    }

    private static void PrintSystemInfo()
    {
        Console.WriteLine($"Colibri Lisp v{typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown"}");

        Console.WriteLine($".NET {Environment.Version}, {Environment.OSVersion}");
            
        Console.WriteLine();
    }

    private static ColibriRuntime CreateRuntime(ReplOptions options)
    {
        var runtime = new ColibriRuntime();

        // HACK.PI: use proper symbols to avoid polluting globals
        runtime.RegisterGlobal("show-ast", nameof(options.ShowAst));

        runtime.RegisterGlobalFunction("repl-config!", cargs =>
        {
            var prop = typeof(ReplOptions).GetProperty(cargs[0]?.ToString() ?? "unknown", BindingFlags.Public | BindingFlags.Instance);

            if (prop == null)
            {
                throw new ArgumentException("Unknown repl-config! property");
            }

            prop.SetValue(options, cargs[1]);

            return Nil.Value;
        });

        return runtime;
    }
}