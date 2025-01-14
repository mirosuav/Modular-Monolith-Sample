using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using RiverBooks.Books.Contracts;
using RiverBooks.SharedKernel.Authentication;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Presentation.ApiClient;

public interface IApiCaller
{
    Task<ResultOf> RegisterUser(string email, string password);
    Task<ResultOf<AuthToken>> LoginUser(string email, string password);
    Task<ResultOf<ListBooksResponse>> GetAllBooks();
    Task<ResultOf<BookDto>> CreateBook(CreateBookRequest bookRequest);
    Task<ResultOf<BookDto>> GetBook(Guid bookId);
    Task<ResultOf> DeleteBook(Guid bookId);
}

public class ApiCaller(ILogger<ApiCaller> logger, HttpClient httpClient) : IApiCaller
{
    public async Task<ResultOf> RegisterUser(string email, string password)
    {
        var response = await httpClient.PostAsJsonAsync("/users", new { email, password });

        if (!response.IsSuccessStatusCode)
        {
            return await CreateError(response);
        }

        return ResultOf.Success();
    }

    public async Task<ResultOf<AuthToken>> LoginUser(string email, string password)
    {
        var response = await httpClient.PostAsJsonAsync("/users/login", new { email, password });

        if (!response.IsSuccessStatusCode)
        {
            return await CreateError(response);
        }

        var authToken = await response.Content.ReadFromJsonAsync<AuthToken>();

        if (string.IsNullOrWhiteSpace(authToken?.Token))
            return Error.ServerError;

        return authToken;
    }

    public async Task<ResultOf<ListBooksResponse>> GetAllBooks()
    {
        var response = await httpClient.GetAsync("/books");
        if (!response.IsSuccessStatusCode)
        {
            return await CreateError(response);
        }

        var booksResponse = await response.Content.ReadFromJsonAsync<ListBooksResponse>();

        if (booksResponse is null)
            return Error.ServerError;

        return booksResponse;
    }

    public async Task<ResultOf<BookDto>> GetBook(Guid bookId)
    {
        var response = await httpClient.GetAsync($"/books/{bookId}");
        if (!response.IsSuccessStatusCode)
        {
            return await CreateError(response);
        }

        var book = await response.Content.ReadFromJsonAsync<BookDto>();

        if (book is null)
            return Error.ServerError;

        return book;
    }

    public async Task<ResultOf<BookDto>> CreateBook(CreateBookRequest bookRequest)
    {
        var response = await httpClient.PostAsJsonAsync($"/books", bookRequest);
        if (!response.IsSuccessStatusCode)
        {
            return await CreateError(response);
        }

        var book = await response.Content.ReadFromJsonAsync<BookDto>();

        if (book is null)
            return Error.ServerError;

        return book;
    }

    public async Task<ResultOf> DeleteBook(Guid bookId)
    {
        var response = await httpClient.DeleteAsync($"/books/{bookId}");
        if (!response.IsSuccessStatusCode)
        {
            return await CreateError(response);
        }

        return ResultOf.Success();
    }

    private async Task<Error> CreateError(HttpResponseMessage response, [CallerMemberName] string methodName = "")
    {
        logger.LogDebug("{ApiMethod} failed with status code {ApiResponseStatusCode}", methodName, response.StatusCode);
        var details = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        if (details is null)
            return Error.Failure(response.StatusCode.ToString(), methodName);

        return Errors.Create(methodName, response.StatusCode, details);
    }
}