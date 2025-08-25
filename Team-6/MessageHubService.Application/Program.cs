using Infrastructure.Shared;
using MassTransit;
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
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(rabbitConfig.Host, "/", h => {
                            h.Username(rabbitConfig.Username);
                            h.Password(rabbitConfig.Password);
                        });
                    });
                });

                services.Configure<TelegramBotOptionsList>(hostContext.Configuration.GetSection(nameof(TelegramBotOptionsList)));
                services.AddHostedService<SendMessageService>();
            });