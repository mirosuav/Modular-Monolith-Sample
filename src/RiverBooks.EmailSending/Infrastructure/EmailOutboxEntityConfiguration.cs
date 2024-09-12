using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverBooks.EmailSending.Domain;

namespace RiverBooks.EmailSending.Infrastructure;

internal class EmailOutboxEntityConfiguration : IEntityTypeConfiguration<EmailOutboxEntity>
{
    public void Configure(EntityTypeBuilder<EmailOutboxEntity> builder)
    {
        builder.HasKey(x => x.Id);

        // Performance index for pending emails by FIFO
        builder.HasIndex(e => e.Id, "IX_EmailOutboxItems_Id_Pending")
            .HasFilter($"[Status] = {(int)EmailProcessingStatus.Pending}");

        // Performance index for processed emails
        builder.HasIndex(e => e.Id, "IX_EmailOutboxItems_Id_Processed")
            .HasFilter($"[Status] > {(int)EmailProcessingStatus.Pending}")
            .IsDescending();
    }
}
