using Foxminded.StudyManager.Infrastructure.Persistence;
using Foxminded.StudyManager.Web.Middleware;
using Foxminded.StudyManager.Web.Middleware.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Foxminded.StudyManager.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var configBuilder = GetConfigurationForAppsettings(builder);
        var projectConfig = ReadConfigurationFromAppsettings(configBuilder);
        CreateLogger(builder);
        builder.Host.UseSerilog();

        builder.Services.AddControllersWithViews();
        builder.Services.AddApplicationServices()
            .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
        builder.Services.GetRequestLocalizationOptions();

        builder.Services.AddDbContext<StudyManagerDbContext>(options =>
                   options.UseSqlServer(builder.Configuration.GetConnectionString("StudyManagerDb")));

        var app = AppConfiguration(builder);
        app.UseGlobalExceptionHandler();
        app.UseRequestLocalization();
        ApplyMigrations(app);
        await app.RunAsync();
    }

    private static void ApplyMigrations(WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Starting migrations");
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<StudyManagerDbContext>();
                dbContext.Database.Migrate();
            }
        }
        catch (Exception exception)
        {
            logger.LogError("migrations failed: {0}", exception.Message);
        }
    }

    private static WebApplication AppConfiguration(WebApplicationBuilder builder)
    {
        var app = builder.Build();

        app.UseHttpsRedirection()
            .UseStaticFiles()
            .UseRouting()
            .UseAuthorization();
        app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

        return app;
    }

    private static IConfigurationBuilder GetConfigurationForAppsettings(WebApplicationBuilder builder)
    {
        return new ConfigurationBuilder()
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
    }

    private static ProjectConfiguration ReadConfigurationFromAppsettings(IConfigurationBuilder configBuilder)
    {
        var config = configBuilder.Build();
        ProjectConfiguration project = config.GetSection("Project").Get<ProjectConfiguration>()!;
        return project;
    }

    private static void CreateLogger(WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
    }
}
