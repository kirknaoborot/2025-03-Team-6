using CitizenRequest.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CitizenRequest.DataAccess.Configuration
{
    public class FilesConfig : IEntityTypeConfiguration<Files>
    {
        public void Configure(EntityTypeBuilder<Files> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("Files", "handbooks");

            builder.Property(e => e.Name).IsRequired(false).HasMaxLength(200);
            builder.Property(e => e.Comment).IsRequired(false).HasMaxLength(200);
            builder.Property(e => e.Request_id).IsRequired();
            builder.Property(x => x.FileId).HasDefaultValue(string.Empty).HasMaxLength(50);

            builder
                .HasOne(e => e.Citizen_req)
                .WithMany(g => g.File)
                .HasForeignKey(e => e.Request_id)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
