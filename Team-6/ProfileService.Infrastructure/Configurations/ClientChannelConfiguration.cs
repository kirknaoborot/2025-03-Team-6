using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProfileService.Domain.Entities;

namespace ProfileService.Infrastructure.Configurations
{
    public class ClientChannelConfiguration : IEntityTypeConfiguration<ClientChannel>
    {
        public void Configure(EntityTypeBuilder<ClientChannel> entity)
        {
            entity.ToTable("client_channels");

            entity.HasKey(e => e.Id);
            entity.HasAlternateKey(e => e.ClientId);
            entity.Property(e => e.ExternalChannelType).HasColumnName("channel_type");
            entity.Property(e => e.ExternalId).HasColumnName("external_id");
        }
    }
}
