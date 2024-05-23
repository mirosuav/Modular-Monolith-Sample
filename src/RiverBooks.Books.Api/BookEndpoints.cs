using Azure.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RiverBooks.Books.Contracts;
using System.Net;

namespace RiverBooks.Books.Api;

internal static class BookEndpoints
{
    internal static RouteGroupBuilder MapBookEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", ListBooksAsync);
        group.MapGet("/{bookId}", GetBookAsync);
        group.MapPost("/", CreateBookAsync);
        group.MapPost("/{bookId}/pricehistory", UpdateBookPriceAsync);
        group.MapDelete("/{bookId}", DeleteBookAsync);

        return group;
    }

    internal static async Task<Results<NoContent, NotFound>> DeleteBookAsync(
            Guid bookId,
            IBookService _bookService)
    {
        // Todo: check if exists
        await _bookService.DeleteBookAsync(bookId);
        return TypedResults.NoContent();
    }

    internal static async Task<Results<NoContent, BadRequest>> UpdateBookPriceAsync(
            Guid bookId,
            [FromBody] decimal newPrice,
            IBookService _bookService)
    {
        await _bookService.UpdateBookPriceAsync(bookId, newPrice);
        return TypedResults.NoContent();
    }

    internal static async Task<Results<Created<BookDto>, BadRequest>> CreateBookAsync(
        CreateBookRequest request,
        IBookService bookService)
    {
        var newBookDto = new BookDto(request.Id ?? Guid.NewGuid(), //TODO don't create Guid in code
        request.Title,
        request.Author,
        request.Price);

        await bookService.CreateBookAsync(newBookDto);
        return TypedResults.Created($"{newBookDto.Id}", newBookDto);
    }

    internal static async Task<Results<Ok<BookDto>, NotFound>> GetBookAsync(
        Guid bookId,
        IBookService bookService)
    {
        return await bookService.GetBookByIdAsync(bookId) is BookDto book
           ? TypedResults.Ok(book)
           : TypedResults.NotFound();
    }

    internal static async Task<Results<Ok<ListBooksResponse>, BadRequest>> ListBooksAsync(
        IBookService bookService,
        CancellationToken cancellationToken)
    {
        var books = await bookService.ListBooksAsync();
        return TypedResults.Ok(new ListBooksResponse() { Books = books });
    }
}
