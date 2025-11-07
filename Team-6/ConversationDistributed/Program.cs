using ConversationDistributed.Consumers;
using ConversationDistributed.Services;
using MassTransit;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

// Настройка Serilog из конфигурации
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateBootstrapLogger(); // bootstrap для раннего логирования

var applicationName = builder.Configuration["Serilog:Properties:Application"] ?? "Unknown Service";
Log.Information("Starting up {@ApplicationName}", applicationName);

// Регистрация Serilog как провайдера логирования
builder.Services.AddSerilog();

// Сервис состояния пользователей
builder.Services.AddSingleton<IAgentStateService, AgentStateService>();
builder.Services.AddSingleton<ConvState>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AgentStatusCommandConsumer>();
    x.AddConsumer<DefineOperatorForConversationCommandConsumer>();
	x.AddConsumer<AgentAnsweredCommandConsumer>();

	x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ReceiveEndpoint("user-command-queue", e =>
        {
            e.ConfigureConsumeTopology = true;
            e.ConfigureConsumer<AgentStatusCommandConsumer>(context);
        });

        cfg.ReceiveEndpoint("define-operator-for-conversation-command-queue", e =>
        {
            e.ConfigureConsumeTopology = true;
            e.ConfigureConsumer<DefineOperatorForConversationCommandConsumer>(context);
        });

		cfg.ReceiveEndpoint("agent-answered-command-queue", e =>
		{
			e.ConfigureConsumeTopology = true;
			e.ConfigureConsumer<AgentAnsweredCommandConsumer>(context);
		});
	});
});

var host = builder.Build();
var convState = host.Services.GetRequiredService<ConvState>();
convState.ExecuteAsync(CancellationToken.None);
await host.RunAsync();