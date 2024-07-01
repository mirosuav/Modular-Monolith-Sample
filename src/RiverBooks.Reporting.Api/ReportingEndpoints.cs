using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

        app.MapGet("/topsales2/{year:int}/{month:int}", GetTopSales2Async)
            .Produces<Ok<TopBooksByMonthReport>>()
            .AllowAnonymous();

        return app;
    }

    internal static Task<IResult> GetTopSalesAsync(
        int year,
        int month,
        ITopSellingBooksReportService reportService,
        CancellationToken cancellationToken)
    {
        var report = reportService.ReachInSqlQuery(month, year);
        return Task.FromResult<IResult>(TypedResults.Ok(report));
    }

    internal static async Task<IResult> GetTopSales2Async(
        int year,
        int month,
        ISalesReportService reportService,
        CancellationToken cancellationToken)
    {
        var report = await reportService.GetTopBooksByMonthReportAsync(month, year);
        return TypedResults.Ok(report);
    }
}
