using ConversationDistributed.Consumers;
using ConversationDistributed.Services;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

// ������ ��������� �������������
builder.Services.AddSingleton<IAgentStateService, AgentStateService>();
builder.Services.AddSingleton<ConvState>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserLoggedInConsumer>();
    x.AddConsumer<DefineOperatorForConversationConsumer>();
	x.AddConsumer<AgentAnsweredConsumer>();

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

		cfg.ReceiveEndpoint("agent-answered-event-queue", e =>
		{
			e.ConfigureConsumeTopology = true;
			e.ConfigureConsumer<AgentAnsweredConsumer>(context);
		});
	});
});

var host = builder.Build();
var convState = host.Services.GetRequiredService<ConvState>();
await convState.ExecuteAsync(CancellationToken.None);
await host.RunAsync();