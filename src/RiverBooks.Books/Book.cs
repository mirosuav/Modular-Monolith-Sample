using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Books;
internal class Book
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public decimal Price { get; private set; }

    internal Book(Guid id, string title, string author, decimal price)
    {
        Id = Throwable.IfEmpty(id);
        Title = Throwable.IfNullOrWhitespace(title);
        Author = Throwable.IfNullOrWhitespace(author);
        Price = Throwable.IfNegative(price);
    }

    internal void UpdatePrice(decimal newPrice)
    {
        Price = Throwable.IfNegative(newPrice);
    }
}
