using AspNetCoreRateLimit;
using Microsoft.Extensions.Options;
using CitizenRequest.DataAccess;
using CitizenRequests.API.Settings;
using CitizenRequests.Core.IRepositories;
using Serilog;

namespace CitizenRequests.API
{
    public static class RegisterDependentServices
    {
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddSerilog();

            builder.Services.AddInMemoryRateLimiting();

            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            builder.Services.AddDbContext<ApplicationDbContext>();

            builder.Services.Configure<StorageStrings>(options =>
            {
                options.DBConnection = builder.Configuration.GetValue<string>("StorageStrings:DBConnection")!;
            });

             builder.Services.AddSingleton<IStorageStrings>(options =>
             options.GetRequiredService<IOptions<StorageStrings>>().Value);

            builder.Services.AddServices();

            return builder;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                    .AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
                    {
                        builder.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader();
                    }))
                    .AddSwaggerGen()
                    .AddControllers();
            return services;
        }
    }
}
