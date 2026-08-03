using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Ahdah.Application.Abstractions.Authentication;
using Ahdah.Application.Abstractions.Context;
using Ahdah.Application.Access;
using Ahdah.Application.Access.Models;
using Ahdah.Application.Advances;
using Ahdah.Application.Advances.Contracts;
using Ahdah.Application.Advances.Models;
using Ahdah.Application.Advances.Services;
using Ahdah.Application.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Ahdah.IntegrationTests;

public sealed class AdvanceApiTests(IdentityApiFactory factory) : IClassFixture<IdentityApiFactory>
{
    [Fact]
    public async Task Advance_list_requires_authentication()
    {
        using var client = factory.CreateSecureClient();

        var response = await client.GetAsync("/api/v1/advances");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("Manager", HttpStatusCode.OK)]
    [InlineData("Deputy", HttpStatusCode.OK)]
    [InlineData("Accountant", HttpStatusCode.OK)]
    [InlineData("Supervisor", HttpStatusCode.OK)]
    [InlineData("Worker", HttpStatusCode.OK)]
    [InlineData("FutureRole", HttpStatusCode.Forbidden)]
    public async Task Advance_viewer_policy_uses_exact_known_roles(
        string role,
        HttpStatusCode expectedStatus)
    {
        using var client = CreateAuthenticatedClient(role);

        var response = await client.GetAsync("/api/v1/advances?page=1&pageSize=20");

        Assert.Equal(expectedStatus, response.StatusCode);
    }

    [Fact]
    public async Task Worker_list_response_contains_only_personal_fake_record()
    {
        using var client = CreateAuthenticatedClient(AccessConstants.WorkerRole);

        var body = await (await client.GetAsync("/api/v1/advances")).Content.ReadAsStringAsync();

        Assert.Contains("Personal Recipient", body, StringComparison.Ordinal);
        Assert.DoesNotContain("Unrelated Holder", body, StringComparison.Ordinal);
        Assert.DoesNotContain("companyId", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Manager_may_create_top_level_advance()
    {
        using var client = CreateAuthenticatedClient(IdentityConstants.ManagerRole);

        var response = await client.PostAsJsonAsync("/api/v1/advances", ValidCreateRequest());
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Contains("PendingConfirmation", body, StringComparison.Ordinal);
        Assert.DoesNotContain("createdByUserId", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Financial_command_requires_idempotency_key()
    {
        using var client = CreateAuthenticatedClient(
            IdentityConstants.ManagerRole,
            includeIdempotencyKey: false);

        var response = await client.PostAsJsonAsync("/api/v1/advances", ValidCreateRequest());
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("advances.invalid_idempotency_key", body, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("Deputy")]
    [InlineData("Accountant")]
    [InlineData("Supervisor")]
    [InlineData("Worker")]
    public async Task Non_managers_cannot_create_top_level_advance(string role)
    {
        using var client = CreateAuthenticatedClient(role);

        var response = await client.PostAsJsonAsync("/api/v1/advances", ValidCreateRequest());

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Accountant_cannot_distribute_but_deputy_can_access_route()
    {
        using var accountant = CreateAuthenticatedClient(AccessConstants.AccountantRole);
        using var deputy = CreateAuthenticatedClient(AccessConstants.DeputyRole);
        var request = ValidDistributionRequest();

        var denied = await accountant.PostAsJsonAsync(
            $"/api/v1/advances/{FakeAdvanceService.AdvanceId}/distributions",
            request);
        var allowed = await deputy.PostAsJsonAsync(
            $"/api/v1/advances/{FakeAdvanceService.AdvanceId}/distributions",
            request);

        Assert.Equal(HttpStatusCode.Forbidden, denied.StatusCode);
        Assert.Equal(HttpStatusCode.Created, allowed.StatusCode);
    }

    [Fact]
    public async Task Confirmation_requires_expected_recipient_and_accountant_is_read_only()
    {
        using var expectedRecipient = CreateAuthenticatedClient(
            AccessConstants.WorkerRole,
            FakeAdvanceService.RecipientUserId);
        using var anotherWorker = CreateAuthenticatedClient(
            AccessConstants.WorkerRole,
            Guid.NewGuid());
        using var accountant = CreateAuthenticatedClient(AccessConstants.AccountantRole);

        var confirmed = await expectedRecipient.PostAsync(
            $"/api/v1/advance-transfers/{FakeAdvanceService.TransferId}/confirm",
            null);
        var hidden = await anotherWorker.PostAsync(
            $"/api/v1/advance-transfers/{FakeAdvanceService.TransferId}/confirm",
            null);
        var denied = await accountant.PostAsync(
            $"/api/v1/advance-transfers/{FakeAdvanceService.TransferId}/confirm",
            null);

        Assert.Equal(HttpStatusCode.OK, confirmed.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, hidden.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, denied.StatusCode);
    }

    [Fact]
    public async Task Holder_return_route_is_available_to_worker()
    {
        using var client = CreateAuthenticatedClient(AccessConstants.WorkerRole);

        var response = await client.PostAsJsonAsync(
            $"/api/v1/advances/{FakeAdvanceService.AdvanceId}/returns",
            ValidReturnRequest());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Balance_me_requires_authentication_and_user_lookup_is_role_limited()
    {
        using var anonymous = factory.CreateSecureClient();
        using var manager = CreateAuthenticatedClient(IdentityConstants.ManagerRole);
        using var worker = CreateAuthenticatedClient(AccessConstants.WorkerRole);

        var unauthorized = await anonymous.GetAsync("/api/v1/advance-balances/me");
        var managerLookup = await manager.GetAsync(
            $"/api/v1/advance-balances/users/{FakeAdvanceService.RecipientUserId}");
        var workerLookup = await worker.GetAsync(
            $"/api/v1/advance-balances/users/{FakeAdvanceService.RecipientUserId}");

        Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);
        Assert.Equal(HttpStatusCode.OK, managerLookup.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, workerLookup.StatusCode);
    }

    [Fact]
    public async Task OpenApi_publishes_advance_dtos_without_generated_entities_or_write_closure_routes()
    {
        using var client = factory.CreateSecureClient();

        var body = await (await client.GetAsync("/openapi/v1.json")).Content.ReadAsStringAsync();

        Assert.Contains("/api/v1/advances", body, StringComparison.Ordinal);
        Assert.Contains("/api/v1/advance-transfers/{transferId}/confirm", body, StringComparison.Ordinal);
        Assert.Contains("/api/v1/advance-balances/me", body, StringComparison.Ordinal);
        Assert.Contains(nameof(AdvanceDetails), body, StringComparison.Ordinal);
        Assert.DoesNotContain("Ahdah.Infrastructure.Persistence.Generated", body, StringComparison.Ordinal);
        Assert.DoesNotContain("advance-settlements", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("advance-closures", body, StringComparison.OrdinalIgnoreCase);
    }

    private HttpClient CreateAuthenticatedClient(
        string role,
        Guid? userId = null,
        bool includeIdempotencyKey = true)
    {
        var client = factory.CreateSecureClient();
        var tokenService = factory.Services.GetRequiredService<IAccessTokenService>();
        var token = tokenService.CreateToken(new AccessTokenSubject(
            userId ?? IdentityApiFactory.UserId,
            IdentityApiFactory.CompanyId,
            role,
            "Test User"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        if (includeIdempotencyKey)
        {
            client.DefaultRequestHeaders.Add("Idempotency-Key", Guid.NewGuid().ToString("N"));
        }

        return client;
    }

    private static CreateAdvanceRequest ValidCreateRequest() => new()
    {
        RecipientUserId = FakeAdvanceService.RecipientUserId,
        Amount = 100m,
        IssueDate = new DateOnly(2026, 8, 3),
        TransferDate = new DateOnly(2026, 8, 3),
        TransferMethod = "Cash",
        Purpose = "Project custody",
        Fundings =
        [
            new AdvanceFundingAllocationRequest
            {
                FundingSourceId = Guid.NewGuid(),
                Amount = 100m
            }
        ]
    };

    private static CreateAdvanceDistributionRequest ValidDistributionRequest() => new()
    {
        RecipientUserId = FakeAdvanceService.RecipientUserId,
        Amount = 10m,
        TransferDate = new DateOnly(2026, 8, 3),
        TransferMethod = "Cash"
    };

    private static CreateAdvanceReturnRequest ValidReturnRequest() => new()
    {
        Amount = 10m,
        TransferDate = new DateOnly(2026, 8, 3),
        TransferMethod = "Cash"
    };
}

internal sealed class FakeAdvanceService(ICurrentUserContext currentUserContext) : IAdvanceService
{
    internal static readonly Guid AdvanceId = Guid.Parse("7387c17b-e1e5-45c1-a63d-e969f1b92592");
    internal static readonly Guid TransferId = Guid.Parse("8a593e4b-3a69-4b25-867a-cef70c63f818");
    internal static readonly Guid RecipientUserId = IdentityApiFactory.UserId;
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-08-03T10:00:00Z");

    public Task<AccessResult<PagedResult<AdvanceSummary>>> ListAsync(
        AdvanceQuery query,
        CancellationToken cancellationToken) => Task.FromResult(
        AccessResult<PagedResult<AdvanceSummary>>.Success(new PagedResult<AdvanceSummary>(
            [Summary(currentUserContext.Role is AccessConstants.WorkerRole ? "Personal Recipient" : "Unrelated Holder")],
            query.Page,
            query.PageSize,
            1,
            1)));

    public Task<AccessResult<AdvanceDetails>> GetAsync(Guid advanceId, CancellationToken cancellationToken) =>
        Task.FromResult(advanceId == AdvanceId
            ? AccessResult<AdvanceDetails>.Success(Details())
            : AccessResult<AdvanceDetails>.Failure(AccessResultStatus.NotFound));

    public Task<AccessResult<PagedResult<AdvanceMovementSummary>>> ListMovementsAsync(
        Guid advanceId,
        AdvanceMovementQuery query,
        CancellationToken cancellationToken) => Task.FromResult(
        advanceId == AdvanceId
            ? AccessResult<PagedResult<AdvanceMovementSummary>>.Success(
                new PagedResult<AdvanceMovementSummary>([], query.Page, query.PageSize, 0, 0))
            : AccessResult<PagedResult<AdvanceMovementSummary>>.Failure(AccessResultStatus.NotFound));

    public Task<AccessResult<AdvanceDetails>> CreateAsync(
        CreateAdvanceRequest request,
        string? idempotencyKey,
        CancellationToken cancellationToken) => Task.FromResult(
        AccessResult<AdvanceDetails>.Success(Details()));

    public Task<AccessResult<AdvanceTransferDetails>> DistributeAsync(
        Guid advanceId,
        CreateAdvanceDistributionRequest request,
        string? idempotencyKey,
        CancellationToken cancellationToken) => Task.FromResult(
        AccessResult<AdvanceTransferDetails>.Success(Transfer()));

    public Task<AccessResult<AdvanceTransferDetails>> ReturnAsync(
        Guid advanceId,
        CreateAdvanceReturnRequest request,
        string? idempotencyKey,
        CancellationToken cancellationToken) => Task.FromResult(
        AccessResult<AdvanceTransferDetails>.Success(Transfer(AdvanceConstants.BalanceReturnTransferType)));

    public Task<AccessResult<AdvanceTransferDetails>> ConfirmTransferAsync(
        Guid transferId,
        string? idempotencyKey,
        CancellationToken cancellationToken) => Task.FromResult(
        transferId == TransferId && currentUserContext.UserId == RecipientUserId
            ? AccessResult<AdvanceTransferDetails>.Success(Transfer() with
                { Status = AdvanceConstants.ConfirmedTransferStatus })
            : AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.NotFound));

    public Task<AccessResult<AdvanceTransferDetails>> RejectTransferAsync(
        Guid transferId,
        RejectAdvanceTransferRequest request,
        string? idempotencyKey,
        CancellationToken cancellationToken) => Task.FromResult(
        currentUserContext.UserId == RecipientUserId
            ? AccessResult<AdvanceTransferDetails>.Success(Transfer() with
                { Status = AdvanceConstants.RejectedTransferStatus })
            : AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.NotFound));

    public Task<AccessResult<AdvanceBalancePage>> GetMyBalancesAsync(
        AdvanceBalanceQuery query,
        CancellationToken cancellationToken) => Task.FromResult(
        AccessResult<AdvanceBalancePage>.Success(BalancePage(query)));

    public Task<AccessResult<AdvanceBalancePage>> GetUserBalancesAsync(
        Guid userId,
        AdvanceBalanceQuery query,
        CancellationToken cancellationToken) => Task.FromResult(
        AccessResult<AdvanceBalancePage>.Success(BalancePage(query)));

    public Task<AccessResult<PagedResult<AvailableFundingSourceSummary>>>
        ListAvailableFundingSourcesAsync(
            AvailableFundingSourceQuery query,
            CancellationToken cancellationToken) => Task.FromResult(
            AccessResult<PagedResult<AvailableFundingSourceSummary>>.Success(
                new PagedResult<AvailableFundingSourceSummary>([], query.Page, query.PageSize, 0, 0)));

    private static AdvanceSummary Summary(string recipientName) => new(
        AdvanceId,
        "ADV-TEST",
        100m,
        0m,
        100m,
        "LYD",
        new DateOnly(2026, 8, 3),
        null,
        "Project custody",
        null,
        AdvanceConstants.PendingConfirmationStatus,
        new AdvanceUserSummary(RecipientUserId, recipientName, AccessConstants.DeputyRole),
        1,
        Now,
        null);

    private static AdvanceDetails Details() => new(Summary("Personal Recipient"), [], [], null);

    private static AdvanceTransferDetails Transfer(
        string transferType = AdvanceConstants.InternalTransferType) => new(
        TransferId,
        "TRF-TEST",
        transferType,
        AdvanceId,
        10m,
        "LYD",
        new AdvanceUserSummary(Guid.NewGuid(), "Sender", AccessConstants.DeputyRole),
        new AdvanceUserSummary(RecipientUserId, "Recipient", AccessConstants.WorkerRole),
        "Cash",
        AdvanceConstants.PendingConfirmationStatus,
        1,
        Now,
        null,
        null);

    private static AdvanceBalancePage BalancePage(AdvanceBalanceQuery query) => new(
        new PagedResult<AdvanceBalanceSummary>([], query.Page, query.PageSize, 0, 0),
        0m);
}
