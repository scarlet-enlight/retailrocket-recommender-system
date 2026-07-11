using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Shop;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders", "shop");
        builder.HasKey(o => o.OrderId);
        builder.Property(o => o.OrderId).ValueGeneratedOnAdd();
        builder.HasOne(o => o.User).WithMany().HasForeignKey(o => o.UserId);
        builder.Property(u => u.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(u => u.Total).HasColumnType("decimal(10,2)");
    }
}