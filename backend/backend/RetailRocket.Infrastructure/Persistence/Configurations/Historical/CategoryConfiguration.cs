using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Historical;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories", "historical");
        builder.HasKey(c => c.category_id);
        builder.Property(c => c.category_id).ValueGeneratedNever();
        builder.HasOne(c => c.parent_category).WithMany().HasForeignKey(c => c.parent_id);
    }
}