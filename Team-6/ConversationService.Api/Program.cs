using ConversationService.Infrastructure.Extensions;
using Infrastructure.Shared.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Настройка Serilog из конфигурации
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateBootstrapLogger(); // bootstrap для раннего логирования

// Регистрация Serilog как провайдера логирования
builder.Services.AddSerilog();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<HeaderClaimsMiddleware>();
app.Run();