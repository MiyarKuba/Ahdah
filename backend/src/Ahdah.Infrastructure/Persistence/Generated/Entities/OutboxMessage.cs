using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class OutboxMessage
{
    public Guid OutboxMessageId { get; set; }

    public long MessageSequence { get; set; }

    public Guid CompanyId { get; set; }

    public string MessageType { get; set; } = null!;

    public int MessageVersion { get; set; }

    public string EventCategory { get; set; } = null!;

    public string? AggregateType { get; set; }

    public Guid? AggregateId { get; set; }

    public int? AggregateVersionNumber { get; set; }

    public string DestinationType { get; set; } = null!;

    public string DestinationName { get; set; } = null!;

    public string? RoutingKey { get; set; }

    public string Payload { get; set; } = null!;

    public string Headers { get; set; } = null!;

    public Guid CorrelationId { get; set; }

    public Guid? CausationId { get; set; }

    public string IdempotencyKey { get; set; } = null!;

    public string ProducerType { get; set; } = null!;

    public Guid? ProducerUserId { get; set; }

    public short Priority { get; set; }

    public string Status { get; set; } = null!;

    public DateTime AvailableAt { get; set; }

    public DateTime? ProcessingStartedAt { get; set; }

    public Guid? LockToken { get; set; }

    public string? LockedBy { get; set; }

    public DateTime? LeaseExpiresAt { get; set; }

    public int AttemptCount { get; set; }

    public int MaxAttempts { get; set; }

    public DateTime? LastAttemptAt { get; set; }

    public DateTime? LastFailedAt { get; set; }

    public DateTime? NextRetryAt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime? DeadLetteredAt { get; set; }

    public string? ProviderMessageId { get; set; }

    public string? LastErrorCode { get; set; }

    public string? LastErrorMessage { get; set; }

    public string? CancelledByType { get; set; }

    public Guid? CancelledByUserId { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public DateTime OccurredAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<OutboxMessageAttempt> OutboxMessageAttempts { get; set; } = new List<OutboxMessageAttempt>();
}
