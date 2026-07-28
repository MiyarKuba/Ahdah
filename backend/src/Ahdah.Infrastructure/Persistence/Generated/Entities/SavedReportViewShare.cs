using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class SavedReportViewShare
{
    public Guid SavedReportViewShareId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid SavedReportViewId { get; set; }

    public string ShareTargetType { get; set; } = null!;

    public Guid? SharedWithUserId { get; set; }

    public string? SharedWithRoleCode { get; set; }

    public bool CanView { get; set; }

    public bool CanRun { get; set; }

    public bool CanExport { get; set; }

    public bool CanEdit { get; set; }

    public bool CanSchedule { get; set; }

    public bool CanReshare { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public string Status { get; set; } = null!;

    public Guid GrantedByUserId { get; set; }

    public string? GrantReason { get; set; }

    public Guid? RevokedByUserId { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? RevocationReason { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public string? Notes { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual SavedReportView SavedReportView { get; set; } = null!;

    public virtual ICollection<SavedReportViewShareEvent> SavedReportViewShareEvents { get; set; } = new List<SavedReportViewShareEvent>();
}
