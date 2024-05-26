using FluentValidation.Results;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.SharedKernel.Extensions;

public static class FluentValidationResultExtensions
{
    public static List<Error> AsErrors(this ValidationResult valResult) =>
        valResult.Errors.Select(error =>
        Error.CreateValidation(error.ErrorCode, error.ErrorMessage + $" Property={error.PropertyName}"))
        .ToList();
}

// TODO map error severity
//public static ValidationSeverity FromSeverity(Severity severity)
//{
//    return severity switch
//    {
//        Severity.Error => ValidationSeverity.Error,
//        Severity.Warning => ValidationSeverity.Warning,
//        Severity.Info => ValidationSeverity.Info,
//        _ => throw new ArgumentOutOfRangeException("severity", "Unexpected Severity"),
//    };
//}

