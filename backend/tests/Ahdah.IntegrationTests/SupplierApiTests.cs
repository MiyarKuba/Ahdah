using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Ahdah.Application.Abstractions.Authentication;
using Ahdah.Application.Access;
using Ahdah.Application.Access.Models;
using Ahdah.Application.Identity;
using Ahdah.Application.Suppliers;
using Ahdah.Application.Suppliers.Contracts;
using Ahdah.Application.Suppliers.Models;
using Ahdah.Application.Suppliers.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Ahdah.IntegrationTests;

public sealed class SupplierApiTests(IdentityApiFactory factory) : IClassFixture<IdentityApiFactory>
{
    [Fact]
    public async Task Supplier_list_requires_authentication()
    {
        using var client = factory.CreateSecureClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/v1/suppliers")).StatusCode);
    }

    [Theory]
    [InlineData("Manager", HttpStatusCode.OK)]
    [InlineData("Deputy", HttpStatusCode.OK)]
    [InlineData("Accountant", HttpStatusCode.OK)]
    [InlineData("Supervisor", HttpStatusCode.OK)]
    [InlineData("Worker", HttpStatusCode.Forbidden)]
    [InlineData("FutureRole", HttpStatusCode.Forbidden)]
    public async Task Supplier_viewer_policy_uses_exact_roles(string role, HttpStatusCode expected)
    {
        using var client = Authenticated(role);
        Assert.Equal(expected, (await client.GetAsync("/api/v1/suppliers")).StatusCode);
    }

    [Fact]
    public async Task Supplier_creation_is_manager_only()
    {
        using var manager = Authenticated("Manager");
        using var accountant = Authenticated("Accountant");
        var request = new CreateSupplierRequest
        {
            SupplierName = "Safe Supplier", SupplierType = "GeneralSupplier",
            DefaultCurrencyCode = "LYD", TransactionMode = "CashAndCredit"
        };
        Assert.Equal(HttpStatusCode.Created,
            (await manager.PostAsJsonAsync("/api/v1/suppliers", request)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,
            (await accountant.PostAsJsonAsync("/api/v1/suppliers", request)).StatusCode);
    }

    [Theory]
    [InlineData("Manager", HttpStatusCode.Created)]
    [InlineData("Deputy", HttpStatusCode.Created)]
    [InlineData("Accountant", HttpStatusCode.Created)]
    [InlineData("Supervisor", HttpStatusCode.Forbidden)]
    public async Task Invoice_creation_policy_is_role_aware(string role, HttpStatusCode expected)
    {
        using var client = Authenticated(role);
        var response = await client.PostAsJsonAsync("/api/v1/supplier-invoices", ValidInvoice());
        Assert.Equal(expected, response.StatusCode);
    }

    [Theory]
    [InlineData("Manager", HttpStatusCode.Created)]
    [InlineData("Accountant", HttpStatusCode.Created)]
    [InlineData("Deputy", HttpStatusCode.Forbidden)]
    [InlineData("Supervisor", HttpStatusCode.Forbidden)]
    public async Task Payment_creation_is_manager_or_accountant(string role, HttpStatusCode expected)
    {
        using var client = Authenticated(role);
        var response = await client.PostAsJsonAsync("/api/v1/supplier-payments", ValidPayment());
        Assert.Equal(expected, response.StatusCode);
    }

    [Fact]
    public async Task Financial_commands_require_idempotency_key()
    {
        using var client = Authenticated("Manager", false);
        var response = await client.PostAsJsonAsync("/api/v1/supplier-payments", ValidPayment());
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("suppliers.invalid_idempotency_key", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Payment_accounts_are_masked_and_tenant_internals_are_absent()
    {
        using var client = Authenticated("Manager");
        var body = await (await client.GetAsync(
            $"/api/v1/suppliers/{FakeSupplierService.SupplierId}/payment-accounts")).Content.ReadAsStringAsync();
        Assert.Contains("****3456", body, StringComparison.Ordinal);
        Assert.DoesNotContain("1234567890123456", body, StringComparison.Ordinal);
        Assert.DoesNotContain("companyId", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Refund_creation_is_not_published_without_expense_return_workflow()
    {
        using var client = Authenticated("Manager");
        Assert.Equal(HttpStatusCode.MethodNotAllowed,
            (await client.PostAsJsonAsync("/api/v1/supplier-refunds", new { amount = 1m })).StatusCode);
    }

    [Fact]
    public async Task OpenApi_publishes_safe_supplier_contracts()
    {
        using var client = factory.CreateSecureClient();
        var body = await (await client.GetAsync("/openapi/v1.json")).Content.ReadAsStringAsync();
        Assert.Contains("/api/v1/suppliers", body, StringComparison.Ordinal);
        Assert.Contains("/api/v1/supplier-invoices", body, StringComparison.Ordinal);
        Assert.Contains("/api/v1/supplier-debts", body, StringComparison.Ordinal);
        Assert.Contains("/api/v1/supplier-payments", body, StringComparison.Ordinal);
        Assert.Contains("/api/v1/supplier-credit-notes", body, StringComparison.Ordinal);
        Assert.Contains(nameof(SupplierPaymentDetails), body, StringComparison.Ordinal);
        Assert.DoesNotContain("Ahdah.Infrastructure.Persistence.Generated", body, StringComparison.Ordinal);
    }

    private HttpClient Authenticated(string role, bool idempotency = true)
    {
        var client = factory.CreateSecureClient();
        var token = factory.Services.GetRequiredService<IAccessTokenService>().CreateToken(new AccessTokenSubject(
            IdentityApiFactory.UserId, IdentityApiFactory.CompanyId, role, "Test User"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        if (idempotency) client.DefaultRequestHeaders.Add("Idempotency-Key", Guid.NewGuid().ToString("N"));
        return client;
    }

    private static CreateSupplierInvoiceRequest ValidInvoice() => new()
    {
        SupplierId = FakeSupplierService.SupplierId, ExpenseCategoryId = Guid.NewGuid(),
        InvoiceDate = new DateOnly(2026, 8, 4), DueDate = new DateOnly(2026, 9, 4),
        Amount = 100m, CurrencyCode = "LYD", InvoiceNumber = "INV-1", Description = "Materials"
    };

    private static CreateSupplierPaymentRequest ValidPayment() => new()
    {
        SupplierId = FakeSupplierService.SupplierId, PaymentDate = new DateOnly(2026, 8, 4),
        PaymentAmount = 100m, CurrencyCode = "LYD", PaymentMethod = "Cash",
        DebtAllocations = [new() { SupplierDebtId = FakeSupplierService.DebtId, Amount = 100m }],
        FundingAllocations = [new() { FundingSourceId = Guid.NewGuid(), Amount = 100m }]
    };
}

internal sealed class FakeSupplierService : ISupplierService
{
    internal static readonly Guid SupplierId = Guid.Parse("b75b1a64-746c-45aa-976d-b8cf8448f272");
    internal static readonly Guid DebtId = Guid.Parse("e3dffef2-a875-44cb-a4e8-7770350dbbd4");
    private static readonly Guid PaymentId = Guid.Parse("18d4e35d-4d7c-485f-b7b6-c251b9d0a120");
    private static readonly Guid CreditId = Guid.Parse("26532d56-ed21-450e-af10-b670b5badb89");
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-08-04T10:00:00Z");

    public Task<AccessResult<PagedResult<SupplierSummary>>> ListSuppliersAsync(SupplierQuery query, CancellationToken token) =>
        Result(new PagedResult<SupplierSummary>([Supplier()], query.Page, query.PageSize, 1, 1));
    public Task<AccessResult<SupplierDetails>> GetSupplierAsync(Guid id, CancellationToken token) =>
        Result(new SupplierDetails(Supplier(), null, null, null, null, null, [new("LYD", 100m, 1)]));
    public Task<AccessResult<SupplierDetails>> CreateSupplierAsync(CreateSupplierRequest request, CancellationToken token) =>
        GetSupplierAsync(SupplierId, token);
    public Task<AccessResult<SupplierDetails>> UpdateSupplierAsync(Guid id, UpdateSupplierRequest request, CancellationToken token) =>
        GetSupplierAsync(id, token);
    public Task<AccessResult<PagedResult<SupplierPaymentAccountSummary>>> ListPaymentAccountsAsync(Guid id, SupplierSubresourceQuery query, CancellationToken token) =>
        Result(new PagedResult<SupplierPaymentAccountSummary>([Account()], query.Page, query.PageSize, 1, 1));
    public Task<AccessResult<SupplierPaymentAccountSummary>> CreatePaymentAccountAsync(Guid id, CreateSupplierPaymentAccountRequest request, string? key, CancellationToken token) => Result(Account());
    public Task<AccessResult<PagedResult<SupplierInvoiceSummary>>> ListInvoicesAsync(SupplierInvoiceQuery query, CancellationToken token) =>
        Result(new PagedResult<SupplierInvoiceSummary>([Invoice()], query.Page, query.PageSize, 1, 1));
    public Task<AccessResult<SupplierInvoiceDetails>> GetInvoiceAsync(Guid id, CancellationToken token) => Result(new SupplierInvoiceDetails(Invoice(), 0m, null, []));
    public Task<AccessResult<SupplierInvoiceDetails>> CreateInvoiceAsync(CreateSupplierInvoiceRequest request, string? key, CancellationToken token) => GetInvoiceAsync(DebtId, token);
    public Task<AccessResult<PagedResult<SupplierPaymentSummary>>> ListPaymentsAsync(SupplierPaymentQuery query, CancellationToken token) =>
        Result(new PagedResult<SupplierPaymentSummary>([Payment()], query.Page, query.PageSize, 1, 1));
    public Task<AccessResult<SupplierPaymentDetails>> GetPaymentAsync(Guid id, CancellationToken token) => Result(PaymentDetails());
    public Task<AccessResult<SupplierPaymentDetails>> CreatePaymentAsync(CreateSupplierPaymentRequest request, string? key, CancellationToken token) => Result(PaymentDetails());
    public Task<AccessResult<SupplierPaymentDetails>> ConfirmPaymentAsync(Guid id, ReviewSupplierPaymentRequest request, string? key, CancellationToken token) => Result(PaymentDetails());
    public Task<AccessResult<SupplierPaymentDetails>> RejectPaymentAsync(Guid id, ReviewSupplierPaymentRequest request, string? key, CancellationToken token) => Result(PaymentDetails());
    public Task<AccessResult<PagedResult<SupplierRefundSummary>>> ListRefundsAsync(SupplierRefundQuery query, CancellationToken token) =>
        Result(new PagedResult<SupplierRefundSummary>([], query.Page, query.PageSize, 0, 0));
    public Task<AccessResult<PagedResult<SupplierCreditNoteSummary>>> ListCreditNotesAsync(SupplierCreditNoteQuery query, CancellationToken token) =>
        Result(new PagedResult<SupplierCreditNoteSummary>([Credit()], query.Page, query.PageSize, 1, 1));
    public Task<AccessResult<SupplierCreditNoteDetails>> GetCreditNoteAsync(Guid id, CancellationToken token) => Result(new SupplierCreditNoteDetails(Credit(), null, []));
    public Task<AccessResult<SupplierCreditNoteDetails>> CreateCreditNoteAsync(CreateSupplierCreditNoteRequest request, string? key, CancellationToken token) => GetCreditNoteAsync(CreditId, token);
    public Task<AccessResult<SupplierCreditNoteDetails>> ApproveCreditNoteAsync(Guid id, ApproveSupplierCreditNoteRequest request, string? key, CancellationToken token) => GetCreditNoteAsync(id, token);
    public Task<AccessResult<SupplierCreditNoteDetails>> ApplyCreditNoteAsync(Guid id, ApplySupplierCreditNoteRequest request, string? key, CancellationToken token) => GetCreditNoteAsync(id, token);
    public Task<AccessResult<SupplierStatement>> GetStatementAsync(Guid id, SupplierStatementQuery query, CancellationToken token) =>
        Result(new SupplierStatement(Supplier(), [new("LYD", 100m, 1)], new([], query.Page, query.PageSize, 0, 0)));

    private static Task<AccessResult<T>> Result<T>(T value) => Task.FromResult(AccessResult<T>.Success(value));
    private static SupplierSummary Supplier() => new(SupplierId, "SUP-1", "Safe Supplier", "GeneralSupplier",
        "Contact", "+218910000000", "safe@example.com", "Tripoli", "LYD", "CashAndCredit", 30,
        1000m, "Cash", true, 1, Now, Now);
    private static SupplierPaymentAccountSummary Account() => new(Guid.NewGuid(), "BankAccount", "Primary",
        "Safe Supplier", "Safe Bank", null, "****3456", "****3456", null, null, "LYD", false,
        "PendingVerification", true, null, 1, Now);
    private static SupplierInvoiceSummary Invoice() => new(DebtId, "SDEBT-1", Guid.NewGuid(), "EXP-1",
        "INV-1", Supplier(), null, new(2026, 8, 4), new(2026, 9, 4), 100m, 0m, 0m, 0m,
        100m, "LYD", "PendingReview", "Open", "Materials", 1, 1, Now);
    private static SupplierPaymentSummary Payment() => new(PaymentId, "SPAY-1", SupplierId, "Safe Supplier",
        new(2026, 8, 4), 100m, "LYD", "Cash", null, false, "Confirmed",
        new(IdentityApiFactory.UserId, "Test User", "Manager"), null, 1, Now, Now);
    private static SupplierPaymentDetails PaymentDetails() => new(Payment(), null, null, null, null, [], []);
    private static SupplierCreditNoteSummary Credit() => new(CreditId, "SCN-1", SupplierId, "Safe Supplier",
        null, new(2026, 8, 4), 10m, 0m, 10m, "LYD", "AdditionalDiscount", "Discount",
        "PendingApproval", 1, Now);
}
