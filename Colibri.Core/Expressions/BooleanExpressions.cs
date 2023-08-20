namespace Colibri.Core.Expressions;

public static class BooleanExpressions
{
    public static dynamic LessThan(dynamic?[] args)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException("< needs at least 2 arguments");
        }

        var prev = args[0];

        for (int i = 1; i < args.Length; i++)
        {
            var next = args[i];

            if (prev >= next)
                return false;

            prev = next;
        }

        return true;
    }

    public static dynamic GreaterThan(dynamic?[] args)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException("> needs at least 2 arguments");
        }

        var prev = args[0];

        for (int i = 1; i < args.Length; i++)
        {
            var next = args[i];

            if (prev <= next)
                return false;

            prev = next;
        }

        return true;
    }

    public static dynamic LessThanOrEqual(dynamic?[] args)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException("<= needs at least 2 arguments");
        }

        var prev = args[0];

        for (int i = 1; i < args.Length; i++)
        {
            var next = args[i];

            if (prev > next)
                return false;

            prev = next;
        }

        return true;
    }

    public static dynamic GreaterThanOrEqual(dynamic?[] args)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException(">= needs at least 2 arguments");
        }

        var prev = args[0];

        for (int i = 1; i < args.Length; i++)
        {
            var next = args[i];

            if (prev < next)
                return false;

            prev = next;
        }

        return true;
    }

    public static object Equivalent(object?[] args)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException("eqv? needs at least 2 arguments");
        }

        var first = args[0];

        for (int i = 1; i < args.Length; i++)
        {
            if (!Equals(first, args[i]))
            {
                return false;
            }
        }

        return true;
    }

    public static object Not(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("not needs one argument");
        }

        return !args[0].IsTruthy() ? true : Nil.Value;
    }

    public static object ReferencesEqual(object?[] args)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException("eq? needs at least 2 arguments");
        }

        var first = args[0];

        for (int i = 1; i < args.Length; i++)
        {
            // HACK: Symbols are currently different objects, even though the value is interned. Should they be part of some global symbol cache?
            if (first is Symbol firstSym && args[i] is Symbol secondSym)
            {
                if (!ReferenceEquals(firstSym.Value, secondSym.Value))
                {
                    return false;
                }
            }
            else if (!ReferenceEquals(first, args[i]))
            {
                return false;
            }
        }

        return true;
    }

    public static object NumericallyEqual(object?[] args)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException("= needs at least 2 arguments");
        }

        if (args[0] == null || !args[0].IsNumber())
        {
            throw new ArgumentException("At least one argument is not a number");
        }

        dynamic? first = args[0];

        for (int i = 1; i < args.Length; i++)
        {
            dynamic? arg = args[i];

            if (arg == null || !args[i].IsNumber())
            {
                throw new ArgumentException("At least one argument is not a number");
            }

            if (first != arg)
            {
                return false;
            }
        }

        return true;
    }

    public static object Equal(object?[] args)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException("eq? needs at least 2 arguments");
        }

        var first = args[0];

        for (int i = 1; i < args.Length; i++)
        {
            if (!EqualEqualityComparer.Instance.Equals(first, args[i]))
            {
                return false;
            }
        }

        return true;
    }

    private class EqualEqualityComparer : IEqualityComparer<object?>
    {
        public static readonly IEqualityComparer<object?> Instance = new EqualEqualityComparer();

        bool IEqualityComparer<object?>.Equals(object? x, object? y) 
            => Equals(x, y) ||
               (x is IEnumerable<object?> firstEnumerable &&
                y is IEnumerable<object?> secondEnumerable &&
                firstEnumerable.SequenceEqual(secondEnumerable, Instance));

        public int GetHashCode(object? obj) => obj?.GetHashCode() ?? 0;
    }

    public static object SymbolEquals(object?[] args)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException("symbol=? requires at least two arguments");
        }

        if (args[0] is not Symbol symbol)
        {
            throw new ArgumentException("At least one argument is not a symbol");
        }

        for (int i = 1; i < args.Length; i++)
        {
            if (args[i] is not Symbol symbol2)
            {
                throw new ArgumentException("At least one argument is not a symbol");
            }

            if (!symbol.Equals(symbol2))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// R7RS-small 6.3: Returns #t if all the arguments are booleans and all are #t or all are #f.
    /// </summary>
    /// <param name="args">Any number of arguments of any type.</param>
    /// <returns>Returns a boolean boxed as object.</returns>
    /// <remarks>
    /// At least one other R7RS-small implementation throws if there are not two
    /// arguments or any are not booleans. We interpret the spec as saying that
    /// if there are no arguments, return #f, and if there is one argument that
    /// is a boolean, return #t. Additionally, if any arguments are not booleans,
    /// return #f instead of throwing.
    /// </remarks>
    public static object BooleansAreEqual(object?[] args)
    {
        bool? firstValue = null;
        
        foreach (var arg in args)
        {
            if (arg is not bool value)
            {
                return false;
            }
            
            if (firstValue == null)
            {
                firstValue = value;
            }
            else if (firstValue != value)
            {
                return false;
            }
        }

        return firstValue != null;
    }
}