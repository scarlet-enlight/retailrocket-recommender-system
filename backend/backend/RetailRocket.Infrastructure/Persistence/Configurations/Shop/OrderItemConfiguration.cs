using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Shop;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items", "shop");
        builder.HasKey(oi => new {oi.order_id, oi.product_id});
        builder.HasOne(oi => oi.order).WithMany().HasForeignKey(o => o.order_id);
        builder.HasOne(oi => oi.product).WithMany().HasForeignKey(o => o.product_id);
        builder.Property(oi => oi.quantity);
        builder.Property(oi => oi.price).HasColumnType("decimal(10,2)");
    }
}