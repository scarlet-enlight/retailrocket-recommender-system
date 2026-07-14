using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Shop;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("carts", "shop");
        builder.HasKey(u => u.CartId);
        builder.HasOne(u => u.User).WithMany().HasForeignKey(u => u.UserId);
        builder.HasOne(u => u.Product).WithMany().HasForeignKey(u => u.ProductId);
        builder.Property(u => u.Quantity).HasDefaultValue(1);
        builder.Property(u => u.AddedAt).HasDefaultValueSql("NOW()");
    }
}