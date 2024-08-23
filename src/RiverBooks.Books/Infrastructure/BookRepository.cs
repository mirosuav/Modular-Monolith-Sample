using Microsoft.EntityFrameworkCore;
using RiverBooks.Books.Application;
using RiverBooks.Books.Domain;

namespace RiverBooks.Books.Infrastructure;

internal class BookRepository(BookDbContext dbContext) : IBookRepository
{
    public Task AddAsync(Book book)
    {
        dbContext.Add(book);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Book book)
    {
        dbContext.Remove(book);
        return Task.CompletedTask;
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        return await dbContext!.Books.FindAsync(id);
    }

    public async Task<Book?> GetByTitleAndAuthorAsync(string title, string author)
    {
        return await dbContext!.Books
            .Where(x=>x.Author == author && x.Title == title)
            .SingleOrDefaultAsync();
    }

    public async Task<List<Book>> ListAsync()
    {
        return await dbContext.Books.ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}
