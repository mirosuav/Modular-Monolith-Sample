using RiverBooks.SharedKernel.Events;

namespace RiverBooks.OrderProcessing.Domain;

internal record SendOrderConfirmationEmailEvent(Guid OrderId, DateTimeOffset OccurredUtc) : EventBase(OccurredUtc);