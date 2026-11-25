using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Historical;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events", "historical");
        builder.HasKey(e => e.event_id);
        builder.Property(e => e.event_id).ValueGeneratedOnAdd();
        builder.HasOne(e => e.visitor).WithMany().HasForeignKey(e => e.visitor_id);
        builder.HasOne(e => e.item).WithMany().HasForeignKey(e => e.item_id);
        builder.Property(e => e.event_type).HasMaxLength(20).HasConversion<string>();
        builder.Property(e => e.ts).IsRequired();
    }
}