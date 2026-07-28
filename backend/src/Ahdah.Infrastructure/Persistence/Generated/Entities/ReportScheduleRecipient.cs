using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ReportScheduleRecipient
{
    public Guid ReportScheduleRecipientId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ReportScheduleId { get; set; }

    public string RecipientType { get; set; } = null!;

    public Guid? RecipientUserId { get; set; }

    public string? RecipientRoleCode { get; set; }

    public string? ExternalEmail { get; set; }

    public string? ExternalDisplayName { get; set; }

    public int RecipientOrder { get; set; }

    public string DeliveryChannel { get; set; } = null!;

    public bool NotifyOnSuccess { get; set; }

    public bool NotifyOnPartialSuccess { get; set; }

    public bool NotifyOnEmptyResult { get; set; }

    public bool NotifyOnFailure { get; set; }

    public bool NotifyOnCancelled { get; set; }

    public bool IncludeDownloadLink { get; set; }

    public bool IncludeEmailAttachment { get; set; }

    public string? EmailSubjectTemplate { get; set; }

    public string? EmailMessageTemplate { get; set; }

    public string? LocaleCode { get; set; }

    public string? TimeZone { get; set; }

    public string DeliveryFailurePolicy { get; set; } = null!;

    public int MaxDeliveryAttempts { get; set; }

    public Guid? ExternalEmailVerifiedByUserId { get; set; }

    public DateTime? ExternalEmailVerifiedAt { get; set; }

    public string? Notes { get; set; }

    public string Status { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }

    public Guid UpdatedByUserId { get; set; }

    public Guid? DisabledByUserId { get; set; }

    public DateTime? DisabledAt { get; set; }

    public string? DisableReason { get; set; }

    public Guid? RemovedByUserId { get; set; }

    public DateTime? RemovedAt { get; set; }

    public string? RemovalReason { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser? AppUser2 { get; set; }

    public virtual AppUser? AppUser3 { get; set; }

    public virtual AppUser AppUser4 { get; set; } = null!;

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual ICollection<ReportDelivery> ReportDeliveries { get; set; } = new List<ReportDelivery>();

    public virtual ReportSchedule ReportSchedule { get; set; } = null!;
}
