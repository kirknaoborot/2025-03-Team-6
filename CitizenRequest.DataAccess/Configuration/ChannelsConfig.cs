using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using CitizenRequest.Domain.Entities;

namespace CitizenRequest.DataAccess.Configuration
{
    public class ChannelsConfig : IEntityTypeConfiguration<Channels>
    {
        public void Configure(EntityTypeBuilder<Channels> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("Channels", "handbooks");

            builder.Property(e => e.Name).IsRequired(false).HasMaxLength(50);
            builder.Property(e => e.Login).IsRequired(false);
            builder.Property(e => e.Password).IsRequired(false);
            builder.Property(e => e.Address).IsRequired(false).HasMaxLength(150);
            builder.Property(e => e.Port).IsRequired(false).HasMaxLength(150);
            builder.Property(e => e.Token).IsRequired(false).HasMaxLength(150);
            builder.Property(e => e.Channel_tip_id).IsRequired();
            builder.Property(e => e.Is_activ).IsRequired(false);
          

            builder
                .HasOne(e => e.Chan_tip)
                .WithMany(g => g.Tips)
                .HasForeignKey(e => e.Channel_tip_id)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
