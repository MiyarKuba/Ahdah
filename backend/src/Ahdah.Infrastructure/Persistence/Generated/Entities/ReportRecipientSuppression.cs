using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ReportRecipientSuppression
{
    public Guid ReportRecipientSuppressionId { get; set; }

    public Guid CompanyId { get; set; }

    public string SuppressionChannel { get; set; } = null!;

    public string RecipientType { get; set; } = null!;

    public Guid? RecipientUserId { get; set; }

    public string? RecipientEmail { get; set; }

    public string? RecipientDisplayName { get; set; }

    public string SuppressionScope { get; set; } = null!;

    public Guid? ReportDefinitionId { get; set; }

    public Guid? ReportScheduleId { get; set; }

    public string ReasonCategory { get; set; } = null!;

    public string? ReasonCode { get; set; }

    public string ReasonMessage { get; set; } = null!;

    public string SourceType { get; set; } = null!;

    public Guid? SourceDeliveryEventId { get; set; }

    public string CreatedByType { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public string Status { get; set; } = null!;

    public string? RevokedByType { get; set; }

    public Guid? RevokedByUserId { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? RevocationReason { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public string Metadata { get; set; } = null!;

    public Guid CorrelationId { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ReportDefinition? ReportDefinition { get; set; }

    public virtual ReportDeliveryEvent? ReportDeliveryEvent { get; set; }

    public virtual ICollection<ReportRecipientSuppressionEvent> ReportRecipientSuppressionEvents { get; set; } = new List<ReportRecipientSuppressionEvent>();

    public virtual ReportSchedule? ReportSchedule { get; set; }
}
