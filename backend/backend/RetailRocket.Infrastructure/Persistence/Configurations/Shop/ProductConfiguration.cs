using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Shop;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products", "shop");
        builder.HasKey(p => p.product_id);
        builder.Property(p => p.product_id).HasColumnType("uuid").ValueGeneratedOnAdd();
        builder.HasOne(p => p.item).WithMany().HasForeignKey(p => p.item_id);
        builder.Property(p => p.name).HasMaxLength(200);
        builder.Property(p => p.price).HasColumnType("decimal(10,2)");
        builder.HasOne(p => p.category).WithMany().HasForeignKey(p => p.category_id);
    }
}