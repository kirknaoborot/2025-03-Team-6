using CitizenRequest.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CitizenRequest.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<Citizen_requests> Citizen_requestList { get; set; }
        public DbSet<Files> FileList { get; set; }
        public DbSet<Channel_tip> Channel_tipList { get; set; }
        public DbSet<Channels> ChannelList { get; set; }
        public DbSet<Workers> WorkerList { get; set; }

        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql(_configuration.GetConnectionString("DBConnection"))
                .UseLoggerFactory(CreateLoggerFactory())
                .UseLazyLoadingProxies()
                .EnableSensitiveDataLogging();
        }

        public ILoggerFactory CreateLoggerFactory()
        {
            return LoggerFactory.Create(builder => builder.AddConsole());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Передаем конфигурацию для создания нашей сущности
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
