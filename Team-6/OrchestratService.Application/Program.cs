using Infrastructure.Shared;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OrchestratService.Application;
using Serilog;

// Настройка Serilog ДО создания хоста
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateBootstrapLogger(); // для раннего логирования

var applicationName = configuration["Serilog:Properties:Application"] ?? "Unknown Service";
Log.Information("Starting up {@ApplicationName}", applicationName);

var host = CreateHostBuilder(args).Build();
host.Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
     Host.CreateDefaultBuilder(args)
     .UseSerilog() // подключает Serilog как провайдер ILogger<T>
     .ConfigureServices((hostContext, services) =>
     {
         var rabbitConfig = hostContext.Configuration
               .GetSection(nameof(RabbitMqOptions))
               .Get<RabbitMqOptions>() ?? throw new ArgumentException("Missing RabbitMq configuration section");

         services.AddMassTransit(x =>
         {
             x.SetSnakeCaseEndpointNameFormatter();
             x.AddConsumer<ClientMessageEventConsumer>();
			 x.AddConsumer<ConversationEventConsumer>();
			 x.AddConsumer<DefineAgentEventConsumer>();
			 x.AddConsumer<SendMessageEventConsumer>();
			 x.AddConsumer<NotifySendEventConsumer>();
			 x.UsingRabbitMq((IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator cfg) =>
             {
                 cfg.Host(rabbitConfig.Host, "/", h => {
                     h.Username(rabbitConfig.Username);
                     h.Password(rabbitConfig.Password);
                 });
                 cfg.ReceiveEndpoint(rabbitConfig.ClientMessageEventQueue, e =>
                 {
                     e.ConfigureConsumer<ClientMessageEventConsumer>(context);
                 });
				 cfg.ReceiveEndpoint(rabbitConfig.ConversationEventQueue, e =>
				 {
					 e.ConfigureConsumer<ConversationEventConsumer>(context);
				 });
				 cfg.ReceiveEndpoint(rabbitConfig.DefineAgentEventQueue, e =>
				 {
					 e.ConfigureConsumer<DefineAgentEventConsumer>(context);
				 });
				 cfg.ReceiveEndpoint(rabbitConfig.SendMessageEventQueue, e =>
				 {
					 e.ConfigureConsumer<SendMessageEventConsumer>(context);
				 });
				 cfg.ReceiveEndpoint(rabbitConfig.NotifySendEventQueue, e =>
				 {
					 e.ConfigureConsumer<NotifySendEventConsumer>(context);
				 });
			 });
         });
     });
