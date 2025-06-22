using CitizenRequest.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.DataAccess.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("Users", "handbooks");

            builder.Property(e => e.Full_name).IsRequired(false).HasMaxLength(100);
            builder.Property(e => e.Login).IsRequired(false);
            builder.Property(e => e.Password_hash).IsRequired(false);
            builder.Property(e => e.Is_active).IsRequired(false);
            builder.Property(e => e.Status).IsRequired(false).HasMaxLength(20);
            builder.Property(e => e.Last_seen).IsRequired(false).HasColumnType("date");
            builder.Property(e => e.Role).IsRequired(false).HasMaxLength(50);
        }
    }
}
