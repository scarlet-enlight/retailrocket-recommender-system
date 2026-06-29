using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Historical;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions", "historical");
        builder.HasKey(t => t.TransactionId);
        builder.HasOne(t => t.Visitor).WithMany().HasForeignKey(t => t.VisitorId);
        builder.Property(t => t.Timestamp).IsRequired();
    }
}