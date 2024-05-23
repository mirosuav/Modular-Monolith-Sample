using System.Net;

namespace RiverBooks.SharedKernel.Helpers;

/// <summary>
/// Error
/// </summary>
/// <param name="Code">Short error code</param>
/// <param name="Description">Error human readable description</param>
/// <param name="ErrorType">Error type</param>
public readonly record struct Error(string Code, string Description, ErrorType ErrorType)
{
    public static readonly Error None = 
        Failure(string.Empty, string.Empty);

    public static readonly Error NullValue = 
        Failure($"{nameof(Error)}.{nameof(NullValue)}", "Null or missing value provided.");

    public static readonly Error ServerError = 
        Failure($"{nameof(Error)}.{nameof(ServerError)}", "Internal server error");

    public static readonly Error OperationCancelled = 
        Failure($"{nameof(Error)}.{nameof(OperationCancelled)}", "Operation cancelled");

    public static readonly Error UserNotAauthorized = 
        Failure($"{nameof(Error)}.{nameof(UserNotAauthorized)}", "Access denied");

    public static Error Failure(string code, string description) => 
        new(code, description, ErrorType.Failure);

    public static Error Validation(string code, string description) => 
        new(code, description, ErrorType.Validation);

    public static Error NotFound(string code, string description) => 
        new(code, description, ErrorType.NotFound);

    public static Error NotFound(string description) => 
        new($"{nameof(Error)}.{nameof(NotFound)}", description, ErrorType.NotFound);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    public static Error Conflict(string description) => 
        new($"{nameof(Error)}.{nameof(Conflict)}", description, ErrorType.Conflict);

    public static Error Unauthorized(string code, string description) => 
        new(code, description, ErrorType.Unauthorized);

    public static Error Unauthorized(string description) =>
        new($"{nameof(Error)}.{nameof(Unauthorized)}", description, ErrorType.Unauthorized);
}

public enum ErrorType
{
    Failure = HttpStatusCode.InternalServerError,
    Validation = HttpStatusCode.BadRequest,
    NotFound = HttpStatusCode.NotFound,
    Conflict = HttpStatusCode.Conflict,
    Unauthorized = HttpStatusCode.Unauthorized
}
