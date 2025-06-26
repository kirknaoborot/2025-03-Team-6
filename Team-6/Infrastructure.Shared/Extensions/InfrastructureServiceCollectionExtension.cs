using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Shared.Extensions
{
    internal static class InfrastructureServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructure<T>(this IServiceCollection services, IConfiguration configuration, string connectionStringName) where T : DbContext
        {
            var connectionSting = configuration.GetConnectionString(connectionStringName);

            services.AddDbContext<T>(options => options.UseNpgsql(connectionSting));

            return services;
        }
    }
}
