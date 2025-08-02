using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.DataAccess.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("users", "auth");

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(e => e.FullName)
                .HasColumnName("full_name")
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(e => e.Login)
                .HasColumnName("login")
                .IsRequired();
            
            builder.Property(e => e.PasswordsHash)
                .HasColumnName("passwords_hash")
                .IsRequired();
            
            builder.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();
            
            builder.Property(e => e.Role)
                .HasColumnName("role")
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
