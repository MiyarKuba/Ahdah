using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class Project
{
    public Guid ProjectId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectOwnerId { get; set; }

    public string ProjectName { get; set; } = null!;

    public string SiteAddress { get; set; } = null!;

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? ContactPhoneNumber { get; set; }

    public decimal ContractValue { get; set; }

    public DateOnly ContractDate { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? ExpectedEndDate { get; set; }

    public DateOnly? ActualEndDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Description { get; set; }

    public string? Notes { get; set; }

    public Guid CreatedByUserId { get; set; }

    public DateTime? CompletedAt { get; set; }

    public Guid? CancelledByUserId { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? ContractCurrencyCode { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<DocumentCustodyEvent> DocumentCustodyEvents { get; set; } = new List<DocumentCustodyEvent>();

    public virtual ICollection<DocumentPolicyException> DocumentPolicyExceptions { get; set; } = new List<DocumentPolicyException>();

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public virtual ICollection<FinancialResolutionEffect> FinancialResolutionEffects { get; set; } = new List<FinancialResolutionEffect>();

    public virtual ICollection<LegacyAttributionReviewProject> LegacyAttributionReviewProjects { get; set; } = new List<LegacyAttributionReviewProject>();

    public virtual ICollection<OwnerPaymentProjectAllocation> OwnerPaymentProjectAllocations { get; set; } = new List<OwnerPaymentProjectAllocation>();

    public virtual ICollection<OwnerPaymentRefund> OwnerPaymentRefunds { get; set; } = new List<OwnerPaymentRefund>();

    public virtual ICollection<PersonalClaim> PersonalClaims { get; set; } = new List<PersonalClaim>();

    public virtual ICollection<ProjectAuthorizationDecision> ProjectAuthorizationDecisions { get; set; } = new List<ProjectAuthorizationDecision>();

    public virtual ICollection<ProjectClosureCertificate> ProjectClosureCertificates { get; set; } = new List<ProjectClosureCertificate>();

    public virtual ProjectContractChange? ProjectContractChange { get; set; }

    public virtual ICollection<ProjectCustodyPosition> ProjectCustodyPositions { get; set; } = new List<ProjectCustodyPosition>();

    public virtual ProjectFinancialControl? ProjectFinancialControl { get; set; }

    public virtual ICollection<ProjectOperationalEvent> ProjectOperationalEvents { get; set; } = new List<ProjectOperationalEvent>();

    public virtual ProjectOwner ProjectOwner { get; set; } = null!;

    public virtual ICollection<ProjectOwnerReconciliationPosition> ProjectOwnerReconciliationPositions { get; set; } = new List<ProjectOwnerReconciliationPosition>();

    public virtual ICollection<ProjectOwnerReconciliation> ProjectOwnerReconciliations { get; set; } = new List<ProjectOwnerReconciliation>();

    public virtual ICollection<ProjectOwnerRetentionItem> ProjectOwnerRetentionItems { get; set; } = new List<ProjectOwnerRetentionItem>();

    public virtual ICollection<ProjectReopening> ProjectReopenings { get; set; } = new List<ProjectReopening>();

    public virtual ICollection<ProjectSettlementEvent> ProjectSettlementEvents { get; set; } = new List<ProjectSettlementEvent>();

    public virtual ICollection<ProjectSettlementSnapshotCategory> ProjectSettlementSnapshotCategories { get; set; } = new List<ProjectSettlementSnapshotCategory>();

    public virtual ICollection<ProjectSettlementSnapshot> ProjectSettlementSnapshots { get; set; } = new List<ProjectSettlementSnapshot>();

    public virtual ICollection<ProjectSettlement> ProjectSettlements { get; set; } = new List<ProjectSettlement>();

    public virtual ICollection<ProjectSupervisor> ProjectSupervisors { get; set; } = new List<ProjectSupervisor>();

    public virtual ICollection<SupplierCreditIntent> SupplierCreditIntents { get; set; } = new List<SupplierCreditIntent>();

    public virtual ICollection<SupplierDebt> SupplierDebts { get; set; } = new List<SupplierDebt>();

    public virtual ICollection<VerifiedEvidenceObject> VerifiedEvidenceObjects { get; set; } = new List<VerifiedEvidenceObject>();
}
