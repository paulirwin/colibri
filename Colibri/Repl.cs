using System.Reflection;
using Antlr4.Runtime;
using Colibri.Core;
using Colibri.PromptConfig;
using PrettyPrompt;
using PrettyPrompt.Consoles;
using PrettyPrompt.Highlighting;

namespace Colibri;

public class Repl
{
    private readonly ReplOptions _options;
    private ColibriRuntime _runtime;
    private IPrompt _prompt;
    private readonly PromptConfiguration _promptConfig;

    public Repl(ReplOptions options)
    {
        _options = options;
        _runtime = CreateRuntime(options);
        _promptConfig = ReplPromptConfig.GetPromptConfig();
        _prompt = CreatePrompt();
    }

    public async Task RunRepl()
    {
        PrintIntro();

        var visitor = new ColibriVisitor();

        string? programText = null;

        while (true)
        {
            _promptConfig.Prompt = programText == null ? ">>> " : "... ";
            
            var promptResult = await _prompt.ReadLineAsync();

            if (promptResult is ExitAppResult)
            {
                break;
            }
            
            if (promptResult is KeyPressCallbackResult callbackResult)
            {
                Console.WriteLine(callbackResult.Output);
                continue;
            }

            var input = promptResult.Text;

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
                    _runtime = CreateRuntime(_options);
                    _prompt = CreatePrompt();
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
                
                lexer.RemoveErrorListeners();
                parser.RemoveErrorListeners();

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
                EvaluateAndPrint(_runtime, _options, programNode);
                programText = null;
            }
        }
    }

    private static void EvaluateAndPrint(ColibriRuntime runtime, ReplOptions options, Node programNode)
    {
        if (options.ShowAst)
        {
            Console.Write($"{AnsiColor.Yellow.GetEscapeSequence()}AST: {AnsiEscapeCodes.Reset}");
            Console.WriteLine(programNode);
        }

        try
        {
            object? result = runtime.EvaluateProgram(programNode);

            Console.Write($"{AnsiColor.BrightBlack.GetEscapeSequence()}-> {AnsiEscapeCodes.Reset}");
            Console.WriteLine(OutputFormatter.FormatRepl(result) ?? "null");
        }
        catch (Exception ex)
        {
            PrintException(ex);
        }
    }
    
    private static void PrintException(Exception ex)
    {
        Console.Write($"{AnsiColor.Red.GetEscapeSequence()}ERROR: {AnsiEscapeCodes.Reset}");
        Console.WriteLine(ex.Message);
    }

    private static void PrintIntro()
    {
        Console.WriteLine($"{AnsiColor.BrightWhite.GetEscapeSequence()}" +
                          $"Colibri Lisp " +
                          $"{AnsiColor.White.GetEscapeSequence()}" +
                          $"v{typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown"}{AnsiEscapeCodes.Reset}");
            
        Console.WriteLine($"Type {AnsiColor.BrightRed.GetEscapeSequence()}exit{AnsiEscapeCodes.Reset} to exit, " +
                          $"{AnsiColor.BrightBlue.GetEscapeSequence()}clear{AnsiEscapeCodes.Reset} to clear the screen, " +
                          $"or {AnsiColor.BrightYellow.GetEscapeSequence()}reset{AnsiEscapeCodes.Reset} to reset the runtime environment.");
        Console.WriteLine();
    }

    private static ColibriRuntime CreateRuntime(ReplOptions options)
    {
        var runtime = new ColibriRuntime(new RuntimeOptions
        {
            ImportStandardLibrary = !options.R5RS,
        });
        
        if (options.R5RS)
        {
            var r5rs = StandardLibraries.R5RS;
            runtime.ImportLibrary(runtime.GlobalScope, new ImportSet(r5rs));
        }

        // HACK.PI: use proper symbols to avoid polluting globals
        runtime.RegisterGlobal("show-ast", nameof(options.ShowAst));

        runtime.RegisterGlobalFunction("repl-config!", args =>
        {
            var prop = typeof(ReplOptions).GetProperty(args[0]?.ToString() ?? "unknown", BindingFlags.Public | BindingFlags.Instance);

            if (prop == null)
            {
                throw new ArgumentException("Unknown repl-config! property");
            }

            prop.SetValue(options, args[1]);

            return Nil.Value;
        });
        
        runtime.Exit += (_, args) =>
        {
            Console.Out.Flush();
            Console.Error.Flush();
            Console.WriteLine("The REPL has exited. Press any key to continue...");
            Console.ReadKey(true);
            Environment.Exit(args.ExitCode);
        };

        return runtime;
    }
    
    private Prompt CreatePrompt()
    {
        return new Prompt(
            callbacks: new ReplPromptCallbacks(_runtime),
            configuration: _promptConfig,
            persistentHistoryFilepath: ReplPromptConfig.GetPromptHistoryDirectory()
        );
    }
}