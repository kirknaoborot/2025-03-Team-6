using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProfileService.Infrastructure.Context;

namespace ProfileService.Infrastructure.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionSting = configuration.GetConnectionString("Profile");

            services.AddDbContext<ProfileServiceContext>(options => options.UseNpgsql(connectionSting));

            return services;
        }
    }
}
