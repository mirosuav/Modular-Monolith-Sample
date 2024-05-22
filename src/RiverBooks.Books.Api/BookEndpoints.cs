using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RiverBooks.Books.Contracts;

namespace RiverBooks.Books.Api;

public static class BookEndpoints
{
  public static RouteGroupBuilder MapBooksEndpoints(this RouteGroupBuilder group)
  {
    group.MapGet("/", static async (IBookService _bookService) =>
    {
      var books = await _bookService.ListBooksAsync();
      return new ListBooksResponse() { Books = books };
    });

    group.MapGet("/{bookId}", static async (Guid bookId, IBookService _bookService) =>
    {
      var book = await _bookService.GetBookByIdAsync(bookId);
      return book is null ? TypedResults.NotFound() : TypedResults.Ok(book);
    });

    group.MapPost("/", static async (CreateBookRequest request, IBookService _bookService) =>
    {
      var newBookDto = new BookDto(request.Id ?? Guid.NewGuid(), //TODO don't create Guid in code
      request.Title,
      request.Author,
      request.Price);

      await _bookService.CreateBookAsync(newBookDto);

      return TypedResults.Created($"{newBookDto.Id}", newBookDto);
    });

    group.MapPost("/{bookId}/pricehistory", static async (
      Guid bookId, 
      [FromBody] decimal newPrice, 
      IBookService _bookService) =>
    {
      await _bookService.UpdateBookPriceAsync(bookId, newPrice);
      return TypedResults.NoContent();
    });

    group.MapDelete("/{bookId}", static async (Guid bookId, IBookService _bookService) =>
    {
      // Todo: check if exists
      await _bookService.DeleteBookAsync(bookId);
      return TypedResults.NoContent();
    });

    return group;
  }
}
