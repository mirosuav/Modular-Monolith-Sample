using Microsoft.AspNetCore.Builder;
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
    public static IEndpointRouteBuilder MapModulesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapBookModuleEndpoints();
        app.MapUserModuleEndpoints();
        app.MapOrderProcessingModuleEndpoints();
        app.MapEmailSendingModuleEndpoints();
        app.MapReportingModuleEndpoints();
        return app;
    }

    public static RouteHandlerBuilder MapVersionPrompt(this IEndpointRouteBuilder builder, string pattern)
    {
        var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location);
        return builder.MapGet(pattern, () => $"RiverBooks API ver. {versionInfo.ProductVersion}");
    }
}

