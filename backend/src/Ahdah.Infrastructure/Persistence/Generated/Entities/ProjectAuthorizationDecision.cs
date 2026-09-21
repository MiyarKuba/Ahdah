using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ProjectAuthorizationDecision
{
    public Guid ProjectAuthorizationDecisionId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectId { get; set; }

    public string ActionCode { get; set; } = null!;

    public Guid ActorUserId { get; set; }

    public string ActorRole { get; set; } = null!;

    public DateTime DecidedAt { get; set; }

    public string Decision { get; set; } = null!;

    public Guid PolicyVersionId { get; set; }

    public Guid? PreparerUserId { get; set; }

    public Guid? SubmitterUserId { get; set; }

    public Guid? BeneficiaryUserId { get; set; }

    public bool ConflictDetected { get; set; }

    public string? ConflictReason { get; set; }

    public Guid? EvidenceId { get; set; }

    public Guid? DelegationReference { get; set; }

    public byte[] RequestHash { get; set; } = null!;

    public string Details { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser? AppUser2 { get; set; }

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<CustodyOperation> CustodyOperations { get; set; } = new List<CustodyOperation>();

    public virtual ICollection<DocumentCustodyEvent> DocumentCustodyEvents { get; set; } = new List<DocumentCustodyEvent>();

    public virtual ICollection<DocumentPolicyException> DocumentPolicyExceptions { get; set; } = new List<DocumentPolicyException>();

    public virtual ICollection<FinancialResolutionEffect> FinancialResolutionEffects { get; set; } = new List<FinancialResolutionEffect>();

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<ProjectClosureCertificate> ProjectClosureCertificates { get; set; } = new List<ProjectClosureCertificate>();

    public virtual ICollection<ProjectOperationalEvent> ProjectOperationalEvents { get; set; } = new List<ProjectOperationalEvent>();

    public virtual ICollection<ProjectReopening> ProjectReopenings { get; set; } = new List<ProjectReopening>();

    public virtual ICollection<ProjectSettlementEvent> ProjectSettlementEvents { get; set; } = new List<ProjectSettlementEvent>();

    public virtual SettlementPolicyVersion SettlementPolicyVersion { get; set; } = null!;

    public virtual VerifiedEvidenceObject? VerifiedEvidenceObject { get; set; }
}
