using Infrastructure.Shared;
using MassTransit;
using MessageHubService.Application;
using MessageHubService.Application.Interfaces;
using MessageHubService.Application.Services;
using MessageHubService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = CreateHostBuilder(args).Build();
host.Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
       Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var rabbitConfig = hostContext.Configuration
                    .GetSection(nameof(RabbitMqOptions))
                    .Get<RabbitMqOptions>() ?? throw new ArgumentException("Missing RabbitMq configuration section");

                services.AddMassTransit(x =>
                {
					x.AddConsumer<SendMessageEventConsumer>();

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
					});
                });

				services.AddScoped<IBotWork, TelegramBotService>();
                services.Configure<TelegramBotOptionsList>(hostContext.Configuration.GetSection(nameof(TelegramBotOptionsList)));
                services.AddHostedService<SendMessageService>();
            });