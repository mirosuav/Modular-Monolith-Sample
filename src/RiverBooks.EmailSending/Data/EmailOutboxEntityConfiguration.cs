using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverBooks.EmailSending.Domain;

namespace RiverBooks.EmailSending.Data;

internal class EmailOutboxEntityConfiguration : IEntityTypeConfiguration<EmailOutboxEntity>
{
    public void Configure(EntityTypeBuilder<EmailOutboxEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.DateTimeUtcProcessed);        
    }
}
