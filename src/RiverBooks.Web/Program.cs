using System.Reflection;
using Asp.Versioning;
using Serilog;

namespace RiverBooks.Web;

public class Program
{
    public static void Main(string[] args)
    {
        // BootstrapLogger
        Log.Logger = new LoggerConfiguration()
            .UseCommonSerilogConfiguration()
            .CreateLogger();

        try
        {
            Log.Logger.Information("Configuring web host");
            
            var builder = WebApplication.CreateBuilder(args);
            {
                builder.AddLogging();
                builder.AddModules(Log.Logger);
                builder.AddAuth();
                builder.AddApplicationServices();
                builder.AddMessaging();
                builder.AddApiVersioning(new ApiVersion(1, 0));
                builder.AddOpenApi();
                builder.MigrateDatabase(Log.Logger);
            }
            
            Log.Logger.Information("Starting web host");

            var app = builder.Build();
            {
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseExceptionHandler();
                app.MapVersionPrompt("/").AllowAnonymous();
                app.MapLogAppRedirect("/logs").AllowAnonymous();
                app.MapModulesEndpoints();
                app.UseSwaggerDevelopmentUI();
                app.Run();
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}