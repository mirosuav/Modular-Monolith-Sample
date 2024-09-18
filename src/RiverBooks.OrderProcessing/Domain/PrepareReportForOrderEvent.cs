using RiverBooks.SharedKernel.Events;

namespace RiverBooks.OrderProcessing.Domain;

internal record PrepareReportForOrderEvent(Guid OrderId, DateTimeOffset OccurredUtc) : EventBase(OccurredUtc);