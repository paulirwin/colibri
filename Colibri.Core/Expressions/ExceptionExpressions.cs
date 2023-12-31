﻿namespace Colibri.Core.Expressions;

public static class ExceptionExpressions
{
    public static object Raise(object?[] args)
    {
        throw args.Length switch
        {
            // TODO: should this be moved to a C# macro so that it can capture the stack trace?
            0 => new RaisedException(null),
            > 1 => new ArgumentException("raise requires zero or one arguments"),
            _ => new RaisedException(args[0])
        };
    }

    public static object Error(object?[] args)
    {
        throw args.Length switch
        {
            // TODO: should this be moved to a C# macro so that it can capture the stack trace?
            0 => new ErrorException(null),
            1 => new ErrorException(args[0]?.ToString()),
            _ => new ErrorException(args[0]?.ToString(), args.Skip(1))
        };
    }
    
    public static object SyntaxError(object?[] args)
    {
        throw args.Length switch
        {
            // TODO: should this be moved to a C# macro so that it can capture the stack trace?
            0 => throw new ArgumentException("syntax-error requires at least one argument"),
            1 => new SyntaxError(args[0]?.ToString() ?? "null"),
            _ => new SyntaxError(args[0]?.ToString() ?? "null", args.Skip(1).ToList())
        };
    }

    /// <summary>
    /// Determines if the argument is an error object (Exception).
    /// </summary>
    /// <remarks>
    /// Scheme R7RS says: "Returns #t if obj is an object created by error or one
    /// of an implementation-defined set of objects." I'm taking this to mean that
    /// this implementation can include the set of all Exceptions (of which
    /// ErrorException is one).
    /// </remarks>
    /// <param name="args">The arguments.</param>
    /// <returns>Boolean</returns>
    public static object ErrorObject(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("error-object? requires one argument");
        }

        return args[0] is Exception;
    }

    public static object ErrorObjectMessage(object?[] args)
    {
        if (args.Length != 1 || args[0] is not Exception ex)
        {
            throw new ArgumentException("error-object-message requires one error object argument");
        }

        return ex.Message;
    }

    public static object ErrorObjectIrritants(object?[] args)
    {
        if (args.Length != 1 || args[0] is not Exception ex)
        {
            throw new ArgumentException("error-object-message requires one error object argument");
        }

        return ex is not ErrorException errorException ? Array.Empty<object?>() : errorException.Irritants;
    }

    public static object FileError(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("file-error? requires one argument");
        }

        return args[0] is Core.FileError or IOException;
    }

    public static object ReadError(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("read-error? requires one argument");
        }

        return args[0] is ReadError;
    }
}