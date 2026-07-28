using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ScheduledJobRun
{
    public Guid ScheduledJobRunId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ScheduledJobDefinitionId { get; set; }

    public int DefinitionVersionNumber { get; set; }

    public int RunNumber { get; set; }

    public DateTime ScheduledFor { get; set; }

    public DateTime DetectedAt { get; set; }

    public DateTime DecisionAt { get; set; }

    public long DelayMilliseconds { get; set; }

    public string RunDecision { get; set; } = null!;

    public Guid? BackgroundJobId { get; set; }

    public Guid? ReplacedBackgroundJobId { get; set; }

    public DateTime? EnqueuedAt { get; set; }

    public string SchedulerInstance { get; set; } = null!;

    public string IdempotencyKey { get; set; } = null!;

    public string JobTypeSnapshot { get; set; } = null!;

    public string QueueNameSnapshot { get; set; } = null!;

    public short PrioritySnapshot { get; set; }

    public string ConcurrencyPolicySnapshot { get; set; } = null!;

    public string MisfirePolicySnapshot { get; set; } = null!;

    public string? DecisionReason { get; set; }

    public Guid CorrelationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual BackgroundJob? BackgroundJob { get; set; }

    public virtual BackgroundJob? BackgroundJobNavigation { get; set; }

    public virtual ReportRun? ReportRun { get; set; }

    public virtual ScheduledJobDefinition ScheduledJobDefinition { get; set; } = null!;
}
