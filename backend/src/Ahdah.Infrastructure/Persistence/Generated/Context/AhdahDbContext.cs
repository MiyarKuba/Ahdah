using System;
using System.Collections.Generic;
using Ahdah.Infrastructure.Persistence.Generated.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ahdah.Infrastructure.Persistence.Generated.Context;

public partial class AhdahDbContext : DbContext
{
    public AhdahDbContext(DbContextOptions<AhdahDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Advance> Advances { get; set; }

    public virtual DbSet<AdvanceClosure> AdvanceClosures { get; set; }

    public virtual DbSet<AdvanceClosureDocument> AdvanceClosureDocuments { get; set; }

    public virtual DbSet<AdvanceFundingSource> AdvanceFundingSources { get; set; }

    public virtual DbSet<AdvanceSettlement> AdvanceSettlements { get; set; }

    public virtual DbSet<AdvanceSettlementDocument> AdvanceSettlementDocuments { get; set; }

    public virtual DbSet<AdvanceSettlementResolution> AdvanceSettlementResolutions { get; set; }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<BackgroundJob> BackgroundJobs { get; set; }

    public virtual DbSet<BackgroundJobAttempt> BackgroundJobAttempts { get; set; }

    public virtual DbSet<BalanceLedgerEntry> BalanceLedgerEntries { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<CompanyCashboxEntry> CompanyCashboxEntries { get; set; }

    public virtual DbSet<CompanyNumberSequence> CompanyNumberSequences { get; set; }

    public virtual DbSet<CompanyNumberSequenceAllocation> CompanyNumberSequenceAllocations { get; set; }

    public virtual DbSet<CompanyNumberSequenceVersion> CompanyNumberSequenceVersions { get; set; }

    public virtual DbSet<CompanySetting> CompanySettings { get; set; }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<ExpenseAdvanceAllocation> ExpenseAdvanceAllocations { get; set; }

    public virtual DbSet<ExpenseCategory> ExpenseCategories { get; set; }

    public virtual DbSet<ExpenseDocument> ExpenseDocuments { get; set; }

    public virtual DbSet<ExpenseItem> ExpenseItems { get; set; }

    public virtual DbSet<ExpenseReturn> ExpenseReturns { get; set; }

    public virtual DbSet<ExpenseReturnAdvanceAllocation> ExpenseReturnAdvanceAllocations { get; set; }

    public virtual DbSet<ExpenseReturnItem> ExpenseReturnItems { get; set; }

    public virtual DbSet<FundingSource> FundingSources { get; set; }

    public virtual DbSet<FundingSourceLedgerEntry> FundingSourceLedgerEntries { get; set; }

    public virtual DbSet<FundingSourcePaymentMethod> FundingSourcePaymentMethods { get; set; }

    public virtual DbSet<IdempotencyRecord> IdempotencyRecords { get; set; }

    public virtual DbSet<Invitation> Invitations { get; set; }

    public virtual DbSet<JoinRequest> JoinRequests { get; set; }

    public virtual DbSet<ManagerContribution> ManagerContributions { get; set; }

    public virtual DbSet<MoneyTransfer> MoneyTransfers { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationDelivery> NotificationDeliveries { get; set; }

    public virtual DbSet<NotificationPreference> NotificationPreferences { get; set; }

    public virtual DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    public virtual DbSet<NotificationTemplateVersion> NotificationTemplateVersions { get; set; }

    public virtual DbSet<OutboxMessage> OutboxMessages { get; set; }

    public virtual DbSet<OutboxMessageAttempt> OutboxMessageAttempts { get; set; }

    public virtual DbSet<OwnerPaymentProjectAllocation> OwnerPaymentProjectAllocations { get; set; }

    public virtual DbSet<OwnerPaymentRefund> OwnerPaymentRefunds { get; set; }

    public virtual DbSet<OwnerRefundFundingSource> OwnerRefundFundingSources { get; set; }

    public virtual DbSet<PersonalClaim> PersonalClaims { get; set; }

    public virtual DbSet<PersonalClaimAdjustment> PersonalClaimAdjustments { get; set; }

    public virtual DbSet<PersonalClaimLedgerEntry> PersonalClaimLedgerEntries { get; set; }

    public virtual DbSet<PersonalClaimPayment> PersonalClaimPayments { get; set; }

    public virtual DbSet<PersonalClaimPaymentAllocation> PersonalClaimPaymentAllocations { get; set; }

    public virtual DbSet<PersonalClaimPaymentFundingSource> PersonalClaimPaymentFundingSources { get; set; }

    public virtual DbSet<PersonalClaimWriteOff> PersonalClaimWriteOffs { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectContractChange> ProjectContractChanges { get; set; }

    public virtual DbSet<ProjectOwner> ProjectOwners { get; set; }

    public virtual DbSet<ProjectOwnerPayment> ProjectOwnerPayments { get; set; }

    public virtual DbSet<ProjectSupervisor> ProjectSupervisors { get; set; }

    public virtual DbSet<ReportDefinition> ReportDefinitions { get; set; }

    public virtual DbSet<ReportDefinitionVersion> ReportDefinitionVersions { get; set; }

    public virtual DbSet<ReportDelivery> ReportDeliveries { get; set; }

    public virtual DbSet<ReportDeliveryAttempt> ReportDeliveryAttempts { get; set; }

    public virtual DbSet<ReportDeliveryEvent> ReportDeliveryEvents { get; set; }

    public virtual DbSet<ReportDeliveryFile> ReportDeliveryFiles { get; set; }

    public virtual DbSet<ReportFileDownload> ReportFileDownloads { get; set; }

    public virtual DbSet<ReportRecipientSuppression> ReportRecipientSuppressions { get; set; }

    public virtual DbSet<ReportRecipientSuppressionEvent> ReportRecipientSuppressionEvents { get; set; }

    public virtual DbSet<ReportRun> ReportRuns { get; set; }

    public virtual DbSet<ReportRunFile> ReportRunFiles { get; set; }

    public virtual DbSet<ReportSchedule> ReportSchedules { get; set; }

    public virtual DbSet<ReportScheduleRecipient> ReportScheduleRecipients { get; set; }

    public virtual DbSet<SavedReportView> SavedReportViews { get; set; }

    public virtual DbSet<SavedReportViewShare> SavedReportViewShares { get; set; }

    public virtual DbSet<SavedReportViewShareEvent> SavedReportViewShareEvents { get; set; }

    public virtual DbSet<SavedReportViewVersion> SavedReportViewVersions { get; set; }

    public virtual DbSet<ScheduledJobDefinition> ScheduledJobDefinitions { get; set; }

    public virtual DbSet<ScheduledJobRun> ScheduledJobRuns { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SupplierCreditNote> SupplierCreditNotes { get; set; }

    public virtual DbSet<SupplierCreditNoteAllocation> SupplierCreditNoteAllocations { get; set; }

    public virtual DbSet<SupplierDebt> SupplierDebts { get; set; }

    public virtual DbSet<SupplierDebtAdjustment> SupplierDebtAdjustments { get; set; }

    public virtual DbSet<SupplierDebtLedgerEntry> SupplierDebtLedgerEntries { get; set; }

    public virtual DbSet<SupplierDebtWriteOff> SupplierDebtWriteOffs { get; set; }

    public virtual DbSet<SupplierPayment> SupplierPayments { get; set; }

    public virtual DbSet<SupplierPaymentAccount> SupplierPaymentAccounts { get; set; }

    public virtual DbSet<SupplierPaymentDebtAllocation> SupplierPaymentDebtAllocations { get; set; }

    public virtual DbSet<SupplierPaymentFundingSource> SupplierPaymentFundingSources { get; set; }

    public virtual DbSet<SupplierRefund> SupplierRefunds { get; set; }

    public virtual DbSet<TransferAdvanceAllocation> TransferAdvanceAllocations { get; set; }

    public virtual DbSet<TransferCorrectionRequest> TransferCorrectionRequests { get; set; }

    public virtual DbSet<UserAdvanceBalance> UserAdvanceBalances { get; set; }

    public virtual DbSet<UserDevice> UserDevices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<Advance>(entity =>
        {
            entity.HasKey(e => e.AdvanceId).HasName("advances_pkey");

            entity.ToTable("advances", "ahdah");

            entity.HasIndex(e => e.CompanyId, "ix_advances_company");

            entity.HasIndex(e => new { e.CompanyId, e.DeputyUserId }, "ix_advances_deputy");

            entity.HasIndex(e => new { e.CompanyId, e.DeputyUserId, e.Status }, "ix_advances_deputy_status");

            entity.HasIndex(e => new { e.CompanyId, e.IssueDate }, "ix_advances_issue_date");

            entity.HasIndex(e => new { e.CompanyId, e.SettlementDueDate }, "ix_advances_settlement_due_date").HasFilter("((settlement_due_date IS NOT NULL) AND ((status)::text <> ALL ((ARRAY['Closed'::character varying, 'Cancelled'::character varying, 'Reversed'::character varying])::text[])))");

            entity.HasIndex(e => new { e.CompanyId, e.Status }, "ix_advances_status");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceId }, "uq_advances_company_advance").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceNumber }, "uq_advances_number").IsUnique();

            entity.Property(e => e.AdvanceId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("advance_id");
            entity.Property(e => e.AdvanceAmount)
                .HasPrecision(18, 2)
                .HasColumnName("advance_amount");
            entity.Property(e => e.AdvanceNumber)
                .HasMaxLength(50)
                .HasColumnName("advance_number");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.ClosedAt).HasColumnName("closed_at");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.ConfirmedAt).HasColumnName("confirmed_at");
            entity.Property(e => e.ConfirmedByUserId).HasColumnName("confirmed_by_user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'LYD'::character varying")
                .HasColumnName("currency_code");
            entity.Property(e => e.DeputyUserId).HasColumnName("deputy_user_id");
            entity.Property(e => e.IssueDate).HasColumnName("issue_date");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.Purpose)
                .HasMaxLength(1000)
                .HasColumnName("purpose");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.SettlementDueDate).HasColumnName("settlement_due_date");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.Company).WithMany(p => p.Advances)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advances_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.AdvanceAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advances_cancelled_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.AdvanceAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ConfirmedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advances_confirmed_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.AdvanceAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advances_created_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.AdvanceAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.DeputyUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advances_deputy");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.AdvanceAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advances_reversed_by");
        });

        modelBuilder.Entity<AdvanceClosure>(entity =>
        {
            entity.HasKey(e => e.AdvanceClosureId).HasName("advance_closures_pkey");

            entity.ToTable("advance_closures", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceId }, "ix_advance_closures_advance");

            entity.HasIndex(e => new { e.CompanyId, e.CreatedByUserId }, "ix_advance_closures_created_by");

            entity.HasIndex(e => new { e.CompanyId, e.ClosureDate }, "ix_advance_closures_date");

            entity.HasIndex(e => new { e.CompanyId, e.SubmittedAt }, "ix_advance_closures_pending").HasFilter("((status)::text = 'PendingApproval'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.ReconciliationStatus }, "ix_advance_closures_reconciliation");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_advance_closures_status");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceClosureId }, "uq_advance_closures_company_closure").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ClosureNumber }, "uq_advance_closures_number").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceId }, "ux_advance_closures_one_active")
                .IsUnique()
                .HasFilter("((status)::text = ANY ((ARRAY['Draft'::character varying, 'PendingApproval'::character varying, 'Approved'::character varying])::text[]))");

            entity.Property(e => e.AdvanceClosureId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("advance_closure_id");
            entity.Property(e => e.AdvanceId).HasColumnName("advance_id");
            entity.Property(e => e.AdvanceVersionNumber).HasColumnName("advance_version_number");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedByUserId).HasColumnName("approved_by_user_id");
            entity.Property(e => e.AuthorizationDocumentUrl)
                .HasMaxLength(1000)
                .HasColumnName("authorization_document_url");
            entity.Property(e => e.BalanceCount).HasColumnName("balance_count");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.ClosureDate).HasColumnName("closure_date");
            entity.Property(e => e.ClosureNumber)
                .HasMaxLength(50)
                .HasColumnName("closure_number");
            entity.Property(e => e.ClosureReason)
                .HasMaxLength(1000)
                .HasColumnName("closure_reason");
            entity.Property(e => e.ClosureType)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Normal'::character varying")
                .HasColumnName("closure_type");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'LYD'::character varying")
                .HasColumnName("currency_code");
            entity.Property(e => e.OutstandingPersonalClaimAmount)
                .HasPrecision(18, 2)
                .HasColumnName("outstanding_personal_claim_amount");
            entity.Property(e => e.OutstandingSupplierDebtAmount)
                .HasPrecision(18, 2)
                .HasColumnName("outstanding_supplier_debt_amount");
            entity.Property(e => e.PendingOperationCount).HasColumnName("pending_operation_count");
            entity.Property(e => e.ReconciliationStatus)
                .HasMaxLength(20)
                .HasComputedColumnSql("\nCASE\n    WHEN ((balance_count = settled_balance_count) AND (pending_operation_count = 0) AND (unresolved_issue_count = 0) AND (total_available_amount = (0)::numeric) AND (total_reserved_amount = (0)::numeric) AND (unresolved_difference_amount = (0)::numeric)) THEN 'ReadyToClose'::text\n    ELSE 'NotReady'::text\nEND", true)
                .HasColumnName("reconciliation_status");
            entity.Property(e => e.RejectedAt).HasColumnName("rejected_at");
            entity.Property(e => e.RejectedByUserId).HasColumnName("rejected_by_user_id");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.ReviewNotes)
                .HasMaxLength(1000)
                .HasColumnName("review_notes");
            entity.Property(e => e.SettledBalanceCount).HasColumnName("settled_balance_count");
            entity.Property(e => e.SettledShortageAmount)
                .HasPrecision(18, 2)
                .HasColumnName("settled_shortage_amount");
            entity.Property(e => e.SettledSurplusAmount)
                .HasPrecision(18, 2)
                .HasColumnName("settled_surplus_amount");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SubmittedAt).HasColumnName("submitted_at");
            entity.Property(e => e.SubmittedByUserId).HasColumnName("submitted_by_user_id");
            entity.Property(e => e.TotalAvailableAmount)
                .HasPrecision(18, 2)
                .HasColumnName("total_available_amount");
            entity.Property(e => e.TotalReservedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("total_reserved_amount");
            entity.Property(e => e.UnresolvedDifferenceAmount)
                .HasPrecision(18, 2)
                .HasColumnName("unresolved_difference_amount");
            entity.Property(e => e.UnresolvedIssueCount).HasColumnName("unresolved_issue_count");
            entity.Property(e => e.UnsettledBalanceCount)
                .HasComputedColumnSql("(balance_count - settled_balance_count)", true)
                .HasColumnName("unsettled_balance_count");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.Advance).WithOne(p => p.AdvanceClosure)
                .HasPrincipalKey<Advance>(p => new { p.CompanyId, p.AdvanceId })
                .HasForeignKey<AdvanceClosure>(d => new { d.CompanyId, d.AdvanceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_closures_advance");

            entity.HasOne(d => d.AppUser).WithMany(p => p.AdvanceClosureAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ApprovedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_closures_approved_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.AdvanceClosureAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_closures_cancelled_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.AdvanceClosureAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_closures_created_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.AdvanceClosureAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RejectedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_closures_rejected_by");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.AdvanceClosureAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_closures_reversed_by");

            entity.HasOne(d => d.AppUser4).WithMany(p => p.AdvanceClosureAppUser4s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.SubmittedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_closures_submitted_by");
        });

        modelBuilder.Entity<AdvanceClosureDocument>(entity =>
        {
            entity.HasKey(e => e.AdvanceClosureDocumentId).HasName("advance_closure_documents_pkey");

            entity.ToTable("advance_closure_documents", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceClosureId, e.CreatedAt }, "ix_advance_closure_documents_closure");

            entity.HasIndex(e => new { e.CompanyId, e.Sha256Hash }, "ix_advance_closure_documents_hash");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceClosureId, e.VerificationStatus }, "ix_advance_closure_documents_required").HasFilter("(is_required = true)");

            entity.HasIndex(e => new { e.CompanyId, e.VerificationStatus }, "ix_advance_closure_documents_status");

            entity.HasIndex(e => new { e.CompanyId, e.DocumentType }, "ix_advance_closure_documents_type");

            entity.HasIndex(e => new { e.CompanyId, e.UploadedByUserId }, "ix_advance_closure_documents_uploaded_by");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceClosureId, e.Sha256Hash }, "uq_advance_closure_documents_closure_hash").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceClosureDocumentId }, "uq_advance_closure_documents_company_document").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceClosureId }, "ux_advance_closure_documents_one_primary")
                .IsUnique()
                .HasFilter("((is_primary = true) AND ((verification_status)::text <> 'Rejected'::text))");

            entity.Property(e => e.AdvanceClosureDocumentId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("advance_closure_document_id");
            entity.Property(e => e.AdvanceClosureId).HasColumnName("advance_closure_id");
            entity.Property(e => e.CaptureSource)
                .HasMaxLength(30)
                .HasDefaultValueSql("'FileUpload'::character varying")
                .HasColumnName("capture_source");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DocumentDate).HasColumnName("document_date");
            entity.Property(e => e.DocumentNumber)
                .HasMaxLength(100)
                .HasColumnName("document_number");
            entity.Property(e => e.DocumentTitle)
                .HasMaxLength(250)
                .HasColumnName("document_title");
            entity.Property(e => e.DocumentType)
                .HasMaxLength(50)
                .HasColumnName("document_type");
            entity.Property(e => e.FileSizeBytes).HasColumnName("file_size_bytes");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(1000)
                .HasColumnName("file_url");
            entity.Property(e => e.IsPrimary).HasColumnName("is_primary");
            entity.Property(e => e.IsRequired).HasColumnName("is_required");
            entity.Property(e => e.IssuerName)
                .HasMaxLength(200)
                .HasColumnName("issuer_name");
            entity.Property(e => e.MimeType)
                .HasMaxLength(150)
                .HasColumnName("mime_type");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.OriginalFileName)
                .HasMaxLength(255)
                .HasColumnName("original_file_name");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.Sha256Hash)
                .HasMaxLength(64)
                .HasColumnName("sha256_hash");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UploadedByUserId).HasColumnName("uploaded_by_user_id");
            entity.Property(e => e.VerificationStatus)
                .HasMaxLength(30)
                .HasDefaultValueSql("'PendingVerification'::character varying")
                .HasColumnName("verification_status");
            entity.Property(e => e.VerifiedAt).HasColumnName("verified_at");
            entity.Property(e => e.VerifiedByUserId).HasColumnName("verified_by_user_id");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AdvanceClosure).WithOne(p => p.AdvanceClosureDocument)
                .HasPrincipalKey<AdvanceClosure>(p => new { p.CompanyId, p.AdvanceClosureId })
                .HasForeignKey<AdvanceClosureDocument>(d => new { d.CompanyId, d.AdvanceClosureId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_closure_documents_closure");

            entity.HasOne(d => d.AppUser).WithMany(p => p.AdvanceClosureDocumentAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.UploadedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_closure_documents_uploaded_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.AdvanceClosureDocumentAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.VerifiedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_closure_documents_verified_by");
        });

        modelBuilder.Entity<AdvanceFundingSource>(entity =>
        {
            entity.HasKey(e => e.AdvanceFundingSourceId).HasName("advance_funding_sources_pkey");

            entity.ToTable("advance_funding_sources", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceId }, "ix_advance_funding_sources_advance");

            entity.HasIndex(e => new { e.CompanyId, e.AllocatedByUserId }, "ix_advance_funding_sources_allocated_by");

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourceId }, "ix_advance_funding_sources_source");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceId, e.FundingSourceId }, "uq_advance_funding_sources_advance_source").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceFundingSourceId }, "uq_advance_funding_sources_company_allocation").IsUnique();

            entity.Property(e => e.AdvanceFundingSourceId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("advance_funding_source_id");
            entity.Property(e => e.AdvanceId).HasColumnName("advance_id");
            entity.Property(e => e.AllocatedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("allocated_amount");
            entity.Property(e => e.AllocatedByUserId).HasColumnName("allocated_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.FundingSourceId).HasColumnName("funding_source_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Advance).WithMany(p => p.AdvanceFundingSources)
                .HasPrincipalKey(p => new { p.CompanyId, p.AdvanceId })
                .HasForeignKey(d => new { d.CompanyId, d.AdvanceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_funding_sources_advance");

            entity.HasOne(d => d.AppUser).WithMany(p => p.AdvanceFundingSources)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.AllocatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_funding_sources_allocated_by");

            entity.HasOne(d => d.FundingSource).WithMany(p => p.AdvanceFundingSources)
                .HasPrincipalKey(p => new { p.CompanyId, p.FundingSourceId })
                .HasForeignKey(d => new { d.CompanyId, d.FundingSourceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_funding_sources_source");
        });

        modelBuilder.Entity<AdvanceSettlement>(entity =>
        {
            entity.HasKey(e => e.AdvanceSettlementId).HasName("advance_settlements_pkey");

            entity.ToTable("advance_settlements", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.UserAdvanceBalanceId }, "ix_advance_settlements_balance");

            entity.HasIndex(e => new { e.CompanyId, e.CreatedByUserId }, "ix_advance_settlements_created_by");

            entity.HasIndex(e => new { e.CompanyId, e.SettlementDate }, "ix_advance_settlements_date");

            entity.HasIndex(e => new { e.CompanyId, e.DifferenceType }, "ix_advance_settlements_difference").HasFilter("((difference_type)::text <> 'Balanced'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.SubmittedAt }, "ix_advance_settlements_pending_review").HasFilter("((status)::text = 'PendingReview'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_advance_settlements_status");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceSettlementId }, "uq_advance_settlements_company_settlement").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.SettlementNumber }, "uq_advance_settlements_number").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.UserAdvanceBalanceId }, "ux_advance_settlements_one_active")
                .IsUnique()
                .HasFilter("((status)::text = ANY ((ARRAY['Draft'::character varying, 'PendingReview'::character varying, 'CorrectionRequired'::character varying, 'Approved'::character varying])::text[]))");

            entity.Property(e => e.AdvanceSettlementId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("advance_settlement_id");
            entity.Property(e => e.AdjustmentOutSnapshotAmount)
                .HasPrecision(18, 2)
                .HasColumnName("adjustment_out_snapshot_amount");
            entity.Property(e => e.AvailableSnapshotAmount)
                .HasPrecision(18, 2)
                .HasColumnName("available_snapshot_amount");
            entity.Property(e => e.BalanceVersionNumber).HasColumnName("balance_version_number");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CorrectionReason)
                .HasMaxLength(1000)
                .HasColumnName("correction_reason");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'LYD'::character varying")
                .HasColumnName("currency_code");
            entity.Property(e => e.DeclaredRemainingAmount)
                .HasPrecision(18, 2)
                .HasColumnName("declared_remaining_amount");
            entity.Property(e => e.DifferenceAmount)
                .HasPrecision(18, 2)
                .HasComputedColumnSql("(declared_remaining_amount - available_snapshot_amount)", true)
                .HasColumnName("difference_amount");
            entity.Property(e => e.DifferenceType)
                .HasMaxLength(20)
                .HasComputedColumnSql("\nCASE\n    WHEN (declared_remaining_amount = available_snapshot_amount) THEN 'Balanced'::text\n    WHEN (declared_remaining_amount < available_snapshot_amount) THEN 'Shortage'::text\n    ELSE 'Surplus'::text\nEND", true)
                .HasColumnName("difference_type");
            entity.Property(e => e.ExpensedSnapshotAmount)
                .HasPrecision(18, 2)
                .HasColumnName("expensed_snapshot_amount");
            entity.Property(e => e.ReceivedSnapshotAmount)
                .HasPrecision(18, 2)
                .HasColumnName("received_snapshot_amount");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(1000)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.ReservedSnapshotAmount)
                .HasPrecision(18, 2)
                .HasColumnName("reserved_snapshot_amount");
            entity.Property(e => e.ResolutionNotes)
                .HasMaxLength(1000)
                .HasColumnName("resolution_notes");
            entity.Property(e => e.RestoredSnapshotAmount)
                .HasPrecision(18, 2)
                .HasColumnName("restored_snapshot_amount");
            entity.Property(e => e.ReturnedSnapshotAmount)
                .HasPrecision(18, 2)
                .HasColumnName("returned_snapshot_amount");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.ReviewNotes)
                .HasMaxLength(1000)
                .HasColumnName("review_notes");
            entity.Property(e => e.ReviewedAt).HasColumnName("reviewed_at");
            entity.Property(e => e.ReviewedByUserId).HasColumnName("reviewed_by_user_id");
            entity.Property(e => e.SettlementDate).HasColumnName("settlement_date");
            entity.Property(e => e.SettlementExplanation)
                .HasMaxLength(1000)
                .HasColumnName("settlement_explanation");
            entity.Property(e => e.SettlementNumber)
                .HasMaxLength(50)
                .HasColumnName("settlement_number");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SubmittedAt).HasColumnName("submitted_at");
            entity.Property(e => e.SubmittedByUserId).HasColumnName("submitted_by_user_id");
            entity.Property(e => e.SupportingDocumentUrl)
                .HasMaxLength(1000)
                .HasColumnName("supporting_document_url");
            entity.Property(e => e.TransferredOutSnapshotAmount)
                .HasPrecision(18, 2)
                .HasColumnName("transferred_out_snapshot_amount");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserAdvanceBalanceId).HasColumnName("user_advance_balance_id");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.AdvanceSettlementAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_settlements_cancelled_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.AdvanceSettlementAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_settlements_created_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.AdvanceSettlementAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_settlements_reversed_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.AdvanceSettlementAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReviewedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_settlements_reviewed_by");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.AdvanceSettlementAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.SubmittedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_settlements_submitted_by");

            entity.HasOne(d => d.UserAdvanceBalance).WithOne(p => p.AdvanceSettlement)
                .HasPrincipalKey<UserAdvanceBalance>(p => new { p.CompanyId, p.UserAdvanceBalanceId })
                .HasForeignKey<AdvanceSettlement>(d => new { d.CompanyId, d.UserAdvanceBalanceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_settlements_balance");
        });

        modelBuilder.Entity<AdvanceSettlementDocument>(entity =>
        {
            entity.HasKey(e => e.AdvanceSettlementDocumentId).HasName("advance_settlement_documents_pkey");

            entity.ToTable("advance_settlement_documents", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.Sha256Hash }, "ix_advance_settlement_documents_hash");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceSettlementId, e.CreatedAt }, "ix_advance_settlement_documents_settlement");

            entity.HasIndex(e => new { e.CompanyId, e.VerificationStatus }, "ix_advance_settlement_documents_status");

            entity.HasIndex(e => new { e.CompanyId, e.DocumentType }, "ix_advance_settlement_documents_type");

            entity.HasIndex(e => new { e.CompanyId, e.UploadedByUserId }, "ix_advance_settlement_documents_uploaded_by");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceSettlementDocumentId }, "uq_advance_settlement_documents_company_document").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceSettlementId, e.Sha256Hash }, "uq_advance_settlement_documents_settlement_hash").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceSettlementId }, "ux_advance_settlement_documents_one_primary")
                .IsUnique()
                .HasFilter("((is_primary = true) AND ((verification_status)::text <> 'Rejected'::text))");

            entity.Property(e => e.AdvanceSettlementDocumentId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("advance_settlement_document_id");
            entity.Property(e => e.AdvanceSettlementId).HasColumnName("advance_settlement_id");
            entity.Property(e => e.CaptureSource)
                .HasMaxLength(30)
                .HasDefaultValueSql("'FileUpload'::character varying")
                .HasColumnName("capture_source");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DocumentDate).HasColumnName("document_date");
            entity.Property(e => e.DocumentNumber)
                .HasMaxLength(100)
                .HasColumnName("document_number");
            entity.Property(e => e.DocumentType)
                .HasMaxLength(40)
                .HasColumnName("document_type");
            entity.Property(e => e.FileSizeBytes).HasColumnName("file_size_bytes");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(1000)
                .HasColumnName("file_url");
            entity.Property(e => e.IsPrimary).HasColumnName("is_primary");
            entity.Property(e => e.IssuerName)
                .HasMaxLength(200)
                .HasColumnName("issuer_name");
            entity.Property(e => e.MimeType)
                .HasMaxLength(150)
                .HasColumnName("mime_type");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.OriginalFileName)
                .HasMaxLength(255)
                .HasColumnName("original_file_name");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.Sha256Hash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("sha256_hash");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UploadedByUserId).HasColumnName("uploaded_by_user_id");
            entity.Property(e => e.VerificationStatus)
                .HasMaxLength(30)
                .HasDefaultValueSql("'PendingVerification'::character varying")
                .HasColumnName("verification_status");
            entity.Property(e => e.VerifiedAt).HasColumnName("verified_at");
            entity.Property(e => e.VerifiedByUserId).HasColumnName("verified_by_user_id");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AdvanceSettlement).WithOne(p => p.AdvanceSettlementDocument)
                .HasPrincipalKey<AdvanceSettlement>(p => new { p.CompanyId, p.AdvanceSettlementId })
                .HasForeignKey<AdvanceSettlementDocument>(d => new { d.CompanyId, d.AdvanceSettlementId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_settlement_documents_settlement");

            entity.HasOne(d => d.AppUser).WithMany(p => p.AdvanceSettlementDocumentAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.UploadedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_settlement_documents_uploaded_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.AdvanceSettlementDocumentAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.VerifiedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_settlement_documents_verified_by");
        });

        modelBuilder.Entity<AdvanceSettlementResolution>(entity =>
        {
            entity.HasKey(e => e.AdvanceSettlementResolutionId).HasName("advance_settlement_resolutions_pkey");

            entity.ToTable("advance_settlement_resolutions", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ResolutionDate }, "ix_advance_settlement_resolutions_date");

            entity.HasIndex(e => new { e.CompanyId, e.SubmittedAt }, "ix_advance_settlement_resolutions_pending").HasFilter("((status)::text = 'PendingApproval'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimId }, "ix_advance_settlement_resolutions_personal_claim").HasFilter("(personal_claim_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceSettlementId }, "ix_advance_settlement_resolutions_settlement");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceSettlementId, e.ResolutionType }, "ix_advance_settlement_resolutions_settlement_type");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_advance_settlement_resolutions_status");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceSettlementResolutionId }, "uq_advance_settlement_resolutions_company_resolution").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ResolutionNumber }, "uq_advance_settlement_resolutions_number").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourceId }, "ux_advance_settlement_resolutions_funding_source")
                .IsUnique()
                .HasFilter("(funding_source_id IS NOT NULL)");

            entity.Property(e => e.AdvanceSettlementResolutionId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("advance_settlement_resolution_id");
            entity.Property(e => e.AdvanceSettlementId).HasColumnName("advance_settlement_id");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedByUserId).HasColumnName("approved_by_user_id");
            entity.Property(e => e.BankName)
                .HasMaxLength(150)
                .HasColumnName("bank_name");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.FundingSourceId).HasColumnName("funding_source_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(30)
                .HasColumnName("payment_method");
            entity.Property(e => e.PersonalClaimId).HasColumnName("personal_claim_id");
            entity.Property(e => e.ProofFileUrl)
                .HasMaxLength(1000)
                .HasColumnName("proof_file_url");
            entity.Property(e => e.ReferenceNumber)
                .HasMaxLength(150)
                .HasColumnName("reference_number");
            entity.Property(e => e.RejectedAt).HasColumnName("rejected_at");
            entity.Property(e => e.RejectedByUserId).HasColumnName("rejected_by_user_id");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.ResolutionAmount)
                .HasPrecision(18, 2)
                .HasColumnName("resolution_amount");
            entity.Property(e => e.ResolutionDate).HasColumnName("resolution_date");
            entity.Property(e => e.ResolutionNumber)
                .HasMaxLength(50)
                .HasColumnName("resolution_number");
            entity.Property(e => e.ResolutionType)
                .HasMaxLength(40)
                .HasColumnName("resolution_type");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SubmittedAt).HasColumnName("submitted_at");
            entity.Property(e => e.SubmittedByUserId).HasColumnName("submitted_by_user_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AdvanceSettlement).WithMany(p => p.AdvanceSettlementResolutions)
                .HasPrincipalKey(p => new { p.CompanyId, p.AdvanceSettlementId })
                .HasForeignKey(d => new { d.CompanyId, d.AdvanceSettlementId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_settlement_resolutions_settlement");

            entity.HasOne(d => d.AppUser).WithMany(p => p.AdvanceSettlementResolutionAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ApprovedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_settlement_resolutions_approved_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.AdvanceSettlementResolutionAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_settlement_resolutions_cancelled_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.AdvanceSettlementResolutionAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_settlement_resolutions_created_by");

            entity.HasOne(d => d.FundingSource).WithOne(p => p.AdvanceSettlementResolution)
                .HasPrincipalKey<FundingSource>(p => new { p.CompanyId, p.FundingSourceId })
                .HasForeignKey<AdvanceSettlementResolution>(d => new { d.CompanyId, d.FundingSourceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_settlement_resolutions_funding_source");

            entity.HasOne(d => d.PersonalClaim).WithMany(p => p.AdvanceSettlementResolutions)
                .HasPrincipalKey(p => new { p.CompanyId, p.PersonalClaimId })
                .HasForeignKey(d => new { d.CompanyId, d.PersonalClaimId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_settlement_resolutions_personal_claim");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.AdvanceSettlementResolutionAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RejectedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_settlement_resolutions_rejected_by");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.AdvanceSettlementResolutionAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_settlement_resolutions_reversed_by");

            entity.HasOne(d => d.AppUser4).WithMany(p => p.AdvanceSettlementResolutionAppUser4s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.SubmittedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_advance_settlement_resolutions_submitted_by");
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("app_users_pkey");

            entity.ToTable("app_users", "ahdah");

            entity.HasIndex(e => e.CompanyId, "ix_app_users_company");

            entity.HasIndex(e => new { e.CompanyId, e.Role, e.Status }, "ix_app_users_company_role_status");

            entity.HasIndex(e => e.Status, "ix_app_users_status");

            entity.HasIndex(e => new { e.CompanyId, e.UserId }, "uq_app_users_company_user").IsUnique();

            entity.HasIndex(e => e.CompanyId, "ux_app_users_one_active_manager")
                .IsUnique()
                .HasFilter("(((role)::text = 'Manager'::text) AND ((status)::text = 'Active'::text))");

            entity.HasIndex(e => e.PhoneNumber, "ux_app_users_phone_number").IsUnique();

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("user_id");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedByUserId).HasColumnName("approved_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeactivatedAt).HasColumnName("deactivated_at");
            entity.Property(e => e.Email)
                .HasMaxLength(254)
                .HasColumnName("email");
            entity.Property(e => e.FailedLoginAttempts).HasColumnName("failed_login_attempts");
            entity.Property(e => e.FullName)
                .HasMaxLength(200)
                .HasColumnName("full_name");
            entity.Property(e => e.IdentityVerificationStatus)
                .HasMaxLength(30)
                .HasDefaultValueSql("'NotRequired'::character varying")
                .HasColumnName("identity_verification_status");
            entity.Property(e => e.IdentityVerifiedAt).HasColumnName("identity_verified_at");
            entity.Property(e => e.LastLoginAt).HasColumnName("last_login_at");
            entity.Property(e => e.LockedUntil).HasColumnName("locked_until");
            entity.Property(e => e.MustChangePassword).HasColumnName("must_change_password");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.PhoneVerifiedAt).HasColumnName("phone_verified_at");
            entity.Property(e => e.Role)
                .HasMaxLength(30)
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'PendingApproval'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.StatusReason)
                .HasMaxLength(500)
                .HasColumnName("status_reason");
            entity.Property(e => e.TransactionPinHash).HasColumnName("transaction_pin_hash");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.Company).WithOne(p => p.AppUser)
                .HasForeignKey<AppUser>(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_app_users_company");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.InverseAppUserNavigation)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ApprovedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_app_users_approved_by");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId).HasName("audit_logs_pkey");

            entity.ToTable("audit_logs", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ActorUserId, e.OccurredAt }, "ix_audit_logs_actor")
                .IsDescending(false, false, true)
                .HasFilter("(actor_user_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.OccurredAt }, "ix_audit_logs_company_time").IsDescending(false, true);

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_audit_logs_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.OccurredAt }, "ix_audit_logs_critical")
                .IsDescending(false, true)
                .HasFilter("((severity)::text = 'Critical'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.EntityType, e.EntityId, e.OccurredAt }, "ix_audit_logs_entity")
                .IsDescending(false, false, false, true)
                .HasFilter("(entity_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.EventCategory, e.EventName, e.OccurredAt }, "ix_audit_logs_event").IsDescending(false, false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.OccurredAt }, "ix_audit_logs_failures")
                .IsDescending(false, true)
                .HasFilter("((outcome)::text = ANY ((ARRAY['Failure'::character varying, 'Denied'::character varying])::text[]))");

            entity.HasIndex(e => e.Metadata, "ix_audit_logs_metadata")
                .HasMethod("gin")
                .HasOperators(new[] { "jsonb_path_ops" });

            entity.HasIndex(e => new { e.CompanyId, e.IpAddress, e.OccurredAt }, "ix_audit_logs_security_ip")
                .IsDescending(false, false, true)
                .HasFilter("(((event_category)::text = ANY ((ARRAY['Authentication'::character varying, 'Authorization'::character varying, 'Security'::character varying])::text[])) AND (ip_address IS NOT NULL))");

            entity.HasIndex(e => new { e.CompanyId, e.SessionId, e.OccurredAt }, "ix_audit_logs_session")
                .IsDescending(false, false, true)
                .HasFilter("(session_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.AuditLogId }, "uq_audit_logs_company_log").IsUnique();

            entity.HasIndex(e => e.AuditSequence, "uq_audit_logs_sequence").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.IdempotencyKey }, "ux_audit_logs_idempotency")
                .IsUnique()
                .HasFilter("(idempotency_key IS NOT NULL)");

            entity.Property(e => e.AuditLogId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("audit_log_id");
            entity.Property(e => e.ActorNameSnapshot)
                .HasMaxLength(200)
                .HasColumnName("actor_name_snapshot");
            entity.Property(e => e.ActorRoleSnapshot)
                .HasMaxLength(30)
                .HasColumnName("actor_role_snapshot");
            entity.Property(e => e.ActorType)
                .HasMaxLength(30)
                .HasColumnName("actor_type");
            entity.Property(e => e.ActorUserId).HasColumnName("actor_user_id");
            entity.Property(e => e.AfterValues)
                .HasColumnType("jsonb")
                .HasColumnName("after_values");
            entity.Property(e => e.AuditSequence)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn()
                .HasColumnName("audit_sequence");
            entity.Property(e => e.BeforeValues)
                .HasColumnType("jsonb")
                .HasColumnName("before_values");
            entity.Property(e => e.ChangedFields)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("changed_fields");
            entity.Property(e => e.ClientAppVersion)
                .HasMaxLength(50)
                .HasColumnName("client_app_version");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(1500)
                .HasColumnName("description");
            entity.Property(e => e.DeviceIdentifierHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("device_identifier_hash");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(100)
                .HasColumnName("entity_type");
            entity.Property(e => e.EntityVersionNumber).HasColumnName("entity_version_number");
            entity.Property(e => e.EventAction)
                .HasMaxLength(50)
                .HasColumnName("event_action");
            entity.Property(e => e.EventCategory)
                .HasMaxLength(40)
                .HasColumnName("event_category");
            entity.Property(e => e.EventName)
                .HasMaxLength(100)
                .HasColumnName("event_name");
            entity.Property(e => e.FailureCode)
                .HasMaxLength(100)
                .HasColumnName("failure_code");
            entity.Property(e => e.FailureMessage)
                .HasMaxLength(1500)
                .HasColumnName("failure_message");
            entity.Property(e => e.HttpStatusCode).HasColumnName("http_status_code");
            entity.Property(e => e.IdempotencyKey)
                .HasMaxLength(200)
                .HasColumnName("idempotency_key");
            entity.Property(e => e.IpAddress).HasColumnName("ip_address");
            entity.Property(e => e.Metadata)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("metadata");
            entity.Property(e => e.OccurredAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("occurred_at");
            entity.Property(e => e.Outcome)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Success'::character varying")
                .HasColumnName("outcome");
            entity.Property(e => e.RequestId)
                .HasMaxLength(150)
                .HasColumnName("request_id");
            entity.Property(e => e.RequestMethod)
                .HasMaxLength(10)
                .HasColumnName("request_method");
            entity.Property(e => e.RequestPath)
                .HasMaxLength(1000)
                .HasColumnName("request_path");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.Severity)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Information'::character varying")
                .HasColumnName("severity");
            entity.Property(e => e.SourceType)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Application'::character varying")
                .HasColumnName("source_type");
            entity.Property(e => e.UserAgent)
                .HasMaxLength(1000)
                .HasColumnName("user_agent");

            entity.HasOne(d => d.Company).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_audit_logs_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.AuditLogs)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ActorUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_audit_logs_actor");
        });

        modelBuilder.Entity<BackgroundJob>(entity =>
        {
            entity.HasKey(e => e.BackgroundJobId).HasName("background_jobs_pkey");

            entity.ToTable("background_jobs", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.CausationId }, "ix_background_jobs_causation").HasFilter("(causation_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_background_jobs_company_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_background_jobs_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.DeadLetteredAt }, "ix_background_jobs_dead_lettered")
                .IsDescending(false, true)
                .HasFilter("((status)::text = 'DeadLettered'::text)");

            entity.HasIndex(e => e.LeaseExpiresAt, "ix_background_jobs_lease").HasFilter("((status)::text = 'Processing'::text)");

            entity.HasIndex(e => new { e.QueueName, e.Priority, e.AvailableAt, e.JobSequence }, "ix_background_jobs_ready")
                .IsDescending(false, true, false, false)
                .HasFilter("((status)::text = ANY ((ARRAY['Pending'::character varying, 'Scheduled'::character varying])::text[]))");

            entity.HasIndex(e => new { e.CompanyId, e.ResourceType, e.ResourceId }, "ix_background_jobs_resource").HasFilter("(resource_id IS NOT NULL)");

            entity.HasIndex(e => new { e.NextRetryAt, e.Priority, e.JobSequence }, "ix_background_jobs_retry")
                .IsDescending(false, true, false)
                .HasFilter("(((status)::text = 'Failed'::text) AND (next_retry_at IS NOT NULL))");

            entity.HasIndex(e => new { e.CompanyId, e.JobType, e.CreatedAt }, "ix_background_jobs_type").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.BackgroundJobId }, "uq_background_jobs_company_job").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.IdempotencyKey }, "uq_background_jobs_idempotency").IsUnique();

            entity.HasIndex(e => e.JobSequence, "uq_background_jobs_sequence").IsUnique();

            entity.Property(e => e.BackgroundJobId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("background_job_id");
            entity.Property(e => e.AttemptCount).HasColumnName("attempt_count");
            entity.Property(e => e.AvailableAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("available_at");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByType)
                .HasMaxLength(30)
                .HasColumnName("cancelled_by_type");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CausationId).HasColumnName("causation_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeadLetteredAt).HasColumnName("dead_lettered_at");
            entity.Property(e => e.FailureCategory)
                .HasMaxLength(30)
                .HasColumnName("failure_category");
            entity.Property(e => e.FailureCode)
                .HasMaxLength(100)
                .HasColumnName("failure_code");
            entity.Property(e => e.FailureMessage)
                .HasMaxLength(2000)
                .HasColumnName("failure_message");
            entity.Property(e => e.Headers)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("headers");
            entity.Property(e => e.IdempotencyKey)
                .HasMaxLength(200)
                .HasColumnName("idempotency_key");
            entity.Property(e => e.JobName)
                .HasMaxLength(250)
                .HasColumnName("job_name");
            entity.Property(e => e.JobSequence)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn()
                .HasColumnName("job_sequence");
            entity.Property(e => e.JobType)
                .HasMaxLength(150)
                .HasColumnName("job_type");
            entity.Property(e => e.LastAttemptAt).HasColumnName("last_attempt_at");
            entity.Property(e => e.LastFailedAt).HasColumnName("last_failed_at");
            entity.Property(e => e.LeaseExpiresAt).HasColumnName("lease_expires_at");
            entity.Property(e => e.LockToken).HasColumnName("lock_token");
            entity.Property(e => e.LockedBy)
                .HasMaxLength(200)
                .HasColumnName("locked_by");
            entity.Property(e => e.MaxAttempts)
                .HasDefaultValue(5)
                .HasColumnName("max_attempts");
            entity.Property(e => e.NextRetryAt).HasColumnName("next_retry_at");
            entity.Property(e => e.Payload)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("payload");
            entity.Property(e => e.Priority)
                .HasDefaultValue((short)5)
                .HasColumnName("priority");
            entity.Property(e => e.ProcessingStartedAt).HasColumnName("processing_started_at");
            entity.Property(e => e.QueueName)
                .HasMaxLength(100)
                .HasDefaultValueSql("'default'::character varying")
                .HasColumnName("queue_name");
            entity.Property(e => e.ResourceId).HasColumnName("resource_id");
            entity.Property(e => e.ResourceType)
                .HasMaxLength(100)
                .HasColumnName("resource_type");
            entity.Property(e => e.ResourceVersionNumber).HasColumnName("resource_version_number");
            entity.Property(e => e.ResultPayload)
                .HasColumnType("jsonb")
                .HasColumnName("result_payload");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TimeoutSeconds)
                .HasDefaultValue(300)
                .HasColumnName("timeout_seconds");
            entity.Property(e => e.TriggeredByType)
                .HasMaxLength(30)
                .HasDefaultValueSql("'System'::character varying")
                .HasColumnName("triggered_by_type");
            entity.Property(e => e.TriggeredByUserId).HasColumnName("triggered_by_user_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.Company).WithMany(p => p.BackgroundJobs)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_background_jobs_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.BackgroundJobAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_background_jobs_cancelled_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.BackgroundJobAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.TriggeredByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_background_jobs_triggered_by");
        });

        modelBuilder.Entity<BackgroundJobAttempt>(entity =>
        {
            entity.HasKey(e => e.BackgroundJobAttemptId).HasName("background_job_attempts_pkey");

            entity.ToTable("background_job_attempts", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_background_job_attempts_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.DurationMilliseconds }, "ix_background_job_attempts_duration").IsDescending(false, true);

            entity.HasIndex(e => new { e.CompanyId, e.FailureCategory, e.CompletedAt }, "ix_background_job_attempts_failures")
                .IsDescending(false, false, true)
                .HasFilter("((attempt_status)::text = ANY ((ARRAY['Failed'::character varying, 'Abandoned'::character varying])::text[]))");

            entity.HasIndex(e => new { e.CompanyId, e.BackgroundJobId, e.AttemptNumber }, "ix_background_job_attempts_job").IsDescending(false, false, true);

            entity.HasIndex(e => e.NextRetryAt, "ix_background_job_attempts_retry").HasFilter("(((retry_decision)::text = 'Retry'::text) AND (next_retry_at IS NOT NULL))");

            entity.HasIndex(e => new { e.CompanyId, e.AttemptStatus, e.CompletedAt }, "ix_background_job_attempts_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.WorkerName, e.CompletedAt }, "ix_background_job_attempts_worker").IsDescending(false, true);

            entity.HasIndex(e => new { e.CompanyId, e.BackgroundJobAttemptId }, "uq_background_job_attempts_company_attempt").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.BackgroundJobId, e.LockToken }, "uq_background_job_attempts_job_lock").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.BackgroundJobId, e.AttemptNumber }, "uq_background_job_attempts_job_number").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.BackgroundJobId, e.BackgroundJobAttemptId }, "ux_bg_job_attempts_job_attempt_ref").IsUnique();

            entity.Property(e => e.BackgroundJobAttemptId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("background_job_attempt_id");
            entity.Property(e => e.AttemptNumber).HasColumnName("attempt_number");
            entity.Property(e => e.AttemptStatus)
                .HasMaxLength(20)
                .HasColumnName("attempt_status");
            entity.Property(e => e.BackgroundJobId).HasColumnName("background_job_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DurationMilliseconds).HasColumnName("duration_milliseconds");
            entity.Property(e => e.ExecutionMetadata)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("execution_metadata");
            entity.Property(e => e.FailureCategory)
                .HasMaxLength(30)
                .HasColumnName("failure_category");
            entity.Property(e => e.FailureCode)
                .HasMaxLength(100)
                .HasColumnName("failure_code");
            entity.Property(e => e.FailureMessage)
                .HasMaxLength(2000)
                .HasColumnName("failure_message");
            entity.Property(e => e.LockToken).HasColumnName("lock_token");
            entity.Property(e => e.NextRetryAt).HasColumnName("next_retry_at");
            entity.Property(e => e.ResultPayload)
                .HasColumnType("jsonb")
                .HasColumnName("result_payload");
            entity.Property(e => e.RetryDecision)
                .HasMaxLength(20)
                .HasColumnName("retry_decision");
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
            entity.Property(e => e.WorkerName)
                .HasMaxLength(200)
                .HasColumnName("worker_name");

            entity.HasOne(d => d.BackgroundJob).WithMany(p => p.BackgroundJobAttempts)
                .HasPrincipalKey(p => new { p.CompanyId, p.BackgroundJobId })
                .HasForeignKey(d => new { d.CompanyId, d.BackgroundJobId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_background_job_attempts_job");
        });

        modelBuilder.Entity<BalanceLedgerEntry>(entity =>
        {
            entity.HasKey(e => e.BalanceLedgerEntryId).HasName("balance_ledger_entries_pkey");

            entity.ToTable("balance_ledger_entries", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.UserAdvanceBalanceId, e.BalanceVersionNumber }, "ix_balance_ledger_entries_balance");

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_balance_ledger_entries_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.OccurredAt }, "ix_balance_ledger_entries_occurred_at");

            entity.HasIndex(e => new { e.CompanyId, e.ReferenceType, e.ReferenceId }, "ix_balance_ledger_entries_reference").HasFilter("(reference_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.EntryType }, "ix_balance_ledger_entries_type");

            entity.HasIndex(e => new { e.CompanyId, e.UserAdvanceBalanceId, e.BalanceVersionNumber }, "uq_balance_ledger_entries_balance_version").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.BalanceLedgerEntryId }, "uq_balance_ledger_entries_company_entry").IsUnique();

            entity.Property(e => e.BalanceLedgerEntryId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("balance_ledger_entry_id");
            entity.Property(e => e.AdjustmentOutAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("adjustment_out_after_amount");
            entity.Property(e => e.AvailableAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("available_after_amount");
            entity.Property(e => e.BalanceVersionNumber).HasColumnName("balance_version_number");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeltaAdjustmentOutAmount)
                .HasPrecision(18, 2)
                .HasColumnName("delta_adjustment_out_amount");
            entity.Property(e => e.DeltaAvailableAmount)
                .HasPrecision(18, 2)
                .HasColumnName("delta_available_amount");
            entity.Property(e => e.DeltaExpensedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("delta_expensed_amount");
            entity.Property(e => e.DeltaReceivedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("delta_received_amount");
            entity.Property(e => e.DeltaReservedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("delta_reserved_amount");
            entity.Property(e => e.DeltaRestoredAmount)
                .HasPrecision(18, 2)
                .HasColumnName("delta_restored_amount");
            entity.Property(e => e.DeltaReturnedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("delta_returned_amount");
            entity.Property(e => e.DeltaTransferredOutAmount)
                .HasPrecision(18, 2)
                .HasColumnName("delta_transferred_out_amount");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.EntryType)
                .HasMaxLength(50)
                .HasColumnName("entry_type");
            entity.Property(e => e.ExpensedAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("expensed_after_amount");
            entity.Property(e => e.OccurredAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("occurred_at");
            entity.Property(e => e.PerformedByUserId).HasColumnName("performed_by_user_id");
            entity.Property(e => e.ReceivedAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("received_after_amount");
            entity.Property(e => e.ReferenceId).HasColumnName("reference_id");
            entity.Property(e => e.ReferenceType)
                .HasMaxLength(40)
                .HasColumnName("reference_type");
            entity.Property(e => e.ReservedAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("reserved_after_amount");
            entity.Property(e => e.RestoredAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("restored_after_amount");
            entity.Property(e => e.ReturnedAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("returned_after_amount");
            entity.Property(e => e.TransferredOutAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("transferred_out_after_amount");
            entity.Property(e => e.UserAdvanceBalanceId).HasColumnName("user_advance_balance_id");

            entity.HasOne(d => d.AppUser).WithMany(p => p.BalanceLedgerEntries)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.PerformedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_balance_ledger_entries_performed_by");

            entity.HasOne(d => d.UserAdvanceBalance).WithMany(p => p.BalanceLedgerEntries)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserAdvanceBalanceId })
                .HasForeignKey(d => new { d.CompanyId, d.UserAdvanceBalanceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_balance_ledger_entries_balance");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("companies_pkey");

            entity.ToTable("companies", "ahdah");

            entity.Property(e => e.CompanyId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("company_id");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.CompanyCode)
                .HasMaxLength(20)
                .HasColumnName("company_code");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(200)
                .HasColumnName("company_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(254)
                .HasColumnName("email");
            entity.Property(e => e.LogoUrl)
                .HasMaxLength(1000)
                .HasColumnName("logo_url");
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .HasColumnName("phone");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'PendingSetup'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");
        });

        modelBuilder.Entity<CompanyCashboxEntry>(entity =>
        {
            entity.HasKey(e => e.CompanyCashboxEntryId).HasName("company_cashbox_entries_pkey");

            entity.ToTable("company_cashbox_entries", "ahdah");

            entity.HasIndex(e => e.CompanyId, "ix_company_cashbox_entries_company");

            entity.HasIndex(e => new { e.CompanyId, e.EntryDate }, "ix_company_cashbox_entries_date");

            entity.HasIndex(e => new { e.CompanyId, e.RecordedByUserId }, "ix_company_cashbox_entries_recorded_by");

            entity.HasIndex(e => new { e.CompanyId, e.Status }, "ix_company_cashbox_entries_status");

            entity.HasIndex(e => new { e.CompanyId, e.EntryType }, "ix_company_cashbox_entries_type");

            entity.HasIndex(e => new { e.CompanyId, e.CompanyCashboxEntryId }, "uq_company_cashbox_entries_company_entry").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourceId }, "uq_company_cashbox_entries_funding_source").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.CashboxEntryNumber }, "uq_company_cashbox_entries_number").IsUnique();

            entity.Property(e => e.CompanyCashboxEntryId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("company_cashbox_entry_id");
            entity.Property(e => e.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("amount");
            entity.Property(e => e.BankName)
                .HasMaxLength(150)
                .HasColumnName("bank_name");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CashboxEntryNumber)
                .HasMaxLength(50)
                .HasColumnName("cashbox_entry_number");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.EntryDate).HasColumnName("entry_date");
            entity.Property(e => e.EntryType)
                .HasMaxLength(40)
                .HasColumnName("entry_type");
            entity.Property(e => e.FundingSourceId).HasColumnName("funding_source_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.ProofFileUrl)
                .HasMaxLength(1000)
                .HasColumnName("proof_file_url");
            entity.Property(e => e.RecordedByUserId).HasColumnName("recorded_by_user_id");
            entity.Property(e => e.ReferenceNumber)
                .HasMaxLength(150)
                .HasColumnName("reference_number");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'PendingVerification'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VerifiedAt).HasColumnName("verified_at");
            entity.Property(e => e.VerifiedByUserId).HasColumnName("verified_by_user_id");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.CompanyCashboxEntryAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_company_cashbox_entries_cancelled_by");

            entity.HasOne(d => d.FundingSource).WithOne(p => p.CompanyCashboxEntry)
                .HasPrincipalKey<FundingSource>(p => new { p.CompanyId, p.FundingSourceId })
                .HasForeignKey<CompanyCashboxEntry>(d => new { d.CompanyId, d.FundingSourceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_company_cashbox_entries_funding_source");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.CompanyCashboxEntryAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecordedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_company_cashbox_entries_recorded_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.CompanyCashboxEntryAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_company_cashbox_entries_reversed_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.CompanyCashboxEntryAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.VerifiedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_company_cashbox_entries_verified_by");
        });

        modelBuilder.Entity<CompanyNumberSequence>(entity =>
        {
            entity.HasKey(e => e.CompanyNumberSequenceId).HasName("company_number_sequences_pkey");

            entity.ToTable("company_number_sequences", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.SequenceCode }, "ix_company_number_sequences_active").HasFilter("((status)::text = 'Active'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.EntityType }, "ix_company_number_sequences_entity");

            entity.HasIndex(e => new { e.CompanyId, e.LastIssuedAt }, "ix_company_number_sequences_last_issue")
                .IsDescending(false, true)
                .HasFilter("(last_issued_at IS NOT NULL)");

            entity.HasIndex(e => e.NextResetAt, "ix_company_number_sequences_reset").HasFilter("(((status)::text = 'Active'::text) AND (next_reset_at IS NOT NULL))");

            entity.HasIndex(e => new { e.CompanyId, e.UpdatedByUserId }, "ix_company_number_sequences_updated_by");

            entity.HasIndex(e => new { e.CompanyId, e.SequenceCode }, "uq_company_number_sequences_code").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.CompanyNumberSequenceId }, "uq_company_number_sequences_company_sequence").IsUnique();

            entity.Property(e => e.CompanyNumberSequenceId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("company_number_sequence_id");
            entity.Property(e => e.ActivatedAt).HasColumnName("activated_at");
            entity.Property(e => e.ActivatedByUserId).HasColumnName("activated_by_user_id");
            entity.Property(e => e.AllocationMode)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Transactional'::character varying")
                .HasColumnName("allocation_mode");
            entity.Property(e => e.AllowCycle).HasColumnName("allow_cycle");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CurrentPeriodKey)
                .HasMaxLength(30)
                .HasColumnName("current_period_key");
            entity.Property(e => e.CurrentPeriodStartedAt).HasColumnName("current_period_started_at");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.EntityType)
                .HasMaxLength(100)
                .HasColumnName("entity_type");
            entity.Property(e => e.FormatTemplate)
                .HasMaxLength(200)
                .HasColumnName("format_template");
            entity.Property(e => e.IncrementBy)
                .HasDefaultValue(1)
                .HasColumnName("increment_by");
            entity.Property(e => e.LastAllocationId).HasColumnName("last_allocation_id");
            entity.Property(e => e.LastIssuedAt).HasColumnName("last_issued_at");
            entity.Property(e => e.LastIssuedByUserId).HasColumnName("last_issued_by_user_id");
            entity.Property(e => e.LastIssuedNumber)
                .HasMaxLength(200)
                .HasColumnName("last_issued_number");
            entity.Property(e => e.LastValue).HasColumnName("last_value");
            entity.Property(e => e.MaximumValue).HasColumnName("maximum_value");
            entity.Property(e => e.NextResetAt).HasColumnName("next_reset_at");
            entity.Property(e => e.PaddingLength)
                .HasDefaultValue((short)6)
                .HasColumnName("padding_length");
            entity.Property(e => e.PauseReason)
                .HasMaxLength(500)
                .HasColumnName("pause_reason");
            entity.Property(e => e.PausedAt).HasColumnName("paused_at");
            entity.Property(e => e.PausedByUserId).HasColumnName("paused_by_user_id");
            entity.Property(e => e.ResetPolicy)
                .HasMaxLength(30)
                .HasDefaultValueSql("'CalendarYear'::character varying")
                .HasColumnName("reset_policy");
            entity.Property(e => e.RetiredAt).HasColumnName("retired_at");
            entity.Property(e => e.RetiredByUserId).HasColumnName("retired_by_user_id");
            entity.Property(e => e.RetirementReason)
                .HasMaxLength(500)
                .HasColumnName("retirement_reason");
            entity.Property(e => e.SequenceCode)
                .HasMaxLength(100)
                .HasColumnName("sequence_code");
            entity.Property(e => e.SequenceName)
                .HasMaxLength(200)
                .HasColumnName("sequence_name");
            entity.Property(e => e.StartValue)
                .HasDefaultValue(1L)
                .HasColumnName("start_value");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedByUserId).HasColumnName("updated_by_user_id");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyNumberSequences)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_company_number_sequences_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.CompanyNumberSequenceAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ActivatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_company_number_sequences_activated_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.CompanyNumberSequenceAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_company_number_sequences_created_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.CompanyNumberSequenceAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.LastIssuedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_company_number_sequences_last_issued_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.CompanyNumberSequenceAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.PausedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_company_number_sequences_paused_by");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.CompanyNumberSequenceAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RetiredByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_company_number_sequences_retired_by");

            entity.HasOne(d => d.AppUser4).WithMany(p => p.CompanyNumberSequenceAppUser4s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.UpdatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_company_number_sequences_updated_by");
        });

        modelBuilder.Entity<CompanyNumberSequenceAllocation>(entity =>
        {
            entity.HasKey(e => e.CompanyNumberSequenceAllocationId).HasName("company_number_sequence_allocations_pkey");

            entity.ToTable("company_number_sequence_allocations", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.AllocatedByUserId, e.CreatedAt }, "ix_number_allocations_allocated_by")
                .IsDescending(false, false, true)
                .HasFilter("(allocated_by_user_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_number_allocations_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.EntityType, e.EntityId }, "ix_number_allocations_entity_lookup").HasFilter("(entity_id IS NOT NULL)");

            entity.HasIndex(e => e.ReservationExpiresAt, "ix_number_allocations_expiring").HasFilter("((allocation_status)::text = 'Reserved'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.IssuedAt }, "ix_number_allocations_issued")
                .IsDescending(false, true)
                .HasFilter("((allocation_status)::text = ANY ((ARRAY['Issued'::character varying, 'Voided'::character varying])::text[]))");

            entity.HasIndex(e => new { e.CompanyId, e.CompanyNumberSequenceId, e.PeriodKey, e.AllocatedValue }, "ix_number_allocations_sequence").IsDescending(false, false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.AllocationStatus, e.CreatedAt }, "ix_number_allocations_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.CompanyNumberSequenceAllocationId }, "uq_number_allocations_company_allocation").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.CompanyNumberSequenceId, e.FormattedNumber }, "uq_number_allocations_formatted_number").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.IdempotencyKey }, "uq_number_allocations_idempotency").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.CompanyNumberSequenceId, e.PeriodKey, e.AllocatedValue }, "uq_number_allocations_sequence_value").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.CompanyNumberSequenceId, e.EntityType, e.EntityId }, "ux_number_allocations_entity")
                .IsUnique()
                .HasFilter("((entity_id IS NOT NULL) AND ((allocation_status)::text = ANY ((ARRAY['Issued'::character varying, 'Voided'::character varying])::text[])))");

            entity.HasIndex(e => new { e.CompanyId, e.ReservationToken }, "ux_number_allocations_reservation_token")
                .IsUnique()
                .HasFilter("(reservation_token IS NOT NULL)");

            entity.Property(e => e.CompanyNumberSequenceAllocationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("company_number_sequence_allocation_id");
            entity.Property(e => e.AllocatedByType)
                .HasMaxLength(30)
                .HasDefaultValueSql("'User'::character varying")
                .HasColumnName("allocated_by_type");
            entity.Property(e => e.AllocatedByUserId).HasColumnName("allocated_by_user_id");
            entity.Property(e => e.AllocatedValue).HasColumnName("allocated_value");
            entity.Property(e => e.AllocationMode)
                .HasMaxLength(30)
                .HasColumnName("allocation_mode");
            entity.Property(e => e.AllocationStatus)
                .HasMaxLength(20)
                .HasColumnName("allocation_status");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CompanyNumberSequenceId).HasColumnName("company_number_sequence_id");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(100)
                .HasColumnName("entity_type");
            entity.Property(e => e.EntityVersionNumber).HasColumnName("entity_version_number");
            entity.Property(e => e.ExpiredAt).HasColumnName("expired_at");
            entity.Property(e => e.FormattedNumber)
                .HasMaxLength(200)
                .HasColumnName("formatted_number");
            entity.Property(e => e.IdempotencyKey)
                .HasMaxLength(200)
                .HasColumnName("idempotency_key");
            entity.Property(e => e.IssuedAt).HasColumnName("issued_at");
            entity.Property(e => e.PeriodKey)
                .HasMaxLength(30)
                .HasColumnName("period_key");
            entity.Property(e => e.ReleaseReason)
                .HasMaxLength(500)
                .HasColumnName("release_reason");
            entity.Property(e => e.ReleasedAt).HasColumnName("released_at");
            entity.Property(e => e.ReleasedByUserId).HasColumnName("released_by_user_id");
            entity.Property(e => e.ReservationExpiresAt).HasColumnName("reservation_expires_at");
            entity.Property(e => e.ReservationToken).HasColumnName("reservation_token");
            entity.Property(e => e.ReservedAt).HasColumnName("reserved_at");
            entity.Property(e => e.ResetPolicySnapshot)
                .HasMaxLength(30)
                .HasColumnName("reset_policy_snapshot");
            entity.Property(e => e.SequenceCodeSnapshot)
                .HasMaxLength(100)
                .HasColumnName("sequence_code_snapshot");
            entity.Property(e => e.SequenceVersionNumber).HasColumnName("sequence_version_number");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VoidReason)
                .HasMaxLength(500)
                .HasColumnName("void_reason");
            entity.Property(e => e.VoidedAt).HasColumnName("voided_at");
            entity.Property(e => e.VoidedByUserId).HasColumnName("voided_by_user_id");

            entity.HasOne(d => d.AppUser).WithMany(p => p.CompanyNumberSequenceAllocationAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.AllocatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_number_allocations_allocated_by");

            entity.HasOne(d => d.CompanyNumberSequence).WithMany(p => p.CompanyNumberSequenceAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.CompanyNumberSequenceId })
                .HasForeignKey(d => new { d.CompanyId, d.CompanyNumberSequenceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_number_allocations_sequence");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.CompanyNumberSequenceAllocationAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReleasedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_number_allocations_released_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.CompanyNumberSequenceAllocationAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.VoidedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_number_allocations_voided_by");
        });

        modelBuilder.Entity<CompanyNumberSequenceVersion>(entity =>
        {
            entity.HasKey(e => e.CompanyNumberSequenceVersionId).HasName("company_number_sequence_versions_pkey");

            entity.ToTable("company_number_sequence_versions", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ChangeAllocationId }, "ix_number_sequence_versions_change_allocation").HasFilter("(change_allocation_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.ChangeType, e.CreatedAt }, "ix_number_sequence_versions_change_type").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.SequenceCodeSnapshot, e.VersionNumber }, "ix_number_sequence_versions_code").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_number_sequence_versions_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.LastAllocationIdSnapshot }, "ix_number_sequence_versions_last_allocation").HasFilter("(last_allocation_id_snapshot IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.CompanyNumberSequenceId, e.CurrentPeriodKeySnapshot }, "ix_number_sequence_versions_period").HasFilter("(current_period_key_snapshot IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.RecordedByUserId, e.CreatedAt }, "ix_number_sequence_versions_recorded_by")
                .IsDescending(false, false, true)
                .HasFilter("(recorded_by_user_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.CompanyNumberSequenceId, e.VersionNumber }, "ix_number_sequence_versions_sequence").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.StatusSnapshot, e.CreatedAt }, "ix_number_sequence_versions_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.CompanyNumberSequenceVersionId }, "uq_number_sequence_versions_company_version").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.CompanyNumberSequenceId, e.VersionNumber }, "uq_number_sequence_versions_sequence_version").IsUnique();

            entity.Property(e => e.CompanyNumberSequenceVersionId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("company_number_sequence_version_id");
            entity.Property(e => e.AllocationModeSnapshot)
                .HasMaxLength(30)
                .HasColumnName("allocation_mode_snapshot");
            entity.Property(e => e.AllowCycleSnapshot).HasColumnName("allow_cycle_snapshot");
            entity.Property(e => e.ChangeAllocationId).HasColumnName("change_allocation_id");
            entity.Property(e => e.ChangeSummary)
                .HasMaxLength(1000)
                .HasColumnName("change_summary");
            entity.Property(e => e.ChangeType)
                .HasMaxLength(40)
                .HasColumnName("change_type");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CompanyNumberSequenceId).HasColumnName("company_number_sequence_id");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrentPeriodKeySnapshot)
                .HasMaxLength(30)
                .HasColumnName("current_period_key_snapshot");
            entity.Property(e => e.CurrentPeriodStartedAtSnapshot).HasColumnName("current_period_started_at_snapshot");
            entity.Property(e => e.DescriptionSnapshot)
                .HasMaxLength(1000)
                .HasColumnName("description_snapshot");
            entity.Property(e => e.EntityTypeSnapshot)
                .HasMaxLength(100)
                .HasColumnName("entity_type_snapshot");
            entity.Property(e => e.FormatTemplateSnapshot)
                .HasMaxLength(200)
                .HasColumnName("format_template_snapshot");
            entity.Property(e => e.IncrementBySnapshot).HasColumnName("increment_by_snapshot");
            entity.Property(e => e.LastAllocationIdSnapshot).HasColumnName("last_allocation_id_snapshot");
            entity.Property(e => e.LastIssuedAtSnapshot).HasColumnName("last_issued_at_snapshot");
            entity.Property(e => e.LastIssuedByUserIdSnapshot).HasColumnName("last_issued_by_user_id_snapshot");
            entity.Property(e => e.LastIssuedNumberSnapshot)
                .HasMaxLength(200)
                .HasColumnName("last_issued_number_snapshot");
            entity.Property(e => e.LastValueSnapshot).HasColumnName("last_value_snapshot");
            entity.Property(e => e.MaximumValueSnapshot).HasColumnName("maximum_value_snapshot");
            entity.Property(e => e.NextResetAtSnapshot).HasColumnName("next_reset_at_snapshot");
            entity.Property(e => e.PaddingLengthSnapshot).HasColumnName("padding_length_snapshot");
            entity.Property(e => e.PreviousVersionNumber).HasColumnName("previous_version_number");
            entity.Property(e => e.RecordedByType)
                .HasMaxLength(30)
                .HasDefaultValueSql("'User'::character varying")
                .HasColumnName("recorded_by_type");
            entity.Property(e => e.RecordedByUserId).HasColumnName("recorded_by_user_id");
            entity.Property(e => e.ResetPolicySnapshot)
                .HasMaxLength(30)
                .HasColumnName("reset_policy_snapshot");
            entity.Property(e => e.RestoredFromVersionNumber).HasColumnName("restored_from_version_number");
            entity.Property(e => e.SequenceCodeSnapshot)
                .HasMaxLength(100)
                .HasColumnName("sequence_code_snapshot");
            entity.Property(e => e.SequenceNameSnapshot)
                .HasMaxLength(200)
                .HasColumnName("sequence_name_snapshot");
            entity.Property(e => e.SourceSequenceUpdatedAt).HasColumnName("source_sequence_updated_at");
            entity.Property(e => e.StartValueSnapshot).HasColumnName("start_value_snapshot");
            entity.Property(e => e.StatusSnapshot)
                .HasMaxLength(20)
                .HasColumnName("status_snapshot");
            entity.Property(e => e.VersionNumber).HasColumnName("version_number");

            entity.HasOne(d => d.CompanyNumberSequenceAllocation).WithMany(p => p.CompanyNumberSequenceVersionCompanyNumberSequenceAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.CompanyNumberSequenceAllocationId })
                .HasForeignKey(d => new { d.CompanyId, d.ChangeAllocationId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_number_sequence_versions_change_allocation");

            entity.HasOne(d => d.CompanyNumberSequence).WithMany(p => p.CompanyNumberSequenceVersions)
                .HasPrincipalKey(p => new { p.CompanyId, p.CompanyNumberSequenceId })
                .HasForeignKey(d => new { d.CompanyId, d.CompanyNumberSequenceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_number_sequence_versions_sequence");

            entity.HasOne(d => d.CompanyNumberSequenceAllocationNavigation).WithMany(p => p.CompanyNumberSequenceVersionCompanyNumberSequenceAllocationNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.CompanyNumberSequenceAllocationId })
                .HasForeignKey(d => new { d.CompanyId, d.LastAllocationIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_number_sequence_versions_last_allocation");

            entity.HasOne(d => d.AppUser).WithMany(p => p.CompanyNumberSequenceVersionAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.LastIssuedByUserIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_number_sequence_versions_last_issued_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.CompanyNumberSequenceVersionAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecordedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_number_sequence_versions_recorded_by");

            entity.HasOne(d => d.CompanyNumberSequenceVersionNavigation).WithMany(p => p.InverseCompanyNumberSequenceVersionNavigation)
                .HasPrincipalKey(p => new { p.CompanyId, p.CompanyNumberSequenceId, p.VersionNumber })
                .HasForeignKey(d => new { d.CompanyId, d.CompanyNumberSequenceId, d.PreviousVersionNumber })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_number_sequence_versions_previous");

            entity.HasOne(d => d.CompanyNumberSequenceVersion1).WithMany(p => p.InverseCompanyNumberSequenceVersion1)
                .HasPrincipalKey(p => new { p.CompanyId, p.CompanyNumberSequenceId, p.VersionNumber })
                .HasForeignKey(d => new { d.CompanyId, d.CompanyNumberSequenceId, d.RestoredFromVersionNumber })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_number_sequence_versions_restored_from");
        });

        modelBuilder.Entity<CompanySetting>(entity =>
        {
            entity.HasKey(e => e.CompanySettingId).HasName("company_settings_pkey");

            entity.ToTable("company_settings", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.IsActive }, "ix_company_settings_active");

            entity.HasIndex(e => e.AdditionalSettings, "ix_company_settings_additional")
                .HasMethod("gin")
                .HasOperators(new[] { "jsonb_path_ops" });

            entity.HasIndex(e => new { e.CompanyId, e.UpdatedByUserId }, "ix_company_settings_updated_by");

            entity.HasIndex(e => e.CompanyId, "uq_company_settings_company").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.CompanySettingId }, "uq_company_settings_company_setting").IsUnique();

            entity.Property(e => e.CompanySettingId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("company_setting_id");
            entity.Property(e => e.AccountLockoutMinutes)
                .HasDefaultValue(30)
                .HasColumnName("account_lockout_minutes");
            entity.Property(e => e.AdditionalSettings)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("additional_settings");
            entity.Property(e => e.AdvanceApprovalMode)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Always'::character varying")
                .HasColumnName("advance_approval_mode");
            entity.Property(e => e.AdvanceApprovalThresholdAmount)
                .HasPrecision(18, 2)
                .HasColumnName("advance_approval_threshold_amount");
            entity.Property(e => e.AllowFinancialOverallocation).HasColumnName("allow_financial_overallocation");
            entity.Property(e => e.AllowMultiCurrency).HasColumnName("allow_multi_currency");
            entity.Property(e => e.AllowNegativeAdvanceBalance).HasColumnName("allow_negative_advance_balance");
            entity.Property(e => e.AllowSelfApproval).HasColumnName("allow_self_approval");
            entity.Property(e => e.AllowedFileExtensions)
                .HasDefaultValueSql("'[\".jpg\", \".jpeg\", \".png\", \".pdf\"]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("allowed_file_extensions");
            entity.Property(e => e.AllowedMimeTypes)
                .HasDefaultValueSql("'[\"image/jpeg\", \"image/png\", \"application/pdf\"]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("allowed_mime_types");
            entity.Property(e => e.AuditLogRetentionDays)
                .HasDefaultValue(2555)
                .HasColumnName("audit_log_retention_days");
            entity.Property(e => e.BackgroundJobRetentionDays)
                .HasDefaultValue(90)
                .HasColumnName("background_job_retention_days");
            entity.Property(e => e.BalanceToleranceAmount)
                .HasPrecision(18, 2)
                .HasColumnName("balance_tolerance_amount");
            entity.Property(e => e.ClosureRequiresApproval)
                .HasDefaultValue(true)
                .HasColumnName("closure_requires_approval");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.DeMinimisWriteOffLimit)
                .HasPrecision(18, 2)
                .HasColumnName("de_minimis_write_off_limit");
            entity.Property(e => e.DefaultAdvanceDueDays)
                .HasDefaultValue(30)
                .HasColumnName("default_advance_due_days");
            entity.Property(e => e.DefaultCurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'LYD'::character varying")
                .HasColumnName("default_currency_code");
            entity.Property(e => e.DefaultLocaleCode)
                .HasMaxLength(10)
                .HasDefaultValueSql("'ar-LY'::character varying")
                .HasColumnName("default_locale_code");
            entity.Property(e => e.DefaultNotificationChannels)
                .HasDefaultValueSql("'[\"InApp\", \"Push\"]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("default_notification_channels");
            entity.Property(e => e.DefaultPersonalClaimDueDays)
                .HasDefaultValue(30)
                .HasColumnName("default_personal_claim_due_days");
            entity.Property(e => e.DefaultSupplierDebtDueDays)
                .HasDefaultValue(30)
                .HasColumnName("default_supplier_debt_due_days");
            entity.Property(e => e.ExpenseApprovalMode)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Always'::character varying")
                .HasColumnName("expense_approval_mode");
            entity.Property(e => e.ExpenseApprovalThresholdAmount)
                .HasPrecision(18, 2)
                .HasColumnName("expense_approval_threshold_amount");
            entity.Property(e => e.ExpenseDocumentMode)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Threshold'::character varying")
                .HasColumnName("expense_document_mode");
            entity.Property(e => e.ExpenseDocumentThresholdAmount)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .HasColumnName("expense_document_threshold_amount");
            entity.Property(e => e.FirstDayOfWeek)
                .HasDefaultValue((short)6)
                .HasColumnName("first_day_of_week");
            entity.Property(e => e.FiscalYearStartMonth)
                .HasDefaultValue((short)1)
                .HasColumnName("fiscal_year_start_month");
            entity.Property(e => e.IdempotencyRetentionHours)
                .HasDefaultValue(24)
                .HasColumnName("idempotency_retention_hours");
            entity.Property(e => e.InvitationExpiryHours)
                .HasDefaultValue(72)
                .HasColumnName("invitation_expiry_hours");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.JoinRequestExpiryDays)
                .HasDefaultValue(30)
                .HasColumnName("join_request_expiry_days");
            entity.Property(e => e.MaxDocumentSizeBytes)
                .HasDefaultValue(10485760L)
                .HasColumnName("max_document_size_bytes");
            entity.Property(e => e.MaxSingleAdvanceAmount)
                .HasPrecision(18, 2)
                .HasColumnName("max_single_advance_amount");
            entity.Property(e => e.MaxSingleExpenseAmount)
                .HasPrecision(18, 2)
                .HasColumnName("max_single_expense_amount");
            entity.Property(e => e.MaxSinglePersonalClaimPaymentAmount)
                .HasPrecision(18, 2)
                .HasColumnName("max_single_personal_claim_payment_amount");
            entity.Property(e => e.MaxSingleSupplierPaymentAmount)
                .HasPrecision(18, 2)
                .HasColumnName("max_single_supplier_payment_amount");
            entity.Property(e => e.MaxSingleTransferAmount)
                .HasPrecision(18, 2)
                .HasColumnName("max_single_transfer_amount");
            entity.Property(e => e.MaximumFailedLoginAttempts)
                .HasDefaultValue(5)
                .HasColumnName("maximum_failed_login_attempts");
            entity.Property(e => e.MoneyDecimalPlaces)
                .HasDefaultValue((short)2)
                .HasColumnName("money_decimal_places");
            entity.Property(e => e.NotificationRetentionDays)
                .HasDefaultValue(365)
                .HasColumnName("notification_retention_days");
            entity.Property(e => e.OutboxRetentionDays)
                .HasDefaultValue(90)
                .HasColumnName("outbox_retention_days");
            entity.Property(e => e.PaymentProofMode)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Threshold'::character varying")
                .HasColumnName("payment_proof_mode");
            entity.Property(e => e.PaymentProofThresholdAmount)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .HasColumnName("payment_proof_threshold_amount");
            entity.Property(e => e.PersonalClaimPaymentApprovalMode)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Always'::character varying")
                .HasColumnName("personal_claim_payment_approval_mode");
            entity.Property(e => e.PersonalClaimPaymentApprovalThresholdAmount)
                .HasPrecision(18, 2)
                .HasColumnName("personal_claim_payment_approval_threshold_amount");
            entity.Property(e => e.QuantityDecimalPlaces)
                .HasDefaultValue((short)3)
                .HasColumnName("quantity_decimal_places");
            entity.Property(e => e.RequireDistinctCreatorApprover)
                .HasDefaultValue(true)
                .HasColumnName("require_distinct_creator_approver");
            entity.Property(e => e.RequireDistinctSubmitterApprover)
                .HasDefaultValue(true)
                .HasColumnName("require_distinct_submitter_approver");
            entity.Property(e => e.RequireMfaForAccountant).HasColumnName("require_mfa_for_accountant");
            entity.Property(e => e.RequireMfaForDeputy).HasColumnName("require_mfa_for_deputy");
            entity.Property(e => e.RequireMfaForManager).HasColumnName("require_mfa_for_manager");
            entity.Property(e => e.RoundingMode)
                .HasMaxLength(30)
                .HasDefaultValueSql("'HalfUp'::character varying")
                .HasColumnName("rounding_mode");
            entity.Property(e => e.SessionTimeoutMinutes)
                .HasDefaultValue(120)
                .HasColumnName("session_timeout_minutes");
            entity.Property(e => e.SettlementRequiresApproval)
                .HasDefaultValue(true)
                .HasColumnName("settlement_requires_approval");
            entity.Property(e => e.SupplierPaymentApprovalMode)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Always'::character varying")
                .HasColumnName("supplier_payment_approval_mode");
            entity.Property(e => e.SupplierPaymentApprovalThresholdAmount)
                .HasPrecision(18, 2)
                .HasColumnName("supplier_payment_approval_threshold_amount");
            entity.Property(e => e.TimeZone)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Africa/Tripoli'::character varying")
                .HasColumnName("time_zone");
            entity.Property(e => e.TransferApprovalMode)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Always'::character varying")
                .HasColumnName("transfer_approval_mode");
            entity.Property(e => e.TransferApprovalThresholdAmount)
                .HasPrecision(18, 2)
                .HasColumnName("transfer_approval_threshold_amount");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedByUserId).HasColumnName("updated_by_user_id");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.Company).WithOne(p => p.CompanySetting)
                .HasForeignKey<CompanySetting>(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_company_settings_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.CompanySettingAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_company_settings_created_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.CompanySettingAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.UpdatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_company_settings_updated_by");
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.ExpenseId).HasName("expenses_pkey");

            entity.ToTable("expenses", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseCategoryId }, "ix_expenses_category");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseDate }, "ix_expenses_date");

            entity.HasIndex(e => new { e.CompanyId, e.IncurredByUserId }, "ix_expenses_incurred_by");

            entity.HasIndex(e => new { e.CompanyId, e.PaymentMode }, "ix_expenses_payment_mode");

            entity.HasIndex(e => new { e.CompanyId, e.SubmittedAt }, "ix_expenses_pending_review").HasFilter("((status)::text = 'PendingReview'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectId }, "ix_expenses_project").HasFilter("(project_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_expenses_status");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierId }, "ix_expenses_supplier").HasFilter("(supplier_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseId }, "uq_expenses_company_expense").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseNumber }, "uq_expenses_number").IsUnique();

            entity.Property(e => e.ExpenseId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("expense_id");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CorrectionReason)
                .HasMaxLength(1000)
                .HasColumnName("correction_reason");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreditDueDate).HasColumnName("credit_due_date");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'LYD'::character varying")
                .HasColumnName("currency_code");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.DiscountAmount)
                .HasPrecision(18, 2)
                .HasColumnName("discount_amount");
            entity.Property(e => e.ExpenseCategoryId).HasColumnName("expense_category_id");
            entity.Property(e => e.ExpenseDate).HasColumnName("expense_date");
            entity.Property(e => e.ExpenseLocation)
                .HasMaxLength(500)
                .HasColumnName("expense_location");
            entity.Property(e => e.ExpenseNumber)
                .HasMaxLength(50)
                .HasColumnName("expense_number");
            entity.Property(e => e.IncurredByUserId).HasColumnName("incurred_by_user_id");
            entity.Property(e => e.InvoiceNumber)
                .HasMaxLength(100)
                .HasColumnName("invoice_number");
            entity.Property(e => e.MerchantName)
                .HasMaxLength(200)
                .HasColumnName("merchant_name");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.PaymentMode)
                .HasMaxLength(30)
                .HasColumnName("payment_mode");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.ReceiptNumber)
                .HasMaxLength(100)
                .HasColumnName("receipt_number");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(1000)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.ReviewedAt).HasColumnName("reviewed_at");
            entity.Property(e => e.ReviewedByUserId).HasColumnName("reviewed_by_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SubmittedAt).HasColumnName("submitted_at");
            entity.Property(e => e.SubmittedByUserId).HasColumnName("submitted_by_user_id");
            entity.Property(e => e.SubtotalAmount)
                .HasPrecision(18, 2)
                .HasColumnName("subtotal_amount");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.TaxAmount)
                .HasPrecision(18, 2)
                .HasColumnName("tax_amount");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(18, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ExpenseAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expenses_cancelled_by");

            entity.HasOne(d => d.ExpenseCategory).WithMany(p => p.Expenses)
                .HasPrincipalKey(p => new { p.CompanyId, p.ExpenseCategoryId })
                .HasForeignKey(d => new { d.CompanyId, d.ExpenseCategoryId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expenses_category");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.ExpenseAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.IncurredByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expenses_incurred_by");

            entity.HasOne(d => d.Project).WithMany(p => p.Expenses)
                .HasPrincipalKey(p => new { p.CompanyId, p.ProjectId })
                .HasForeignKey(d => new { d.CompanyId, d.ProjectId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expenses_project");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.ExpenseAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expenses_reversed_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.ExpenseAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReviewedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expenses_reviewed_by");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.ExpenseAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.SubmittedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expenses_submitted_by");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Expenses)
                .HasPrincipalKey(p => new { p.CompanyId, p.SupplierId })
                .HasForeignKey(d => new { d.CompanyId, d.SupplierId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expenses_supplier");
        });

        modelBuilder.Entity<ExpenseAdvanceAllocation>(entity =>
        {
            entity.HasKey(e => e.ExpenseAdvanceAllocationId).HasName("expense_advance_allocations_pkey");

            entity.ToTable("expense_advance_allocations", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.AllocatedByUserId }, "ix_expense_advance_allocations_allocated_by");

            entity.HasIndex(e => new { e.CompanyId, e.UserAdvanceBalanceId }, "ix_expense_advance_allocations_balance");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseId }, "ix_expense_advance_allocations_expense");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseAdvanceAllocationId }, "uq_expense_advance_allocations_company_allocation").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseId, e.UserAdvanceBalanceId }, "uq_expense_advance_allocations_expense_balance").IsUnique();

            entity.Property(e => e.ExpenseAdvanceAllocationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("expense_advance_allocation_id");
            entity.Property(e => e.AllocatedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("allocated_amount");
            entity.Property(e => e.AllocatedByUserId).HasColumnName("allocated_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpenseId).HasColumnName("expense_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserAdvanceBalanceId).HasColumnName("user_advance_balance_id");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ExpenseAdvanceAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.AllocatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_advance_allocations_allocated_by");

            entity.HasOne(d => d.Expense).WithMany(p => p.ExpenseAdvanceAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.ExpenseId })
                .HasForeignKey(d => new { d.CompanyId, d.ExpenseId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_advance_allocations_expense");

            entity.HasOne(d => d.UserAdvanceBalance).WithMany(p => p.ExpenseAdvanceAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserAdvanceBalanceId })
                .HasForeignKey(d => new { d.CompanyId, d.UserAdvanceBalanceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_advance_allocations_balance");
        });

        modelBuilder.Entity<ExpenseCategory>(entity =>
        {
            entity.HasKey(e => e.ExpenseCategoryId).HasName("expense_categories_pkey");

            entity.ToTable("expense_categories", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.DisplayOrder, e.CategoryName }, "ix_expense_categories_active").HasFilter("(is_active = true)");

            entity.HasIndex(e => new { e.CompanyId, e.CategoryGroup }, "ix_expense_categories_group");

            entity.HasIndex(e => new { e.CompanyId, e.ParentExpenseCategoryId }, "ix_expense_categories_parent").HasFilter("(parent_expense_category_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseScope }, "ix_expense_categories_scope");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseCategoryId }, "uq_expense_categories_company_category").IsUnique();

            entity.Property(e => e.ExpenseCategoryId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("expense_category_id");
            entity.Property(e => e.CategoryCode)
                .HasMaxLength(30)
                .HasColumnName("category_code");
            entity.Property(e => e.CategoryGroup)
                .HasMaxLength(30)
                .HasColumnName("category_group");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(150)
                .HasColumnName("category_name");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.DeactivatedAt).HasColumnName("deactivated_at");
            entity.Property(e => e.DeactivatedByUserId).HasColumnName("deactivated_by_user_id");
            entity.Property(e => e.DeactivationReason)
                .HasMaxLength(500)
                .HasColumnName("deactivation_reason");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
            entity.Property(e => e.ExpenseScope)
                .HasMaxLength(20)
                .HasDefaultValueSql("'ProjectOnly'::character varying")
                .HasColumnName("expense_scope");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ParentExpenseCategoryId).HasColumnName("parent_expense_category_id");
            entity.Property(e => e.RequiresReceipt)
                .HasDefaultValue(true)
                .HasColumnName("requires_receipt");
            entity.Property(e => e.RequiresSupplier).HasColumnName("requires_supplier");
            entity.Property(e => e.SupportsQuantityDetails).HasColumnName("supports_quantity_details");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.Company).WithMany(p => p.ExpenseCategories)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_categories_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ExpenseCategoryAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_categories_created_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.ExpenseCategoryAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.DeactivatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_categories_deactivated_by");

            entity.HasOne(d => d.ExpenseCategoryNavigation).WithMany(p => p.InverseExpenseCategoryNavigation)
                .HasPrincipalKey(p => new { p.CompanyId, p.ExpenseCategoryId })
                .HasForeignKey(d => new { d.CompanyId, d.ParentExpenseCategoryId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_categories_parent");
        });

        modelBuilder.Entity<ExpenseDocument>(entity =>
        {
            entity.HasKey(e => e.ExpenseDocumentId).HasName("expense_documents_pkey");

            entity.ToTable("expense_documents", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseId, e.CreatedAt }, "ix_expense_documents_expense");

            entity.HasIndex(e => new { e.CompanyId, e.Sha256Hash }, "ix_expense_documents_hash");

            entity.HasIndex(e => new { e.CompanyId, e.VerificationStatus }, "ix_expense_documents_status");

            entity.HasIndex(e => new { e.CompanyId, e.DocumentType }, "ix_expense_documents_type");

            entity.HasIndex(e => new { e.CompanyId, e.UploadedByUserId }, "ix_expense_documents_uploaded_by");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseDocumentId }, "uq_expense_documents_company_document").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseId, e.Sha256Hash }, "uq_expense_documents_expense_hash").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseId }, "ux_expense_documents_one_primary")
                .IsUnique()
                .HasFilter("((is_primary = true) AND ((verification_status)::text <> 'Rejected'::text))");

            entity.Property(e => e.ExpenseDocumentId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("expense_document_id");
            entity.Property(e => e.CaptureSource)
                .HasMaxLength(30)
                .HasDefaultValueSql("'FileUpload'::character varying")
                .HasColumnName("capture_source");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DocumentDate).HasColumnName("document_date");
            entity.Property(e => e.DocumentNumber)
                .HasMaxLength(100)
                .HasColumnName("document_number");
            entity.Property(e => e.DocumentType)
                .HasMaxLength(30)
                .HasColumnName("document_type");
            entity.Property(e => e.ExpenseId).HasColumnName("expense_id");
            entity.Property(e => e.FileSizeBytes).HasColumnName("file_size_bytes");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(1000)
                .HasColumnName("file_url");
            entity.Property(e => e.IsPrimary).HasColumnName("is_primary");
            entity.Property(e => e.IssuerName)
                .HasMaxLength(200)
                .HasColumnName("issuer_name");
            entity.Property(e => e.MimeType)
                .HasMaxLength(150)
                .HasColumnName("mime_type");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.OriginalFileName)
                .HasMaxLength(255)
                .HasColumnName("original_file_name");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.Sha256Hash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("sha256_hash");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UploadedByUserId).HasColumnName("uploaded_by_user_id");
            entity.Property(e => e.VerificationStatus)
                .HasMaxLength(30)
                .HasDefaultValueSql("'PendingVerification'::character varying")
                .HasColumnName("verification_status");
            entity.Property(e => e.VerifiedAt).HasColumnName("verified_at");
            entity.Property(e => e.VerifiedByUserId).HasColumnName("verified_by_user_id");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.Expense).WithOne(p => p.ExpenseDocument)
                .HasPrincipalKey<Expense>(p => new { p.CompanyId, p.ExpenseId })
                .HasForeignKey<ExpenseDocument>(d => new { d.CompanyId, d.ExpenseId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_documents_expense");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ExpenseDocumentAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.UploadedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_documents_uploaded_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.ExpenseDocumentAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.VerifiedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_documents_verified_by");
        });

        modelBuilder.Entity<ExpenseItem>(entity =>
        {
            entity.HasKey(e => e.ExpenseItemId).HasName("expense_items_pkey");

            entity.ToTable("expense_items", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ItemCode }, "ix_expense_items_code").HasFilter("(item_code IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.CreatedByUserId }, "ix_expense_items_created_by");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseId, e.LineNumber }, "ix_expense_items_expense");

            entity.HasIndex(e => new { e.CompanyId, e.ItemName }, "ix_expense_items_name");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseItemId }, "uq_expense_items_company_item").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseId, e.LineNumber }, "uq_expense_items_expense_line").IsUnique();

            entity.Property(e => e.ExpenseItemId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("expense_item_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CustomUnitName)
                .HasMaxLength(50)
                .HasColumnName("custom_unit_name");
            entity.Property(e => e.DiscountAmount)
                .HasPrecision(18, 2)
                .HasColumnName("discount_amount");
            entity.Property(e => e.ExpenseId).HasColumnName("expense_id");
            entity.Property(e => e.ItemCode)
                .HasMaxLength(100)
                .HasColumnName("item_code");
            entity.Property(e => e.ItemDescription)
                .HasMaxLength(500)
                .HasColumnName("item_description");
            entity.Property(e => e.ItemName)
                .HasMaxLength(200)
                .HasColumnName("item_name");
            entity.Property(e => e.LineNumber).HasColumnName("line_number");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.Quantity)
                .HasPrecision(18, 3)
                .HasColumnName("quantity");
            entity.Property(e => e.SubtotalAmount)
                .HasPrecision(18, 2)
                .HasComputedColumnSql("round((quantity * unit_price), 2)", true)
                .HasColumnName("subtotal_amount");
            entity.Property(e => e.TaxAmount)
                .HasPrecision(18, 2)
                .HasColumnName("tax_amount");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(18, 2)
                .HasComputedColumnSql("((round((quantity * unit_price), 2) - discount_amount) + tax_amount)", true)
                .HasColumnName("total_amount");
            entity.Property(e => e.UnitCode)
                .HasMaxLength(30)
                .HasColumnName("unit_code");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(18, 2)
                .HasColumnName("unit_price");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ExpenseItems)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_items_created_by");

            entity.HasOne(d => d.Expense).WithMany(p => p.ExpenseItems)
                .HasPrincipalKey(p => new { p.CompanyId, p.ExpenseId })
                .HasForeignKey(d => new { d.CompanyId, d.ExpenseId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_items_expense");
        });

        modelBuilder.Entity<ExpenseReturn>(entity =>
        {
            entity.HasKey(e => e.ExpenseReturnId).HasName("expense_returns_pkey");

            entity.ToTable("expense_returns", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseId }, "ix_expense_returns_expense");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseId, e.ReturnDate }, "ix_expense_returns_expense_date");

            entity.HasIndex(e => new { e.CompanyId, e.SubmittedAt }, "ix_expense_returns_pending").HasFilter("((status)::text = 'PendingApproval'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.ResolutionType }, "ix_expense_returns_resolution");

            entity.HasIndex(e => new { e.CompanyId, e.ReturnDate }, "ix_expense_returns_return_date");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_expense_returns_status");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseReturnId }, "uq_expense_returns_company_return").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReturnNumber }, "uq_expense_returns_number").IsUnique();

            entity.Property(e => e.ExpenseReturnId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("expense_return_id");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedByUserId).HasColumnName("approved_by_user_id");
            entity.Property(e => e.BankName)
                .HasMaxLength(150)
                .HasColumnName("bank_name");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'LYD'::character varying")
                .HasColumnName("currency_code");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.ExpenseId).HasColumnName("expense_id");
            entity.Property(e => e.ExpenseVersionNumber).HasColumnName("expense_version_number");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.ProofFileUrl)
                .HasMaxLength(1000)
                .HasColumnName("proof_file_url");
            entity.Property(e => e.ReferenceNumber)
                .HasMaxLength(150)
                .HasColumnName("reference_number");
            entity.Property(e => e.RefundMethod)
                .HasMaxLength(30)
                .HasColumnName("refund_method");
            entity.Property(e => e.RejectedAt).HasColumnName("rejected_at");
            entity.Property(e => e.RejectedByUserId).HasColumnName("rejected_by_user_id");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.ResolutionType)
                .HasMaxLength(40)
                .HasColumnName("resolution_type");
            entity.Property(e => e.ReturnAmount)
                .HasPrecision(18, 2)
                .HasColumnName("return_amount");
            entity.Property(e => e.ReturnDate).HasColumnName("return_date");
            entity.Property(e => e.ReturnNumber)
                .HasMaxLength(50)
                .HasColumnName("return_number");
            entity.Property(e => e.ReturnReasonType)
                .HasMaxLength(40)
                .HasColumnName("return_reason_type");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SubmittedAt).HasColumnName("submitted_at");
            entity.Property(e => e.SubmittedByUserId).HasColumnName("submitted_by_user_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ExpenseReturnAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ApprovedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_returns_approved_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.ExpenseReturnAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_returns_cancelled_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.ExpenseReturnAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_returns_created_by");

            entity.HasOne(d => d.Expense).WithMany(p => p.ExpenseReturns)
                .HasPrincipalKey(p => new { p.CompanyId, p.ExpenseId })
                .HasForeignKey(d => new { d.CompanyId, d.ExpenseId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_returns_expense");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.ExpenseReturnAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RejectedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_returns_rejected_by");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.ExpenseReturnAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_returns_reversed_by");

            entity.HasOne(d => d.AppUser4).WithMany(p => p.ExpenseReturnAppUser4s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.SubmittedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_returns_submitted_by");
        });

        modelBuilder.Entity<ExpenseReturnAdvanceAllocation>(entity =>
        {
            entity.HasKey(e => e.ExpenseReturnAdvanceAllocationId).HasName("expense_return_advance_allocations_pkey");

            entity.ToTable("expense_return_advance_allocations", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.AllocatedByUserId }, "ix_expense_return_advance_allocations_allocated_by");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseAdvanceAllocationId }, "ix_expense_return_advance_allocations_original");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseReturnId }, "ix_expense_return_advance_allocations_return");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseReturnAdvanceAllocationId }, "uq_expense_return_advance_allocations_company_allocation").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseReturnId, e.ExpenseAdvanceAllocationId }, "uq_expense_return_advance_allocations_return_source").IsUnique();

            entity.Property(e => e.ExpenseReturnAdvanceAllocationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("expense_return_advance_allocation_id");
            entity.Property(e => e.AllocatedByUserId).HasColumnName("allocated_by_user_id");
            entity.Property(e => e.BalanceVersionNumber).HasColumnName("balance_version_number");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpenseAdvanceAllocationId).HasColumnName("expense_advance_allocation_id");
            entity.Property(e => e.ExpenseReturnId).HasColumnName("expense_return_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.RestoredAmount)
                .HasPrecision(18, 2)
                .HasColumnName("restored_amount");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ExpenseReturnAdvanceAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.AllocatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_return_advance_allocations_allocated_by");

            entity.HasOne(d => d.ExpenseAdvanceAllocation).WithMany(p => p.ExpenseReturnAdvanceAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.ExpenseAdvanceAllocationId })
                .HasForeignKey(d => new { d.CompanyId, d.ExpenseAdvanceAllocationId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_return_advance_allocations_original_allocation");

            entity.HasOne(d => d.ExpenseReturn).WithMany(p => p.ExpenseReturnAdvanceAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.ExpenseReturnId })
                .HasForeignKey(d => new { d.CompanyId, d.ExpenseReturnId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_return_advance_allocations_return");
        });

        modelBuilder.Entity<ExpenseReturnItem>(entity =>
        {
            entity.HasKey(e => e.ExpenseReturnItemId).HasName("expense_return_items_pkey");

            entity.ToTable("expense_return_items", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ItemCondition }, "ix_expense_return_items_condition").HasFilter("(item_condition IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.CreatedByUserId }, "ix_expense_return_items_created_by");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseItemId }, "ix_expense_return_items_expense_item");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseReturnId, e.LineNumber }, "ix_expense_return_items_return");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseReturnItemId }, "uq_expense_return_items_company_item").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseReturnId, e.ExpenseItemId }, "uq_expense_return_items_return_expense_item").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseReturnId, e.LineNumber }, "uq_expense_return_items_return_line").IsUnique();

            entity.Property(e => e.ExpenseReturnItemId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("expense_return_item_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CustomUnitNameSnapshot)
                .HasMaxLength(50)
                .HasColumnName("custom_unit_name_snapshot");
            entity.Property(e => e.DiscountAmount)
                .HasPrecision(18, 2)
                .HasColumnName("discount_amount");
            entity.Property(e => e.ExpenseItemId).HasColumnName("expense_item_id");
            entity.Property(e => e.ExpenseReturnId).HasColumnName("expense_return_id");
            entity.Property(e => e.IsRestockable).HasColumnName("is_restockable");
            entity.Property(e => e.ItemCondition)
                .HasMaxLength(30)
                .HasColumnName("item_condition");
            entity.Property(e => e.ItemNameSnapshot)
                .HasMaxLength(200)
                .HasColumnName("item_name_snapshot");
            entity.Property(e => e.LineNumber).HasColumnName("line_number");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.ReturnedQuantity)
                .HasPrecision(18, 3)
                .HasColumnName("returned_quantity");
            entity.Property(e => e.SubtotalAmount)
                .HasPrecision(18, 2)
                .HasComputedColumnSql("round((returned_quantity * unit_return_price), 2)", true)
                .HasColumnName("subtotal_amount");
            entity.Property(e => e.TaxAmount)
                .HasPrecision(18, 2)
                .HasColumnName("tax_amount");
            entity.Property(e => e.TotalReturnAmount)
                .HasPrecision(18, 2)
                .HasComputedColumnSql("((round((returned_quantity * unit_return_price), 2) - discount_amount) + tax_amount)", true)
                .HasColumnName("total_return_amount");
            entity.Property(e => e.UnitCodeSnapshot)
                .HasMaxLength(30)
                .HasColumnName("unit_code_snapshot");
            entity.Property(e => e.UnitReturnPrice)
                .HasPrecision(18, 2)
                .HasColumnName("unit_return_price");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ExpenseReturnItems)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_return_items_created_by");

            entity.HasOne(d => d.ExpenseItem).WithMany(p => p.ExpenseReturnItems)
                .HasPrincipalKey(p => new { p.CompanyId, p.ExpenseItemId })
                .HasForeignKey(d => new { d.CompanyId, d.ExpenseItemId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_return_items_expense_item");

            entity.HasOne(d => d.ExpenseReturn).WithMany(p => p.ExpenseReturnItems)
                .HasPrincipalKey(p => new { p.CompanyId, p.ExpenseReturnId })
                .HasForeignKey(d => new { d.CompanyId, d.ExpenseReturnId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_expense_return_items_return");
        });

        modelBuilder.Entity<FundingSource>(entity =>
        {
            entity.HasKey(e => e.FundingSourceId).HasName("funding_sources_pkey");

            entity.ToTable("funding_sources", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.AvailableAmount }, "ix_funding_sources_available").HasFilter("((status)::text = ANY ((ARRAY['Available'::character varying, 'PartiallyUsed'::character varying])::text[]))");

            entity.HasIndex(e => e.CompanyId, "ix_funding_sources_company");

            entity.HasIndex(e => new { e.CompanyId, e.Status }, "ix_funding_sources_company_status");

            entity.HasIndex(e => new { e.CompanyId, e.SourceType }, "ix_funding_sources_company_type");

            entity.HasIndex(e => new { e.CompanyId, e.SourceDate }, "ix_funding_sources_source_date");

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourceId }, "uq_funding_sources_company_source").IsUnique();

            entity.Property(e => e.FundingSourceId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("funding_source_id");
            entity.Property(e => e.AvailableAmount)
                .HasPrecision(18, 2)
                .HasColumnName("available_amount");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'LYD'::character varying")
                .HasColumnName("currency_code");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.FeeAmount)
                .HasPrecision(18, 2)
                .HasColumnName("fee_amount");
            entity.Property(e => e.GrossAmount)
                .HasPrecision(18, 2)
                .HasColumnName("gross_amount");
            entity.Property(e => e.NetAmount)
                .HasPrecision(18, 2)
                .HasColumnName("net_amount");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.ReservedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("reserved_amount");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("reversed_amount");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.SourceDate).HasColumnName("source_date");
            entity.Property(e => e.SourceType)
                .HasMaxLength(40)
                .HasColumnName("source_type");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'PendingVerification'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UsedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("used_amount");
            entity.Property(e => e.VerifiedAt).HasColumnName("verified_at");
            entity.Property(e => e.VerifiedByUserId).HasColumnName("verified_by_user_id");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.Company).WithMany(p => p.FundingSources)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_funding_sources_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.FundingSourceAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_funding_sources_cancelled_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.FundingSourceAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_funding_sources_created_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.FundingSourceAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_funding_sources_reversed_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.FundingSourceAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.VerifiedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_funding_sources_verified_by");
        });

        modelBuilder.Entity<FundingSourceLedgerEntry>(entity =>
        {
            entity.HasKey(e => e.FundingSourceLedgerEntryId).HasName("funding_source_ledger_entries_pkey");

            entity.ToTable("funding_source_ledger_entries", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.EntryType }, "ix_funding_source_ledger_company_type");

            entity.HasIndex(e => e.CorrelationId, "ix_funding_source_ledger_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.CreatedAt }, "ix_funding_source_ledger_created_at");

            entity.HasIndex(e => new { e.CompanyId, e.ReferenceType, e.ReferenceId }, "ix_funding_source_ledger_reference").HasFilter("(reference_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourceId, e.SourceVersionNumber }, "ix_funding_source_ledger_source");

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourceLedgerEntryId }, "uq_funding_source_ledger_company_entry").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourceId, e.SourceVersionNumber }, "uq_funding_source_ledger_version").IsUnique();

            entity.Property(e => e.FundingSourceLedgerEntryId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("funding_source_ledger_entry_id");
            entity.Property(e => e.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("amount");
            entity.Property(e => e.AvailableAfter)
                .HasPrecision(18, 2)
                .HasColumnName("available_after");
            entity.Property(e => e.AvailableDelta)
                .HasPrecision(18, 2)
                .HasColumnName("available_delta");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.EntryType)
                .HasMaxLength(50)
                .HasColumnName("entry_type");
            entity.Property(e => e.FundingSourceId).HasColumnName("funding_source_id");
            entity.Property(e => e.PerformedByUserId).HasColumnName("performed_by_user_id");
            entity.Property(e => e.ReferenceId).HasColumnName("reference_id");
            entity.Property(e => e.ReferenceType)
                .HasMaxLength(100)
                .HasColumnName("reference_type");
            entity.Property(e => e.ReservedAfter)
                .HasPrecision(18, 2)
                .HasColumnName("reserved_after");
            entity.Property(e => e.ReservedDelta)
                .HasPrecision(18, 2)
                .HasColumnName("reserved_delta");
            entity.Property(e => e.ReversedAfter)
                .HasPrecision(18, 2)
                .HasColumnName("reversed_after");
            entity.Property(e => e.ReversedDelta)
                .HasPrecision(18, 2)
                .HasColumnName("reversed_delta");
            entity.Property(e => e.SourceVersionNumber).HasColumnName("source_version_number");
            entity.Property(e => e.UsedAfter)
                .HasPrecision(18, 2)
                .HasColumnName("used_after");
            entity.Property(e => e.UsedDelta)
                .HasPrecision(18, 2)
                .HasColumnName("used_delta");

            entity.HasOne(d => d.FundingSource).WithMany(p => p.FundingSourceLedgerEntries)
                .HasPrincipalKey(p => new { p.CompanyId, p.FundingSourceId })
                .HasForeignKey(d => new { d.CompanyId, d.FundingSourceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_funding_source_ledger_source");

            entity.HasOne(d => d.AppUser).WithMany(p => p.FundingSourceLedgerEntries)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.PerformedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_funding_source_ledger_performed_by");
        });

        modelBuilder.Entity<FundingSourcePaymentMethod>(entity =>
        {
            entity.HasKey(e => e.FundingSourcePaymentMethodId).HasName("funding_source_payment_methods_pkey");

            entity.ToTable("funding_source_payment_methods", "ahdah");

            entity.HasIndex(e => e.CompanyId, "ix_funding_source_payment_methods_company");

            entity.HasIndex(e => new { e.CompanyId, e.ReferenceNumber }, "ix_funding_source_payment_methods_reference").HasFilter("(reference_number IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourceId }, "ix_funding_source_payment_methods_source");

            entity.HasIndex(e => new { e.CompanyId, e.PaymentMethod }, "ix_funding_source_payment_methods_type");

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourcePaymentMethodId }, "uq_funding_source_payment_methods_company_method").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourceId, e.SequenceNumber }, "uq_funding_source_payment_methods_sequence").IsUnique();

            entity.Property(e => e.FundingSourcePaymentMethodId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("funding_source_payment_method_id");
            entity.Property(e => e.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("amount");
            entity.Property(e => e.BankName)
                .HasMaxLength(150)
                .HasColumnName("bank_name");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.FundingSourceId).HasColumnName("funding_source_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.PayerName)
                .HasMaxLength(200)
                .HasColumnName("payer_name");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(30)
                .HasColumnName("payment_method");
            entity.Property(e => e.ProofFileUrl)
                .HasMaxLength(1000)
                .HasColumnName("proof_file_url");
            entity.Property(e => e.RecordedByUserId).HasColumnName("recorded_by_user_id");
            entity.Property(e => e.ReferenceNumber)
                .HasMaxLength(150)
                .HasColumnName("reference_number");
            entity.Property(e => e.SequenceNumber)
                .HasDefaultValue((short)1)
                .HasColumnName("sequence_number");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.FundingSource).WithMany(p => p.FundingSourcePaymentMethods)
                .HasPrincipalKey(p => new { p.CompanyId, p.FundingSourceId })
                .HasForeignKey(d => new { d.CompanyId, d.FundingSourceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_funding_source_payment_methods_source");

            entity.HasOne(d => d.AppUser).WithMany(p => p.FundingSourcePaymentMethods)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecordedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_funding_source_payment_methods_recorded_by");
        });

        modelBuilder.Entity<IdempotencyRecord>(entity =>
        {
            entity.HasKey(e => e.IdempotencyRecordId).HasName("idempotency_records_pkey");

            entity.ToTable("idempotency_records", "ahdah");

            entity.HasIndex(e => e.LeaseExpiresAt, "ix_idem_active_lease").HasFilter("((status)::text = 'InProgress'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.ActorUserId, e.CreatedAt }, "ix_idem_actor")
                .IsDescending(false, false, true)
                .HasFilter("(actor_user_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_idem_correlation");

            entity.HasIndex(e => e.ExpiresAt, "ix_idem_expiry").HasFilter("((status)::text = ANY ((ARRAY['Completed'::character varying, 'Failed'::character varying, 'Cancelled'::character varying, 'Expired'::character varying])::text[]))");

            entity.HasIndex(e => new { e.CompanyId, e.RequestFingerprintHash }, "ix_idem_fingerprint");

            entity.HasIndex(e => new { e.CompanyId, e.OperationName, e.CreatedAt }, "ix_idem_operation").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ResourceType, e.ResourceId }, "ix_idem_resource").HasFilter("(resource_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.CompletedAt }, "ix_idem_retryable_failed").HasFilter("(((status)::text = 'Failed'::text) AND (is_retryable = true))");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_idem_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.IdempotencyKey }, "uq_idem_company_key").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.IdempotencyRecordId }, "uq_idem_company_record").IsUnique();

            entity.Property(e => e.IdempotencyRecordId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idempotency_record_id");
            entity.Property(e => e.ActorType)
                .HasMaxLength(30)
                .HasDefaultValueSql("'User'::character varying")
                .HasColumnName("actor_type");
            entity.Property(e => e.ActorUserId).HasColumnName("actor_user_id");
            entity.Property(e => e.AttemptCount)
                .HasDefaultValue(1)
                .HasColumnName("attempt_count");
            entity.Property(e => e.ClientIdentifier)
                .HasMaxLength(200)
                .HasColumnName("client_identifier");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt)
                .HasDefaultValueSql("(CURRENT_TIMESTAMP + '24:00:00'::interval)")
                .HasColumnName("expires_at");
            entity.Property(e => e.FailureCode)
                .HasMaxLength(100)
                .HasColumnName("failure_code");
            entity.Property(e => e.FailureMessage)
                .HasMaxLength(2000)
                .HasColumnName("failure_message");
            entity.Property(e => e.IdempotencyKey)
                .HasMaxLength(200)
                .HasColumnName("idempotency_key");
            entity.Property(e => e.IsRetryable).HasColumnName("is_retryable");
            entity.Property(e => e.LastReplayedAt).HasColumnName("last_replayed_at");
            entity.Property(e => e.LeaseExpiresAt).HasColumnName("lease_expires_at");
            entity.Property(e => e.LockAcquiredAt).HasColumnName("lock_acquired_at");
            entity.Property(e => e.LockToken).HasColumnName("lock_token");
            entity.Property(e => e.LockedBy)
                .HasMaxLength(200)
                .HasColumnName("locked_by");
            entity.Property(e => e.OperationName)
                .HasMaxLength(150)
                .HasColumnName("operation_name");
            entity.Property(e => e.ReplayCount).HasColumnName("replay_count");
            entity.Property(e => e.RequestFingerprintHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("request_fingerprint_hash");
            entity.Property(e => e.RequestMethod)
                .HasMaxLength(10)
                .HasColumnName("request_method");
            entity.Property(e => e.RequestPath)
                .HasMaxLength(1000)
                .HasColumnName("request_path");
            entity.Property(e => e.RequestPayloadHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("request_payload_hash");
            entity.Property(e => e.RequestSource)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Application'::character varying")
                .HasColumnName("request_source");
            entity.Property(e => e.ResourceId).HasColumnName("resource_id");
            entity.Property(e => e.ResourceType)
                .HasMaxLength(100)
                .HasColumnName("resource_type");
            entity.Property(e => e.ResourceVersionNumber).HasColumnName("resource_version_number");
            entity.Property(e => e.ResponseContentType)
                .HasMaxLength(150)
                .HasColumnName("response_content_type");
            entity.Property(e => e.ResponseHttpStatus).HasColumnName("response_http_status");
            entity.Property(e => e.ResponsePayload)
                .HasColumnType("jsonb")
                .HasColumnName("response_payload");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("started_at");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'InProgress'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Company).WithMany(p => p.IdempotencyRecords)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_idem_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.IdempotencyRecords)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ActorUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_idem_actor");
        });

        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.HasKey(e => e.InvitationId).HasName("invitations_pkey");

            entity.ToTable("invitations", "ahdah");

            entity.HasIndex(e => e.CompanyId, "ix_invitations_company");

            entity.HasIndex(e => new { e.CompanyId, e.Status }, "ix_invitations_company_status");

            entity.HasIndex(e => e.ExpiresAt, "ix_invitations_pending_expiration").HasFilter("((status)::text = 'Pending'::text)");

            entity.HasIndex(e => e.InvitedPhoneNumber, "ix_invitations_phone");

            entity.HasIndex(e => new { e.CompanyId, e.InvitationId }, "uq_invitations_company_invitation").IsUnique();

            entity.HasIndex(e => e.InvitationCodeHash, "ux_invitations_code_hash").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.InvitedPhoneNumber }, "ux_invitations_pending_phone")
                .IsUnique()
                .HasFilter("((status)::text = 'Pending'::text)");

            entity.Property(e => e.InvitationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("invitation_id");
            entity.Property(e => e.AcceptedAt).HasColumnName("accepted_at");
            entity.Property(e => e.AcceptedByUserId).HasColumnName("accepted_by_user_id");
            entity.Property(e => e.AssignedRole)
                .HasMaxLength(30)
                .HasColumnName("assigned_role");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.InvitationCodeHash).HasColumnName("invitation_code_hash");
            entity.Property(e => e.InvitedByUserId).HasColumnName("invited_by_user_id");
            entity.Property(e => e.InvitedPhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("invited_phone_number");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Company).WithMany(p => p.Invitations)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_invitations_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.InvitationAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.AcceptedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_invitations_accepted_by_user");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.InvitationAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.InvitedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_invitations_invited_by_user");
        });

        modelBuilder.Entity<JoinRequest>(entity =>
        {
            entity.HasKey(e => e.JoinRequestId).HasName("join_requests_pkey");

            entity.ToTable("join_requests", "ahdah");

            entity.HasIndex(e => e.CompanyId, "ix_join_requests_company");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.RequestedAt }, "ix_join_requests_company_status");

            entity.HasIndex(e => new { e.CompanyId, e.ReviewedByUserId }, "ix_join_requests_reviewer").HasFilter("(reviewed_by_user_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.UserId }, "ix_join_requests_user");

            entity.HasIndex(e => new { e.CompanyId, e.JoinRequestId }, "uq_join_requests_company_request").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.UserId }, "ux_join_requests_one_pending")
                .IsUnique()
                .HasFilter("((status)::text = 'Pending'::text)");

            entity.Property(e => e.JoinRequestId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("join_request_id");
            entity.Property(e => e.AssignedRole)
                .HasMaxLength(30)
                .HasColumnName("assigned_role");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.RequestMessage)
                .HasMaxLength(1000)
                .HasColumnName("request_message");
            entity.Property(e => e.RequestedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("requested_at");
            entity.Property(e => e.RequestedRole)
                .HasMaxLength(30)
                .HasColumnName("requested_role");
            entity.Property(e => e.ReviewNotes)
                .HasMaxLength(1000)
                .HasColumnName("review_notes");
            entity.Property(e => e.ReviewedAt).HasColumnName("reviewed_at");
            entity.Property(e => e.ReviewedByUserId).HasColumnName("reviewed_by_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Company).WithMany(p => p.JoinRequests)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_join_requests_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.JoinRequestAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReviewedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_join_requests_reviewed_by");

            entity.HasOne(d => d.AppUserNavigation).WithOne(p => p.JoinRequestAppUserNavigation)
                .HasPrincipalKey<AppUser>(p => new { p.CompanyId, p.UserId })
                .HasForeignKey<JoinRequest>(d => new { d.CompanyId, d.UserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_join_requests_user");
        });

        modelBuilder.Entity<ManagerContribution>(entity =>
        {
            entity.HasKey(e => e.ManagerContributionId).HasName("manager_contributions_pkey");

            entity.ToTable("manager_contributions", "ahdah");

            entity.HasIndex(e => e.CompanyId, "ix_manager_contributions_company");

            entity.HasIndex(e => new { e.CompanyId, e.ContributionDate }, "ix_manager_contributions_contribution_date");

            entity.HasIndex(e => new { e.CompanyId, e.ManagerUserId }, "ix_manager_contributions_manager");

            entity.HasIndex(e => new { e.CompanyId, e.ManagerUserId, e.ContributionDate }, "ix_manager_contributions_manager_date");

            entity.HasIndex(e => new { e.CompanyId, e.Status }, "ix_manager_contributions_status");

            entity.HasIndex(e => new { e.CompanyId, e.ManagerContributionId }, "uq_manager_contributions_company_contribution").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourceId }, "uq_manager_contributions_funding_source").IsUnique();

            entity.Property(e => e.ManagerContributionId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("manager_contribution_id");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.ContributionAmount)
                .HasPrecision(18, 2)
                .HasColumnName("contribution_amount");
            entity.Property(e => e.ContributionDate).HasColumnName("contribution_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.FundingSourceId).HasColumnName("funding_source_id");
            entity.Property(e => e.ManagerUserId).HasColumnName("manager_user_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.Purpose)
                .HasMaxLength(1000)
                .HasColumnName("purpose");
            entity.Property(e => e.RecordedByUserId).HasColumnName("recorded_by_user_id");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'PendingVerification'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VerifiedAt).HasColumnName("verified_at");
            entity.Property(e => e.VerifiedByUserId).HasColumnName("verified_by_user_id");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ManagerContributionAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_manager_contributions_cancelled_by");

            entity.HasOne(d => d.FundingSource).WithOne(p => p.ManagerContribution)
                .HasPrincipalKey<FundingSource>(p => new { p.CompanyId, p.FundingSourceId })
                .HasForeignKey<ManagerContribution>(d => new { d.CompanyId, d.FundingSourceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_manager_contributions_funding_source");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.ManagerContributionAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ManagerUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_manager_contributions_manager");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.ManagerContributionAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecordedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_manager_contributions_recorded_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.ManagerContributionAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_manager_contributions_reversed_by");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.ManagerContributionAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.VerifiedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_manager_contributions_verified_by");
        });

        modelBuilder.Entity<MoneyTransfer>(entity =>
        {
            entity.HasKey(e => e.MoneyTransferId).HasName("money_transfers_pkey");

            entity.ToTable("money_transfers", "ahdah");

            entity.HasIndex(e => e.CompanyId, "ix_money_transfers_company");

            entity.HasIndex(e => new { e.CompanyId, e.TransferDate }, "ix_money_transfers_date");

            entity.HasIndex(e => new { e.CompanyId, e.RecipientUserId }, "ix_money_transfers_recipient");

            entity.HasIndex(e => new { e.CompanyId, e.RecipientUserId, e.TransferDate }, "ix_money_transfers_recipient_pending").HasFilter("((status)::text = 'PendingConfirmation'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.SenderUserId }, "ix_money_transfers_sender");

            entity.HasIndex(e => new { e.CompanyId, e.Status }, "ix_money_transfers_status");

            entity.HasIndex(e => new { e.CompanyId, e.TransferType }, "ix_money_transfers_type");

            entity.HasIndex(e => new { e.CompanyId, e.MoneyTransferId }, "uq_money_transfers_company_transfer").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.TransferNumber }, "uq_money_transfers_number").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceId }, "ux_money_transfers_advance_delivery")
                .IsUnique()
                .HasFilter("((transfer_type)::text = 'AdvanceDelivery'::text)");

            entity.Property(e => e.MoneyTransferId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("money_transfer_id");
            entity.Property(e => e.AdvanceId).HasColumnName("advance_id");
            entity.Property(e => e.BankName)
                .HasMaxLength(150)
                .HasColumnName("bank_name");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.ConfirmedAt).HasColumnName("confirmed_at");
            entity.Property(e => e.ConfirmedByUserId).HasColumnName("confirmed_by_user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'LYD'::character varying")
                .HasColumnName("currency_code");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.InitiatedByUserId).HasColumnName("initiated_by_user_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.ProofFileUrl)
                .HasMaxLength(1000)
                .HasColumnName("proof_file_url");
            entity.Property(e => e.RecipientUserId).HasColumnName("recipient_user_id");
            entity.Property(e => e.ReferenceNumber)
                .HasMaxLength(150)
                .HasColumnName("reference_number");
            entity.Property(e => e.RejectedAt).HasColumnName("rejected_at");
            entity.Property(e => e.RejectedByUserId).HasColumnName("rejected_by_user_id");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.SenderUserId).HasColumnName("sender_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TransferAmount)
                .HasPrecision(18, 2)
                .HasColumnName("transfer_amount");
            entity.Property(e => e.TransferDate).HasColumnName("transfer_date");
            entity.Property(e => e.TransferMethod)
                .HasMaxLength(30)
                .HasColumnName("transfer_method");
            entity.Property(e => e.TransferNumber)
                .HasMaxLength(50)
                .HasColumnName("transfer_number");
            entity.Property(e => e.TransferType)
                .HasMaxLength(30)
                .HasColumnName("transfer_type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.Advance).WithOne(p => p.MoneyTransfer)
                .HasPrincipalKey<Advance>(p => new { p.CompanyId, p.AdvanceId })
                .HasForeignKey<MoneyTransfer>(d => new { d.CompanyId, d.AdvanceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_money_transfers_advance");

            entity.HasOne(d => d.AppUser).WithMany(p => p.MoneyTransferAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_money_transfers_cancelled_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.MoneyTransferAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ConfirmedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_money_transfers_confirmed_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.MoneyTransferAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.InitiatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_money_transfers_initiated_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.MoneyTransferAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecipientUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_money_transfers_recipient");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.MoneyTransferAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RejectedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_money_transfers_rejected_by");

            entity.HasOne(d => d.AppUser4).WithMany(p => p.MoneyTransferAppUser4s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_money_transfers_reversed_by");

            entity.HasOne(d => d.AppUser5).WithMany(p => p.MoneyTransferAppUser5s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.SenderUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_money_transfers_sender");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("notifications_pkey");

            entity.ToTable("notifications", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.NotificationCode, e.CreatedAt }, "ix_notifications_code").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_notifications_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.EntityType, e.EntityId }, "ix_notifications_entity").HasFilter("(entity_id IS NOT NULL)");

            entity.HasIndex(e => e.ExpiresAt, "ix_notifications_expiring").HasFilter("(((status)::text = 'Published'::text) AND (expires_at IS NOT NULL))");

            entity.HasIndex(e => new { e.CompanyId, e.Priority, e.PublishedAt }, "ix_notifications_priority")
                .IsDescending(false, false, true)
                .HasFilter("(((status)::text = 'Published'::text) AND ((priority)::text = ANY ((ARRAY['High'::character varying, 'Critical'::character varying])::text[])))");

            entity.HasIndex(e => new { e.CompanyId, e.RecipientUserId, e.ArchivedAt }, "ix_notifications_recipient_archived")
                .IsDescending(false, false, true)
                .HasFilter("(archived_at IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.RecipientUserId, e.Status, e.PublishedAt }, "ix_notifications_recipient_inbox").IsDescending(false, false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.RecipientUserId, e.PublishedAt }, "ix_notifications_recipient_unread")
                .IsDescending(false, false, true)
                .HasFilter("(((status)::text = 'Published'::text) AND (read_at IS NULL) AND (archived_at IS NULL))");

            entity.HasIndex(e => e.ScheduledAt, "ix_notifications_scheduled").HasFilter("((status)::text = 'Scheduled'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.NotificationId }, "uq_notifications_company_notification").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.RecipientUserId, e.DeduplicationKey }, "ux_notifications_deduplication")
                .IsUnique()
                .HasFilter("(deduplication_key IS NOT NULL)");

            entity.Property(e => e.NotificationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("notification_id");
            entity.Property(e => e.ActionTarget)
                .HasMaxLength(1000)
                .HasColumnName("action_target");
            entity.Property(e => e.ActionType)
                .HasMaxLength(30)
                .HasDefaultValueSql("'None'::character varying")
                .HasColumnName("action_type");
            entity.Property(e => e.ArchivedAt).HasColumnName("archived_at");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByType)
                .HasMaxLength(30)
                .HasDefaultValueSql("'System'::character varying")
                .HasColumnName("created_by_type");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.DeduplicationKey)
                .HasMaxLength(200)
                .HasColumnName("deduplication_key");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(100)
                .HasColumnName("entity_type");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.LocaleCode)
                .HasMaxLength(10)
                .HasDefaultValueSql("'ar-LY'::character varying")
                .HasColumnName("locale_code");
            entity.Property(e => e.Message)
                .HasMaxLength(2000)
                .HasColumnName("message");
            entity.Property(e => e.NotificationCategory)
                .HasMaxLength(30)
                .HasColumnName("notification_category");
            entity.Property(e => e.NotificationCode)
                .HasMaxLength(100)
                .HasColumnName("notification_code");
            entity.Property(e => e.Payload)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("payload");
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Normal'::character varying")
                .HasColumnName("priority");
            entity.Property(e => e.PublishedAt).HasColumnName("published_at");
            entity.Property(e => e.ReadAt).HasColumnName("read_at");
            entity.Property(e => e.RecipientUserId).HasColumnName("recipient_user_id");
            entity.Property(e => e.ScheduledAt).HasColumnName("scheduled_at");
            entity.Property(e => e.SeenAt).HasColumnName("seen_at");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(250)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.Company).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notifications_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.NotificationAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notifications_cancelled_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.NotificationAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notifications_created_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.NotificationAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecipientUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notifications_recipient");
        });

        modelBuilder.Entity<NotificationDelivery>(entity =>
        {
            entity.HasKey(e => e.NotificationDeliveryId).HasName("notification_deliveries_pkey");

            entity.ToTable("notification_deliveries", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_notification_deliveries_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.NotificationId }, "ix_notification_deliveries_notification");

            entity.HasIndex(e => new { e.ProviderName, e.ProviderMessageId }, "ix_notification_deliveries_provider_message").HasFilter("(provider_message_id IS NOT NULL)");

            entity.HasIndex(e => new { e.DeliveryChannel, e.QueuedAt }, "ix_notification_deliveries_queue").HasFilter("((delivery_status)::text = 'Queued'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.RecipientUserId, e.CreatedAt }, "ix_notification_deliveries_recipient").IsDescending(false, false, true);

            entity.HasIndex(e => e.NextRetryAt, "ix_notification_deliveries_retry").HasFilter("(((delivery_status)::text = 'Failed'::text) AND (next_retry_at IS NOT NULL))");

            entity.HasIndex(e => new { e.CompanyId, e.DeliveryStatus, e.CreatedAt }, "ix_notification_deliveries_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.TargetIdentifierHash }, "ix_notification_deliveries_target_hash").HasFilter("(target_identifier_hash IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.NotificationDeliveryId }, "uq_notification_deliveries_company_delivery").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.IdempotencyKey }, "uq_notification_deliveries_idempotency").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.NotificationId, e.DeliveryChannel, e.TargetIdentifierHash }, "ux_notification_deliveries_external_target")
                .IsUnique()
                .HasFilter("((delivery_channel)::text <> 'InApp'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.NotificationId }, "ux_notification_deliveries_in_app")
                .IsUnique()
                .HasFilter("((delivery_channel)::text = 'InApp'::text)");

            entity.Property(e => e.NotificationDeliveryId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("notification_delivery_id");
            entity.Property(e => e.AttemptCount).HasColumnName("attempt_count");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeliveredAt).HasColumnName("delivered_at");
            entity.Property(e => e.DeliveryChannel)
                .HasMaxLength(20)
                .HasColumnName("delivery_channel");
            entity.Property(e => e.DeliveryStatus)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("delivery_status");
            entity.Property(e => e.DeliveryTargetType)
                .HasMaxLength(30)
                .HasColumnName("delivery_target_type");
            entity.Property(e => e.DestinationSnapshot)
                .HasMaxLength(500)
                .HasColumnName("destination_snapshot");
            entity.Property(e => e.FailureCode)
                .HasMaxLength(100)
                .HasColumnName("failure_code");
            entity.Property(e => e.FailureMessage)
                .HasMaxLength(1500)
                .HasColumnName("failure_message");
            entity.Property(e => e.IdempotencyKey)
                .HasMaxLength(200)
                .HasColumnName("idempotency_key");
            entity.Property(e => e.IsFinal)
                .HasComputedColumnSql("\nCASE\n    WHEN ((delivery_status)::text = ANY ((ARRAY['Delivered'::character varying, 'Skipped'::character varying, 'Cancelled'::character varying])::text[])) THEN true\n    WHEN (((delivery_status)::text = 'Failed'::text) AND (attempt_count >= max_attempts)) THEN true\n    ELSE false\nEND", true)
                .HasColumnName("is_final");
            entity.Property(e => e.LastAttemptAt).HasColumnName("last_attempt_at");
            entity.Property(e => e.LastFailedAt).HasColumnName("last_failed_at");
            entity.Property(e => e.MaxAttempts)
                .HasDefaultValue(3)
                .HasColumnName("max_attempts");
            entity.Property(e => e.MessageSnapshot)
                .HasMaxLength(2000)
                .HasColumnName("message_snapshot");
            entity.Property(e => e.NextRetryAt).HasColumnName("next_retry_at");
            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.NotificationVersionNumber).HasColumnName("notification_version_number");
            entity.Property(e => e.PayloadSnapshot)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("payload_snapshot");
            entity.Property(e => e.ProviderMessageId)
                .HasMaxLength(250)
                .HasColumnName("provider_message_id");
            entity.Property(e => e.ProviderMetadata)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("provider_metadata");
            entity.Property(e => e.ProviderName)
                .HasMaxLength(100)
                .HasColumnName("provider_name");
            entity.Property(e => e.QueuedAt).HasColumnName("queued_at");
            entity.Property(e => e.RecipientUserId).HasColumnName("recipient_user_id");
            entity.Property(e => e.SentAt).HasColumnName("sent_at");
            entity.Property(e => e.SkipReason)
                .HasMaxLength(500)
                .HasColumnName("skip_reason");
            entity.Property(e => e.TargetIdentifierHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("target_identifier_hash");
            entity.Property(e => e.TitleSnapshot)
                .HasMaxLength(250)
                .HasColumnName("title_snapshot");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AppUser).WithMany(p => p.NotificationDeliveryAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notification_deliveries_cancelled_by");

            entity.HasOne(d => d.Notification).WithOne(p => p.NotificationDelivery)
                .HasPrincipalKey<Notification>(p => new { p.CompanyId, p.NotificationId })
                .HasForeignKey<NotificationDelivery>(d => new { d.CompanyId, d.NotificationId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notification_deliveries_notification");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.NotificationDeliveryAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecipientUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notification_deliveries_recipient");
        });

        modelBuilder.Entity<NotificationPreference>(entity =>
        {
            entity.HasKey(e => e.NotificationPreferenceId).HasName("notification_preferences_pkey");

            entity.ToTable("notification_preferences", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.NotificationCategory, e.DeliveryChannel }, "ix_notification_preferences_category").HasFilter("(notification_category IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.NotificationCode, e.DeliveryChannel }, "ix_notification_preferences_code").HasFilter("(notification_code IS NOT NULL)");

            entity.HasIndex(e => new { e.DeliveryChannel, e.DigestFrequency, e.DigestTime }, "ix_notification_preferences_digest").HasFilter("((is_enabled = true) AND ((delivery_mode)::text = 'Digest'::text))");

            entity.HasIndex(e => new { e.CompanyId, e.UserId, e.DeliveryChannel }, "ix_notification_preferences_mandatory").HasFilter("(is_mandatory = true)");

            entity.HasIndex(e => new { e.CompanyId, e.UserId, e.MutedUntil }, "ix_notification_preferences_muted").HasFilter("(muted_until IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.UserId }, "ix_notification_preferences_user");

            entity.HasIndex(e => new { e.CompanyId, e.NotificationPreferenceId }, "uq_notification_preferences_company_preference").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.UserId, e.DeliveryChannel, e.NotificationCategory }, "ux_notification_preferences_category")
                .IsUnique()
                .HasFilter("((preference_scope)::text = 'Category'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.UserId, e.DeliveryChannel, e.NotificationCode }, "ux_notification_preferences_code")
                .IsUnique()
                .HasFilter("((preference_scope)::text = 'Code'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.UserId, e.DeliveryChannel }, "ux_notification_preferences_global")
                .IsUnique()
                .HasFilter("((preference_scope)::text = 'Global'::text)");

            entity.Property(e => e.NotificationPreferenceId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("notification_preference_id");
            entity.Property(e => e.AllowFallback)
                .HasDefaultValue(true)
                .HasColumnName("allow_fallback");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeliveryChannel)
                .HasMaxLength(20)
                .HasColumnName("delivery_channel");
            entity.Property(e => e.DeliveryMode)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Immediate'::character varying")
                .HasColumnName("delivery_mode");
            entity.Property(e => e.DigestDayOfWeek).HasColumnName("digest_day_of_week");
            entity.Property(e => e.DigestFrequency)
                .HasMaxLength(20)
                .HasColumnName("digest_frequency");
            entity.Property(e => e.DigestTime).HasColumnName("digest_time");
            entity.Property(e => e.IsEnabled)
                .HasDefaultValue(true)
                .HasColumnName("is_enabled");
            entity.Property(e => e.IsMandatory).HasColumnName("is_mandatory");
            entity.Property(e => e.MinimumPriority)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Low'::character varying")
                .HasColumnName("minimum_priority");
            entity.Property(e => e.MutedUntil).HasColumnName("muted_until");
            entity.Property(e => e.NotificationCategory)
                .HasMaxLength(30)
                .HasColumnName("notification_category");
            entity.Property(e => e.NotificationCode)
                .HasMaxLength(100)
                .HasColumnName("notification_code");
            entity.Property(e => e.PreferenceScope)
                .HasMaxLength(20)
                .HasColumnName("preference_scope");
            entity.Property(e => e.QuietHoursEnabled).HasColumnName("quiet_hours_enabled");
            entity.Property(e => e.QuietHoursEnd).HasColumnName("quiet_hours_end");
            entity.Property(e => e.QuietHoursStart).HasColumnName("quiet_hours_start");
            entity.Property(e => e.TimeZone)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Africa/Tripoli'::character varying")
                .HasColumnName("time_zone");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedByUserId).HasColumnName("updated_by_user_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.NotificationPreferenceAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.UpdatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notification_preferences_updated_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.NotificationPreferenceAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.UserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notification_preferences_user");
        });

        modelBuilder.Entity<NotificationTemplate>(entity =>
        {
            entity.HasKey(e => e.NotificationTemplateId).HasName("notification_templates_pkey");

            entity.ToTable("notification_templates", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.NotificationCode, e.DeliveryChannel, e.LocaleCode }, "ix_notification_templates_active").HasFilter("((status)::text = 'Active'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.NotificationCategory, e.DeliveryChannel }, "ix_notification_templates_category");

            entity.HasIndex(e => new { e.CompanyId, e.CreatedByUserId }, "ix_notification_templates_created_by");

            entity.HasIndex(e => new { e.CompanyId, e.NotificationCode, e.DeliveryChannel, e.LocaleCode, e.Status }, "ix_notification_templates_lookup");

            entity.HasIndex(e => e.RequiredVariables, "ix_notification_templates_required_variables").HasMethod("gin");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.UpdatedAt }, "ix_notification_templates_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.NotificationTemplateId }, "uq_notification_templates_company_template").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.NotificationCode, e.DeliveryChannel, e.LocaleCode }, "uq_notification_templates_definition").IsUnique();

            entity.Property(e => e.NotificationTemplateId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("notification_template_id");
            entity.Property(e => e.ActionTargetTemplate)
                .HasMaxLength(1000)
                .HasColumnName("action_target_template");
            entity.Property(e => e.ActionType)
                .HasMaxLength(30)
                .HasDefaultValueSql("'None'::character varying")
                .HasColumnName("action_type");
            entity.Property(e => e.ActivatedAt).HasColumnName("activated_at");
            entity.Property(e => e.ActivatedByUserId).HasColumnName("activated_by_user_id");
            entity.Property(e => e.BodyTemplate).HasColumnName("body_template");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CurrentVersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("current_version_number");
            entity.Property(e => e.DefaultPriority)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Normal'::character varying")
                .HasColumnName("default_priority");
            entity.Property(e => e.DeliveryChannel)
                .HasMaxLength(20)
                .HasColumnName("delivery_channel");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.DisabledAt).HasColumnName("disabled_at");
            entity.Property(e => e.DisabledByUserId).HasColumnName("disabled_by_user_id");
            entity.Property(e => e.DisabledReason)
                .HasMaxLength(500)
                .HasColumnName("disabled_reason");
            entity.Property(e => e.HtmlBodyTemplate).HasColumnName("html_body_template");
            entity.Property(e => e.LocaleCode)
                .HasMaxLength(10)
                .HasDefaultValueSql("'ar-LY'::character varying")
                .HasColumnName("locale_code");
            entity.Property(e => e.NotificationCategory)
                .HasMaxLength(30)
                .HasColumnName("notification_category");
            entity.Property(e => e.NotificationCode)
                .HasMaxLength(100)
                .HasColumnName("notification_code");
            entity.Property(e => e.PayloadTemplate)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("payload_template");
            entity.Property(e => e.RequiredVariables)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("required_variables");
            entity.Property(e => e.RetiredAt).HasColumnName("retired_at");
            entity.Property(e => e.RetiredByUserId).HasColumnName("retired_by_user_id");
            entity.Property(e => e.RetirementReason)
                .HasMaxLength(500)
                .HasColumnName("retirement_reason");
            entity.Property(e => e.SampleData)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("sample_data");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SubjectTemplate)
                .HasMaxLength(500)
                .HasColumnName("subject_template");
            entity.Property(e => e.TemplateName)
                .HasMaxLength(200)
                .HasColumnName("template_name");
            entity.Property(e => e.TitleTemplate)
                .HasMaxLength(500)
                .HasColumnName("title_template");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedByUserId).HasColumnName("updated_by_user_id");

            entity.HasOne(d => d.Company).WithMany(p => p.NotificationTemplates)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notification_templates_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.NotificationTemplateAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ActivatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notification_templates_activated_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.NotificationTemplateAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notification_templates_created_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.NotificationTemplateAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.DisabledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notification_templates_disabled_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.NotificationTemplateAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RetiredByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notification_templates_retired_by");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.NotificationTemplateAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.UpdatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notification_templates_updated_by");
        });

        modelBuilder.Entity<NotificationTemplateVersion>(entity =>
        {
            entity.HasKey(e => e.NotificationTemplateVersionId).HasName("notification_template_versions_pkey");

            entity.ToTable("notification_template_versions", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ChangeType, e.CreatedAt }, "ix_notification_template_versions_change_type").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.NotificationCodeSnapshot, e.DeliveryChannelSnapshot, e.LocaleCodeSnapshot }, "ix_notification_template_versions_code");

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_notification_template_versions_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.RecordedByUserId, e.CreatedAt }, "ix_notification_template_versions_recorded_by").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.TemplateStatusSnapshot, e.CreatedAt }, "ix_notification_template_versions_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.NotificationTemplateId, e.VersionNumber }, "ix_notification_template_versions_template").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.NotificationTemplateVersionId }, "uq_notification_template_versions_company_version_id").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.NotificationTemplateId, e.VersionNumber }, "uq_notification_template_versions_template_version").IsUnique();

            entity.Property(e => e.NotificationTemplateVersionId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("notification_template_version_id");
            entity.Property(e => e.ActionTargetTemplateSnapshot)
                .HasMaxLength(1000)
                .HasColumnName("action_target_template_snapshot");
            entity.Property(e => e.ActionTypeSnapshot)
                .HasMaxLength(30)
                .HasColumnName("action_type_snapshot");
            entity.Property(e => e.BodyTemplateSnapshot).HasColumnName("body_template_snapshot");
            entity.Property(e => e.ChangeSummary)
                .HasMaxLength(1000)
                .HasColumnName("change_summary");
            entity.Property(e => e.ChangeType)
                .HasMaxLength(30)
                .HasColumnName("change_type");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DefaultPrioritySnapshot)
                .HasMaxLength(20)
                .HasColumnName("default_priority_snapshot");
            entity.Property(e => e.DeliveryChannelSnapshot)
                .HasMaxLength(20)
                .HasColumnName("delivery_channel_snapshot");
            entity.Property(e => e.DescriptionSnapshot)
                .HasMaxLength(1000)
                .HasColumnName("description_snapshot");
            entity.Property(e => e.HtmlBodyTemplateSnapshot).HasColumnName("html_body_template_snapshot");
            entity.Property(e => e.LocaleCodeSnapshot)
                .HasMaxLength(10)
                .HasColumnName("locale_code_snapshot");
            entity.Property(e => e.NotificationCategorySnapshot)
                .HasMaxLength(30)
                .HasColumnName("notification_category_snapshot");
            entity.Property(e => e.NotificationCodeSnapshot)
                .HasMaxLength(100)
                .HasColumnName("notification_code_snapshot");
            entity.Property(e => e.NotificationTemplateId).HasColumnName("notification_template_id");
            entity.Property(e => e.PayloadTemplateSnapshot)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("payload_template_snapshot");
            entity.Property(e => e.PreviousVersionNumber).HasColumnName("previous_version_number");
            entity.Property(e => e.RecordedByUserId).HasColumnName("recorded_by_user_id");
            entity.Property(e => e.RequiredVariablesSnapshot)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("required_variables_snapshot");
            entity.Property(e => e.RestoredFromVersionNumber).HasColumnName("restored_from_version_number");
            entity.Property(e => e.SampleDataSnapshot)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("sample_data_snapshot");
            entity.Property(e => e.SourceTemplateUpdatedAt).HasColumnName("source_template_updated_at");
            entity.Property(e => e.SubjectTemplateSnapshot)
                .HasMaxLength(500)
                .HasColumnName("subject_template_snapshot");
            entity.Property(e => e.TemplateNameSnapshot)
                .HasMaxLength(200)
                .HasColumnName("template_name_snapshot");
            entity.Property(e => e.TemplateStatusSnapshot)
                .HasMaxLength(20)
                .HasColumnName("template_status_snapshot");
            entity.Property(e => e.TitleTemplateSnapshot)
                .HasMaxLength(500)
                .HasColumnName("title_template_snapshot");
            entity.Property(e => e.VersionNumber).HasColumnName("version_number");

            entity.HasOne(d => d.NotificationTemplate).WithMany(p => p.NotificationTemplateVersions)
                .HasPrincipalKey(p => new { p.CompanyId, p.NotificationTemplateId })
                .HasForeignKey(d => new { d.CompanyId, d.NotificationTemplateId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notification_template_versions_template");

            entity.HasOne(d => d.AppUser).WithMany(p => p.NotificationTemplateVersions)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecordedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notification_template_versions_recorded_by");

            entity.HasOne(d => d.NotificationTemplateVersionNavigation).WithMany(p => p.InverseNotificationTemplateVersionNavigation)
                .HasPrincipalKey(p => new { p.CompanyId, p.NotificationTemplateId, p.VersionNumber })
                .HasForeignKey(d => new { d.CompanyId, d.NotificationTemplateId, d.PreviousVersionNumber })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notification_template_versions_previous");

            entity.HasOne(d => d.NotificationTemplateVersion1).WithMany(p => p.InverseNotificationTemplateVersion1)
                .HasPrincipalKey(p => new { p.CompanyId, p.NotificationTemplateId, p.VersionNumber })
                .HasForeignKey(d => new { d.CompanyId, d.NotificationTemplateId, d.RestoredFromVersionNumber })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notification_template_versions_restored_from");
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(e => e.OutboxMessageId).HasName("outbox_messages_pkey");

            entity.ToTable("outbox_messages", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.AggregateType, e.AggregateId, e.AggregateVersionNumber }, "ix_outbox_messages_aggregate").HasFilter("(aggregate_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.CausationId }, "ix_outbox_messages_causation").HasFilter("(causation_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_outbox_messages_company_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_outbox_messages_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.DeadLetteredAt }, "ix_outbox_messages_dead_lettered")
                .IsDescending(false, true)
                .HasFilter("((status)::text = 'DeadLettered'::text)");

            entity.HasIndex(e => new { e.DestinationType, e.DestinationName, e.Status }, "ix_outbox_messages_destination");

            entity.HasIndex(e => new { e.Priority, e.AvailableAt, e.MessageSequence }, "ix_outbox_messages_pending")
                .IsDescending(true, false, false)
                .HasFilter("((status)::text = 'Pending'::text)");

            entity.HasIndex(e => e.LeaseExpiresAt, "ix_outbox_messages_processing_lease").HasFilter("((status)::text = 'Processing'::text)");

            entity.HasIndex(e => new { e.NextRetryAt, e.Priority, e.MessageSequence }, "ix_outbox_messages_retry")
                .IsDescending(false, true, false)
                .HasFilter("(((status)::text = 'Failed'::text) AND (next_retry_at IS NOT NULL))");

            entity.HasIndex(e => new { e.CompanyId, e.MessageType, e.CreatedAt }, "ix_outbox_messages_type").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.OutboxMessageId }, "uq_outbox_messages_company_message").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.IdempotencyKey }, "uq_outbox_messages_idempotency").IsUnique();

            entity.HasIndex(e => e.MessageSequence, "uq_outbox_messages_sequence").IsUnique();

            entity.Property(e => e.OutboxMessageId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("outbox_message_id");
            entity.Property(e => e.AggregateId).HasColumnName("aggregate_id");
            entity.Property(e => e.AggregateType)
                .HasMaxLength(100)
                .HasColumnName("aggregate_type");
            entity.Property(e => e.AggregateVersionNumber).HasColumnName("aggregate_version_number");
            entity.Property(e => e.AttemptCount).HasColumnName("attempt_count");
            entity.Property(e => e.AvailableAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("available_at");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByType)
                .HasMaxLength(30)
                .HasColumnName("cancelled_by_type");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CausationId).HasColumnName("causation_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeadLetteredAt).HasColumnName("dead_lettered_at");
            entity.Property(e => e.DestinationName)
                .HasMaxLength(200)
                .HasColumnName("destination_name");
            entity.Property(e => e.DestinationType)
                .HasMaxLength(30)
                .HasColumnName("destination_type");
            entity.Property(e => e.EventCategory)
                .HasMaxLength(40)
                .HasColumnName("event_category");
            entity.Property(e => e.Headers)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("headers");
            entity.Property(e => e.IdempotencyKey)
                .HasMaxLength(200)
                .HasColumnName("idempotency_key");
            entity.Property(e => e.LastAttemptAt).HasColumnName("last_attempt_at");
            entity.Property(e => e.LastErrorCode)
                .HasMaxLength(100)
                .HasColumnName("last_error_code");
            entity.Property(e => e.LastErrorMessage)
                .HasMaxLength(2000)
                .HasColumnName("last_error_message");
            entity.Property(e => e.LastFailedAt).HasColumnName("last_failed_at");
            entity.Property(e => e.LeaseExpiresAt).HasColumnName("lease_expires_at");
            entity.Property(e => e.LockToken).HasColumnName("lock_token");
            entity.Property(e => e.LockedBy)
                .HasMaxLength(200)
                .HasColumnName("locked_by");
            entity.Property(e => e.MaxAttempts)
                .HasDefaultValue(10)
                .HasColumnName("max_attempts");
            entity.Property(e => e.MessageSequence)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn()
                .HasColumnName("message_sequence");
            entity.Property(e => e.MessageType)
                .HasMaxLength(150)
                .HasColumnName("message_type");
            entity.Property(e => e.MessageVersion)
                .HasDefaultValue(1)
                .HasColumnName("message_version");
            entity.Property(e => e.NextRetryAt).HasColumnName("next_retry_at");
            entity.Property(e => e.OccurredAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("occurred_at");
            entity.Property(e => e.Payload)
                .HasColumnType("jsonb")
                .HasColumnName("payload");
            entity.Property(e => e.Priority)
                .HasDefaultValue((short)5)
                .HasColumnName("priority");
            entity.Property(e => e.ProcessingStartedAt).HasColumnName("processing_started_at");
            entity.Property(e => e.ProducerType)
                .HasMaxLength(30)
                .HasDefaultValueSql("'System'::character varying")
                .HasColumnName("producer_type");
            entity.Property(e => e.ProducerUserId).HasColumnName("producer_user_id");
            entity.Property(e => e.ProviderMessageId)
                .HasMaxLength(250)
                .HasColumnName("provider_message_id");
            entity.Property(e => e.PublishedAt).HasColumnName("published_at");
            entity.Property(e => e.RoutingKey)
                .HasMaxLength(200)
                .HasColumnName("routing_key");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Company).WithMany(p => p.OutboxMessages)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_outbox_messages_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.OutboxMessageAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_outbox_messages_cancelled_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.OutboxMessageAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ProducerUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_outbox_messages_producer_user");
        });

        modelBuilder.Entity<OutboxMessageAttempt>(entity =>
        {
            entity.HasKey(e => e.OutboxMessageAttemptId).HasName("outbox_message_attempts_pkey");

            entity.ToTable("outbox_message_attempts", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_outbox_attempts_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.FailureCategory, e.CompletedAt }, "ix_outbox_attempts_failures")
                .IsDescending(false, false, true)
                .HasFilter("((attempt_status)::text = ANY ((ARRAY['Failed'::character varying, 'Abandoned'::character varying])::text[]))");

            entity.HasIndex(e => new { e.CompanyId, e.OutboxMessageId, e.AttemptNumber }, "ix_outbox_attempts_message").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.ProviderName, e.ProviderMessageId }, "ix_outbox_attempts_provider_message").HasFilter("(provider_message_id IS NOT NULL)");

            entity.HasIndex(e => e.NextRetryAt, "ix_outbox_attempts_retry").HasFilter("((retry_decision)::text = 'Retry'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.AttemptStatus, e.CompletedAt }, "ix_outbox_attempts_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.WorkerName, e.CompletedAt }, "ix_outbox_attempts_worker").IsDescending(false, true);

            entity.HasIndex(e => new { e.CompanyId, e.OutboxMessageAttemptId }, "uq_outbox_attempts_company_id").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.OutboxMessageId, e.LockToken }, "uq_outbox_attempts_message_lock").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.OutboxMessageId, e.AttemptNumber }, "uq_outbox_attempts_message_number").IsUnique();

            entity.Property(e => e.OutboxMessageAttemptId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("outbox_message_attempt_id");
            entity.Property(e => e.AttemptNumber).HasColumnName("attempt_number");
            entity.Property(e => e.AttemptStatus)
                .HasMaxLength(20)
                .HasColumnName("attempt_status");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DurationMilliseconds).HasColumnName("duration_milliseconds");
            entity.Property(e => e.ErrorCode)
                .HasMaxLength(100)
                .HasColumnName("error_code");
            entity.Property(e => e.ErrorMessage)
                .HasMaxLength(2000)
                .HasColumnName("error_message");
            entity.Property(e => e.FailureCategory)
                .HasMaxLength(30)
                .HasColumnName("failure_category");
            entity.Property(e => e.LockToken).HasColumnName("lock_token");
            entity.Property(e => e.NextRetryAt).HasColumnName("next_retry_at");
            entity.Property(e => e.OutboxMessageId).HasColumnName("outbox_message_id");
            entity.Property(e => e.ProviderMessageId)
                .HasMaxLength(250)
                .HasColumnName("provider_message_id");
            entity.Property(e => e.ProviderName)
                .HasMaxLength(100)
                .HasColumnName("provider_name");
            entity.Property(e => e.RequestMetadata)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("request_metadata");
            entity.Property(e => e.ResponseCode)
                .HasMaxLength(100)
                .HasColumnName("response_code");
            entity.Property(e => e.ResponseMetadata)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("response_metadata");
            entity.Property(e => e.RetryDecision)
                .HasMaxLength(20)
                .HasColumnName("retry_decision");
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
            entity.Property(e => e.WorkerName)
                .HasMaxLength(200)
                .HasColumnName("worker_name");

            entity.HasOne(d => d.OutboxMessage).WithMany(p => p.OutboxMessageAttempts)
                .HasPrincipalKey(p => new { p.CompanyId, p.OutboxMessageId })
                .HasForeignKey(d => new { d.CompanyId, d.OutboxMessageId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_outbox_attempts_message");
        });

        modelBuilder.Entity<OwnerPaymentProjectAllocation>(entity =>
        {
            entity.HasKey(e => e.OwnerPaymentProjectAllocationId).HasName("owner_payment_project_allocations_pkey");

            entity.ToTable("owner_payment_project_allocations", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectOwnerPaymentId }, "ix_owner_payment_project_allocations_payment");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectId }, "ix_owner_payment_project_allocations_project");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectId, e.CreatedAt }, "ix_owner_payment_project_allocations_project_created");

            entity.HasIndex(e => new { e.CompanyId, e.OwnerPaymentProjectAllocationId }, "uq_owner_payment_project_allocations_company_allocation").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ProjectOwnerPaymentId, e.ProjectId }, "uq_owner_payment_project_allocations_payment_project").IsUnique();

            entity.Property(e => e.OwnerPaymentProjectAllocationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("owner_payment_project_allocation_id");
            entity.Property(e => e.AllocatedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("allocated_amount");
            entity.Property(e => e.AllocatedByUserId).HasColumnName("allocated_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.ProjectOwnerPaymentId).HasColumnName("project_owner_payment_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AppUser).WithMany(p => p.OwnerPaymentProjectAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.AllocatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_owner_payment_project_allocations_allocated_by");

            entity.HasOne(d => d.Project).WithMany(p => p.OwnerPaymentProjectAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.ProjectId })
                .HasForeignKey(d => new { d.CompanyId, d.ProjectId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_owner_payment_project_allocations_project");

            entity.HasOne(d => d.ProjectOwnerPayment).WithMany(p => p.OwnerPaymentProjectAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.ProjectOwnerPaymentId })
                .HasForeignKey(d => new { d.CompanyId, d.ProjectOwnerPaymentId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_owner_payment_project_allocations_payment");
        });

        modelBuilder.Entity<OwnerPaymentRefund>(entity =>
        {
            entity.HasKey(e => e.OwnerPaymentRefundId).HasName("owner_payment_refunds_pkey");

            entity.ToTable("owner_payment_refunds", "ahdah");

            entity.HasIndex(e => e.CompanyId, "ix_owner_payment_refunds_company");

            entity.HasIndex(e => new { e.CompanyId, e.RefundDate }, "ix_owner_payment_refunds_date");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectOwnerPaymentId }, "ix_owner_payment_refunds_original_payment").HasFilter("(project_owner_payment_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectOwnerId }, "ix_owner_payment_refunds_owner");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectId }, "ix_owner_payment_refunds_project");

            entity.HasIndex(e => new { e.CompanyId, e.Status }, "ix_owner_payment_refunds_status");

            entity.HasIndex(e => new { e.CompanyId, e.OwnerPaymentRefundId }, "uq_owner_payment_refunds_company_refund").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.RefundNumber }, "uq_owner_payment_refunds_number").IsUnique();

            entity.Property(e => e.OwnerPaymentRefundId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("owner_payment_refund_id");
            entity.Property(e => e.BankName)
                .HasMaxLength(150)
                .HasColumnName("bank_name");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.ProjectOwnerId).HasColumnName("project_owner_id");
            entity.Property(e => e.ProjectOwnerPaymentId).HasColumnName("project_owner_payment_id");
            entity.Property(e => e.ProofFileUrl)
                .HasMaxLength(1000)
                .HasColumnName("proof_file_url");
            entity.Property(e => e.Reason)
                .HasMaxLength(1000)
                .HasColumnName("reason");
            entity.Property(e => e.ReferenceNumber)
                .HasMaxLength(150)
                .HasColumnName("reference_number");
            entity.Property(e => e.RefundAmount)
                .HasPrecision(18, 2)
                .HasColumnName("refund_amount");
            entity.Property(e => e.RefundDate).HasColumnName("refund_date");
            entity.Property(e => e.RefundMethod)
                .HasMaxLength(30)
                .HasColumnName("refund_method");
            entity.Property(e => e.RefundNumber)
                .HasMaxLength(50)
                .HasColumnName("refund_number");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.RequestedByUserId).HasColumnName("requested_by_user_id");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.ReviewedAt).HasColumnName("reviewed_at");
            entity.Property(e => e.ReviewedByUserId).HasColumnName("reviewed_by_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'PendingApproval'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.OwnerPaymentRefundAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_owner_payment_refunds_cancelled_by");

            entity.HasOne(d => d.Project).WithMany(p => p.OwnerPaymentRefunds)
                .HasPrincipalKey(p => new { p.CompanyId, p.ProjectId })
                .HasForeignKey(d => new { d.CompanyId, d.ProjectId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_owner_payment_refunds_project");

            entity.HasOne(d => d.ProjectOwner).WithMany(p => p.OwnerPaymentRefunds)
                .HasPrincipalKey(p => new { p.CompanyId, p.ProjectOwnerId })
                .HasForeignKey(d => new { d.CompanyId, d.ProjectOwnerId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_owner_payment_refunds_owner");

            entity.HasOne(d => d.ProjectOwnerPayment).WithMany(p => p.OwnerPaymentRefunds)
                .HasPrincipalKey(p => new { p.CompanyId, p.ProjectOwnerPaymentId })
                .HasForeignKey(d => new { d.CompanyId, d.ProjectOwnerPaymentId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_owner_payment_refunds_original_payment");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.OwnerPaymentRefundAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RequestedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_owner_payment_refunds_requested_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.OwnerPaymentRefundAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_owner_payment_refunds_reversed_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.OwnerPaymentRefundAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReviewedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_owner_payment_refunds_reviewed_by");
        });

        modelBuilder.Entity<OwnerRefundFundingSource>(entity =>
        {
            entity.HasKey(e => e.OwnerRefundFundingSourceId).HasName("owner_refund_funding_sources_pkey");

            entity.ToTable("owner_refund_funding_sources", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.AllocatedByUserId }, "ix_owner_refund_funding_sources_allocated_by");

            entity.HasIndex(e => new { e.CompanyId, e.OwnerPaymentRefundId }, "ix_owner_refund_funding_sources_refund");

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourceId }, "ix_owner_refund_funding_sources_source");

            entity.HasIndex(e => new { e.CompanyId, e.OwnerRefundFundingSourceId }, "uq_owner_refund_funding_sources_company_allocation").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.OwnerPaymentRefundId, e.FundingSourceId }, "uq_owner_refund_funding_sources_refund_source").IsUnique();

            entity.Property(e => e.OwnerRefundFundingSourceId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("owner_refund_funding_source_id");
            entity.Property(e => e.AllocatedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("allocated_amount");
            entity.Property(e => e.AllocatedByUserId).HasColumnName("allocated_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.FundingSourceId).HasColumnName("funding_source_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.OwnerPaymentRefundId).HasColumnName("owner_payment_refund_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AppUser).WithMany(p => p.OwnerRefundFundingSources)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.AllocatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_owner_refund_funding_sources_allocated_by");

            entity.HasOne(d => d.FundingSource).WithMany(p => p.OwnerRefundFundingSources)
                .HasPrincipalKey(p => new { p.CompanyId, p.FundingSourceId })
                .HasForeignKey(d => new { d.CompanyId, d.FundingSourceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_owner_refund_funding_sources_source");

            entity.HasOne(d => d.OwnerPaymentRefund).WithMany(p => p.OwnerRefundFundingSources)
                .HasPrincipalKey(p => new { p.CompanyId, p.OwnerPaymentRefundId })
                .HasForeignKey(d => new { d.CompanyId, d.OwnerPaymentRefundId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_owner_refund_funding_sources_refund");
        });

        modelBuilder.Entity<PersonalClaim>(entity =>
        {
            entity.HasKey(e => e.PersonalClaimId).HasName("personal_claims_pkey");

            entity.ToTable("personal_claims", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ClaimantUserId }, "ix_personal_claims_claimant");

            entity.HasIndex(e => new { e.CompanyId, e.ClaimantUserId, e.Status }, "ix_personal_claims_claimant_status");

            entity.HasIndex(e => new { e.CompanyId, e.DueDate }, "ix_personal_claims_due_date").HasFilter("((due_date IS NOT NULL) AND ((status)::text = ANY ((ARRAY['Open'::character varying, 'PartiallySettled'::character varying])::text[])))");

            entity.HasIndex(e => new { e.CompanyId, e.ClaimantUserId, e.OutstandingAmount }, "ix_personal_claims_outstanding").HasFilter("((status)::text = ANY ((ARRAY['Open'::character varying, 'PartiallySettled'::character varying])::text[]))");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectId }, "ix_personal_claims_project").HasFilter("(project_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.SourceType }, "ix_personal_claims_source_type");

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimId }, "uq_personal_claims_company_claim").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ClaimNumber }, "uq_personal_claims_number").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseId }, "ux_personal_claims_expense")
                .IsUnique()
                .HasFilter("(expense_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.ManagerContributionId }, "ux_personal_claims_manager_contribution")
                .IsUnique()
                .HasFilter("(manager_contribution_id IS NOT NULL)");

            entity.Property(e => e.PersonalClaimId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("personal_claim_id");
            entity.Property(e => e.AdjustmentAmount)
                .HasPrecision(18, 2)
                .HasColumnName("adjustment_amount");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.ClaimAmount)
                .HasPrecision(18, 2)
                .HasColumnName("claim_amount");
            entity.Property(e => e.ClaimDate).HasColumnName("claim_date");
            entity.Property(e => e.ClaimNumber)
                .HasMaxLength(50)
                .HasColumnName("claim_number");
            entity.Property(e => e.ClaimantUserId).HasColumnName("claimant_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'LYD'::character varying")
                .HasColumnName("currency_code");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.ExpenseId).HasColumnName("expense_id");
            entity.Property(e => e.ManagerContributionId).HasColumnName("manager_contribution_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.OutstandingAmount)
                .HasPrecision(18, 2)
                .HasComputedColumnSql("((((claim_amount + adjustment_amount) - paid_amount) - reduction_amount) - written_off_amount)", true)
                .HasColumnName("outstanding_amount");
            entity.Property(e => e.PaidAmount)
                .HasPrecision(18, 2)
                .HasColumnName("paid_amount");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.RecordedByUserId).HasColumnName("recorded_by_user_id");
            entity.Property(e => e.ReductionAmount)
                .HasPrecision(18, 2)
                .HasColumnName("reduction_amount");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.SettledAt).HasColumnName("settled_at");
            entity.Property(e => e.SourceType)
                .HasMaxLength(40)
                .HasColumnName("source_type");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Open'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");
            entity.Property(e => e.WrittenOffAmount)
                .HasPrecision(18, 2)
                .HasColumnName("written_off_amount");

            entity.HasOne(d => d.AppUser).WithMany(p => p.PersonalClaimAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claims_cancelled_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.PersonalClaimAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ClaimantUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claims_claimant");

            entity.HasOne(d => d.Expense).WithOne(p => p.PersonalClaim)
                .HasPrincipalKey<Expense>(p => new { p.CompanyId, p.ExpenseId })
                .HasForeignKey<PersonalClaim>(d => new { d.CompanyId, d.ExpenseId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claims_expense");

            entity.HasOne(d => d.ManagerContribution).WithOne(p => p.PersonalClaim)
                .HasPrincipalKey<ManagerContribution>(p => new { p.CompanyId, p.ManagerContributionId })
                .HasForeignKey<PersonalClaim>(d => new { d.CompanyId, d.ManagerContributionId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claims_manager_contribution");

            entity.HasOne(d => d.Project).WithMany(p => p.PersonalClaims)
                .HasPrincipalKey(p => new { p.CompanyId, p.ProjectId })
                .HasForeignKey(d => new { d.CompanyId, d.ProjectId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claims_project");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.PersonalClaimAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecordedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claims_recorded_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.PersonalClaimAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claims_reversed_by");
        });

        modelBuilder.Entity<PersonalClaimAdjustment>(entity =>
        {
            entity.HasKey(e => e.PersonalClaimAdjustmentId).HasName("personal_claim_adjustments_pkey");

            entity.ToTable("personal_claim_adjustments", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimId }, "ix_personal_claim_adjustments_claim");

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimId, e.AdjustmentDate }, "ix_personal_claim_adjustments_claim_date");

            entity.HasIndex(e => new { e.CompanyId, e.CreatedByUserId }, "ix_personal_claim_adjustments_created_by");

            entity.HasIndex(e => new { e.CompanyId, e.SubmittedAt }, "ix_personal_claim_adjustments_pending").HasFilter("((status)::text = 'PendingApproval'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_personal_claim_adjustments_status");

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimAdjustmentId }, "uq_personal_claim_adjustments_company_adjustment").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.AdjustmentNumber }, "uq_personal_claim_adjustments_number").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimId }, "ux_personal_claim_adjustments_one_pending")
                .IsUnique()
                .HasFilter("((status)::text = 'PendingApproval'::text)");

            entity.Property(e => e.PersonalClaimAdjustmentId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("personal_claim_adjustment_id");
            entity.Property(e => e.AdjustmentAmount)
                .HasPrecision(18, 2)
                .HasColumnName("adjustment_amount");
            entity.Property(e => e.AdjustmentDate).HasColumnName("adjustment_date");
            entity.Property(e => e.AdjustmentNumber)
                .HasMaxLength(50)
                .HasColumnName("adjustment_number");
            entity.Property(e => e.AdjustmentType)
                .HasMaxLength(30)
                .HasColumnName("adjustment_type");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedByUserId).HasColumnName("approved_by_user_id");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.ClaimVersionNumber).HasColumnName("claim_version_number");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.PersonalClaimId).HasColumnName("personal_claim_id");
            entity.Property(e => e.ProofFileUrl)
                .HasMaxLength(1000)
                .HasColumnName("proof_file_url");
            entity.Property(e => e.ReasonType)
                .HasMaxLength(40)
                .HasColumnName("reason_type");
            entity.Property(e => e.RejectedAt).HasColumnName("rejected_at");
            entity.Property(e => e.RejectedByUserId).HasColumnName("rejected_by_user_id");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SubmittedAt).HasColumnName("submitted_at");
            entity.Property(e => e.SubmittedByUserId).HasColumnName("submitted_by_user_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.PersonalClaimAdjustmentAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ApprovedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_adjustments_approved_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.PersonalClaimAdjustmentAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_adjustments_cancelled_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.PersonalClaimAdjustmentAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_adjustments_created_by");

            entity.HasOne(d => d.PersonalClaim).WithOne(p => p.PersonalClaimAdjustment)
                .HasPrincipalKey<PersonalClaim>(p => new { p.CompanyId, p.PersonalClaimId })
                .HasForeignKey<PersonalClaimAdjustment>(d => new { d.CompanyId, d.PersonalClaimId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_adjustments_claim");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.PersonalClaimAdjustmentAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RejectedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_adjustments_rejected_by");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.PersonalClaimAdjustmentAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_adjustments_reversed_by");

            entity.HasOne(d => d.AppUser4).WithMany(p => p.PersonalClaimAdjustmentAppUser4s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.SubmittedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_adjustments_submitted_by");
        });

        modelBuilder.Entity<PersonalClaimLedgerEntry>(entity =>
        {
            entity.HasKey(e => e.PersonalClaimLedgerEntryId).HasName("personal_claim_ledger_entries_pkey");

            entity.ToTable("personal_claim_ledger_entries", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimId, e.ClaimVersionNumber }, "ix_personal_claim_ledger_claim");

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_personal_claim_ledger_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.EntryType }, "ix_personal_claim_ledger_entry_type");

            entity.HasIndex(e => new { e.CompanyId, e.OccurredAt }, "ix_personal_claim_ledger_occurred_at");

            entity.HasIndex(e => new { e.CompanyId, e.ReferenceType, e.ReferenceId }, "ix_personal_claim_ledger_reference").HasFilter("(reference_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimId, e.ClaimVersionNumber }, "uq_personal_claim_ledger_claim_version").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimLedgerEntryId }, "uq_personal_claim_ledger_company_entry").IsUnique();

            entity.Property(e => e.PersonalClaimLedgerEntryId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("personal_claim_ledger_entry_id");
            entity.Property(e => e.AdjustmentAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("adjustment_after_amount");
            entity.Property(e => e.ClaimAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("claim_after_amount");
            entity.Property(e => e.ClaimStatusAfter)
                .HasMaxLength(30)
                .HasColumnName("claim_status_after");
            entity.Property(e => e.ClaimVersionNumber).HasColumnName("claim_version_number");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeltaAdjustmentAmount)
                .HasPrecision(18, 2)
                .HasColumnName("delta_adjustment_amount");
            entity.Property(e => e.DeltaClaimAmount)
                .HasPrecision(18, 2)
                .HasColumnName("delta_claim_amount");
            entity.Property(e => e.DeltaPaidAmount)
                .HasPrecision(18, 2)
                .HasColumnName("delta_paid_amount");
            entity.Property(e => e.DeltaReductionAmount)
                .HasPrecision(18, 2)
                .HasColumnName("delta_reduction_amount");
            entity.Property(e => e.DeltaWrittenOffAmount)
                .HasPrecision(18, 2)
                .HasColumnName("delta_written_off_amount");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.EntryType)
                .HasMaxLength(40)
                .HasColumnName("entry_type");
            entity.Property(e => e.OccurredAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("occurred_at");
            entity.Property(e => e.OutstandingAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("outstanding_after_amount");
            entity.Property(e => e.PaidAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("paid_after_amount");
            entity.Property(e => e.PerformedByUserId).HasColumnName("performed_by_user_id");
            entity.Property(e => e.PersonalClaimId).HasColumnName("personal_claim_id");
            entity.Property(e => e.ReductionAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("reduction_after_amount");
            entity.Property(e => e.ReferenceId).HasColumnName("reference_id");
            entity.Property(e => e.ReferenceType)
                .HasMaxLength(40)
                .HasColumnName("reference_type");
            entity.Property(e => e.WrittenOffAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("written_off_after_amount");

            entity.HasOne(d => d.AppUser).WithMany(p => p.PersonalClaimLedgerEntries)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.PerformedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_ledger_performed_by");

            entity.HasOne(d => d.PersonalClaim).WithMany(p => p.PersonalClaimLedgerEntries)
                .HasPrincipalKey(p => new { p.CompanyId, p.PersonalClaimId })
                .HasForeignKey(d => new { d.CompanyId, d.PersonalClaimId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_ledger_claim");
        });

        modelBuilder.Entity<PersonalClaimPayment>(entity =>
        {
            entity.HasKey(e => e.PersonalClaimPaymentId).HasName("personal_claim_payments_pkey");

            entity.ToTable("personal_claim_payments", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ClaimantUserId }, "ix_personal_claim_payments_claimant");

            entity.HasIndex(e => new { e.CompanyId, e.ClaimantUserId, e.PaymentDate }, "ix_personal_claim_payments_claimant_date");

            entity.HasIndex(e => new { e.CompanyId, e.PaymentDate }, "ix_personal_claim_payments_payment_date");

            entity.HasIndex(e => new { e.CompanyId, e.SubmittedAt }, "ix_personal_claim_payments_pending").HasFilter("((status)::text = 'PendingApproval'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.ReferenceNumber }, "ix_personal_claim_payments_reference").HasFilter("(reference_number IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_personal_claim_payments_status");

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimPaymentId }, "uq_personal_claim_payments_company_payment").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.PaymentNumber }, "uq_personal_claim_payments_number").IsUnique();

            entity.Property(e => e.PersonalClaimPaymentId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("personal_claim_payment_id");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.ClaimantUserId).HasColumnName("claimant_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.ConfirmedAt).HasColumnName("confirmed_at");
            entity.Property(e => e.ConfirmedByUserId).HasColumnName("confirmed_by_user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'LYD'::character varying")
                .HasColumnName("currency_code");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.FeeAmount)
                .HasPrecision(18, 2)
                .HasColumnName("fee_amount");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.PaymentAmount)
                .HasPrecision(18, 2)
                .HasColumnName("payment_amount");
            entity.Property(e => e.PaymentDate).HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(30)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentNumber)
                .HasMaxLength(50)
                .HasColumnName("payment_number");
            entity.Property(e => e.ProofFileUrl)
                .HasMaxLength(1000)
                .HasColumnName("proof_file_url");
            entity.Property(e => e.RecipientAccountNumber)
                .HasMaxLength(100)
                .HasColumnName("recipient_account_number");
            entity.Property(e => e.RecipientBankName)
                .HasMaxLength(150)
                .HasColumnName("recipient_bank_name");
            entity.Property(e => e.RecipientIban)
                .HasMaxLength(34)
                .HasColumnName("recipient_iban");
            entity.Property(e => e.RecipientNameSnapshot)
                .HasMaxLength(200)
                .HasColumnName("recipient_name_snapshot");
            entity.Property(e => e.ReferenceNumber)
                .HasMaxLength(150)
                .HasColumnName("reference_number");
            entity.Property(e => e.RejectedAt).HasColumnName("rejected_at");
            entity.Property(e => e.RejectedByUserId).HasColumnName("rejected_by_user_id");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SubmittedAt).HasColumnName("submitted_at");
            entity.Property(e => e.SubmittedByUserId).HasColumnName("submitted_by_user_id");
            entity.Property(e => e.TotalDisbursedAmount)
                .HasPrecision(18, 2)
                .HasComputedColumnSql("(payment_amount + fee_amount)", true)
                .HasColumnName("total_disbursed_amount");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");
            entity.Property(e => e.WalletNumber)
                .HasMaxLength(20)
                .HasColumnName("wallet_number");
            entity.Property(e => e.WalletProvider)
                .HasMaxLength(100)
                .HasColumnName("wallet_provider");

            entity.HasOne(d => d.AppUser).WithMany(p => p.PersonalClaimPaymentAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_payments_cancelled_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.PersonalClaimPaymentAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ClaimantUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_payments_claimant");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.PersonalClaimPaymentAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ConfirmedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_payments_confirmed_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.PersonalClaimPaymentAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_payments_created_by");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.PersonalClaimPaymentAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RejectedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_payments_rejected_by");

            entity.HasOne(d => d.AppUser4).WithMany(p => p.PersonalClaimPaymentAppUser4s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_payments_reversed_by");

            entity.HasOne(d => d.AppUser5).WithMany(p => p.PersonalClaimPaymentAppUser5s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.SubmittedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_payments_submitted_by");
        });

        modelBuilder.Entity<PersonalClaimPaymentAllocation>(entity =>
        {
            entity.HasKey(e => e.PersonalClaimPaymentAllocationId).HasName("personal_claim_payment_allocations_pkey");

            entity.ToTable("personal_claim_payment_allocations", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.AllocatedByUserId }, "ix_personal_claim_payment_allocations_allocated_by");

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimId }, "ix_personal_claim_payment_allocations_claim");

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimId, e.CreatedAt }, "ix_personal_claim_payment_allocations_claim_created");

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimPaymentId }, "ix_personal_claim_payment_allocations_payment");

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimPaymentAllocationId }, "uq_personal_claim_payment_allocations_company_allocation").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimPaymentId, e.PersonalClaimId }, "uq_personal_claim_payment_allocations_payment_claim").IsUnique();

            entity.Property(e => e.PersonalClaimPaymentAllocationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("personal_claim_payment_allocation_id");
            entity.Property(e => e.AllocatedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("allocated_amount");
            entity.Property(e => e.AllocatedByUserId).HasColumnName("allocated_by_user_id");
            entity.Property(e => e.ClaimVersionNumber).HasColumnName("claim_version_number");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.PersonalClaimId).HasColumnName("personal_claim_id");
            entity.Property(e => e.PersonalClaimPaymentId).HasColumnName("personal_claim_payment_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AppUser).WithMany(p => p.PersonalClaimPaymentAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.AllocatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_payment_allocations_allocated_by");

            entity.HasOne(d => d.PersonalClaim).WithMany(p => p.PersonalClaimPaymentAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.PersonalClaimId })
                .HasForeignKey(d => new { d.CompanyId, d.PersonalClaimId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_payment_allocations_claim");

            entity.HasOne(d => d.PersonalClaimPayment).WithMany(p => p.PersonalClaimPaymentAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.PersonalClaimPaymentId })
                .HasForeignKey(d => new { d.CompanyId, d.PersonalClaimPaymentId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_payment_allocations_payment");
        });

        modelBuilder.Entity<PersonalClaimPaymentFundingSource>(entity =>
        {
            entity.HasKey(e => e.PersonalClaimPaymentFundingSourceId).HasName("personal_claim_payment_funding_sources_pkey");

            entity.ToTable("personal_claim_payment_funding_sources", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.AllocatedByUserId }, "ix_personal_claim_payment_funding_sources_allocated_by");

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimPaymentId }, "ix_personal_claim_payment_funding_sources_payment");

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourceId }, "ix_personal_claim_payment_funding_sources_source");

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourceId, e.CreatedAt }, "ix_personal_claim_payment_funding_sources_source_created");

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimPaymentFundingSourceId }, "uq_personal_claim_payment_funding_sources_company_allocation").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimPaymentId, e.FundingSourceId }, "uq_personal_claim_payment_funding_sources_payment_source").IsUnique();

            entity.Property(e => e.PersonalClaimPaymentFundingSourceId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("personal_claim_payment_funding_source_id");
            entity.Property(e => e.AllocatedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("allocated_amount");
            entity.Property(e => e.AllocatedByUserId).HasColumnName("allocated_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.FundingSourceId).HasColumnName("funding_source_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.PersonalClaimPaymentId).HasColumnName("personal_claim_payment_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AppUser).WithMany(p => p.PersonalClaimPaymentFundingSources)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.AllocatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_payment_funding_sources_allocated_by");

            entity.HasOne(d => d.FundingSource).WithMany(p => p.PersonalClaimPaymentFundingSources)
                .HasPrincipalKey(p => new { p.CompanyId, p.FundingSourceId })
                .HasForeignKey(d => new { d.CompanyId, d.FundingSourceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_payment_funding_sources_source");

            entity.HasOne(d => d.PersonalClaimPayment).WithMany(p => p.PersonalClaimPaymentFundingSources)
                .HasPrincipalKey(p => new { p.CompanyId, p.PersonalClaimPaymentId })
                .HasForeignKey(d => new { d.CompanyId, d.PersonalClaimPaymentId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_payment_funding_sources_payment");
        });

        modelBuilder.Entity<PersonalClaimWriteOff>(entity =>
        {
            entity.HasKey(e => e.PersonalClaimWriteOffId).HasName("personal_claim_write_offs_pkey");

            entity.ToTable("personal_claim_write_offs", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimId }, "ix_personal_claim_write_offs_claim");

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimId, e.WriteOffDate }, "ix_personal_claim_write_offs_claim_date");

            entity.HasIndex(e => new { e.CompanyId, e.CreatedByUserId }, "ix_personal_claim_write_offs_created_by");

            entity.HasIndex(e => new { e.CompanyId, e.SubmittedAt }, "ix_personal_claim_write_offs_pending").HasFilter("((status)::text = 'PendingApproval'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_personal_claim_write_offs_status");

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimWriteOffId }, "uq_personal_claim_write_offs_company").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.WriteOffNumber }, "uq_personal_claim_write_offs_number").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.PersonalClaimId }, "ux_personal_claim_write_offs_one_pending")
                .IsUnique()
                .HasFilter("((status)::text = 'PendingApproval'::text)");

            entity.Property(e => e.PersonalClaimWriteOffId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("personal_claim_write_off_id");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedByUserId).HasColumnName("approved_by_user_id");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.ClaimVersionNumber).HasColumnName("claim_version_number");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.PersonalClaimId).HasColumnName("personal_claim_id");
            entity.Property(e => e.ProofFileUrl)
                .HasMaxLength(1000)
                .HasColumnName("proof_file_url");
            entity.Property(e => e.ReasonType)
                .HasMaxLength(40)
                .HasColumnName("reason_type");
            entity.Property(e => e.RejectedAt).HasColumnName("rejected_at");
            entity.Property(e => e.RejectedByUserId).HasColumnName("rejected_by_user_id");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SubmittedAt).HasColumnName("submitted_at");
            entity.Property(e => e.SubmittedByUserId).HasColumnName("submitted_by_user_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");
            entity.Property(e => e.WriteOffAmount)
                .HasPrecision(18, 2)
                .HasColumnName("write_off_amount");
            entity.Property(e => e.WriteOffDate).HasColumnName("write_off_date");
            entity.Property(e => e.WriteOffNumber)
                .HasMaxLength(50)
                .HasColumnName("write_off_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.PersonalClaimWriteOffAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ApprovedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_write_offs_approved_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.PersonalClaimWriteOffAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_write_offs_cancelled_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.PersonalClaimWriteOffAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_write_offs_created_by");

            entity.HasOne(d => d.PersonalClaim).WithOne(p => p.PersonalClaimWriteOff)
                .HasPrincipalKey<PersonalClaim>(p => new { p.CompanyId, p.PersonalClaimId })
                .HasForeignKey<PersonalClaimWriteOff>(d => new { d.CompanyId, d.PersonalClaimId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_write_offs_claim");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.PersonalClaimWriteOffAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RejectedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_write_offs_rejected_by");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.PersonalClaimWriteOffAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_write_offs_reversed_by");

            entity.HasOne(d => d.AppUser4).WithMany(p => p.PersonalClaimWriteOffAppUser4s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.SubmittedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_personal_claim_write_offs_submitted_by");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("projects_pkey");

            entity.ToTable("projects", "ahdah");

            entity.HasIndex(e => e.CompanyId, "ix_projects_company");

            entity.HasIndex(e => new { e.CompanyId, e.Status }, "ix_projects_company_status");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectOwnerId }, "ix_projects_owner");

            entity.HasIndex(e => new { e.CompanyId, e.StartDate }, "ix_projects_start_date");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectId }, "uq_projects_company_project").IsUnique();

            entity.Property(e => e.ProjectId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("project_id");
            entity.Property(e => e.ActualEndDate).HasColumnName("actual_end_date");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.ContactPhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("contact_phone_number");
            entity.Property(e => e.ContractDate).HasColumnName("contract_date");
            entity.Property(e => e.ContractValue)
                .HasPrecision(18, 2)
                .HasColumnName("contract_value");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.Description)
                .HasMaxLength(1500)
                .HasColumnName("description");
            entity.Property(e => e.ExpectedEndDate).HasColumnName("expected_end_date");
            entity.Property(e => e.Latitude)
                .HasPrecision(9, 6)
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasPrecision(9, 6)
                .HasColumnName("longitude");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(200)
                .HasColumnName("project_name");
            entity.Property(e => e.ProjectOwnerId).HasColumnName("project_owner_id");
            entity.Property(e => e.SiteAddress)
                .HasMaxLength(500)
                .HasColumnName("site_address");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Active'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.Company).WithMany(p => p.Projects)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_projects_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ProjectAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_projects_cancelled_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.ProjectAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_projects_created_by");

            entity.HasOne(d => d.ProjectOwner).WithMany(p => p.Projects)
                .HasPrincipalKey(p => new { p.CompanyId, p.ProjectOwnerId })
                .HasForeignKey(d => new { d.CompanyId, d.ProjectOwnerId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_projects_owner");
        });

        modelBuilder.Entity<ProjectContractChange>(entity =>
        {
            entity.HasKey(e => e.ProjectContractChangeId).HasName("project_contract_changes_pkey");

            entity.ToTable("project_contract_changes", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectId }, "ix_project_contract_changes_project");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectId, e.EffectiveDate }, "ix_project_contract_changes_project_date");

            entity.HasIndex(e => new { e.CompanyId, e.RequestedByUserId }, "ix_project_contract_changes_requested_by");

            entity.HasIndex(e => new { e.CompanyId, e.Status }, "ix_project_contract_changes_status");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectContractChangeId }, "uq_project_contract_changes_company_change").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ProjectId }, "ux_project_contract_changes_one_pending")
                .IsUnique()
                .HasFilter("((status)::text = 'PendingApproval'::text)");

            entity.Property(e => e.ProjectContractChangeId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("project_contract_change_id");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.ChangeAmount)
                .HasPrecision(18, 2)
                .HasColumnName("change_amount");
            entity.Property(e => e.ChangeType)
                .HasMaxLength(30)
                .HasColumnName("change_type");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.EffectiveDate).HasColumnName("effective_date");
            entity.Property(e => e.NewContractValue)
                .HasPrecision(18, 2)
                .HasColumnName("new_contract_value");
            entity.Property(e => e.PreviousContractValue)
                .HasPrecision(18, 2)
                .HasColumnName("previous_contract_value");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Reason)
                .HasMaxLength(1000)
                .HasColumnName("reason");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.RequestedByUserId).HasColumnName("requested_by_user_id");
            entity.Property(e => e.ReviewedAt).HasColumnName("reviewed_at");
            entity.Property(e => e.ReviewedByUserId).HasColumnName("reviewed_by_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'PendingApproval'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SupportingDocumentUrl)
                .HasMaxLength(1000)
                .HasColumnName("supporting_document_url");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ProjectContractChangeAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_project_contract_changes_cancelled_by");

            entity.HasOne(d => d.Project).WithOne(p => p.ProjectContractChange)
                .HasPrincipalKey<Project>(p => new { p.CompanyId, p.ProjectId })
                .HasForeignKey<ProjectContractChange>(d => new { d.CompanyId, d.ProjectId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_project_contract_changes_project");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.ProjectContractChangeAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RequestedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_project_contract_changes_requested_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.ProjectContractChangeAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReviewedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_project_contract_changes_reviewed_by");
        });

        modelBuilder.Entity<ProjectOwner>(entity =>
        {
            entity.HasKey(e => e.ProjectOwnerId).HasName("project_owners_pkey");

            entity.ToTable("project_owners", "ahdah");

            entity.HasIndex(e => e.CompanyId, "ix_project_owners_company");

            entity.HasIndex(e => new { e.CompanyId, e.IsActive }, "ix_project_owners_company_active");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectOwnerId }, "uq_project_owners_company_owner").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.PhoneNumber }, "ux_project_owners_company_phone").IsUnique();

            entity.Property(e => e.ProjectOwnerId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("project_owner_id");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(254)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.OwnerName)
                .HasMaxLength(200)
                .HasColumnName("owner_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.Company).WithMany(p => p.ProjectOwners)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_project_owners_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ProjectOwners)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_project_owners_created_by");
        });

        modelBuilder.Entity<ProjectOwnerPayment>(entity =>
        {
            entity.HasKey(e => e.ProjectOwnerPaymentId).HasName("project_owner_payments_pkey");

            entity.ToTable("project_owner_payments", "ahdah");

            entity.HasIndex(e => e.CompanyId, "ix_project_owner_payments_company");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectOwnerId }, "ix_project_owner_payments_owner");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectOwnerId, e.PaymentDate }, "ix_project_owner_payments_owner_date");

            entity.HasIndex(e => new { e.CompanyId, e.PaymentDate }, "ix_project_owner_payments_payment_date");

            entity.HasIndex(e => new { e.CompanyId, e.Status }, "ix_project_owner_payments_status");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectOwnerPaymentId }, "uq_project_owner_payments_company_payment").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourceId }, "uq_project_owner_payments_funding_source").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.PaymentNumber }, "uq_project_owner_payments_number").IsUnique();

            entity.Property(e => e.ProjectOwnerPaymentId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("project_owner_payment_id");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.FundingSourceId).HasColumnName("funding_source_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.PaymentAmount)
                .HasPrecision(18, 2)
                .HasColumnName("payment_amount");
            entity.Property(e => e.PaymentDate).HasColumnName("payment_date");
            entity.Property(e => e.PaymentNumber)
                .HasMaxLength(50)
                .HasColumnName("payment_number");
            entity.Property(e => e.ProjectOwnerId).HasColumnName("project_owner_id");
            entity.Property(e => e.RecordedByUserId).HasColumnName("recorded_by_user_id");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'PendingVerification'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VerifiedAt).HasColumnName("verified_at");
            entity.Property(e => e.VerifiedByUserId).HasColumnName("verified_by_user_id");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ProjectOwnerPaymentAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_project_owner_payments_cancelled_by");

            entity.HasOne(d => d.FundingSource).WithOne(p => p.ProjectOwnerPayment)
                .HasPrincipalKey<FundingSource>(p => new { p.CompanyId, p.FundingSourceId })
                .HasForeignKey<ProjectOwnerPayment>(d => new { d.CompanyId, d.FundingSourceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_project_owner_payments_funding_source");

            entity.HasOne(d => d.ProjectOwner).WithMany(p => p.ProjectOwnerPayments)
                .HasPrincipalKey(p => new { p.CompanyId, p.ProjectOwnerId })
                .HasForeignKey(d => new { d.CompanyId, d.ProjectOwnerId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_project_owner_payments_owner");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.ProjectOwnerPaymentAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecordedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_project_owner_payments_recorded_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.ProjectOwnerPaymentAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_project_owner_payments_reversed_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.ProjectOwnerPaymentAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.VerifiedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_project_owner_payments_verified_by");
        });

        modelBuilder.Entity<ProjectSupervisor>(entity =>
        {
            entity.HasKey(e => e.ProjectSupervisorId).HasName("project_supervisors_pkey");

            entity.ToTable("project_supervisors", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectId }, "ix_project_supervisors_project");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectId, e.IsActive }, "ix_project_supervisors_project_active");

            entity.HasIndex(e => new { e.CompanyId, e.SupervisorUserId }, "ix_project_supervisors_supervisor");

            entity.HasIndex(e => new { e.CompanyId, e.SupervisorUserId, e.IsActive }, "ix_project_supervisors_supervisor_active");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectSupervisorId }, "uq_project_supervisors_company_assignment").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ProjectId, e.SupervisorUserId }, "ux_project_supervisors_active_assignment")
                .IsUnique()
                .HasFilter("(is_active = true)");

            entity.Property(e => e.ProjectSupervisorId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("project_supervisor_id");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("assigned_at");
            entity.Property(e => e.AssignedByUserId).HasColumnName("assigned_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.RemovalReason)
                .HasMaxLength(500)
                .HasColumnName("removal_reason");
            entity.Property(e => e.RemovedAt).HasColumnName("removed_at");
            entity.Property(e => e.RemovedByUserId).HasColumnName("removed_by_user_id");
            entity.Property(e => e.SupervisorUserId).HasColumnName("supervisor_user_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ProjectSupervisorAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.AssignedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_project_supervisors_assigned_by");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectSupervisors)
                .HasPrincipalKey(p => new { p.CompanyId, p.ProjectId })
                .HasForeignKey(d => new { d.CompanyId, d.ProjectId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_project_supervisors_project");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.ProjectSupervisorAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RemovedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_project_supervisors_removed_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.ProjectSupervisorAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.SupervisorUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_project_supervisors_supervisor");
        });

        modelBuilder.Entity<ReportDefinition>(entity =>
        {
            entity.HasKey(e => e.ReportDefinitionId).HasName("report_definitions_pkey");

            entity.ToTable("report_definitions", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ReportName }, "ix_report_definitions_active").HasFilter("((status)::text = 'Active'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.ReportCategory, e.Status }, "ix_report_definitions_category");

            entity.HasIndex(e => e.ColumnDefinitions, "ix_report_definitions_columns").HasMethod("gin");

            entity.HasIndex(e => new { e.CompanyId, e.DataSourceCode }, "ix_report_definitions_data_source");

            entity.HasIndex(e => new { e.CompanyId, e.ReportCode, e.Status }, "ix_report_definitions_lookup");

            entity.HasIndex(e => new { e.CompanyId, e.DefinitionOrigin }, "ix_report_definitions_origin");

            entity.HasIndex(e => e.ParameterSchema, "ix_report_definitions_parameters")
                .HasMethod("gin")
                .HasOperators(new[] { "jsonb_path_ops" });

            entity.HasIndex(e => new { e.CompanyId, e.ReportScope, e.Status }, "ix_report_definitions_scope");

            entity.HasIndex(e => new { e.CompanyId, e.UpdatedByUserId, e.UpdatedAt }, "ix_report_definitions_updated_by").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ReportCode }, "uq_report_definitions_code").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReportDefinitionId }, "uq_report_definitions_company_report").IsUnique();

            entity.Property(e => e.ReportDefinitionId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("report_definition_id");
            entity.Property(e => e.ActivatedAt).HasColumnName("activated_at");
            entity.Property(e => e.ActivatedByUserId).HasColumnName("activated_by_user_id");
            entity.Property(e => e.AggregationDefinitions)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("aggregation_definitions");
            entity.Property(e => e.AllowScheduling)
                .HasDefaultValue(true)
                .HasColumnName("allow_scheduling");
            entity.Property(e => e.AllowUserColumnSelection)
                .HasDefaultValue(true)
                .HasColumnName("allow_user_column_selection");
            entity.Property(e => e.AllowUserFilters)
                .HasDefaultValue(true)
                .HasColumnName("allow_user_filters");
            entity.Property(e => e.AllowedExportFormats)
                .HasDefaultValueSql("'[\"PDF\", \"XLSX\", \"CSV\"]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("allowed_export_formats");
            entity.Property(e => e.AllowedRoles)
                .HasDefaultValueSql("'[\"Manager\", \"Deputy\", \"Accountant\"]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("allowed_roles");
            entity.Property(e => e.CacheTtlSeconds).HasColumnName("cache_ttl_seconds");
            entity.Property(e => e.ColumnDefinitions)
                .HasColumnType("jsonb")
                .HasColumnName("column_definitions");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.ContainsSensitiveData).HasColumnName("contains_sensitive_data");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CurrentVersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("current_version_number");
            entity.Property(e => e.DataSourceCode)
                .HasMaxLength(150)
                .HasColumnName("data_source_code");
            entity.Property(e => e.DefaultCurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'LYD'::character varying")
                .HasColumnName("default_currency_code");
            entity.Property(e => e.DefaultExportFormat)
                .HasMaxLength(20)
                .HasDefaultValueSql("'PDF'::character varying")
                .HasColumnName("default_export_format");
            entity.Property(e => e.DefaultLocaleCode)
                .HasMaxLength(10)
                .HasDefaultValueSql("'ar-LY'::character varying")
                .HasColumnName("default_locale_code");
            entity.Property(e => e.DefaultParameters)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("default_parameters");
            entity.Property(e => e.DefinitionOrigin)
                .HasMaxLength(30)
                .HasDefaultValueSql("'CompanyCustom'::character varying")
                .HasColumnName("definition_origin");
            entity.Property(e => e.Description)
                .HasMaxLength(1500)
                .HasColumnName("description");
            entity.Property(e => e.DisabledAt).HasColumnName("disabled_at");
            entity.Property(e => e.DisabledByUserId).HasColumnName("disabled_by_user_id");
            entity.Property(e => e.DisabledReason)
                .HasMaxLength(500)
                .HasColumnName("disabled_reason");
            entity.Property(e => e.ExecutionTimeoutSeconds)
                .HasDefaultValue(300)
                .HasColumnName("execution_timeout_seconds");
            entity.Property(e => e.FilterDefinitions)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("filter_definitions");
            entity.Property(e => e.GeneratedFileRetentionDays)
                .HasDefaultValue(30)
                .HasColumnName("generated_file_retention_days");
            entity.Property(e => e.GroupingDefinitions)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("grouping_definitions");
            entity.Property(e => e.LayoutSettings)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("layout_settings");
            entity.Property(e => e.MaximumRowCount)
                .HasDefaultValue(100000)
                .HasColumnName("maximum_row_count");
            entity.Property(e => e.ParameterSchema)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("parameter_schema");
            entity.Property(e => e.ReportCategory)
                .HasMaxLength(40)
                .HasColumnName("report_category");
            entity.Property(e => e.ReportCode)
                .HasMaxLength(120)
                .HasColumnName("report_code");
            entity.Property(e => e.ReportName)
                .HasMaxLength(250)
                .HasColumnName("report_name");
            entity.Property(e => e.ReportScope)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Company'::character varying")
                .HasColumnName("report_scope");
            entity.Property(e => e.RequiresExplicitExportPermission)
                .HasDefaultValue(true)
                .HasColumnName("requires_explicit_export_permission");
            entity.Property(e => e.RetiredAt).HasColumnName("retired_at");
            entity.Property(e => e.RetiredByUserId).HasColumnName("retired_by_user_id");
            entity.Property(e => e.RetirementReason)
                .HasMaxLength(500)
                .HasColumnName("retirement_reason");
            entity.Property(e => e.SortingDefinitions)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("sorting_definitions");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TimeZone)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Africa/Tripoli'::character varying")
                .HasColumnName("time_zone");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedByUserId).HasColumnName("updated_by_user_id");

            entity.HasOne(d => d.Company).WithMany(p => p.ReportDefinitions)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_definitions_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ReportDefinitionAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ActivatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_definitions_activated_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.ReportDefinitionAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_definitions_created_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.ReportDefinitionAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.DisabledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_definitions_disabled_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.ReportDefinitionAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RetiredByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_definitions_retired_by");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.ReportDefinitionAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.UpdatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_definitions_updated_by");
        });

        modelBuilder.Entity<ReportDefinitionVersion>(entity =>
        {
            entity.HasKey(e => e.ReportDefinitionVersionId).HasName("report_definition_versions_pkey");

            entity.ToTable("report_definition_versions", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ReportCategorySnapshot, e.CreatedAt }, "ix_report_versions_category").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ChangeType, e.CreatedAt }, "ix_report_versions_change_type").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ReportCodeSnapshot, e.VersionNumber }, "ix_report_versions_code").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_report_versions_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.DataSourceCodeSnapshot }, "ix_report_versions_data_source");

            entity.HasIndex(e => e.ParameterSchemaSnapshot, "ix_report_versions_parameters")
                .HasMethod("gin")
                .HasOperators(new[] { "jsonb_path_ops" });

            entity.HasIndex(e => new { e.CompanyId, e.RecordedByUserId, e.CreatedAt }, "ix_report_versions_recorded_by").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ReportDefinitionId, e.VersionNumber }, "ix_report_versions_report").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ReportStatusSnapshot, e.CreatedAt }, "ix_report_versions_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ReportDefinitionVersionId }, "uq_report_versions_company_version").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReportDefinitionId, e.VersionNumber }, "uq_report_versions_report_number").IsUnique();

            entity.Property(e => e.ReportDefinitionVersionId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("report_definition_version_id");
            entity.Property(e => e.AggregationDefinitionsSnapshot)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("aggregation_definitions_snapshot");
            entity.Property(e => e.AllowSchedulingSnapshot).HasColumnName("allow_scheduling_snapshot");
            entity.Property(e => e.AllowUserColumnSelectionSnapshot).HasColumnName("allow_user_column_selection_snapshot");
            entity.Property(e => e.AllowUserFiltersSnapshot).HasColumnName("allow_user_filters_snapshot");
            entity.Property(e => e.AllowedExportFormatsSnapshot)
                .HasColumnType("jsonb")
                .HasColumnName("allowed_export_formats_snapshot");
            entity.Property(e => e.AllowedRolesSnapshot)
                .HasColumnType("jsonb")
                .HasColumnName("allowed_roles_snapshot");
            entity.Property(e => e.CacheTtlSecondsSnapshot).HasColumnName("cache_ttl_seconds_snapshot");
            entity.Property(e => e.ChangeSummary)
                .HasMaxLength(1000)
                .HasColumnName("change_summary");
            entity.Property(e => e.ChangeType)
                .HasMaxLength(40)
                .HasColumnName("change_type");
            entity.Property(e => e.ColumnDefinitionsSnapshot)
                .HasColumnType("jsonb")
                .HasColumnName("column_definitions_snapshot");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.ContainsSensitiveDataSnapshot).HasColumnName("contains_sensitive_data_snapshot");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DataSourceCodeSnapshot)
                .HasMaxLength(150)
                .HasColumnName("data_source_code_snapshot");
            entity.Property(e => e.DefaultCurrencyCodeSnapshot)
                .HasMaxLength(3)
                .HasColumnName("default_currency_code_snapshot");
            entity.Property(e => e.DefaultExportFormatSnapshot)
                .HasMaxLength(20)
                .HasColumnName("default_export_format_snapshot");
            entity.Property(e => e.DefaultLocaleCodeSnapshot)
                .HasMaxLength(10)
                .HasColumnName("default_locale_code_snapshot");
            entity.Property(e => e.DefaultParametersSnapshot)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("default_parameters_snapshot");
            entity.Property(e => e.DefinitionOriginSnapshot)
                .HasMaxLength(30)
                .HasColumnName("definition_origin_snapshot");
            entity.Property(e => e.DescriptionSnapshot)
                .HasMaxLength(1500)
                .HasColumnName("description_snapshot");
            entity.Property(e => e.ExecutionTimeoutSecondsSnapshot).HasColumnName("execution_timeout_seconds_snapshot");
            entity.Property(e => e.FilterDefinitionsSnapshot)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("filter_definitions_snapshot");
            entity.Property(e => e.GeneratedFileRetentionDaysSnapshot).HasColumnName("generated_file_retention_days_snapshot");
            entity.Property(e => e.GroupingDefinitionsSnapshot)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("grouping_definitions_snapshot");
            entity.Property(e => e.LayoutSettingsSnapshot)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("layout_settings_snapshot");
            entity.Property(e => e.MaximumRowCountSnapshot).HasColumnName("maximum_row_count_snapshot");
            entity.Property(e => e.ParameterSchemaSnapshot)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("parameter_schema_snapshot");
            entity.Property(e => e.PreviousVersionNumber).HasColumnName("previous_version_number");
            entity.Property(e => e.RecordedByUserId).HasColumnName("recorded_by_user_id");
            entity.Property(e => e.ReportCategorySnapshot)
                .HasMaxLength(40)
                .HasColumnName("report_category_snapshot");
            entity.Property(e => e.ReportCodeSnapshot)
                .HasMaxLength(120)
                .HasColumnName("report_code_snapshot");
            entity.Property(e => e.ReportDefinitionId).HasColumnName("report_definition_id");
            entity.Property(e => e.ReportNameSnapshot)
                .HasMaxLength(250)
                .HasColumnName("report_name_snapshot");
            entity.Property(e => e.ReportScopeSnapshot)
                .HasMaxLength(30)
                .HasColumnName("report_scope_snapshot");
            entity.Property(e => e.ReportStatusSnapshot)
                .HasMaxLength(20)
                .HasColumnName("report_status_snapshot");
            entity.Property(e => e.RequiresExportPermissionSnapshot).HasColumnName("requires_export_permission_snapshot");
            entity.Property(e => e.RestoredFromVersionNumber).HasColumnName("restored_from_version_number");
            entity.Property(e => e.SortingDefinitionsSnapshot)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("sorting_definitions_snapshot");
            entity.Property(e => e.SourceReportUpdatedAt).HasColumnName("source_report_updated_at");
            entity.Property(e => e.TimeZoneSnapshot)
                .HasMaxLength(100)
                .HasColumnName("time_zone_snapshot");
            entity.Property(e => e.VersionNumber).HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ReportDefinitionVersions)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecordedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_versions_recorded_by");

            entity.HasOne(d => d.ReportDefinition).WithMany(p => p.ReportDefinitionVersions)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDefinitionId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDefinitionId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_versions_report");

            entity.HasOne(d => d.ReportDefinitionVersionNavigation).WithMany(p => p.InverseReportDefinitionVersionNavigation)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDefinitionId, p.VersionNumber })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDefinitionId, d.PreviousVersionNumber })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_versions_previous");

            entity.HasOne(d => d.ReportDefinitionVersion1).WithMany(p => p.InverseReportDefinitionVersion1)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDefinitionId, p.VersionNumber })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDefinitionId, d.RestoredFromVersionNumber })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_versions_restored");
        });

        modelBuilder.Entity<ReportDelivery>(entity =>
        {
            entity.HasKey(e => e.ReportDeliveryId).HasName("report_deliveries_pkey");

            entity.ToTable("report_deliveries", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.DeliveryChannel, e.Status, e.CreatedAt }, "ix_report_deliveries_channel").IsDescending(false, false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_report_deliveries_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.FailureCategory, e.CompletedAt }, "ix_report_deliveries_failures")
                .IsDescending(false, false, true)
                .HasFilter("((status)::text = 'Failed'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.LastAttemptStartedAt }, "ix_report_deliveries_processing").HasFilter("((status)::text = 'Processing'::text)");

            entity.HasIndex(e => new { e.Priority, e.AvailableAt, e.DeliverySequence }, "ix_report_deliveries_ready")
                .IsDescending(true, false, false)
                .HasFilter("((status)::text = ANY ((ARRAY['Pending'::character varying, 'Queued'::character varying])::text[]))");

            entity.HasIndex(e => new { e.CompanyId, e.RecipientUserId, e.CreatedAt }, "ix_report_deliveries_recipient")
                .IsDescending(false, false, true)
                .HasFilter("(recipient_user_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.ReportRunId, e.Status, e.DeliverySequence }, "ix_report_deliveries_report_run");

            entity.HasIndex(e => new { e.NextRetryAt, e.Priority, e.DeliverySequence }, "ix_report_deliveries_retry")
                .IsDescending(false, true, false)
                .HasFilter("((status)::text = 'RetryScheduled'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.ReportScheduleId, e.CreatedAt }, "ix_report_deliveries_schedule").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.SkipCategory, e.CompletedAt }, "ix_report_deliveries_skipped")
                .IsDescending(false, false, true)
                .HasFilter("((status)::text = 'Skipped'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.ReportDeliveryId }, "uq_report_deliveries_company_delivery").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.IdempotencyKey }, "uq_report_deliveries_idempotency").IsUnique();

            entity.HasIndex(e => e.DeliverySequence, "uq_report_deliveries_sequence").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.BackgroundJobId }, "ux_report_deliveries_background_job")
                .IsUnique()
                .HasFilter("(background_job_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.ReportDeliveryId, e.BackgroundJobId }, "ux_report_deliveries_delivery_job_ref").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReportRunId, e.ReportDeliveryId }, "ux_report_deliveries_run_delivery_ref").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReportRunId, e.RecipientUserId, e.DeliveryChannel, e.DeliveryEvent }, "ux_report_deliveries_user_channel")
                .IsUnique()
                .HasFilter("((recipient_type)::text = 'User'::text)");

            entity.Property(e => e.ReportDeliveryId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("report_delivery_id");
            entity.Property(e => e.AttachmentCount).HasColumnName("attachment_count");
            entity.Property(e => e.AttemptCount).HasColumnName("attempt_count");
            entity.Property(e => e.AvailableAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("available_at");
            entity.Property(e => e.BackgroundJobId)
                .IsRequired()
                .HasColumnName("background_job_id");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByType)
                .HasMaxLength(30)
                .HasColumnName("cancelled_by_type");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.ContainsSensitiveData).HasColumnName("contains_sensitive_data");
            entity.Property(e => e.ContentMode)
                .HasMaxLength(30)
                .HasColumnName("content_mode");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeliveredAt).HasColumnName("delivered_at");
            entity.Property(e => e.DeliveryChannel)
                .HasMaxLength(20)
                .HasColumnName("delivery_channel");
            entity.Property(e => e.DeliveryEvent)
                .HasMaxLength(30)
                .HasColumnName("delivery_event");
            entity.Property(e => e.DeliverySequence)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn()
                .HasColumnName("delivery_sequence");
            entity.Property(e => e.DownloadLinkCount).HasColumnName("download_link_count");
            entity.Property(e => e.ExternalEmail)
                .HasMaxLength(320)
                .HasColumnName("external_email");
            entity.Property(e => e.FailureCategory)
                .HasMaxLength(30)
                .HasColumnName("failure_category");
            entity.Property(e => e.FailureCode)
                .HasMaxLength(100)
                .HasColumnName("failure_code");
            entity.Property(e => e.FailureDetails)
                .HasColumnType("jsonb")
                .HasColumnName("failure_details");
            entity.Property(e => e.FailureMessage)
                .HasMaxLength(2000)
                .HasColumnName("failure_message");
            entity.Property(e => e.IdempotencyKey)
                .HasMaxLength(200)
                .HasColumnName("idempotency_key");
            entity.Property(e => e.LastAttemptCompletedAt).HasColumnName("last_attempt_completed_at");
            entity.Property(e => e.LastAttemptStartedAt).HasColumnName("last_attempt_started_at");
            entity.Property(e => e.LocaleCode)
                .HasMaxLength(10)
                .HasDefaultValueSql("'ar-LY'::character varying")
                .HasColumnName("locale_code");
            entity.Property(e => e.MaxAttempts)
                .HasDefaultValue(5)
                .HasColumnName("max_attempts");
            entity.Property(e => e.MessageSnapshot)
                .HasMaxLength(4000)
                .HasColumnName("message_snapshot");
            entity.Property(e => e.NextRetryAt).HasColumnName("next_retry_at");
            entity.Property(e => e.Priority)
                .HasDefaultValue((short)5)
                .HasColumnName("priority");
            entity.Property(e => e.ProviderMessageId)
                .HasMaxLength(250)
                .HasColumnName("provider_message_id");
            entity.Property(e => e.ProviderName)
                .HasMaxLength(100)
                .HasColumnName("provider_name");
            entity.Property(e => e.ProviderResponseMetadata)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("provider_response_metadata");
            entity.Property(e => e.QueuedAt).HasColumnName("queued_at");
            entity.Property(e => e.RecipientDisplayName)
                .HasMaxLength(200)
                .HasColumnName("recipient_display_name");
            entity.Property(e => e.RecipientSourceType)
                .HasMaxLength(30)
                .HasColumnName("recipient_source_type");
            entity.Property(e => e.RecipientType)
                .HasMaxLength(30)
                .HasColumnName("recipient_type");
            entity.Property(e => e.RecipientUserId).HasColumnName("recipient_user_id");
            entity.Property(e => e.ReportRunId).HasColumnName("report_run_id");
            entity.Property(e => e.ReportScheduleId).HasColumnName("report_schedule_id");
            entity.Property(e => e.ReportScheduleRecipientId).HasColumnName("report_schedule_recipient_id");
            entity.Property(e => e.RequiresAuthorization)
                .HasDefaultValue(true)
                .HasColumnName("requires_authorization");
            entity.Property(e => e.SkipCategory)
                .HasMaxLength(30)
                .HasColumnName("skip_category");
            entity.Property(e => e.SkipReason)
                .HasMaxLength(1000)
                .HasColumnName("skip_reason");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SubjectSnapshot)
                .HasMaxLength(500)
                .HasColumnName("subject_snapshot");
            entity.Property(e => e.TemplateData)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("template_data");
            entity.Property(e => e.TimeZone)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Africa/Tripoli'::character varying")
                .HasColumnName("time_zone");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.BackgroundJob).WithOne(p => p.ReportDelivery)
                .HasPrincipalKey<BackgroundJob>(p => new { p.CompanyId, p.BackgroundJobId })
                .HasForeignKey<ReportDelivery>(d => new { d.CompanyId, d.BackgroundJobId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_deliveries_background_job");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ReportDeliveryAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_deliveries_cancelled_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.ReportDeliveryAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecipientUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_deliveries_recipient_user");

            entity.HasOne(d => d.ReportRun).WithMany(p => p.ReportDeliveries)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportRunId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportRunId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_deliveries_run");

            entity.HasOne(d => d.ReportSchedule).WithMany(p => p.ReportDeliveries)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportScheduleId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportScheduleId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_deliveries_schedule");

            entity.HasOne(d => d.ReportScheduleRecipient).WithMany(p => p.ReportDeliveries)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportScheduleRecipientId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportScheduleRecipientId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_deliveries_schedule_recipient");
        });

        modelBuilder.Entity<ReportDeliveryAttempt>(entity =>
        {
            entity.HasKey(e => e.ReportDeliveryAttemptId).HasName("report_delivery_attempts_pkey");

            entity.ToTable("report_delivery_attempts", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.BackgroundJobId, e.AttemptNumber }, "ix_report_delivery_attempts_background_job").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_report_delivery_attempts_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.ReportDeliveryId, e.AttemptNumber }, "ix_report_delivery_attempts_delivery").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.DurationMilliseconds }, "ix_report_delivery_attempts_duration").IsDescending(false, true);

            entity.HasIndex(e => new { e.CompanyId, e.FailureCategory, e.CompletedAt }, "ix_report_delivery_attempts_failures")
                .IsDescending(false, false, true)
                .HasFilter("((attempt_status)::text = ANY ((ARRAY['Failed'::character varying, 'Abandoned'::character varying])::text[]))");

            entity.HasIndex(e => new { e.CompanyId, e.ProviderName, e.CompletedAt }, "ix_report_delivery_attempts_provider").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ProviderMessageId }, "ix_report_delivery_attempts_provider_message").HasFilter("(provider_message_id IS NOT NULL)");

            entity.HasIndex(e => e.NextRetryAt, "ix_report_delivery_attempts_retry").HasFilter("(((retry_decision)::text = 'Retry'::text) AND (next_retry_at IS NOT NULL))");

            entity.HasIndex(e => new { e.CompanyId, e.AttemptStatus, e.CompletedAt }, "ix_report_delivery_attempts_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.RecipientUserIdSnapshot, e.CompletedAt }, "ix_report_delivery_attempts_user")
                .IsDescending(false, false, true)
                .HasFilter("(recipient_user_id_snapshot IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.BackgroundJobAttemptId }, "uq_report_delivery_attempts_bg_attempt").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReportDeliveryAttemptId }, "uq_report_delivery_attempts_company").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReportDeliveryId, e.AttemptNumber }, "uq_report_delivery_attempts_number").IsUnique();

            entity.HasIndex(e => e.AttemptSequence, "uq_report_delivery_attempts_sequence").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReportDeliveryId, e.ReportDeliveryAttemptId }, "ux_report_delivery_attempts_delivery_event_ref").IsUnique();

            entity.Property(e => e.ReportDeliveryAttemptId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("report_delivery_attempt_id");
            entity.Property(e => e.AttachmentBytesSnapshot).HasColumnName("attachment_bytes_snapshot");
            entity.Property(e => e.AttachmentCountSnapshot).HasColumnName("attachment_count_snapshot");
            entity.Property(e => e.AttemptNumber).HasColumnName("attempt_number");
            entity.Property(e => e.AttemptSequence)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn()
                .HasColumnName("attempt_sequence");
            entity.Property(e => e.AttemptStatus)
                .HasMaxLength(20)
                .HasColumnName("attempt_status");
            entity.Property(e => e.BackgroundJobAttemptId).HasColumnName("background_job_attempt_id");
            entity.Property(e => e.BackgroundJobId).HasColumnName("background_job_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeliveryChannelSnapshot)
                .HasMaxLength(20)
                .HasColumnName("delivery_channel_snapshot");
            entity.Property(e => e.DestinationIdentifierSnapshot)
                .HasMaxLength(320)
                .HasColumnName("destination_identifier_snapshot");
            entity.Property(e => e.DownloadLinkCountSnapshot).HasColumnName("download_link_count_snapshot");
            entity.Property(e => e.DurationMilliseconds).HasColumnName("duration_milliseconds");
            entity.Property(e => e.FailureCategory)
                .HasMaxLength(30)
                .HasColumnName("failure_category");
            entity.Property(e => e.FailureCode)
                .HasMaxLength(100)
                .HasColumnName("failure_code");
            entity.Property(e => e.FailureDetails)
                .HasColumnType("jsonb")
                .HasColumnName("failure_details");
            entity.Property(e => e.FailureMessage)
                .HasMaxLength(2000)
                .HasColumnName("failure_message");
            entity.Property(e => e.HttpStatusCode).HasColumnName("http_status_code");
            entity.Property(e => e.NextRetryAt).HasColumnName("next_retry_at");
            entity.Property(e => e.ProviderCompletedAt).HasColumnName("provider_completed_at");
            entity.Property(e => e.ProviderDurationMilliseconds).HasColumnName("provider_duration_milliseconds");
            entity.Property(e => e.ProviderEndpoint)
                .HasMaxLength(1000)
                .HasColumnName("provider_endpoint");
            entity.Property(e => e.ProviderMessageId)
                .HasMaxLength(250)
                .HasColumnName("provider_message_id");
            entity.Property(e => e.ProviderName)
                .HasMaxLength(100)
                .HasColumnName("provider_name");
            entity.Property(e => e.ProviderRequestId)
                .HasMaxLength(250)
                .HasColumnName("provider_request_id");
            entity.Property(e => e.ProviderResponseReceived).HasColumnName("provider_response_received");
            entity.Property(e => e.ProviderStartedAt).HasColumnName("provider_started_at");
            entity.Property(e => e.ProviderStatusCode)
                .HasMaxLength(100)
                .HasColumnName("provider_status_code");
            entity.Property(e => e.ProviderStatusMessage)
                .HasMaxLength(1000)
                .HasColumnName("provider_status_message");
            entity.Property(e => e.RecipientDisplayNameSnapshot)
                .HasMaxLength(200)
                .HasColumnName("recipient_display_name_snapshot");
            entity.Property(e => e.RecipientEmailSnapshot)
                .HasMaxLength(320)
                .HasColumnName("recipient_email_snapshot");
            entity.Property(e => e.RecipientTypeSnapshot)
                .HasMaxLength(30)
                .HasColumnName("recipient_type_snapshot");
            entity.Property(e => e.RecipientUserIdSnapshot).HasColumnName("recipient_user_id_snapshot");
            entity.Property(e => e.ReportDeliveryId).HasColumnName("report_delivery_id");
            entity.Property(e => e.RequestMetadata)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("request_metadata");
            entity.Property(e => e.RequestedPayloadHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("requested_payload_hash");
            entity.Property(e => e.ResponseMetadata)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("response_metadata");
            entity.Property(e => e.ResponsePayloadHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("response_payload_hash");
            entity.Property(e => e.RetryDecision)
                .HasMaxLength(20)
                .HasColumnName("retry_decision");
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
            entity.Property(e => e.WasProviderContacted).HasColumnName("was_provider_contacted");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ReportDeliveryAttempts)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecipientUserIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_delivery_attempts_user");

            entity.HasOne(d => d.BackgroundJobAttempt).WithMany(p => p.ReportDeliveryAttempts)
                .HasPrincipalKey(p => new { p.CompanyId, p.BackgroundJobId, p.BackgroundJobAttemptId })
                .HasForeignKey(d => new { d.CompanyId, d.BackgroundJobId, d.BackgroundJobAttemptId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_delivery_attempts_bg_attempt");

            entity.HasOne(d => d.ReportDelivery).WithMany(p => p.ReportDeliveryAttempts)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDeliveryId, p.BackgroundJobId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDeliveryId, d.BackgroundJobId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_delivery_attempts_delivery");
        });

        modelBuilder.Entity<ReportDeliveryEvent>(entity =>
        {
            entity.HasKey(e => e.ReportDeliveryEventId).HasName("report_delivery_events_pkey");

            entity.ToTable("report_delivery_events", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ReportDeliveryAttemptId, e.OccurredAt }, "ix_report_delivery_events_attempt").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.EventCategory, e.OccurredAt }, "ix_report_delivery_events_category").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ClickedUrlHash, e.OccurredAt }, "ix_report_delivery_events_clicked")
                .IsDescending(false, false, true)
                .HasFilter("(clicked_url_hash IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.EventType, e.OccurredAt }, "ix_report_delivery_events_compliance")
                .IsDescending(false, false, true)
                .HasFilter("((event_category)::text = 'Compliance'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_report_delivery_events_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.ReportDeliveryId, e.OccurredAt }, "ix_report_delivery_events_delivery").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.EventType, e.OccurredAt }, "ix_report_delivery_events_failures")
                .IsDescending(false, false, true)
                .HasFilter("((event_category)::text = 'Failure'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.ProviderEventId }, "ix_report_delivery_events_provider_event_lookup").HasFilter("(provider_event_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.ProviderName, e.ProviderMessageId, e.OccurredAt }, "ix_report_delivery_events_provider_message")
                .IsDescending(false, false, false, true)
                .HasFilter("(provider_message_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.ProcessingResult, e.ReceivedAt }, "ix_report_delivery_events_rejected")
                .IsDescending(false, false, true)
                .HasFilter("((processing_result)::text = ANY ((ARRAY['Rejected'::character varying, 'Ignored'::character varying, 'Duplicate'::character varying])::text[]))");

            entity.HasIndex(e => new { e.CompanyId, e.EventType, e.OccurredAt }, "ix_report_delivery_events_type").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.RecipientUserIdSnapshot, e.OccurredAt }, "ix_report_delivery_events_user")
                .IsDescending(false, false, true)
                .HasFilter("(recipient_user_id_snapshot IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.ReportDeliveryEventId }, "uq_report_delivery_events_company_event").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.IdempotencyKey }, "uq_report_delivery_events_idempotency").IsUnique();

            entity.HasIndex(e => e.EventSequence, "uq_report_delivery_events_sequence").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ProviderName, e.ProviderEventId }, "ux_report_delivery_events_provider_event")
                .IsUnique()
                .HasFilter("((provider_event_id IS NOT NULL) AND ((processing_result)::text <> 'Duplicate'::text))");

            entity.Property(e => e.ReportDeliveryEventId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("report_delivery_event_id");
            entity.Property(e => e.BounceType)
                .HasMaxLength(20)
                .HasColumnName("bounce_type");
            entity.Property(e => e.ClickedUrlHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("clicked_url_hash");
            entity.Property(e => e.ClientIp).HasColumnName("client_ip");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeliveryChannelSnapshot)
                .HasMaxLength(20)
                .HasColumnName("delivery_channel_snapshot");
            entity.Property(e => e.DuplicateOfEventId).HasColumnName("duplicate_of_event_id");
            entity.Property(e => e.EventCategory)
                .HasMaxLength(20)
                .HasColumnName("event_category");
            entity.Property(e => e.EventMetadata)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("event_metadata");
            entity.Property(e => e.EventReasonCode)
                .HasMaxLength(100)
                .HasColumnName("event_reason_code");
            entity.Property(e => e.EventReasonMessage)
                .HasMaxLength(2000)
                .HasColumnName("event_reason_message");
            entity.Property(e => e.EventSequence)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn()
                .HasColumnName("event_sequence");
            entity.Property(e => e.EventSource)
                .HasMaxLength(30)
                .HasColumnName("event_source");
            entity.Property(e => e.EventType)
                .HasMaxLength(30)
                .HasColumnName("event_type");
            entity.Property(e => e.IdempotencyKey)
                .HasMaxLength(200)
                .HasColumnName("idempotency_key");
            entity.Property(e => e.OccurredAt).HasColumnName("occurred_at");
            entity.Property(e => e.PayloadHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("payload_hash");
            entity.Property(e => e.ProcessedAt).HasColumnName("processed_at");
            entity.Property(e => e.ProcessingResult)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Applied'::character varying")
                .HasColumnName("processing_result");
            entity.Property(e => e.ProviderEventId)
                .HasMaxLength(250)
                .HasColumnName("provider_event_id");
            entity.Property(e => e.ProviderMessageId)
                .HasMaxLength(250)
                .HasColumnName("provider_message_id");
            entity.Property(e => e.ProviderName)
                .HasMaxLength(100)
                .HasColumnName("provider_name");
            entity.Property(e => e.ProviderStatusCode)
                .HasMaxLength(100)
                .HasColumnName("provider_status_code");
            entity.Property(e => e.ProviderStatusMessage)
                .HasMaxLength(1000)
                .HasColumnName("provider_status_message");
            entity.Property(e => e.ReceivedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("received_at");
            entity.Property(e => e.RecipientDisplayNameSnapshot)
                .HasMaxLength(200)
                .HasColumnName("recipient_display_name_snapshot");
            entity.Property(e => e.RecipientEmailSnapshot)
                .HasMaxLength(320)
                .HasColumnName("recipient_email_snapshot");
            entity.Property(e => e.RecipientTypeSnapshot)
                .HasMaxLength(30)
                .HasColumnName("recipient_type_snapshot");
            entity.Property(e => e.RecipientUserIdSnapshot).HasColumnName("recipient_user_id_snapshot");
            entity.Property(e => e.ReportDeliveryAttemptId).HasColumnName("report_delivery_attempt_id");
            entity.Property(e => e.ReportDeliveryId).HasColumnName("report_delivery_id");
            entity.Property(e => e.SignatureVerificationStatus)
                .HasMaxLength(20)
                .HasDefaultValueSql("'NotApplicable'::character varying")
                .HasColumnName("signature_verification_status");
            entity.Property(e => e.SmtpResponseCode)
                .HasMaxLength(100)
                .HasColumnName("smtp_response_code");
            entity.Property(e => e.UserAgent)
                .HasMaxLength(1000)
                .HasColumnName("user_agent");
            entity.Property(e => e.WebhookRequestId)
                .HasMaxLength(250)
                .HasColumnName("webhook_request_id");

            entity.HasOne(d => d.ReportDeliveryEventNavigation).WithMany(p => p.InverseReportDeliveryEventNavigation)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDeliveryEventId })
                .HasForeignKey(d => new { d.CompanyId, d.DuplicateOfEventId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_delivery_events_duplicate");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ReportDeliveryEvents)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecipientUserIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_delivery_events_recipient");

            entity.HasOne(d => d.ReportDeliveryAttempt).WithMany(p => p.ReportDeliveryEvents)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDeliveryId, p.ReportDeliveryAttemptId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDeliveryId, d.ReportDeliveryAttemptId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_delivery_events_attempt");
        });

        modelBuilder.Entity<ReportDeliveryFile>(entity =>
        {
            entity.HasKey(e => e.ReportDeliveryFileId).HasName("report_delivery_files_pkey");

            entity.ToTable("report_delivery_files", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_report_delivery_files_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.ReportDeliveryId, e.FileSequence }, "ix_report_delivery_files_delivery");

            entity.HasIndex(e => new { e.CompanyId, e.LastSuccessfulDownloadAt }, "ix_report_delivery_files_downloaded")
                .IsDescending(false, true)
                .HasFilter("(successful_download_count > 0)");

            entity.HasIndex(e => new { e.CompanyId, e.FailureCategory, e.FailedAt }, "ix_report_delivery_files_failed")
                .IsDescending(false, false, true)
                .HasFilter("((status)::text = 'Failed'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.Sha256HashSnapshot }, "ix_report_delivery_files_hash");

            entity.HasIndex(e => e.TemporaryUrlExpiresAt, "ix_report_delivery_files_link_expiry").HasFilter("(((delivery_file_usage)::text = 'DownloadLink'::text) AND ((status)::text = ANY ((ARRAY['Prepared'::character varying, 'Delivered'::character varying])::text[])))");

            entity.HasIndex(e => new { e.CompanyId, e.ReportRunId, e.ReportRunFileId }, "ix_report_delivery_files_run");

            entity.HasIndex(e => new { e.CompanyId, e.SkipCategory, e.SkippedAt }, "ix_report_delivery_files_skipped")
                .IsDescending(false, false, true)
                .HasFilter("((status)::text = 'Skipped'::text)");

            entity.HasIndex(e => e.SourceFileExpiresAtSnapshot, "ix_report_delivery_files_source_expiry").HasFilter("((status)::text = ANY ((ARRAY['Pending'::character varying, 'Prepared'::character varying, 'Delivered'::character varying])::text[]))");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_report_delivery_files_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.DeliveryFileUsage, e.Status, e.CreatedAt }, "ix_report_delivery_files_usage").IsDescending(false, false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ReportDeliveryFileId }, "uq_report_delivery_files_company_file").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.IdempotencyKey }, "uq_report_delivery_files_idempotency").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReportDeliveryId, e.FileSequence }, "uq_report_delivery_files_sequence").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReportDeliveryId, e.ReportRunFileId, e.DeliveryFileUsage }, "uq_report_delivery_files_usage").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.TemporaryUrlTokenHash }, "ux_report_delivery_files_token")
                .IsUnique()
                .HasFilter("(temporary_url_token_hash IS NOT NULL)");

            entity.Property(e => e.ReportDeliveryFileId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("report_delivery_file_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.ContentTypeSnapshot)
                .HasMaxLength(150)
                .HasColumnName("content_type_snapshot");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeliveredAt).HasColumnName("delivered_at");
            entity.Property(e => e.DeliveryFileUsage)
                .HasMaxLength(20)
                .HasColumnName("delivery_file_usage");
            entity.Property(e => e.ExpiredAt).HasColumnName("expired_at");
            entity.Property(e => e.ExportFormatSnapshot)
                .HasMaxLength(20)
                .HasColumnName("export_format_snapshot");
            entity.Property(e => e.FailedAt).HasColumnName("failed_at");
            entity.Property(e => e.FailureCategory)
                .HasMaxLength(30)
                .HasColumnName("failure_category");
            entity.Property(e => e.FailureCode)
                .HasMaxLength(100)
                .HasColumnName("failure_code");
            entity.Property(e => e.FailureDetails)
                .HasColumnType("jsonb")
                .HasColumnName("failure_details");
            entity.Property(e => e.FailureMessage)
                .HasMaxLength(2000)
                .HasColumnName("failure_message");
            entity.Property(e => e.FileNameSnapshot)
                .HasMaxLength(500)
                .HasColumnName("file_name_snapshot");
            entity.Property(e => e.FileSequence).HasColumnName("file_sequence");
            entity.Property(e => e.FileSizeBytesSnapshot).HasColumnName("file_size_bytes_snapshot");
            entity.Property(e => e.IdempotencyKey)
                .HasMaxLength(200)
                .HasColumnName("idempotency_key");
            entity.Property(e => e.LastSuccessfulDownloadAt).HasColumnName("last_successful_download_at");
            entity.Property(e => e.MaximumDownloadCount).HasColumnName("maximum_download_count");
            entity.Property(e => e.PreparedAt).HasColumnName("prepared_at");
            entity.Property(e => e.ProviderAttachmentId)
                .HasMaxLength(250)
                .HasColumnName("provider_attachment_id");
            entity.Property(e => e.ProviderMetadata)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("provider_metadata");
            entity.Property(e => e.ReportDeliveryId).HasColumnName("report_delivery_id");
            entity.Property(e => e.ReportRunFileId).HasColumnName("report_run_file_id");
            entity.Property(e => e.ReportRunId).HasColumnName("report_run_id");
            entity.Property(e => e.RequiresAuthorizationSnapshot).HasColumnName("requires_authorization_snapshot");
            entity.Property(e => e.RevocationReason)
                .HasMaxLength(500)
                .HasColumnName("revocation_reason");
            entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");
            entity.Property(e => e.RevokedByType)
                .HasMaxLength(30)
                .HasColumnName("revoked_by_type");
            entity.Property(e => e.RevokedByUserId).HasColumnName("revoked_by_user_id");
            entity.Property(e => e.Sha256HashSnapshot)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("sha256_hash_snapshot");
            entity.Property(e => e.SkipCategory)
                .HasMaxLength(30)
                .HasColumnName("skip_category");
            entity.Property(e => e.SkipReason)
                .HasMaxLength(1000)
                .HasColumnName("skip_reason");
            entity.Property(e => e.SkippedAt).HasColumnName("skipped_at");
            entity.Property(e => e.SourceFileEncryptedSnapshot).HasColumnName("source_file_encrypted_snapshot");
            entity.Property(e => e.SourceFileExpiresAtSnapshot).HasColumnName("source_file_expires_at_snapshot");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SuccessfulDownloadCount).HasColumnName("successful_download_count");
            entity.Property(e => e.TemporaryUrlExpiresAt).HasColumnName("temporary_url_expires_at");
            entity.Property(e => e.TemporaryUrlIssuedAt).HasColumnName("temporary_url_issued_at");
            entity.Property(e => e.TemporaryUrlTokenHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("temporary_url_token_hash");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ReportDeliveryFiles)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RevokedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_delivery_files_revoked_by");

            entity.HasOne(d => d.ReportDelivery).WithMany(p => p.ReportDeliveryFiles)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportRunId, p.ReportDeliveryId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportRunId, d.ReportDeliveryId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_delivery_files_delivery");

            entity.HasOne(d => d.ReportRunFile).WithMany(p => p.ReportDeliveryFiles)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportRunId, p.ReportRunFileId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportRunId, d.ReportRunFileId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_delivery_files_run_file");
        });

        modelBuilder.Entity<ReportFileDownload>(entity =>
        {
            entity.HasKey(e => e.ReportFileDownloadId).HasName("report_file_downloads_pkey");

            entity.ToTable("report_file_downloads", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.CompletedAt }, "ix_report_downloads_completed")
                .IsDescending(false, true)
                .HasFilter("((download_status)::text = 'Completed'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_report_downloads_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.DenialCategory, e.RequestedAt }, "ix_report_downloads_denied")
                .IsDescending(false, false, true)
                .HasFilter("((download_status)::text = 'Denied'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.FailureCategory, e.RequestedAt }, "ix_report_downloads_failed")
                .IsDescending(false, false, true)
                .HasFilter("((download_status)::text = 'Failed'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.ReportRunFileId, e.RequestedAt }, "ix_report_downloads_file").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ClientIp, e.RequestedAt }, "ix_report_downloads_ip")
                .IsDescending(false, false, true)
                .HasFilter("(client_ip IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.TemporaryUrlTokenHash }, "ix_report_downloads_signed_url").HasFilter("(temporary_url_token_hash IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.DownloadStatus, e.RequestedAt }, "ix_report_downloads_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.RequestedByUserId, e.RequestedAt }, "ix_report_downloads_user")
                .IsDescending(false, false, true)
                .HasFilter("(requested_by_user_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.ReportFileDownloadId }, "uq_report_downloads_company_download").IsUnique();

            entity.HasIndex(e => e.DownloadSequence, "uq_report_downloads_sequence").IsUnique();

            entity.Property(e => e.ReportFileDownloadId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("report_file_download_id");
            entity.Property(e => e.AccessChannel)
                .HasMaxLength(30)
                .HasColumnName("access_channel");
            entity.Property(e => e.AccessLinkType)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Direct'::character varying")
                .HasColumnName("access_link_type");
            entity.Property(e => e.AuthenticationMethod)
                .HasMaxLength(30)
                .HasColumnName("authentication_method");
            entity.Property(e => e.AuthorizationDecision)
                .HasMaxLength(20)
                .HasColumnName("authorization_decision");
            entity.Property(e => e.AuthorizationPolicyCode)
                .HasMaxLength(150)
                .HasColumnName("authorization_policy_code");
            entity.Property(e => e.AuthorizedAt).HasColumnName("authorized_at");
            entity.Property(e => e.BytesTransferred).HasColumnName("bytes_transferred");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.ClientApplication)
                .HasMaxLength(150)
                .HasColumnName("client_application");
            entity.Property(e => e.ClientIp).HasColumnName("client_ip");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.ContentTypeSnapshot)
                .HasMaxLength(150)
                .HasColumnName("content_type_snapshot");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DenialCategory)
                .HasMaxLength(30)
                .HasColumnName("denial_category");
            entity.Property(e => e.DenialCode)
                .HasMaxLength(100)
                .HasColumnName("denial_code");
            entity.Property(e => e.DenialMessage)
                .HasMaxLength(1000)
                .HasColumnName("denial_message");
            entity.Property(e => e.DeviceIdentifier)
                .HasMaxLength(250)
                .HasColumnName("device_identifier");
            entity.Property(e => e.DownloadPurpose)
                .HasMaxLength(500)
                .HasColumnName("download_purpose");
            entity.Property(e => e.DownloadSequence)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn()
                .HasColumnName("download_sequence");
            entity.Property(e => e.DownloadStatus)
                .HasMaxLength(20)
                .HasColumnName("download_status");
            entity.Property(e => e.DurationMilliseconds).HasColumnName("duration_milliseconds");
            entity.Property(e => e.ExportFormatSnapshot)
                .HasMaxLength(20)
                .HasColumnName("export_format_snapshot");
            entity.Property(e => e.FailureCategory)
                .HasMaxLength(30)
                .HasColumnName("failure_category");
            entity.Property(e => e.FailureCode)
                .HasMaxLength(100)
                .HasColumnName("failure_code");
            entity.Property(e => e.FailureDetails)
                .HasColumnType("jsonb")
                .HasColumnName("failure_details");
            entity.Property(e => e.FailureMessage)
                .HasMaxLength(2000)
                .HasColumnName("failure_message");
            entity.Property(e => e.FileNameSnapshot)
                .HasMaxLength(500)
                .HasColumnName("file_name_snapshot");
            entity.Property(e => e.FileSizeBytesSnapshot).HasColumnName("file_size_bytes_snapshot");
            entity.Property(e => e.ForwardedFor)
                .HasMaxLength(500)
                .HasColumnName("forwarded_for");
            entity.Property(e => e.HttpStatusCode).HasColumnName("http_status_code");
            entity.Property(e => e.IsEncryptedSnapshot).HasColumnName("is_encrypted_snapshot");
            entity.Property(e => e.IsPartialDownload).HasColumnName("is_partial_download");
            entity.Property(e => e.RangeEndByte).HasColumnName("range_end_byte");
            entity.Property(e => e.RangeStartByte).HasColumnName("range_start_byte");
            entity.Property(e => e.ReportRunFileId).HasColumnName("report_run_file_id");
            entity.Property(e => e.RequestedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("requested_at");
            entity.Property(e => e.RequestedByType)
                .HasMaxLength(30)
                .HasDefaultValueSql("'User'::character varying")
                .HasColumnName("requested_by_type");
            entity.Property(e => e.RequestedByUserId).HasColumnName("requested_by_user_id");
            entity.Property(e => e.RequiresAuthorizationSnapshot).HasColumnName("requires_authorization_snapshot");
            entity.Property(e => e.SessionIdentifier)
                .HasMaxLength(250)
                .HasColumnName("session_identifier");
            entity.Property(e => e.Sha256HashSnapshot)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("sha256_hash_snapshot");
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
            entity.Property(e => e.TemporaryUrlExpiresAt).HasColumnName("temporary_url_expires_at");
            entity.Property(e => e.TemporaryUrlIssuedAt).HasColumnName("temporary_url_issued_at");
            entity.Property(e => e.TemporaryUrlTokenHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("temporary_url_token_hash");
            entity.Property(e => e.UserAgent)
                .HasMaxLength(1000)
                .HasColumnName("user_agent");

            entity.HasOne(d => d.ReportRunFile).WithMany(p => p.ReportFileDownloads)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportRunFileId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportRunFileId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_downloads_file");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ReportFileDownloads)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RequestedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_downloads_requested_by");
        });

        modelBuilder.Entity<ReportRecipientSuppression>(entity =>
        {
            entity.HasKey(e => e.ReportRecipientSuppressionId).HasName("report_recipient_suppressions_pkey");

            entity.ToTable("report_recipient_suppressions", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_report_suppressions_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.CreatedByUserId, e.CreatedAt }, "ix_report_suppressions_created_by")
                .IsDescending(false, false, true)
                .HasFilter("(created_by_user_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.ReportDefinitionId, e.Status }, "ix_report_suppressions_definition").HasFilter("(report_definition_id IS NOT NULL)");

            entity.HasIndex(e => e.ExpiresAt, "ix_report_suppressions_expiring").HasFilter("(((status)::text = 'Active'::text) AND (expires_at IS NOT NULL))");

            entity.HasIndex(e => e.Metadata, "ix_report_suppressions_metadata")
                .HasMethod("gin")
                .HasOperators(new[] { "jsonb_path_ops" });

            entity.HasIndex(e => new { e.CompanyId, e.ReasonCategory, e.CreatedAt }, "ix_report_suppressions_reason").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ReportScheduleId, e.Status }, "ix_report_suppressions_schedule").HasFilter("(report_schedule_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.SourceDeliveryEventId }, "ix_report_suppressions_source_event").HasFilter("(source_delivery_event_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.UpdatedAt }, "ix_report_suppressions_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.RecipientUserId, e.SuppressionChannel, e.SuppressionScope }, "ix_report_suppressions_user_active").HasFilter("(((status)::text = 'Active'::text) AND (recipient_user_id IS NOT NULL))");

            entity.HasIndex(e => new { e.CompanyId, e.ReportRecipientSuppressionId }, "uq_report_suppressions_company").IsUnique();

            entity.Property(e => e.ReportRecipientSuppressionId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("report_recipient_suppression_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByType)
                .HasMaxLength(30)
                .HasColumnName("created_by_type");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.EffectiveFrom)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("effective_from");
            entity.Property(e => e.ExpiredAt).HasColumnName("expired_at");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.Metadata)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("metadata");
            entity.Property(e => e.ReasonCategory)
                .HasMaxLength(40)
                .HasColumnName("reason_category");
            entity.Property(e => e.ReasonCode)
                .HasMaxLength(100)
                .HasColumnName("reason_code");
            entity.Property(e => e.ReasonMessage)
                .HasMaxLength(1000)
                .HasColumnName("reason_message");
            entity.Property(e => e.RecipientDisplayName)
                .HasMaxLength(200)
                .HasColumnName("recipient_display_name");
            entity.Property(e => e.RecipientEmail)
                .HasMaxLength(320)
                .HasColumnName("recipient_email");
            entity.Property(e => e.RecipientType)
                .HasMaxLength(30)
                .HasColumnName("recipient_type");
            entity.Property(e => e.RecipientUserId).HasColumnName("recipient_user_id");
            entity.Property(e => e.ReportDefinitionId).HasColumnName("report_definition_id");
            entity.Property(e => e.ReportScheduleId).HasColumnName("report_schedule_id");
            entity.Property(e => e.RevocationReason)
                .HasMaxLength(1000)
                .HasColumnName("revocation_reason");
            entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");
            entity.Property(e => e.RevokedByType)
                .HasMaxLength(30)
                .HasColumnName("revoked_by_type");
            entity.Property(e => e.RevokedByUserId).HasColumnName("revoked_by_user_id");
            entity.Property(e => e.SourceDeliveryEventId).HasColumnName("source_delivery_event_id");
            entity.Property(e => e.SourceType)
                .HasMaxLength(30)
                .HasColumnName("source_type");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Active'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SuppressionChannel)
                .HasMaxLength(20)
                .HasColumnName("suppression_channel");
            entity.Property(e => e.SuppressionScope)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Company'::character varying")
                .HasColumnName("suppression_scope");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.Company).WithMany(p => p.ReportRecipientSuppressions)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_suppressions_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ReportRecipientSuppressionAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_suppressions_created_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.ReportRecipientSuppressionAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecipientUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_suppressions_user");

            entity.HasOne(d => d.ReportDefinition).WithMany(p => p.ReportRecipientSuppressions)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDefinitionId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDefinitionId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_suppressions_definition");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.ReportRecipientSuppressionAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RevokedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_suppressions_revoked_by");

            entity.HasOne(d => d.ReportDeliveryEvent).WithMany(p => p.ReportRecipientSuppressions)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDeliveryEventId })
                .HasForeignKey(d => new { d.CompanyId, d.SourceDeliveryEventId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_suppressions_source_event");

            entity.HasOne(d => d.ReportSchedule).WithMany(p => p.ReportRecipientSuppressions)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDefinitionId, p.ReportScheduleId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDefinitionId, d.ReportScheduleId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_suppressions_schedule");
        });

        modelBuilder.Entity<ReportRecipientSuppressionEvent>(entity =>
        {
            entity.HasKey(e => e.ReportRecipientSuppressionEventId).HasName("report_recipient_suppression_events_pkey");

            entity.ToTable("report_recipient_suppression_events", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ChangedByUserId, e.OccurredAt }, "ix_supp_events_actor")
                .IsDescending(false, false, true)
                .HasFilter("(changed_by_user_id IS NOT NULL)");

            entity.HasIndex(e => e.AfterState, "ix_supp_events_after_state")
                .HasMethod("gin")
                .HasOperators(new[] { "jsonb_path_ops" });

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_supp_events_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.ReportDefinitionIdSnapshot, e.OccurredAt }, "ix_supp_events_definition")
                .IsDescending(false, false, true)
                .HasFilter("(report_definition_id_snapshot IS NOT NULL)");

            entity.HasIndex(e => e.EventMetadata, "ix_supp_events_metadata")
                .HasMethod("gin")
                .HasOperators(new[] { "jsonb_path_ops" });

            entity.HasIndex(e => new { e.CompanyId, e.ReasonCategorySnapshot, e.OccurredAt }, "ix_supp_events_reason").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ReportScheduleIdSnapshot, e.OccurredAt }, "ix_supp_events_schedule")
                .IsDescending(false, false, true)
                .HasFilter("(report_schedule_id_snapshot IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.SourceDeliveryEventIdSnapshot }, "ix_supp_events_source_delivery").HasFilter("(source_delivery_event_id_snapshot IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.StatusSnapshot, e.OccurredAt }, "ix_supp_events_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ReportRecipientSuppressionId, e.EventNumber }, "ix_supp_events_suppression").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.EventType, e.OccurredAt }, "ix_supp_events_type").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.RecipientUserIdSnapshot, e.OccurredAt }, "ix_supp_events_user")
                .IsDescending(false, false, true)
                .HasFilter("(recipient_user_id_snapshot IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.ReportRecipientSuppressionEventId }, "uq_supp_events_company_event").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.IdempotencyKey }, "uq_supp_events_idempotency").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReportRecipientSuppressionId, e.EventNumber }, "uq_supp_events_number").IsUnique();

            entity.HasIndex(e => e.EventSequence, "uq_supp_events_sequence").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReportRecipientSuppressionId, e.SuppressionVersionNumber }, "uq_supp_events_version").IsUnique();

            entity.Property(e => e.ReportRecipientSuppressionEventId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("report_recipient_suppression_event_id");
            entity.Property(e => e.AfterState)
                .HasColumnType("jsonb")
                .HasColumnName("after_state");
            entity.Property(e => e.BeforeState)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("before_state");
            entity.Property(e => e.ChangeSummary)
                .HasMaxLength(1000)
                .HasColumnName("change_summary");
            entity.Property(e => e.ChangedByType)
                .HasMaxLength(30)
                .HasColumnName("changed_by_type");
            entity.Property(e => e.ChangedByUserId).HasColumnName("changed_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.EffectiveFromSnapshot).HasColumnName("effective_from_snapshot");
            entity.Property(e => e.EventMetadata)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("event_metadata");
            entity.Property(e => e.EventNumber).HasColumnName("event_number");
            entity.Property(e => e.EventSequence)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn()
                .HasColumnName("event_sequence");
            entity.Property(e => e.EventType)
                .HasMaxLength(30)
                .HasColumnName("event_type");
            entity.Property(e => e.ExpiredAtSnapshot).HasColumnName("expired_at_snapshot");
            entity.Property(e => e.ExpiresAtSnapshot).HasColumnName("expires_at_snapshot");
            entity.Property(e => e.IdempotencyKey)
                .HasMaxLength(200)
                .HasColumnName("idempotency_key");
            entity.Property(e => e.OccurredAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("occurred_at");
            entity.Property(e => e.PreviousStatus)
                .HasMaxLength(20)
                .HasColumnName("previous_status");
            entity.Property(e => e.PreviousSuppressionVersionNumber).HasColumnName("previous_suppression_version_number");
            entity.Property(e => e.ReasonCategorySnapshot)
                .HasMaxLength(40)
                .HasColumnName("reason_category_snapshot");
            entity.Property(e => e.ReasonCodeSnapshot)
                .HasMaxLength(100)
                .HasColumnName("reason_code_snapshot");
            entity.Property(e => e.ReasonMessageSnapshot)
                .HasMaxLength(1000)
                .HasColumnName("reason_message_snapshot");
            entity.Property(e => e.RecipientDisplayNameSnapshot)
                .HasMaxLength(200)
                .HasColumnName("recipient_display_name_snapshot");
            entity.Property(e => e.RecipientEmailSnapshot)
                .HasMaxLength(320)
                .HasColumnName("recipient_email_snapshot");
            entity.Property(e => e.RecipientTypeSnapshot)
                .HasMaxLength(30)
                .HasColumnName("recipient_type_snapshot");
            entity.Property(e => e.RecipientUserIdSnapshot).HasColumnName("recipient_user_id_snapshot");
            entity.Property(e => e.ReportDefinitionIdSnapshot).HasColumnName("report_definition_id_snapshot");
            entity.Property(e => e.ReportRecipientSuppressionId).HasColumnName("report_recipient_suppression_id");
            entity.Property(e => e.ReportScheduleIdSnapshot).HasColumnName("report_schedule_id_snapshot");
            entity.Property(e => e.RevocationReasonSnapshot)
                .HasMaxLength(1000)
                .HasColumnName("revocation_reason_snapshot");
            entity.Property(e => e.RevokedAtSnapshot).HasColumnName("revoked_at_snapshot");
            entity.Property(e => e.RevokedByTypeSnapshot)
                .HasMaxLength(30)
                .HasColumnName("revoked_by_type_snapshot");
            entity.Property(e => e.RevokedByUserIdSnapshot).HasColumnName("revoked_by_user_id_snapshot");
            entity.Property(e => e.SourceDeliveryEventIdSnapshot).HasColumnName("source_delivery_event_id_snapshot");
            entity.Property(e => e.SourceTypeSnapshot)
                .HasMaxLength(30)
                .HasColumnName("source_type_snapshot");
            entity.Property(e => e.StatusSnapshot)
                .HasMaxLength(20)
                .HasColumnName("status_snapshot");
            entity.Property(e => e.SuppressionChannelSnapshot)
                .HasMaxLength(20)
                .HasColumnName("suppression_channel_snapshot");
            entity.Property(e => e.SuppressionScopeSnapshot)
                .HasMaxLength(30)
                .HasColumnName("suppression_scope_snapshot");
            entity.Property(e => e.SuppressionVersionNumber).HasColumnName("suppression_version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ReportRecipientSuppressionEventAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ChangedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supp_events_changed_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.ReportRecipientSuppressionEventAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecipientUserIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supp_events_recipient_user");

            entity.HasOne(d => d.ReportDefinition).WithMany(p => p.ReportRecipientSuppressionEvents)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDefinitionId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDefinitionIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supp_events_report_definition");

            entity.HasOne(d => d.ReportRecipientSuppression).WithMany(p => p.ReportRecipientSuppressionEvents)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportRecipientSuppressionId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportRecipientSuppressionId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supp_events_suppression");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.ReportRecipientSuppressionEventAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RevokedByUserIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supp_events_revoked_by");

            entity.HasOne(d => d.ReportDeliveryEvent).WithMany(p => p.ReportRecipientSuppressionEvents)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDeliveryEventId })
                .HasForeignKey(d => new { d.CompanyId, d.SourceDeliveryEventIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supp_events_source_delivery");

            entity.HasOne(d => d.ReportSchedule).WithMany(p => p.ReportRecipientSuppressionEvents)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDefinitionId, p.ReportScheduleId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDefinitionIdSnapshot, d.ReportScheduleIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supp_events_report_schedule");
        });

        modelBuilder.Entity<ReportRun>(entity =>
        {
            entity.HasKey(e => e.ReportRunId).HasName("report_runs_pkey");

            entity.ToTable("report_runs", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.CacheKeyHash, e.CompletedAt }, "ix_report_runs_cache")
                .IsDescending(false, false, true)
                .HasFilter("(((status)::text = 'Succeeded'::text) AND (cache_key_hash IS NOT NULL) AND (output_expires_at IS NOT NULL))");

            entity.HasIndex(e => new { e.CompanyId, e.CachedFromReportRunId }, "ix_report_runs_cached_from").HasFilter("(cached_from_report_run_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_report_runs_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.ReportDefinitionId, e.CreatedAt }, "ix_report_runs_definition").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.FailureCategory, e.CompletedAt }, "ix_report_runs_failures")
                .IsDescending(false, false, true)
                .HasFilter("((status)::text = 'Failed'::text)");

            entity.HasIndex(e => e.OutputExpiresAt, "ix_report_runs_output_expiry").HasFilter("((status)::text = ANY ((ARRAY['Succeeded'::character varying, 'PartiallySucceeded'::character varying])::text[]))");

            entity.HasIndex(e => new { e.CompanyId, e.ReportDefinitionId, e.ParameterHash, e.CreatedAt }, "ix_report_runs_parameter_hash").IsDescending(false, false, false, true);

            entity.HasIndex(e => new { e.Priority, e.CreatedAt, e.RunSequence }, "ix_report_runs_queue")
                .IsDescending(true, false, false)
                .HasFilter("((status)::text = ANY ((ARRAY['Pending'::character varying, 'Queued'::character varying])::text[]))");

            entity.HasIndex(e => new { e.CompanyId, e.RequestedByUserId, e.CreatedAt }, "ix_report_runs_requester")
                .IsDescending(false, false, true)
                .HasFilter("(requested_by_user_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.StartedAt }, "ix_report_runs_running").HasFilter("((status)::text = 'Running'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_report_runs_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ReportRunId }, "uq_report_runs_company_run").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.IdempotencyKey }, "uq_report_runs_idempotency").IsUnique();

            entity.HasIndex(e => e.RunSequence, "uq_report_runs_sequence").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.BackgroundJobId }, "ux_report_runs_background_job")
                .IsUnique()
                .HasFilter("(background_job_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.ReportDefinitionId, e.ReportRunId }, "ux_report_runs_definition_run_ref").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ScheduledJobRunId }, "ux_report_runs_scheduled_job_run")
                .IsUnique()
                .HasFilter("(scheduled_job_run_id IS NOT NULL)");

            entity.Property(e => e.ReportRunId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("report_run_id");
            entity.Property(e => e.BackgroundJobId).HasColumnName("background_job_id");
            entity.Property(e => e.CacheKeyHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("cache_key_hash");
            entity.Property(e => e.CacheTtlSecondsSnapshot).HasColumnName("cache_ttl_seconds_snapshot");
            entity.Property(e => e.CachedFromReportRunId).HasColumnName("cached_from_report_run_id");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByType)
                .HasMaxLength(30)
                .HasColumnName("cancelled_by_type");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.ContainsSensitiveDataSnapshot).HasColumnName("contains_sensitive_data_snapshot");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasColumnName("currency_code");
            entity.Property(e => e.ExecutionMetrics)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("execution_metrics");
            entity.Property(e => e.ExecutionMode)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Background'::character varying")
                .HasColumnName("execution_mode");
            entity.Property(e => e.ExecutionTimeoutSecondsSnapshot).HasColumnName("execution_timeout_seconds_snapshot");
            entity.Property(e => e.FailureCategory)
                .HasMaxLength(30)
                .HasColumnName("failure_category");
            entity.Property(e => e.FailureCode)
                .HasMaxLength(100)
                .HasColumnName("failure_code");
            entity.Property(e => e.FailureDetails)
                .HasColumnType("jsonb")
                .HasColumnName("failure_details");
            entity.Property(e => e.FailureMessage)
                .HasMaxLength(2000)
                .HasColumnName("failure_message");
            entity.Property(e => e.GeneratedFileCount).HasColumnName("generated_file_count");
            entity.Property(e => e.GeneratedFileRetentionDaysSnapshot).HasColumnName("generated_file_retention_days_snapshot");
            entity.Property(e => e.IdempotencyKey)
                .HasMaxLength(200)
                .HasColumnName("idempotency_key");
            entity.Property(e => e.IsCacheHit).HasColumnName("is_cache_hit");
            entity.Property(e => e.LocaleCode)
                .HasMaxLength(10)
                .HasDefaultValueSql("'ar-LY'::character varying")
                .HasColumnName("locale_code");
            entity.Property(e => e.MaximumRowCountSnapshot).HasColumnName("maximum_row_count_snapshot");
            entity.Property(e => e.OutputExpiresAt).HasColumnName("output_expires_at");
            entity.Property(e => e.ParameterHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("parameter_hash");
            entity.Property(e => e.Parameters)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("parameters");
            entity.Property(e => e.Priority)
                .HasDefaultValue((short)5)
                .HasColumnName("priority");
            entity.Property(e => e.ProcessedRowCount).HasColumnName("processed_row_count");
            entity.Property(e => e.ProgressPercentage)
                .HasPrecision(5, 2)
                .HasColumnName("progress_percentage");
            entity.Property(e => e.QueuedAt).HasColumnName("queued_at");
            entity.Property(e => e.ReportDefinitionId).HasColumnName("report_definition_id");
            entity.Property(e => e.ReportDefinitionVersionNumber).HasColumnName("report_definition_version_number");
            entity.Property(e => e.RequestFingerprintHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("request_fingerprint_hash");
            entity.Property(e => e.RequestedByType)
                .HasMaxLength(30)
                .HasDefaultValueSql("'User'::character varying")
                .HasColumnName("requested_by_type");
            entity.Property(e => e.RequestedByUserId).HasColumnName("requested_by_user_id");
            entity.Property(e => e.RequestedExportFormats)
                .HasDefaultValueSql("'[\"PDF\"]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("requested_export_formats");
            entity.Property(e => e.ResultSummary)
                .HasColumnType("jsonb")
                .HasColumnName("result_summary");
            entity.Property(e => e.RunSequence)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn()
                .HasColumnName("run_sequence");
            entity.Property(e => e.ScheduledJobRunId).HasColumnName("scheduled_job_run_id");
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TimeZone)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Africa/Tripoli'::character varying")
                .HasColumnName("time_zone");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");
            entity.Property(e => e.WarningCount).HasColumnName("warning_count");

            entity.HasOne(d => d.BackgroundJob).WithOne(p => p.ReportRun)
                .HasPrincipalKey<BackgroundJob>(p => new { p.CompanyId, p.BackgroundJobId })
                .HasForeignKey<ReportRun>(d => new { d.CompanyId, d.BackgroundJobId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_runs_background_job");

            entity.HasOne(d => d.ReportRunNavigation).WithMany(p => p.InverseReportRunNavigation)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportRunId })
                .HasForeignKey(d => new { d.CompanyId, d.CachedFromReportRunId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_runs_cached_from");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ReportRunAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_runs_cancelled_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.ReportRunAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RequestedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_runs_requested_by");

            entity.HasOne(d => d.ScheduledJobRun).WithOne(p => p.ReportRun)
                .HasPrincipalKey<ScheduledJobRun>(p => new { p.CompanyId, p.ScheduledJobRunId })
                .HasForeignKey<ReportRun>(d => new { d.CompanyId, d.ScheduledJobRunId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_runs_scheduled_run");

            entity.HasOne(d => d.ReportDefinitionVersion).WithMany(p => p.ReportRuns)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDefinitionId, p.VersionNumber })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDefinitionId, d.ReportDefinitionVersionNumber })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_runs_definition_version");
        });

        modelBuilder.Entity<ReportRunFile>(entity =>
        {
            entity.HasKey(e => e.ReportRunFileId).HasName("report_run_files_pkey");

            entity.ToTable("report_run_files", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.AvailableAt }, "ix_report_run_files_available")
                .IsDescending(false, true)
                .HasFilter("((file_status)::text = 'Available'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_report_run_files_correlation");

            entity.HasIndex(e => e.ExpiredAt, "ix_report_run_files_expired").HasFilter("((file_status)::text = 'Expired'::text)");

            entity.HasIndex(e => e.ExpiresAt, "ix_report_run_files_expiring").HasFilter("((file_status)::text = 'Available'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.FailureCategory, e.FailedAt }, "ix_report_run_files_failed")
                .IsDescending(false, false, true)
                .HasFilter("((file_status)::text = 'Failed'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.ReportRunId, e.ExportFormat, e.PartNumber }, "ix_report_run_files_format");

            entity.HasIndex(e => new { e.CompanyId, e.Sha256Hash }, "ix_report_run_files_hash").HasFilter("(sha256_hash IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.LastDownloadedAt }, "ix_report_run_files_last_download")
                .IsDescending(false, true)
                .HasFilter("(last_downloaded_at IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.ReportRunId, e.FileSequence }, "ix_report_run_files_run");

            entity.HasIndex(e => new { e.CompanyId, e.SourceReportRunFileId }, "ix_report_run_files_source").HasFilter("(source_report_run_file_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.FileStatus, e.CreatedAt }, "ix_report_run_files_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ReportRunFileId }, "uq_report_run_files_company_file").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReportRunId, e.ExportFormat, e.PartNumber }, "uq_report_run_files_format_part").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReportRunId, e.FileSequence }, "uq_report_run_files_sequence").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReportRunId }, "ux_report_run_files_primary")
                .IsUnique()
                .HasFilter("(is_primary = true)");

            entity.HasIndex(e => new { e.CompanyId, e.ReportRunId, e.ReportRunFileId }, "ux_report_run_files_run_file_ref").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.StorageProvider, e.StorageContainer, e.StorageKey }, "ux_report_run_files_storage")
                .IsUnique()
                .HasFilter("((storage_key IS NOT NULL) AND ((file_origin)::text = 'Generated'::text))");

            entity.Property(e => e.ReportRunFileId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("report_run_file_id");
            entity.Property(e => e.AvailableAt).HasColumnName("available_at");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.ContentType)
                .HasMaxLength(150)
                .HasColumnName("content_type");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedByType)
                .HasMaxLength(30)
                .HasColumnName("deleted_by_type");
            entity.Property(e => e.DeletedByUserId).HasColumnName("deleted_by_user_id");
            entity.Property(e => e.DeletionReason)
                .HasMaxLength(500)
                .HasColumnName("deletion_reason");
            entity.Property(e => e.DownloadCount).HasColumnName("download_count");
            entity.Property(e => e.EncryptionAlgorithm)
                .HasMaxLength(50)
                .HasColumnName("encryption_algorithm");
            entity.Property(e => e.EncryptionKeyReference)
                .HasMaxLength(500)
                .HasColumnName("encryption_key_reference");
            entity.Property(e => e.ExpiredAt).HasColumnName("expired_at");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.ExportFormat)
                .HasMaxLength(20)
                .HasColumnName("export_format");
            entity.Property(e => e.FailedAt).HasColumnName("failed_at");
            entity.Property(e => e.FailureCategory)
                .HasMaxLength(30)
                .HasColumnName("failure_category");
            entity.Property(e => e.FailureCode)
                .HasMaxLength(100)
                .HasColumnName("failure_code");
            entity.Property(e => e.FailureDetails)
                .HasColumnType("jsonb")
                .HasColumnName("failure_details");
            entity.Property(e => e.FailureMessage)
                .HasMaxLength(2000)
                .HasColumnName("failure_message");
            entity.Property(e => e.FileExtension)
                .HasMaxLength(20)
                .HasColumnName("file_extension");
            entity.Property(e => e.FileName)
                .HasMaxLength(500)
                .HasColumnName("file_name");
            entity.Property(e => e.FileOrigin)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Generated'::character varying")
                .HasColumnName("file_origin");
            entity.Property(e => e.FileSequence).HasColumnName("file_sequence");
            entity.Property(e => e.FileSizeBytes).HasColumnName("file_size_bytes");
            entity.Property(e => e.FileStatus)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("file_status");
            entity.Property(e => e.GeneratedAt).HasColumnName("generated_at");
            entity.Property(e => e.GenerationStartedAt).HasColumnName("generation_started_at");
            entity.Property(e => e.IsEncrypted).HasColumnName("is_encrypted");
            entity.Property(e => e.IsPrimary).HasColumnName("is_primary");
            entity.Property(e => e.LastDownloadedAt).HasColumnName("last_downloaded_at");
            entity.Property(e => e.LastDownloadedByUserId).HasColumnName("last_downloaded_by_user_id");
            entity.Property(e => e.PartNumber)
                .HasDefaultValue(1)
                .HasColumnName("part_number");
            entity.Property(e => e.ReportRunId).HasColumnName("report_run_id");
            entity.Property(e => e.RequiresAuthorization)
                .HasDefaultValue(true)
                .HasColumnName("requires_authorization");
            entity.Property(e => e.Sha256Hash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("sha256_hash");
            entity.Property(e => e.SourceReportRunFileId).HasColumnName("source_report_run_file_id");
            entity.Property(e => e.StorageContainer)
                .HasMaxLength(250)
                .HasColumnName("storage_container");
            entity.Property(e => e.StorageEtag)
                .HasMaxLength(250)
                .HasColumnName("storage_etag");
            entity.Property(e => e.StorageKey)
                .HasMaxLength(1000)
                .HasColumnName("storage_key");
            entity.Property(e => e.StorageProvider)
                .HasMaxLength(30)
                .HasColumnName("storage_provider");
            entity.Property(e => e.StorageRegion)
                .HasMaxLength(100)
                .HasColumnName("storage_region");
            entity.Property(e => e.StorageVersionId)
                .HasMaxLength(250)
                .HasColumnName("storage_version_id");
            entity.Property(e => e.TotalParts)
                .HasDefaultValue(1)
                .HasColumnName("total_parts");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ReportRunFileAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.DeletedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_run_files_deleted_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.ReportRunFileAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.LastDownloadedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_run_files_last_downloader");

            entity.HasOne(d => d.ReportRun).WithOne(p => p.ReportRunFile)
                .HasPrincipalKey<ReportRun>(p => new { p.CompanyId, p.ReportRunId })
                .HasForeignKey<ReportRunFile>(d => new { d.CompanyId, d.ReportRunId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_run_files_run");

            entity.HasOne(d => d.ReportRunFileNavigation).WithMany(p => p.InverseReportRunFileNavigation)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportRunFileId })
                .HasForeignKey(d => new { d.CompanyId, d.SourceReportRunFileId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_run_files_source");
        });

        modelBuilder.Entity<ReportSchedule>(entity =>
        {
            entity.HasKey(e => e.ReportScheduleId).HasName("report_schedules_pkey");

            entity.ToTable("report_schedules", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.NextExpectedRunAt }, "ix_report_schedules_active").HasFilter("((status)::text = 'Active'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.CreatedByUserId, e.CreatedAt }, "ix_report_schedules_created_by").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ReportDefinitionId, e.Status }, "ix_report_schedules_definition");

            entity.HasIndex(e => new { e.CompanyId, e.ExecutionOwnerUserId, e.Status }, "ix_report_schedules_execution_owner");

            entity.HasIndex(e => new { e.CompanyId, e.ConsecutiveFailureCount, e.LastFailureAt }, "ix_report_schedules_failures")
                .IsDescending(false, true, true)
                .HasFilter("(consecutive_failure_count > 0)");

            entity.HasIndex(e => new { e.CompanyId, e.ScheduledJobDefinitionId }, "ix_report_schedules_job_definition");

            entity.HasIndex(e => new { e.CompanyId, e.LastReportRunId }, "ix_report_schedules_last_run").HasFilter("(last_report_run_id IS NOT NULL)");

            entity.HasIndex(e => e.Parameters, "ix_report_schedules_parameters")
                .HasMethod("gin")
                .HasOperators(new[] { "jsonb_path_ops" });

            entity.HasIndex(e => e.RelativeParameterRules, "ix_report_schedules_relative_rules")
                .HasMethod("gin")
                .HasOperators(new[] { "jsonb_path_ops" });

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.UpdatedAt }, "ix_report_schedules_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ScheduleCode }, "uq_report_schedules_code").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReportScheduleId }, "uq_report_schedules_company_schedule").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ScheduledJobDefinitionId }, "uq_report_schedules_scheduled_job").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReportDefinitionId, e.ReportScheduleId }, "ux_report_schedules_report_schedule_ref").IsUnique();

            entity.Property(e => e.ReportScheduleId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("report_schedule_id");
            entity.Property(e => e.ActivatedAt).HasColumnName("activated_at");
            entity.Property(e => e.ActivatedByUserId).HasColumnName("activated_by_user_id");
            entity.Property(e => e.AttachFilesToEmail).HasColumnName("attach_files_to_email");
            entity.Property(e => e.AutoPauseOnFailureLimit)
                .HasDefaultValue(true)
                .HasColumnName("auto_pause_on_failure_limit");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.ConsecutiveFailureCount).HasColumnName("consecutive_failure_count");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasColumnName("currency_code");
            entity.Property(e => e.DeliveryMode)
                .HasMaxLength(30)
                .HasDefaultValueSql("'NotifyRecipients'::character varying")
                .HasColumnName("delivery_mode");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.DisabledAt).HasColumnName("disabled_at");
            entity.Property(e => e.DisabledByUserId).HasColumnName("disabled_by_user_id");
            entity.Property(e => e.DisabledReason)
                .HasMaxLength(500)
                .HasColumnName("disabled_reason");
            entity.Property(e => e.EmptyResultPolicy)
                .HasMaxLength(30)
                .HasDefaultValueSql("'GenerateEmptyReport'::character varying")
                .HasColumnName("empty_result_policy");
            entity.Property(e => e.ExecutionOwnerUserId).HasColumnName("execution_owner_user_id");
            entity.Property(e => e.FailureNotificationPolicy)
                .HasMaxLength(30)
                .HasDefaultValueSql("'OwnerAndRecipients'::character varying")
                .HasColumnName("failure_notification_policy");
            entity.Property(e => e.FileNameTemplate)
                .HasMaxLength(500)
                .HasDefaultValueSql("'{REPORT_CODE}-{YYYY}-{MM}-{DD}'::character varying")
                .HasColumnName("file_name_template");
            entity.Property(e => e.GeneratedFileRetentionDays)
                .HasDefaultValue(30)
                .HasColumnName("generated_file_retention_days");
            entity.Property(e => e.IdempotencyKeyPrefix)
                .HasMaxLength(150)
                .HasColumnName("idempotency_key_prefix");
            entity.Property(e => e.LastFailureAt).HasColumnName("last_failure_at");
            entity.Property(e => e.LastReportRunId).HasColumnName("last_report_run_id");
            entity.Property(e => e.LastRunAt).HasColumnName("last_run_at");
            entity.Property(e => e.LastRunStatus)
                .HasMaxLength(30)
                .HasColumnName("last_run_status");
            entity.Property(e => e.LastSuccessAt).HasColumnName("last_success_at");
            entity.Property(e => e.LocaleCode)
                .HasMaxLength(10)
                .HasDefaultValueSql("'ar-LY'::character varying")
                .HasColumnName("locale_code");
            entity.Property(e => e.MaxConsecutiveFailures)
                .HasDefaultValue(5)
                .HasColumnName("max_consecutive_failures");
            entity.Property(e => e.MaximumEmailAttachmentBytes)
                .HasDefaultValue(10485760L)
                .HasColumnName("maximum_email_attachment_bytes");
            entity.Property(e => e.NextExpectedRunAt).HasColumnName("next_expected_run_at");
            entity.Property(e => e.ParameterEvaluationMode)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Static'::character varying")
                .HasColumnName("parameter_evaluation_mode");
            entity.Property(e => e.ParameterHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("parameter_hash");
            entity.Property(e => e.Parameters)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("parameters");
            entity.Property(e => e.PauseReason)
                .HasMaxLength(500)
                .HasColumnName("pause_reason");
            entity.Property(e => e.PausedAt).HasColumnName("paused_at");
            entity.Property(e => e.PausedByType)
                .HasMaxLength(30)
                .HasColumnName("paused_by_type");
            entity.Property(e => e.PausedByUserId).HasColumnName("paused_by_user_id");
            entity.Property(e => e.PermissionEvaluationPolicy)
                .HasMaxLength(30)
                .HasDefaultValueSql("'RevalidateOnRun'::character varying")
                .HasColumnName("permission_evaluation_policy");
            entity.Property(e => e.PinnedReportVersionNumber).HasColumnName("pinned_report_version_number");
            entity.Property(e => e.RelativeParameterRules)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("relative_parameter_rules");
            entity.Property(e => e.ReportDefinitionId).HasColumnName("report_definition_id");
            entity.Property(e => e.ReportVersionPolicy)
                .HasMaxLength(30)
                .HasDefaultValueSql("'LatestActive'::character varying")
                .HasColumnName("report_version_policy");
            entity.Property(e => e.RequestedExportFormats)
                .HasDefaultValueSql("'[\"PDF\"]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("requested_export_formats");
            entity.Property(e => e.RetiredAt).HasColumnName("retired_at");
            entity.Property(e => e.RetiredByUserId).HasColumnName("retired_by_user_id");
            entity.Property(e => e.RetirementReason)
                .HasMaxLength(500)
                .HasColumnName("retirement_reason");
            entity.Property(e => e.ScheduleCode)
                .HasMaxLength(120)
                .HasColumnName("schedule_code");
            entity.Property(e => e.ScheduleName)
                .HasMaxLength(250)
                .HasColumnName("schedule_name");
            entity.Property(e => e.ScheduledJobDefinitionId).HasColumnName("scheduled_job_definition_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SuccessNotificationPolicy)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Recipients'::character varying")
                .HasColumnName("success_notification_policy");
            entity.Property(e => e.TimeZone)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Africa/Tripoli'::character varying")
                .HasColumnName("time_zone");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedByUserId).HasColumnName("updated_by_user_id");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.Company).WithMany(p => p.ReportSchedules)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedules_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ReportScheduleAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ActivatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedules_activated_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.ReportScheduleAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedules_created_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.ReportScheduleAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.DisabledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedules_disabled_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.ReportScheduleAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ExecutionOwnerUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedules_execution_owner");

            entity.HasOne(d => d.ReportRun).WithMany(p => p.ReportSchedules)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportRunId })
                .HasForeignKey(d => new { d.CompanyId, d.LastReportRunId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedules_last_run");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.ReportScheduleAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.PausedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedules_paused_by");

            entity.HasOne(d => d.ReportDefinition).WithMany(p => p.ReportSchedules)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDefinitionId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDefinitionId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedules_report_definition");

            entity.HasOne(d => d.AppUser4).WithMany(p => p.ReportScheduleAppUser4s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RetiredByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedules_retired_by");

            entity.HasOne(d => d.ScheduledJobDefinition).WithOne(p => p.ReportSchedule)
                .HasPrincipalKey<ScheduledJobDefinition>(p => new { p.CompanyId, p.ScheduledJobDefinitionId })
                .HasForeignKey<ReportSchedule>(d => new { d.CompanyId, d.ScheduledJobDefinitionId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedules_job_definition");

            entity.HasOne(d => d.AppUser5).WithMany(p => p.ReportScheduleAppUser5s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.UpdatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedules_updated_by");

            entity.HasOne(d => d.ReportDefinitionVersion).WithMany(p => p.ReportSchedules)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDefinitionId, p.VersionNumber })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDefinitionId, d.PinnedReportVersionNumber })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedules_pinned_version");
        });

        modelBuilder.Entity<ReportScheduleRecipient>(entity =>
        {
            entity.HasKey(e => e.ReportScheduleRecipientId).HasName("report_schedule_recipients_pkey");

            entity.ToTable("report_schedule_recipients", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.DeliveryChannel, e.Status }, "ix_report_schedule_recipients_channel");

            entity.HasIndex(e => new { e.CompanyId, e.DisabledAt }, "ix_report_schedule_recipients_disabled")
                .IsDescending(false, true)
                .HasFilter("((status)::text = 'Disabled'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.RemovedAt }, "ix_report_schedule_recipients_removed")
                .IsDescending(false, true)
                .HasFilter("((status)::text = 'Removed'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.RecipientRoleCode, e.Status }, "ix_report_schedule_recipients_role_lookup").HasFilter("(recipient_role_code IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.ReportScheduleId, e.Status, e.RecipientOrder }, "ix_report_schedule_recipients_schedule");

            entity.HasIndex(e => new { e.CompanyId, e.RecipientUserId, e.Status }, "ix_report_schedule_recipients_user_lookup").HasFilter("(recipient_user_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.ReportScheduleRecipientId }, "uq_report_schedule_recipients_company").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReportScheduleId, e.RecipientRoleCode }, "ux_report_schedule_recipients_role")
                .IsUnique()
                .HasFilter("(((recipient_type)::text = 'Role'::text) AND ((status)::text = ANY ((ARRAY['Active'::character varying, 'Disabled'::character varying])::text[])))");

            entity.HasIndex(e => new { e.CompanyId, e.ReportScheduleId, e.RecipientUserId }, "ux_report_schedule_recipients_user")
                .IsUnique()
                .HasFilter("(((recipient_type)::text = 'User'::text) AND ((status)::text = ANY ((ARRAY['Active'::character varying, 'Disabled'::character varying])::text[])))");

            entity.Property(e => e.ReportScheduleRecipientId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("report_schedule_recipient_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.DeliveryChannel)
                .HasMaxLength(20)
                .HasDefaultValueSql("'InApp'::character varying")
                .HasColumnName("delivery_channel");
            entity.Property(e => e.DeliveryFailurePolicy)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Retry'::character varying")
                .HasColumnName("delivery_failure_policy");
            entity.Property(e => e.DisableReason)
                .HasMaxLength(500)
                .HasColumnName("disable_reason");
            entity.Property(e => e.DisabledAt).HasColumnName("disabled_at");
            entity.Property(e => e.DisabledByUserId).HasColumnName("disabled_by_user_id");
            entity.Property(e => e.EmailMessageTemplate)
                .HasMaxLength(4000)
                .HasColumnName("email_message_template");
            entity.Property(e => e.EmailSubjectTemplate)
                .HasMaxLength(500)
                .HasColumnName("email_subject_template");
            entity.Property(e => e.ExternalDisplayName)
                .HasMaxLength(200)
                .HasColumnName("external_display_name");
            entity.Property(e => e.ExternalEmail)
                .HasMaxLength(320)
                .HasColumnName("external_email");
            entity.Property(e => e.ExternalEmailVerifiedAt).HasColumnName("external_email_verified_at");
            entity.Property(e => e.ExternalEmailVerifiedByUserId).HasColumnName("external_email_verified_by_user_id");
            entity.Property(e => e.IncludeDownloadLink)
                .HasDefaultValue(true)
                .HasColumnName("include_download_link");
            entity.Property(e => e.IncludeEmailAttachment).HasColumnName("include_email_attachment");
            entity.Property(e => e.LocaleCode)
                .HasMaxLength(10)
                .HasColumnName("locale_code");
            entity.Property(e => e.MaxDeliveryAttempts)
                .HasDefaultValue(5)
                .HasColumnName("max_delivery_attempts");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.NotifyOnCancelled).HasColumnName("notify_on_cancelled");
            entity.Property(e => e.NotifyOnEmptyResult)
                .HasDefaultValue(true)
                .HasColumnName("notify_on_empty_result");
            entity.Property(e => e.NotifyOnFailure)
                .HasDefaultValue(true)
                .HasColumnName("notify_on_failure");
            entity.Property(e => e.NotifyOnPartialSuccess)
                .HasDefaultValue(true)
                .HasColumnName("notify_on_partial_success");
            entity.Property(e => e.NotifyOnSuccess)
                .HasDefaultValue(true)
                .HasColumnName("notify_on_success");
            entity.Property(e => e.RecipientOrder)
                .HasDefaultValue(1)
                .HasColumnName("recipient_order");
            entity.Property(e => e.RecipientRoleCode)
                .HasMaxLength(50)
                .HasColumnName("recipient_role_code");
            entity.Property(e => e.RecipientType)
                .HasMaxLength(30)
                .HasColumnName("recipient_type");
            entity.Property(e => e.RecipientUserId).HasColumnName("recipient_user_id");
            entity.Property(e => e.RemovalReason)
                .HasMaxLength(500)
                .HasColumnName("removal_reason");
            entity.Property(e => e.RemovedAt).HasColumnName("removed_at");
            entity.Property(e => e.RemovedByUserId).HasColumnName("removed_by_user_id");
            entity.Property(e => e.ReportScheduleId).HasColumnName("report_schedule_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Active'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TimeZone)
                .HasMaxLength(100)
                .HasColumnName("time_zone");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedByUserId).HasColumnName("updated_by_user_id");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ReportScheduleRecipientAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedule_recipients_created_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.ReportScheduleRecipientAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.DisabledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedule_recipients_disabled_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.ReportScheduleRecipientAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ExternalEmailVerifiedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedule_recipients_verified_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.ReportScheduleRecipientAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecipientUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedule_recipients_user");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.ReportScheduleRecipientAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RemovedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedule_recipients_removed_by");

            entity.HasOne(d => d.ReportSchedule).WithMany(p => p.ReportScheduleRecipients)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportScheduleId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportScheduleId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedule_recipients_schedule");

            entity.HasOne(d => d.AppUser4).WithMany(p => p.ReportScheduleRecipientAppUser4s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.UpdatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_schedule_recipients_updated_by");
        });

        modelBuilder.Entity<SavedReportView>(entity =>
        {
            entity.HasKey(e => e.SavedReportViewId).HasName("saved_report_views_pkey");

            entity.ToTable("saved_report_views", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ArchivedAt }, "ix_saved_report_views_archived")
                .IsDescending(false, true)
                .HasFilter("((status)::text = 'Archived'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.ClonedFromSavedReportViewId }, "ix_saved_report_views_cloned_from").HasFilter("(cloned_from_saved_report_view_id IS NOT NULL)");

            entity.HasIndex(e => e.SelectedColumns, "ix_saved_report_views_columns").HasMethod("gin");

            entity.HasIndex(e => new { e.CompanyId, e.DeletedAt }, "ix_saved_report_views_deleted")
                .IsDescending(false, true)
                .HasFilter("((status)::text = 'Deleted'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.OwnerUserId, e.UpdatedAt }, "ix_saved_report_views_favorites")
                .IsDescending(false, false, true)
                .HasFilter("(((status)::text = 'Active'::text) AND (is_favorite = true))");

            entity.HasIndex(e => e.FilterDefinitions, "ix_saved_report_views_filters").HasMethod("gin");

            entity.HasIndex(e => new { e.CompanyId, e.LastReportRunId }, "ix_saved_report_views_last_run").HasFilter("(last_report_run_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.OwnerUserId, e.Status, e.UpdatedAt }, "ix_saved_report_views_owner").IsDescending(false, false, false, true);

            entity.HasIndex(e => e.Parameters, "ix_saved_report_views_parameters")
                .HasMethod("gin")
                .HasOperators(new[] { "jsonb_path_ops" });

            entity.HasIndex(e => new { e.CompanyId, e.ReportDefinitionId, e.Status, e.ViewName }, "ix_saved_report_views_report");

            entity.HasIndex(e => new { e.CompanyId, e.ReportDefinitionId, e.Visibility, e.UpdatedAt }, "ix_saved_report_views_shared")
                .IsDescending(false, false, false, true)
                .HasFilter("(((status)::text = 'Active'::text) AND ((visibility)::text = ANY ((ARRAY['Shared'::character varying, 'Company'::character varying])::text[])))");

            entity.HasIndex(e => new { e.CompanyId, e.SavedReportViewId }, "uq_saved_report_views_company_view").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ReportDefinitionId, e.SavedReportViewId }, "uq_saved_report_views_report_ref").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ViewCode }, "ux_saved_report_views_code")
                .IsUnique()
                .HasFilter("((view_code IS NOT NULL) AND ((status)::text <> 'Deleted'::text))");

            entity.HasIndex(e => new { e.CompanyId, e.ReportDefinitionId }, "ux_saved_report_views_company_default")
                .IsUnique()
                .HasFilter("((is_company_default = true) AND ((status)::text = 'Active'::text))");

            entity.HasIndex(e => new { e.CompanyId, e.ReportDefinitionId, e.OwnerUserId }, "ux_saved_report_views_owner_default")
                .IsUnique()
                .HasFilter("((is_default_for_owner = true) AND ((status)::text = 'Active'::text))");

            entity.Property(e => e.SavedReportViewId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("saved_report_view_id");
            entity.Property(e => e.AggregationDefinitions)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("aggregation_definitions");
            entity.Property(e => e.AllowScheduling)
                .HasDefaultValue(true)
                .HasColumnName("allow_scheduling");
            entity.Property(e => e.ArchiveReason)
                .HasMaxLength(500)
                .HasColumnName("archive_reason");
            entity.Property(e => e.ArchivedAt).HasColumnName("archived_at");
            entity.Property(e => e.ArchivedByUserId).HasColumnName("archived_by_user_id");
            entity.Property(e => e.ClonedFromSavedReportViewId).HasColumnName("cloned_from_saved_report_view_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.ContainsSensitiveParameters).HasColumnName("contains_sensitive_parameters");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedByUserId).HasColumnName("deleted_by_user_id");
            entity.Property(e => e.DeletionReason)
                .HasMaxLength(500)
                .HasColumnName("deletion_reason");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.ExportSettings)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("export_settings");
            entity.Property(e => e.FilterDefinitions)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("filter_definitions");
            entity.Property(e => e.GroupingDefinitions)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("grouping_definitions");
            entity.Property(e => e.IsCompanyDefault).HasColumnName("is_company_default");
            entity.Property(e => e.IsDefaultForOwner).HasColumnName("is_default_for_owner");
            entity.Property(e => e.IsFavorite).HasColumnName("is_favorite");
            entity.Property(e => e.LastReportRunId).HasColumnName("last_report_run_id");
            entity.Property(e => e.LastRunAt).HasColumnName("last_run_at");
            entity.Property(e => e.LayoutSettings)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("layout_settings");
            entity.Property(e => e.OwnerUserId).HasColumnName("owner_user_id");
            entity.Property(e => e.Parameters)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("parameters");
            entity.Property(e => e.PinnedReportVersionNumber).HasColumnName("pinned_report_version_number");
            entity.Property(e => e.PreferredExportFormat)
                .HasMaxLength(20)
                .HasDefaultValueSql("'PDF'::character varying")
                .HasColumnName("preferred_export_format");
            entity.Property(e => e.ReportDefinitionId).HasColumnName("report_definition_id");
            entity.Property(e => e.ReportVersionPolicy)
                .HasMaxLength(30)
                .HasDefaultValueSql("'LatestActive'::character varying")
                .HasColumnName("report_version_policy");
            entity.Property(e => e.RunCount).HasColumnName("run_count");
            entity.Property(e => e.SelectedColumns)
                .HasColumnType("jsonb")
                .HasColumnName("selected_columns");
            entity.Property(e => e.SortingDefinitions)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("sorting_definitions");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Active'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedByUserId).HasColumnName("updated_by_user_id");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");
            entity.Property(e => e.ViewCode)
                .HasMaxLength(120)
                .HasColumnName("view_code");
            entity.Property(e => e.ViewName)
                .HasMaxLength(250)
                .HasColumnName("view_name");
            entity.Property(e => e.ViewOrigin)
                .HasMaxLength(30)
                .HasDefaultValueSql("'UserCreated'::character varying")
                .HasColumnName("view_origin");
            entity.Property(e => e.Visibility)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Personal'::character varying")
                .HasColumnName("visibility");

            entity.HasOne(d => d.Company).WithMany(p => p.SavedReportViews)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_report_views_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.SavedReportViewAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ArchivedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_report_views_archived_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.SavedReportViewAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_report_views_created_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.SavedReportViewAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.DeletedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_report_views_deleted_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.SavedReportViewAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.OwnerUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_report_views_owner");

            entity.HasOne(d => d.ReportDefinition).WithOne(p => p.SavedReportView)
                .HasPrincipalKey<ReportDefinition>(p => new { p.CompanyId, p.ReportDefinitionId })
                .HasForeignKey<SavedReportView>(d => new { d.CompanyId, d.ReportDefinitionId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_report_views_definition");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.SavedReportViewAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.UpdatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_report_views_updated_by");

            entity.HasOne(d => d.SavedReportViewNavigation).WithMany(p => p.InverseSavedReportViewNavigation)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDefinitionId, p.SavedReportViewId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDefinitionId, d.ClonedFromSavedReportViewId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_report_views_cloned_from");

            entity.HasOne(d => d.ReportRun).WithMany(p => p.SavedReportViews)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDefinitionId, p.ReportRunId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDefinitionId, d.LastReportRunId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_report_views_last_run");

            entity.HasOne(d => d.ReportDefinitionVersion).WithMany(p => p.SavedReportViews)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDefinitionId, p.VersionNumber })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDefinitionId, d.PinnedReportVersionNumber })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_report_views_pinned_version");
        });

        modelBuilder.Entity<SavedReportViewShare>(entity =>
        {
            entity.HasKey(e => e.SavedReportViewShareId).HasName("saved_report_view_shares_pkey");

            entity.ToTable("saved_report_view_shares", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ExpiredAt }, "ix_saved_view_shares_expired")
                .IsDescending(false, true)
                .HasFilter("((status)::text = 'Expired'::text)");

            entity.HasIndex(e => e.ExpiresAt, "ix_saved_view_shares_expiring").HasFilter("(((status)::text = 'Active'::text) AND (expires_at IS NOT NULL))");

            entity.HasIndex(e => new { e.CompanyId, e.GrantedByUserId, e.CreatedAt }, "ix_saved_view_shares_granted_by").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.RevokedAt }, "ix_saved_view_shares_revoked")
                .IsDescending(false, true)
                .HasFilter("((status)::text = 'Revoked'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.SharedWithRoleCode, e.Status, e.EffectiveFrom }, "ix_saved_view_shares_role")
                .IsDescending(false, false, false, true)
                .HasFilter("(shared_with_role_code IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.SharedWithUserId, e.Status, e.EffectiveFrom }, "ix_saved_view_shares_user")
                .IsDescending(false, false, false, true)
                .HasFilter("(shared_with_user_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.SavedReportViewId, e.Status }, "ix_saved_view_shares_view");

            entity.HasIndex(e => new { e.CompanyId, e.SavedReportViewShareId }, "uq_saved_view_shares_company_share").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.SavedReportViewId, e.SharedWithRoleCode }, "ux_saved_view_shares_active_role")
                .IsUnique()
                .HasFilter("(((share_target_type)::text = 'Role'::text) AND ((status)::text = 'Active'::text))");

            entity.HasIndex(e => new { e.CompanyId, e.SavedReportViewId, e.SharedWithUserId }, "ux_saved_view_shares_active_user")
                .IsUnique()
                .HasFilter("(((share_target_type)::text = 'User'::text) AND ((status)::text = 'Active'::text))");

            entity.HasIndex(e => new { e.CompanyId, e.SavedReportViewId, e.SavedReportViewShareId }, "ux_saved_view_shares_view_share_ref").IsUnique();

            entity.Property(e => e.SavedReportViewShareId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("saved_report_view_share_id");
            entity.Property(e => e.CanEdit).HasColumnName("can_edit");
            entity.Property(e => e.CanExport)
                .HasDefaultValue(true)
                .HasColumnName("can_export");
            entity.Property(e => e.CanReshare).HasColumnName("can_reshare");
            entity.Property(e => e.CanRun)
                .HasDefaultValue(true)
                .HasColumnName("can_run");
            entity.Property(e => e.CanSchedule).HasColumnName("can_schedule");
            entity.Property(e => e.CanView)
                .HasDefaultValue(true)
                .HasColumnName("can_view");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.EffectiveFrom)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("effective_from");
            entity.Property(e => e.ExpiredAt).HasColumnName("expired_at");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.GrantReason)
                .HasMaxLength(500)
                .HasColumnName("grant_reason");
            entity.Property(e => e.GrantedByUserId).HasColumnName("granted_by_user_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.RevocationReason)
                .HasMaxLength(500)
                .HasColumnName("revocation_reason");
            entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");
            entity.Property(e => e.RevokedByUserId).HasColumnName("revoked_by_user_id");
            entity.Property(e => e.SavedReportViewId).HasColumnName("saved_report_view_id");
            entity.Property(e => e.ShareTargetType)
                .HasMaxLength(20)
                .HasColumnName("share_target_type");
            entity.Property(e => e.SharedWithRoleCode)
                .HasMaxLength(50)
                .HasColumnName("shared_with_role_code");
            entity.Property(e => e.SharedWithUserId).HasColumnName("shared_with_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Active'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.SavedReportViewShareAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.GrantedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_shares_granted_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.SavedReportViewShareAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RevokedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_shares_revoked_by");

            entity.HasOne(d => d.SavedReportView).WithMany(p => p.SavedReportViewShares)
                .HasPrincipalKey(p => new { p.CompanyId, p.SavedReportViewId })
                .HasForeignKey(d => new { d.CompanyId, d.SavedReportViewId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_shares_view");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.SavedReportViewShareAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.SharedWithUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_shares_user");
        });

        modelBuilder.Entity<SavedReportViewShareEvent>(entity =>
        {
            entity.HasKey(e => e.SavedReportViewShareEventId).HasName("saved_report_view_share_events_pkey");

            entity.ToTable("saved_report_view_share_events", "ahdah");

            entity.HasIndex(e => e.AfterState, "ix_saved_view_share_events_after_state")
                .HasMethod("gin")
                .HasOperators(new[] { "jsonb_path_ops" });

            entity.HasIndex(e => e.BeforeState, "ix_saved_view_share_events_before_state")
                .HasMethod("gin")
                .HasOperators(new[] { "jsonb_path_ops" });

            entity.HasIndex(e => new { e.CompanyId, e.ChangedByUserId, e.OccurredAt }, "ix_saved_view_share_events_changed_by")
                .IsDescending(false, false, true)
                .HasFilter("(changed_by_user_id IS NOT NULL)");

            entity.HasIndex(e => e.ChangedFields, "ix_saved_view_share_events_changed_fields").HasMethod("gin");

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_saved_view_share_events_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.ExpiresAtSnapshot }, "ix_saved_view_share_events_expiry").HasFilter("(expires_at_snapshot IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.GrantedByUserIdSnapshot, e.OccurredAt }, "ix_saved_view_share_events_granted_by").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.SnapshotHash }, "ix_saved_view_share_events_hash");

            entity.HasIndex(e => new { e.CompanyId, e.SavedReportViewShareId, e.ShareVersionNumber }, "ix_saved_view_share_events_share").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.StatusSnapshot, e.OccurredAt }, "ix_saved_view_share_events_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.SharedWithRoleCodeSnapshot, e.OccurredAt }, "ix_saved_view_share_events_target_role")
                .IsDescending(false, false, true)
                .HasFilter("(shared_with_role_code_snapshot IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.SharedWithUserIdSnapshot, e.OccurredAt }, "ix_saved_view_share_events_target_user")
                .IsDescending(false, false, true)
                .HasFilter("(shared_with_user_id_snapshot IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.EventType, e.OccurredAt }, "ix_saved_view_share_events_type").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.SavedReportViewIdSnapshot, e.OccurredAt }, "ix_saved_view_share_events_view").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.SavedReportViewShareEventId }, "uq_saved_view_share_events_company").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.IdempotencyKey }, "uq_saved_view_share_events_idempotency").IsUnique();

            entity.HasIndex(e => e.EventSequence, "uq_saved_view_share_events_sequence").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.SavedReportViewShareId, e.ShareVersionNumber }, "uq_saved_view_share_events_version").IsUnique();

            entity.Property(e => e.SavedReportViewShareEventId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("saved_report_view_share_event_id");
            entity.Property(e => e.AfterState)
                .HasColumnType("jsonb")
                .HasColumnName("after_state");
            entity.Property(e => e.BeforeState)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("before_state");
            entity.Property(e => e.CanEditSnapshot).HasColumnName("can_edit_snapshot");
            entity.Property(e => e.CanExportSnapshot).HasColumnName("can_export_snapshot");
            entity.Property(e => e.CanReshareSnapshot).HasColumnName("can_reshare_snapshot");
            entity.Property(e => e.CanRunSnapshot).HasColumnName("can_run_snapshot");
            entity.Property(e => e.CanScheduleSnapshot).HasColumnName("can_schedule_snapshot");
            entity.Property(e => e.CanViewSnapshot).HasColumnName("can_view_snapshot");
            entity.Property(e => e.ChangeSummary)
                .HasMaxLength(1000)
                .HasColumnName("change_summary");
            entity.Property(e => e.ChangedByType)
                .HasMaxLength(30)
                .HasColumnName("changed_by_type");
            entity.Property(e => e.ChangedByUserId).HasColumnName("changed_by_user_id");
            entity.Property(e => e.ChangedFields)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("changed_fields");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.EffectiveFromSnapshot).HasColumnName("effective_from_snapshot");
            entity.Property(e => e.EventSequence)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn()
                .HasColumnName("event_sequence");
            entity.Property(e => e.EventType)
                .HasMaxLength(30)
                .HasColumnName("event_type");
            entity.Property(e => e.ExpiredAtSnapshot).HasColumnName("expired_at_snapshot");
            entity.Property(e => e.ExpiresAtSnapshot).HasColumnName("expires_at_snapshot");
            entity.Property(e => e.GrantReasonSnapshot)
                .HasMaxLength(500)
                .HasColumnName("grant_reason_snapshot");
            entity.Property(e => e.GrantedByUserIdSnapshot).HasColumnName("granted_by_user_id_snapshot");
            entity.Property(e => e.IdempotencyKey)
                .HasMaxLength(200)
                .HasColumnName("idempotency_key");
            entity.Property(e => e.NotesSnapshot)
                .HasMaxLength(1000)
                .HasColumnName("notes_snapshot");
            entity.Property(e => e.OccurredAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("occurred_at");
            entity.Property(e => e.PreviousShareVersionNumber).HasColumnName("previous_share_version_number");
            entity.Property(e => e.PreviousSnapshotHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("previous_snapshot_hash");
            entity.Property(e => e.PreviousStatus)
                .HasMaxLength(20)
                .HasColumnName("previous_status");
            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.RevocationReasonSnapshot)
                .HasMaxLength(500)
                .HasColumnName("revocation_reason_snapshot");
            entity.Property(e => e.RevokedAtSnapshot).HasColumnName("revoked_at_snapshot");
            entity.Property(e => e.RevokedByUserIdSnapshot).HasColumnName("revoked_by_user_id_snapshot");
            entity.Property(e => e.SavedReportViewIdSnapshot).HasColumnName("saved_report_view_id_snapshot");
            entity.Property(e => e.SavedReportViewShareId).HasColumnName("saved_report_view_share_id");
            entity.Property(e => e.ShareTargetTypeSnapshot)
                .HasMaxLength(20)
                .HasColumnName("share_target_type_snapshot");
            entity.Property(e => e.ShareVersionNumber).HasColumnName("share_version_number");
            entity.Property(e => e.SharedWithRoleCodeSnapshot)
                .HasMaxLength(50)
                .HasColumnName("shared_with_role_code_snapshot");
            entity.Property(e => e.SharedWithUserIdSnapshot).HasColumnName("shared_with_user_id_snapshot");
            entity.Property(e => e.SnapshotHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("snapshot_hash");
            entity.Property(e => e.StatusSnapshot)
                .HasMaxLength(20)
                .HasColumnName("status_snapshot");

            entity.HasOne(d => d.AppUser).WithMany(p => p.SavedReportViewShareEventAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ChangedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_share_events_changed_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.SavedReportViewShareEventAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.GrantedByUserIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_share_events_granted_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.SavedReportViewShareEventAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RevokedByUserIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_share_events_revoked_by");

            entity.HasOne(d => d.SavedReportView).WithMany(p => p.SavedReportViewShareEvents)
                .HasPrincipalKey(p => new { p.CompanyId, p.SavedReportViewId })
                .HasForeignKey(d => new { d.CompanyId, d.SavedReportViewIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_share_events_view");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.SavedReportViewShareEventAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.SharedWithUserIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_share_events_target_user");

            entity.HasOne(d => d.SavedReportViewShare).WithMany(p => p.SavedReportViewShareEvents)
                .HasPrincipalKey(p => new { p.CompanyId, p.SavedReportViewId, p.SavedReportViewShareId })
                .HasForeignKey(d => new { d.CompanyId, d.SavedReportViewIdSnapshot, d.SavedReportViewShareId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_share_events_share");
        });

        modelBuilder.Entity<SavedReportViewVersion>(entity =>
        {
            entity.HasKey(e => e.SavedReportViewVersionId).HasName("saved_report_view_versions_pkey");

            entity.ToTable("saved_report_view_versions", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ChangeType, e.OccurredAt }, "ix_saved_view_versions_change_type").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ChangedByUserId, e.OccurredAt }, "ix_saved_view_versions_changed_by")
                .IsDescending(false, false, true)
                .HasFilter("(changed_by_user_id IS NOT NULL)");

            entity.HasIndex(e => e.ChangedFields, "ix_saved_view_versions_changed_fields").HasMethod("gin");

            entity.HasIndex(e => e.SelectedColumnsSnapshot, "ix_saved_view_versions_columns").HasMethod("gin");

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_saved_view_versions_correlation");

            entity.HasIndex(e => e.FilterDefinitionsSnapshot, "ix_saved_view_versions_filters").HasMethod("gin");

            entity.HasIndex(e => new { e.CompanyId, e.LastReportRunIdSnapshot }, "ix_saved_view_versions_last_run").HasFilter("(last_report_run_id_snapshot IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.OwnerUserIdSnapshot, e.OccurredAt }, "ix_saved_view_versions_owner").IsDescending(false, false, true);

            entity.HasIndex(e => e.ParametersSnapshot, "ix_saved_view_versions_parameters")
                .HasMethod("gin")
                .HasOperators(new[] { "jsonb_path_ops" });

            entity.HasIndex(e => new { e.CompanyId, e.ReportDefinitionIdSnapshot, e.OccurredAt }, "ix_saved_view_versions_report").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.SnapshotHash }, "ix_saved_view_versions_snapshot_hash");

            entity.HasIndex(e => new { e.CompanyId, e.StatusSnapshot, e.OccurredAt }, "ix_saved_view_versions_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.SavedReportViewId, e.ViewVersionNumber }, "ix_saved_view_versions_view").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.SavedReportViewVersionId }, "uq_saved_view_versions_company").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.SavedReportViewId, e.ViewVersionNumber }, "uq_saved_view_versions_number").IsUnique();

            entity.HasIndex(e => e.VersionSequence, "uq_saved_view_versions_sequence").IsUnique();

            entity.Property(e => e.SavedReportViewVersionId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("saved_report_view_version_id");
            entity.Property(e => e.AggregationDefinitionsSnapshot)
                .HasColumnType("jsonb")
                .HasColumnName("aggregation_definitions_snapshot");
            entity.Property(e => e.AllowSchedulingSnapshot).HasColumnName("allow_scheduling_snapshot");
            entity.Property(e => e.ArchiveReasonSnapshot)
                .HasMaxLength(500)
                .HasColumnName("archive_reason_snapshot");
            entity.Property(e => e.ArchivedAtSnapshot).HasColumnName("archived_at_snapshot");
            entity.Property(e => e.ArchivedByUserIdSnapshot).HasColumnName("archived_by_user_id_snapshot");
            entity.Property(e => e.ChangeSummary)
                .HasMaxLength(1000)
                .HasColumnName("change_summary");
            entity.Property(e => e.ChangeType)
                .HasMaxLength(40)
                .HasColumnName("change_type");
            entity.Property(e => e.ChangedByType)
                .HasMaxLength(30)
                .HasColumnName("changed_by_type");
            entity.Property(e => e.ChangedByUserId).HasColumnName("changed_by_user_id");
            entity.Property(e => e.ChangedFields)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("changed_fields");
            entity.Property(e => e.ClonedFromSavedReportViewIdSnapshot).HasColumnName("cloned_from_saved_report_view_id_snapshot");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.ContainsSensitiveParametersSnapshot).HasColumnName("contains_sensitive_parameters_snapshot");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserIdSnapshot).HasColumnName("created_by_user_id_snapshot");
            entity.Property(e => e.DeletedAtSnapshot).HasColumnName("deleted_at_snapshot");
            entity.Property(e => e.DeletedByUserIdSnapshot).HasColumnName("deleted_by_user_id_snapshot");
            entity.Property(e => e.DeletionReasonSnapshot)
                .HasMaxLength(500)
                .HasColumnName("deletion_reason_snapshot");
            entity.Property(e => e.DescriptionSnapshot)
                .HasMaxLength(1000)
                .HasColumnName("description_snapshot");
            entity.Property(e => e.ExportSettingsSnapshot)
                .HasColumnType("jsonb")
                .HasColumnName("export_settings_snapshot");
            entity.Property(e => e.FilterDefinitionsSnapshot)
                .HasColumnType("jsonb")
                .HasColumnName("filter_definitions_snapshot");
            entity.Property(e => e.GroupingDefinitionsSnapshot)
                .HasColumnType("jsonb")
                .HasColumnName("grouping_definitions_snapshot");
            entity.Property(e => e.IsCompanyDefaultSnapshot).HasColumnName("is_company_default_snapshot");
            entity.Property(e => e.IsDefaultForOwnerSnapshot).HasColumnName("is_default_for_owner_snapshot");
            entity.Property(e => e.IsFavoriteSnapshot).HasColumnName("is_favorite_snapshot");
            entity.Property(e => e.LastReportRunIdSnapshot).HasColumnName("last_report_run_id_snapshot");
            entity.Property(e => e.LastRunAtSnapshot).HasColumnName("last_run_at_snapshot");
            entity.Property(e => e.LayoutSettingsSnapshot)
                .HasColumnType("jsonb")
                .HasColumnName("layout_settings_snapshot");
            entity.Property(e => e.OccurredAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("occurred_at");
            entity.Property(e => e.OwnerUserIdSnapshot).HasColumnName("owner_user_id_snapshot");
            entity.Property(e => e.ParametersSnapshot)
                .HasColumnType("jsonb")
                .HasColumnName("parameters_snapshot");
            entity.Property(e => e.PinnedReportVersionNumberSnapshot).HasColumnName("pinned_report_version_number_snapshot");
            entity.Property(e => e.PreferredExportFormatSnapshot)
                .HasMaxLength(20)
                .HasColumnName("preferred_export_format_snapshot");
            entity.Property(e => e.PreviousSnapshotHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("previous_snapshot_hash");
            entity.Property(e => e.PreviousStatus)
                .HasMaxLength(20)
                .HasColumnName("previous_status");
            entity.Property(e => e.PreviousViewVersionNumber).HasColumnName("previous_view_version_number");
            entity.Property(e => e.ReportDefinitionIdSnapshot).HasColumnName("report_definition_id_snapshot");
            entity.Property(e => e.ReportVersionPolicySnapshot)
                .HasMaxLength(30)
                .HasColumnName("report_version_policy_snapshot");
            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.RunCountSnapshot).HasColumnName("run_count_snapshot");
            entity.Property(e => e.SavedReportViewId).HasColumnName("saved_report_view_id");
            entity.Property(e => e.SelectedColumnsSnapshot)
                .HasColumnType("jsonb")
                .HasColumnName("selected_columns_snapshot");
            entity.Property(e => e.SnapshotHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("snapshot_hash");
            entity.Property(e => e.SortingDefinitionsSnapshot)
                .HasColumnType("jsonb")
                .HasColumnName("sorting_definitions_snapshot");
            entity.Property(e => e.StatusSnapshot)
                .HasMaxLength(20)
                .HasColumnName("status_snapshot");
            entity.Property(e => e.UpdatedByUserIdSnapshot).HasColumnName("updated_by_user_id_snapshot");
            entity.Property(e => e.VersionSequence)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn()
                .HasColumnName("version_sequence");
            entity.Property(e => e.ViewCodeSnapshot)
                .HasMaxLength(120)
                .HasColumnName("view_code_snapshot");
            entity.Property(e => e.ViewNameSnapshot)
                .HasMaxLength(250)
                .HasColumnName("view_name_snapshot");
            entity.Property(e => e.ViewOriginSnapshot)
                .HasMaxLength(30)
                .HasColumnName("view_origin_snapshot");
            entity.Property(e => e.ViewVersionNumber).HasColumnName("view_version_number");
            entity.Property(e => e.VisibilitySnapshot)
                .HasMaxLength(20)
                .HasColumnName("visibility_snapshot");

            entity.HasOne(d => d.AppUser).WithMany(p => p.SavedReportViewVersionAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ArchivedByUserIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_versions_archived_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.SavedReportViewVersionAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ChangedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_versions_changed_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.SavedReportViewVersionAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_versions_created_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.SavedReportViewVersionAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.DeletedByUserIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_versions_deleted_by");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.SavedReportViewVersionAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.OwnerUserIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_versions_owner");

            entity.HasOne(d => d.AppUser4).WithMany(p => p.SavedReportViewVersionAppUser4s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.UpdatedByUserIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_versions_updated_by");

            entity.HasOne(d => d.SavedReportView).WithMany(p => p.SavedReportViewVersionSavedReportViews)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDefinitionId, p.SavedReportViewId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDefinitionIdSnapshot, d.ClonedFromSavedReportViewIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_versions_cloned_from");

            entity.HasOne(d => d.ReportRun).WithMany(p => p.SavedReportViewVersions)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDefinitionId, p.ReportRunId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDefinitionIdSnapshot, d.LastReportRunIdSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_versions_last_run");

            entity.HasOne(d => d.ReportDefinitionVersion).WithMany(p => p.SavedReportViewVersions)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDefinitionId, p.VersionNumber })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDefinitionIdSnapshot, d.PinnedReportVersionNumberSnapshot })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_versions_pinned_report");

            entity.HasOne(d => d.SavedReportViewNavigation).WithMany(p => p.SavedReportViewVersionSavedReportViewNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.ReportDefinitionId, p.SavedReportViewId })
                .HasForeignKey(d => new { d.CompanyId, d.ReportDefinitionIdSnapshot, d.SavedReportViewId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_saved_view_versions_view");
        });

        modelBuilder.Entity<ScheduledJobDefinition>(entity =>
        {
            entity.HasKey(e => e.ScheduledJobDefinitionId).HasName("scheduled_job_definitions_pkey");

            entity.ToTable("scheduled_job_definitions", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.UpdatedAt }, "ix_sched_jobs_company_status").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.CreatedByUserId }, "ix_sched_jobs_created_by");

            entity.HasIndex(e => new { e.NextRunAt, e.Priority }, "ix_sched_jobs_due")
                .IsDescending(false, true)
                .HasFilter("((status)::text = 'Active'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.LastBackgroundJobId }, "ix_sched_jobs_last_background_job").HasFilter("(last_background_job_id IS NOT NULL)");

            entity.HasIndex(e => new { e.QueueName, e.Status, e.NextRunAt }, "ix_sched_jobs_queue");

            entity.HasIndex(e => e.ScheduleEndAt, "ix_sched_jobs_schedule_end").HasFilter("(((status)::text = 'Active'::text) AND (schedule_end_at IS NOT NULL))");

            entity.HasIndex(e => new { e.CompanyId, e.JobType }, "ix_sched_jobs_type");

            entity.HasIndex(e => new { e.CompanyId, e.JobCode }, "uq_sched_jobs_company_code").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ScheduledJobDefinitionId }, "uq_sched_jobs_company_definition").IsUnique();

            entity.Property(e => e.ScheduledJobDefinitionId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("scheduled_job_definition_id");
            entity.Property(e => e.ActivatedAt).HasColumnName("activated_at");
            entity.Property(e => e.ActivatedByUserId).HasColumnName("activated_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.ConcurrencyPolicy)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Forbid'::character varying")
                .HasColumnName("concurrency_policy");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CronExpression)
                .HasMaxLength(200)
                .HasColumnName("cron_expression");
            entity.Property(e => e.DayOfMonth).HasColumnName("day_of_month");
            entity.Property(e => e.DayOfWeek).HasColumnName("day_of_week");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.DisabledAt).HasColumnName("disabled_at");
            entity.Property(e => e.DisabledByUserId).HasColumnName("disabled_by_user_id");
            entity.Property(e => e.DisabledReason)
                .HasMaxLength(500)
                .HasColumnName("disabled_reason");
            entity.Property(e => e.GeneratedRunCount).HasColumnName("generated_run_count");
            entity.Property(e => e.Headers)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("headers");
            entity.Property(e => e.IdempotencyKeyPrefix)
                .HasMaxLength(150)
                .HasColumnName("idempotency_key_prefix");
            entity.Property(e => e.InitialRetryDelaySeconds)
                .HasDefaultValue(60)
                .HasColumnName("initial_retry_delay_seconds");
            entity.Property(e => e.IntervalSeconds).HasColumnName("interval_seconds");
            entity.Property(e => e.JobCode)
                .HasMaxLength(150)
                .HasColumnName("job_code");
            entity.Property(e => e.JobMaxAttempts)
                .HasDefaultValue(5)
                .HasColumnName("job_max_attempts");
            entity.Property(e => e.JobName)
                .HasMaxLength(250)
                .HasColumnName("job_name");
            entity.Property(e => e.JobType)
                .HasMaxLength(150)
                .HasColumnName("job_type");
            entity.Property(e => e.LastBackgroundJobId).HasColumnName("last_background_job_id");
            entity.Property(e => e.LastEnqueuedAt).HasColumnName("last_enqueued_at");
            entity.Property(e => e.LastJobStatus)
                .HasMaxLength(20)
                .HasColumnName("last_job_status");
            entity.Property(e => e.LastScheduledFor).HasColumnName("last_scheduled_for");
            entity.Property(e => e.MaxConcurrentRuns)
                .HasDefaultValue(1)
                .HasColumnName("max_concurrent_runs");
            entity.Property(e => e.MaxRunCount).HasColumnName("max_run_count");
            entity.Property(e => e.MaximumRetryDelaySeconds)
                .HasDefaultValue(3600)
                .HasColumnName("maximum_retry_delay_seconds");
            entity.Property(e => e.MisfirePolicy)
                .HasMaxLength(30)
                .HasDefaultValueSql("'RunImmediately'::character varying")
                .HasColumnName("misfire_policy");
            entity.Property(e => e.NextRunAt).HasColumnName("next_run_at");
            entity.Property(e => e.OneTimeRunAt).HasColumnName("one_time_run_at");
            entity.Property(e => e.PauseReason)
                .HasMaxLength(500)
                .HasColumnName("pause_reason");
            entity.Property(e => e.PausedAt).HasColumnName("paused_at");
            entity.Property(e => e.PausedByUserId).HasColumnName("paused_by_user_id");
            entity.Property(e => e.Payload)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("payload");
            entity.Property(e => e.Priority)
                .HasDefaultValue((short)5)
                .HasColumnName("priority");
            entity.Property(e => e.QueueName)
                .HasMaxLength(100)
                .HasDefaultValueSql("'default'::character varying")
                .HasColumnName("queue_name");
            entity.Property(e => e.RetiredAt).HasColumnName("retired_at");
            entity.Property(e => e.RetiredByUserId).HasColumnName("retired_by_user_id");
            entity.Property(e => e.RetirementReason)
                .HasMaxLength(500)
                .HasColumnName("retirement_reason");
            entity.Property(e => e.RetryBackoffMultiplier)
                .HasPrecision(6, 2)
                .HasDefaultValue(2.00m)
                .HasColumnName("retry_backoff_multiplier");
            entity.Property(e => e.RetryJitterEnabled)
                .HasDefaultValue(true)
                .HasColumnName("retry_jitter_enabled");
            entity.Property(e => e.RetryStrategy)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Exponential'::character varying")
                .HasColumnName("retry_strategy");
            entity.Property(e => e.RunTime).HasColumnName("run_time");
            entity.Property(e => e.ScheduleEndAt).HasColumnName("schedule_end_at");
            entity.Property(e => e.ScheduleStartAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("schedule_start_at");
            entity.Property(e => e.ScheduleType)
                .HasMaxLength(30)
                .HasColumnName("schedule_type");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TimeZone)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Africa/Tripoli'::character varying")
                .HasColumnName("time_zone");
            entity.Property(e => e.TimeoutSeconds)
                .HasDefaultValue(300)
                .HasColumnName("timeout_seconds");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedByUserId).HasColumnName("updated_by_user_id");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.Company).WithMany(p => p.ScheduledJobDefinitions)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_sched_jobs_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.ScheduledJobDefinitionAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ActivatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_sched_jobs_activated_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.ScheduledJobDefinitionAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_sched_jobs_created_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.ScheduledJobDefinitionAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.DisabledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_sched_jobs_disabled_by");

            entity.HasOne(d => d.BackgroundJob).WithMany(p => p.ScheduledJobDefinitions)
                .HasPrincipalKey(p => new { p.CompanyId, p.BackgroundJobId })
                .HasForeignKey(d => new { d.CompanyId, d.LastBackgroundJobId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_sched_jobs_last_background_job");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.ScheduledJobDefinitionAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.PausedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_sched_jobs_paused_by");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.ScheduledJobDefinitionAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RetiredByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_sched_jobs_retired_by");

            entity.HasOne(d => d.AppUser4).WithMany(p => p.ScheduledJobDefinitionAppUser4s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.UpdatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_sched_jobs_updated_by");
        });

        modelBuilder.Entity<ScheduledJobRun>(entity =>
        {
            entity.HasKey(e => e.ScheduledJobRunId).HasName("scheduled_job_runs_pkey");

            entity.ToTable("scheduled_job_runs", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_sched_runs_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.RunDecision, e.DecisionAt }, "ix_sched_runs_decision").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ScheduledJobDefinitionId, e.RunNumber }, "ix_sched_runs_definition").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CompanyId, e.ReplacedBackgroundJobId }, "ix_sched_runs_replaced_job").HasFilter("(replaced_background_job_id IS NOT NULL)");

            entity.HasIndex(e => e.ScheduledFor, "ix_sched_runs_scheduled_for").IsDescending();

            entity.HasIndex(e => new { e.SchedulerInstance, e.DecisionAt }, "ix_sched_runs_scheduler").IsDescending(false, true);

            entity.HasIndex(e => new { e.CompanyId, e.DecisionAt }, "ix_sched_runs_skipped")
                .IsDescending(false, true)
                .HasFilter("((run_decision)::text = ANY ((ARRAY['SkippedMisfire'::character varying, 'SkippedConcurrency'::character varying, 'SkippedRunLimit'::character varying, 'SkippedScheduleWindow'::character varying, 'SkippedInactive'::character varying])::text[]))");

            entity.HasIndex(e => new { e.CompanyId, e.ScheduledJobRunId }, "uq_sched_runs_company_run").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ScheduledJobDefinitionId, e.RunNumber }, "uq_sched_runs_definition_number").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ScheduledJobDefinitionId, e.ScheduledFor }, "uq_sched_runs_definition_time").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.IdempotencyKey }, "uq_sched_runs_idempotency").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.BackgroundJobId }, "ux_sched_runs_background_job")
                .IsUnique()
                .HasFilter("(background_job_id IS NOT NULL)");

            entity.Property(e => e.ScheduledJobRunId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("scheduled_job_run_id");
            entity.Property(e => e.BackgroundJobId).HasColumnName("background_job_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.ConcurrencyPolicySnapshot)
                .HasMaxLength(20)
                .HasColumnName("concurrency_policy_snapshot");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DecisionAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("decision_at");
            entity.Property(e => e.DecisionReason)
                .HasMaxLength(1000)
                .HasColumnName("decision_reason");
            entity.Property(e => e.DefinitionVersionNumber).HasColumnName("definition_version_number");
            entity.Property(e => e.DelayMilliseconds).HasColumnName("delay_milliseconds");
            entity.Property(e => e.DetectedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("detected_at");
            entity.Property(e => e.EnqueuedAt).HasColumnName("enqueued_at");
            entity.Property(e => e.IdempotencyKey)
                .HasMaxLength(200)
                .HasColumnName("idempotency_key");
            entity.Property(e => e.JobTypeSnapshot)
                .HasMaxLength(150)
                .HasColumnName("job_type_snapshot");
            entity.Property(e => e.MisfirePolicySnapshot)
                .HasMaxLength(30)
                .HasColumnName("misfire_policy_snapshot");
            entity.Property(e => e.PrioritySnapshot).HasColumnName("priority_snapshot");
            entity.Property(e => e.QueueNameSnapshot)
                .HasMaxLength(100)
                .HasColumnName("queue_name_snapshot");
            entity.Property(e => e.ReplacedBackgroundJobId).HasColumnName("replaced_background_job_id");
            entity.Property(e => e.RunDecision)
                .HasMaxLength(40)
                .HasColumnName("run_decision");
            entity.Property(e => e.RunNumber).HasColumnName("run_number");
            entity.Property(e => e.ScheduledFor).HasColumnName("scheduled_for");
            entity.Property(e => e.ScheduledJobDefinitionId).HasColumnName("scheduled_job_definition_id");
            entity.Property(e => e.SchedulerInstance)
                .HasMaxLength(200)
                .HasColumnName("scheduler_instance");

            entity.HasOne(d => d.BackgroundJob).WithOne(p => p.ScheduledJobRunBackgroundJob)
                .HasPrincipalKey<BackgroundJob>(p => new { p.CompanyId, p.BackgroundJobId })
                .HasForeignKey<ScheduledJobRun>(d => new { d.CompanyId, d.BackgroundJobId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_sched_runs_background_job");

            entity.HasOne(d => d.BackgroundJobNavigation).WithMany(p => p.ScheduledJobRunBackgroundJobNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.BackgroundJobId })
                .HasForeignKey(d => new { d.CompanyId, d.ReplacedBackgroundJobId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_sched_runs_replaced_job");

            entity.HasOne(d => d.ScheduledJobDefinition).WithMany(p => p.ScheduledJobRuns)
                .HasPrincipalKey(p => new { p.CompanyId, p.ScheduledJobDefinitionId })
                .HasForeignKey(d => new { d.CompanyId, d.ScheduledJobDefinitionId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_sched_runs_definition");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("suppliers_pkey");

            entity.ToTable("suppliers", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierName }, "ix_suppliers_active").HasFilter("(is_active = true)");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierName }, "ix_suppliers_name");

            entity.HasIndex(e => new { e.CompanyId, e.PhoneNumber }, "ix_suppliers_phone").HasFilter("(phone_number IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierType }, "ix_suppliers_type");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierId }, "uq_suppliers_company_supplier").IsUnique();

            entity.Property(e => e.SupplierId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("supplier_id");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.CommercialRegistrationNumber)
                .HasMaxLength(100)
                .HasColumnName("commercial_registration_number");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.ContactPersonName)
                .HasMaxLength(150)
                .HasColumnName("contact_person_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CreditLimit)
                .HasPrecision(18, 2)
                .HasColumnName("credit_limit");
            entity.Property(e => e.DeactivatedAt).HasColumnName("deactivated_at");
            entity.Property(e => e.DeactivatedByUserId).HasColumnName("deactivated_by_user_id");
            entity.Property(e => e.DeactivationReason)
                .HasMaxLength(500)
                .HasColumnName("deactivation_reason");
            entity.Property(e => e.DefaultCurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'LYD'::character varying")
                .HasColumnName("default_currency_code");
            entity.Property(e => e.DefaultPaymentTermsDays).HasColumnName("default_payment_terms_days");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.PreferredPaymentMethod)
                .HasMaxLength(30)
                .HasColumnName("preferred_payment_method");
            entity.Property(e => e.SecondaryPhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("secondary_phone_number");
            entity.Property(e => e.SupplierCode)
                .HasMaxLength(30)
                .HasColumnName("supplier_code");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(200)
                .HasColumnName("supplier_name");
            entity.Property(e => e.SupplierType)
                .HasMaxLength(40)
                .HasDefaultValueSql("'GeneralSupplier'::character varying")
                .HasColumnName("supplier_type");
            entity.Property(e => e.TaxRegistrationNumber)
                .HasMaxLength(100)
                .HasColumnName("tax_registration_number");
            entity.Property(e => e.TransactionMode)
                .HasMaxLength(30)
                .HasDefaultValueSql("'CashAndCredit'::character varying")
                .HasColumnName("transaction_mode");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.Company).WithMany(p => p.Suppliers)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_suppliers_company");

            entity.HasOne(d => d.AppUser).WithMany(p => p.SupplierAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_suppliers_created_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.SupplierAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.DeactivatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_suppliers_deactivated_by");
        });

        modelBuilder.Entity<SupplierCreditNote>(entity =>
        {
            entity.HasKey(e => e.SupplierCreditNoteId).HasName("supplier_credit_notes_pkey");

            entity.ToTable("supplier_credit_notes", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.CreditNoteDate }, "ix_supplier_credit_notes_date");

            entity.HasIndex(e => new { e.CompanyId, e.SubmittedAt }, "ix_supplier_credit_notes_pending").HasFilter("((status)::text = 'PendingApproval'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_supplier_credit_notes_status");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierId }, "ix_supplier_credit_notes_supplier");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierId, e.CreditNoteDate }, "ix_supplier_credit_notes_supplier_date");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierCreditNoteId }, "uq_supplier_credit_notes_company_note").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.CreditNoteNumber }, "uq_supplier_credit_notes_number").IsUnique();

            entity.Property(e => e.SupplierCreditNoteId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("supplier_credit_note_id");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedByUserId).HasColumnName("approved_by_user_id");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CreditNoteAmount)
                .HasPrecision(18, 2)
                .HasColumnName("credit_note_amount");
            entity.Property(e => e.CreditNoteDate).HasColumnName("credit_note_date");
            entity.Property(e => e.CreditNoteNumber)
                .HasMaxLength(50)
                .HasColumnName("credit_note_number");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'LYD'::character varying")
                .HasColumnName("currency_code");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.ProofFileUrl)
                .HasMaxLength(1000)
                .HasColumnName("proof_file_url");
            entity.Property(e => e.ReasonType)
                .HasMaxLength(40)
                .HasColumnName("reason_type");
            entity.Property(e => e.RejectedAt).HasColumnName("rejected_at");
            entity.Property(e => e.RejectedByUserId).HasColumnName("rejected_by_user_id");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SubmittedAt).HasColumnName("submitted_at");
            entity.Property(e => e.SubmittedByUserId).HasColumnName("submitted_by_user_id");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.SupplierReferenceNumber)
                .HasMaxLength(100)
                .HasColumnName("supplier_reference_number");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.SupplierCreditNoteAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ApprovedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_credit_notes_approved_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.SupplierCreditNoteAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_credit_notes_cancelled_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.SupplierCreditNoteAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_credit_notes_created_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.SupplierCreditNoteAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RejectedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_credit_notes_rejected_by");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.SupplierCreditNoteAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_credit_notes_reversed_by");

            entity.HasOne(d => d.AppUser4).WithMany(p => p.SupplierCreditNoteAppUser4s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.SubmittedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_credit_notes_submitted_by");

            entity.HasOne(d => d.Supplier).WithMany(p => p.SupplierCreditNotes)
                .HasPrincipalKey(p => new { p.CompanyId, p.SupplierId })
                .HasForeignKey(d => new { d.CompanyId, d.SupplierId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_credit_notes_supplier");
        });

        modelBuilder.Entity<SupplierCreditNoteAllocation>(entity =>
        {
            entity.HasKey(e => e.SupplierCreditNoteAllocationId).HasName("supplier_credit_note_allocations_pkey");

            entity.ToTable("supplier_credit_note_allocations", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.AllocatedByUserId }, "ix_supplier_credit_note_allocations_allocated_by");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierCreditNoteId }, "ix_supplier_credit_note_allocations_credit_note");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierDebtId }, "ix_supplier_credit_note_allocations_debt");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierDebtId, e.CreatedAt }, "ix_supplier_credit_note_allocations_debt_created");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierCreditNoteAllocationId }, "uq_supplier_credit_note_allocations_company_allocation").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.SupplierCreditNoteId, e.SupplierDebtId }, "uq_supplier_credit_note_allocations_note_debt").IsUnique();

            entity.Property(e => e.SupplierCreditNoteAllocationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("supplier_credit_note_allocation_id");
            entity.Property(e => e.AllocatedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("allocated_amount");
            entity.Property(e => e.AllocatedByUserId).HasColumnName("allocated_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DebtVersionNumber).HasColumnName("debt_version_number");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.SupplierCreditNoteId).HasColumnName("supplier_credit_note_id");
            entity.Property(e => e.SupplierDebtId).HasColumnName("supplier_debt_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AppUser).WithMany(p => p.SupplierCreditNoteAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.AllocatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_credit_note_allocations_allocated_by");

            entity.HasOne(d => d.SupplierCreditNote).WithMany(p => p.SupplierCreditNoteAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.SupplierCreditNoteId })
                .HasForeignKey(d => new { d.CompanyId, d.SupplierCreditNoteId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_credit_note_allocations_credit_note");

            entity.HasOne(d => d.SupplierDebt).WithMany(p => p.SupplierCreditNoteAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.SupplierDebtId })
                .HasForeignKey(d => new { d.CompanyId, d.SupplierDebtId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_credit_note_allocations_debt");
        });

        modelBuilder.Entity<SupplierDebt>(entity =>
        {
            entity.HasKey(e => e.SupplierDebtId).HasName("supplier_debts_pkey");

            entity.ToTable("supplier_debts", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.DebtDate }, "ix_supplier_debts_debt_date");

            entity.HasIndex(e => new { e.CompanyId, e.DueDate }, "ix_supplier_debts_due_date").HasFilter("((status)::text = ANY ((ARRAY['Open'::character varying, 'PartiallySettled'::character varying])::text[]))");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierId, e.OutstandingAmount }, "ix_supplier_debts_outstanding").HasFilter("((status)::text = ANY ((ARRAY['Open'::character varying, 'PartiallySettled'::character varying])::text[]))");

            entity.HasIndex(e => new { e.CompanyId, e.ProjectId }, "ix_supplier_debts_project").HasFilter("(project_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierId }, "ix_supplier_debts_supplier");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierId, e.Status }, "ix_supplier_debts_supplier_status");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierDebtId }, "uq_supplier_debts_company_debt").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseId }, "uq_supplier_debts_expense").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.DebtNumber }, "uq_supplier_debts_number").IsUnique();

            entity.Property(e => e.SupplierDebtId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("supplier_debt_id");
            entity.Property(e => e.AdjustmentAmount)
                .HasPrecision(18, 2)
                .HasColumnName("adjustment_amount");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreditNoteAmount)
                .HasPrecision(18, 2)
                .HasColumnName("credit_note_amount");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'LYD'::character varying")
                .HasColumnName("currency_code");
            entity.Property(e => e.DebtAmount)
                .HasPrecision(18, 2)
                .HasColumnName("debt_amount");
            entity.Property(e => e.DebtDate).HasColumnName("debt_date");
            entity.Property(e => e.DebtNumber)
                .HasMaxLength(50)
                .HasColumnName("debt_number");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.ExpenseId).HasColumnName("expense_id");
            entity.Property(e => e.ExpenseVersionNumber).HasColumnName("expense_version_number");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.OutstandingAmount)
                .HasPrecision(18, 2)
                .HasComputedColumnSql("((((debt_amount + adjustment_amount) - paid_amount) - credit_note_amount) - written_off_amount)", true)
                .HasColumnName("outstanding_amount");
            entity.Property(e => e.PaidAmount)
                .HasPrecision(18, 2)
                .HasColumnName("paid_amount");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.RecordedByUserId).HasColumnName("recorded_by_user_id");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.SettledAt).HasColumnName("settled_at");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Open'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");
            entity.Property(e => e.WrittenOffAmount)
                .HasPrecision(18, 2)
                .HasColumnName("written_off_amount");

            entity.HasOne(d => d.AppUser).WithMany(p => p.SupplierDebtAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debts_cancelled_by");

            entity.HasOne(d => d.Expense).WithOne(p => p.SupplierDebt)
                .HasPrincipalKey<Expense>(p => new { p.CompanyId, p.ExpenseId })
                .HasForeignKey<SupplierDebt>(d => new { d.CompanyId, d.ExpenseId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debts_expense");

            entity.HasOne(d => d.Project).WithMany(p => p.SupplierDebts)
                .HasPrincipalKey(p => new { p.CompanyId, p.ProjectId })
                .HasForeignKey(d => new { d.CompanyId, d.ProjectId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debts_project");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.SupplierDebtAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecordedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debts_recorded_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.SupplierDebtAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debts_reversed_by");

            entity.HasOne(d => d.Supplier).WithMany(p => p.SupplierDebts)
                .HasPrincipalKey(p => new { p.CompanyId, p.SupplierId })
                .HasForeignKey(d => new { d.CompanyId, d.SupplierId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debts_supplier");
        });

        modelBuilder.Entity<SupplierDebtAdjustment>(entity =>
        {
            entity.HasKey(e => e.SupplierDebtAdjustmentId).HasName("supplier_debt_adjustments_pkey");

            entity.ToTable("supplier_debt_adjustments", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.CreatedByUserId }, "ix_supplier_debt_adjustments_created_by");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierDebtId }, "ix_supplier_debt_adjustments_debt");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierDebtId, e.AdjustmentDate }, "ix_supplier_debt_adjustments_debt_date");

            entity.HasIndex(e => new { e.CompanyId, e.SubmittedAt }, "ix_supplier_debt_adjustments_pending").HasFilter("((status)::text = 'PendingApproval'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_supplier_debt_adjustments_status");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierDebtAdjustmentId }, "uq_supplier_debt_adjustments_company_adjustment").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.AdjustmentNumber }, "uq_supplier_debt_adjustments_number").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.SupplierDebtId }, "ux_supplier_debt_adjustments_one_pending")
                .IsUnique()
                .HasFilter("((status)::text = 'PendingApproval'::text)");

            entity.Property(e => e.SupplierDebtAdjustmentId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("supplier_debt_adjustment_id");
            entity.Property(e => e.AdjustmentAmount)
                .HasPrecision(18, 2)
                .HasColumnName("adjustment_amount");
            entity.Property(e => e.AdjustmentDate).HasColumnName("adjustment_date");
            entity.Property(e => e.AdjustmentNumber)
                .HasMaxLength(50)
                .HasColumnName("adjustment_number");
            entity.Property(e => e.AdjustmentType)
                .HasMaxLength(30)
                .HasColumnName("adjustment_type");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedByUserId).HasColumnName("approved_by_user_id");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.DebtVersionNumber).HasColumnName("debt_version_number");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.ProofFileUrl)
                .HasMaxLength(1000)
                .HasColumnName("proof_file_url");
            entity.Property(e => e.ReasonType)
                .HasMaxLength(40)
                .HasColumnName("reason_type");
            entity.Property(e => e.RejectedAt).HasColumnName("rejected_at");
            entity.Property(e => e.RejectedByUserId).HasColumnName("rejected_by_user_id");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SubmittedAt).HasColumnName("submitted_at");
            entity.Property(e => e.SubmittedByUserId).HasColumnName("submitted_by_user_id");
            entity.Property(e => e.SupplierDebtId).HasColumnName("supplier_debt_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.SupplierDebtAdjustmentAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ApprovedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debt_adjustments_approved_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.SupplierDebtAdjustmentAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debt_adjustments_cancelled_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.SupplierDebtAdjustmentAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debt_adjustments_created_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.SupplierDebtAdjustmentAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RejectedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debt_adjustments_rejected_by");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.SupplierDebtAdjustmentAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debt_adjustments_reversed_by");

            entity.HasOne(d => d.AppUser4).WithMany(p => p.SupplierDebtAdjustmentAppUser4s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.SubmittedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debt_adjustments_submitted_by");

            entity.HasOne(d => d.SupplierDebt).WithOne(p => p.SupplierDebtAdjustment)
                .HasPrincipalKey<SupplierDebt>(p => new { p.CompanyId, p.SupplierDebtId })
                .HasForeignKey<SupplierDebtAdjustment>(d => new { d.CompanyId, d.SupplierDebtId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debt_adjustments_debt");
        });

        modelBuilder.Entity<SupplierDebtLedgerEntry>(entity =>
        {
            entity.HasKey(e => e.SupplierDebtLedgerEntryId).HasName("supplier_debt_ledger_entries_pkey");

            entity.ToTable("supplier_debt_ledger_entries", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.CorrelationId }, "ix_supplier_debt_ledger_correlation");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierDebtId, e.DebtVersionNumber }, "ix_supplier_debt_ledger_debt");

            entity.HasIndex(e => new { e.CompanyId, e.EntryType }, "ix_supplier_debt_ledger_entry_type");

            entity.HasIndex(e => new { e.CompanyId, e.OccurredAt }, "ix_supplier_debt_ledger_occurred_at");

            entity.HasIndex(e => new { e.CompanyId, e.ReferenceType, e.ReferenceId }, "ix_supplier_debt_ledger_reference").HasFilter("(reference_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierDebtLedgerEntryId }, "uq_supplier_debt_ledger_company_entry").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.SupplierDebtId, e.DebtVersionNumber }, "uq_supplier_debt_ledger_debt_version").IsUnique();

            entity.Property(e => e.SupplierDebtLedgerEntryId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("supplier_debt_ledger_entry_id");
            entity.Property(e => e.AdjustmentAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("adjustment_after_amount");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CorrelationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreditNoteAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("credit_note_after_amount");
            entity.Property(e => e.DebtAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("debt_after_amount");
            entity.Property(e => e.DebtStatusAfter)
                .HasMaxLength(30)
                .HasColumnName("debt_status_after");
            entity.Property(e => e.DebtVersionNumber).HasColumnName("debt_version_number");
            entity.Property(e => e.DeltaAdjustmentAmount)
                .HasPrecision(18, 2)
                .HasColumnName("delta_adjustment_amount");
            entity.Property(e => e.DeltaCreditNoteAmount)
                .HasPrecision(18, 2)
                .HasColumnName("delta_credit_note_amount");
            entity.Property(e => e.DeltaDebtAmount)
                .HasPrecision(18, 2)
                .HasColumnName("delta_debt_amount");
            entity.Property(e => e.DeltaPaidAmount)
                .HasPrecision(18, 2)
                .HasColumnName("delta_paid_amount");
            entity.Property(e => e.DeltaWrittenOffAmount)
                .HasPrecision(18, 2)
                .HasColumnName("delta_written_off_amount");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.EntryType)
                .HasMaxLength(40)
                .HasColumnName("entry_type");
            entity.Property(e => e.OccurredAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("occurred_at");
            entity.Property(e => e.OutstandingAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("outstanding_after_amount");
            entity.Property(e => e.PaidAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("paid_after_amount");
            entity.Property(e => e.PerformedByUserId).HasColumnName("performed_by_user_id");
            entity.Property(e => e.ReferenceId).HasColumnName("reference_id");
            entity.Property(e => e.ReferenceType)
                .HasMaxLength(40)
                .HasColumnName("reference_type");
            entity.Property(e => e.SupplierDebtId).HasColumnName("supplier_debt_id");
            entity.Property(e => e.WrittenOffAfterAmount)
                .HasPrecision(18, 2)
                .HasColumnName("written_off_after_amount");

            entity.HasOne(d => d.AppUser).WithMany(p => p.SupplierDebtLedgerEntries)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.PerformedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debt_ledger_performed_by");

            entity.HasOne(d => d.SupplierDebt).WithMany(p => p.SupplierDebtLedgerEntries)
                .HasPrincipalKey(p => new { p.CompanyId, p.SupplierDebtId })
                .HasForeignKey(d => new { d.CompanyId, d.SupplierDebtId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debt_ledger_debt");
        });

        modelBuilder.Entity<SupplierDebtWriteOff>(entity =>
        {
            entity.HasKey(e => e.SupplierDebtWriteOffId).HasName("supplier_debt_write_offs_pkey");

            entity.ToTable("supplier_debt_write_offs", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.CreatedByUserId }, "ix_supplier_debt_write_offs_created_by");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierDebtId }, "ix_supplier_debt_write_offs_debt");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierDebtId, e.WriteOffDate }, "ix_supplier_debt_write_offs_debt_date");

            entity.HasIndex(e => new { e.CompanyId, e.SubmittedAt }, "ix_supplier_debt_write_offs_pending").HasFilter("((status)::text = 'PendingApproval'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_supplier_debt_write_offs_status");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierDebtWriteOffId }, "uq_supplier_debt_write_offs_company_write_off").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.WriteOffNumber }, "uq_supplier_debt_write_offs_number").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.SupplierDebtId }, "ux_supplier_debt_write_offs_one_pending")
                .IsUnique()
                .HasFilter("((status)::text = 'PendingApproval'::text)");

            entity.Property(e => e.SupplierDebtWriteOffId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("supplier_debt_write_off_id");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedByUserId).HasColumnName("approved_by_user_id");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.DebtVersionNumber).HasColumnName("debt_version_number");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.ProofFileUrl)
                .HasMaxLength(1000)
                .HasColumnName("proof_file_url");
            entity.Property(e => e.ReasonType)
                .HasMaxLength(40)
                .HasColumnName("reason_type");
            entity.Property(e => e.RejectedAt).HasColumnName("rejected_at");
            entity.Property(e => e.RejectedByUserId).HasColumnName("rejected_by_user_id");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SubmittedAt).HasColumnName("submitted_at");
            entity.Property(e => e.SubmittedByUserId).HasColumnName("submitted_by_user_id");
            entity.Property(e => e.SupplierDebtId).HasColumnName("supplier_debt_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");
            entity.Property(e => e.WriteOffAmount)
                .HasPrecision(18, 2)
                .HasColumnName("write_off_amount");
            entity.Property(e => e.WriteOffDate).HasColumnName("write_off_date");
            entity.Property(e => e.WriteOffNumber)
                .HasMaxLength(50)
                .HasColumnName("write_off_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.SupplierDebtWriteOffAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ApprovedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debt_write_offs_approved_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.SupplierDebtWriteOffAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debt_write_offs_cancelled_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.SupplierDebtWriteOffAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debt_write_offs_created_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.SupplierDebtWriteOffAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RejectedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debt_write_offs_rejected_by");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.SupplierDebtWriteOffAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debt_write_offs_reversed_by");

            entity.HasOne(d => d.AppUser4).WithMany(p => p.SupplierDebtWriteOffAppUser4s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.SubmittedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debt_write_offs_submitted_by");

            entity.HasOne(d => d.SupplierDebt).WithOne(p => p.SupplierDebtWriteOff)
                .HasPrincipalKey<SupplierDebt>(p => new { p.CompanyId, p.SupplierDebtId })
                .HasForeignKey<SupplierDebtWriteOff>(d => new { d.CompanyId, d.SupplierDebtId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_debt_write_offs_debt");
        });

        modelBuilder.Entity<SupplierPayment>(entity =>
        {
            entity.HasKey(e => e.SupplierPaymentId).HasName("supplier_payments_pkey");

            entity.ToTable("supplier_payments", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierPaymentAccountId }, "ix_supplier_payments_account").HasFilter("(supplier_payment_account_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.PaymentDate }, "ix_supplier_payments_payment_date");

            entity.HasIndex(e => new { e.CompanyId, e.SubmittedAt }, "ix_supplier_payments_pending").HasFilter("((status)::text = 'PendingApproval'::text)");

            entity.HasIndex(e => new { e.CompanyId, e.ReferenceNumber }, "ix_supplier_payments_reference").HasFilter("(reference_number IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_supplier_payments_status");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierId }, "ix_supplier_payments_supplier");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierId, e.PaymentDate }, "ix_supplier_payments_supplier_date");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierPaymentId }, "uq_supplier_payments_company_payment").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.PaymentNumber }, "uq_supplier_payments_number").IsUnique();

            entity.Property(e => e.SupplierPaymentId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("supplier_payment_id");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.ConfirmedAt).HasColumnName("confirmed_at");
            entity.Property(e => e.ConfirmedByUserId).HasColumnName("confirmed_by_user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'LYD'::character varying")
                .HasColumnName("currency_code");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.FeeAmount)
                .HasPrecision(18, 2)
                .HasColumnName("fee_amount");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.PayerBankName)
                .HasMaxLength(150)
                .HasColumnName("payer_bank_name");
            entity.Property(e => e.PaymentAmount)
                .HasPrecision(18, 2)
                .HasColumnName("payment_amount");
            entity.Property(e => e.PaymentDate).HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(30)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentNumber)
                .HasMaxLength(50)
                .HasColumnName("payment_number");
            entity.Property(e => e.ProofFileUrl)
                .HasMaxLength(1000)
                .HasColumnName("proof_file_url");
            entity.Property(e => e.ReferenceNumber)
                .HasMaxLength(150)
                .HasColumnName("reference_number");
            entity.Property(e => e.RejectedAt).HasColumnName("rejected_at");
            entity.Property(e => e.RejectedByUserId).HasColumnName("rejected_by_user_id");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SubmittedAt).HasColumnName("submitted_at");
            entity.Property(e => e.SubmittedByUserId).HasColumnName("submitted_by_user_id");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.SupplierPaymentAccountId).HasColumnName("supplier_payment_account_id");
            entity.Property(e => e.TotalDisbursedAmount)
                .HasPrecision(18, 2)
                .HasComputedColumnSql("(payment_amount + fee_amount)", true)
                .HasColumnName("total_disbursed_amount");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.SupplierPaymentAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_payments_cancelled_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.SupplierPaymentAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ConfirmedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_payments_confirmed_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.SupplierPaymentAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_payments_created_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.SupplierPaymentAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RejectedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_payments_rejected_by");

            entity.HasOne(d => d.AppUser3).WithMany(p => p.SupplierPaymentAppUser3s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_payments_reversed_by");

            entity.HasOne(d => d.AppUser4).WithMany(p => p.SupplierPaymentAppUser4s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.SubmittedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_payments_submitted_by");

            entity.HasOne(d => d.Supplier).WithMany(p => p.SupplierPayments)
                .HasPrincipalKey(p => new { p.CompanyId, p.SupplierId })
                .HasForeignKey(d => new { d.CompanyId, d.SupplierId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_payments_supplier");

            entity.HasOne(d => d.SupplierPaymentAccount).WithMany(p => p.SupplierPayments)
                .HasPrincipalKey(p => new { p.CompanyId, p.SupplierPaymentAccountId })
                .HasForeignKey(d => new { d.CompanyId, d.SupplierPaymentAccountId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_payments_account");
        });

        modelBuilder.Entity<SupplierPaymentAccount>(entity =>
        {
            entity.HasKey(e => e.SupplierPaymentAccountId).HasName("supplier_payment_accounts_pkey");

            entity.ToTable("supplier_payment_accounts", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierId, e.AccountLabel }, "ix_supplier_payment_accounts_active").HasFilter("(is_active = true)");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierId }, "ix_supplier_payment_accounts_supplier");

            entity.HasIndex(e => new { e.CompanyId, e.AccountType }, "ix_supplier_payment_accounts_type");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierId, e.VerificationStatus }, "ix_supplier_payment_accounts_verified");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierPaymentAccountId }, "uq_supplier_payment_accounts_company_account").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.SupplierId, e.AccountNumber }, "ux_supplier_payment_accounts_bank_number")
                .IsUnique()
                .HasFilter("(account_number IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierId, e.Iban }, "ux_supplier_payment_accounts_iban")
                .IsUnique()
                .HasFilter("(iban IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierId, e.CurrencyCode }, "ux_supplier_payment_accounts_one_default")
                .IsUnique()
                .HasFilter("((is_default = true) AND (is_active = true))");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierId, e.WalletProvider, e.WalletNumber }, "ux_supplier_payment_accounts_wallet")
                .IsUnique()
                .HasFilter("(wallet_number IS NOT NULL)");

            entity.Property(e => e.SupplierPaymentAccountId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("supplier_payment_account_id");
            entity.Property(e => e.AccountHolderName)
                .HasMaxLength(200)
                .HasColumnName("account_holder_name");
            entity.Property(e => e.AccountLabel)
                .HasMaxLength(150)
                .HasColumnName("account_label");
            entity.Property(e => e.AccountNumber)
                .HasMaxLength(100)
                .HasColumnName("account_number");
            entity.Property(e => e.AccountType)
                .HasMaxLength(30)
                .HasColumnName("account_type");
            entity.Property(e => e.BankBranchName)
                .HasMaxLength(150)
                .HasColumnName("bank_branch_name");
            entity.Property(e => e.BankName)
                .HasMaxLength(150)
                .HasColumnName("bank_name");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'LYD'::character varying")
                .HasColumnName("currency_code");
            entity.Property(e => e.DeactivatedAt).HasColumnName("deactivated_at");
            entity.Property(e => e.DeactivatedByUserId).HasColumnName("deactivated_by_user_id");
            entity.Property(e => e.DeactivationReason)
                .HasMaxLength(500)
                .HasColumnName("deactivation_reason");
            entity.Property(e => e.Iban)
                .HasMaxLength(34)
                .HasColumnName("iban");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDefault).HasColumnName("is_default");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VerificationStatus)
                .HasMaxLength(30)
                .HasDefaultValueSql("'PendingVerification'::character varying")
                .HasColumnName("verification_status");
            entity.Property(e => e.VerifiedAt).HasColumnName("verified_at");
            entity.Property(e => e.VerifiedByUserId).HasColumnName("verified_by_user_id");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");
            entity.Property(e => e.WalletNumber)
                .HasMaxLength(20)
                .HasColumnName("wallet_number");
            entity.Property(e => e.WalletProvider)
                .HasMaxLength(100)
                .HasColumnName("wallet_provider");

            entity.HasOne(d => d.AppUser).WithMany(p => p.SupplierPaymentAccountAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_payment_accounts_created_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.SupplierPaymentAccountAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.DeactivatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_payment_accounts_deactivated_by");

            entity.HasOne(d => d.Supplier).WithMany(p => p.SupplierPaymentAccounts)
                .HasPrincipalKey(p => new { p.CompanyId, p.SupplierId })
                .HasForeignKey(d => new { d.CompanyId, d.SupplierId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_payment_accounts_supplier");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.SupplierPaymentAccountAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.VerifiedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_payment_accounts_verified_by");
        });

        modelBuilder.Entity<SupplierPaymentDebtAllocation>(entity =>
        {
            entity.HasKey(e => e.SupplierPaymentDebtAllocationId).HasName("supplier_payment_debt_allocations_pkey");

            entity.ToTable("supplier_payment_debt_allocations", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.AllocatedByUserId }, "ix_supplier_payment_debt_allocations_allocated_by");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierDebtId }, "ix_supplier_payment_debt_allocations_debt");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierDebtId, e.CreatedAt }, "ix_supplier_payment_debt_allocations_debt_created");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierPaymentId }, "ix_supplier_payment_debt_allocations_payment");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierPaymentDebtAllocationId }, "uq_supplier_payment_debt_allocations_company_allocation").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.SupplierPaymentId, e.SupplierDebtId }, "uq_supplier_payment_debt_allocations_payment_debt").IsUnique();

            entity.Property(e => e.SupplierPaymentDebtAllocationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("supplier_payment_debt_allocation_id");
            entity.Property(e => e.AllocatedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("allocated_amount");
            entity.Property(e => e.AllocatedByUserId).HasColumnName("allocated_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DebtVersionNumber).HasColumnName("debt_version_number");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.SupplierDebtId).HasColumnName("supplier_debt_id");
            entity.Property(e => e.SupplierPaymentId).HasColumnName("supplier_payment_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AppUser).WithMany(p => p.SupplierPaymentDebtAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.AllocatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_payment_debt_allocations_allocated_by");

            entity.HasOne(d => d.SupplierDebt).WithMany(p => p.SupplierPaymentDebtAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.SupplierDebtId })
                .HasForeignKey(d => new { d.CompanyId, d.SupplierDebtId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_payment_debt_allocations_debt");

            entity.HasOne(d => d.SupplierPayment).WithMany(p => p.SupplierPaymentDebtAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.SupplierPaymentId })
                .HasForeignKey(d => new { d.CompanyId, d.SupplierPaymentId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_payment_debt_allocations_payment");
        });

        modelBuilder.Entity<SupplierPaymentFundingSource>(entity =>
        {
            entity.HasKey(e => e.SupplierPaymentFundingSourceId).HasName("supplier_payment_funding_sources_pkey");

            entity.ToTable("supplier_payment_funding_sources", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.AllocatedByUserId }, "ix_supplier_payment_funding_sources_allocated_by");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierPaymentId }, "ix_supplier_payment_funding_sources_payment");

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourceId }, "ix_supplier_payment_funding_sources_source");

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourceId, e.CreatedAt }, "ix_supplier_payment_funding_sources_source_created");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierPaymentFundingSourceId }, "uq_supplier_payment_funding_sources_company_allocation").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.SupplierPaymentId, e.FundingSourceId }, "uq_supplier_payment_funding_sources_payment_source").IsUnique();

            entity.Property(e => e.SupplierPaymentFundingSourceId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("supplier_payment_funding_source_id");
            entity.Property(e => e.AllocatedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("allocated_amount");
            entity.Property(e => e.AllocatedByUserId).HasColumnName("allocated_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.FundingSourceId).HasColumnName("funding_source_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.SupplierPaymentId).HasColumnName("supplier_payment_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AppUser).WithMany(p => p.SupplierPaymentFundingSources)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.AllocatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_payment_funding_sources_allocated_by");

            entity.HasOne(d => d.FundingSource).WithMany(p => p.SupplierPaymentFundingSources)
                .HasPrincipalKey(p => new { p.CompanyId, p.FundingSourceId })
                .HasForeignKey(d => new { d.CompanyId, d.FundingSourceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_payment_funding_sources_source");

            entity.HasOne(d => d.SupplierPayment).WithMany(p => p.SupplierPaymentFundingSources)
                .HasPrincipalKey(p => new { p.CompanyId, p.SupplierPaymentId })
                .HasForeignKey(d => new { d.CompanyId, d.SupplierPaymentId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_payment_funding_sources_payment");
        });

        modelBuilder.Entity<SupplierRefund>(entity =>
        {
            entity.HasKey(e => e.SupplierRefundId).HasName("supplier_refunds_pkey");

            entity.ToTable("supplier_refunds", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.ExpenseReturnId }, "ix_supplier_refunds_expense_return");

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourceId }, "ix_supplier_refunds_funding_source");

            entity.HasIndex(e => new { e.CompanyId, e.RefundDate }, "ix_supplier_refunds_refund_date");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_supplier_refunds_status");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierId }, "ix_supplier_refunds_supplier");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierId, e.RefundDate }, "ix_supplier_refunds_supplier_date");

            entity.HasIndex(e => new { e.CompanyId, e.TransactionReferenceNumber }, "ix_supplier_refunds_transaction_reference").HasFilter("(transaction_reference_number IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.SupplierRefundId }, "uq_supplier_refunds_company_refund").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.FundingSourceId }, "uq_supplier_refunds_funding_source").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.RefundNumber }, "uq_supplier_refunds_number").IsUnique();

            entity.Property(e => e.SupplierRefundId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("supplier_refund_id");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'LYD'::character varying")
                .HasColumnName("currency_code");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.ExpenseReturnId).HasColumnName("expense_return_id");
            entity.Property(e => e.FeeAmount)
                .HasPrecision(18, 2)
                .HasColumnName("fee_amount");
            entity.Property(e => e.FundingSourceId).HasColumnName("funding_source_id");
            entity.Property(e => e.NetReceivedAmount)
                .HasPrecision(18, 2)
                .HasComputedColumnSql("(refund_amount - fee_amount)", true)
                .HasColumnName("net_received_amount");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.ProofFileUrl)
                .HasMaxLength(1000)
                .HasColumnName("proof_file_url");
            entity.Property(e => e.RecordedByUserId).HasColumnName("recorded_by_user_id");
            entity.Property(e => e.RefundAmount)
                .HasPrecision(18, 2)
                .HasColumnName("refund_amount");
            entity.Property(e => e.RefundDate).HasColumnName("refund_date");
            entity.Property(e => e.RefundMethod)
                .HasMaxLength(30)
                .HasColumnName("refund_method");
            entity.Property(e => e.RefundNumber)
                .HasMaxLength(50)
                .HasColumnName("refund_number");
            entity.Property(e => e.ReversalReason)
                .HasMaxLength(500)
                .HasColumnName("reversal_reason");
            entity.Property(e => e.ReversedAt).HasColumnName("reversed_at");
            entity.Property(e => e.ReversedByUserId).HasColumnName("reversed_by_user_id");
            entity.Property(e => e.SenderBankName)
                .HasMaxLength(150)
                .HasColumnName("sender_bank_name");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'PendingVerification'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.SupplierReferenceNumber)
                .HasMaxLength(150)
                .HasColumnName("supplier_reference_number");
            entity.Property(e => e.TransactionReferenceNumber)
                .HasMaxLength(150)
                .HasColumnName("transaction_reference_number");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.VerifiedAt).HasColumnName("verified_at");
            entity.Property(e => e.VerifiedByUserId).HasColumnName("verified_by_user_id");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.AppUser).WithMany(p => p.SupplierRefundAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_refunds_cancelled_by");

            entity.HasOne(d => d.ExpenseReturn).WithMany(p => p.SupplierRefunds)
                .HasPrincipalKey(p => new { p.CompanyId, p.ExpenseReturnId })
                .HasForeignKey(d => new { d.CompanyId, d.ExpenseReturnId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_refunds_expense_return");

            entity.HasOne(d => d.FundingSource).WithOne(p => p.SupplierRefund)
                .HasPrincipalKey<FundingSource>(p => new { p.CompanyId, p.FundingSourceId })
                .HasForeignKey<SupplierRefund>(d => new { d.CompanyId, d.FundingSourceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_refunds_funding_source");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.SupplierRefundAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RecordedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_refunds_recorded_by");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.SupplierRefundAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReversedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_refunds_reversed_by");

            entity.HasOne(d => d.Supplier).WithMany(p => p.SupplierRefunds)
                .HasPrincipalKey(p => new { p.CompanyId, p.SupplierId })
                .HasForeignKey(d => new { d.CompanyId, d.SupplierId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_refunds_supplier");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.SupplierRefundAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.VerifiedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_supplier_refunds_verified_by");
        });

        modelBuilder.Entity<TransferAdvanceAllocation>(entity =>
        {
            entity.HasKey(e => e.TransferAdvanceAllocationId).HasName("transfer_advance_allocations_pkey");

            entity.ToTable("transfer_advance_allocations", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceId }, "ix_transfer_advance_allocations_advance");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceId, e.CreatedAt }, "ix_transfer_advance_allocations_advance_created");

            entity.HasIndex(e => new { e.CompanyId, e.AllocatedByUserId }, "ix_transfer_advance_allocations_allocated_by");

            entity.HasIndex(e => new { e.CompanyId, e.MoneyTransferId }, "ix_transfer_advance_allocations_transfer");

            entity.HasIndex(e => new { e.CompanyId, e.TransferAdvanceAllocationId }, "uq_transfer_advance_allocations_company_allocation").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.MoneyTransferId, e.AdvanceId }, "uq_transfer_advance_allocations_transfer_advance").IsUnique();

            entity.Property(e => e.TransferAdvanceAllocationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("transfer_advance_allocation_id");
            entity.Property(e => e.AdvanceId).HasColumnName("advance_id");
            entity.Property(e => e.AllocatedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("allocated_amount");
            entity.Property(e => e.AllocatedByUserId).HasColumnName("allocated_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.MoneyTransferId).HasColumnName("money_transfer_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Advance).WithMany(p => p.TransferAdvanceAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.AdvanceId })
                .HasForeignKey(d => new { d.CompanyId, d.AdvanceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_transfer_advance_allocations_advance");

            entity.HasOne(d => d.AppUser).WithMany(p => p.TransferAdvanceAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.AllocatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_transfer_advance_allocations_allocated_by");

            entity.HasOne(d => d.MoneyTransfer).WithMany(p => p.TransferAdvanceAllocations)
                .HasPrincipalKey(p => new { p.CompanyId, p.MoneyTransferId })
                .HasForeignKey(d => new { d.CompanyId, d.MoneyTransferId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_transfer_advance_allocations_transfer");
        });

        modelBuilder.Entity<TransferCorrectionRequest>(entity =>
        {
            entity.HasKey(e => e.TransferCorrectionRequestId).HasName("transfer_correction_requests_pkey");

            entity.ToTable("transfer_correction_requests", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.RequestedByUserId }, "ix_transfer_correction_requests_requested_by");

            entity.HasIndex(e => new { e.CompanyId, e.ReviewedByUserId }, "ix_transfer_correction_requests_reviewed_by").HasFilter("(reviewed_by_user_id IS NOT NULL)");

            entity.HasIndex(e => new { e.CompanyId, e.Status, e.CreatedAt }, "ix_transfer_correction_requests_status");

            entity.HasIndex(e => new { e.CompanyId, e.MoneyTransferId }, "ix_transfer_correction_requests_transfer");

            entity.HasIndex(e => new { e.CompanyId, e.TransferCorrectionRequestId }, "uq_transfer_correction_requests_company_request").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.MoneyTransferId }, "ux_transfer_correction_requests_one_active")
                .IsUnique()
                .HasFilter("((status)::text = ANY ((ARRAY['PendingReview'::character varying, 'Approved'::character varying])::text[]))");

            entity.Property(e => e.TransferCorrectionRequestId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("transfer_correction_request_id");
            entity.Property(e => e.AppliedAt).HasColumnName("applied_at");
            entity.Property(e => e.AppliedByUserId).HasColumnName("applied_by_user_id");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CorrectionType)
                .HasMaxLength(40)
                .HasColumnName("correction_type");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.MoneyTransferId).HasColumnName("money_transfer_id");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.RequestReason)
                .HasMaxLength(1000)
                .HasColumnName("request_reason");
            entity.Property(e => e.RequestedByUserId).HasColumnName("requested_by_user_id");
            entity.Property(e => e.RequestedChanges)
                .HasColumnType("jsonb")
                .HasColumnName("requested_changes");
            entity.Property(e => e.ReviewNotes)
                .HasMaxLength(1000)
                .HasColumnName("review_notes");
            entity.Property(e => e.ReviewedAt).HasColumnName("reviewed_at");
            entity.Property(e => e.ReviewedByUserId).HasColumnName("reviewed_by_user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'PendingReview'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TransferVersionNumber).HasColumnName("transfer_version_number");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AppUser).WithMany(p => p.TransferCorrectionRequestAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.AppliedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_transfer_correction_requests_applied_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.TransferCorrectionRequestAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CancelledByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_transfer_correction_requests_cancelled_by");

            entity.HasOne(d => d.MoneyTransfer).WithOne(p => p.TransferCorrectionRequest)
                .HasPrincipalKey<MoneyTransfer>(p => new { p.CompanyId, p.MoneyTransferId })
                .HasForeignKey<TransferCorrectionRequest>(d => new { d.CompanyId, d.MoneyTransferId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_transfer_correction_requests_transfer");

            entity.HasOne(d => d.AppUser1).WithMany(p => p.TransferCorrectionRequestAppUser1s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.RequestedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_transfer_correction_requests_requested_by");

            entity.HasOne(d => d.AppUser2).WithMany(p => p.TransferCorrectionRequestAppUser2s)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.ReviewedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_transfer_correction_requests_reviewed_by");
        });

        modelBuilder.Entity<UserAdvanceBalance>(entity =>
        {
            entity.HasKey(e => e.UserAdvanceBalanceId).HasName("user_advance_balances_pkey");

            entity.ToTable("user_advance_balances", "ahdah");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceId }, "ix_user_advance_balances_advance");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceId, e.Status }, "ix_user_advance_balances_advance_status");

            entity.HasIndex(e => new { e.CompanyId, e.UserId, e.AvailableAmount }, "ix_user_advance_balances_available").HasFilter("((status)::text = ANY ((ARRAY['Active'::character varying, 'InSettlement'::character varying])::text[]))");

            entity.HasIndex(e => new { e.CompanyId, e.UserId }, "ix_user_advance_balances_user");

            entity.HasIndex(e => new { e.CompanyId, e.UserId, e.Status }, "ix_user_advance_balances_user_status");

            entity.HasIndex(e => new { e.CompanyId, e.AdvanceId, e.UserId }, "uq_user_advance_balances_advance_user").IsUnique();

            entity.HasIndex(e => new { e.CompanyId, e.UserAdvanceBalanceId }, "uq_user_advance_balances_company_balance").IsUnique();

            entity.Property(e => e.UserAdvanceBalanceId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("user_advance_balance_id");
            entity.Property(e => e.AdvanceId).HasColumnName("advance_id");
            entity.Property(e => e.AvailableAmount)
                .HasPrecision(18, 2)
                .HasColumnName("available_amount");
            entity.Property(e => e.ClosedAt).HasColumnName("closed_at");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.ReservedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("reserved_amount");
            entity.Property(e => e.SettledAt).HasColumnName("settled_at");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Active'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TotalAdjustmentOutAmount)
                .HasPrecision(18, 2)
                .HasColumnName("total_adjustment_out_amount");
            entity.Property(e => e.TotalExpensedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("total_expensed_amount");
            entity.Property(e => e.TotalReceivedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("total_received_amount");
            entity.Property(e => e.TotalRestoredAmount)
                .HasPrecision(18, 2)
                .HasColumnName("total_restored_amount");
            entity.Property(e => e.TotalReturnedAmount)
                .HasPrecision(18, 2)
                .HasColumnName("total_returned_amount");
            entity.Property(e => e.TotalTransferredOutAmount)
                .HasPrecision(18, 2)
                .HasColumnName("total_transferred_out_amount");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VersionNumber)
                .HasDefaultValue(1)
                .HasColumnName("version_number");

            entity.HasOne(d => d.Advance).WithMany(p => p.UserAdvanceBalances)
                .HasPrincipalKey(p => new { p.CompanyId, p.AdvanceId })
                .HasForeignKey(d => new { d.CompanyId, d.AdvanceId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_user_advance_balances_advance");

            entity.HasOne(d => d.AppUser).WithMany(p => p.UserAdvanceBalanceAppUsers)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.CreatedByUserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_user_advance_balances_created_by");

            entity.HasOne(d => d.AppUserNavigation).WithMany(p => p.UserAdvanceBalanceAppUserNavigations)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.UserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_user_advance_balances_user");
        });

        modelBuilder.Entity<UserDevice>(entity =>
        {
            entity.HasKey(e => e.DeviceId).HasName("user_devices_pkey");

            entity.ToTable("user_devices", "ahdah");

            entity.HasIndex(e => e.CompanyId, "ix_user_devices_company");

            entity.HasIndex(e => e.LastSeenAt, "ix_user_devices_last_seen");

            entity.HasIndex(e => new { e.CompanyId, e.UserId }, "ix_user_devices_user");

            entity.HasIndex(e => new { e.CompanyId, e.UserId, e.IsActive }, "ix_user_devices_user_active");

            entity.HasIndex(e => new { e.CompanyId, e.UserId, e.DeviceIdentifier }, "uq_user_devices_company_device").IsUnique();

            entity.HasIndex(e => e.PushToken, "ux_user_devices_active_push_token")
                .IsUnique()
                .HasFilter("((push_token IS NOT NULL) AND (is_active = true))");

            entity.Property(e => e.DeviceId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("device_id");
            entity.Property(e => e.AppVersion)
                .HasMaxLength(50)
                .HasColumnName("app_version");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeactivatedAt).HasColumnName("deactivated_at");
            entity.Property(e => e.DeviceIdentifier)
                .HasMaxLength(200)
                .HasColumnName("device_identifier");
            entity.Property(e => e.DeviceName)
                .HasMaxLength(150)
                .HasColumnName("device_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsTrusted).HasColumnName("is_trusted");
            entity.Property(e => e.LastSeenAt).HasColumnName("last_seen_at");
            entity.Property(e => e.OperatingSystemVersion)
                .HasMaxLength(50)
                .HasColumnName("operating_system_version");
            entity.Property(e => e.Platform)
                .HasMaxLength(20)
                .HasColumnName("platform");
            entity.Property(e => e.PushToken).HasColumnName("push_token");
            entity.Property(e => e.RegisteredAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("registered_at");
            entity.Property(e => e.TrustedAt).HasColumnName("trusted_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.AppUser).WithMany(p => p.UserDevices)
                .HasPrincipalKey(p => new { p.CompanyId, p.UserId })
                .HasForeignKey(d => new { d.CompanyId, d.UserId })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_user_devices_user");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
