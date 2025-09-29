using ConversationDistributed.Consumers;
using ConversationDistributed.Services;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

// Сервис состояния пользователей
builder.Services.AddSingleton<IAgentStateService, AgentStateService>();
builder.Services.AddSingleton<ConvState>();

// MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserLoggedInConsumer>();
    x.AddConsumer<DefineOperatorForConversationConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Слушаем: кто вошёл
        cfg.ReceiveEndpoint("user-login-event-queue", e =>
        {
            e.ConfigureConsumer<UserLoggedInConsumer>(context);
        });

        // Слушаем: новое обращение
        cfg.ReceiveEndpoint("define-operator-for-conversation-command-queue", e =>
        {
            e.ConfigureConsumer<DefineOperatorForConversationConsumer>(context);
        });
    });
});


var host = builder.Build();
var convState = host.Services.GetRequiredService<ConvState>();
convState.ExecuteAsync(CancellationToken.None);
await host.RunAsync();