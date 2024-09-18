using Microsoft.EntityFrameworkCore;
using RiverBooks.Reporting.Application;
using RiverBooks.Reporting.Domain;

namespace RiverBooks.Reporting.Infrastructure.Data;

internal class SalesReportRepository(ReportingDbContext dbContext) : ISalesReportRepository
{
    public void AddBookSale(BookSale sale)
    {
        dbContext.BookSales.Add(sale);
    }

    public Task<List<BookSale>> GetBookSalesByYearAndMonthAsync(int year, int month,
        CancellationToken cancellationToken)
    {
        return dbContext.BookSales
            .AsNoTracking()
            .Where(b => b.SoldAtUtc.Year == year && b.SoldAtUtc.Month == month)
            .ToListAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}