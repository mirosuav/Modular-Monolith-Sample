using Asp.Versioning;
using RiverBooks.Web;
using Serilog;
using Serilog.Events;
using System.Reflection;

// BootstrapLogger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

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
        .AddEvents()
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

