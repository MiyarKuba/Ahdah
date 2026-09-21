using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class SettlementPolicyVersion
{
    public Guid SettlementPolicyVersionId { get; set; }

    public Guid CompanyId { get; set; }

    public string PolicyCode { get; set; } = null!;

    public int MajorVersion { get; set; }

    public int ConfigurationVersion { get; set; }

    public string PolicyText { get; set; } = null!;

    public byte[] CanonicalRules { get; set; } = null!;

    public string Rules { get; set; } = null!;

    public byte[] ContentHash { get; set; } = null!;

    public Guid ApprovedByUserId { get; set; }

    public DateTime ApprovedAt { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public Guid? PredecessorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<CompanySettlementPolicyBinding> CompanySettlementPolicyBindings { get; set; } = new List<CompanySettlementPolicyBinding>();

    public virtual ICollection<DocumentCustodyEvent> DocumentCustodyEvents { get; set; } = new List<DocumentCustodyEvent>();

    public virtual ICollection<DocumentPolicyException> DocumentPolicyExceptions { get; set; } = new List<DocumentPolicyException>();

    public virtual ICollection<SettlementPolicyVersion> InverseSettlementPolicyVersionNavigation { get; set; } = new List<SettlementPolicyVersion>();

    public virtual ICollection<ProjectAuthorizationDecision> ProjectAuthorizationDecisions { get; set; } = new List<ProjectAuthorizationDecision>();

    public virtual ICollection<ProjectClosureCertificate> ProjectClosureCertificates { get; set; } = new List<ProjectClosureCertificate>();

    public virtual ICollection<ProjectOwnerReconciliation> ProjectOwnerReconciliations { get; set; } = new List<ProjectOwnerReconciliation>();

    public virtual ICollection<ProjectSettlementSnapshot> ProjectSettlementSnapshots { get; set; } = new List<ProjectSettlementSnapshot>();

    public virtual ICollection<ProjectSettlement> ProjectSettlements { get; set; } = new List<ProjectSettlement>();

    public virtual SettlementPolicyVersion? SettlementPolicyVersionNavigation { get; set; }
}
