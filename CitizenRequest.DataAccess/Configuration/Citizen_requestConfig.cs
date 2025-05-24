using CitizenRequest.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CitizenRequest.DataAccess.Configuration
{
    public class Citizen_requestConfig : IEntityTypeConfiguration<Citizen_requests>
    {
        public void Configure(EntityTypeBuilder<Citizen_requests> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("Citizen_requests", "handbooks");

            builder.Property(e => e.Channel_id).IsRequired();
            builder.Property(e => e.Sender_name).IsRequired();
            builder.Property(e => e.Sender_contact).IsRequired();
            builder.Property(e => e.Message).IsRequired(false).HasMaxLength(255);
            builder.Property(e => e.Status).IsRequired(false).HasMaxLength(20);
            builder.Property(e => e.Worker_id).IsRequired();
            builder.Property(e => e.Created_at).IsRequired(false).HasColumnType("date");
            builder.Property(e => e.Updated_at).IsRequired(false).HasColumnType("date");


            builder
                .HasOne(e => e.Channel)
                .WithMany(g => g.Citizen_req)
                .HasForeignKey(e => e.Channel_id)
                .OnDelete(DeleteBehavior.Cascade);

            builder
             .HasOne(e => e.Worker)
             .WithMany(g => g.Citizen_req)
             .HasForeignKey(e => e.Worker_id)
             .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
