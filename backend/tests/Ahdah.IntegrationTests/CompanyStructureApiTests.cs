using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Ahdah.Application.Abstractions.Authentication;
using Ahdah.Application.Abstractions.Context;
using Ahdah.Application.Access;
using Ahdah.Application.Access.Models;
using Ahdah.Application.CompanyMembers.Contracts;
using Ahdah.Application.CompanyMembers.Models;
using Ahdah.Application.CompanyMembers.Services;
using Ahdah.Application.Identity;
using Ahdah.Application.Projects;
using Ahdah.Application.Projects.Contracts;
using Ahdah.Application.Projects.Models;
using Ahdah.Application.Projects.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Ahdah.IntegrationTests;

public sealed class CompanyStructureApiTests(IdentityApiFactory factory)
    : IClassFixture<IdentityApiFactory>
{
    [Fact]
    public async Task Company_member_list_requires_authentication()
    {
        using var client = factory.CreateSecureClient();

        var response = await client.GetAsync("/api/v1/company/members");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("Manager", HttpStatusCode.OK)]
    [InlineData("Deputy", HttpStatusCode.OK)]
    [InlineData("Accountant", HttpStatusCode.Forbidden)]
    [InlineData("Supervisor", HttpStatusCode.Forbidden)]
    [InlineData("Worker", HttpStatusCode.Forbidden)]
    public async Task Company_directory_uses_approved_role_matrix(
        string role,
        HttpStatusCode expectedStatus)
    {
        using var client = CreateAuthenticatedClient(role);

        var response = await client.GetAsync("/api/v1/company/members?page=1&pageSize=20");

        Assert.Equal(expectedStatus, response.StatusCode);
    }

    [Fact]
    public async Task Project_list_requires_authentication()
    {
        using var client = factory.CreateSecureClient();

        var response = await client.GetAsync("/api/v1/projects");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Contract_value_is_present_for_manager_and_omitted_for_accountant()
    {
        using var manager = CreateAuthenticatedClient(IdentityConstants.ManagerRole);
        using var accountant = CreateAuthenticatedClient(AccessConstants.AccountantRole);

        var managerBody = await (await manager.GetAsync("/api/v1/projects")).Content.ReadAsStringAsync();
        var accountantBody = await (await accountant.GetAsync("/api/v1/projects")).Content.ReadAsStringAsync();

        Assert.Contains("\"contractValue\":1250.25", managerBody, StringComparison.Ordinal);
        Assert.DoesNotContain("contractValue", accountantBody, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("Deputy")]
    [InlineData("Accountant")]
    [InlineData("Supervisor")]
    [InlineData("Worker")]
    public async Task Non_managers_cannot_create_projects(string role)
    {
        using var client = CreateAuthenticatedClient(role);

        var response = await client.PostAsJsonAsync("/api/v1/projects", ValidCreateRequest());

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Manager_project_creation_returns_safe_dto()
    {
        using var client = CreateAuthenticatedClient(IdentityConstants.ManagerRole);

        var response = await client.PostAsJsonAsync("/api/v1/projects", ValidCreateRequest());
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Contains("\"contractValue\":1250.25", body, StringComparison.Ordinal);
        Assert.DoesNotContain("companyId", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("createdByUserId", body, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("Deputy")]
    [InlineData("Accountant")]
    [InlineData("Supervisor")]
    [InlineData("Worker")]
    public async Task Non_managers_cannot_assign_project_supervisor(string role)
    {
        using var client = CreateAuthenticatedClient(role);

        var response = await client.PutAsJsonAsync(
            $"/api/v1/projects/{FakeProjectService.ProjectId}/supervisor",
            new AssignProjectSupervisorRequest
            {
                SupervisorUserId = Guid.NewGuid(),
                ExpectedVersion = 1
            });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Manager_can_access_supervisor_assignment_route()
    {
        using var client = CreateAuthenticatedClient(IdentityConstants.ManagerRole);

        var response = await client.PutAsJsonAsync(
            $"/api/v1/projects/{FakeProjectService.ProjectId}/supervisor",
            new AssignProjectSupervisorRequest
            {
                SupervisorUserId = Guid.NewGuid(),
                ExpectedVersion = 1
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Unknown_project_returns_safe_404()
    {
        using var client = CreateAuthenticatedClient(IdentityConstants.ManagerRole);

        var response = await client.GetAsync($"/api/v1/projects/{Guid.NewGuid()}");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("projects.not_found", body, StringComparison.Ordinal);
        Assert.DoesNotContain("sql", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Accountant_cannot_access_project_members()
    {
        using var client = CreateAuthenticatedClient(AccessConstants.AccountantRole);

        var response = await client.GetAsync(
            $"/api/v1/projects/{FakeProjectService.ProjectId}/members");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task OpenApi_publishes_company_structure_dtos_without_generated_entities()
    {
        using var client = factory.CreateSecureClient();

        var response = await client.GetAsync("/openapi/v1.json");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("/api/v1/company/members", body, StringComparison.Ordinal);
        Assert.Contains("/api/v1/projects", body, StringComparison.Ordinal);
        Assert.Contains("/api/v1/projects/{projectId}/supervisor", body, StringComparison.Ordinal);
        Assert.Contains("/api/v1/projects/{projectId}/members", body, StringComparison.Ordinal);
        Assert.Contains(nameof(ProjectDetails), body, StringComparison.Ordinal);
        Assert.Contains(nameof(CompanyMemberDetails), body, StringComparison.Ordinal);
        Assert.DoesNotContain("Ahdah.Infrastructure.Persistence.Generated", body, StringComparison.Ordinal);
        Assert.DoesNotContain("passwordHash", body, StringComparison.OrdinalIgnoreCase);
    }

    private HttpClient CreateAuthenticatedClient(string role)
    {
        var client = factory.CreateSecureClient();
        var tokenService = factory.Services.GetRequiredService<IAccessTokenService>();
        var token = tokenService.CreateToken(new AccessTokenSubject(
            IdentityApiFactory.UserId,
            IdentityApiFactory.CompanyId,
            role,
            "Test User"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        return client;
    }

    private static CreateProjectRequest ValidCreateRequest() => new()
    {
        ProjectName = "Project One",
        SiteAddress = "Tripoli",
        ContractValue = 1250.25m,
        ContractDate = new DateOnly(2026, 8, 1),
        StartDate = new DateOnly(2026, 8, 1),
        NewOwner = new NewProjectOwnerRequest
        {
            OwnerName = "Owner",
            PhoneNumber = "+218912345678"
        }
    };
}

internal sealed class FakeCompanyMemberService : ICompanyMemberService
{
    public Task<AccessResult<PagedResult<CompanyMemberSummary>>> ListAsync(
        CompanyMemberQuery query,
        CancellationToken cancellationToken) =>
        Task.FromResult(AccessResult<PagedResult<CompanyMemberSummary>>.Success(
            new PagedResult<CompanyMemberSummary>([], query.Page, query.PageSize, 0, 0)));

    public Task<AccessResult<CompanyMemberDetails>> GetAsync(
        Guid memberId,
        CancellationToken cancellationToken) =>
        Task.FromResult(AccessResult<CompanyMemberDetails>.Failure(AccessResultStatus.NotFound));
}

internal sealed class FakeProjectService(ICurrentUserContext currentUserContext) : IProjectService
{
    internal static readonly Guid ProjectId = Guid.Parse("c2f2fd54-b7dc-4d0f-9917-067751f42622");

    public Task<AccessResult<PagedResult<ProjectDetails>>> ListAsync(
        ProjectQuery query,
        CancellationToken cancellationToken) =>
        Task.FromResult(AccessResult<PagedResult<ProjectDetails>>.Success(
            new PagedResult<ProjectDetails>(
                [Project(VisibleContractValue())],
                query.Page,
                query.PageSize,
                1,
                1)));

    public Task<AccessResult<ProjectDetails>> GetAsync(
        Guid projectId,
        CancellationToken cancellationToken) =>
        Task.FromResult(projectId == ProjectId
            ? AccessResult<ProjectDetails>.Success(Project(VisibleContractValue()))
            : AccessResult<ProjectDetails>.Failure(AccessResultStatus.NotFound));

    public Task<AccessResult<ProjectDetails>> CreateAsync(
        CreateProjectRequest request,
        CancellationToken cancellationToken) =>
        Task.FromResult(AccessResult<ProjectDetails>.Success(Project(request.ContractValue)));

    public Task<AccessResult<ProjectDetails>> UpdateAsync(
        Guid projectId,
        UpdateProjectRequest request,
        CancellationToken cancellationToken) =>
        GetAsync(projectId, cancellationToken);

    public Task<AccessResult<ProjectDetails>> AssignSupervisorAsync(
        Guid projectId,
        AssignProjectSupervisorRequest request,
        CancellationToken cancellationToken) =>
        GetAsync(projectId, cancellationToken);

    public Task<AccessResult<PagedResult<ProjectMemberSummary>>> ListMembersAsync(
        Guid projectId,
        ProjectMemberQuery query,
        CancellationToken cancellationToken) =>
        Task.FromResult(currentUserContext.Role == AccessConstants.AccountantRole
            ? AccessResult<PagedResult<ProjectMemberSummary>>.Failure(AccessResultStatus.Forbidden)
            : AccessResult<PagedResult<ProjectMemberSummary>>.Success(
                new PagedResult<ProjectMemberSummary>([], query.Page, query.PageSize, 0, 0)));

    private decimal? VisibleContractValue() =>
        currentUserContext.Role == IdentityConstants.ManagerRole ? 1250.25m : null;

    private static ProjectDetails Project(decimal? contractValue)
    {
        var now = DateTimeOffset.Parse("2026-08-01T00:00:00Z");
        return new ProjectDetails(
            ProjectId,
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
}
