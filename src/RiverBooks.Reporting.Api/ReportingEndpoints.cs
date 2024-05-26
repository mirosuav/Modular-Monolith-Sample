using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using RiverBooks.Reporting.Contracts;

namespace RiverBooks.Reporting.Api;
internal static class ReportingEndpoints
{
    internal static IEndpointRouteBuilder MapReportingEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/topsales", GetTopSalesAsync)
            .Produces<Ok<TopSalesByMonthResponse>>()
            .AllowAnonymous();

        app.MapGet("/topsales2", GetTopSales2Async)
            .Produces<Ok<TopSalesByMonthResponse>>()
            .AllowAnonymous();

        return app;
    }

    internal static IResult GetTopSalesAsync(
        TopSalesByMonthRequest request,
        ITopSellingBooksReportService reportService,
        CancellationToken ct = default)
    {
        var report = reportService.ReachInSqlQuery(request.Month, request.Year);
        var response = new TopSalesByMonthResponse { Report = report };
        return TypedResults.Ok(response);
    }

    internal static async Task<IResult> GetTopSales2Async(TopSalesByMonthRequest request,
        ISalesReportService reportService,
        CancellationToken ct = default)
    {
        var report = await reportService.GetTopBooksByMonthReportAsync(request.Month, request.Year);
        var response = new TopSalesByMonthResponse { Report = report };
        return TypedResults.Ok(response);
    }
}
