using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace RiverBooks.Books.Data;

internal class BookDbContext(DbContextOptions<BookDbContext> options) : DbContext(options)
{
    internal DbSet<Book> Books { get; set; }

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
