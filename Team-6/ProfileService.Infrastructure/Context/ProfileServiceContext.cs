using Microsoft.EntityFrameworkCore;
using ProfileService.Infrastructure.Entities;

namespace ProfileService.Infrastructure.Context
{
    public class ProfileServiceContext : DbContext
    {
        public ProfileServiceContext(DbContextOptions<ProfileServiceContext> options) : base(options) 
        {

        }

        public DbSet<WorkerProfile> WorkerProfiles { get; set; }
        public DbSet<ClientProfile> ClientProfiles { get; set; }
    }
}
