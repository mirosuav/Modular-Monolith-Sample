using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using static System.ArgumentNullException;
using static System.ArgumentException;

namespace RiverBooks.SharedKernel.Helpers;

public static class ThrowWhen
{
    public static void Null([NotNull] object? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ThrowIfNull(argument, paramName);
    }

    public static void NullOrEmpty([NotNull] string? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ThrowIfNullOrEmpty(argument, paramName);
    }

    public static void NullOrWhiteSpace([NotNull] string? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ThrowIfNullOrWhiteSpace(argument, paramName);
    }

    public static void Empty<T>(T argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : ICollection
    {
        if (argument is { Count: 0 })
            throw new ArgumentException("Collection must not be empty.", paramName);
    }

    public static void Empty<T>(T[] argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is { Length: 0 })
            throw new ArgumentException("Array must not be empty.", paramName);
    }

    public static void Empty<T>(IEnumerable<T> argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (!argument.Any())
            throw new ArgumentException("Collection must not be empty.", paramName);
    }

    public static void NullOrEmpty<T>([NotNull] T? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : ICollection
    {
        Null(argument, paramName);
        Empty(argument, paramName);
    }

    public static void NullOrEmpty<T>([NotNull] T[] argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        Null(argument, paramName);
        Empty(argument, paramName);
    }

    public static void NullOrEmpty<T>([NotNull] IEnumerable<T>? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        Null(argument, paramName);
        Empty(argument, paramName);
    }

    public static void Empty(Guid argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument.Equals(Guid.Empty))
            throw new ArgumentException("Value cannot be empty Guid", paramName);
    }

    public static void NullOrEmpty([NotNull] Guid? argument,
        [CallerArgumentExpression(nameof(argument))]
        string? paramName = null)
    {
        Null(argument, paramName);
        Empty(argument.Value, paramName);
    }

    public static void NullOrShorterThan<T>([NotNull] T? argument, int minimumLength,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : ICollection
    {
        Null(argument, paramName);
        if (argument.Count < minimumLength)
            throw new ArgumentException($"Collection must have at least {minimumLength} elements.", paramName);
    }

    public static void NullOrShorterThan([NotNull] string? argument, int minimumLength,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        Null(argument, paramName);
        ShorterThan(argument, minimumLength, paramName);
    }

    public static void ShorterThan(string argument, int minimumLength,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument.Length < minimumLength)
            throw new ArgumentException($"Value must be at least {minimumLength} characters long.", paramName);
    }

    public static void Negative<TNumber>(TNumber argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TNumber : INumber<TNumber>
    {
        if (argument < TNumber.Zero)
            throw new ArgumentException("Value cannot be negative", paramName);
    }

    public static void NegativeOrZero<TNumber>(TNumber argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TNumber : INumber<TNumber>
    {
        if (argument <= TNumber.Zero)
            throw new ArgumentException("Value cannot be negative or zero", paramName);
    }
}