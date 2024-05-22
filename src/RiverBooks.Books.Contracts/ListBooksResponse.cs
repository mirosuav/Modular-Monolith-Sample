namespace RiverBooks.Books.Contracts;

public class ListBooksResponse
{
  public List<BookDto> Books { get; set; } = new List<BookDto>();
}
