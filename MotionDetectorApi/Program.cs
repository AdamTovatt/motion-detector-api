using Dapper;
using MotionDetectorApi.Authorization;
using MotionDetectorApi.Helpers;
using MotionDetectorApi.RateLimiting;

namespace MotionDetectorApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("API_KEY")))
                throw new Exception("The api is badly configured and is missing api key value in the environment variables");

            SetupDatabase();

            StringIdProvider.LoadAsync().Wait();

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSingleton<ApiKeyAuthorizationFilter>();
            builder.Services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();

            WebApplication app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseCors(options =>
            {
                options.AllowAnyHeader()
                       .AllowAnyMethod()
                       .SetIsOriginAllowed(origin => true) // Allow any origin
                       .AllowCredentials(); // Allow credentials (e.g., cookies)
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.UseRateLimiting();

            app.Run();
        }

        public static void SetupDatabase()
        {
            Migrator.PerformDatabaseMigrations(); // run database migrations

            DefaultTypeMap.MatchNamesWithUnderscores = true; // set up dapper to match column names with underscore
        }
    }
}