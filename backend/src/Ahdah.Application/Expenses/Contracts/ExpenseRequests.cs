using System.ComponentModel.DataAnnotations;
using Ahdah.Application.Access.Contracts;

namespace Ahdah.Application.Expenses.Contracts;

public sealed record ExpenseCategoryQuery : PagedAccessQuery
{
    [StringLength(30)]
    public string? CategoryGroup { get; init; }

    [StringLength(20)]
    public string? ExpenseScope { get; init; }
}

public sealed record CreateExpenseCategoryRequest : IValidatableObject
{
    public Guid? ParentExpenseCategoryId { get; init; }

    [StringLength(30, MinimumLength = 2)]
    public string? CategoryCode { get; init; }

    [Required]
    [StringLength(150, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public required string CategoryName { get; init; }

    [Required]
    [StringLength(30)]
    public required string CategoryGroup { get; init; }

    [Required]
    [StringLength(20)]
    public required string ExpenseScope { get; init; }

    [StringLength(500, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? Description { get; init; }

    public bool RequiresSupplier { get; init; }
    public bool RequiresReceipt { get; init; } = true;
    public bool SupportsQuantityDetails { get; init; }

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!ExpenseConstants.CategoryGroups.Contains(CategoryGroup))
        {
            yield return new ValidationResult("Category group is invalid.", [nameof(CategoryGroup)]);
        }

        if (!ExpenseConstants.CategoryScopes.Contains(ExpenseScope))
        {
            yield return new ValidationResult("Expense scope is invalid.", [nameof(ExpenseScope)]);
        }

        if (CategoryCode is { } code
            && (code.Trim().ToUpperInvariant() != code.Trim()
                || !System.Text.RegularExpressions.Regex.IsMatch(
                    code.Trim(), "^[A-Z0-9][A-Z0-9_-]{1,29}$")))
        {
            yield return new ValidationResult("Category code is invalid.", [nameof(CategoryCode)]);
        }
    }
}

public sealed record ExpenseAdvanceAllocationRequest
{
    public Guid UserAdvanceBalanceId { get; init; }
    public decimal Amount { get; init; }

    [StringLength(500, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? Notes { get; init; }
}

public sealed record CreateExpenseRequest : IValidatableObject
{
    public Guid ExpenseCategoryId { get; init; }
    public Guid? ProjectId { get; init; }
    public DateOnly ExpenseDate { get; init; }
    public decimal Amount { get; init; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    [RegularExpression("^[A-Z]{3}$")]
    public required string CurrencyCode { get; init; }

    [Required]
    [StringLength(30)]
    public required string PaymentMode { get; init; }

    [Required]
    [StringLength(1000, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public required string Description { get; init; }

    [StringLength(100, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? InvoiceNumber { get; init; }

    [StringLength(100, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? ReceiptNumber { get; init; }

    [StringLength(200, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? MerchantName { get; init; }

    [StringLength(500, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? ExpenseLocation { get; init; }

    [StringLength(1000, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? Notes { get; init; }

    [MaxLength(100)]
    public IReadOnlyList<ExpenseAdvanceAllocationRequest> AdvanceAllocations { get; init; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ExpenseCategoryId == Guid.Empty)
        {
            yield return new ValidationResult("Expense category ID is required.", [nameof(ExpenseCategoryId)]);
        }

        if (ExpenseDate == default)
        {
            yield return new ValidationResult("Expense date is required.", [nameof(ExpenseDate)]);
        }

        if (!ExpenseRules.IsValidMoney(Amount))
        {
            yield return new ValidationResult("Amount must be positive and fit NUMERIC(18,2).", [nameof(Amount)]);
        }

        if (!ExpenseRules.IsSupportedCreationPaymentMode(PaymentMode))
        {
            yield return new ValidationResult(
                "Only AdvanceBalance and PersonalFunds are supported for expense creation.",
                [nameof(PaymentMode)]);
        }

        if (PaymentMode == ExpenseConstants.AdvanceBalancePaymentMode
            && !ExpenseRules.AllocationsMatch(AdvanceAllocations, Amount))
        {
            yield return new ValidationResult(
                "Advance allocations must be unique, positive, and total the expense amount.",
                [nameof(AdvanceAllocations), nameof(Amount)]);
        }

        if (PaymentMode == ExpenseConstants.PersonalFundsPaymentMode && AdvanceAllocations.Count != 0)
        {
            yield return new ValidationResult(
                "Personal-funds expenses cannot include advance allocations.",
                [nameof(AdvanceAllocations)]);
        }
    }
}

public sealed record ExpenseQuery : PagedAccessQuery
{
    [StringLength(30)]
    public string? Status { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? ProjectId { get; init; }
    public Guid? IncurredByUserId { get; init; }
    public Guid? SubmittedByUserId { get; init; }

    [StringLength(30)]
    public string? PaymentMode { get; init; }

    [StringLength(ExpenseConstants.MaximumSearchLength, MinimumLength = 2)]
    [RegularExpression("^[^\\r\\n]+$")]
    public string? Reference { get; init; }
}

public sealed record ExpenseHistoryQuery : PagedAccessQuery;

public sealed record ExpenseSubresourceQuery : PagedAccessQuery;

public sealed record ApproveExpenseRequest
{
    [Range(1, int.MaxValue)]
    public int ExpectedVersion { get; init; }
}

public sealed record RejectExpenseRequest
{
    [Range(1, int.MaxValue)]
    public int ExpectedVersion { get; init; }

    [Required]
    [StringLength(1000, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public required string Reason { get; init; }
}

public sealed record AddExpenseDocumentRequest : IValidatableObject
{
    [Required]
    [StringLength(30)]
    public required string DocumentType { get; init; }

    [StringLength(100, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? DocumentNumber { get; init; }

    public DateOnly? DocumentDate { get; init; }

    [StringLength(200, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? IssuerName { get; init; }

    [Required]
    [StringLength(255, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n/\\\\]+$")]
    public required string OriginalFileName { get; init; }

    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public required string FileUrl { get; init; }

    [Required]
    [StringLength(150, MinimumLength = 3)]
    public required string MimeType { get; init; }

    [Range(1, long.MaxValue)]
    public long FileSizeBytes { get; init; }

    [Required]
    [StringLength(64, MinimumLength = 64)]
    [RegularExpression("^[A-Fa-f0-9]{64}$")]
    public required string Sha256Hash { get; init; }

    [Required]
    [StringLength(30)]
    public required string CaptureSource { get; init; }

    public bool IsPrimary { get; init; }

    [StringLength(1000, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? Notes { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!ExpenseConstants.DocumentTypes.Contains(DocumentType))
        {
            yield return new ValidationResult("Document type is invalid.", [nameof(DocumentType)]);
        }

        if (!ExpenseConstants.CaptureSources.Contains(CaptureSource))
        {
            yield return new ValidationResult("Capture source is invalid.", [nameof(CaptureSource)]);
        }

        if (!FileUrl.StartsWith("/", StringComparison.Ordinal)
            || FileUrl.Contains("..", StringComparison.Ordinal)
            || FileUrl.Contains("://", StringComparison.Ordinal))
        {
            yield return new ValidationResult(
                "File URL must be a safe application-relative path.",
                [nameof(FileUrl)]);
        }
    }
}

public sealed record ReimbursementQuery : PagedAccessQuery
{
    [StringLength(30)]
    public string? Status { get; init; }
    public Guid? ClaimantUserId { get; init; }
}
