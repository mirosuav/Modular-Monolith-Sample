using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverBooks.Reporting.Domain;

namespace RiverBooks.Reporting.Infrastructure.Data;

public class BookSaleConfiguration : IEntityTypeConfiguration<BookSale>
{
    public void Configure(EntityTypeBuilder<BookSale> builder)
    {
        builder.ToTable(nameof(BookSale));
        builder.HasKey(b => new { b.OrderId, b.BookId });
        builder.HasIndex(b => b.SoldAtUtc);
    }
}