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
        IBookService bookService,
        CancellationToken cancellationToken)
    {
        var books = await bookService.ListBooksAsync();
        return TypedResults.Ok(new ListBooksResponse() { Books = books });
    }

    internal static async Task<IResult> CreateBookAsync(
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

    internal static async Task<IResult> UpdateBookPriceAsync(
            Guid bookId,
            [FromBody] decimal newPrice,
            IBookService _bookService)
    {
        // Todo: check if exists
        await _bookService.UpdateBookPriceAsync(bookId, newPrice);
        return TypedResults.NoContent();
    }

    internal static async Task<IResult> DeleteBookAsync(
            Guid bookId,
            IBookService _bookService)
    {
        // Todo: check if exists
        await _bookService.DeleteBookAsync(bookId);
        return TypedResults.NoContent();
    }
}
