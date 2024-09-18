namespace RiverBooks.Books.Contracts;

public record BookDto(Guid Id, string Title, string Author, decimal Price);