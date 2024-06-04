using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RiverBooks.Reporting.Contracts;
using System.Globalization;

namespace RiverBooks.Reporting;

internal class DefaultSalesReportService(
    IConfiguration config,
    ILogger<DefaultSalesReportService> logger) : ISalesReportService
{
    private readonly ILogger<DefaultSalesReportService> _logger = logger;
    private readonly string _connString = config.GetConnectionString("ReportingConnectionString")!;
    private const string getTopBooksByMonthReportSql = @"
            SELECT BookId, Title, Author, UnitsSold AS Units, TotalSales AS Sales
            FROM Reporting.MonthlyBookSales
            WHERE Month = @month AND Year = @year
            ORDER BY TotalSales DESC
            ";

    public async Task<TopBooksByMonthReport> GetTopBooksByMonthReportAsync(int month, int year)
    {
        using var conn = new SqlConnection(_connString);
        _logger.LogInformation("Executing query: {sql}", getTopBooksByMonthReportSql);
        var results = (await conn.QueryAsync<BookSalesResult>(getTopBooksByMonthReportSql, new { month, year }))
          .ToList();

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

