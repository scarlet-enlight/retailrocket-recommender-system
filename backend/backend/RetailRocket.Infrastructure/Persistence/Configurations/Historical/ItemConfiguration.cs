using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Historical;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("items", "historical");
        builder.HasKey(i => i.item_id);
        builder.Property(i => i.item_id).ValueGeneratedOnAdd();
        builder.HasOne(i => i.category).WithMany().HasForeignKey(i => i.category_id);
        builder.Property(i => i.available).IsRequired();
    }
}