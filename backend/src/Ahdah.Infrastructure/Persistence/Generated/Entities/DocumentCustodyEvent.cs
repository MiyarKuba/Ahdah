using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class DocumentCustodyEvent
{
    public Guid DocumentCustodyEventId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid EvidenceId { get; set; }

    public Guid? ExpenseDocumentId { get; set; }

    public string EventType { get; set; } = null!;

    public Guid ActorUserId { get; set; }

    public string ActorRole { get; set; } = null!;

    public DateTime OccurredAt { get; set; }

    public string Reason { get; set; } = null!;

    public Guid PolicyVersionId { get; set; }

    public string CategoryCode { get; set; } = null!;

    public Guid? PreviousEventId { get; set; }

    public Guid? AuthorizationDecisionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual DocumentCustodyEvent? DocumentCustodyEventNavigation { get; set; }

    public virtual ExpenseDocument? ExpenseDocument { get; set; }

    public virtual ICollection<DocumentCustodyEvent> InverseDocumentCustodyEventNavigation { get; set; } = new List<DocumentCustodyEvent>();

    public virtual Project Project { get; set; } = null!;

    public virtual ProjectAuthorizationDecision? ProjectAuthorizationDecision { get; set; }

    public virtual SettlementPolicyVersion SettlementPolicyVersion { get; set; } = null!;

    public virtual VerifiedEvidenceObject VerifiedEvidenceObject { get; set; } = null!;
}
