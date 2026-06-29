using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Shop;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items", "shop");
        builder.HasKey(oi => new {oi.OrderId, oi.ProductId});
        builder.HasOne(oi => oi.Order).WithMany().HasForeignKey(o => o.OrderId);
        builder.HasOne(oi => oi.Product).WithMany().HasForeignKey(o => o.ProductId);
        builder.Property(oi => oi.Quantity);
        builder.Property(oi => oi.Price).HasColumnType("decimal(10,2)");
    }
}