using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.ML;

namespace RetailRocket.Infrastructure.Persistence.Configurations.ML;

public class RecommendationRuleConfiguration : IEntityTypeConfiguration<RecommendationRule>
{
    public void Configure(EntityTypeBuilder<RecommendationRule> builder)
    {
        builder.ToTable("recommendation_rules", "ml");
        builder.HasKey(rr => rr.rule_id);
        builder.Property(rr => rr.rule_id).HasColumnType("uuid").ValueGeneratedOnAdd();
        builder.HasOne(rr => rr.if_item).WithMany().HasForeignKey(rr => rr.if_item_id);
        builder.HasOne(rr => rr.then_item).WithMany().HasForeignKey(rr => rr.then_item_id);
        builder.Property(rr => rr.support);
        builder.Property(rr => rr.confidence);
        builder.Property(rr => rr.lift);
        builder.Property(rr => rr.created_at).HasDefaultValueSql("NOW()");
    }
}