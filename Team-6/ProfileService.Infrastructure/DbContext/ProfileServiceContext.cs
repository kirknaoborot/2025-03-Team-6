using Microsoft.EntityFrameworkCore;
using ProfileService.Domain.Entities;

namespace ProfileService.Infrastructure.Context
{
    public class ProfileServiceContext : DbContext
    {
        private const string SCHEMA = "profile";

        public ProfileServiceContext(DbContextOptions<ProfileServiceContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientChannel> ClientChannels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(SCHEMA);
        }
    }
}
