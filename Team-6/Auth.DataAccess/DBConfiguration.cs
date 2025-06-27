using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.DataAccess
{
   public static class DBConfiguration
    {
        public static IServiceCollection ConfigureContext(this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContext<ApplicationDbContexts>(optionsBuilder =>
                optionsBuilder
                 .UseLazyLoadingProxies()
                .UseNpgsql(connectionString));
            return services;
        }
    }
}
