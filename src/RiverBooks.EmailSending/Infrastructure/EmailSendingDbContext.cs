using Microsoft.EntityFrameworkCore;
using RiverBooks.EmailSending.Domain;
using System.Reflection;

namespace RiverBooks.EmailSending.Data;

public class EmailSendingDbContext(DbContextOptions<EmailSendingDbContext> options) : DbContext(options)
{
    public DbSet<EmailOutboxEntity> EmailOutboxItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("EmailSending");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
