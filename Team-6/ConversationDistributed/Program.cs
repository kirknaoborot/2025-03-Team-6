using ConversationDistributed.Consumers;
using ConversationDistributed.Services;
using MassTransit;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

// ��������� Serilog �� ������������
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateBootstrapLogger(); // bootstrap ��� ������� �����������

var applicationName = builder.Configuration["Serilog:Properties:Application"] ?? "Unknown Service";
Log.Information("Starting up {@ApplicationName}", applicationName);

// ����������� Serilog ��� ���������� �����������
builder.Services.AddSerilog();

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
convState.ExecuteAsync(CancellationToken.None);
await host.RunAsync();