using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Shop;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("cart", "shop");
        builder.HasKey(u => u.cart_id);
        builder.Property(u => u.cart_id).HasColumnType("uuid").ValueGeneratedOnAdd();
        builder.HasOne(u => u.user).WithMany().HasForeignKey(u => u.user_id);
        builder.HasOne(u => u.product).WithMany().HasForeignKey(u => u.product_id);
        builder.Property(u => u.quantity).HasDefaultValue(1);
        builder.Property(u => u.added_at).HasDefaultValueSql("NOW()");
    }
}