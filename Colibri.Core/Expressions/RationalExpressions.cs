using System.Numerics;
using Rationals;

namespace Colibri.Core.Expressions;

public static class RationalExpressions
{
    public static object? Rationalize(object?[] args)
    {
        if (args.Length is 0 or > 2)
        {
            throw new InvalidOperationException("rationalize requires one or two arguments");
        }

        switch (args[0])
        {
            case null:
                return null;
            case Rational r:
                return r;
        }

        if (args[0].IsInteger())
        {
            return new Rational((long)args[0]!);
        }

        var value = Convert.ToDouble(args[0]);

        double tolerance = 0;

        if (args.Length == 2)
        {
            tolerance = Convert.ToDouble(args[1]);
        }

        return Rational.Approximate(value, tolerance);
    }

    public static object? Numerator(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("numerator requires one argument");
        }

        return args[0] switch
        {
            null => null,
            Rational r => r.Numerator,
            BigInteger or ulong or long or uint or int or ushort or short or byte or sbyte => args[0],
            decimal d => (decimal)Rational.Approximate((double)d).Numerator,
            double d => (double)Rational.Approximate(d).Numerator,
            float f => (float)Rational.Approximate(f).Numerator,
            _ => throw new ArgumentException($"Getting the numerator of a {args[0]!.GetType()} is not supported. For inexact numbers, try rationalizing it first.")
        };
    }

    public static object? Denominator(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("denominator requires one argument");
        }

        return args[0] switch
        {
            null => null,
            Rational r => r.Denominator,
            BigInteger or ulong or long or uint or int or ushort or short or byte or sbyte => 1,
            decimal d => (decimal)Rational.Approximate((double)d).Denominator,
            double d => (double)Rational.Approximate(d).Denominator,
            float f => (float)Rational.Approximate(f).Denominator,
            _ => throw new ArgumentException($"Getting the denominator of a {args[0]!.GetType()} is not supported. For inexact numbers, try rationalizing it first.")
        };
    }

    public static object? Simplify(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("simplify requires one argument");
        }

        return args[0] switch
        {
            null => null,
            Rational r => r.CanonicalForm,
            BigInteger or ulong or long or uint or int or ushort or short or byte or sbyte => args[0],
            decimal d => (decimal)Rational.Approximate((double)d).CanonicalForm,
            double d => (double)Rational.Approximate(d).CanonicalForm,
            float f => (float)Rational.Approximate(f).CanonicalForm,
            _ => throw new ArgumentException($"Simplifying a {args[0]!.GetType()} is not supported. For inexact numbers, try rationalizing it first.")
        };
    }
}