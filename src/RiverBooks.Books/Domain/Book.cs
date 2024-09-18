using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Books.Domain;

internal class Book
{
    internal Book(Guid id, string title, string author, decimal price)
    {
        Id = PassOrThrowWhen.Empty(id);
        Title = PassOrThrowWhen.NullOrWhiteSpace(title);
        Author = PassOrThrowWhen.NullOrWhiteSpace(author);
        Price = PassOrThrowWhen.Negative(price);
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public decimal Price { get; private set; }

    internal void UpdatePrice(decimal newPrice)
    {
        Price = PassOrThrowWhen.Negative(newPrice);
    }
}