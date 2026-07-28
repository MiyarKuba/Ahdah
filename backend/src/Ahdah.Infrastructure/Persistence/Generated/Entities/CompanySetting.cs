using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class CompanySetting
{
    public Guid CompanySettingId { get; set; }

    public Guid CompanyId { get; set; }

    public string DefaultCurrencyCode { get; set; } = null!;

    public string DefaultLocaleCode { get; set; } = null!;

    public string TimeZone { get; set; } = null!;

    public short FiscalYearStartMonth { get; set; }

    public short FirstDayOfWeek { get; set; }

    public short MoneyDecimalPlaces { get; set; }

    public short QuantityDecimalPlaces { get; set; }

    public string RoundingMode { get; set; } = null!;

    public decimal BalanceToleranceAmount { get; set; }

    public decimal DeMinimisWriteOffLimit { get; set; }

    public bool AllowMultiCurrency { get; set; }

    public bool AllowNegativeAdvanceBalance { get; set; }

    public bool AllowFinancialOverallocation { get; set; }

    public string AdvanceApprovalMode { get; set; } = null!;

    public decimal? AdvanceApprovalThresholdAmount { get; set; }

    public string ExpenseApprovalMode { get; set; } = null!;

    public decimal? ExpenseApprovalThresholdAmount { get; set; }

    public string TransferApprovalMode { get; set; } = null!;

    public decimal? TransferApprovalThresholdAmount { get; set; }

    public string SupplierPaymentApprovalMode { get; set; } = null!;

    public decimal? SupplierPaymentApprovalThresholdAmount { get; set; }

    public string PersonalClaimPaymentApprovalMode { get; set; } = null!;

    public decimal? PersonalClaimPaymentApprovalThresholdAmount { get; set; }

    public bool SettlementRequiresApproval { get; set; }

    public bool ClosureRequiresApproval { get; set; }

    public bool AllowSelfApproval { get; set; }

    public bool RequireDistinctCreatorApprover { get; set; }

    public bool RequireDistinctSubmitterApprover { get; set; }

    public string ExpenseDocumentMode { get; set; } = null!;

    public decimal? ExpenseDocumentThresholdAmount { get; set; }

    public string PaymentProofMode { get; set; } = null!;

    public decimal? PaymentProofThresholdAmount { get; set; }

    public long MaxDocumentSizeBytes { get; set; }

    public string AllowedMimeTypes { get; set; } = null!;

    public string AllowedFileExtensions { get; set; } = null!;

    public decimal? MaxSingleAdvanceAmount { get; set; }

    public decimal? MaxSingleExpenseAmount { get; set; }

    public decimal? MaxSingleTransferAmount { get; set; }

    public decimal? MaxSingleSupplierPaymentAmount { get; set; }

    public decimal? MaxSinglePersonalClaimPaymentAmount { get; set; }

    public int DefaultAdvanceDueDays { get; set; }

    public int DefaultSupplierDebtDueDays { get; set; }

    public int DefaultPersonalClaimDueDays { get; set; }

    public int InvitationExpiryHours { get; set; }

    public int JoinRequestExpiryDays { get; set; }

    public int SessionTimeoutMinutes { get; set; }

    public int MaximumFailedLoginAttempts { get; set; }

    public int AccountLockoutMinutes { get; set; }

    public bool RequireMfaForManager { get; set; }

    public bool RequireMfaForDeputy { get; set; }

    public bool RequireMfaForAccountant { get; set; }

    public int AuditLogRetentionDays { get; set; }

    public int NotificationRetentionDays { get; set; }

    public int OutboxRetentionDays { get; set; }

    public int BackgroundJobRetentionDays { get; set; }

    public int IdempotencyRetentionHours { get; set; }

    public string DefaultNotificationChannels { get; set; } = null!;

    public string AdditionalSettings { get; set; } = null!;

    public bool IsActive { get; set; }

    public Guid CreatedByUserId { get; set; }

    public Guid UpdatedByUserId { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;
}
