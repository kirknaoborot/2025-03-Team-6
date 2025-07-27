using MassTransit;
using OperatorStatusService;
using OperatorStatusService.Domain.Interfaces;
using OperatorStatusService.Infrastructure.Config;
using OperatorStatusService.Infrastructure.Services;


public class Program
{
    public static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                var appSettings = context.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

                services.AddMassTransit(x =>
                {
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(appSettings.RabbitMq.HostName, h =>
                        {
                            h.Username(appSettings.RabbitMq.UserName);
                            h.Password(appSettings.RabbitMq.Password);
                        });

                        cfg.ConfigureEndpoints(context);
                    });
                });

                services.AddMassTransitHostedService();

                services.AddHttpClient();

                services.AddScoped<OperatorMonitoringService>();
                services.AddScoped<IOperatorStatusService, AuthApiService>();

                services.AddHostedService<Worker>();
            })
            .Build()
            .RunAsync();
    }
}