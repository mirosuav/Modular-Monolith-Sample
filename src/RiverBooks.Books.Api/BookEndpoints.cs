using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RiverBooks.Books.Contracts;

namespace RiverBooks.Books.Api;

internal static class BookEndpoints
{
    internal static RouteGroupBuilder MapBookEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", static async (IBookService bookService) =>
        {
            var books = await bookService.ListBooksAsync();
            return new ListBooksResponse() { Books = books };
        })
        .Produces<Ok<ListBooksResponse>>();

        group.MapGet("/{bookId}", static async Task<IResult> (Guid bookId, IBookService bookService) =>
        {
            return await bookService.GetBookByIdAsync(bookId) is BookDto book
               ? TypedResults.Ok(book)
               : TypedResults.NotFound();
        })
        .Produces<Ok<BookDto>>()
        .Produces<NotFound>();

        group.MapPost("/", static async (CreateBookRequest request, IBookService _bookService) =>
        {
            var newBookDto = new BookDto(request.Id ?? Guid.NewGuid(), //TODO don't create Guid in code
            request.Title,
            request.Author,
            request.Price);

            await _bookService.CreateBookAsync(newBookDto);

            return Results.Created($"{newBookDto.Id}", newBookDto);
        });

        group.MapPost("/{bookId}/pricehistory", static async (
            Guid bookId,
            [FromBody] decimal newPrice,
            IBookService _bookService) =>
        {
            await _bookService.UpdateBookPriceAsync(bookId, newPrice);
            return Results.NoContent();
        });

        group.MapDelete("/{bookId}", static async (Guid bookId, IBookService _bookService) =>
        {
            // Todo: check if exists
            await _bookService.DeleteBookAsync(bookId);
            return Results.NoContent();
        });

        return group;
    }
}
