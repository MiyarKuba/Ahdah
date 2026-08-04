using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Ahdah.Application.Abstractions.Authentication;
using Ahdah.Application.Abstractions.Context;
using Ahdah.Application.Access;
using Ahdah.Application.Access.Models;
using Ahdah.Application.Expenses;
using Ahdah.Application.Expenses.Contracts;
using Ahdah.Application.Expenses.Models;
using Ahdah.Application.Expenses.Services;
using Ahdah.Application.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Ahdah.IntegrationTests;

public sealed class ExpenseApiTests(IdentityApiFactory factory) : IClassFixture<IdentityApiFactory>
{
    [Fact]
    public async Task Expense_list_requires_authentication()
    {
        using var client = factory.CreateSecureClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/v1/expenses")).StatusCode);
    }

    [Theory]
    [InlineData("Manager", HttpStatusCode.OK)]
    [InlineData("Deputy", HttpStatusCode.OK)]
    [InlineData("Accountant", HttpStatusCode.OK)]
    [InlineData("Supervisor", HttpStatusCode.OK)]
    [InlineData("Worker", HttpStatusCode.OK)]
    [InlineData("FutureRole", HttpStatusCode.Forbidden)]
    public async Task Expense_viewer_policy_uses_exact_roles(string role, HttpStatusCode expected)
    {
        using var client = CreateAuthenticatedClient(role);
        Assert.Equal(expected, (await client.GetAsync("/api/v1/expenses")).StatusCode);
    }

    [Theory]
    [InlineData("Manager", HttpStatusCode.Created)]
    [InlineData("Deputy", HttpStatusCode.Created)]
    [InlineData("Supervisor", HttpStatusCode.Created)]
    [InlineData("Worker", HttpStatusCode.Created)]
    [InlineData("Accountant", HttpStatusCode.Forbidden)]
    public async Task Expense_creation_policy_is_role_aware(string role, HttpStatusCode expected)
    {
        using var client = CreateAuthenticatedClient(role);
        var response = await client.PostAsJsonAsync("/api/v1/expenses", ValidCreateRequest());
        Assert.Equal(expected, response.StatusCode);
    }

    [Fact]
    public async Task Financial_creation_requires_idempotency_key()
    {
        using var client = CreateAuthenticatedClient(IdentityConstants.ManagerRole, false);
        var response = await client.PostAsJsonAsync("/api/v1/expenses", ValidCreateRequest());
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("expenses.invalid_idempotency_key", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Expense_response_is_safe_and_contains_no_storage_or_tenant_internals()
    {
        using var client = CreateAuthenticatedClient(IdentityConstants.ManagerRole);
        var body = await (await client.GetAsync($"/api/v1/expenses/{FakeExpenseService.ExpenseId}"))
            .Content.ReadAsStringAsync();

        Assert.Contains("PendingReview", body, StringComparison.Ordinal);
        Assert.DoesNotContain("companyId", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("fileUrl", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("sha256", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Category_creation_is_manager_only()
    {
        using var manager = CreateAuthenticatedClient(IdentityConstants.ManagerRole);
        using var deputy = CreateAuthenticatedClient(AccessConstants.DeputyRole);
        var request = new CreateExpenseCategoryRequest
        {
            CategoryName = "Materials",
            CategoryGroup = "Materials",
            ExpenseScope = "ProjectOnly"
        };

        Assert.Equal(HttpStatusCode.Created,
            (await manager.PostAsJsonAsync("/api/v1/expense-categories", request)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,
            (await deputy.PostAsJsonAsync("/api/v1/expense-categories", request)).StatusCode);
    }

    [Fact]
    public async Task Review_routes_are_manager_deputy_accountant_only()
    {
        using var accountant = CreateAuthenticatedClient(AccessConstants.AccountantRole);
        using var worker = CreateAuthenticatedClient(AccessConstants.WorkerRole);
        var request = new ApproveExpenseRequest { ExpectedVersion = 1 };

        Assert.Equal(HttpStatusCode.OK,
            (await accountant.PostAsJsonAsync(
                $"/api/v1/expenses/{FakeExpenseService.ExpenseId}/approve", request)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,
            (await worker.PostAsJsonAsync(
                $"/api/v1/expenses/{FakeExpenseService.ExpenseId}/approve", request)).StatusCode);
    }

    [Fact]
    public async Task Attachment_command_is_metadata_only_and_requires_idempotency()
    {
        using var client = CreateAuthenticatedClient(AccessConstants.WorkerRole, false);
        var response = await client.PostAsJsonAsync(
            $"/api/v1/expenses/{FakeExpenseService.ExpenseId}/attachments",
            new AddExpenseDocumentRequest
            {
                DocumentType = "Receipt",
                OriginalFileName = "receipt.pdf",
                FileUrl = "/uploads/receipt.pdf",
                MimeType = "application/pdf",
                FileSizeBytes = 100,
                Sha256Hash = new string('a', 64),
                CaptureSource = "FileUpload"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task OpenApi_publishes_expense_dtos_and_omits_unsupported_workflows()
    {
        using var client = factory.CreateSecureClient();
        var body = await (await client.GetAsync("/openapi/v1.json")).Content.ReadAsStringAsync();

        Assert.Contains("/api/v1/expenses", body, StringComparison.Ordinal);
        Assert.Contains("/api/v1/expense-categories", body, StringComparison.Ordinal);
        Assert.Contains("/api/v1/reimbursements", body, StringComparison.Ordinal);
        Assert.Contains(nameof(ExpenseDetails), body, StringComparison.Ordinal);
        Assert.DoesNotContain("Ahdah.Infrastructure.Persistence.Generated", body, StringComparison.Ordinal);
        Assert.DoesNotContain("original-status", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("supplier-debts", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("advance-settlements", body, StringComparison.OrdinalIgnoreCase);
    }

    private HttpClient CreateAuthenticatedClient(string role, bool includeIdempotencyKey = true)
    {
        var client = factory.CreateSecureClient();
        var tokenService = factory.Services.GetRequiredService<IAccessTokenService>();
        var token = tokenService.CreateToken(new AccessTokenSubject(
            IdentityApiFactory.UserId,
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

    private static CreateExpenseRequest ValidCreateRequest() => new()
    {
        ExpenseCategoryId = FakeExpenseService.CategoryId,
        ExpenseDate = new DateOnly(2026, 8, 4),
        Amount = 25m,
        CurrencyCode = "LYD",
        PaymentMode = ExpenseConstants.PersonalFundsPaymentMode,
        Description = "Personal project purchase"
    };
}

internal sealed class FakeExpenseService(ICurrentUserContext currentUserContext) : IExpenseService
{
    internal static readonly Guid ExpenseId = Guid.Parse("f52252b0-7fc4-4e12-8344-58d0e1d4c9d5");
    internal static readonly Guid CategoryId = Guid.Parse("2e265aa8-8b89-48dc-9d36-bc1c74bb3286");
    private static readonly Guid ClaimId = Guid.Parse("b39055c8-2372-4219-a910-af805e6e3833");
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-08-04T10:00:00Z");

    public Task<AccessResult<PagedResult<ExpenseCategorySummary>>> ListCategoriesAsync(
        ExpenseCategoryQuery query, CancellationToken cancellationToken) => Task.FromResult(
        AccessResult<PagedResult<ExpenseCategorySummary>>.Success(new(
            [Category()], query.Page, query.PageSize, 1, 1)));

    public Task<AccessResult<ExpenseCategorySummary>> CreateCategoryAsync(
        CreateExpenseCategoryRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(AccessResult<ExpenseCategorySummary>.Success(Category()));

    public Task<AccessResult<PagedResult<ExpenseSummary>>> ListAsync(
        ExpenseQuery query, CancellationToken cancellationToken) => Task.FromResult(
        AccessResult<PagedResult<ExpenseSummary>>.Success(new(
            [Summary()], query.Page, query.PageSize, 1, 1)));

    public Task<AccessResult<ExpenseDetails>> GetAsync(Guid expenseId, CancellationToken cancellationToken) =>
        Task.FromResult(expenseId == ExpenseId
            ? AccessResult<ExpenseDetails>.Success(Details())
            : AccessResult<ExpenseDetails>.Failure(AccessResultStatus.NotFound));

    public Task<AccessResult<ExpenseDetails>> CreateAsync(
        CreateExpenseRequest request, string? idempotencyKey, CancellationToken cancellationToken) =>
        Task.FromResult(AccessResult<ExpenseDetails>.Success(Details()));

    public Task<AccessResult<ExpenseDetails>> ApproveAsync(
        Guid expenseId, ApproveExpenseRequest request, string? idempotencyKey,
        CancellationToken cancellationToken) => Task.FromResult(
        AccessResult<ExpenseDetails>.Success(Details(ExpenseConstants.ApprovedStatus)));

    public Task<AccessResult<ExpenseDetails>> RejectAsync(
        Guid expenseId, RejectExpenseRequest request, string? idempotencyKey,
        CancellationToken cancellationToken) => Task.FromResult(
        AccessResult<ExpenseDetails>.Success(Details(ExpenseConstants.RejectedStatus)));

    public Task<AccessResult<PagedResult<ExpenseAdvanceAllocationSummary>>> ListAllocationsAsync(
        Guid expenseId, ExpenseSubresourceQuery query, CancellationToken cancellationToken) =>
        Task.FromResult(AccessResult<PagedResult<ExpenseAdvanceAllocationSummary>>.Success(new(
            [], query.Page, query.PageSize, 0, 0)));

    public Task<AccessResult<PagedResult<ExpenseDocumentSummary>>> ListDocumentsAsync(
        Guid expenseId, ExpenseSubresourceQuery query, CancellationToken cancellationToken) =>
        Task.FromResult(AccessResult<PagedResult<ExpenseDocumentSummary>>.Success(new(
            [], query.Page, query.PageSize, 0, 0)));

    public Task<AccessResult<ExpenseDocumentSummary>> AddDocumentAsync(
        Guid expenseId, AddExpenseDocumentRequest request, string? idempotencyKey,
        CancellationToken cancellationToken) => Task.FromResult(
        AccessResult<ExpenseDocumentSummary>.Success(new(
            Guid.NewGuid(), request.DocumentType, request.DocumentNumber, request.DocumentDate,
            request.IssuerName, request.OriginalFileName, request.MimeType, request.FileSizeBytes,
            request.CaptureSource, request.IsPrimary, ExpenseConstants.PendingVerificationStatus,
            User(), null, Now, null, null, request.Notes, 1)));

    public Task<AccessResult<PagedResult<ExpenseHistoryEntry>>> ListHistoryAsync(
        Guid expenseId, ExpenseHistoryQuery query, CancellationToken cancellationToken) =>
        Task.FromResult(AccessResult<PagedResult<ExpenseHistoryEntry>>.Success(new(
            [], query.Page, query.PageSize, 0, 0)));

    public Task<AccessResult<PagedResult<ReimbursementSummary>>> ListReimbursementsAsync(
        ReimbursementQuery query, CancellationToken cancellationToken) => Task.FromResult(
        AccessResult<PagedResult<ReimbursementSummary>>.Success(new(
            [Reimbursement()], query.Page, query.PageSize, 1, 1)));

    public Task<AccessResult<ReimbursementSummary>> GetReimbursementAsync(
        Guid reimbursementId, CancellationToken cancellationToken) => Task.FromResult(
        reimbursementId == ClaimId
            ? AccessResult<ReimbursementSummary>.Success(Reimbursement())
            : AccessResult<ReimbursementSummary>.Failure(AccessResultStatus.NotFound));

    private ExpenseUserSummary User() => new(
        IdentityApiFactory.UserId, "Personal User", currentUserContext.Role ?? AccessConstants.WorkerRole);

    private static ExpenseCategorySummary Category() => new(
        CategoryId, null, "MAT", "Materials", "Materials", "Both", null,
        false, false, false, true, 0, 1);

    private ExpenseSummary Summary(string status = ExpenseConstants.PendingReviewStatus) => new(
        ExpenseId, "EXP-TEST", new DateOnly(2026, 8, 4), 25m, "LYD",
        ExpenseConstants.PersonalFundsPaymentMode, "Personal project purchase", status,
        Category(), null, User(), User(), null, null, false, ExpenseConstants.OpenClaimStatus,
        1, Now, Now, null);

    private ExpenseDetails Details(string status = ExpenseConstants.PendingReviewStatus) => new(
        Summary(status), 25m, 0m, 0m, null, null, null, null, null, null,
        [], [], [], Reimbursement());

    private ReimbursementSummary Reimbursement() => new(
        ClaimId, "CLM-TEST", ExpenseId, "EXP-TEST", User(), null,
        new DateOnly(2026, 8, 4), null, 25m, 25m, "LYD",
        "Personal project purchase", ExpenseConstants.OpenClaimStatus, 1, Now);
}
