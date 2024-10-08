﻿namespace RiverBooks.SharedKernel.Extensions;

public static class CommonExtensions
{
    public static DateTime GetUtcDateTime(this TimeProvider timeProvider)
    {
        return timeProvider.GetUtcNow().UtcDateTime;
    }
}