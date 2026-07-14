using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Historical;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories", "historical");
        builder.HasKey(c => c.CategoryId);
        builder.HasOne(c => c.ParentCategory).WithMany().HasForeignKey(c => c.ParentCategoryId);
    }
}