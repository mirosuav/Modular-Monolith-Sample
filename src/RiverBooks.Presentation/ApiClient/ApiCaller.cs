using Microsoft.AspNetCore.Mvc;
using RiverBooks.Books.Contracts;
using RiverBooks.SharedKernel.Authentication;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Presentation.ApiClient;

public interface IApiCaller
{
    Task<ResultOf> RegisterNewUser(string email, string password);
    Task<ResultOf<AuthToken>> LoginUser(string email, string password);
    Task<ResultOf<ListBooksResponse>> ListBooksAsync();
}

public class ApiCaller(ILogger<ApiCaller> logger, HttpClient httpClient) : IApiCaller
{
    public async Task<ResultOf> RegisterNewUser(string email, string password)
    {
        var response = await httpClient.PostAsJsonAsync("/users", new { email, password });

        if (!response.IsSuccessStatusCode)
        {
            logger.LogDebug("RegisterNewUser failed with status code {RegisterUserStatusCode}", response.StatusCode);
            var details = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            return Error.Validation(details?.Title ?? "User registration failed", details?.Detail ?? "UnexpectedError");
        }

        return ResultOf.Success();
    }

    public async Task<ResultOf<AuthToken>> LoginUser(string email, string password)
    {
        var response = await httpClient.PostAsJsonAsync("/users/login", new { email, password });

        if (!response.IsSuccessStatusCode)
        {
            logger.LogDebug("RegisterNewUser failed with status code {RegisterUserStatusCode}", response.StatusCode);
            var details = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            return Error.Validation("Invalid.Credentials", "User login failed");
        }

        var authToken = await response.Content.ReadFromJsonAsync<AuthToken>();

        if (string.IsNullOrWhiteSpace(authToken?.Token))
            return Error.ServerError;

        return authToken;
    }

    public async Task<ResultOf<ListBooksResponse>> ListBooksAsync()
    {
        var response = await httpClient.GetAsync("/books");
        if (!response.IsSuccessStatusCode)
        {
            logger.LogDebug("ListBooksAsync failed with status code {ListBooksStatusCode}", response.StatusCode);
            var details = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            return Error.NotFound(details?.Detail ?? "Books not found");
        }

        var booksResponse = await response.Content.ReadFromJsonAsync<ListBooksResponse>();

        if (booksResponse is null)
            return Error.ServerError;

        return booksResponse;
    }
}