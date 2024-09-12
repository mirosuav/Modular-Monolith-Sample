using RiverBooks.SharedKernel.Events;

namespace RiverBooks.OrderProcessing.Domain;

internal record OrderCreated_PrepareReportEvent(Guid OrderId, DateTimeOffset OccurredUtc) : EventBase(OccurredUtc);
