using MassTransit;
using MessageHubService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConversationService.Infrastructure.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<SendMessageEventConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest"); // или guest
                    h.Password("guest"); // или guest
                });

                cfg.ReceiveEndpoint("send-message-event-queue", e =>
                {
                    e.ConfigureConsumer<SendMessageEventConsumer>(context);
                });
            });
        });

        return services;
    }
}