using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ProjectSettlementSnapshot
{
    public Guid ProjectSettlementSnapshotId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid ProjectSettlementId { get; set; }

    public int SnapshotNumber { get; set; }

    public string Purpose { get; set; } = null!;

    public int ProjectVersion { get; set; }

    public long FinancialRevision { get; set; }

    public Guid PolicyVersionId { get; set; }

    public string EvaluatorVersion { get; set; } = null!;

    public int SchemaVersion { get; set; }

    public DateTime EvaluatedAt { get; set; }

    public byte[] CanonicalPayload { get; set; } = null!;

    public string Payload { get; set; } = null!;

    public string HashAlgorithm { get; set; } = null!;

    public byte[] SnapshotHash { get; set; } = null!;

    public Guid EvaluatedByUserId { get; set; }

    public int BlockerCount { get; set; }

    public int MandatoryGapCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<ProjectClosureCertificate> ProjectClosureCertificateProjectSettlementSnapshot1s { get; set; } = new List<ProjectClosureCertificate>();

    public virtual ICollection<ProjectClosureCertificate> ProjectClosureCertificateProjectSettlementSnapshot2s { get; set; } = new List<ProjectClosureCertificate>();

    public virtual ICollection<ProjectClosureCertificate> ProjectClosureCertificateProjectSettlementSnapshotNavigations { get; set; } = new List<ProjectClosureCertificate>();

    public virtual ICollection<ProjectClosureCertificate> ProjectClosureCertificateProjectSettlementSnapshots { get; set; } = new List<ProjectClosureCertificate>();

    public virtual ProjectSettlement ProjectSettlement { get; set; } = null!;

    public virtual ICollection<ProjectSettlementEvent> ProjectSettlementEvents { get; set; } = new List<ProjectSettlementEvent>();

    public virtual ICollection<ProjectSettlementSnapshotCategory> ProjectSettlementSnapshotCategories { get; set; } = new List<ProjectSettlementSnapshotCategory>();

    public virtual ICollection<ProjectSettlement> ProjectSettlements { get; set; } = new List<ProjectSettlement>();

    public virtual SettlementPolicyVersion SettlementPolicyVersion { get; set; } = null!;
}
