using RiverBooks.SharedKernel.Events;

namespace RiverBooks.OrderProcessing.Domain;

internal record OrderCreated_SendEmailEvent(Guid OrderId, DateTimeOffset OccuredUtc) : EventBase(OccuredUtc);
