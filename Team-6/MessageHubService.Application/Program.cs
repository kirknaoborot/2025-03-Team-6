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

                var recEndp = hostContext.Configuration
                 .GetSection(nameof(ReceiveEndpointOptions))
                 .Get<ReceiveEndpointOptions>() ?? throw new ArgumentException("Missing ReceiveEndpointOptions configuration section");

                services.AddMassTransit(x =>
                {
                    x.SetSnakeCaseEndpointNameFormatter();
                    x.UsingRabbitMq((IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator cfg) =>
                    {
                        cfg.Host(rabbitConfig.Host, "/", h => {
                            h.Username(rabbitConfig.Username);
                            h.Password(rabbitConfig.Password);
                        });
                        cfg.Message<ClientMessage>(m => m.SetEntityName(recEndp.ExchangeName));
                        cfg.Send<ClientMessage>(s =>
                        {
                            s.UseRoutingKeyFormatter(m => m.Message.Priority);
                        });
                        cfg.Publish<ClientMessage>(t =>
                        {
                            t.ExchangeType = "topic";
                        });
                    });
                });

                services.Configure<TelegramBotOptionsList>(hostContext.Configuration.GetSection(nameof(TelegramBotOptionsList)));
                services.AddHostedService<SendMessageService>();
            });