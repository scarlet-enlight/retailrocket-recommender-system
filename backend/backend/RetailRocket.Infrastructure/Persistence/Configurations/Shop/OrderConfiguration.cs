using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Shop;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders", "shop");
        builder.HasKey(o => o.order_id);
        builder.Property(o => o.order_id).HasColumnType("uuid").ValueGeneratedOnAdd();
        builder.HasOne(o => o.user).WithMany().HasForeignKey(o => o.user_id);
        builder.Property(u => u.created_at).HasDefaultValueSql("NOW()");
        builder.Property(u => u.total).HasColumnType("decimal(10,2)");
    }
}