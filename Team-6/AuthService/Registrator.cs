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
                .AddHttpContextAccessor()
                .InstallServices()
                .ConfigureContext(connectionSettings.ApplicationDbContext)
                .InstallRepositories();
            return services;
        }

        private static IServiceCollection InstallServices(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<IUserService, UserService>()
                .AddTransient<IAuthService, Auth.Application.Services.AuthService>();

            return serviceCollection;
        }

        private static IServiceCollection InstallRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<IUserRepository, UserRepository>()
                .AddTransient<ITokenRepository, TokenRepository>();

            return serviceCollection;
        }
    }
}
