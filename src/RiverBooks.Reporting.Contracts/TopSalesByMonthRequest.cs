using Microsoft.AspNetCore.Mvc;

namespace RiverBooks.Reporting.Contracts;

public class TopSalesByMonthRequest
{
    [FromQuery]
    public int Month { get; set; }
    [FromQuery]
    public int Year { get; set; }
}



