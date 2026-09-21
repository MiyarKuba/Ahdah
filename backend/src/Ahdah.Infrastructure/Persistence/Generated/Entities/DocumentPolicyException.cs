using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class DocumentPolicyException
{
    public Guid DocumentPolicyExceptionId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid EvidenceId { get; set; }

    public Guid? ExpenseDocumentId { get; set; }

    public string CategoryCode { get; set; } = null!;

    public string RequirementCode { get; set; } = null!;

    public Guid PolicyVersionId { get; set; }

    public Guid ApprovedByUserId { get; set; }

    public string ActorRole { get; set; } = null!;

    public DateTime ApprovedAt { get; set; }

    public string Reason { get; set; } = null!;

    public Guid AuthorizationDecisionId { get; set; }

    public Guid? SupersedesExceptionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual DocumentPolicyException? DocumentPolicyExceptionNavigation { get; set; }

    public virtual ExpenseDocument? ExpenseDocument { get; set; }

    public virtual ICollection<DocumentPolicyException> InverseDocumentPolicyExceptionNavigation { get; set; } = new List<DocumentPolicyException>();

    public virtual Project Project { get; set; } = null!;

    public virtual ProjectAuthorizationDecision ProjectAuthorizationDecision { get; set; } = null!;

    public virtual SettlementPolicyVersion SettlementPolicyVersion { get; set; } = null!;

    public virtual VerifiedEvidenceObject VerifiedEvidenceObject { get; set; } = null!;
}
