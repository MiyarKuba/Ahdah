using System.ComponentModel.DataAnnotations;
using Ahdah.Application.Access.Contracts;

namespace Ahdah.Application.Suppliers.Contracts;

public sealed record SupplierQuery : PagedAccessQuery
{
    public bool? IsActive { get; init; }

    [StringLength(40)]
    public string? SupplierType { get; init; }

    [StringLength(SupplierConstants.MaximumSearchLength, MinimumLength = 2)]
    [RegularExpression("^[^\\r\\n]+$")]
    public string? Search { get; init; }
}

public sealed record CreateSupplierRequest : IValidatableObject
{
    [StringLength(30, MinimumLength = 2)]
    public string? SupplierCode { get; init; }

    [Required, StringLength(200, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public required string SupplierName { get; init; }

    [Required, StringLength(40)]
    public required string SupplierType { get; init; }

    [StringLength(150, MinimumLength = 1)] public string? ContactPersonName { get; init; }
    [StringLength(20)] public string? PhoneNumber { get; init; }
    [StringLength(20)] public string? SecondaryPhoneNumber { get; init; }
    [EmailAddress, StringLength(200)] public string? Email { get; init; }
    [StringLength(100, MinimumLength = 1)] public string? CommercialRegistrationNumber { get; init; }
    [StringLength(100, MinimumLength = 1)] public string? TaxRegistrationNumber { get; init; }
    [StringLength(500, MinimumLength = 1)] public string? Address { get; init; }
    [StringLength(100, MinimumLength = 1)] public string? City { get; init; }

    [Required, StringLength(3, MinimumLength = 3), RegularExpression("^[A-Z]{3}$")]
    public required string DefaultCurrencyCode { get; init; }

    [Required, StringLength(30)] public required string TransactionMode { get; init; }
    [Range(0, int.MaxValue)] public int DefaultPaymentTermsDays { get; init; }
    public decimal? CreditLimit { get; init; }
    [StringLength(30)] public string? PreferredPaymentMethod { get; init; }
    [StringLength(1000, MinimumLength = 1)] public string? Notes { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!SupplierConstants.SupplierTypes.Contains(SupplierType))
            yield return new("Supplier type is invalid.", [nameof(SupplierType)]);
        if (!SupplierConstants.TransactionModes.Contains(TransactionMode))
            yield return new("Transaction mode is invalid.", [nameof(TransactionMode)]);
        if (PreferredPaymentMethod is not null
            && !SupplierConstants.PaymentMethods.Contains(PreferredPaymentMethod))
            yield return new("Preferred payment method is invalid.", [nameof(PreferredPaymentMethod)]);
        if (CreditLimit is < 0m || CreditLimit is { } limit && limit != 0m && !SupplierRules.IsValidMoney(limit))
            yield return new("Credit limit must fit NUMERIC(18,2).", [nameof(CreditLimit)]);
        if (TransactionMode == SupplierConstants.CashOnlyTransactionMode
            && (DefaultPaymentTermsDays != 0 || CreditLimit is > 0m))
            yield return new("Cash-only suppliers cannot have credit terms.",
                [nameof(TransactionMode), nameof(DefaultPaymentTermsDays), nameof(CreditLimit)]);
        if (!ValidCode(SupplierCode))
            yield return new("Supplier code is invalid.", [nameof(SupplierCode)]);
        if (!ValidPhone(PhoneNumber) || !ValidPhone(SecondaryPhoneNumber)
            || PhoneNumber is not null && PhoneNumber == SecondaryPhoneNumber)
            yield return new("Supplier phone numbers are invalid.",
                [nameof(PhoneNumber), nameof(SecondaryPhoneNumber)]);
    }

    private static bool ValidCode(string? value) => value is null
        || System.Text.RegularExpressions.Regex.IsMatch(value.Trim(), "^[A-Z0-9][A-Z0-9_-]{1,29}$")
            && value.Trim() == value.Trim().ToUpperInvariant();
    private static bool ValidPhone(string? value) => value is null
        || System.Text.RegularExpressions.Regex.IsMatch(value, "^\\+?[0-9]{7,15}$");
}

public sealed record UpdateSupplierRequest : IValidatableObject
{
    [Range(1, int.MaxValue)] public int ExpectedVersion { get; init; }
    [StringLength(150, MinimumLength = 1)] public string? ContactPersonName { get; init; }
    [StringLength(20)] public string? PhoneNumber { get; init; }
    [StringLength(20)] public string? SecondaryPhoneNumber { get; init; }
    [EmailAddress, StringLength(200)] public string? Email { get; init; }
    [StringLength(500, MinimumLength = 1)] public string? Address { get; init; }
    [StringLength(100, MinimumLength = 1)] public string? City { get; init; }
    [StringLength(1000, MinimumLength = 1)] public string? Notes { get; init; }
    public bool? IsActive { get; init; }
    [StringLength(500, MinimumLength = 1)] public string? DeactivationReason { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (IsActive == false && string.IsNullOrWhiteSpace(DeactivationReason))
            yield return new("Deactivation reason is required.", [nameof(DeactivationReason)]);
        if (IsActive != false && DeactivationReason is not null)
            yield return new("Deactivation reason is only valid when deactivating.", [nameof(DeactivationReason)]);
        if (!ValidPhone(PhoneNumber) || !ValidPhone(SecondaryPhoneNumber)
            || PhoneNumber is not null && PhoneNumber == SecondaryPhoneNumber)
            yield return new("Supplier phone numbers are invalid.",
                [nameof(PhoneNumber), nameof(SecondaryPhoneNumber)]);
    }

    private static bool ValidPhone(string? value) => value is null
        || System.Text.RegularExpressions.Regex.IsMatch(value, "^\\+?[0-9]{7,15}$");
}

public sealed record SupplierSubresourceQuery : PagedAccessQuery;

public sealed record CreateSupplierPaymentAccountRequest : IValidatableObject
{
    [Required, StringLength(30)] public required string AccountType { get; init; }
    [Required, StringLength(150, MinimumLength = 1)] public required string AccountLabel { get; init; }
    [StringLength(200, MinimumLength = 1)] public string? AccountHolderName { get; init; }
    [StringLength(150, MinimumLength = 1)] public string? BankName { get; init; }
    [StringLength(150, MinimumLength = 1)] public string? BankBranchName { get; init; }
    [StringLength(100, MinimumLength = 1)] public string? AccountNumber { get; init; }
    [StringLength(34, MinimumLength = 15)] public string? Iban { get; init; }
    [StringLength(100, MinimumLength = 1)] public string? WalletProvider { get; init; }
    [StringLength(20)] public string? WalletNumber { get; init; }
    [Required, StringLength(3, MinimumLength = 3), RegularExpression("^[A-Z]{3}$")]
    public required string CurrencyCode { get; init; }
    [StringLength(1000, MinimumLength = 1)] public string? Notes { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!SupplierConstants.AccountTypes.Contains(AccountType))
            yield return new("Account type is invalid.", [nameof(AccountType)]);
        if (AccountType == "BankAccount"
            && (string.IsNullOrWhiteSpace(AccountHolderName) || string.IsNullOrWhiteSpace(BankName)
                || string.IsNullOrWhiteSpace(AccountNumber) && string.IsNullOrWhiteSpace(Iban)))
            yield return new("Bank account details are incomplete.", [nameof(AccountType)]);
        if (AccountType == "MobileWallet"
            && (string.IsNullOrWhiteSpace(AccountHolderName) || string.IsNullOrWhiteSpace(WalletProvider)
                || string.IsNullOrWhiteSpace(WalletNumber)))
            yield return new("Mobile wallet details are incomplete.", [nameof(AccountType)]);
        if (AccountType == "CashCollection"
            && new[] { BankName, BankBranchName, AccountNumber, Iban, WalletProvider, WalletNumber }
                .Any(value => value is not null))
            yield return new("Cash collection cannot contain bank or wallet details.", [nameof(AccountType)]);
        if (AccountType == "Other" && string.IsNullOrWhiteSpace(Notes))
            yield return new("Notes are required for Other account type.", [nameof(Notes)]);
        if (Iban is not null && !System.Text.RegularExpressions.Regex.IsMatch(
            Iban.Replace(" ", "", StringComparison.Ordinal).ToUpperInvariant(), "^[A-Z0-9]{15,34}$"))
            yield return new("IBAN is invalid.", [nameof(Iban)]);
        if (WalletNumber is not null
            && !System.Text.RegularExpressions.Regex.IsMatch(WalletNumber, "^\\+?[0-9]{7,15}$"))
            yield return new("Wallet number is invalid.", [nameof(WalletNumber)]);
    }
}

public sealed record SupplierInvoiceItemRequest
{
    [Required, StringLength(200, MinimumLength = 1)] public required string ItemName { get; init; }
    [StringLength(100, MinimumLength = 1)] public string? ItemCode { get; init; }
    [StringLength(1000, MinimumLength = 1)] public string? ItemDescription { get; init; }
    public decimal Quantity { get; init; }
    [Required, StringLength(30)] public required string UnitCode { get; init; }
    [StringLength(100, MinimumLength = 1)] public string? CustomUnitName { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TaxAmount { get; init; }
    [StringLength(500, MinimumLength = 1)] public string? Notes { get; init; }
}

public sealed record CreateSupplierInvoiceRequest : IValidatableObject
{
    public Guid SupplierId { get; init; }
    public Guid ExpenseCategoryId { get; init; }
    public Guid? ProjectId { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public DateOnly DueDate { get; init; }
    public decimal Amount { get; init; }
    [Required, StringLength(3, MinimumLength = 3), RegularExpression("^[A-Z]{3}$")]
    public required string CurrencyCode { get; init; }
    [StringLength(100, MinimumLength = 1)] public string? InvoiceNumber { get; init; }
    [Required, StringLength(1000, MinimumLength = 1)] public required string Description { get; init; }
    [StringLength(1000, MinimumLength = 1)] public string? Notes { get; init; }
    [MaxLength(100)] public IReadOnlyList<SupplierInvoiceItemRequest> Items { get; init; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (SupplierId == Guid.Empty) yield return new("Supplier ID is required.", [nameof(SupplierId)]);
        if (ExpenseCategoryId == Guid.Empty) yield return new("Expense category ID is required.", [nameof(ExpenseCategoryId)]);
        if (InvoiceDate == default || DueDate < InvoiceDate)
            yield return new("Due date cannot precede invoice date.", [nameof(InvoiceDate), nameof(DueDate)]);
        if (!SupplierRules.IsValidMoney(Amount))
            yield return new("Amount must fit NUMERIC(18,2).", [nameof(Amount)]);
        if (!SupplierRules.InvoiceItemsMatch(Items, Amount))
            yield return new("Invoice items must be valid and total the invoice amount.", [nameof(Items), nameof(Amount)]);
    }
}

public sealed record SupplierInvoiceQuery : PagedAccessQuery
{
    public Guid? SupplierId { get; init; }
    public Guid? ProjectId { get; init; }
    [StringLength(30)] public string? Status { get; init; }
    [StringLength(3, MinimumLength = 3)] public string? CurrencyCode { get; init; }
    public DateOnly? DueFrom { get; init; }
    public DateOnly? DueTo { get; init; }
    public bool? UnpaidOnly { get; init; }
    public bool? OverdueOnly { get; init; }
    [StringLength(SupplierConstants.MaximumSearchLength, MinimumLength = 2)] public string? Reference { get; init; }
}

public sealed record SupplierPaymentDebtAllocationRequest
{
    public Guid SupplierDebtId { get; init; }
    public decimal Amount { get; init; }
    [StringLength(500, MinimumLength = 1)] public string? Notes { get; init; }
}

public sealed record SupplierPaymentFundingAllocationRequest
{
    public Guid FundingSourceId { get; init; }
    public decimal Amount { get; init; }
    [StringLength(500, MinimumLength = 1)] public string? Notes { get; init; }
}

public sealed record CreateSupplierPaymentRequest : IValidatableObject
{
    public Guid SupplierId { get; init; }
    public Guid? SupplierPaymentAccountId { get; init; }
    public DateOnly PaymentDate { get; init; }
    public decimal PaymentAmount { get; init; }
    [Required, StringLength(3, MinimumLength = 3), RegularExpression("^[A-Z]{3}$")]
    public required string CurrencyCode { get; init; }
    [Required, StringLength(30)] public required string PaymentMethod { get; init; }
    [StringLength(150, MinimumLength = 1)] public string? PayerBankName { get; init; }
    [StringLength(150, MinimumLength = 1)] public string? ReferenceNumber { get; init; }
    [StringLength(1000, MinimumLength = 1)] public string? ProofFileUrl { get; init; }
    [StringLength(1000, MinimumLength = 1)] public string? Description { get; init; }
    [StringLength(1000, MinimumLength = 1)] public string? Notes { get; init; }
    [MaxLength(100)] public IReadOnlyList<SupplierPaymentDebtAllocationRequest> DebtAllocations { get; init; } = [];
    [MaxLength(100)] public IReadOnlyList<SupplierPaymentFundingAllocationRequest> FundingAllocations { get; init; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (SupplierId == Guid.Empty) yield return new("Supplier ID is required.", [nameof(SupplierId)]);
        if (PaymentDate == default) yield return new("Payment date is required.", [nameof(PaymentDate)]);
        if (!SupplierRules.IsValidMoney(PaymentAmount))
            yield return new("Payment amount must fit NUMERIC(18,2).", [nameof(PaymentAmount)]);
        if (!SupplierConstants.PaymentMethods.Contains(PaymentMethod))
            yield return new("Payment method is invalid.", [nameof(PaymentMethod)]);
        if (!SupplierRules.PaymentAllocationsMatch(this))
            yield return new("Debt and funding allocations must each be unique and total the payment amount.",
                [nameof(DebtAllocations), nameof(FundingAllocations), nameof(PaymentAmount)]);
        if (PaymentMethod is "BankTransfer" or "MobileWallet" && SupplierPaymentAccountId is null)
            yield return new("A verified supplier account is required.", [nameof(SupplierPaymentAccountId)]);
        if (PaymentMethod is "BankTransfer" or "Cheque" && string.IsNullOrWhiteSpace(PayerBankName))
            yield return new("Payer bank name is required.", [nameof(PayerBankName)]);
        if (PaymentMethod == "Other" && string.IsNullOrWhiteSpace(Description))
            yield return new("Description is required for Other payment method.", [nameof(Description)]);
        if (ProofFileUrl is not null && (!ProofFileUrl.StartsWith('/') || ProofFileUrl.Contains("..")
            || ProofFileUrl.Contains("://", StringComparison.Ordinal)))
            yield return new("Proof path must be application-relative.", [nameof(ProofFileUrl)]);
    }
}

public sealed record SupplierPaymentQuery : PagedAccessQuery
{
    public Guid? SupplierId { get; init; }
    public Guid? SupplierDebtId { get; init; }
    public Guid? FundingSourceId { get; init; }
    [StringLength(30)] public string? Status { get; init; }
    [StringLength(30)] public string? PaymentMethod { get; init; }
    [StringLength(3, MinimumLength = 3)] public string? CurrencyCode { get; init; }
    public DateOnly? DateFrom { get; init; }
    public DateOnly? DateTo { get; init; }
}

public sealed record ReviewSupplierPaymentRequest
{
    [Range(1, int.MaxValue)] public int ExpectedVersion { get; init; }
    [StringLength(500, MinimumLength = 1)] public string? RejectionReason { get; init; }
}

public sealed record CreateSupplierCreditNoteRequest : IValidatableObject
{
    public Guid SupplierId { get; init; }
    public DateOnly CreditNoteDate { get; init; }
    public decimal Amount { get; init; }
    [Required, StringLength(3, MinimumLength = 3), RegularExpression("^[A-Z]{3}$")]
    public required string CurrencyCode { get; init; }
    [StringLength(100, MinimumLength = 1)] public string? SupplierReferenceNumber { get; init; }
    [Required, StringLength(40)] public required string ReasonType { get; init; }
    [Required, StringLength(1000, MinimumLength = 1)] public required string Description { get; init; }
    [StringLength(1000, MinimumLength = 1)] public string? Notes { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (SupplierId == Guid.Empty) yield return new("Supplier ID is required.", [nameof(SupplierId)]);
        if (CreditNoteDate == default) yield return new("Credit note date is required.", [nameof(CreditNoteDate)]);
        if (!SupplierRules.IsValidMoney(Amount)) yield return new("Amount must fit NUMERIC(18,2).", [nameof(Amount)]);
        if (!SupplierConstants.CreditNoteReasonTypes.Contains(ReasonType))
            yield return new("Credit note reason is invalid.", [nameof(ReasonType)]);
    }
}

public sealed record SupplierCreditNoteQuery : PagedAccessQuery
{
    public Guid? SupplierId { get; init; }
    [StringLength(30)] public string? Status { get; init; }
    [StringLength(3, MinimumLength = 3)] public string? CurrencyCode { get; init; }
}

public sealed record ApproveSupplierCreditNoteRequest
{
    [Range(1, int.MaxValue)] public int ExpectedVersion { get; init; }
}

public sealed record SupplierCreditAllocationRequest
{
    public Guid SupplierDebtId { get; init; }
    public decimal Amount { get; init; }
    [StringLength(500, MinimumLength = 1)] public string? Notes { get; init; }
}

public sealed record ApplySupplierCreditNoteRequest : IValidatableObject
{
    [Range(1, int.MaxValue)] public int ExpectedVersion { get; init; }
    [MaxLength(100)] public IReadOnlyList<SupplierCreditAllocationRequest> Allocations { get; init; } = [];
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!SupplierRules.CreditAllocationsAreValid(Allocations))
            yield return new("Credit allocations must be unique and positive.", [nameof(Allocations)]);
    }
}

public sealed record SupplierRefundQuery : PagedAccessQuery
{
    public Guid? SupplierId { get; init; }
    [StringLength(30)] public string? Status { get; init; }
    [StringLength(3, MinimumLength = 3)] public string? CurrencyCode { get; init; }
}

public sealed record SupplierStatementQuery : PagedAccessQuery
{
    [StringLength(3, MinimumLength = 3)] public string? CurrencyCode { get; init; }
}
