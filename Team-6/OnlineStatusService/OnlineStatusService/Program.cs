using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using MassTransit; // <-- обязательно для MassTransit
using OnlineStatusService;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Фоновый сервис
        services.AddHostedService<Worker>();

        // SignalR
        services.AddSignalR();

        // MassTransit + RabbitMQ
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
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
