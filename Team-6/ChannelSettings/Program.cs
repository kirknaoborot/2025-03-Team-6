using ChannelSettings;

var app = WebApplication.CreateBuilder(args)
    .RegisterServices()
    .Build();

using (var scope = app.Services.CreateScope())
{
    app.SetupMiddleware();
}

app.Run();







