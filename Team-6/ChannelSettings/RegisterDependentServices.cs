using AutoMapper;
using ChannelSettings.Application.Services;
using ChannelSettings.Core.IRepositories;
using ChannelSettings.Core.IServices;
using ChannelSettings.Core.MappingModels;
using ChannelSettings.DataAccess;
using ChannelSettings.DataAccess.Repositories;
using ChannelSettings.MappingModel;
using ChannelSettings.Settings;
using MassTransit;
using Microsoft.Extensions.Options;
using Serilog;

namespace ChannelSettings
{
    public static class RegisterDependentServices
    {
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });

            builder.Services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            builder.Services.AddMemoryCache();



            builder.Services.AddDbContext<ApplicationDbContext>();


            builder.Services.Configure<StorageStrings>(options =>
            {
                options.ApplicationDbContext = builder.Configuration.GetValue<string>("StorageStrings:ApplicationDbContext")!;
            });

            builder.Services.AddSingleton<IStorageStrings>(options =>
               options.GetRequiredService<IOptions<StorageStrings>>().Value);


            builder.Services.AddServices();

            return builder;
        }


        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                    .InstallServices()
                    .InstallRepositories()
                    .InstallAutomapper()
                    .InstallHostedServices()
                    .AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
                    {
                        builder.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader()
                        .AllowCredentials()
                        .WithExposedHeaders("X-Pagination");
                    }))
                    .AddSwaggerGen()
                    .AddControllers();
            return services;
        }

        private static MapperConfiguration GetMapperConfiguration()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ChannelModelMapper>();
                cfg.AddProfile<ChannelDtoMapper>();

            });
            configuration.AssertConfigurationIsValid();
            return configuration;
        }

        private static IServiceCollection InstallAutomapper(this IServiceCollection services)
        {
            services.AddSingleton<IMapper>(new Mapper(GetMapperConfiguration()));
            return services;
        }

        private static IServiceCollection InstallServices(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<IChannelService, ChannelService>();
            return serviceCollection;
        }

        private static IServiceCollection InstallRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<IChannelRepository, ChannelRepository>();
            return serviceCollection;
        }

        private static IServiceCollection InstallHostedServices(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddHostedService<Worker>();
            return serviceCollection;
        }
    }
}
