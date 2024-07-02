using RiverBooks.Reporting.Contracts;
using RiverBooks.Reporting.Domain;

namespace RiverBooks.Reporting.Application;
internal interface ISalesReportRepository
{
    void AddBookSale(BookSale sale);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<List<BookSale>> GetBookSalesByYearAndMonthAsync(int year, int month, CancellationToken cancellationToken);
}

