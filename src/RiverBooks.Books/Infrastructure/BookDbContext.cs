using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RiverBooks.Books.Domain;

namespace RiverBooks.Books.Infrastructure;

internal class BookDbContext(DbContextOptions<BookDbContext> options) : DbContext(options)
{
    internal DbSet<Book> Books { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Books");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void ConfigureConventions(
      ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>()
          .HavePrecision(18, 6);
    }
}
