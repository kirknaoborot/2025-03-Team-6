using Infrastructure.Shared;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrchestratService.Application;

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
             x.SetSnakeCaseEndpointNameFormatter();
             x.AddConsumer<ClientMessageEventConsumer>();
             x.UsingRabbitMq((IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator cfg) =>
             {
                 cfg.Host(rabbitConfig.Host, "/", h => {
                     h.Username(rabbitConfig.Username);
                     h.Password(rabbitConfig.Password);
                 });
                 cfg.Message<ClientMessage>(m => m.SetEntityName("ClientMessage"));
                 cfg.Send<ClientMessage>(s =>
                 {
                     s.UseRoutingKeyFormatter(m => m.Message.Priority);
                 });
                 cfg.Publish<ClientMessage>(t =>
                 {
                     t.ExchangeType = "topic";
                 });
                 cfg.ReceiveEndpoint("ClientMessageEvent", e =>
                 {
                     e.ConfigureConsumer<ClientMessageEventConsumer>(context);
                     e.Bind<ClientMessage>(p =>
                     {
                         p.ExchangeType = "topic";
                         p.RoutingKey = "high";
                     });
                 });
             });
         });
     });
