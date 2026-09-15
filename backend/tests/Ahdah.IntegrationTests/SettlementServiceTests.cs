using Ahdah.Application.Abstractions.Context;
using Ahdah.Application.Access.Models;
using Ahdah.Application.Settlements;
using Ahdah.Infrastructure.Persistence.Generated.Context;
using Ahdah.Infrastructure.Persistence.Generated.Entities;
using Ahdah.Infrastructure.Settlements;
using Microsoft.EntityFrameworkCore;

namespace Ahdah.IntegrationTests;

public sealed class SettlementServiceTests
{
    [Fact]
    public async Task No_known_blockers_is_explicitly_indeterminate_not_financial_authority()
    {
        using var store = new SettlementStore();
        var summary = await store.Read();
        Assert.Equal(0, summary.TotalBlockerCount);
        Assert.False(summary.HasKnownFinancialBlockers);
        Assert.Null(summary.CanSettle);
        Assert.False(summary.CanClose);
        Assert.Contains("ProjectNotCompleted", summary.ClosureImpediments);
        Assert.Contains(summary.EvaluationGaps, gap => gap.Code == "ProjectAdvanceAttributionUnavailable");
        Assert.Empty(store.Db.ChangeTracker.Entries());
    }

    [Theory]
    [InlineData("Draft", "ExpenseDraft")]
    [InlineData("PendingReview", "ExpensePendingReview")]
    [InlineData("CorrectionRequired", "ExpenseCorrectionRequired")]
    [InlineData("FutureStatus", "UnknownExpenseStatus")]
    public async Task Unfinished_expenses_identify_the_exact_record_and_reason(string status, string code)
    {
        using var store = new SettlementStore();
        var expense = store.Expense(status);
        var summary = await store.Read();
        var blocker = Assert.Single(Category(summary, "Expenses").Blockers);
        Assert.Equal(code, blocker.Code);
        Assert.Equal(expense.ExpenseId, blocker.RecordId);
        Assert.Equal($"/api/v1/expenses/{expense.ExpenseId}", blocker.ResourcePath);
        Assert.False(summary.CanSettle);
    }

    [Theory]
    [InlineData("Approved")]
    [InlineData("Rejected")]
    [InlineData("Cancelled")]
    [InlineData("Reversed")]
    public async Task Final_expense_states_do_not_block_review(string status)
    {
        using var store = new SettlementStore();
        store.Expense(status);
        Assert.Empty(Category(await store.Read(), "Expenses").Blockers);
    }

    [Theory]
    [InlineData("Open", "PersonalExpense", "Reimbursements", "UnresolvedReimbursement")]
    [InlineData("PartiallySettled", "PersonalExpense", "Reimbursements", "UnresolvedReimbursement")]
    [InlineData("Open", "ManagerContribution", "ManagerContributions", "UnsettledManagerContributionClaim")]
    [InlineData("Open", "ManualClaim", "Reimbursements", "UnresolvedReimbursement")]
    public async Task Claim_authority_is_computed_outstanding_and_manager_claim_is_counted_once(
        string status, string source, string category, string code)
    {
        using var store = new SettlementStore();
        var claim = store.Claim(status, source, 23.17m);
        var summary = await store.Read();
        var blocker = Assert.Single(Category(summary, category).Blockers);
        Assert.Equal(claim.PersonalClaimId, blocker.RecordId);
        Assert.Equal(23.17m, blocker.Amount);
        Assert.Equal(code, blocker.Code);
        Assert.Equal(1, summary.TotalBlockerCount);
    }

    [Theory]
    [InlineData("Settled")]
    [InlineData("Cancelled")]
    [InlineData("Reversed")]
    public async Task Final_claims_and_debts_are_excluded_even_when_cancelled_original_amount_remains(string status)
    {
        using var store = new SettlementStore();
        store.Claim(status);
        store.Debt(status);
        var summary = await store.Read();
        Assert.Empty(Category(summary, "Reimbursements").Blockers);
        Assert.Empty(Category(summary, "SupplierDebt").Blockers);
    }

    [Theory]
    [InlineData("Open", "100.00")]
    [InlineData("PartiallySettled", "40.01")]
    public async Task Supplier_debt_reads_generated_outstanding_without_reconstructing_it(string status, string expected)
    {
        using var store = new SettlementStore();
        var amount = decimal.Parse(expected, System.Globalization.CultureInfo.InvariantCulture);
        store.Debt(status, amount);
        var summary = await store.Read();
        Assert.Equal(amount, Assert.Single(Category(summary, "SupplierDebt").Blockers).Amount);
    }

    [Fact]
    public async Task Null_computed_balance_fails_closed_and_is_not_coerced_to_zero()
    {
        using var store = new SettlementStore();
        var debt = store.Debt();
        debt.OutstandingAmount = null;
        var claim = store.Claim();
        claim.OutstandingAmount = null;
        var summary = await store.Read();
        Assert.All(summary.Categories.SelectMany(c => c.Blockers), row =>
        {
            Assert.Equal("OutstandingAmountUnavailable", row.Code);
            Assert.Null(row.Amount);
        });
        Assert.False(summary.CanSettle);
    }

    [Fact]
    public async Task Pending_payment_is_one_operation_and_only_project_allocations_are_summed()
    {
        using var store = new SettlementStore();
        var first = store.Debt("PartiallySettled", 75.19m);
        var second = store.Debt("Open", 20m);
        var other = store.Debt();
        other.ProjectId = Guid.NewGuid();
        var payment = store.Payment("PendingApproval", (first, 30.10m), (second, 10m), (other, 800m));
        var summary = await store.Read();
        Assert.Equal(95.19m, Assert.Single(Category(summary, "SupplierDebt").Totals).Amount);
        var blocker = Assert.Single(Category(summary, "SupplierPayments").Blockers);
        Assert.Equal(payment.SupplierPaymentId, blocker.RecordId);
        Assert.Equal(40.10m, blocker.Amount);
        Assert.Equal(3, summary.TotalBlockerCount);
    }

    [Theory]
    [InlineData("Confirmed")]
    [InlineData("Rejected")]
    [InlineData("Cancelled")]
    [InlineData("Reversed")]
    public async Task Final_payments_are_not_pending_reservations(string status)
    {
        using var store = new SettlementStore();
        store.Payment(status, (store.Debt(), 10m));
        Assert.Empty(Category(await store.Read(), "SupplierPayments").Blockers);
    }

    [Theory]
    [InlineData("Draft", true)]
    [InlineData("PendingApproval", true)]
    [InlineData("Approved", false)]
    [InlineData("Rejected", false)]
    [InlineData("Cancelled", false)]
    [InlineData("Reversed", false)]
    public async Task Allocated_credit_lifecycle_is_respected(string status, bool blocks)
    {
        using var store = new SettlementStore();
        store.Credit(status, store.Debt());
        Assert.Equal(blocks ? 1 : 0, Category(await store.Read(), "SupplierCredits").BlockerCount);
    }

    [Theory]
    [InlineData("Approved")]
    [InlineData("PendingApproval")]
    public async Task Unallocated_credit_does_not_become_a_project_liability(string status)
    {
        using var store = new SettlementStore();
        store.Credit(status);
        Assert.Empty(Category(await store.Read(), "SupplierCredits").Blockers);
    }

    [Fact]
    public async Task Mixed_currencies_and_large_decimals_remain_exact_and_separate()
    {
        using var store = new SettlementStore();
        store.Debt("Open", 90071992547409.91m);
        store.Debt("Open", 0.09m);
        store.Debt("Open", 7.23m).CurrencyCode = "USD";
        store.Expense("PendingReview");
        store.Claim();
        var summary = await store.Read();
        var totals = Category(summary, "SupplierDebt").Totals;
        Assert.Equal(2, totals.Count);
        Assert.Equal(90071992547410.00m, totals.Single(t => t.CurrencyCode == "LYD").Amount);
        Assert.Equal(7.23m, totals.Single(t => t.CurrencyCode == "USD").Amount);
        Assert.Equal(5, summary.TotalBlockerCount);
    }

    [Theory]
    [InlineData("Always", null, "1", true)]
    [InlineData("Threshold", "100", "100", true)]
    [InlineData("Threshold", "100", "99.99", false)]
    [InlineData("Never", null, "1000", false)]
    public async Task Documents_reuse_company_approval_threshold(string mode, string? threshold, string amount, bool missing)
    {
        using var store = new SettlementStore();
        store.Settings.ExpenseDocumentMode = mode;
        store.Settings.ExpenseDocumentThresholdAmount = threshold is null ? null : decimal.Parse(threshold,
            System.Globalization.CultureInfo.InvariantCulture);
        store.Expense("Approved").TotalAmount = decimal.Parse(amount, System.Globalization.CultureInfo.InvariantCulture);
        Assert.Equal(missing ? 1 : 0, Category(await store.Read(), "ExpenseDocuments").BlockerCount);
    }

    [Theory]
    [InlineData("Receipt", "PendingVerification", false)]
    [InlineData("Invoice", "Verified", false)]
    [InlineData("Receipt", "Rejected", true)]
    [InlineData("Other", "Verified", true)]
    public async Task Receipt_requirement_matches_existing_approval_metadata_rule(string type, string status, bool missing)
    {
        using var store = new SettlementStore();
        store.Category.RequiresReceipt = true;
        store.Settings.ExpenseDocumentMode = "Always";
        var expense = store.Expense("Approved");
        store.Document(expense, type, status);
        Assert.Equal(missing ? 1 : 0, Category(await store.Read(), "ExpenseDocuments").BlockerCount);
    }

    [Fact]
    public async Task Multiple_document_rows_are_queried_directly_and_do_not_duplicate_expense_amount()
    {
        using var store = new SettlementStore();
        store.Category.RequiresReceipt = true;
        store.Settings.ExpenseDocumentMode = "Always";
        var expense = store.Expense("PendingReview");
        store.Document(expense, "Other", "Rejected");
        // Separate units of work mirror the physical multi-row table despite the
        // generated singular navigation. The reader must not use that navigation.
        await store.Save();
        store.Document(expense, "Receipt", "Verified");
        var summary = await store.Read();
        Assert.Empty(Category(summary, "ExpenseDocuments").Blockers);
        Assert.Equal(1, Category(summary, "Expenses").BlockerCount);
    }

    [Fact]
    public async Task Missing_document_produces_no_duplicate_monetary_total()
    {
        using var store = new SettlementStore();
        store.Category.RequiresReceipt = true;
        store.Expense("CorrectionRequired");
        var summary = await store.Read();
        Assert.Equal(2, summary.TotalBlockerCount);
        Assert.Empty(Category(summary, "ExpenseDocuments").Totals);
    }

    [Fact]
    public async Task Foreign_tenant_documents_cannot_satisfy_receipt_policy()
    {
        using var store = new SettlementStore();
        store.Category.RequiresReceipt = true;
        var expense = store.Expense("Approved");
        store.Document(expense, "Receipt", "Verified").CompanyId = Guid.NewGuid();
        Assert.Single(Category(await store.Read(), "ExpenseDocuments").Blockers);
    }

    [Theory]
    [InlineData("Draft", true)]
    [InlineData("PendingApproval", true)]
    [InlineData("Approved", false)]
    [InlineData("Rejected", false)]
    [InlineData("Cancelled", false)]
    [InlineData("Reversed", false)]
    public async Task Return_lifecycle_and_approved_effect_uncertainty_are_distinct(string status, bool blocks)
    {
        using var store = new SettlementStore();
        store.Return(status);
        var summary = await store.Read();
        Assert.Equal(blocks ? 1 : 0, Category(summary, "ExpenseReturns").BlockerCount);
        Assert.Equal(status == "Approved", summary.EvaluationGaps.Any(g => g.Code == "ApprovedReturnEffectsRequireReconciliation"));
    }

    [Theory]
    [InlineData("PendingVerification", true)]
    [InlineData("Confirmed", false)]
    [InlineData("Cancelled", false)]
    [InlineData("Reversed", false)]
    public async Task Refund_lifecycle_uses_project_expense_return_lineage(string status, bool blocks)
    {
        using var store = new SettlementStore();
        var expenseReturn = store.Return("Approved");
        store.Db.SupplierRefunds.Add(new SupplierRefund { SupplierRefundId = Guid.NewGuid(), CompanyId = store.CompanyId,
            ExpenseReturnId = expenseReturn.ExpenseReturnId, Status = status, CurrencyCode = "LYD", RefundAmount = 8.73m });
        Assert.Equal(blocks ? 1 : 0, Category(await store.Read(), "SupplierRefunds").BlockerCount);
    }

    [Fact]
    public async Task Pending_claim_payment_uses_project_allocation_without_subtracting_from_claim()
    {
        using var store = new SettlementStore();
        var claim = store.Claim("PartiallySettled", amount: 31.19m);
        var payment = new PersonalClaimPayment { PersonalClaimPaymentId = Guid.NewGuid(), CompanyId = store.CompanyId,
            Status = "PendingApproval", CurrencyCode = "LYD", PaymentAmount = 100m };
        store.Db.Add(payment);
        store.Db.PersonalClaimPaymentAllocations.Add(new() { PersonalClaimPaymentAllocationId = Guid.NewGuid(),
            CompanyId = store.CompanyId, PersonalClaimId = claim.PersonalClaimId,
            PersonalClaimPaymentId = payment.PersonalClaimPaymentId, AllocatedAmount = 10.17m });
        var summary = await store.Read();
        Assert.Equal(31.19m, Assert.Single(Category(summary, "Reimbursements").Totals).Amount);
        Assert.Equal(10.17m, Assert.Single(Category(summary, "ReimbursementPayments").Totals).Amount);
    }

    [Fact]
    public async Task Company_advance_balance_is_not_invented_as_project_obligation()
    {
        using var store = new SettlementStore();
        var advance = new Advance { CompanyId = store.CompanyId, AdvanceId = Guid.NewGuid(), Status = "Open",
            CurrencyCode = "LYD", AdvanceAmount = 500m };
        store.Db.Add(advance);
        store.Db.UserAdvanceBalances.Add(new() { CompanyId = store.CompanyId, UserAdvanceBalanceId = Guid.NewGuid(),
            AdvanceId = advance.AdvanceId, UserId = store.UserId, AvailableAmount = 500m, Status = "Active" });
        store.Db.ManagerContributions.Add(new() { CompanyId = store.CompanyId, ManagerContributionId = Guid.NewGuid(),
            ContributionAmount = 300m, Status = "Confirmed", FundingSourceId = Guid.NewGuid() });
        var summary = await store.Read();
        Assert.Equal(0, summary.TotalBlockerCount);
        Assert.Equal("NotAttributable", Category(summary, "Advances").EvaluationStatus);
        Assert.Null(summary.CanSettle);
        Assert.Empty(Category(summary, "ManagerContributions").Blockers);
    }

    [Theory]
    [InlineData("Active", "ProjectNotCompleted")]
    [InlineData("Paused", "ProjectNotCompleted")]
    [InlineData("Cancelled", "ProjectCancelled")]
    [InlineData("FinanciallyClosed", "ProjectAlreadyFinanciallyClosed")]
    public async Task Project_lifecycle_is_not_conflated_with_zero_liabilities(string status, string reason)
    {
        using var store = new SettlementStore();
        store.Project.Status = status;
        var summary = await store.Read();
        Assert.False(summary.CanClose);
        Assert.Contains(reason, summary.ClosureImpediments);
    }

    [Fact]
    public async Task Completed_project_still_reports_financial_blockers_and_policy_gap()
    {
        using var store = new SettlementStore();
        store.Project.Status = "Completed";
        store.Project.CompletedAt = DateTime.UtcNow;
        store.Project.ActualEndDate = DateOnly.FromDateTime(DateTime.UtcNow);
        store.Debt();
        var summary = await store.Read();
        Assert.False(summary.CanSettle);
        Assert.False(summary.CanClose);
        Assert.DoesNotContain("ProjectNotCompleted", summary.ClosureImpediments);
        Assert.Contains(summary.EvaluationGaps, g => g.Code == "ProjectClosurePolicyUndefined");
    }

    [Fact]
    public async Task Clean_completed_project_cannot_be_certified_from_undefined_policy()
    {
        using var store = new SettlementStore();
        store.Project.Status = "Completed";
        store.Project.CompletedAt = DateTime.UtcNow;
        store.Project.ActualEndDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var summary = await store.Read();
        Assert.Null(summary.CanSettle);
        Assert.Null(summary.CanClose);
        Assert.Equal("Indeterminate", summary.ClosureReadiness);
    }

    [Theory]
    [InlineData("Manager")]
    [InlineData("Deputy")]
    [InlineData("Accountant")]
    public async Task Company_financial_roles_read_project_obligations(string role)
    {
        using var store = new SettlementStore(role);
        store.Claim();
        Assert.Single(Category(await store.Read(), "Reimbursements").Blockers);
    }

    [Fact]
    public async Task Supervisor_assigned_access_does_not_leak_hidden_financial_records_or_counts()
    {
        using var store = new SettlementStore("Supervisor");
        store.Assign();
        store.Claim();
        store.Payment("PendingApproval", (store.Debt(), 9m));
        store.Expense("PendingReview");
        var summary = await store.Read();
        Assert.Equal("AssignedProjectLimited", summary.VisibilityScope);
        Assert.Equal(2, summary.TotalBlockerCount);
        Assert.Equal("NotVisible", Category(summary, "Reimbursements").EvaluationStatus);
        Assert.Equal("NotVisible", Category(summary, "SupplierPayments").EvaluationStatus);
        Assert.Empty(Category(summary, "SupplierPayments").Totals);
        Assert.Empty(Category(summary, "Reimbursements").Blockers);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public async Task Supervisor_inactive_or_foreign_assignment_is_not_access(bool active, bool foreign)
    {
        using var store = new SettlementStore("Supervisor");
        var assignment = store.Assign();
        assignment.IsActive = active;
        if (foreign) assignment.CompanyId = Guid.NewGuid();
        Assert.Equal(AccessResultStatus.NotFound, (await store.Result()).Status);
    }

    [Theory]
    [InlineData("Worker")]
    [InlineData("FutureRole")]
    public async Task Service_denies_roles_even_without_controller_policy(string role)
    {
        using var store = new SettlementStore(role);
        Assert.Equal(AccessResultStatus.Forbidden, (await store.Result()).Status);
    }

    [Fact]
    public async Task Missing_and_cross_company_projects_are_indistinguishable()
    {
        using var store = new SettlementStore();
        store.Project.CompanyId = Guid.NewGuid();
        Assert.Equal(AccessResultStatus.NotFound, (await store.Result()).Status);
        Assert.Equal(AccessResultStatus.NotFound, (await store.Result(Guid.NewGuid())).Status);
    }

    [Fact]
    public async Task Foreign_company_records_and_allocations_are_filtered_before_projection()
    {
        using var store = new SettlementStore();
        var foreign = Guid.NewGuid();
        store.Expense("PendingReview").CompanyId = foreign;
        store.Claim().CompanyId = foreign;
        var debt = store.Debt();
        debt.CompanyId = foreign;
        store.Payment("PendingApproval", (debt, 10m));
        var local = store.Debt("Settled", 0m);
        var payment = store.Payment("Draft", (local, 12m));
        payment.CompanyId = foreign;
        Assert.Equal(0, (await store.Read()).TotalBlockerCount);
    }

    [Theory]
    [InlineData("Suspended", "Active", "Manager")]
    [InlineData("Active", "Inactive", "Manager")]
    [InlineData("Active", "Active", "Deputy")]
    public async Task Stale_membership_company_or_role_claim_is_unauthorized(string userStatus, string companyStatus, string role)
    {
        using var store = new SettlementStore();
        store.User.Status = userStatus;
        store.User.Role = role;
        store.Company.Status = companyStatus;
        Assert.Equal(AccessResultStatus.Unauthorized, (await store.Result()).Status);
    }

    [Fact]
    public async Task Owner_pending_operations_have_references_but_no_invented_currency()
    {
        using var store = new SettlementStore();
        store.Db.OwnerPaymentRefunds.Add(new() { CompanyId = store.CompanyId, ProjectId = store.ProjectId,
            OwnerPaymentRefundId = Guid.NewGuid(), Status = "PendingApproval", RefundAmount = 100m });
        store.Db.ProjectContractChanges.Add(new() { CompanyId = store.CompanyId, ProjectId = store.ProjectId,
            ProjectContractChangeId = Guid.NewGuid(), Status = "PendingApproval" });
        var summary = await store.Read();
        Assert.Equal(2, Category(summary, "OwnerOperations").BlockerCount);
        Assert.Empty(Category(summary, "OwnerOperations").Totals);
    }

    [Fact]
    public void All_category_queries_translate_through_the_real_PostgreSQL_provider()
    {
        using var db = new AhdahDbContext(new DbContextOptionsBuilder<AhdahDbContext>()
            .UseNpgsql("Host=localhost;Database=metadata_only;Username=unused;Password=unused").Options);
        foreach (var query in ProjectSettlementQueries.Create(db, Guid.NewGuid(), Guid.NewGuid(), false, 100m))
        {
            var sql = query.Records.ToQueryString();
            Assert.Contains("company_id", sql);
            Assert.Contains("project_id", sql);
            Assert.DoesNotContain("SELECT *", sql, StringComparison.OrdinalIgnoreCase);
        }
    }

    private static SettlementCategorySummary Category(ProjectSettlementSummary summary, string name) =>
        summary.Categories.Single(category => category.Category == name);
}

internal sealed class SettlementStore : IDisposable
{
    internal Guid CompanyId { get; } = Guid.NewGuid();
    internal Guid UserId { get; } = Guid.NewGuid();
    internal Guid ProjectId { get; } = Guid.NewGuid();
    internal AhdahDbContext Db { get; }
    internal Company Company { get; }
    internal AppUser User { get; }
    internal Project Project { get; }
    internal ExpenseCategory Category { get; }
    internal CompanySetting Settings { get; }
    internal string Role { get; }
    private readonly DbContextOptions<AhdahDbContext> options;

    internal SettlementStore(string role = "Manager")
    {
        Role = role;
        options = new DbContextOptionsBuilder<AhdahDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString(), o => o.EnableNullChecks(false)).Options;
        Db = Open();
        Company = new() { CompanyId = CompanyId, Status = "Active" };
        User = new() { CompanyId = CompanyId, UserId = UserId, Role = role, Status = "Active" };
        Project = new() { ProjectId = ProjectId, CompanyId = CompanyId, Status = "Active", VersionNumber = 7 };
        Category = new() { ExpenseCategoryId = Guid.NewGuid(), CompanyId = CompanyId };
        Settings = new() { CompanySettingId = Guid.NewGuid(), CompanyId = CompanyId, IsActive = true,
            ExpenseDocumentMode = "Never" };
        Db.AddRange(Company, User, Project, Category, Settings);
    }

    internal AhdahDbContext Open() => new(options);
    internal async Task Save()
    {
        await Db.SaveChangesAsync();
        Db.ChangeTracker.Clear();
    }
    internal async Task<AccessResult<ProjectSettlementSummary>> Result(Guid? id = null)
    {
        await Save();
        return await new ProjectSettlementService(Db, new TestCaller(CompanyId, UserId, Role), TimeProvider.System)
            .GetAsync(id ?? ProjectId, CancellationToken.None);
    }
    internal async Task<ProjectSettlementSummary> Read()
    {
        var result = await Result();
        Assert.Equal(AccessResultStatus.Success, result.Status);
        return Assert.IsType<ProjectSettlementSummary>(result.Value);
    }
    internal Expense Expense(string status = "Approved")
    {
        var e = new Expense { CompanyId = CompanyId, ProjectId = ProjectId, ExpenseId = Guid.NewGuid(),
            ExpenseCategoryId = Category.ExpenseCategoryId, Status = status, TotalAmount = 100m,
            CurrencyCode = "LYD", IncurredByUserId = UserId };
        Db.Add(e);
        return e;
    }
    internal SupplierDebt Debt(string status = "Open", decimal amount = 100m)
    {
        var d = new SupplierDebt { CompanyId = CompanyId, ProjectId = ProjectId, SupplierDebtId = Guid.NewGuid(),
            ExpenseId = Expense().ExpenseId, Status = status, CurrencyCode = "LYD",
            DebtAmount = 999m, OutstandingAmount = amount };
        Db.Add(d);
        return d;
    }
    internal PersonalClaim Claim(string status = "Open", string source = "PersonalExpense", decimal amount = 100m)
    {
        var c = new PersonalClaim { CompanyId = CompanyId, ProjectId = ProjectId, PersonalClaimId = Guid.NewGuid(),
            Status = status, CurrencyCode = "LYD", SourceType = source, ClaimAmount = 999m, OutstandingAmount = amount };
        Db.Add(c);
        return c;
    }
    internal SupplierPayment Payment(string status, params (SupplierDebt Debt, decimal Amount)[] allocations)
    {
        var p = new SupplierPayment { CompanyId = CompanyId, SupplierPaymentId = Guid.NewGuid(), Status = status,
            CurrencyCode = "LYD", PaymentAmount = allocations.Sum(a => a.Amount) };
        Db.Add(p);
        foreach (var a in allocations)
            Db.SupplierPaymentDebtAllocations.Add(new() { CompanyId = CompanyId, SupplierPaymentDebtAllocationId = Guid.NewGuid(),
                SupplierPaymentId = p.SupplierPaymentId, SupplierDebtId = a.Debt.SupplierDebtId, AllocatedAmount = a.Amount });
        return p;
    }
    internal SupplierCreditNote Credit(string status, SupplierDebt? debt = null)
    {
        var n = new SupplierCreditNote { CompanyId = CompanyId, SupplierCreditNoteId = Guid.NewGuid(), Status = status,
            CurrencyCode = "LYD", CreditNoteAmount = 200m };
        Db.Add(n);
        if (debt is not null)
            Db.SupplierCreditNoteAllocations.Add(new() { CompanyId = CompanyId, SupplierCreditNoteAllocationId = Guid.NewGuid(),
                SupplierCreditNoteId = n.SupplierCreditNoteId, SupplierDebtId = debt.SupplierDebtId, AllocatedAmount = 10m });
        return n;
    }
    internal ExpenseDocument Document(Expense expense, string type, string status)
    {
        var d = new ExpenseDocument { CompanyId = CompanyId, ExpenseDocumentId = Guid.NewGuid(),
            ExpenseId = expense.ExpenseId, DocumentType = type, VerificationStatus = status };
        Db.Add(d);
        return d;
    }
    internal ExpenseReturn Return(string status)
    {
        var r = new ExpenseReturn { CompanyId = CompanyId, ExpenseReturnId = Guid.NewGuid(),
            ExpenseId = Expense().ExpenseId, Status = status, CurrencyCode = "LYD", ReturnAmount = 10m };
        Db.Add(r);
        return r;
    }
    internal ProjectSupervisor Assign()
    {
        var a = new ProjectSupervisor { CompanyId = CompanyId, ProjectSupervisorId = Guid.NewGuid(),
            ProjectId = ProjectId, SupervisorUserId = UserId, IsActive = true };
        Db.Add(a);
        return a;
    }
    public void Dispose() => Db.Dispose();
    private sealed record TestCaller(Guid? CompanyId, Guid? UserId, string? Role) : ICurrentUserContext
    {
        public bool IsAuthenticated => true;
    }
}
