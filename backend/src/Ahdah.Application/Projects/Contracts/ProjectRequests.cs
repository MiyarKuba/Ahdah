using System.ComponentModel.DataAnnotations;
using Ahdah.Application.Access.Contracts;

namespace Ahdah.Application.Projects.Contracts;

public sealed record NewProjectOwnerRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public required string OwnerName { get; init; }

    [Required]
    [StringLength(20)]
    [RegularExpression("^\\s*\\+[1-9][0-9]{7,14}\\s*$")]
    public required string PhoneNumber { get; init; }

    [EmailAddress]
    [StringLength(254)]
    public string? Email { get; init; }

    [StringLength(500, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? Address { get; init; }

    [StringLength(1000, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? Notes { get; init; }
}

public sealed record CreateProjectRequest : IValidatableObject
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public required string ProjectName { get; init; }

    [Required]
    [StringLength(500, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public required string SiteAddress { get; init; }

    public decimal? Latitude { get; init; }

    public decimal? Longitude { get; init; }

    [StringLength(20)]
    [RegularExpression("^\\s*\\+[1-9][0-9]{7,14}\\s*$")]
    public string? ContactPhoneNumber { get; init; }

    public decimal ContractValue { get; init; }

    public DateOnly ContractDate { get; init; }

    public DateOnly StartDate { get; init; }

    public DateOnly? ExpectedEndDate { get; init; }

    [StringLength(1500, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? Description { get; init; }

    [StringLength(1000, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? Notes { get; init; }

    public Guid? ProjectOwnerId { get; init; }

    public NewProjectOwnerRequest? NewOwner { get; init; }

    public Guid? SupervisorUserId { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ContractDate is { } contractDate && contractDate == default)
        {
            yield return new ValidationResult(
                "Contract date is required.",
                [nameof(ContractDate)]);
        }

        if (StartDate is { } startDateValue && startDateValue == default)
        {
            yield return new ValidationResult(
                "Start date is required.",
                [nameof(StartDate)]);
        }

        if (ProjectOwnerId.HasValue == (NewOwner is not null)
            || ProjectOwnerId == Guid.Empty)
        {
            yield return new ValidationResult(
                "Provide exactly one existing projectOwnerId or newOwner.",
                [nameof(ProjectOwnerId), nameof(NewOwner)]);
        }

        if (SupervisorUserId == Guid.Empty)
        {
            yield return new ValidationResult(
                "Supervisor user ID must be a non-empty GUID.",
                [nameof(SupervisorUserId)]);
        }

        if (!ProjectRequestRules.IsContractValueValid(ContractValue))
        {
            yield return new ValidationResult(
                "Contract value must be positive and fit NUMERIC(18,2).",
                [nameof(ContractValue)]);
        }

        if (!ProjectRequestRules.AreCoordinatesValid(Latitude, Longitude))
        {
            yield return new ValidationResult(
                "Latitude and longitude must be supplied together and be in range.",
                [nameof(Latitude), nameof(Longitude)]);
        }

        if (!ProjectRequestRules.AreDatesValid(StartDate, ExpectedEndDate))
        {
            yield return new ValidationResult(
                "Expected end date cannot be before the start date.",
                [nameof(ExpectedEndDate)]);
        }
    }
}

public sealed record UpdateProjectRequest : IValidatableObject
{
    [Range(1, int.MaxValue)]
    public int ExpectedVersion { get; init; }

    [StringLength(200, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? ProjectName { get; init; }

    [StringLength(500, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? SiteAddress { get; init; }

    [StringLength(20)]
    [RegularExpression("^\\s*\\+[1-9][0-9]{7,14}\\s*$")]
    public string? ContactPhoneNumber { get; init; }

    public DateOnly? ContractDate { get; init; }

    public DateOnly? StartDate { get; init; }

    public DateOnly? ExpectedEndDate { get; init; }

    [RegularExpression("^(Active|Paused)$")]
    public string? Status { get; init; }

    [StringLength(1500, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? Description { get; init; }

    [StringLength(1000, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? Notes { get; init; }

    public Guid? ProjectOwnerId { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ContractDate is { } contractDate && contractDate == default)
        {
            yield return new ValidationResult(
                "Contract date must be valid when supplied.",
                [nameof(ContractDate)]);
        }

        if (StartDate is { } startDateValue && startDateValue == default)
        {
            yield return new ValidationResult(
                "Start date must be valid when supplied.",
                [nameof(StartDate)]);
        }

        if (ProjectOwnerId == Guid.Empty)
        {
            yield return new ValidationResult(
                "Project owner ID must be a non-empty GUID.",
                [nameof(ProjectOwnerId)]);
        }

        if (StartDate is { } startDate
            && ExpectedEndDate is { } expectedEndDate
            && !ProjectRequestRules.AreDatesValid(startDate, expectedEndDate))
        {
            yield return new ValidationResult(
                "Expected end date cannot be before the start date.",
                [nameof(ExpectedEndDate)]);
        }

        if (ProjectName is null
            && SiteAddress is null
            && ContactPhoneNumber is null
            && ContractDate is null
            && StartDate is null
            && ExpectedEndDate is null
            && Status is null
            && Description is null
            && Notes is null
            && ProjectOwnerId is null)
        {
            yield return new ValidationResult("At least one project field must be supplied.");
        }
    }
}

public sealed record AssignProjectSupervisorRequest
{
    public Guid SupervisorUserId { get; init; }

    [Range(1, int.MaxValue)]
    public int ExpectedVersion { get; init; }
}

public sealed record ProjectQuery : PagedAccessQuery
{
    [RegularExpression("^(Active|Paused|Completed|FinanciallyClosed|Cancelled)$")]
    public string? Status { get; init; }

    [StringLength(ProjectConstants.MaximumSearchLength, MinimumLength = 2)]
    [RegularExpression("^[^\\r\\n]+$")]
    public string? Search { get; init; }
}

public sealed record ProjectMemberQuery : PagedAccessQuery;
