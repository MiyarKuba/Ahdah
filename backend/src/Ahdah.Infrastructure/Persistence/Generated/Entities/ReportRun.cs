using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ReportRun
{
    public Guid ReportRunId { get; set; }

    public long RunSequence { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ReportDefinitionId { get; set; }

    public int ReportDefinitionVersionNumber { get; set; }

    public string ExecutionMode { get; set; } = null!;

    public Guid? ScheduledJobRunId { get; set; }

    public Guid? BackgroundJobId { get; set; }

    public string RequestedByType { get; set; } = null!;

    public Guid? RequestedByUserId { get; set; }

    public string IdempotencyKey { get; set; } = null!;

    public string RequestFingerprintHash { get; set; } = null!;

    public string Parameters { get; set; } = null!;

    public string ParameterHash { get; set; } = null!;

    public string LocaleCode { get; set; } = null!;

    public string TimeZone { get; set; } = null!;

    public string? CurrencyCode { get; set; }

    public string RequestedExportFormats { get; set; } = null!;

    public short Priority { get; set; }

    public int MaximumRowCountSnapshot { get; set; }

    public int ExecutionTimeoutSecondsSnapshot { get; set; }

    public int CacheTtlSecondsSnapshot { get; set; }

    public int GeneratedFileRetentionDaysSnapshot { get; set; }

    public bool ContainsSensitiveDataSnapshot { get; set; }

    public string? CacheKeyHash { get; set; }

    public bool IsCacheHit { get; set; }

    public Guid? CachedFromReportRunId { get; set; }

    public string Status { get; set; } = null!;

    public decimal ProgressPercentage { get; set; }

    public long ProcessedRowCount { get; set; }

    public int GeneratedFileCount { get; set; }

    public int WarningCount { get; set; }

    public DateTime? QueuedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? OutputExpiresAt { get; set; }

    public string? ResultSummary { get; set; }

    public string ExecutionMetrics { get; set; } = null!;

    public string? FailureCategory { get; set; }

    public string? FailureCode { get; set; }

    public string? FailureMessage { get; set; }

    public string? FailureDetails { get; set; }

    public string? CancelledByType { get; set; }

    public Guid? CancelledByUserId { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public Guid CorrelationId { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual BackgroundJob? BackgroundJob { get; set; }

    public virtual ICollection<ReportRun> InverseReportRunNavigation { get; set; } = new List<ReportRun>();

    public virtual ReportDefinitionVersion ReportDefinitionVersion { get; set; } = null!;

    public virtual ICollection<ReportDelivery> ReportDeliveries { get; set; } = new List<ReportDelivery>();

    public virtual ReportRunFile? ReportRunFile { get; set; }

    public virtual ReportRun? ReportRunNavigation { get; set; }

    public virtual ICollection<ReportSchedule> ReportSchedules { get; set; } = new List<ReportSchedule>();

    public virtual ICollection<SavedReportViewVersion> SavedReportViewVersions { get; set; } = new List<SavedReportViewVersion>();

    public virtual ICollection<SavedReportView> SavedReportViews { get; set; } = new List<SavedReportView>();

    public virtual ScheduledJobRun? ScheduledJobRun { get; set; }
}
