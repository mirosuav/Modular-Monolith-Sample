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
        Id = ThrowIf.Empty(id);
        Title = ThrowIf.NullOrWhitespace(title);
        Author = ThrowIf.NullOrWhitespace(author);
        Price = ThrowIf.Negative(price);
    }

    internal void UpdatePrice(decimal newPrice)
    {
        Price = ThrowIf.Negative(newPrice);
    }
}
