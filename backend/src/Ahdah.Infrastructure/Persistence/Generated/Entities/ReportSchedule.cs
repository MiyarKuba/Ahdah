using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ReportSchedule
{
    public Guid ReportScheduleId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ScheduledJobDefinitionId { get; set; }

    public Guid ReportDefinitionId { get; set; }

    public string ScheduleCode { get; set; } = null!;

    public string ScheduleName { get; set; } = null!;

    public string? Description { get; set; }

    public string ReportVersionPolicy { get; set; } = null!;

    public int? PinnedReportVersionNumber { get; set; }

    public string ParameterEvaluationMode { get; set; } = null!;

    public string Parameters { get; set; } = null!;

    public string RelativeParameterRules { get; set; } = null!;

    public string ParameterHash { get; set; } = null!;

    public string RequestedExportFormats { get; set; } = null!;

    public string FileNameTemplate { get; set; } = null!;

    public string LocaleCode { get; set; } = null!;

    public string TimeZone { get; set; } = null!;

    public string? CurrencyCode { get; set; }

    public string EmptyResultPolicy { get; set; } = null!;

    public string DeliveryMode { get; set; } = null!;

    public string SuccessNotificationPolicy { get; set; } = null!;

    public string FailureNotificationPolicy { get; set; } = null!;

    public bool AttachFilesToEmail { get; set; }

    public long MaximumEmailAttachmentBytes { get; set; }

    public int GeneratedFileRetentionDays { get; set; }

    public Guid ExecutionOwnerUserId { get; set; }

    public string PermissionEvaluationPolicy { get; set; } = null!;

    public int MaxConsecutiveFailures { get; set; }

    public int ConsecutiveFailureCount { get; set; }

    public bool AutoPauseOnFailureLimit { get; set; }

    public string IdempotencyKeyPrefix { get; set; } = null!;

    public Guid? LastReportRunId { get; set; }

    public DateTime? LastRunAt { get; set; }

    public string? LastRunStatus { get; set; }

    public DateTime? LastSuccessAt { get; set; }

    public DateTime? LastFailureAt { get; set; }

    public DateTime? NextExpectedRunAt { get; set; }

    public string Status { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }

    public Guid UpdatedByUserId { get; set; }

    public Guid? ActivatedByUserId { get; set; }

    public DateTime? ActivatedAt { get; set; }

    public string? PausedByType { get; set; }

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

    public virtual AppUser AppUser2 { get; set; } = null!;

    public virtual AppUser? AppUser3 { get; set; }

    public virtual AppUser? AppUser4 { get; set; }

    public virtual AppUser AppUser5 { get; set; } = null!;

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual ReportDefinition ReportDefinition { get; set; } = null!;

    public virtual ReportDefinitionVersion? ReportDefinitionVersion { get; set; }

    public virtual ICollection<ReportDelivery> ReportDeliveries { get; set; } = new List<ReportDelivery>();

    public virtual ICollection<ReportRecipientSuppressionEvent> ReportRecipientSuppressionEvents { get; set; } = new List<ReportRecipientSuppressionEvent>();

    public virtual ICollection<ReportRecipientSuppression> ReportRecipientSuppressions { get; set; } = new List<ReportRecipientSuppression>();

    public virtual ReportRun? ReportRun { get; set; }

    public virtual ICollection<ReportScheduleRecipient> ReportScheduleRecipients { get; set; } = new List<ReportScheduleRecipient>();

    public virtual ScheduledJobDefinition ScheduledJobDefinition { get; set; } = null!;
}
