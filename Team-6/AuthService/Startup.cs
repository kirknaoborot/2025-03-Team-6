using Auth.Application.Services;
using Auth.Core.Services;
using Auth.DataAccess;
using Auth.Domain.Entities;
using AuthService.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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
            this.ConfigureAuthentication(services);
            services.AddAuthorization();
        }

        private void ConfigureAuthentication(IServiceCollection services)
        {
            var settingsSection = Configuration.GetSection("ApiSettings:JwtOptions");

            var secret = settingsSection.GetValue<string>("Secret");
            var issuer = settingsSection.GetValue<string>("Issuer");
            var audience = settingsSection.GetValue<string>("Audience");

            var key = Encoding.ASCII.GetBytes(secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidateAudience = true,
                };
            });
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
