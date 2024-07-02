using RiverBooks.Reporting.Contracts;

namespace RiverBooks.Reporting.Application;

internal interface ISalesReportService
{
    Task<TopBooksByMonthReport> GetTopBooksByMonthReportAsync(int month, int year, CancellationToken cancellationToken);
}

