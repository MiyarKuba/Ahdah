using Ahdah.Application.Access.Models;

namespace Ahdah.Application.Suppliers.Models;

public sealed record SupplierUserSummary(Guid UserId, string FullName, string Role);
public sealed record SupplierProjectSummary(Guid ProjectId, string ProjectName);
public sealed record SupplierBalanceSummary(string CurrencyCode, decimal OutstandingAmount, int OpenDebtCount);

public sealed record SupplierSummary(
    Guid SupplierId, string? SupplierCode, string SupplierName, string SupplierType,
    string? ContactPersonName, string? PhoneNumber, string? Email, string? City,
    string DefaultCurrencyCode, string TransactionMode, int DefaultPaymentTermsDays,
    decimal? CreditLimit, string? PreferredPaymentMethod, bool IsActive, int VersionNumber,
    DateTimeOffset CreatedAtUtc, DateTimeOffset UpdatedAtUtc);

public sealed record SupplierDetails(
    SupplierSummary Supplier, string? SecondaryPhoneNumber, string? CommercialRegistrationNumber,
    string? TaxRegistrationNumber, string? Address, string? Notes,
    IReadOnlyList<SupplierBalanceSummary> Balances);

public sealed record SupplierPaymentAccountSummary(
    Guid SupplierPaymentAccountId, string AccountType, string AccountLabel,
    string? AccountHolderName, string? BankName, string? BankBranchName,
    string? MaskedAccountNumber, string? MaskedIban, string? WalletProvider,
    string? MaskedWalletNumber, string CurrencyCode, bool IsDefault,
    string VerificationStatus, bool IsActive, string? Notes, int VersionNumber,
    DateTimeOffset CreatedAtUtc);

public sealed record SupplierInvoiceItemSummary(
    Guid ExpenseItemId, int LineNumber, string ItemName, string? ItemCode,
    string? ItemDescription, decimal Quantity, string UnitCode, string? CustomUnitName,
    decimal UnitPrice, decimal SubtotalAmount, decimal DiscountAmount, decimal TaxAmount,
    decimal TotalAmount, string? Notes);

public sealed record SupplierInvoiceSummary(
    Guid SupplierDebtId, string DebtNumber, Guid ExpenseId, string ExpenseNumber,
    string? InvoiceNumber, SupplierSummary Supplier, SupplierProjectSummary? Project,
    DateOnly InvoiceDate, DateOnly DueDate, decimal Amount, decimal PaidAmount,
    decimal CreditNoteAmount, decimal WrittenOffAmount, decimal OutstandingAmount,
    string CurrencyCode, string ExpenseStatus, string DebtStatus, string Description,
    int ExpenseVersionNumber, int DebtVersionNumber, DateTimeOffset CreatedAtUtc);

public sealed record SupplierInvoiceDetails(
    SupplierInvoiceSummary Invoice, decimal AdjustmentAmount, string? Notes,
    IReadOnlyList<SupplierInvoiceItemSummary> Items);

public sealed record SupplierPaymentDebtAllocationSummary(
    Guid SupplierPaymentDebtAllocationId, Guid SupplierDebtId, string DebtNumber,
    decimal AllocatedAmount, DateTimeOffset CreatedAtUtc);

public sealed record SupplierPaymentFundingSummary(
    Guid SupplierPaymentFundingSourceId, Guid FundingSourceId, string SourceType,
    decimal AllocatedAmount, DateTimeOffset CreatedAtUtc);

public sealed record SupplierFundingSourceSummary(
    Guid FundingSourceId, string SourceType, DateOnly SourceDate,
    string CurrencyCode, decimal AvailableAmount, string Status,
    IReadOnlyList<string> PaymentMethods);

public sealed record SupplierPaymentSummary(
    Guid SupplierPaymentId, string PaymentNumber, Guid SupplierId, string SupplierName,
    DateOnly PaymentDate, decimal PaymentAmount, string CurrencyCode, string PaymentMethod,
    string? ReferenceNumber, bool HasProof, string Status, SupplierUserSummary CreatedBy,
    SupplierUserSummary? ConfirmedBy, int VersionNumber, DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ConfirmedAtUtc);

public sealed record SupplierPaymentDetails(
    SupplierPaymentSummary Payment, Guid? SupplierPaymentAccountId,
    string? PayerBankName, string? Description, string? Notes,
    IReadOnlyList<SupplierPaymentDebtAllocationSummary> DebtAllocations,
    IReadOnlyList<SupplierPaymentFundingSummary> FundingSources);

public sealed record SupplierRefundSummary(
    Guid SupplierRefundId, string RefundNumber, Guid SupplierId, string SupplierName,
    Guid ExpenseReturnId, DateOnly RefundDate, decimal RefundAmount, decimal FeeAmount,
    decimal NetReceivedAmount, string CurrencyCode, string RefundMethod,
    string? SupplierReferenceNumber, string? TransactionReferenceNumber, bool HasProof,
    string Description, string Status, int VersionNumber, DateTimeOffset CreatedAtUtc);

public sealed record SupplierCreditAllocationSummary(
    Guid SupplierCreditNoteAllocationId, Guid SupplierDebtId, string DebtNumber,
    decimal AllocatedAmount, DateTimeOffset CreatedAtUtc);

public sealed record SupplierCreditNoteSummary(
    Guid SupplierCreditNoteId, string CreditNoteNumber, Guid SupplierId, string SupplierName,
    string? SupplierReferenceNumber, DateOnly CreditNoteDate, decimal CreditNoteAmount,
    decimal AppliedAmount, decimal AvailableAmount, string CurrencyCode, string ReasonType,
    string Description, string Status, int VersionNumber, DateTimeOffset CreatedAtUtc);

public sealed record SupplierCreditNoteDetails(
    SupplierCreditNoteSummary CreditNote, string? Notes,
    IReadOnlyList<SupplierCreditAllocationSummary> Allocations);

public sealed record SupplierStatementEntry(
    Guid EventId, string EventType, string Reference, DateOnly EventDate,
    decimal Amount, string CurrencyCode, string Status, string Description,
    DateTimeOffset OccurredAtUtc);

public sealed record SupplierStatement(
    SupplierSummary Supplier, IReadOnlyList<SupplierBalanceSummary> Balances,
    PagedResult<SupplierStatementEntry> Entries);
