using Microsoft.EntityFrameworkCore;
using RiverBooks.EmailSending.Domain;
using System.Reflection;

namespace RiverBooks.EmailSending.Data;

internal class EmailSendingDbContext(DbContextOptions<EmailSendingDbContext> options) : DbContext(options)
{
    internal DbSet<EmailOutboxEntity> EmailOutboxItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("EmailSending");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
