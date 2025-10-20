using ConversationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ConversationService.Infrastructure.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> entity)
    {
        entity.ToTable("conversations", schema: "conversation");

        entity.HasKey(e => e.ConversationId);
        entity.Property(e => e.ConversationId).HasColumnName("conversation_id");

        entity.Property(e => e.Channel)
            .HasColumnName("channel")
            .HasConversion<string>() // сохранение enum как строки
            .IsRequired();

        entity.Property(e => e.Message)
            .HasColumnName("message")
            .IsRequired();

        entity.Property(e => e.Status)
            .HasColumnName("status")
            .HasConversion<string>() // сохранение enum как строки
            .IsRequired();

        entity.Property(e => e.WorkerId)
            .HasColumnName("worker_id");

        var dtoUtcConverter = new ValueConverter<DateTimeOffset, DateTime>(
            v => v.UtcDateTime,
            v => new DateTimeOffset(DateTime.SpecifyKind(v, DateTimeKind.Utc))
        );

        entity.Property(e => e.CreateDate)
            .HasColumnName("create_date")
            .HasConversion(dtoUtcConverter)
            .IsRequired();

        entity.Property(e => e.UpdateDate)
            .HasColumnName("update_date")
            .HasConversion(dtoUtcConverter)
            .IsRequired();

        entity.Property(e => e.Answer)
            .HasColumnName("answer");

        entity.Property(e => e.PrefixNumber)
            .HasColumnName("prefix_number");

        entity.Property(e => e.Number)
            .HasColumnName("number")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.UserId)
            .HasColumnName("user_id");

        entity.Property(e => e.ChannelSettingsId)
            .HasColumnName("channel_settings_id");


        entity.Property(e => e.Number)
            .Metadata.SetBeforeSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);

        entity.Property(e => e.Number)
            .Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);

        entity.HasIndex(e => e.Number)
            .IsUnique();
    }
}