using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class VerifiedEvidenceObject
{
    public Guid VerifiedEvidenceObjectId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid? ProjectId { get; set; }

    public Guid? ExpenseDocumentId { get; set; }

    public string StorageObjectKey { get; set; } = null!;

    public string StorageVersion { get; set; } = null!;

    public byte[] Sha256Hash { get; set; } = null!;

    public string MediaType { get; set; } = null!;

    public long ByteLength { get; set; }

    public Guid VerifiedByUserId { get; set; }

    public DateTime VerifiedAt { get; set; }

    public string VerificationMethod { get; set; } = null!;

    public Guid? SourceEvidenceId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<DocumentCustodyEvent> DocumentCustodyEvents { get; set; } = new List<DocumentCustodyEvent>();

    public virtual ICollection<DocumentPolicyException> DocumentPolicyExceptions { get; set; } = new List<DocumentPolicyException>();

    public virtual ExpenseDocument? ExpenseDocument { get; set; }

    public virtual ICollection<FinancialResolutionEffect> FinancialResolutionEffects { get; set; } = new List<FinancialResolutionEffect>();

    public virtual ICollection<VerifiedEvidenceObject> InverseVerifiedEvidenceObjectNavigation { get; set; } = new List<VerifiedEvidenceObject>();

    public virtual ICollection<LegacyAttributionReview> LegacyAttributionReviews { get; set; } = new List<LegacyAttributionReview>();

    public virtual Project? Project { get; set; }

    public virtual ICollection<ProjectAuthorizationDecision> ProjectAuthorizationDecisions { get; set; } = new List<ProjectAuthorizationDecision>();

    public virtual ICollection<ProjectOperationalEvent> ProjectOperationalEvents { get; set; } = new List<ProjectOperationalEvent>();

    public virtual ICollection<ProjectOwnerReconciliation> ProjectOwnerReconciliations { get; set; } = new List<ProjectOwnerReconciliation>();

    public virtual ICollection<ProjectOwnerRetentionItem> ProjectOwnerRetentionItems { get; set; } = new List<ProjectOwnerRetentionItem>();

    public virtual VerifiedEvidenceObject? VerifiedEvidenceObjectNavigation { get; set; }
}
