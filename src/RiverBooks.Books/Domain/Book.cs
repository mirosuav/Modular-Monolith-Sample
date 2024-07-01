using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Books.Domain;
internal class Book
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public decimal Price { get; private set; }

    internal Book(Guid id, string title, string author, decimal price)
    {
        Id = PassOrThrow.IfEmpty(id);
        Title = PassOrThrow.IfNullOrWhitespace(title);
        Author = PassOrThrow.IfNullOrWhitespace(author);
        Price = PassOrThrow.IfNegative(price);
    }

    internal void UpdatePrice(decimal newPrice)
    {
        Price = PassOrThrow.IfNegative(newPrice);
    }
}
