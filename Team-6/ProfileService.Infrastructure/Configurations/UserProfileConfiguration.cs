using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProfileService.Domain.Entities;

namespace ProfileService.Infrastructure.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> entity)
        {
            entity.ToTable("user_profiles");

            entity.HasKey(e => e.Id);
            entity.HasAlternateKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FirstName).HasColumnName("first_name");
            entity.Property(e => e.LastName).HasColumnName("last_name");
            entity.Property(e => e.MiddleName).HasColumnName("middle_name");
            entity.Property(e => e.EditDate).HasColumnName("edit_date");

            entity.HasOne(e => e.User)
                .WithOne(e => e.UserProfile)
                .HasForeignKey<UserProfile>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
