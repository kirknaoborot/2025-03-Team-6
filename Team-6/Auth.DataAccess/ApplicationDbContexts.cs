using CitizenRequest.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Auth.DataAccess.Configuration;

namespace Auth.DataAccess
{
    public class ApplicationDbContexts : IdentityDbContext<ApplicationUser>
    {
   
        public ApplicationDbContexts(DbContextOptions<ApplicationDbContexts> options) : base(options)
        { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
