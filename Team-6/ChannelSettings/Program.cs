using ChannelSettings;

var app = WebApplication.CreateBuilder(args)
    .RegisterServices()
    .Build();

using (var scope = app.Services.CreateScope())
{
    var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    // Создаем логгер, указывая тип или имя, откуда идут логи
    var startupLogger = loggerFactory.CreateLogger("Startup");

    //  Передача ILogger в SetupMiddleware 
    // Теперь вызываем SetupMiddleware с логгером
    app.SetupMiddleware(startupLogger);
}

app.Run();







