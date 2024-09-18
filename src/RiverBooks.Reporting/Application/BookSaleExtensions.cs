using RiverBooks.Reporting.Contracts;
using RiverBooks.Reporting.Domain;

namespace RiverBooks.Reporting.Application;

internal static class BookSaleExtensions
{
    public static BookSalesResult ToResult(this BookSale bookSale)
    {
        return new BookSalesResult(bookSale.BookId, bookSale.Title, bookSale.Author, bookSale.UnitsSold,
            bookSale.TotalSales);
    }
}