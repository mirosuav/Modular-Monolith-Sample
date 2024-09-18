using System.Globalization;
using RiverBooks.Reporting.Contracts;

namespace RiverBooks.Reporting.Application;

internal class SalesReportService(ISalesReportRepository bookSaleRepository) : ISalesReportService
{
    public async Task<TopBooksByMonthReport> GetTopBooksByMonthReportAsync(int month, int year,
        CancellationToken cancellationToken)
    {
        var results = await bookSaleRepository.GetBookSalesByYearAndMonthAsync(year, month, cancellationToken);

        var report = new TopBooksByMonthReport
        {
            Year = year,
            Month = month,
            MonthName = CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(month),
            Results = results.Select(b => b.ToResult()).ToList()
        };

        return report;
    }
}