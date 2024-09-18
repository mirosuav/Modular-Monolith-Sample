using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RiverBooks.EmailSending.Domain;

namespace RiverBooks.EmailSending.Infrastructure;

public class EmailSendingDbContext(DbContextOptions<EmailSendingDbContext> options)
    : DbContext(options)
{
    public DbSet<EmailOutboxEntity> EmailOutboxItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(ModuleDescriptor.Name);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}