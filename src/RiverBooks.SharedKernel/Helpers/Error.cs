using System.Net;

namespace RiverBooks.SharedKernel.Helpers;

/// <summary>
/// <see cref="Error"/> struct
/// </summary>
/// <param name="Code">Short error code</param>
/// <param name="Description">Error human readable description</param>
/// <param name="ErrorType">Error type mappedto HttpStatusCode</param>
public readonly record struct Error(string Code, string Description, ErrorType ErrorType)
{
    public static readonly Error None =
        CreateFailure(string.Empty, string.Empty);

    public static readonly Error NullOrEmpty =
        CreateFailure("Error.NullOrEmptyValue", "Null or empty value provided.");

    public static readonly Error ServerError =
        CreateFailure("Error.ServerError", "Internal server error");

    public static readonly Error OperationCancelled =
        CreateFailure("Error.OperationCancelled", "Operation cancelled");

    public static readonly Error Unauthorized =
        CreateUnauthorized("Unauthorized");

    public static Error CreateFailure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error CreateValidation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    public static Error CreateNotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error CreateNotFound(string description) =>
        new("Error.NotFound", description, ErrorType.NotFound);

    public static Error CreateConflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    public static Error CreateConflict(string description) =>
        new("Error.Conflict", description, ErrorType.Conflict);

    public static Error CreateUnauthorized(string description) =>
        new("Error.Unauthorized", description, ErrorType.Unauthorized);
}

public enum ErrorType
{
    Failure = HttpStatusCode.InternalServerError,
    Validation = HttpStatusCode.BadRequest,
    NotFound = HttpStatusCode.NotFound,
    Conflict = HttpStatusCode.Conflict,
    Unauthorized = HttpStatusCode.Unauthorized
}
