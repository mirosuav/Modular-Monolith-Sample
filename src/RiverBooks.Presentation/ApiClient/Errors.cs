using System.Net;
using Microsoft.AspNetCore.Mvc;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Presentation.ApiClient;

public static class Errors
{
    public static Error Create(string code, HttpStatusCode statusCode, ProblemDetails problemDetails)
    {
        var errorType = MapsFromHttpStatusCode(statusCode);
        return new Error(code, problemDetails.Detail ?? code, errorType, problemDetails);
    }

    private static ErrorType MapsFromHttpStatusCode(HttpStatusCode httpStatusCode)
    {
        return httpStatusCode switch
        {
            >= HttpStatusCode.InternalServerError and < HttpStatusCode.NetworkAuthenticationRequired 
                => ErrorType.Failure,
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden 
                => ErrorType.Unauthorized,
            HttpStatusCode.Conflict 
                => ErrorType.Conflict,
            HttpStatusCode.NotFound or HttpStatusCode.Gone 
                => ErrorType.NotFound,
            >= HttpStatusCode.BadRequest and < HttpStatusCode.InternalServerError 
                => ErrorType.Validation,
            _ => ErrorType.Failure,
        };
    }
}