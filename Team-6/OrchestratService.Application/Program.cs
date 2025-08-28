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
			 x.AddConsumer<ConversationEventConsumer>();
			 x.AddConsumer<DefineAgentConsumer>();
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
				 cfg.ReceiveEndpoint(rabbitConfig.DefineAgentEvent, e =>
				 {
					 e.ConfigureConsumer<DefineAgentConsumer>(context);
				 });
			 });
         });
     });
