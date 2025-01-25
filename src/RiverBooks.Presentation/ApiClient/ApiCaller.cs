using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RiverBooks.Books.Contracts;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.Presentation.Auth;
using RiverBooks.SharedKernel.Authentication;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Contracts;

namespace RiverBooks.Presentation.ApiClient;

public interface IApiCaller
{
    Task<ResultOf> RegisterUser(string email, string password);
    Task<ResultOf<AuthToken>> LoginUser(string email, string password);
    Task<ResultOf<ListBooksResponse>> GetAllBooks();
    Task<ResultOf<BookDto>> CreateBook(CreateBookRequest bookRequest);
    Task<ResultOf<BookDto>> GetBook(Guid bookId);
    Task<ResultOf> UpdateBookPrice(Guid bookId, decimal newPrice);
    Task<ResultOf> DeleteBook(Guid bookId);
    Task<ResultOf> AddBookToCart(Guid bookId, int quantity);
    Task<ResultOf<List<CartItemDto>>> ListCartItems();
    Task<ResultOf<List<OrderSummary>>> ListOrdersForUser();
    Task<ResultOf> CheckoutCart(Guid shippingAddressId, Guid billingAddressId);

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

    public async Task<ResultOf> UpdateBookPrice(Guid bookId, decimal newPrice)
    {
        var response = await httpClient.PostAsJsonAsync($"/books/{bookId}/pricehistory", newPrice);

        if (!response.IsSuccessStatusCode)
        {
            return await CreateError(response);
        }

        return ResultOf.Success();
    }

    public async Task<ResultOf> AddBookToCart(Guid bookId, int quantity)
    {
        var response = await httpClient.PostAsJsonAsync($"/cart", new { bookId, quantity });
        if (!response.IsSuccessStatusCode)
        {
            return await CreateError(response);
        }
        return ResultOf.Success();
    }

    public async Task<ResultOf<List<CartItemDto>>> ListCartItems()
    {
        var response = await httpClient.GetAsync($"/cart");
        if (!response.IsSuccessStatusCode)
        {
            return await CreateError(response);
        }
        var cartResponse = await response.Content.ReadFromJsonAsync<CartResponse>();
        if (cartResponse is null)
            return Error.ServerError;
        return cartResponse.CartItems;
    }

    public async Task<ResultOf<List<OrderSummary>>> ListOrdersForUser()
    {
        var response = await httpClient.GetAsync($"/orders");
        if (!response.IsSuccessStatusCode)
        {
            return await CreateError(response);
        }
        var ordersResponse = await response.Content.ReadFromJsonAsync<ListOrdersForUserResponse>();
        if (ordersResponse is null)
            return Error.ServerError;
        return ordersResponse.Orders;
    }

    public async Task<ResultOf> CheckoutCart(Guid shippingAddressId, Guid billingAddressId)
    {
        var response = await httpClient.PostAsJsonAsync($"/cart/checkout", new CheckoutRequest(shippingAddressId, billingAddressId));
        if (!response.IsSuccessStatusCode)
        {
            return await CreateError(response);
        }
        return ResultOf.Success();
    }


    private async Task<Error> CreateError(HttpResponseMessage response, [CallerMemberName] string methodName = "")
    {
        logger.LogDebug("{ApiMethod} failed with status code {ApiResponseStatusCode}", methodName, response.StatusCode);
        try
        {
            var details = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            if (details is not null)
                return Errors.Create(methodName, response.StatusCode, details);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error occured when deserializing ProblemDetails from API response of {ApiMethod}", methodName);
        }

        return Error.Failure(response.StatusCode.ToString(), $"Calling {methodName} resulted with {response.StatusCode}");
    }
}