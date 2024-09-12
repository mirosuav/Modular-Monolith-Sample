using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RiverBooks.Books.Domain;
using RiverBooks.SharedKernel.Events;

namespace RiverBooks.Books.Infrastructure;

internal class BookDbContext(DbContextOptions<BookDbContext> options)
    : DbContext(options)
{
    internal DbSet<Book> Books { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(ModuleDescriptor.Name);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(
      ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>()
          .HavePrecision(18, 6);
    }
}
