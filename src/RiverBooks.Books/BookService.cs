
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Books;

internal class BookService(IBookRepository bookRepository) : IBookService
{
    private readonly IBookRepository _bookRepository = bookRepository;

    public async Task<Resultable> CreateBookAsync(BookDto newBook)
    {
        var book = new Book(newBook.Id, newBook.Title, newBook.Author, newBook.Price);

        await _bookRepository.AddAsync(book);
        await _bookRepository.SaveChangesAsync();

        return Resultable.Success();
    }

    public async Task<Resultable> DeleteBookAsync(Guid id)
    {
        var bookToDelete = await _bookRepository.GetByIdAsync(id);

        if (bookToDelete is null)
            return Error.NotFound();

        await _bookRepository.DeleteAsync(bookToDelete);
        await _bookRepository.SaveChangesAsync();

        return Resultable.Success();
    }

    public async Task<Resultable<BookDto>> GetBookByIdAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);

        if (book is null)
            return Error.NotFound();

        return new BookDto(book!.Id, book.Title, book.Author, book.Price);
    }

    public async Task<List<BookDto>> ListBooksAsync()
    {
        var books = (await _bookRepository.ListAsync())
          .Select(book => new BookDto(book.Id, book.Title, book.Author, book.Price))
          .ToList();

        return books;
    }

    public async Task<Resultable> UpdateBookPriceAsync(Guid bookId, decimal newPrice)
    {
        if (newPrice <= decimal.Zero)
            return Error.Validation("Invalid price", "Price must be positive number");

        var book = await _bookRepository.GetByIdAsync(bookId);

        if (book is null)
            return Error.NotFound();

        book.UpdatePrice(newPrice);
        await _bookRepository.SaveChangesAsync();

        return Resultable.Success();
    }
}
