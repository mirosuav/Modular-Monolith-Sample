using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverBooks.Books.Domain;
using RiverBooks.SharedKernel;

namespace RiverBooks.Books.Infrastructure;

internal class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.Property(p => p.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Author)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasData(GetSampleBookData());
    }

    private static IEnumerable<Book> GetSampleBookData()
    {
        var tolkien = "J.R.R. Tolkien";
        yield return new Book(SequentialGuid.NewGuid(), "The Fellowship of the Ring", tolkien, 10.99m);
        yield return new Book(SequentialGuid.NewGuid(), "The Two Towers", tolkien, 11.99m);
        yield return new Book(SequentialGuid.NewGuid(), "The Return of the King", tolkien, 12.99m);
    }
}