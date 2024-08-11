using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace RiverBooks.SharedKernel.Helpers;

public static class PassOrThrowWhen
{
    public static T Null<T>([NotNull] T? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ThrowWhen.Null(argument, paramName);
        return argument;
    }

    public static string NullOrEmpty([NotNull] string? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ThrowWhen.NullOrEmpty(argument, paramName);
        return argument;
    }

    public static string NullOrWhiteSpace([NotNull] string? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ThrowWhen.NullOrWhiteSpace(argument, paramName);
        return argument;
    }

    public static T Empty<T>(T argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : ICollection
    {
        ThrowWhen.Empty(argument, paramName);
        return argument;
    }

    public static T NullOrEmpty<T>([NotNull] T? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : ICollection
    {
        ThrowWhen.NullOrEmpty(argument, paramName);
        return argument;
    }

    public static T[] NullOrEmpty<T>([NotNull] T[] argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ThrowWhen.NullOrEmpty(argument, paramName);
        return argument;
    }

    public static IEnumerable<T> NullOrEmpty<T>([NotNull] IEnumerable<T>? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ThrowWhen.NullOrEmpty(argument, paramName);
        return argument;
    }

    public static Guid Empty(Guid argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ThrowWhen.Empty(argument, paramName);
        return argument;
    }

    public static Guid NullOrEmpty([NotNull] Guid? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ThrowWhen.NullOrEmpty(argument, paramName);
        return argument.Value;
    }
    
    public static TNumber Negative<TNumber>(TNumber argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TNumber : INumber<TNumber>
    {
        ThrowWhen.Negative(argument, paramName);
        return argument;
    }

    public static TNumber NegativeOrZero<TNumber>(TNumber argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TNumber : INumber<TNumber>
    {
        ThrowWhen.NegativeOrZero(argument, paramName);
        return argument;
    }
}
