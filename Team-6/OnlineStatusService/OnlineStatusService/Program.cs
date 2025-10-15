using MassTransit; // <-- обязательно для MassTransit
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using OnlineStatusService;
using OnlineStatusService.Consumers;
using Serilog;

var builder = Host.CreateDefaultBuilder(args)

    //Подключаем Serilog к хосту
    .UseSerilog((hostingContext, loggerConfiguration) =>
    {
        loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration);
    })
    .ConfigureServices((hostContext, services) =>
    {
        // Фоновый сервис
        services.AddHostedService<Worker>();

        // SignalR
        services.AddSignalR();
        services.AddSingleton<IAgentStatusNotifier, AgentStatusNotifier>();

        // MassTransit + RabbitMQ
        services.AddMassTransit(x =>
        {
            x.AddConsumer<NotifySendConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ReceiveEndpoint("notify-send-event-queue", e =>
                {
                    e.ConfigureConsumeTopology = true;
                    e.ConfigureConsumer<NotifySendConsumer>(context);
                });
            });

        });

        // CORS для фронтенда
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins("http://localhost:5173")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.Configure(app =>
        {
            app.UseRouting();
            app.UseCors("AllowFrontend");

            // SignalR Hub
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<OnlineStatusHub>("/onlinestatus");
            });
        });

        webBuilder.UseUrls("http://localhost:54000");
    });

var host = builder.Build();
await host.RunAsync();
