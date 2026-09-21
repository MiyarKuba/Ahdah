using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class Company
{
    public Guid CompanyId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string CompanyCode { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? LogoUrl { get; set; }

    public string Status { get; set; } = null!;

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Advance> Advances { get; set; } = new List<Advance>();

    public virtual AppUser? AppUser { get; set; }

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<BackgroundJob> BackgroundJobs { get; set; } = new List<BackgroundJob>();

    public virtual ICollection<CompanyNumberSequence> CompanyNumberSequences { get; set; } = new List<CompanyNumberSequence>();

    public virtual CompanySetting? CompanySetting { get; set; }

    public virtual CompanySettlementPolicyBinding? CompanySettlementPolicyBinding { get; set; }

    public virtual ICollection<CustodyMovementEntry> CustodyMovementEntries { get; set; } = new List<CustodyMovementEntry>();

    public virtual ICollection<CustodyOperation> CustodyOperations { get; set; } = new List<CustodyOperation>();

    public virtual ICollection<CustodyReservation> CustodyReservations { get; set; } = new List<CustodyReservation>();

    public virtual ICollection<DocumentCustodyEvent> DocumentCustodyEvents { get; set; } = new List<DocumentCustodyEvent>();

    public virtual ICollection<DocumentPolicyException> DocumentPolicyExceptions { get; set; } = new List<DocumentPolicyException>();

    public virtual ICollection<ExpenseCategory> ExpenseCategories { get; set; } = new List<ExpenseCategory>();

    public virtual ICollection<FinancialResolutionEffect> FinancialResolutionEffects { get; set; } = new List<FinancialResolutionEffect>();

    public virtual ICollection<FundingSource> FundingSources { get; set; } = new List<FundingSource>();

    public virtual ICollection<IdempotencyRecord> IdempotencyRecords { get; set; } = new List<IdempotencyRecord>();

    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();

    public virtual ICollection<JoinRequest> JoinRequests { get; set; } = new List<JoinRequest>();

    public virtual ICollection<LegacyAttributionReviewProject> LegacyAttributionReviewProjects { get; set; } = new List<LegacyAttributionReviewProject>();

    public virtual ICollection<LegacyAttributionReview> LegacyAttributionReviews { get; set; } = new List<LegacyAttributionReview>();

    public virtual ICollection<NotificationTemplate> NotificationTemplates { get; set; } = new List<NotificationTemplate>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<OutboxMessage> OutboxMessages { get; set; } = new List<OutboxMessage>();

    public virtual ICollection<ProjectAuthorizationDecision> ProjectAuthorizationDecisions { get; set; } = new List<ProjectAuthorizationDecision>();

    public virtual ICollection<ProjectClosureCertificate> ProjectClosureCertificates { get; set; } = new List<ProjectClosureCertificate>();

    public virtual ICollection<ProjectCustodyPosition> ProjectCustodyPositions { get; set; } = new List<ProjectCustodyPosition>();

    public virtual ICollection<ProjectFinancialControl> ProjectFinancialControls { get; set; } = new List<ProjectFinancialControl>();

    public virtual ICollection<ProjectOperationalEvent> ProjectOperationalEvents { get; set; } = new List<ProjectOperationalEvent>();

    public virtual ICollection<ProjectOwnerReconciliationPosition> ProjectOwnerReconciliationPositions { get; set; } = new List<ProjectOwnerReconciliationPosition>();

    public virtual ICollection<ProjectOwnerReconciliation> ProjectOwnerReconciliations { get; set; } = new List<ProjectOwnerReconciliation>();

    public virtual ICollection<ProjectOwnerRetentionItem> ProjectOwnerRetentionItems { get; set; } = new List<ProjectOwnerRetentionItem>();

    public virtual ICollection<ProjectOwner> ProjectOwners { get; set; } = new List<ProjectOwner>();

    public virtual ICollection<ProjectReopening> ProjectReopenings { get; set; } = new List<ProjectReopening>();

    public virtual ICollection<ProjectSettlementEvent> ProjectSettlementEvents { get; set; } = new List<ProjectSettlementEvent>();

    public virtual ICollection<ProjectSettlementSnapshotCategory> ProjectSettlementSnapshotCategories { get; set; } = new List<ProjectSettlementSnapshotCategory>();

    public virtual ICollection<ProjectSettlementSnapshot> ProjectSettlementSnapshots { get; set; } = new List<ProjectSettlementSnapshot>();

    public virtual ICollection<ProjectSettlement> ProjectSettlements { get; set; } = new List<ProjectSettlement>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<ReportDefinition> ReportDefinitions { get; set; } = new List<ReportDefinition>();

    public virtual ICollection<ReportRecipientSuppression> ReportRecipientSuppressions { get; set; } = new List<ReportRecipientSuppression>();

    public virtual ICollection<ReportSchedule> ReportSchedules { get; set; } = new List<ReportSchedule>();

    public virtual ICollection<SavedReportView> SavedReportViews { get; set; } = new List<SavedReportView>();

    public virtual ICollection<ScheduledJobDefinition> ScheduledJobDefinitions { get; set; } = new List<ScheduledJobDefinition>();

    public virtual ICollection<SettlementPolicyVersion> SettlementPolicyVersions { get; set; } = new List<SettlementPolicyVersion>();

    public virtual ICollection<SupplierCreditIntentApplication> SupplierCreditIntentApplications { get; set; } = new List<SupplierCreditIntentApplication>();

    public virtual ICollection<SupplierCreditIntent> SupplierCreditIntents { get; set; } = new List<SupplierCreditIntent>();

    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();

    public virtual ICollection<VerifiedEvidenceObject> VerifiedEvidenceObjects { get; set; } = new List<VerifiedEvidenceObject>();
}
