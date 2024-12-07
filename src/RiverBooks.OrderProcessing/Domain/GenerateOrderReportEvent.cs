using RiverBooks.SharedKernel.Events;

namespace RiverBooks.OrderProcessing.Domain;

internal record GenerateOrderReportEvent(Guid OrderId, DateTimeOffset OccurredUtc) : EventBase(OccurredUtc);