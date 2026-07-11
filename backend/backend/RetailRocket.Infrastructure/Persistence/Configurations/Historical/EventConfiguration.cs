using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Infrastructure.Persistence.Configurations.Historical;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events", "historical");
        builder.HasKey(e => e.EventId);
        builder.Property(e => e.EventId).ValueGeneratedOnAdd();
        builder.HasOne(e => e.Visitor).WithMany().HasForeignKey(e => e.VisitorId);
        builder.HasOne(e => e.Item).WithMany().HasForeignKey(e => e.ItemId);
        builder.Property(e => e.EventType).HasMaxLength(20).HasConversion<string>();
        builder.Property(e => e.Timestamp).IsRequired();
    }
}