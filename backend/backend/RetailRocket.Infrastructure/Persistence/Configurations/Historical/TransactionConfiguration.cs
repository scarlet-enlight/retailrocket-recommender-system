using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Historical;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions", "historical");
        builder.HasKey(t => t.transaction_id);
        builder.Property(t => t.transaction_id).ValueGeneratedOnAdd();
        builder.HasOne(t => t.visitor).WithMany().HasForeignKey(t => t.visitor_id);
        builder.Property(t => t.ts).IsRequired();
    }
}