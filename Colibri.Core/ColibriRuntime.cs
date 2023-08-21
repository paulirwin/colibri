using System.Collections;
using System.Diagnostics;
using System.Reflection;
using Antlr4.Runtime;
using Colibri.Core.Expressions;
using Colibri.Core.Macros;

namespace Colibri.Core;

public class ColibriRuntime
{
    /// <summary>
    /// The default collection of system "macros."
    /// </summary>
    /// <remarks>
    /// Note that this use of the term "macro" is different than the Scheme/Lisp use of the term, and
    /// they are not intended to mean the same thing. Here, macros are .NET functions that have access
    /// to the runtime, the current environment, and the AST of the expression. Therefore, they have
    /// the power to conditionally choose not to evaluate code, manipulate the environment (scope), or
    /// otherwise cause mayhem. Normal expressions, on the other hand, can only operate with their
    /// already-evaluated inputs. Ideally, most functions would be implemented as expressions rather
    /// than macros, unless access to the runtime, AST, or environment is needed.
    /// 
    /// Also note that "unquote" and "unquote-splicing" are implemented in ColibriVisitor, as they
    /// are special forms.
    /// </remarks>
    private static readonly IReadOnlyDictionary<string, MacroExpression> _systemMacros = new Dictionary<string, MacroExpression>
    {
        ["++!"] = MathMacros.Increment,
        ["--!"] = MathMacros.Decrement,
        ["def"] = CoreMacros.Define,
        ["defenum"] = EmitMacros.DefineEnum,
        ["defrecord"] = EmitMacros.DefineRecord,
        ["defun"] = CoreMacros.Defun,
        ["fn"] = CoreMacros.Defun, // Alias for defun
        ["from"] = LispINQMacros.From,
        ["import"] = CoreMacros.Import,
        ["new"] = InteropMacros.New,
        ["use"] = InteropMacros.Use,
    };

    private static readonly IReadOnlyDictionary<string, Expression> _systemFunctions = new Dictionary<string, Expression>
    {
        ["%"] = MathExpressions.Modulo,
        ["**"] = MathExpressions.Power,
        [">>"] = MathExpressions.ShiftRight,
        ["<<"] = MathExpressions.ShiftLeft,
        ["count"] = DynamicExpressions.Count,
        ["dec"] = MathExpressions.Decrement,
        ["cast"] = InteropExpressions.Cast,
        ["convert"] = InteropExpressions.Convert,
        ["get"] = DynamicExpressions.Get,
        ["inc"] = MathExpressions.Increment,
        ["ln"] = MathExpressions.Ln,
        ["match?"] = RegularExpressions.IsMatch,
        ["pow"] = MathExpressions.Power,
        ["print"] = StringExpressions.Print,
        ["println"] = StringExpressions.PrintLn,
        ["pr"] = StringExpressions.Pr,
        ["prn"] = StringExpressions.Prn,
        ["range"] = ListExpressions.Range,
        ["simplify"] = RationalExpressions.Simplify,
        ["->string"] = StringExpressions.ConvertToString,
        ["typeof"] = TypeExpressions.TypeOf,
    };

    private static readonly IReadOnlyDictionary<string, object?> _systemGlobals = new Dictionary<string, object?>
    {
        ["pi"] = Math.PI,
        ["e"] = Math.E,
        ["tau"] = Math.Tau,
        ["i8"] = typeof(sbyte),
        ["i16"] = typeof(short),
        ["i32"] = typeof(int),
        ["i64"] = typeof(long),
        ["u8"] = typeof(byte),
        ["u16"] = typeof(ushort),
        ["u32"] = typeof(uint),
        ["u64"] = typeof(ulong),
        ["f32"] = typeof(float),
        ["f64"] = typeof(double),
        ["char"] = typeof(char),
        ["str"] = typeof(string),
        ["bool"] = typeof(bool),
        ["void"] = typeof(void),
        ["dec"] = typeof(decimal),
        ["obj"] = typeof(object),
    };

    public ColibriRuntime(RuntimeOptions? options = null)
    {
        options ??= new RuntimeOptions();
        
        GlobalScope = new Scope(options.MaxStackDepth);
        GlobalScope.AddAllFrom(_systemMacros);
        GlobalScope.AddAllFrom(_systemFunctions);
        GlobalScope.AddAllFrom(_systemGlobals);

        LoadStandardLibraries(import: options.ImportStandardLibrary);

        UserScope = GlobalScope.CreateChildScope();
    }
    
    public Scope GlobalScope { get; }

    public Scope UserScope { get; }

    private void LoadStandardLibraries(bool import)
    {
        foreach (var library in StandardLibraries.Libraries)
        {
            GlobalScope.AddLibrary(library.Name, library.Library);

            if (import)
            {
                ImportLibrary(GlobalScope, new ImportSet(library.Library));
            }
        }
    }

    public void ImportLibrary(
        Scope scope, 
        ImportSet importSet)
    {
        var childScope = scope.CreateChildScope();
        
        foreach (var definition in importSet.Library.RuntimeDefinitions)
        {
            childScope.Define(definition.Key, definition.Value);
        }

        if (importSet.Library.EmbeddedResourceName is string resourceName)
        {
            EvaluateLibraryResource(resourceName, childScope);
        }
        
        foreach (var (importAs, original) in importSet.Imports)
        {
            scope.DefineOrSet(importAs, childScope.Env[original]);
        }
    }

    public void RegisterGlobal(string symbol, object? value)
    {
        GlobalScope.Define(symbol, value);
    }

    public void RegisterGlobalFunction(string symbol, Expression func)
    {
        GlobalScope.Define(symbol, func);
    }

    public object? EvaluateProgram(string program) => EvaluateProgram(UserScope, program);

    public object? EvaluateProgram(Scope scope, string program)
    {
        var prog = ParseProgramText(program);

        return Evaluate(scope, prog);
    }

    public static Node ParseProgramText(string program)
    {
        var lexer = new ColibriLexer(new AntlrInputStream(program));
        var parser = new ColibriParser(new CommonTokenStream(lexer));
        var visitor = new ColibriVisitor();
            
        var prog = visitor.Visit(parser.prog());
        return prog;
    }

    public object? EvaluateProgram(object? node) => Evaluate(UserScope, node);

    private object? Quote(Scope scope, object? node)
    {
        return node switch
        {
            Program program => program.Children.Select(i => Quote(scope, i)).ToArray(),
            Vector vector => new Vector(vector.Select(i => Quote(scope, i))),
            Bytevector bv => bv, // TODO: is this correct?
            Pair pair => QuotePair(Quote(scope, pair.Car), Quote(scope, pair.Cdr)),
            Symbol symbol => symbol,
            Atom atom => atom.Value,
            Quote quote => Quote(scope, quote.Value),
            Quasiquote quote => Quote(scope, quote.Value),
            Unquote { Splicing: false } unquote => Evaluate(scope, unquote.Value),
            Unquote { Splicing: true } unquote => EvaluateUnquoteSplicing(scope, unquote.Value),
            Nil nil => nil,
            AuxiliarySyntax aux => aux.Value,
            _ => throw new ArgumentOutOfRangeException(nameof(node))
        };
    }

    private static Node QuotePair(object? car, object? cdr)
    {
        if (car is not Splice && cdr is not Splice)
        {
            return new Pair(car, cdr);
        }

        var values = new List<object?>();

        if (car is Splice carSplice)
        {
            values.AddRange(carSplice.Values);
        }
        else
        {
            values.Add(car);
        }

        if (cdr is Splice cdrSplice)
        {
            values.AddRange(cdrSplice.Values);
        }
        else if (cdr is not Nil)
        {
            values.Add(cdr);
        }

        return List.FromNodes(values);
    }

    private Splice EvaluateUnquoteSplicing(Scope scope, Node unquoteValue)
    {
        var result = Evaluate(scope, unquoteValue);

        if (result is not IEnumerable enumerable)
        {
            throw new InvalidOperationException("Result of unquote-splicing operation is not enumerable");
        }

        return new Splice(enumerable.Cast<object?>());
    }

    public object? Evaluate(Scope scope, object? node) => Evaluate(scope, node, null);

    private object? Evaluate(Scope scope, object? node, int? arity)
    {
        return node switch
        {
            Program program => EvaluateProgram(scope, program),
            Vector vector => EvaluateVector(scope, vector),
            Bytevector bv => bv,
            Pair pair => EvaluateExpression(scope, pair),
            SyntaxBinding binding => TryEvaluateSymbol(binding.Scope, binding, arity, out var value) ? value : EvaluateSymbol(scope, binding, arity),
            Symbol symbol => EvaluateSymbol(scope, symbol, arity),
            Atom atom => atom.Value,
            Quote quote => Quote(scope, quote),
            Quasiquote quote => Quote(scope, quote),
            RegexLiteral regex => regex.ToRegex(),
            Nil nil => nil,
            StatementBlock stmtBlock => EvaluateStatementBlock(scope, stmtBlock),
            AuxiliarySyntax aux => throw new InvalidOperationException($"Invalid use of auxiliary syntax: {aux}"),
            _ => node
        };
    }

    private object? EvaluateStatementBlock(Scope scope, StatementBlock stmtBlock)
    {
        object? yieldedValue = Nil.Value;
        
        foreach (var node in stmtBlock)
        {
            yieldedValue = Evaluate(scope, node);
        }

        return yieldedValue;
    }

    private Vector EvaluateVector(Scope scope, Vector vector)
    {
        var items = vector.Select(i => i is Node node ? Evaluate(scope, node) : i);

        return new Vector(items);
    }
        
    private static object? EvaluateSymbol(Scope scope, Symbol node, int? arity)
    {
        if (TryEvaluateSymbol(scope, node, arity, out var value))
        {
            return value;
        }

        throw new ArgumentException($"Unable to resolve symbol {node}");
    }

    private static bool TryEvaluateSymbol(Scope scope, Symbol node, int? arity, out object? value)
    {
        value = null;

        string? symbol = node.Value;

        switch (symbol)
        {
            case null or "null":
                return true;
            case "nil":
                value = Nil.Value;
                return true;
        }

        if (scope.TryResolve(symbol, out value))
        {
            if (value is AuxiliarySyntax aux)
            {
                throw new InvalidOperationException($"Invalid use of auxiliary syntax: {aux}");
            }
            
            return true;
        }
        
        return Interop.TryResolveSymbol(scope, symbol, arity, out value);
    }

    private object? EvaluateExpression(Scope scope, Pair pair)
    {
        var result = EvaluateExpression(scope, pair, false);

        while (result is TailCall tailCall)
        {
            result = EvaluateExpression(tailCall.Scope, tailCall.Node, true);
        }

        return result;
    }

    internal static TailCall TailCall(Scope scope, Pair pair)
    {
        return new TailCall(scope, pair);
    }

    private object? EvaluateExpression(Scope scope, Pair pair, bool isTailCall)
    {
        Debug.WriteLine(pair);
        
        switch (pair.Car)
        {
            case Nil:
                throw new InvalidOperationException("nil is not a function");
            case Symbol symbol when symbol.Value.StartsWith('.'):
            {
                var memberArgs = pair.Skip(1).Select(i => Evaluate(scope, i)).ToArray();

                return Interop.InvokeMember(scope, symbol.Value, memberArgs);
            }
        }

        int? arity = null;

        if (pair.Cdr is Pair { IsList: true } cdrList)
        {
            arity = cdrList.Count();
        }

        var op = Evaluate(scope, pair.Car, arity);

        switch (op)
        {
            case MacroExpression macro:
                return macro(this, scope, pair.Skip(1).ToArray());
            case Syntax syntax:
            {
                Debug.WriteLine("Evaluating syntax: {0}", pair);
                var node = syntax.Transform(pair.Skip(1).Cast<Node>().Select(i => BindSyntaxArgNode(scope, i)).ToArray());
                Debug.WriteLine("Syntax expanded to: {0}", node);
                return Evaluate(scope, node);
            }
        }

        var args = pair.Skip(1).Select(i => Evaluate(scope, i)).ToArray();

        if (isTailCall)
        {
            scope = scope.PopMergeScope();
        }

        return InvokeExpression(scope, op, args);
    }

    private static Node BindSyntaxArgNode(Scope scope, Node node)
    {
        return node switch
        {
            SyntaxBinding binding => binding, // pass-thru
            Symbol symbol => new SyntaxBinding(symbol, scope),
            Pair { Car: Node carNode, Cdr: Node cdrNode } => new Pair(BindSyntaxArgNode(scope, carNode), BindSyntaxArgNode(scope, cdrNode)),
            _ => node,
        };
    }

    public object? InvokePossibleTailCallExpression(Scope scope, object? expression, object?[] args)
    {
        var result = InvokeExpression(scope, expression, args);

        while (result is TailCall tailCall)
        {
            result = EvaluateExpression(tailCall.Scope, tailCall.Node, true);
        }

        return result;
    }

    public object? InvokeExpression(Scope scope, object? expression, object?[] args)
    {
        try
        {
            return expression switch
            {
                IInvokable invokable => invokable.Invoke(this, scope, args),
                MethodInfo method => method.Invoke(null, args),
                InteropStaticOverloadSet overloadSet => overloadSet.Invoke(args),
                MacroExpression macro => macro(this, scope, args),
                Expression expr => expr(args),
                Func<object?[], object?> expr => expr(args),
                Type genericType => genericType.MakeGenericType(args.Cast<Type>().ToArray()),
                _ => throw new InvalidOperationException($"Invalid operation: {expression}")
            };
        }
        catch (ExitException ex)
        {
            if (Exit is { } exitEvent)
            {
                exitEvent.Invoke(this, new ExitEventArgs(ex.ExitCode));
                return null;
            }

            throw;
        }
    }

    private object? EvaluateProgram(Scope scope, Program node)
    {
        object? result = null;

        foreach (var child in node.Children)
        {
            result = Evaluate(scope, child);
        }

        return result;
    }

    private void EvaluateLibraryResource(string name, Scope scope)
    {
        using var stream = typeof(ColibriRuntime).Assembly.GetManifestResourceStream(name);

        if (stream == null)
        {
            throw new InvalidOperationException($"Unable to find embedded resource with name {name}");
        }

        using var sr = new StreamReader(stream);

        string text = sr.ReadToEnd();

        var prog = ParseProgramText(text);

        Evaluate(scope, prog);
    }

    public event EventHandler<ExitEventArgs>? Exit;
}