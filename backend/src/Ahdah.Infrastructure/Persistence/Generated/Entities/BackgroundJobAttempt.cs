using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class BackgroundJobAttempt
{
    public Guid BackgroundJobAttemptId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid BackgroundJobId { get; set; }

    public int AttemptNumber { get; set; }

    public string WorkerName { get; set; } = null!;

    public Guid LockToken { get; set; }

    public string AttemptStatus { get; set; } = null!;

    public DateTime StartedAt { get; set; }

    public DateTime CompletedAt { get; set; }

    public long DurationMilliseconds { get; set; }

    public string? ResultPayload { get; set; }

    public string? FailureCategory { get; set; }

    public string? FailureCode { get; set; }

    public string? FailureMessage { get; set; }

    public string RetryDecision { get; set; } = null!;

    public DateTime? NextRetryAt { get; set; }

    public string ExecutionMetadata { get; set; } = null!;

    public Guid CorrelationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual BackgroundJob BackgroundJob { get; set; } = null!;

    public virtual ICollection<ReportDeliveryAttempt> ReportDeliveryAttempts { get; set; } = new List<ReportDeliveryAttempt>();
}
