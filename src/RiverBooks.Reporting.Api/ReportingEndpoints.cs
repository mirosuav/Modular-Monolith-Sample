using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using RiverBooks.Reporting.Application;
using RiverBooks.Reporting.Contracts;

namespace RiverBooks.Reporting.Api;

internal static class ReportingEndpoints
{
    internal static IEndpointRouteBuilder MapReportingEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/topsales/{year:int}/{month:int}", GetTopSalesAsync)
            .Produces<Ok<TopBooksByMonthReport>>()
            .AllowAnonymous();

        return app;
    }

    internal static async Task<IResult> GetTopSalesAsync(
        int year,
        int month,
        ISalesReportService reportService,
        CancellationToken cancellationToken)
    {
        var report = await reportService.GetTopBooksByMonthReportAsync(month, year, cancellationToken);
        return TypedResults.Ok(report);
    }
}