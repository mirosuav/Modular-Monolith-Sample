using FluentValidation.Results;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.SharedKernel.Extensions;

public static class FluentValidationResultExtensions
{
    public static IEnumerable<Error> AsErrors(this ValidationResult valResult) =>
        valResult.Errors.Select(error =>
            Error.Validation(error.ErrorCode, error.ErrorMessage + $" Property={error.PropertyName}"));
}

