using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RiverBooks.Reporting.Application;
using RiverBooks.Reporting.Contracts;
using System.Globalization;

namespace RiverBooks.Reporting.Infrastructure;

internal class TopSellingBooksReportService(IConfiguration config,
  ILogger<TopSellingBooksReportService> logger) : ITopSellingBooksReportService
{
    private readonly string _connString = config.GetConnectionString("OrderProcessingConnectionString")!;
    private const string topBooksByMonthSql = @"
            SELECT b.Id as BookId, b.Title, b.Author, sum(oi.Quantity) AS Units, sum(oi.UnitPrice * oi.Quantity) AS Sales
            FROM Books.Books b 
	            INNER JOIN OrderProcessing.OrderItem oi ON b.Id = oi.BookId
	            INNER JOIN OrderProcessing.Orders o ON o.Id = oi.OrderId
            WHERE MONTH(o.DateCreated) = @month AND YEAR(o.DateCreated) = @year
            GROUP BY b.Id, b.Title, b.Author
            ORDER BY Sales DESC
            ";

    public TopBooksByMonthReport ReachInSqlQuery(int month, int year)
    {
        using var conn = new SqlConnection(_connString);

        logger.LogInformation("Executing query: {sql}", topBooksByMonthSql);

        var results = conn.Query<BookSalesResult>(topBooksByMonthSql, new { month, year }).ToList();

        var report = new TopBooksByMonthReport
        {
            Year = year,
            Month = month,
            MonthName = CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(month),
            Results = results
        };

        return report;
    }
}
