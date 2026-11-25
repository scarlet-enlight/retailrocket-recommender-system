using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Shop;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "shop");
        builder.HasKey(u => u.user_id);
        builder.Property(u => u.user_id).HasColumnType("uuid").ValueGeneratedOnAdd();
        builder.Property(u => u.username).HasMaxLength(100).IsRequired();
        builder.Property(u => u.email).HasMaxLength(200).IsRequired();
        builder.Property(u => u.password_hash).HasMaxLength(255).IsRequired();
        builder.Property(u => u.role_level).HasDefaultValue(0).IsRequired();
        builder.Property(u => u.created_at).HasDefaultValueSql("NOW()");
    }
}