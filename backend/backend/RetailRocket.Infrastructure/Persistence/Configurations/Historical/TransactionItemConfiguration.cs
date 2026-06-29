using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Historical;

public class TransactionItemConfiguration : IEntityTypeConfiguration<TransactionItem>
{
    public void Configure(EntityTypeBuilder<TransactionItem> builder)
    {
        builder.ToTable("transaction_items", "historical");
        builder.HasKey(ti => new { ti.TransactionId, ti.ItemId });
        builder.HasOne(ti => ti.Transaction).WithMany().HasForeignKey(ti => ti.TransactionId);
        builder.HasOne(ti => ti.Item).WithMany().HasForeignKey(ti => ti.ItemId);
    }
}