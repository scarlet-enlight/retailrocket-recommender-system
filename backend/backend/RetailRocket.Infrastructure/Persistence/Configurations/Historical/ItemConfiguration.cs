using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Historical;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("items", "historical");
        builder.HasKey(i => i.ItemId);
        builder.Property(i => i.ItemId).ValueGeneratedOnAdd();
        builder.HasOne(i => i.Category).WithMany().HasForeignKey(i => i.CategoryId);
        builder.Property(i => i.IsAvailable).IsRequired();
    }
}