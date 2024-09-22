using RiverBooks.Books.Api;
using RiverBooks.EmailSending.Api;
using RiverBooks.OrderProcessing.Api;
using RiverBooks.Reporting.Api;
using RiverBooks.Users.Api;
using System.Diagnostics;
using System.Reflection;

namespace RiverBooks.Web;

public static class BuilderExtensions
{
    public static IEndpointRouteBuilder MigrateDatabase(this IEndpointRouteBuilder app)
    {
        using var scope = app.ServiceProvider.CreateScope();
        Books.Api.ModuleBootstrap.MigrateDatabase(scope.ServiceProvider);
        Users.Api.ModuleBootstrap.MigrateDatabase(scope.ServiceProvider);
        Reporting.Api.ModuleBootstrap.MigrateDatabase(scope.ServiceProvider);
        EmailSending.Api.ModuleBootstrap.MigrateDatabase(scope.ServiceProvider);
        OrderProcessing.Api.ModuleBootstrap.MigrateDatabase(scope.ServiceProvider);
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
        var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location);
        return builder.MapGet(pattern, () => $"RiverBooks API ver. {versionInfo.ProductVersion}");
    }
}