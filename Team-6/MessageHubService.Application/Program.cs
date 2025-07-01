using MassTransit;
using MessageHubService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TestConsumer;

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
                    x.AddConsumer<EventConsumer>();
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(rabbitConfig.Host, "/", h => {
                            h.Username(rabbitConfig.Username);
                            h.Password(rabbitConfig.Password);
                        });
                        cfg.Message<ClientMessage>(m => m.SetEntityName(recEndp.ExchangeName));

                        cfg.ReceiveEndpoint(recEndp.QueueName, e =>
                        {
                            e.ConfigureConsumeTopology = false;
                            e.ConfigureConsumer<EventConsumer>(context);
                            e.Bind<ClientMessage>(p =>
                            {
                                p.ExchangeType = "topic";
                                p.RoutingKey = "high";
                            });
                            //e.UseMessageRetry(r =>
                            //{
                            // r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
                            //});
                        });
                    });
                });
            });