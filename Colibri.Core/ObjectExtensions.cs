﻿using System.Numerics;
using Rationals;

namespace Colibri.Core;

internal static class ObjectExtensions
{
    public static bool IsTruthy(this object? value) => value is not false;

    public static bool IsNumber(this object? value) => value is Complex
        or BigInteger
        or Rational
        or decimal
        or double
        or float
        or long
        or ulong
        or int
        or uint
        or short
        or ushort
        or byte
        or sbyte;

    // HACK: per Scheme docs: "In many implementations the complex? procedure will
    // be the same as number?, but unusual implementations may rep-
    // resent some irrational numbers exactly or may extend the num-
    // ber system to support some kind of non-complex numbers."
    public static bool IsComplex(this object? value) => IsNumber(value);

    public static bool IsRealNumber(this object? value) =>
        value switch
        {
            BigInteger
                or Rational
                or decimal
                or double
                or float
                or long
                or ulong
                or int
                or uint
                or short
                or ushort
                or byte
                or sbyte => true,
            Complex { Imaginary: 0 } => true,
            _ => false
        };

    public static bool IsRationalNumber(this object? value) =>
        value switch
        {
            double.NegativeInfinity
                or double.PositiveInfinity
                or double.NaN
                or float.NegativeInfinity
                or float.PositiveInfinity
                or float.NaN => false,
            BigInteger
                or Rational
                or decimal
                or double
                or float
                or long
                or ulong
                or int
                or uint
                or short
                or ushort
                or byte
                or sbyte => true,
            Complex { Imaginary: 0 } => true,
            _ => false,
        };

    /// <summary>
    /// Returns true if the <paramref name="value"/> is an integer type or
    /// safely convertible to exact integer without losing data. 
    /// </summary>
    /// <param name="value">The value to consider.</param>
    /// <returns>Boolean</returns>
    public static bool IsInteger(this object? value) =>
        value switch
        {
            BigInteger
                or long
                or ulong
                or int
                or uint
                or short
                or ushort
                or byte
                or sbyte => true,
            decimal de when Math.Truncate(de) == de => true,
            double d when d % 1 == 0 => true,
            float f when f % 1 == 0 => true,
            Rational r when r.Numerator % r.Denominator == 0 => true,
            Complex c when c.Real % 1 == 0 && c.Imaginary == 0 => true,
            _ => false
        };

    public static bool IsUnsignedNumber(this object? value) => value is ulong or uint or ushort or byte;

    public static bool IsFinite(this float value) =>
        !float.IsPositiveInfinity(value) && !float.IsNegativeInfinity(value);

    public static bool IsFinite(this double value) =>
        !double.IsPositiveInfinity(value) && !double.IsNegativeInfinity(value);

    public static bool IsInfinite(this float value) => value is float.PositiveInfinity or float.NegativeInfinity;

    public static bool IsInfinite(this double value) => value is double.PositiveInfinity or double.NegativeInfinity;

    public static BigInteger ToBigInteger(this object value)
    {
        return value switch
        {
            BigInteger bi => bi,
            ulong ul => ul,
            long l => l,
            uint ui => ui,
            int i => i,
            ushort us => us,
            short s => s,
            sbyte sb => sb,
            byte b => b,
            _ => throw new InvalidOperationException("Value is not an integer type")
        };
    }
    
    public static bool IsConvertibleToRational(this object? value)
    {
        return value switch
        {
            BigInteger
                or Rational
                or long
                or ulong
                or int
                or uint
                or short
                or ushort
                or byte
                or sbyte => true,
            _ => false
        };
    }

    public static Rational ToRational(this object value)
    {
        return value switch
        {
            Rational r => r,
            BigInteger bi => bi,
            ulong ul => ul,
            long l => l,
            uint ui => ui,
            int i => i,
            ushort us => us,
            short s => s,
            sbyte sb => sb,
            byte b => b,
            _ => throw new InvalidOperationException("Value is not a rational type")
        };
    }

    public static bool IsInputPort(this object? value) =>
        value switch
        {
            Stream or TextReader => true,
            _ => false,
        };
    
    public static bool IsOutputPort(this object? value) =>
        value switch
        {
            Stream or TextWriter => true,
            _ => false,
        };

    public static bool IsPort(this object? value) => value.IsInputPort() || value.IsOutputPort();
}