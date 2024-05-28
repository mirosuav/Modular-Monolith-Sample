using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace RiverBooks.SharedKernel.Helpers;

public static class ThrowIf
{
    public static T Null<T>([NotNull] T? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
        return argument;
    }

    public static string NullOrEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(argument, paramName);
        return argument;
    }

    public static string NullOrWhitespace([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(argument, paramName);
        return argument;
    }

    public static Guid Empty(Guid argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument.Equals(Guid.Empty))
            throw new ArgumentException("Value cannot be empty Guid", paramName);
        return argument;
    }
    public static string ShorterThan(string? argument, int minimumLength, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null || argument.Length < minimumLength)
            throw new ArgumentException($"Value must have at least {minimumLength} characters long.", paramName);
        return argument;
    }

    public static TNumber Negative<TNumber>(TNumber argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TNumber : INumber<TNumber>
    {
        if (argument < TNumber.Zero)
            throw new ArgumentException("Value cannot be negative", paramName);

        return argument;
    }

    public static TNumber NegativeOrZero<TNumber>(TNumber argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TNumber : INumber<TNumber>
    {
        if (argument <= TNumber.Zero)
            throw new ArgumentException("Value cannot be negative or zero", paramName);

        return argument;
    }
}
