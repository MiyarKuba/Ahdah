using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class IdempotencyRecord
{
    public Guid IdempotencyRecordId { get; set; }

    public Guid CompanyId { get; set; }

    public string IdempotencyKey { get; set; } = null!;

    public string OperationName { get; set; } = null!;

    public string RequestMethod { get; set; } = null!;

    public string RequestPath { get; set; } = null!;

    public string RequestSource { get; set; } = null!;

    public string RequestFingerprintHash { get; set; } = null!;

    public string? RequestPayloadHash { get; set; }

    public string ActorType { get; set; } = null!;

    public Guid? ActorUserId { get; set; }

    public string? ClientIdentifier { get; set; }

    public string Status { get; set; } = null!;

    public int AttemptCount { get; set; }

    public Guid? LockToken { get; set; }

    public string? LockedBy { get; set; }

    public DateTime? LockAcquiredAt { get; set; }

    public DateTime? LeaseExpiresAt { get; set; }

    public int? ResponseHttpStatus { get; set; }

    public string? ResponseContentType { get; set; }

    public string? ResponsePayload { get; set; }

    public string? ResourceType { get; set; }

    public Guid? ResourceId { get; set; }

    public int? ResourceVersionNumber { get; set; }

    public string? FailureCode { get; set; }

    public string? FailureMessage { get; set; }

    public bool? IsRetryable { get; set; }

    public int ReplayCount { get; set; }

    public DateTime? LastReplayedAt { get; set; }

    public Guid CorrelationId { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual Company Company { get; set; } = null!;
}
