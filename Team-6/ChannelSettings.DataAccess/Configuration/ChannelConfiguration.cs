using ChannelSettings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChannelSettings.DataAccess.Configuration
{
    public class ChannelConfiguration : IEntityTypeConfiguration<Channel>
    {
        public void Configure(EntityTypeBuilder<Channel> builder)
        {
			builder.ToTable("channels", "channel");

			builder.HasKey(e => e.Id);

			builder.Property(e => e.Id)
			.HasColumnName("id")
			.IsRequired();

			builder.Property(e => e.Name)
			.HasColumnName("name")
			.IsRequired();
			
			builder.Property(e => e.Token)
			.HasColumnName("token")
			.IsRequired();
			
			builder.Property(e => e.Type)
			.HasColumnName("type")
			.HasConversion<string>() // сохранение enum как строки
			.IsRequired();
        }
    }
}
