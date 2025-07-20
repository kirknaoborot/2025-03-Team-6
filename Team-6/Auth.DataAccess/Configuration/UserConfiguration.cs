using Auth.Domain.Entities;
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

            builder.Property(e => e.FullName).IsRequired(false).HasMaxLength(100);
            builder.Property(e => e.Login).IsRequired(false);
            builder.Property(e => e.PasswordsHash).IsRequired(false);
            builder.Property(e => e.IsActive).IsRequired(false);
            builder.Property(e => e.Status).IsRequired(false).HasMaxLength(20);
            builder.Property(e => e.LastSeen).IsRequired(false).HasColumnType("date");
            builder.Property(e => e.Role).IsRequired(false).HasMaxLength(50);
        }
    }
}
