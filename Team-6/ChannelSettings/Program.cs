using ChannelSettings;

var app = WebApplication.CreateBuilder(args)
    .RegisterServices()
    .Build();

using (var scope = app.Services.CreateScope())
{
    var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    // ������� ������, �������� ��� ��� ���, ������ ���� ����
    var startupLogger = loggerFactory.CreateLogger("Startup");

    //  �������� ILogger � SetupMiddleware 
    // ������ �������� SetupMiddleware � ��������
    app.SetupMiddleware(startupLogger);
}

app.Run();







