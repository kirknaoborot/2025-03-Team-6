using Auth.Application.HubSignalR;
using Auth.Application.Services;
using Auth.Core.IRepositories;
using Auth.Core.IServices;
using Auth.DataAccess;
using Auth.DataAccess.Repositories;
using AuthService.Settings;


namespace AuthService
{
    public static class Registrator
    {

        public static IServiceCollection AddServices(this IServiceCollection services, ConnectionOptions connectionSettings, IConfiguration configuration)
        {
            services
                    .InstallServices()
                    .ConfigureContext(connectionSettings.ApplicationDbContext)
                    .InstallRepositories()
                    .InstallSignalR();
            return services;
        }

        private static IServiceCollection InstallServices(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<IUserService, UserService>();

            return serviceCollection;
        }

        private static IServiceCollection InstallRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<IUserRepository, UserRepository>();

            return serviceCollection;
        }

        private static IServiceCollection InstallSignalR(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSignalR().AddHubOptions<MessageHub>(options =>
            {
                options.EnableDetailedErrors = true;
            });
            return serviceCollection;
        }
    }
}
