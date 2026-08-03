using System.ComponentModel.DataAnnotations;
using Ahdah.Application.Access.Contracts;

namespace Ahdah.Application.Advances.Contracts;

public sealed record AdvanceFundingAllocationRequest
{
    public Guid FundingSourceId { get; init; }

    public decimal Amount { get; init; }

    [StringLength(500, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? Notes { get; init; }
}

public abstract record AdvanceTransferRequestBase : IValidatableObject
{
    public decimal Amount { get; init; }

    public DateOnly TransferDate { get; init; }

    [Required]
    [StringLength(30)]
    public required string TransferMethod { get; init; }

    [StringLength(150, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? BankName { get; init; }

    [StringLength(150, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? ReferenceNumber { get; init; }

    [StringLength(1000, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? ProofFileUrl { get; init; }

    [StringLength(1000, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? Description { get; init; }

    [StringLength(1000, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? Notes { get; init; }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!AdvanceRules.IsValidMoney(Amount))
        {
            yield return new ValidationResult(
                "Amount must be positive and fit NUMERIC(18,2).",
                [nameof(Amount)]);
        }

        if (TransferDate == default)
        {
            yield return new ValidationResult("Transfer date is required.", [nameof(TransferDate)]);
        }

        if (!AdvanceRules.IsTransferMethodDetailValid(
                TransferMethod,
                BankName,
                ReferenceNumber,
                Description))
        {
            yield return new ValidationResult(
                "Transfer method details are invalid.",
                [nameof(TransferMethod), nameof(BankName), nameof(ReferenceNumber), nameof(Description)]);
        }
    }
}

public sealed record CreateAdvanceRequest : AdvanceTransferRequestBase
{
    public Guid RecipientUserId { get; init; }

    public DateOnly IssueDate { get; init; }

    public DateOnly? SettlementDueDate { get; init; }

    [Required]
    [StringLength(1000, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public required string Purpose { get; init; }

    [MinLength(1)]
    [MaxLength(100)]
    public IReadOnlyList<AdvanceFundingAllocationRequest> Fundings { get; init; } = [];

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var result in base.Validate(validationContext))
        {
            yield return result;
        }

        if (RecipientUserId == Guid.Empty)
        {
            yield return new ValidationResult("Recipient user ID is required.", [nameof(RecipientUserId)]);
        }

        if (IssueDate == default)
        {
            yield return new ValidationResult("Issue date is required.", [nameof(IssueDate)]);
        }

        if (SettlementDueDate is { } dueDate && dueDate < IssueDate)
        {
            yield return new ValidationResult(
                "Settlement due date cannot be before issue date.",
                [nameof(SettlementDueDate)]);
        }

        if (Fundings.Count == 0
            || Fundings.Any(funding => funding.FundingSourceId == Guid.Empty
                || !AdvanceRules.IsValidMoney(funding.Amount))
            || Fundings.Select(funding => funding.FundingSourceId).Distinct().Count() != Fundings.Count
            || !AdvanceRules.FundingTotalMatches(Fundings, Amount))
        {
            yield return new ValidationResult(
                "Funding allocations must be unique, valid, and total the advance amount.",
                [nameof(Fundings), nameof(Amount)]);
        }
    }
}

public sealed record CreateAdvanceDistributionRequest : AdvanceTransferRequestBase
{
    public Guid RecipientUserId { get; init; }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var result in base.Validate(validationContext))
        {
            yield return result;
        }

        if (RecipientUserId == Guid.Empty)
        {
            yield return new ValidationResult("Recipient user ID is required.", [nameof(RecipientUserId)]);
        }
    }
}

public sealed record CreateAdvanceReturnRequest : AdvanceTransferRequestBase;

public sealed record RejectAdvanceTransferRequest
{
    [Required]
    [StringLength(500, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public required string Reason { get; init; }
}

public sealed record AdvanceQuery : PagedAccessQuery
{
    [StringLength(30)]
    public string? Status { get; init; }

    public Guid? UserId { get; init; }

    [StringLength(AdvanceConstants.MaximumSearchLength, MinimumLength = 2)]
    [RegularExpression("^[^\\r\\n]+$")]
    public string? Reference { get; init; }
}

public sealed record AdvanceMovementQuery : PagedAccessQuery;

public sealed record AdvanceBalanceQuery : PagedAccessQuery;

public sealed record AvailableFundingSourceQuery : PagedAccessQuery;
