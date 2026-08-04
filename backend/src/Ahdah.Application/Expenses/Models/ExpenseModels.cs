using Ahdah.Application.Access.Models;

namespace Ahdah.Application.Expenses.Models;

public sealed record ExpenseUserSummary(Guid UserId, string FullName, string Role);

public sealed record ExpenseCategorySummary(
    Guid ExpenseCategoryId,
    Guid? ParentExpenseCategoryId,
    string? CategoryCode,
    string CategoryName,
    string CategoryGroup,
    string ExpenseScope,
    string? Description,
    bool RequiresSupplier,
    bool RequiresReceipt,
    bool SupportsQuantityDetails,
    bool IsActive,
    int DisplayOrder,
    int VersionNumber);

public sealed record ExpenseProjectSummary(Guid ProjectId, string ProjectName);

public sealed record ExpenseSummary(
    Guid ExpenseId,
    string ExpenseNumber,
    DateOnly ExpenseDate,
    decimal TotalAmount,
    string CurrencyCode,
    string PaymentMode,
    string Description,
    string Status,
    ExpenseCategorySummary Category,
    ExpenseProjectSummary? Project,
    ExpenseUserSummary IncurredBy,
    ExpenseUserSummary SubmittedBy,
    string? ReceiptNumber,
    string? InvoiceNumber,
    bool HasReceiptDocument,
    string? ReimbursementStatus,
    int VersionNumber,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? SubmittedAtUtc,
    DateTimeOffset? ReviewedAtUtc);

public sealed record ExpenseAdvanceAllocationSummary(
    Guid ExpenseAdvanceAllocationId,
    Guid AdvanceId,
    string AdvanceNumber,
    decimal AllocatedAmount,
    DateTimeOffset CreatedAtUtc);

public sealed record ExpenseDocumentSummary(
    Guid ExpenseDocumentId,
    string DocumentType,
    string? DocumentNumber,
    DateOnly? DocumentDate,
    string? IssuerName,
    string OriginalFileName,
    string MimeType,
    long FileSizeBytes,
    string CaptureSource,
    bool IsPrimary,
    string VerificationStatus,
    ExpenseUserSummary UploadedBy,
    ExpenseUserSummary? VerifiedBy,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? VerifiedAtUtc,
    string? RejectionReason,
    string? Notes,
    int VersionNumber);

public sealed record ExpenseItemSummary(
    Guid ExpenseItemId,
    int LineNumber,
    string ItemName,
    string? ItemCode,
    string? ItemDescription,
    decimal Quantity,
    string UnitCode,
    string? CustomUnitName,
    decimal UnitPrice,
    decimal SubtotalAmount,
    decimal DiscountAmount,
    decimal TaxAmount,
    decimal TotalAmount,
    string? Notes);

public sealed record ReimbursementSummary(
    Guid ReimbursementId,
    string ReimbursementNumber,
    Guid ExpenseId,
    string ExpenseNumber,
    ExpenseUserSummary Claimant,
    ExpenseProjectSummary? Project,
    DateOnly ClaimDate,
    DateOnly? DueDate,
    decimal ClaimAmount,
    decimal OutstandingAmount,
    string CurrencyCode,
    string Description,
    string Status,
    int VersionNumber,
    DateTimeOffset CreatedAtUtc);

public sealed record ExpenseDetails(
    ExpenseSummary Expense,
    decimal SubtotalAmount,
    decimal DiscountAmount,
    decimal TaxAmount,
    string? MerchantName,
    string? ExpenseLocation,
    string? Notes,
    string? CorrectionReason,
    string? RejectionReason,
    ExpenseUserSummary? ReviewedBy,
    IReadOnlyList<ExpenseAdvanceAllocationSummary> AdvanceAllocations,
    IReadOnlyList<ExpenseDocumentSummary> Documents,
    IReadOnlyList<ExpenseItemSummary> Items,
    ReimbursementSummary? Reimbursement);

public sealed record ExpenseHistoryEntry(
    Guid EventId,
    string EventName,
    string EventAction,
    string Outcome,
    string Description,
    ExpenseUserSummary? Actor,
    int? ExpenseVersionNumber,
    DateTimeOffset OccurredAtUtc);

public sealed record ExpenseDocumentPage(PagedResult<ExpenseDocumentSummary> Page);
