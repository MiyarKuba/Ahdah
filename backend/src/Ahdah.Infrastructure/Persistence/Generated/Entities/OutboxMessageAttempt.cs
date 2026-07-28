using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class OutboxMessageAttempt
{
    public Guid OutboxMessageAttemptId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid OutboxMessageId { get; set; }

    public int AttemptNumber { get; set; }

    public string WorkerName { get; set; } = null!;

    public Guid LockToken { get; set; }

    public string AttemptStatus { get; set; } = null!;

    public DateTime StartedAt { get; set; }

    public DateTime CompletedAt { get; set; }

    public long DurationMilliseconds { get; set; }

    public string? ProviderName { get; set; }

    public string? ProviderMessageId { get; set; }

    public string? ResponseCode { get; set; }

    public string? FailureCategory { get; set; }

    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }

    public string RetryDecision { get; set; } = null!;

    public DateTime? NextRetryAt { get; set; }

    public string RequestMetadata { get; set; } = null!;

    public string ResponseMetadata { get; set; } = null!;

    public Guid CorrelationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual OutboxMessage OutboxMessage { get; set; } = null!;
}
