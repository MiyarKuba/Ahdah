using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ProjectSettlement
{
    public Guid ProjectSettlementId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectId { get; set; }

    public int CycleNumber { get; set; }

    public Guid? PreviousSettlementId { get; set; }

    public string Status { get; set; } = null!;

    public string OperationalBasis { get; set; } = null!;

    public int CapturedProjectVersion { get; set; }

    public long CapturedFinancialRevision { get; set; }

    public Guid PolicyVersionId { get; set; }

    public string EvaluatorVersion { get; set; } = null!;

    public int SnapshotSchemaVersion { get; set; }

    public DateTime? EvaluatedAt { get; set; }

    public Guid PreparedByUserId { get; set; }

    public Guid? SubmittedByUserId { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public Guid? SnapshotId { get; set; }

    public byte[]? SnapshotHash { get; set; }

    public string? Notes { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<ProjectSettlement> InverseProjectSettlementNavigation { get; set; } = new List<ProjectSettlement>();

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<ProjectClosureCertificate> ProjectClosureCertificates { get; set; } = new List<ProjectClosureCertificate>();

    public virtual ICollection<ProjectFinancialControl> ProjectFinancialControls { get; set; } = new List<ProjectFinancialControl>();

    public virtual ICollection<ProjectReopening> ProjectReopenings { get; set; } = new List<ProjectReopening>();

    public virtual ICollection<ProjectSettlementEvent> ProjectSettlementEvents { get; set; } = new List<ProjectSettlementEvent>();

    public virtual ProjectSettlement? ProjectSettlementNavigation { get; set; }

    public virtual ProjectSettlementSnapshot? ProjectSettlementSnapshot { get; set; }

    public virtual ICollection<ProjectSettlementSnapshot> ProjectSettlementSnapshots { get; set; } = new List<ProjectSettlementSnapshot>();

    public virtual SettlementPolicyVersion SettlementPolicyVersion { get; set; } = null!;
}
