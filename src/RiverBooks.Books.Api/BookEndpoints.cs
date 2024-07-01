using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RiverBooks.Books.Application;
using RiverBooks.Books.Contracts;
using RiverBooks.SharedKernel;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Books.Api;

internal static class BookEndpoints
{
    internal static RouteGroupBuilder MapBookEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", ListBooksAsync)
            .Produces<Ok<ListBooksResponse>>();

        group.MapGet("/{bookId}", GetBookAsync)
            .Produces<Ok<BookDto>>()
            .Produces<NotFound>();

        group.MapPost("/", CreateBookAsync)
            .Produces<Created<BookDto>>()
            .Produces<BadRequest>();

        group.MapPost("/{bookId}/pricehistory", UpdateBookPriceAsync)
            .Produces<NoContent>()
            .Produces<BadRequest>()
            .Produces<NotFound>();

        group.MapDelete("/{bookId}", DeleteBookAsync)
            .Produces<NoContent>()
            .Produces<BadRequest>();

        return group;
    }

    internal static async Task<IResult> ListBooksAsync(
        [FromServices] IBookService bookService,
        CancellationToken cancellationToken)
    {
        var books = await bookService.ListBooksAsync();
        return TypedResults.Ok(new ListBooksResponse() { Books = books });
    }

    internal static async Task<IResult> CreateBookAsync(
        CreateBookRequest request,
        [FromServices] IBookService bookService,
        CancellationToken cancellationToken)
    {
        var newBookDto = new BookDto(SequentialGuid.NewGuid(), request.Title, request.Author, request.Price);

        await bookService.CreateBookAsync(newBookDto);
        return TypedResults.Created($"{newBookDto.Id}", newBookDto);
    }

    internal static async Task<IResult> GetBookAsync(
        Guid bookId,
        [FromServices] IBookService bookService,
        CancellationToken cancellationToken)
    {
        return (await bookService.GetBookByIdAsync(bookId))
            .ToHttpOk();
    }

    internal static async Task<IResult> UpdateBookPriceAsync(
            Guid bookId,
            [FromBody] decimal newPrice,
            [FromServices] IBookService _bookService,
            CancellationToken cancellationToken)
    {
        return (await _bookService.UpdateBookPriceAsync(bookId, newPrice))
            .ToHttpNoContent();
    }

    internal static async Task<IResult> DeleteBookAsync(
            Guid bookId,
            [FromServices] IBookService _bookService,
            CancellationToken cancellationToken)
    {
        return (await _bookService.DeleteBookAsync(bookId))
            .ToHttpNoContent();
    }
}
