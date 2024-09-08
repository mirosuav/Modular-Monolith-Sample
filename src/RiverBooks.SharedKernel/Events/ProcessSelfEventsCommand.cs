using MediatR;

namespace RiverBooks.SharedKernel.Events;

/// <summary>
/// This command is being sent by EventsProcessing module to each individual module to trigger publishing all its outboxed events
/// </summary>
public record ProcessSelfEventsCommand(Guid Id, DateTime CreatedUtc) : INotification;
