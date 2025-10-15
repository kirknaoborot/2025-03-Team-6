using Auth.Application.Consumer;
using Auth.Application.Services;
using Auth.Core.Services;
using Auth.DataAccess;
using Auth.Domain.Entities;
using AuthService.Settings;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;


namespace AuthService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public ConnectionOptions ConnectionStrings { get; init; } = new ConnectionOptions();


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Configuration.GetSection(ConnectionOptions.Section).Bind(ConnectionStrings);
            Configuration.GetSection($"{ConnectionOptions.Section}:{RmqSettings.Section}").Bind(ConnectionStrings.RmqSettings);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContexts>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("ApplicationDbContext")));


            services.AddSignalR();

            services.AddServices(ConnectionStrings, Configuration);
            services.AddSerilog();

            services.AddControllers();
            services.AddControllers().AddMvcOptions(x => x.SuppressAsyncSuffixInActionNames = false);

            services.Configure<JwtOptions>(Configuration.GetSection("ApiSettings:JwtOptions"));

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            
            services.AddEndpointsApiExplorer();
            
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            // использование Bearer токенов
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                "Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
                "Example: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                        {
                            new OpenApiSecurityScheme
                            {
                                 Reference=new OpenApiReference
                                     {
                                         Type=ReferenceType.SecurityScheme,
                                         Id=JwtBearerDefaults.AuthenticationScheme
                                     }
                            }, Array.Empty<string>()

                        }
                 });
            });

            // Хардкодим настройки JWT
            var secret = "pE3kZs7V1cG9Yd9aM6+UjzQfXw2L8b0qR9nNw4eWzvJtC1kXh5mTzZsV7pQyU8hR";
            var issuer = "Authorization-api";
            var audience = "Authorization-client";

            // Подключаем аутентификацию
            services
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
                        ClockSkew = TimeSpan.FromMinutes(30) // допуск по времени
                    };
                });

            services.AddAuthorization();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContexts db)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Authorization.Api");

            });

            app.UseHttpsRedirection();
            app.UseWebSockets();

            app.UseCors("AllowFrontend");
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
