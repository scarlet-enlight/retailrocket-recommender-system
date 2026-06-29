using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Shop;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products", "shop");
        builder.HasKey(p => p.ProductId);
        builder.HasOne(p => p.Item).WithMany().HasForeignKey(p => p.ProductId);
        builder.Property(p => p.Name).HasMaxLength(200);
        builder.Property(p => p.Price).HasColumnType("decimal(10,2)");
        builder.HasOne(p => p.Category).WithMany().HasForeignKey(p => p.CategoryId);
    }
}