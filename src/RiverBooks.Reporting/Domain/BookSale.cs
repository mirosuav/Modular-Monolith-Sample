namespace RiverBooks.Reporting.Domain;

public class BookSale
{
   // public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public DateTime SoldAtUtc { get; set; }
    public int UnitsSold { get; set; }
    public decimal TotalSales { get; set; }
}

