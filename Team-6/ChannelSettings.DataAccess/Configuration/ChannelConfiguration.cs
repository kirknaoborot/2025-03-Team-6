using ChannelSettings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChannelSettings.DataAccess.Configuration
{
    public class ChannelConfiguration : IEntityTypeConfiguration<Channel>
    {
        public void Configure(EntityTypeBuilder<Channel> builder)
        {
            builder.HasKey(e => e.Id);            
            
            builder.ToTable("channel", "public");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Token)
                .IsRequired();

            builder.Property(e => e.Type)
                .IsRequired()
                .HasConversion<string>(); // сохранение enum как строки
        }
    }
}
