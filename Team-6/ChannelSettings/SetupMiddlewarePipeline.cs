using ChannelSettings.DataAccess;


namespace ChannelSettings
{
    public static class SetupMiddlewarePipeline
    {
        public static WebApplication SetupMiddleware(this WebApplication app, ILogger logger)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
            }

            app.UseHttpsRedirection();
            app.UseWebSockets();

            app.UseStaticFiles();
            app.UseCors("ApiCorsPolicy");
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Логирование перед попыткой создания БД
            logger.LogInformation("Проверка и создание базы данных...");

            using (var scope = app.Services.CreateScope())
            {
                ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    logger.LogInformation("Попытка создания таблиц через EnsureCreated...");
                    var created = db.Database.EnsureCreated();
                    if (!created)
                    {
                        logger.LogInformation($"Таблицы ранее были созданы");
                    }
                    else
                    {
                        logger.LogInformation($"Таблицы созданы: {created}");

                    }
                }
                catch (Exception ex2)
                {
                    logger.LogError(ex2, "Ошибка при создании таблиц: {Message}", ex2.Message);
                }

            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            return app;
        }
    }
}
