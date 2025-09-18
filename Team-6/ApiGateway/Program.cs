using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

    // Хардкодим настройки JWT
    var secret = "pE3kZs7V1cG9Yd9aM6+UjzQfXw2L8b0qR9nNw4eWzvJtC1kXh5mTzZsV7pQyU8hR";
    var issuer = "Authorization-api";
    var audience = "Authorization-client";

    // Подключаем аутентификацию
    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "JwtAuth";
            options.DefaultChallengeScheme = "JwtAuth";
        })
        .AddJwtBearer("JwtAuth", options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,

                ValidateAudience = true,
                ValidAudience = audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30) // допуск по времени
            };
        });


var app = builder.Build();
app.UseCors("AllowFrontend");
app.UseAuthentication();
await app.UseOcelot();

app.Run();