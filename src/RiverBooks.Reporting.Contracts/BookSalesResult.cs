﻿namespace RiverBooks.Reporting.Contracts;

public record BookSalesResult(
    Guid BookId,
    string Title,
    string Author,
    int Units,
    decimal Sales);