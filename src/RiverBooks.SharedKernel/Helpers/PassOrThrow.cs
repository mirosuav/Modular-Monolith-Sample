using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace RiverBooks.SharedKernel.Helpers;

public static class PassOrThrow
{
    public static T IfNull<T>([NotNull] T? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
        return argument;
    }

    public static string IfNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(argument, paramName);
        return argument;
    }

    public static string IfNullOrWhitespace([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(argument, paramName);
        return argument;
    }

    public static Guid IfEmpty(Guid argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument.Equals(Guid.Empty))
            throw new ArgumentException("Value cannot be empty Guid", paramName);
        return argument;
    }
    public static string IfShorterThan(string? argument, int minimumLength, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null || argument.Length < minimumLength)
            throw new ArgumentException($"Value must have at least {minimumLength} characters long.", paramName);
        return argument;
    }

    public static TNumber IfNegative<TNumber>(TNumber argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TNumber : INumber<TNumber>
    {
        if (argument < TNumber.Zero)
            throw new ArgumentException("Value cannot be negative", paramName);

        return argument;
    }

    public static TNumber IfNegativeOrZero<TNumber>(TNumber argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TNumber : INumber<TNumber>
    {
        if (argument <= TNumber.Zero)
            throw new ArgumentException("Value cannot be negative or zero", paramName);

        return argument;
    }
}
