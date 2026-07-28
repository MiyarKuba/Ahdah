using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ReportRecipientSuppressionEvent
{
    public Guid ReportRecipientSuppressionEventId { get; set; }

    public long EventSequence { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ReportRecipientSuppressionId { get; set; }

    public int EventNumber { get; set; }

    public int SuppressionVersionNumber { get; set; }

    public int? PreviousSuppressionVersionNumber { get; set; }

    public string EventType { get; set; } = null!;

    public string? PreviousStatus { get; set; }

    public string StatusSnapshot { get; set; } = null!;

    public string SuppressionChannelSnapshot { get; set; } = null!;

    public string RecipientTypeSnapshot { get; set; } = null!;

    public Guid? RecipientUserIdSnapshot { get; set; }

    public string? RecipientEmailSnapshot { get; set; }

    public string? RecipientDisplayNameSnapshot { get; set; }

    public string SuppressionScopeSnapshot { get; set; } = null!;

    public Guid? ReportDefinitionIdSnapshot { get; set; }

    public Guid? ReportScheduleIdSnapshot { get; set; }

    public string ReasonCategorySnapshot { get; set; } = null!;

    public string? ReasonCodeSnapshot { get; set; }

    public string ReasonMessageSnapshot { get; set; } = null!;

    public string SourceTypeSnapshot { get; set; } = null!;

    public Guid? SourceDeliveryEventIdSnapshot { get; set; }

    public DateTime EffectiveFromSnapshot { get; set; }

    public DateTime? ExpiresAtSnapshot { get; set; }

    public string? RevokedByTypeSnapshot { get; set; }

    public Guid? RevokedByUserIdSnapshot { get; set; }

    public DateTime? RevokedAtSnapshot { get; set; }

    public string? RevocationReasonSnapshot { get; set; }

    public DateTime? ExpiredAtSnapshot { get; set; }

    public string ChangedByType { get; set; } = null!;

    public Guid? ChangedByUserId { get; set; }

    public string ChangeSummary { get; set; } = null!;

    public string BeforeState { get; set; } = null!;

    public string AfterState { get; set; } = null!;

    public string EventMetadata { get; set; } = null!;

    public string IdempotencyKey { get; set; } = null!;

    public Guid CorrelationId { get; set; }

    public DateTime OccurredAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual ReportDefinition? ReportDefinition { get; set; }

    public virtual ReportDeliveryEvent? ReportDeliveryEvent { get; set; }

    public virtual ReportRecipientSuppression ReportRecipientSuppression { get; set; } = null!;

    public virtual ReportSchedule? ReportSchedule { get; set; }
}
