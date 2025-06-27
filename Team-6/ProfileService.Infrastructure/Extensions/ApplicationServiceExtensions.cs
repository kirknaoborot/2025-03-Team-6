using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProfileService.Application.Interfaces;
using ProfileService.Infrastructure.Context;
using ProfileService.Infrastructure.Repositories;

namespace ProfileService.Infrastructure.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionSting = configuration.GetConnectionString("Profile");

            services.AddDbContext<ProfileServiceContext>(options => options.UseNpgsql(connectionSting));

            // register services
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();

            return services;
        }
    }
}
