using RiverBooks.Reporting.Contracts;

namespace RiverBooks.Reporting.Application;

internal interface ITopSellingBooksReportService
{
    TopBooksByMonthReport ReachInSqlQuery(int month, int year);
}
