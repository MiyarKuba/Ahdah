using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class BackgroundJob
{
    public Guid BackgroundJobId { get; set; }

    public long JobSequence { get; set; }

    public Guid CompanyId { get; set; }

    public string JobType { get; set; } = null!;

    public string JobName { get; set; } = null!;

    public string QueueName { get; set; } = null!;

    public string Payload { get; set; } = null!;

    public string Headers { get; set; } = null!;

    public short Priority { get; set; }

    public int TimeoutSeconds { get; set; }

    public string IdempotencyKey { get; set; } = null!;

    public string? ResourceType { get; set; }

    public Guid? ResourceId { get; set; }

    public int? ResourceVersionNumber { get; set; }

    public string TriggeredByType { get; set; } = null!;

    public Guid? TriggeredByUserId { get; set; }

    public Guid CorrelationId { get; set; }

    public Guid? CausationId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime AvailableAt { get; set; }

    public int AttemptCount { get; set; }

    public int MaxAttempts { get; set; }

    public DateTime? ProcessingStartedAt { get; set; }

    public Guid? LockToken { get; set; }

    public string? LockedBy { get; set; }

    public DateTime? LeaseExpiresAt { get; set; }

    public DateTime? LastAttemptAt { get; set; }

    public DateTime? LastFailedAt { get; set; }

    public DateTime? NextRetryAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? DeadLetteredAt { get; set; }

    public string? ResultPayload { get; set; }

    public string? FailureCategory { get; set; }

    public string? FailureCode { get; set; }

    public string? FailureMessage { get; set; }

    public string? CancelledByType { get; set; }

    public Guid? CancelledByUserId { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual ICollection<BackgroundJobAttempt> BackgroundJobAttempts { get; set; } = new List<BackgroundJobAttempt>();

    public virtual Company Company { get; set; } = null!;

    public virtual ReportDelivery? ReportDelivery { get; set; }

    public virtual ReportRun? ReportRun { get; set; }

    public virtual ICollection<ScheduledJobDefinition> ScheduledJobDefinitions { get; set; } = new List<ScheduledJobDefinition>();

    public virtual ScheduledJobRun? ScheduledJobRunBackgroundJob { get; set; }

    public virtual ICollection<ScheduledJobRun> ScheduledJobRunBackgroundJobNavigations { get; set; } = new List<ScheduledJobRun>();
}
