using Microsoft.AspNetCore.Builder;
using RiverBooks.Books.Api;
using RiverBooks.EmailSending.Api;
using RiverBooks.OrderProcessing.Api;
using RiverBooks.Reporting.Api;
using RiverBooks.Users.Api;

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
}

