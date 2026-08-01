using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Ahdah.Application.Access;
using Ahdah.Application.CompanyMembers.Contracts;
using Ahdah.Application.Identity;
using Ahdah.Application.Projects;
using Ahdah.Application.Projects.Contracts;
using Ahdah.Application.Projects.Models;

namespace Ahdah.UnitTests;

public sealed class CompanyStructureRulesTests
{
    [Theory]
    [InlineData("Manager", true, false, true, true, true)]
    [InlineData("Deputy", true, false, false, false, true)]
    [InlineData("Accountant", true, false, false, false, false)]
    [InlineData("Supervisor", false, true, false, false, true)]
    [InlineData("Worker", false, false, false, false, false)]
    [InlineData("FutureRole", false, false, false, false, false)]
    public void Project_visibility_and_capabilities_are_conservative(
        string role,
        bool viewAll,
        bool assignedOnly,
        bool contractValue,
        bool mutate,
        bool viewMembers)
    {
        var capabilities = ProjectRoleCapabilities.For(role);

        Assert.Equal(viewAll, capabilities.CanViewAllCompanyProjects);
        Assert.Equal(assignedOnly, capabilities.RequiresSupervisorAssignment);
        Assert.Equal(contractValue, capabilities.CanViewContractValue);
        Assert.Equal(mutate, capabilities.CanCreate);
        Assert.Equal(mutate, capabilities.CanUpdate);
        Assert.Equal(mutate, capabilities.CanAssignSupervisor);
        Assert.Equal(viewMembers, capabilities.CanViewProjectMembers);
    }

    [Fact]
    public void Contract_value_is_serialized_for_manager_mapping_and_omitted_for_others()
    {
        var managerJson = JsonSerializer.Serialize(Project(1250.25m));
        var nonManagerJson = JsonSerializer.Serialize(Project(null));

        Assert.Contains("ContractValue", managerJson, StringComparison.Ordinal);
        Assert.Contains("1250.25", managerJson, StringComparison.Ordinal);
        Assert.DoesNotContain("ContractValue", nonManagerJson, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("1.001")]
    [InlineData("10000000000000000")]
    public void Project_creation_rejects_invalid_contract_values(string value)
    {
        var request = ValidCreate() with { ContractValue = decimal.Parse(value) };

        Assert.Contains(Validate(request), result =>
            result.MemberNames.Contains(nameof(request.ContractValue)));
    }

    [Fact]
    public void Project_creation_accepts_numeric_18_2_contract_value()
    {
        var request = ValidCreate() with { ContractValue = ProjectConstants.MaximumContractValue };

        Assert.Empty(Validate(request));
    }

    [Fact]
    public void Project_creation_requires_exactly_one_owner_source()
    {
        var request = ValidCreate() with
        {
            ProjectOwnerId = Guid.NewGuid(),
            NewOwner = new NewProjectOwnerRequest
            {
                OwnerName = "Owner",
                PhoneNumber = "+218912345678"
            }
        };

        Assert.Contains(Validate(request), result =>
            result.MemberNames.Contains(nameof(request.ProjectOwnerId)));
    }

    [Fact]
    public void Project_creation_rejects_incomplete_coordinates_and_invalid_date_order()
    {
        var request = ValidCreate() with
        {
            Latitude = 32.9m,
            Longitude = null,
            ExpectedEndDate = new DateOnly(2026, 7, 31)
        };

        var results = Validate(request);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(request.Latitude)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(request.ExpectedEndDate)));
    }

    [Fact]
    public void Project_creation_rejects_missing_dates_and_excess_coordinate_scale()
    {
        var request = ValidCreate() with
        {
            ContractDate = default,
            StartDate = default,
            Latitude = 32.1234567m,
            Longitude = 13.123456m
        };

        var results = Validate(request);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(request.ContractDate)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(request.StartDate)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(request.Latitude)));
    }

    [Theory]
    [InlineData("Completed")]
    [InlineData("FinanciallyClosed")]
    [InlineData("Cancelled")]
    public void Project_update_keeps_final_and_cancellation_transitions_deferred(string status)
    {
        var request = new UpdateProjectRequest
        {
            ExpectedVersion = 1,
            Status = status
        };

        Assert.Contains(Validate(request), result => result.MemberNames.Contains(nameof(request.Status)));
        Assert.False(ProjectLifecycleRules.IsAllowedMetadataTransition(status));
    }

    [Theory]
    [InlineData("Active")]
    [InlineData("Paused")]
    public void Project_update_allows_only_non_final_statuses(string status)
    {
        var request = new UpdateProjectRequest
        {
            ExpectedVersion = 1,
            Status = status
        };

        Assert.Empty(Validate(request));
        Assert.True(ProjectLifecycleRules.IsAllowedMetadataTransition(status));
    }

    [Fact]
    public void Project_search_and_pagination_are_bounded()
    {
        var query = new ProjectQuery
        {
            Page = 0,
            PageSize = AccessConstants.MaximumPageSize + 1,
            Search = new string('a', ProjectConstants.MaximumSearchLength + 1)
        };

        var results = Validate(query);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(query.Page)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(query.PageSize)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(query.Search)));
        Assert.False(ProjectRequestRules.IsSearchValid("  "));
    }

    [Fact]
    public void Supervisor_assignment_requires_exact_role_and_active_status()
    {
        Assert.True(ProjectAssignmentRules.IsEligibleSupervisor(
            AccessConstants.SupervisorRole,
            AccessConstants.ActiveUserStatus));
        Assert.False(ProjectAssignmentRules.IsEligibleSupervisor(
            IdentityConstants.ManagerRole,
            AccessConstants.ActiveUserStatus));
        Assert.False(ProjectAssignmentRules.IsEligibleSupervisor(
            AccessConstants.SupervisorRole,
            "Suspended"));
    }

    [Fact]
    public void Company_member_filters_reject_unknown_values()
    {
        var query = new CompanyMemberQuery
        {
            Role = "Administrator",
            Status = "Deleted"
        };

        var results = Validate(query);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(query.Role)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(query.Status)));
    }

    private static CreateProjectRequest ValidCreate() => new()
    {
        ProjectName = "Project One",
        SiteAddress = "Tripoli",
        ContractValue = 150000.25m,
        ContractDate = new DateOnly(2026, 8, 1),
        StartDate = new DateOnly(2026, 8, 1),
        ExpectedEndDate = new DateOnly(2027, 8, 1),
        NewOwner = new NewProjectOwnerRequest
        {
            OwnerName = "Owner One",
            PhoneNumber = "+218912345678"
        }
    };

    private static ProjectDetails Project(decimal? contractValue)
    {
        var now = DateTimeOffset.Parse("2026-08-01T00:00:00Z");
        return new ProjectDetails(
            Guid.NewGuid(),
            "Project One",
            new ProjectOwnerSummary(Guid.NewGuid(), "Owner", "+218912345678", null, null),
            "Tripoli",
            null,
            null,
            null,
            contractValue,
            new DateOnly(2026, 8, 1),
            new DateOnly(2026, 8, 1),
            null,
            null,
            ProjectConstants.ActiveStatus,
            null,
            null,
            [],
            1,
            now,
            now);
    }

    private static List<ValidationResult> Validate(object value)
    {
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(value, new ValidationContext(value), validationResults, true);
        return validationResults;
    }
}
