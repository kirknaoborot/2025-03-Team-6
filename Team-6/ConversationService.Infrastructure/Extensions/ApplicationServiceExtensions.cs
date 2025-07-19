using ConversationService.Application.Interfaces;
using ConversationService.Infrastructure.DbContext;
using ConversationService.Infrastructure.Repositories;
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

        // register services
        services.AddScoped<IConversationRepository, ConversationRepository>();

        return services;
    }
}