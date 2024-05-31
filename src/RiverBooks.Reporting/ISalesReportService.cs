using RiverBooks.Reporting.Contracts;

namespace RiverBooks.Reporting;

internal interface ISalesReportService
{
    Task<TopBooksByMonthReport> GetTopBooksByMonthReportAsync(int month, int year);
}

