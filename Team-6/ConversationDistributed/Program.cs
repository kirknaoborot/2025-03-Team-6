using ConversationDistributed.Consumers;
using ConversationDistributed.Services;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

// Сервис состояния пользователей
builder.Services.AddSingleton<IAgentStateService, AgentStateService>();
builder.Services.AddSingleton<ConvState>();
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
            e.ConfigureConsumeTopology = true;
            e.ConfigureConsumer<UserLoggedInConsumer>(context);
        });

        cfg.ReceiveEndpoint("define-operator-for-conversation-command-queue", e =>
        {
            e.ConfigureConsumeTopology = true;
            e.ConfigureConsumer<DefineOperatorForConversationConsumer>(context);
        });
    });
});

var host = builder.Build();
var convState = host.Services.GetRequiredService<ConvState>();
convState.ExecuteAsync(CancellationToken.None);
await host.RunAsync();