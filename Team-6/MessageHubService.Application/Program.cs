using Infrastructure.Shared;
using MassTransit;
using MessageHubService.Application;
using MessageHubService.Application.Interfaces;
using MessageHubService.Application.Services;
using MessageHubService.Application.Services.TelegramBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

// Настройка Serilog ДО создания хоста
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateBootstrapLogger(); 

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
				x.AddConsumer<SendMessageEventConsumer>();
				x.AddConsumer<ChannelEventConsumer>();

				x.UsingRabbitMq((context, cfg) =>
				{
					cfg.Host(rabbitConfig.Host, "/", h => {
						h.Username(rabbitConfig.Username);
						h.Password(rabbitConfig.Password);
					});

					cfg.ReceiveEndpoint("send-message-event-queue", e =>
					{
						e.ConfigureConsumer<SendMessageEventConsumer>(context);
					});

					cfg.ReceiveEndpoint("channel-command-queue", e =>
					{
						e.ConfigureConsumer<ChannelEventConsumer>(context);
					});
				});
			});

			services.AddMemoryCache();
			services.AddScoped<IBot, TelegramBot>();
			services.AddSingleton<IBotManagerService, BotManagerService>();
		});