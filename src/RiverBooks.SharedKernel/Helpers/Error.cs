using System.Net;

namespace RiverBooks.SharedKernel.Helpers;

/// <summary>
/// <see cref="Error"/> struct
/// </summary>
/// <param name="Code">Short error code</param>
/// <param name="Description">Error human readable description</param>
/// <param name="ErrorType">Error type mapped to HttpStatusCode</param>
public readonly record struct Error(string Code, string Description, ErrorType ErrorType)
{
    public static readonly Error None =
        Failure(string.Empty, string.Empty);

    public static readonly Error NullOrEmpty =
        Failure("Error.NullOrEmptyValue", "Null or empty value provided.");

    public static readonly Error ServerError =
        Failure("Error.ServerError", "Internal server error");

    public static readonly Error OperationCancelled =
        Failure("Error.OperationCancelled", "Operation cancelled");

    public static readonly Error NotAuthorized =
        Unauthorized("Unauthorized");

    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error NotFound(string description) =>
        NotFound("Error.NotFound", description);

    public static Error NotFound() =>
        NotFound("Requested resource not found.");

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    public static Error Conflict(string description) =>
        Conflict("Error.Conflict", description);

    public static Error Unauthorized(string description) =>
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
