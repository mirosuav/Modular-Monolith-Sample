using RiverBooks.Books.Api;
using RiverBooks.EmailSending.Api;
using RiverBooks.OrderProcessing.Api;
using RiverBooks.Reporting.Api;
using RiverBooks.Users.Api;
using System.Diagnostics;
using System.Reflection;

namespace RiverBooks.Web;

public static class ApplicationBuilderExtensions
{
    public static IEndpointRouteBuilder MigrateDatabase(this IEndpointRouteBuilder app)
    {
        using var scope = app.ServiceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationBuilder>>();
        Books.Api.ModuleBootstrap.MigrateDatabase(scope.ServiceProvider, logger);
        Users.Api.ModuleBootstrap.MigrateDatabase(scope.ServiceProvider, logger);
        Reporting.Api.ModuleBootstrap.MigrateDatabase(scope.ServiceProvider, logger);
        EmailSending.Api.ModuleBootstrap.MigrateDatabase(scope.ServiceProvider, logger);
        OrderProcessing.Api.ModuleBootstrap.MigrateDatabase(scope.ServiceProvider, logger);
        logger.LogInformation("Database up to date.");
        return app;
    }

    public static IEndpointRouteBuilder MapModulesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapBookModuleEndpoints();
        app.MapUserModuleEndpoints();
        app.MapOrderProcessingModuleEndpoints();
        app.MapEmailSendingModuleEndpoints();
        app.MapReportingModuleEndpoints();
        return app;
    }

    public static WebApplication UseSwaggerDevelopmentUI(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }

    public static RouteHandlerBuilder MapVersionPrompt(this IEndpointRouteBuilder builder, string pattern)
    {
        return builder.MapGet(pattern, static () =>
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location);
            return $"RiverBooks API ver. {versionInfo.ProductVersion}";
        });
    }

    public static RouteHandlerBuilder MapLogAppRedirect(this IEndpointRouteBuilder builder, string pattern)
    {
        return builder.MapGet(pattern, static (IConfiguration config) =>
        {
            var logAppUrl = config.GetValue<string>("LogMonitoringAppUrl");
            if (logAppUrl is not null)
                return Results.Redirect(logAppUrl);
            return Results.NotFound();
        });
    }
}