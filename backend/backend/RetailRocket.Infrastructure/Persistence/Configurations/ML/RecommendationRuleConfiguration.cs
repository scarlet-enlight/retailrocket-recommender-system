using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.ML;

namespace RetailRocket.Infrastructure.Persistence.Configurations.ML;

public class RecommendationRuleConfiguration : IEntityTypeConfiguration<RecommendationRule>
{
    public void Configure(EntityTypeBuilder<RecommendationRule> builder)
    {
        builder.ToTable("recommendation_rules", "ml");
        builder.HasKey(rr => rr.RecommendationRuleId);
        builder.Property(rr => rr.RecommendationRuleId).HasColumnType("uuid").ValueGeneratedOnAdd();
        builder.HasOne(rr => rr.IfItem).WithMany().HasForeignKey(rr => rr.IfItemId);
        builder.HasOne(rr => rr.ThenItem).WithMany().HasForeignKey(rr => rr.ThenItemId);
        builder.Property(rr => rr.Support);
        builder.Property(rr => rr.Confidence);
        builder.Property(rr => rr.Lift);
        builder.Property(rr => rr.CreatedAt);
    }
}