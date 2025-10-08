using ChannelSettings.DataAccess;


namespace ChannelSettings
{
    public static class SetupMiddlewarePipeline
    {
        public static WebApplication SetupMiddleware(this WebApplication app)
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

            using (var scope = app.Services.CreateScope())
            {
                ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var created = db.Database.EnsureCreated();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            return app;
        }
    }
}
