﻿namespace Colibri.Core.Macros;

public static class BooleanMacros
{
    public static object? And(ColibriRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length == 0)
        {
            return true;
        }

        object? value = false;

        for (var index = 0; index < args.Length; index++)
        {
            var arg = args[index];
            
            value = arg is Node node
                ? node is Pair pair and not Nil && index == args.Length - 1
                    ? ColibriRuntime.TailCall(scope, pair)
                    : runtime.Evaluate(scope, node)
                : arg;

            if (!value.IsTruthy())
                return false;
        }

        return value;
    }

    public static object? Or(ColibriRuntime runtime, Scope scope, object?[] args)
    {
        for (var index = 0; index < args.Length; index++)
        {
            var arg = args[index];
            
            object? value = arg is Node node
                ? node is Pair pair and not Nil && index == args.Length - 1
                    ? ColibriRuntime.TailCall(scope, pair)
                    : runtime.Evaluate(scope, node)
                : arg;

            if (value.IsTruthy())
                return value;
        }

        return false;
    }

    /// <summary>
    /// The test is evaluated, and if it evaluates to
    /// a true value, the expressions are evaluated in order.
    /// </summary>
    /// <remarks>
    /// Scheme R7RS states that the return value is unspecified. In order to support tail recursion,
    /// we are returning the result of the last expression if the test evaluates true, otherwise nil. 
    /// </remarks>
    /// <param name="runtime">The current runtime.</param>
    /// <param name="scope">The current scope (environment)</param>
    /// <param name="args">The arguments to `when`</param>
    /// <returns>Returns the result of the last expression, or nil.</returns>
    public static object? When(ColibriRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length < 2 || args[0] is not Node test)
        {
            throw new ArgumentException("when requires at least a test and an expression argument");
        }

        var result = runtime.Evaluate(scope, test);

        if (!result.IsTruthy())
        {
            return Nil.Value;
        }

        result = EvaluateExpressionsWithTailCall(runtime, scope, args, 1, result);

        return result;
    }

    public static object? Unless(ColibriRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length < 2 || args[0] is not Node test)
        {
            throw new ArgumentException("unless requires at least a test and an expression argument");
        }

        var result = runtime.Evaluate(scope, test);

        if (result.IsTruthy())
        {
            return Nil.Value;
        }

        result = EvaluateExpressionsWithTailCall(runtime, scope, args, 1, result);

        return result;
    }

    private static object? EvaluateExpressionsWithTailCall(ColibriRuntime runtime, Scope scope, IReadOnlyList<object?> args, int startArgIndex, object? result)
    {
        for (int i = startArgIndex; i < args.Count; i++)
        {
            var arg = args[i];

            if (arg is Node node)
            {
                result = i == args.Count - 1 && node is Pair pair and not Nil
                    ? ColibriRuntime.TailCall(scope, pair) 
                    : runtime.Evaluate(scope, node);
            }
        }

        return result;
    }
}