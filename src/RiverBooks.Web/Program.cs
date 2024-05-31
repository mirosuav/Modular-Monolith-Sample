using Asp.Versioning;
using RiverBooks.Web;
using Serilog;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Logger.Information("Starting web host");

// Collect modules assemblies
List<Assembly> moduleAssemblies = [typeof(Marker).Assembly];

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .AddModules(builder.Configuration, Log.Logger, moduleAssemblies)
        .AddLogging(builder.Configuration, moduleAssemblies)
        .AddAuth(builder.Configuration)
        .AddApplicationServices()
        .AddMessaging(moduleAssemblies)
        .AddEvents()
        .AddApiVersioning(new ApiVersion(1, 0));
}

var app = builder.Build();
{
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapModulesEndpoints();
    app.Run();
}

