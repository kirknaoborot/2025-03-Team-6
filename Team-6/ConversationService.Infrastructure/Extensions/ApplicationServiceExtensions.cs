using ConversationService.Application.Interfaces;
using ConversationService.Infrastructure.DbContext;
using ConversationService.Infrastructure.Messaging;
using ConversationService.Infrastructure.Messaging.Consumers;
using ConversationService.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConversationService.Infrastructure.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionSting = configuration.GetConnectionString("Conversation");

        services.AddDbContext<ConversationServiceContext>(options => options.UseNpgsql(connectionSting));

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ConversationConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest"); // или guest
                    h.Password("guest"); // или guest
                });

                cfg.ReceiveEndpoint("conversation-command-queue", e =>
                {
                    e.ConfigureConsumer<ConversationConsumer>(context);
                });
            });
        });
        
        // register services
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IConversationService, Application.Services.ConversationService>();
        services.AddScoped<IMessageBusPublisher, MessageBusPublisher>();

        return services;
    }
}