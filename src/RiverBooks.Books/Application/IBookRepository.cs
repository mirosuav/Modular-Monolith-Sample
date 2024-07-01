using RiverBooks.Books.Domain;

namespace RiverBooks.Books.Application;

internal interface IBookRepository : IReadOnlyBookRepository
{
    Task AddAsync(Book book);
    Task DeleteAsync(Book book);
    Task SaveChangesAsync();
}
