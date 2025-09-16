using ConversationDistributed.Consumers;
using ConversationDistributed.Services;
using ConversationState;
using ConversationState.Services;
using MassTransit;


var builder = Host.CreateApplicationBuilder(args);

// Сервис состояния операторов
builder.Services.AddSingleton<IAgentStateService, AgentStateService>();

// Новый сервис отслеживания обращений
builder.Services.AddHostedService<ConvState>();

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

        cfg.ReceiveEndpoint("user-login-event-queue", e =>
        {
            e.ConfigureConsumer<UserLoggedInConsumer>(context);
        });

        cfg.ReceiveEndpoint("define-operator-for-conversation-command-queue", e =>
        {
            e.ConfigureConsumer<DefineOperatorForConversationConsumer>(context);
        });
    });
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();