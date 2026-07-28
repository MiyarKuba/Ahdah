using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Ahdah.Application.Abstractions.Authentication;
using Ahdah.Application.Access;
using Ahdah.Application.Access.Contracts;
using Ahdah.Application.Access.Models;
using Ahdah.Application.Access.Services;
using Ahdah.Application.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Ahdah.IntegrationTests;

public sealed class AccessApiTests(IdentityApiFactory factory)
    : IClassFixture<IdentityApiFactory>
{
    [Fact]
    public async Task Invitation_creation_requires_manager_and_succeeds_through_fake_service()
    {
        using var anonymous = factory.CreateSecureClient();
        using var worker = CreateAuthenticatedClient("Worker");
        using var manager = CreateAuthenticatedClient(IdentityConstants.ManagerRole);
        var request = new CreateInvitationRequest("+218912345678", "Worker");

        var anonymousResponse = await anonymous.PostAsJsonAsync("/api/v1/invitations", request);
        var workerResponse = await worker.PostAsJsonAsync("/api/v1/invitations", request);
        var managerResponse = await manager.PostAsJsonAsync("/api/v1/invitations", request);
        var result = await managerResponse.Content.ReadFromJsonAsync<CreateInvitationResult>();

        Assert.Equal(HttpStatusCode.Unauthorized, anonymousResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, workerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Created, managerResponse.StatusCode);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task Join_request_submission_is_public_and_rejects_manager_role()
    {
        using var client = factory.CreateSecureClient();
        var valid = new SubmitJoinRequestRequest(
            "TEST01",
            "Applicant",
            "+218912345679",
            null,
            "a sufficiently long password",
            "Worker",
            null);
        var managerRole = valid with { RequestedRole = "Manager" };

        var validResponse = await client.PostAsJsonAsync("/api/v1/join-requests", valid);
        var invalidResponse = await client.PostAsJsonAsync("/api/v1/join-requests", managerRole);

        Assert.Equal(HttpStatusCode.Accepted, validResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, invalidResponse.StatusCode);
    }

    [Fact]
    public async Task Invitation_listing_returns_401_without_authentication()
    {
        using var client = factory.CreateSecureClient();

        var response = await client.GetAsync("/api/v1/invitations");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Invitation_listing_returns_403_for_non_manager()
    {
        using var client = CreateAuthenticatedClient("Worker");

        var response = await client.GetAsync("/api/v1/invitations");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Invitation_listing_succeeds_for_manager_through_fake_service()
    {
        using var client = CreateAuthenticatedClient(IdentityConstants.ManagerRole);

        var response = await client.GetAsync("/api/v1/invitations?page=1&pageSize=20&status=Pending");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Invitation_acceptance_remains_public()
    {
        using var client = factory.CreateSecureClient();
        var request = new AcceptInvitationRequest(
            new string('a', 43),
            "Invited User",
            "a sufficiently long password",
            null);

        var response = await client.PostAsJsonAsync("/api/v1/invitations/accept", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Join_request_listing_requires_manager_authentication()
    {
        using var anonymous = factory.CreateSecureClient();
        using var worker = CreateAuthenticatedClient("Worker");
        using var manager = CreateAuthenticatedClient(IdentityConstants.ManagerRole);

        var anonymousResponse = await anonymous.GetAsync("/api/v1/join-requests");
        var workerResponse = await worker.GetAsync("/api/v1/join-requests");
        var managerResponse = await manager.GetAsync("/api/v1/join-requests");

        Assert.Equal(HttpStatusCode.Unauthorized, anonymousResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, workerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, managerResponse.StatusCode);
    }

    [Fact]
    public async Task Approval_rejects_manager_role_before_calling_service()
    {
        using var client = CreateAuthenticatedClient(IdentityConstants.ManagerRole);

        var response = await client.PostAsJsonAsync(
            $"/api/v1/join-requests/{Guid.NewGuid()}/approve",
            new ApproveJoinRequestRequest("Manager", null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Approval_and_rejection_are_forbidden_for_non_managers()
    {
        using var client = CreateAuthenticatedClient("Worker");
        var requestId = Guid.NewGuid();

        var approval = await client.PostAsJsonAsync(
            $"/api/v1/join-requests/{requestId}/approve",
            new ApproveJoinRequestRequest("Worker", null));
        var rejection = await client.PostAsJsonAsync(
            $"/api/v1/join-requests/{requestId}/reject",
            new RejectJoinRequestRequest("Not eligible"));

        Assert.Equal(HttpStatusCode.Forbidden, approval.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, rejection.StatusCode);
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
}

internal sealed class FakeInvitationService : IInvitationService
{
    public Task<AccessResult<CreateInvitationResult>> CreateAsync(
        CreateInvitationRequest request,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.Parse("2026-07-28T00:00:00Z");
        var invitation = new InvitationSummary(
            Guid.NewGuid(),
            request.PhoneNumber,
            request.AssignedRole,
            "Pending",
            now.AddDays(7),
            null,
            null,
            now,
            now);
        return Task.FromResult(AccessResult<CreateInvitationResult>.Success(
            new CreateInvitationResult(invitation, "fake-one-time-invitation-token-value-123456")));
    }

    public Task<AccessResult<PagedResult<InvitationSummary>>> ListAsync(
        InvitationQuery query,
        CancellationToken cancellationToken) =>
        Task.FromResult(AccessResult<PagedResult<InvitationSummary>>.Success(
            new PagedResult<InvitationSummary>([], query.Page, query.PageSize, 0, 0)));

    public Task<AccessResult<InvitationAcceptanceResult>> AcceptAsync(
        AcceptInvitationRequest request,
        CancellationToken cancellationToken) =>
        Task.FromResult(AccessResult<InvitationAcceptanceResult>.Success(
            new InvitationAcceptanceResult(
                AccessConstants.AcceptedAndActivatedOutcome,
                "Active",
                "Worker",
                null)));

    public Task<AccessResult<InvitationSummary>> CancelAsync(
        Guid invitationId,
        CancellationToken cancellationToken) =>
        Task.FromResult(AccessResult<InvitationSummary>.Failure(AccessResultStatus.NotFound));
}

internal sealed class FakeJoinRequestService : IJoinRequestService
{
    public Task<AccessResult<JoinRequestSubmissionResult>> SubmitAsync(
        SubmitJoinRequestRequest request,
        CancellationToken cancellationToken) =>
        Task.FromResult(AccessResult<JoinRequestSubmissionResult>.Success(
            new JoinRequestSubmissionResult("Pending")));

    public Task<AccessResult<PagedResult<JoinRequestSummary>>> ListAsync(
        JoinRequestQuery query,
        CancellationToken cancellationToken) =>
        Task.FromResult(AccessResult<PagedResult<JoinRequestSummary>>.Success(
            new PagedResult<JoinRequestSummary>([], query.Page, query.PageSize, 0, 0)));

    public Task<AccessResult<JoinRequestDecisionResult>> ApproveAsync(
        Guid joinRequestId,
        ApproveJoinRequestRequest request,
        CancellationToken cancellationToken) =>
        Task.FromResult(AccessResult<JoinRequestDecisionResult>.Failure(AccessResultStatus.NotFound));

    public Task<AccessResult<JoinRequestDecisionResult>> RejectAsync(
        Guid joinRequestId,
        RejectJoinRequestRequest request,
        CancellationToken cancellationToken) =>
        Task.FromResult(AccessResult<JoinRequestDecisionResult>.Failure(AccessResultStatus.NotFound));
}
