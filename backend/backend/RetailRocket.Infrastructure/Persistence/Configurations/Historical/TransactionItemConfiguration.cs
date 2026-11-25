using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Historical;

public class TransactionItemConfiguration : IEntityTypeConfiguration<TransactionItem>
{
    public void Configure(EntityTypeBuilder<TransactionItem> builder)
    {
        builder.ToTable("transaction_items", "historical");
        builder.HasKey(ti => new { ti.transaction_id, ti.item_id });
        builder.HasOne(ti => ti.transaction).WithMany().HasForeignKey(ti => ti.transaction_id);
        builder.HasOne(ti => ti.item).WithMany().HasForeignKey(ti => ti.item_id);
    }
}