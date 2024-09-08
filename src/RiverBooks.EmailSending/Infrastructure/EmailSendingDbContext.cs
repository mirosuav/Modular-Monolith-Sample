using Microsoft.EntityFrameworkCore;
using RiverBooks.EmailSending.Domain;
using System.Reflection;
using RiverBooks.SharedKernel.Events;

namespace RiverBooks.EmailSending.Infrastructure;

public class EmailSendingDbContext(DbContextOptions<EmailSendingDbContext> options)
    : TransactionalOutboxDbContext(options)
{
    public DbSet<EmailOutboxEntity> EmailOutboxItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(ModuleDescriptor.Name);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
