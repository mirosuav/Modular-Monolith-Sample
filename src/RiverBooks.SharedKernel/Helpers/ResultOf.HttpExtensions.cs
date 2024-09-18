using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace RiverBooks.SharedKernel.Helpers;

public static class ResultOfHttpExtensions
{
    public static IResult ToHttpOk(this ResultOf result)
    {
        return result.IsSuccess
            ? TypedResults.Ok()
            : result.ToProblemHttpResult();
    }

    public static IResult ToHttpNoContent(this IResultOf result)
    {
        return result.IsSuccess
            ? TypedResults.NoContent()
            : result.ToProblemHttpResult();
    }

    public static IResult ToHttpCreated(this IResultOf result, string? uri = null)
    {
        return result.IsSuccess
            ? TypedResults.Created(uri)
            : result.ToProblemHttpResult();
    }

    public static IResult ToHttpCreated<T>(this ResultOf<T> result, string? uri = null)
    {
        return result.IsSuccess
            ? TypedResults.Created(uri, result.Value)
            : result.ToProblemHttpResult();
    }

    public static IResult ToHttpOk<T>(this ResultOf<T> result)
    {
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemHttpResult();
    }

    public static IResult ToHttpOk<T, TResult>(this ResultOf<T> result, Func<T, TResult> resultFactory)
    {
        return result.IsSuccess
            ? TypedResults.Ok(resultFactory(result.Value))
            : result.ToProblemHttpResult();
    }

    public static ProblemHttpResult ToProblemHttpResult(this IResultOf result)
    {
        if (result.IsSuccess)
            throw new ApplicationException("No Error to details to crete.");

        return TypedResults.Problem(result.ToProblemDetails());
    }

    public static ProblemHttpResult ToProblemHttpResult(this IReadOnlyList<Error> errors, string? instance = null)
    {
        return TypedResults.Problem(errors.ToProblemDetails());
    }

    /// <see href="https://datatracker.ietf.org/doc/html/rfc7807#section-3.1" />
    public static ProblemDetails ToProblemDetails(this IResultOf result, string? instance = null)
    {
        if (result.Errors is null or [])
            throw new ArgumentException("Not a failure result.");

        return result.Errors.ToProblemDetails(instance);
    }

    public static ProblemDetails ToProblemDetails(this IReadOnlyList<Error> errors, string? instance = null)
    {
        if (errors is [])
            throw new ArgumentException("No errors.");

        var pd = new ProblemDetails
        {
            Title = errors[0].Code,
            Detail = errors[0].Description,
            Type = errors[0].ErrorType.GetRFCUri(),
            Status = (int)errors[0].ErrorType,
            Extensions = new Dictionary<string, object?>
            {
                { "errors", errors }
            }
        };

        if (instance is not null)
            pd.Instance = instance;

        return pd;
    }

    public static string GetRFCUri(this ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            ErrorType.Unauthorized => "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
            _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };
    }
}