using Infrastructure.Shared;
using MassTransit;
using Microsoft.Extensions.Configuration;
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
			 x.AddConsumer<CreateConversationEventConsumer>();
             x.UsingRabbitMq((IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator cfg) =>
             {
                 cfg.Host(rabbitConfig.Host, "/", h => {
                     h.Username(rabbitConfig.Username);
                     h.Password(rabbitConfig.Password);
                 });
                 cfg.ReceiveEndpoint("client-message-event-queue", e =>
                 {
                     e.ConfigureConsumer<ClientMessageEventConsumer>(context);
                 });
				 cfg.ReceiveEndpoint("create-conversation-event-queue", e =>
				 {
					 e.ConfigureConsumer<CreateConversationEventConsumer>(context);
				 });

             });
         });
     });
