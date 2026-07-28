using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ReportDelivery
{
    public Guid ReportDeliveryId { get; set; }

    public long DeliverySequence { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ReportRunId { get; set; }

    public Guid ReportScheduleId { get; set; }

    public Guid? ReportScheduleRecipientId { get; set; }

    public string RecipientSourceType { get; set; } = null!;

    public string RecipientType { get; set; } = null!;

    public Guid? RecipientUserId { get; set; }

    public string? ExternalEmail { get; set; }

    public string? RecipientDisplayName { get; set; }

    public string DeliveryChannel { get; set; } = null!;

    public string DeliveryEvent { get; set; } = null!;

    public string ContentMode { get; set; } = null!;

    public string SubjectSnapshot { get; set; } = null!;

    public string MessageSnapshot { get; set; } = null!;

    public string TemplateData { get; set; } = null!;

    public string LocaleCode { get; set; } = null!;

    public string TimeZone { get; set; } = null!;

    public bool ContainsSensitiveData { get; set; }

    public bool RequiresAuthorization { get; set; }

    public int DownloadLinkCount { get; set; }

    public int AttachmentCount { get; set; }

    public string IdempotencyKey { get; set; } = null!;

    public Guid? BackgroundJobId { get; set; }

    public string Status { get; set; } = null!;

    public short Priority { get; set; }

    public int AttemptCount { get; set; }

    public int MaxAttempts { get; set; }

    public DateTime AvailableAt { get; set; }

    public DateTime? QueuedAt { get; set; }

    public DateTime? LastAttemptStartedAt { get; set; }

    public DateTime? LastAttemptCompletedAt { get; set; }

    public DateTime? NextRetryAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? ProviderName { get; set; }

    public string? ProviderMessageId { get; set; }

    public string ProviderResponseMetadata { get; set; } = null!;

    public string? FailureCategory { get; set; }

    public string? FailureCode { get; set; }

    public string? FailureMessage { get; set; }

    public string? FailureDetails { get; set; }

    public string? SkipCategory { get; set; }

    public string? SkipReason { get; set; }

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

    public virtual ICollection<ReportDeliveryAttempt> ReportDeliveryAttempts { get; set; } = new List<ReportDeliveryAttempt>();

    public virtual ICollection<ReportDeliveryFile> ReportDeliveryFiles { get; set; } = new List<ReportDeliveryFile>();

    public virtual ReportRun ReportRun { get; set; } = null!;

    public virtual ReportSchedule ReportSchedule { get; set; } = null!;

    public virtual ReportScheduleRecipient? ReportScheduleRecipient { get; set; }
}
