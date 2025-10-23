using ChannelSettings.Domain.Entities;
using Infrastructure.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace ChannelSettings.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<Channel> Channels { get; set; }


        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql(ConnectionStringEncryption.Decrypt(_configuration.GetConnectionString("ApplicationDbContext")))
                .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Передаем конфигурацию для создания нашей сущности
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
