using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace ChannelSettings.DataAccess
{
    public static class DBConfiguration
    {
        public static IServiceCollection ConfigureContext(this IServiceCollection services,
           string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
                optionsBuilder
                 .UseLazyLoadingProxies()
                .UseNpgsql(connectionString));
            return services;
        }
    }
}
