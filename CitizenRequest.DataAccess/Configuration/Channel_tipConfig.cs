using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using CitizenRequest.Domain.Entities;

namespace CitizenRequest.DataAccess.Configuration
{
    public class Channel_tipConfig : IEntityTypeConfiguration<Channel_tip>
    {
        public void Configure(EntityTypeBuilder<Channel_tip> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("Channel_tip", "handbooks");

            builder.Property(e => e.Tip).IsRequired(false).HasMaxLength(150);
        }
    }
}
