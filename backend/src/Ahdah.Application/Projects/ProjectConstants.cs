using Ahdah.Application.Access;
using Ahdah.Application.Identity;

namespace Ahdah.Application.Projects;

public static class ProjectConstants
{
    public const string ActiveStatus = "Active";
    public const string PausedStatus = "Paused";
    public const string CompletedStatus = "Completed";
    public const string FinanciallyClosedStatus = "FinanciallyClosed";
    public const string CancelledStatus = "Cancelled";

    public const int MaximumSearchLength = 100;
    public const decimal MaximumContractValue = 9999999999999999.99m;
}

public sealed record ProjectRoleCapabilities(
    bool CanViewAllCompanyProjects,
    bool RequiresSupervisorAssignment,
    bool CanViewContractValue,
    bool CanCreate,
    bool CanUpdate,
    bool CanAssignSupervisor,
    bool CanViewProjectMembers)
{
    public static ProjectRoleCapabilities For(string? role) => role switch
    {
        IdentityConstants.ManagerRole => new(true, false, true, true, true, true, true),
        AccessConstants.DeputyRole => new(true, false, false, false, false, false, true),
        AccessConstants.AccountantRole => new(true, false, false, false, false, false, false),
        AccessConstants.SupervisorRole => new(false, true, false, false, false, false, true),
        _ => new(false, false, false, false, false, false, false)
    };
}

public static class ProjectLifecycleRules
{
    public static bool IsKnownStatus(string? status) => status is
        ProjectConstants.ActiveStatus or
        ProjectConstants.PausedStatus or
        ProjectConstants.CompletedStatus or
        ProjectConstants.FinanciallyClosedStatus or
        ProjectConstants.CancelledStatus;

    public static bool IsMutableStatus(string? status) => status is
        ProjectConstants.ActiveStatus or
        ProjectConstants.PausedStatus;

    public static bool IsAllowedMetadataTransition(string? status) => IsMutableStatus(status);
}

public static class ProjectAssignmentRules
{
    public static bool IsEligibleSupervisor(string? role, string? status) =>
        role == AccessConstants.SupervisorRole && status == AccessConstants.ActiveUserStatus;
}

public static class ProjectRequestRules
{
    public static bool IsContractValueValid(decimal value) =>
        value > 0m
        && value <= ProjectConstants.MaximumContractValue
        && decimal.Truncate(value * 100m) == value * 100m;

    public static bool AreCoordinatesValid(decimal? latitude, decimal? longitude) =>
        latitude is null && longitude is null
        || latitude is >= -90m and <= 90m
        && longitude is >= -180m and <= 180m
        && HasMaximumScale(latitude.Value, 6)
        && HasMaximumScale(longitude.Value, 6);

    public static bool AreDatesValid(
        DateOnly startDate,
        DateOnly? expectedEndDate) =>
        expectedEndDate is null || expectedEndDate >= startDate;

    public static bool IsSearchValid(string? search) =>
        search is null
        || search.Trim().Length is >= 2 and <= ProjectConstants.MaximumSearchLength;

    private static bool HasMaximumScale(decimal value, int scale)
    {
        var multiplier = 1m;
        for (var index = 0; index < scale; index++)
        {
            multiplier *= 10m;
        }

        return decimal.Truncate(value * multiplier) == value * multiplier;
    }
}
