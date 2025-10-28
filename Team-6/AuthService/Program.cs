using Auth.DataAccess;
using AuthService;
using Serilog;


public class Program
{
    public static void Main(string[] args)
    {
        // Создаём конфигурацию
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateBootstrapLogger(); //  bootstrap-логгер для старта

        try
        {
            var applicationName = configuration["Serilog:Properties:Application"] ?? "Unknown Service";
            Log.Information("Starting up {@ApplicationName}", applicationName);

            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContexts>();

                db.Database.EnsureCreated();
                db.SaveChanges();
            }

            host.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, $"{nameof(Program)}.{nameof(Main)}() -> Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }


    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}


