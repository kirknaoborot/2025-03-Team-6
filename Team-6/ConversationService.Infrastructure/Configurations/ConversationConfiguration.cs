using ConversationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConversationService.Infrastructure.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> entity)
    {
        entity.ToTable("conversations");

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

        entity.Property(e => e.CreateDate)
            .HasColumnName("create_date")
            .IsRequired();

        entity.Property(e => e.UpdateDate)
            .HasColumnName("update_date")
            .IsRequired();
    }
}