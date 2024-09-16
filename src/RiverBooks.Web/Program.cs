using Asp.Versioning;
using Serilog;
using Serilog.Events;
using System.Reflection;

namespace RiverBooks.Web;

public class Program
{
    public static void Main(string[] args)
    {

        // BootstrapLogger
        Log.Logger = new LoggerConfiguration()
            .UseCommonSerilogConfiguration()
            .CreateBootstrapLogger();

        try
        {

            Log.Logger.Information("Starting web host");

            // Collect modules assemblies
            List<Assembly> moduleAssemblies = [typeof(IMarker).Assembly];

            var builder = WebApplication.CreateBuilder(args);
            {
                builder.Services
                    .AddModules(builder.Configuration, Log.Logger, moduleAssemblies)
                    .AddLogging(builder.Configuration, moduleAssemblies)
                    .AddAuth(builder.Configuration, builder.Environment)
                    .AddApplicationServices()
                    .AddMessaging(moduleAssemblies)
                    .AddApiVersioning(new ApiVersion(1, 0))
                    .AddOpenApi();
            }

            var app = builder.Build();
            {
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapVersionPrompt("/").AllowAnonymous();
                app.MapModulesEndpoints();
                app.UseSwaggerDevelopmentUI();
                app.Run();
            }

        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application startup failed");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}

