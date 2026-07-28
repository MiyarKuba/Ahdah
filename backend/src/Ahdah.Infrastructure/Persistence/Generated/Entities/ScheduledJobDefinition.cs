using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ScheduledJobDefinition
{
    public Guid ScheduledJobDefinitionId { get; set; }

    public Guid CompanyId { get; set; }

    public string JobCode { get; set; } = null!;

    public string JobName { get; set; } = null!;

    public string JobType { get; set; } = null!;

    public string? Description { get; set; }

    public string QueueName { get; set; } = null!;

    public string ScheduleType { get; set; } = null!;

    public string? CronExpression { get; set; }

    public int? IntervalSeconds { get; set; }

    public TimeOnly? RunTime { get; set; }

    public short? DayOfWeek { get; set; }

    public short? DayOfMonth { get; set; }

    public DateTime? OneTimeRunAt { get; set; }

    public string TimeZone { get; set; } = null!;

    public DateTime ScheduleStartAt { get; set; }

    public DateTime? ScheduleEndAt { get; set; }

    public int? MaxRunCount { get; set; }

    public int GeneratedRunCount { get; set; }

    public DateTime? NextRunAt { get; set; }

    public string Payload { get; set; } = null!;

    public string Headers { get; set; } = null!;

    public short Priority { get; set; }

    public int TimeoutSeconds { get; set; }

    public int JobMaxAttempts { get; set; }

    public string RetryStrategy { get; set; } = null!;

    public int InitialRetryDelaySeconds { get; set; }

    public int MaximumRetryDelaySeconds { get; set; }

    public decimal RetryBackoffMultiplier { get; set; }

    public bool RetryJitterEnabled { get; set; }

    public string ConcurrencyPolicy { get; set; } = null!;

    public int MaxConcurrentRuns { get; set; }

    public string MisfirePolicy { get; set; } = null!;

    public string IdempotencyKeyPrefix { get; set; } = null!;

    public DateTime? LastScheduledFor { get; set; }

    public DateTime? LastEnqueuedAt { get; set; }

    public Guid? LastBackgroundJobId { get; set; }

    public string? LastJobStatus { get; set; }

    public string Status { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }

    public Guid UpdatedByUserId { get; set; }

    public Guid? ActivatedByUserId { get; set; }

    public DateTime? ActivatedAt { get; set; }

    public Guid? PausedByUserId { get; set; }

    public DateTime? PausedAt { get; set; }

    public string? PauseReason { get; set; }

    public Guid? DisabledByUserId { get; set; }

    public DateTime? DisabledAt { get; set; }

    public string? DisabledReason { get; set; }

    public Guid? RetiredByUserId { get; set; }

    public DateTime? RetiredAt { get; set; }

    public string? RetirementReason { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser? AppUser2 { get; set; }

    public virtual AppUser? AppUser3 { get; set; }

    public virtual AppUser AppUser4 { get; set; } = null!;

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual BackgroundJob? BackgroundJob { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ReportSchedule? ReportSchedule { get; set; }

    public virtual ICollection<ScheduledJobRun> ScheduledJobRuns { get; set; } = new List<ScheduledJobRun>();
}
