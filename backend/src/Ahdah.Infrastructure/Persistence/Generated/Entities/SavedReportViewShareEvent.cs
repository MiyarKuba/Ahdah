using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class SavedReportViewShareEvent
{
    public Guid SavedReportViewShareEventId { get; set; }

    public long EventSequence { get; set; }

    public Guid CompanyId { get; set; }

    public Guid SavedReportViewShareId { get; set; }

    public Guid SavedReportViewIdSnapshot { get; set; }

    public int ShareVersionNumber { get; set; }

    public int? PreviousShareVersionNumber { get; set; }

    public string EventType { get; set; } = null!;

    public string? PreviousStatus { get; set; }

    public string StatusSnapshot { get; set; } = null!;

    public string ShareTargetTypeSnapshot { get; set; } = null!;

    public Guid? SharedWithUserIdSnapshot { get; set; }

    public string? SharedWithRoleCodeSnapshot { get; set; }

    public bool CanViewSnapshot { get; set; }

    public bool CanRunSnapshot { get; set; }

    public bool CanExportSnapshot { get; set; }

    public bool CanEditSnapshot { get; set; }

    public bool CanScheduleSnapshot { get; set; }

    public bool CanReshareSnapshot { get; set; }

    public DateTime EffectiveFromSnapshot { get; set; }

    public DateTime? ExpiresAtSnapshot { get; set; }

    public Guid GrantedByUserIdSnapshot { get; set; }

    public string? GrantReasonSnapshot { get; set; }

    public Guid? RevokedByUserIdSnapshot { get; set; }

    public DateTime? RevokedAtSnapshot { get; set; }

    public string? RevocationReasonSnapshot { get; set; }

    public DateTime? ExpiredAtSnapshot { get; set; }

    public string? NotesSnapshot { get; set; }

    public string ChangedByType { get; set; } = null!;

    public Guid? ChangedByUserId { get; set; }

    public string ChangeSummary { get; set; } = null!;

    public string ChangedFields { get; set; } = null!;

    public string BeforeState { get; set; } = null!;

    public string AfterState { get; set; } = null!;

    public string? PreviousSnapshotHash { get; set; }

    public string SnapshotHash { get; set; } = null!;

    public Guid? RequestId { get; set; }

    public string IdempotencyKey { get; set; } = null!;

    public Guid CorrelationId { get; set; }

    public DateTime OccurredAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser? AppUser2 { get; set; }

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual SavedReportView SavedReportView { get; set; } = null!;

    public virtual SavedReportViewShare SavedReportViewShare { get; set; } = null!;
}
