using Ahdah.Application.Abstractions.Context;
using Ahdah.Application.Access.Models;
using Ahdah.Application.Expenses;
using Ahdah.Application.Expenses.Contracts;
using Ahdah.Application.Projects.Contracts;
using Ahdah.Application.Suppliers;
using Ahdah.Application.Suppliers.Contracts;
using Ahdah.Infrastructure.Expenses;
using Ahdah.Infrastructure.FinancialControl;
using Ahdah.Infrastructure.Persistence.Generated.Context;
using Ahdah.Infrastructure.Persistence.Generated.Entities;
using Ahdah.Infrastructure.Projects;
using Ahdah.Infrastructure.Suppliers;
using Microsoft.EntityFrameworkCore;

namespace Ahdah.IntegrationTests;

[Collection("PostgreSQL custody serial")]
public sealed class FinancialWriterGatePostgresTests
{
    [Fact]
    public async Task Expense_gate_is_atomic_replay_safe_and_serializes_concurrent_review()
    {
        var connectionString = Environment.GetEnvironmentVariable("AHDAH_CUSTODY_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        var ids = new FixtureIds();
        await SeedIdentityAsync(Db(connectionString), ids);
        var manager = User(ids.CompanyId, ids.ManagerId, "Manager");
        var deputy = User(ids.CompanyId, ids.DeputyId, "Deputy");
        var accountant = User(ids.CompanyId, ids.AccountantId, "Accountant");
        ids.ProjectA = await CreateProject(connectionString, manager, "Expense Gate A");
        await SeedCategoryAsync(Db(connectionString), ids, requiresSupplier: false);

        var request = new CreateExpenseRequest
        {
            ExpenseCategoryId = ids.ExpenseCategoryId,
            ProjectId = ids.ProjectA,
            ExpenseDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Amount = 37.45m,
            CurrencyCode = "LYD",
            PaymentMode = ExpenseConstants.PersonalFundsPaymentMode,
            Description = "Project expense gate integration"
        };
        var key = $"expense-create-{Guid.NewGuid():N}";
        Guid expenseId;
        await using (var createDb = Db(connectionString))
        {
            var created = await Expenses(createDb, deputy).CreateAsync(request, key, default);
            Assert.Equal(AccessResultStatus.Success, created.Status);
            expenseId = created.Value!.Expense.ExpenseId;
            Assert.Equal(2, await Revision(createDb, ids.CompanyId, ids.ProjectA));
        }

        await using (var replayDb = Db(connectionString))
        {
            var replay = await Expenses(replayDb, deputy).CreateAsync(request, key, default);
            Assert.Equal(AccessResultStatus.Success, replay.Status);
            Assert.Equal(expenseId, replay.Value!.Expense.ExpenseId);
            Assert.Equal(2, await Revision(replayDb, ids.CompanyId, ids.ProjectA));
        }

        async Task<AccessResult<Ahdah.Application.Expenses.Models.ExpenseDetails>> Approve(
            ICurrentUserContext actor)
        {
            await using var context = Db(connectionString);
            return await Expenses(context, actor).ApproveAsync(expenseId,
                new ApproveExpenseRequest { ExpectedVersion = 1 },
                $"expense-approve-{Guid.NewGuid():N}", default);
        }

        var reviews = await Task.WhenAll(Approve(manager), Approve(accountant));
        Assert.Single(reviews, value => value.Status == AccessResultStatus.Success);
        Assert.Single(reviews, value => value.Status == AccessResultStatus.Conflict);
        await using (var verifyDb = Db(connectionString))
        {
            Assert.Equal(3, await Revision(verifyDb, ids.CompanyId, ids.ProjectA));
            Assert.Equal(ExpenseConstants.ApprovedStatus,
                (await verifyDb.Expenses.AsNoTracking().SingleAsync(value => value.ExpenseId == expenseId)).Status);
        }

        await using (var legacyDb = Db(connectionString))
        {
            var ownerId = await legacyDb.ProjectOwners.AsNoTracking()
                .Where(value => value.CompanyId == ids.CompanyId)
                .Select(value => value.ProjectOwnerId).FirstAsync();
            ids.LegacyProject = Guid.NewGuid();
            legacyDb.Projects.Add(new Project
            {
                ProjectId = ids.LegacyProject,
                CompanyId = ids.CompanyId,
                ProjectOwnerId = ownerId,
                ProjectName = "Legacy project without T01",
                SiteAddress = "Tripoli",
                ContractValue = 1m,
                ContractDate = DateOnly.FromDateTime(DateTime.UtcNow),
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Status = "Active",
                CreatedByUserId = ids.ManagerId,
                VersionNumber = 1
            });
            await legacyDb.SaveChangesAsync();
        }
        await using (var missingGateDb = Db(connectionString))
        {
            var rejected = await Expenses(missingGateDb, deputy).CreateAsync(request with
            {
                ProjectId = ids.LegacyProject,
                Description = "Must fail without T01"
            }, $"expense-create-{Guid.NewGuid():N}", default);
            Assert.Equal(AccessResultStatus.Conflict, rejected.Status);
            Assert.False(await missingGateDb.Expenses.AsNoTracking().AnyAsync(value =>
                value.CompanyId == ids.CompanyId && value.ProjectId == ids.LegacyProject));
        }
    }

    [Fact]
    public async Task Supplier_writers_lock_multi_project_sets_once_and_preserve_pending_debt()
    {
        var connectionString = Environment.GetEnvironmentVariable("AHDAH_CUSTODY_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        var ids = new FixtureIds();
        await SeedIdentityAsync(Db(connectionString), ids);
        var manager = User(ids.CompanyId, ids.ManagerId, "Manager");
        var accountant = User(ids.CompanyId, ids.AccountantId, "Accountant");
        ids.ProjectA = await CreateProject(connectionString, manager, "Supplier Gate A");
        ids.ProjectB = await CreateProject(connectionString, manager, "Supplier Gate B");
        await SeedCategoryAsync(Db(connectionString), ids, requiresSupplier: true);

        await using (var supplierDb = Db(connectionString))
        {
            var supplier = await Suppliers(supplierDb, manager).CreateSupplierAsync(new CreateSupplierRequest
            {
                SupplierName = "Gate Integration Supplier",
                SupplierType = "MaterialsSupplier",
                DefaultCurrencyCode = "LYD",
                TransactionMode = SupplierConstants.CreditOnlyTransactionMode,
                DefaultPaymentTermsDays = 30
            }, default);
            Assert.Equal(AccessResultStatus.Success, supplier.Status);
            ids.SupplierId = supplier.Value!.Supplier.SupplierId;
        }

        async Task<(Guid DebtId, Guid ExpenseId)> CreateAndApproveInvoice(Guid projectId, string label)
        {
            Ahdah.Application.Suppliers.Models.SupplierInvoiceDetails invoice;
            await using (var invoiceDb = Db(connectionString))
            {
                var created = await Suppliers(invoiceDb, accountant).CreateInvoiceAsync(
                    new CreateSupplierInvoiceRequest
                    {
                        SupplierId = ids.SupplierId,
                        ExpenseCategoryId = ids.ExpenseCategoryId,
                        ProjectId = projectId,
                        InvoiceDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
                        Amount = 100m,
                        CurrencyCode = "LYD",
                        Description = label
                    }, $"invoice-create-{Guid.NewGuid():N}", default);
                Assert.Equal(AccessResultStatus.Success, created.Status);
                invoice = created.Value!;
            }
            await using (var approveDb = Db(connectionString))
            {
                var approved = await Expenses(approveDb, manager).ApproveAsync(invoice.Invoice.ExpenseId,
                    new ApproveExpenseRequest { ExpectedVersion = invoice.Invoice.ExpenseVersionNumber },
                    $"invoice-approve-{Guid.NewGuid():N}", default);
                Assert.Equal(AccessResultStatus.Success, approved.Status);
            }
            return (invoice.Invoice.SupplierDebtId, invoice.Invoice.ExpenseId);
        }

        var debtA = await CreateAndApproveInvoice(ids.ProjectA, "Project A supplier invoice");
        var debtB = await CreateAndApproveInvoice(ids.ProjectB, "Project B supplier invoice");
        await using (var seededDb = Db(connectionString))
        {
            Assert.Equal(3, await Revision(seededDb, ids.CompanyId, ids.ProjectA));
            Assert.Equal(3, await Revision(seededDb, ids.CompanyId, ids.ProjectB));
            await SeedFundingAsync(seededDb, ids, 200m);
        }

        var paymentRequest = Payment(ids, debtA.DebtId, debtB.DebtId, 10m, 10m);
        var paymentKey = $"supplier-payment-{Guid.NewGuid():N}";
        Guid paymentId;
        await using (var paymentDb = Db(connectionString))
        {
            var created = await Suppliers(paymentDb, manager).CreatePaymentAsync(
                paymentRequest, paymentKey, default);
            Assert.Equal(AccessResultStatus.Success, created.Status);
            Assert.Equal(SupplierConstants.PendingApprovalStatus, created.Value!.Payment.Status);
            paymentId = created.Value.Payment.SupplierPaymentId;
            Assert.Equal(4, await Revision(paymentDb, ids.CompanyId, ids.ProjectA));
            Assert.Equal(4, await Revision(paymentDb, ids.CompanyId, ids.ProjectB));
            var debts = await paymentDb.SupplierDebts.AsNoTracking()
                .Where(value => value.SupplierDebtId == debtA.DebtId || value.SupplierDebtId == debtB.DebtId)
                .ToArrayAsync();
            Assert.All(debts, debt => Assert.Equal(0m, debt.PaidAmount));
            Assert.All(debts, debt => Assert.Equal(100m, debt.OutstandingAmount));
        }

        await using (var replayDb = Db(connectionString))
        {
            var replay = await Suppliers(replayDb, manager).CreatePaymentAsync(
                paymentRequest, paymentKey, default);
            Assert.Equal(AccessResultStatus.Success, replay.Status);
            Assert.Equal(paymentId, replay.Value!.Payment.SupplierPaymentId);
            Assert.Equal(4, await Revision(replayDb, ids.CompanyId, ids.ProjectA));
            Assert.Equal(4, await Revision(replayDb, ids.CompanyId, ids.ProjectB));
        }

        await using (var rejectDb = Db(connectionString))
        {
            var rejected = await Suppliers(rejectDb, accountant).RejectPaymentAsync(paymentId,
                new ReviewSupplierPaymentRequest { ExpectedVersion = 1, RejectionReason = "Test rejection" },
                $"supplier-payment-reject-{Guid.NewGuid():N}", default);
            Assert.Equal(AccessResultStatus.Success, rejected.Status);
            Assert.Equal(5, await Revision(rejectDb, ids.CompanyId, ids.ProjectA));
            Assert.Equal(5, await Revision(rejectDb, ids.CompanyId, ids.ProjectB));
        }

        Guid creditId;
        await using (var creditDb = Db(connectionString))
        {
            var credit = await Suppliers(creditDb, accountant).CreateCreditNoteAsync(
                new CreateSupplierCreditNoteRequest
                {
                    SupplierId = ids.SupplierId,
                    CreditNoteDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    Amount = 10m,
                    CurrencyCode = "LYD",
                    ReasonType = "PricingCorrection",
                    Description = "Multi-project credit"
                }, $"supplier-credit-{Guid.NewGuid():N}", default);
            Assert.Equal(AccessResultStatus.Success, credit.Status);
            creditId = credit.Value!.CreditNote.SupplierCreditNoteId;
        }
        await using (var approveCreditDb = Db(connectionString))
        {
            var approved = await Suppliers(approveCreditDb, manager).ApproveCreditNoteAsync(creditId,
                new ApproveSupplierCreditNoteRequest { ExpectedVersion = 1 },
                $"supplier-credit-approve-{Guid.NewGuid():N}", default);
            Assert.Equal(AccessResultStatus.Success, approved.Status);
        }
        await using (var applyCreditDb = Db(connectionString))
        {
            var applied = await Suppliers(applyCreditDb, manager).ApplyCreditNoteAsync(creditId,
                new ApplySupplierCreditNoteRequest
                {
                    ExpectedVersion = 2,
                    Allocations =
                    [
                        new SupplierCreditAllocationRequest { SupplierDebtId = debtB.DebtId, Amount = 5m },
                        new SupplierCreditAllocationRequest { SupplierDebtId = debtA.DebtId, Amount = 5m }
                    ]
                }, $"supplier-credit-apply-{Guid.NewGuid():N}", default);
            Assert.Equal(AccessResultStatus.Success, applied.Status);
            Assert.Equal(6, await Revision(applyCreditDb, ids.CompanyId, ids.ProjectA));
            Assert.Equal(6, await Revision(applyCreditDb, ids.CompanyId, ids.ProjectB));
        }

        async Task<AccessResult<Ahdah.Application.Suppliers.Models.SupplierPaymentDetails>> CreateRacePayment(
            CreateSupplierPaymentRequest request)
        {
            await using var context = Db(connectionString);
            return await Suppliers(context, manager).CreatePaymentAsync(
                request, $"supplier-payment-race-{Guid.NewGuid():N}", default);
        }

        var forward = Payment(ids, debtA.DebtId, debtB.DebtId, 2m, 2m);
        var reverse = Payment(ids, debtB.DebtId, debtA.DebtId, 2m, 2m);
        var races = await Task.WhenAll(CreateRacePayment(forward), CreateRacePayment(reverse));
        Assert.All(races, result => Assert.Equal(AccessResultStatus.Success, result.Status));
        await using (var verifyDb = Db(connectionString))
        {
            Assert.Equal(8, await Revision(verifyDb, ids.CompanyId, ids.ProjectA));
            Assert.Equal(8, await Revision(verifyDb, ids.CompanyId, ids.ProjectB));
        }
    }

    private static ExpenseService Expenses(AhdahDbContext db, ICurrentUserContext user) =>
        new(db, user, new ProjectFinancialGate(db, TimeProvider.System), TimeProvider.System);

    private static SupplierService Suppliers(AhdahDbContext db, ICurrentUserContext user) =>
        new(db, user, new ProjectFinancialGate(db, TimeProvider.System), TimeProvider.System);

    private static TestCurrentUser User(Guid companyId, Guid userId, string role) =>
        new(companyId, userId, role);

    private static AhdahDbContext Db(string connection) => new(
        new DbContextOptionsBuilder<AhdahDbContext>().UseNpgsql(connection).Options);

    private static async Task<long> Revision(AhdahDbContext db, Guid companyId, Guid projectId) =>
        await db.ProjectFinancialControls.AsNoTracking()
            .Where(value => value.CompanyId == companyId && value.ProjectId == projectId)
            .Select(value => value.FinancialRevision).SingleAsync();

    private static async Task<Guid> CreateProject(
        string connectionString, ICurrentUserContext manager, string name)
    {
        await using var db = Db(connectionString);
        var result = await new ProjectService(db, manager, TimeProvider.System).CreateAsync(
            new CreateProjectRequest
            {
                ProjectName = name,
                SiteAddress = "Tripoli",
                ContractValue = 1000m,
                ContractDate = DateOnly.FromDateTime(DateTime.UtcNow),
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                NewOwner = new NewProjectOwnerRequest
                {
                    OwnerName = $"{name} Owner",
                    PhoneNumber = $"+2189{Random.Shared.Next(1000000, 9999999)}"
                }
            }, default);
        Assert.Equal(AccessResultStatus.Success, result.Status);
        return result.Value!.ProjectId;
    }

    private static async Task SeedIdentityAsync(AhdahDbContext db, FixtureIds ids)
    {
        await using (db)
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO ahdah.companies(company_id,company_name,company_code,status) VALUES ({ids.CompanyId}, {"Writer Gate Company"}, {ids.CompanyCode}, {"Active"})");
            await InsertUser(db, ids.CompanyId, ids.ManagerId, "Manager", $"+21891{Random.Shared.Next(1000000, 9999999)}");
            await InsertUser(db, ids.CompanyId, ids.DeputyId, "Deputy", $"+21892{Random.Shared.Next(1000000, 9999999)}");
            await InsertUser(db, ids.CompanyId, ids.AccountantId, "Accountant", $"+21893{Random.Shared.Next(1000000, 9999999)}");
        }
    }

    private static Task<int> InsertUser(
        AhdahDbContext db, Guid companyId, Guid userId, string role, string phone) =>
        db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO ahdah.app_users(user_id,company_id,full_name,phone_number,password_hash,role,status,identity_verification_status) VALUES ({userId},{companyId},{role + " Writer Gate"},{phone},{"integration-only-hash"},{role},{"Active"},{"Verified"})");

    private static async Task SeedCategoryAsync(
        AhdahDbContext db, FixtureIds ids, bool requiresSupplier)
    {
        await using (db)
        {
            db.ExpenseCategories.Add(new ExpenseCategory
            {
                ExpenseCategoryId = ids.ExpenseCategoryId,
                CompanyId = ids.CompanyId,
                CategoryCode = requiresSupplier ? "SUPGATE" : "EXPGATE",
                CategoryName = requiresSupplier ? "Supplier Gate" : "Expense Gate",
                CategoryGroup = "Materials",
                ExpenseScope = ExpenseConstants.ProjectOnlyScope,
                RequiresSupplier = requiresSupplier,
                RequiresReceipt = false,
                SupportsQuantityDetails = true,
                IsActive = true,
                DisplayOrder = 1,
                CreatedByUserId = ids.ManagerId,
                VersionNumber = 1
            });
            await db.SaveChangesAsync();
        }
    }

    private static async Task SeedFundingAsync(AhdahDbContext db, FixtureIds ids, decimal amount)
    {
        var now = DateTime.UtcNow;
        db.FundingSources.Add(new FundingSource
        {
            FundingSourceId = ids.FundingSourceId,
            CompanyId = ids.CompanyId,
            SourceType = SupplierConstants.ManagerContributionSourceType,
            SourceDate = DateOnly.FromDateTime(now),
            CurrencyCode = "LYD",
            GrossAmount = amount,
            NetAmount = amount,
            AvailableAmount = amount,
            ReservedAmount = 0m,
            UsedAmount = 0m,
            ReversedAmount = 0m,
            Status = SupplierConstants.AvailableFundingStatus,
            Description = "Writer gate integration funding",
            CreatedByUserId = ids.ManagerId,
            VerifiedByUserId = ids.ManagerId,
            VerifiedAt = now,
            CreatedAt = now.AddMinutes(-1),
            UpdatedAt = now,
            VersionNumber = 1
        });
        db.ManagerContributions.Add(new ManagerContribution
        {
            ManagerContributionId = Guid.NewGuid(),
            CompanyId = ids.CompanyId,
            FundingSourceId = ids.FundingSourceId,
            ManagerUserId = ids.ManagerId,
            ContributionDate = DateOnly.FromDateTime(now),
            ContributionAmount = amount,
            Purpose = "Writer gate integration",
            Status = "Confirmed",
            RecordedByUserId = ids.ManagerId,
            VerifiedByUserId = ids.ManagerId,
            VerifiedAt = now,
            CreatedAt = now.AddMinutes(-1),
            UpdatedAt = now,
            VersionNumber = 1
        });
        await db.SaveChangesAsync();
    }

    private static CreateSupplierPaymentRequest Payment(
        FixtureIds ids, Guid firstDebtId, Guid secondDebtId, decimal first, decimal second) => new()
    {
        SupplierId = ids.SupplierId,
        PaymentDate = DateOnly.FromDateTime(DateTime.UtcNow),
        PaymentAmount = first + second,
        CurrencyCode = "LYD",
        PaymentMethod = "Cash",
        DebtAllocations =
        [
            new SupplierPaymentDebtAllocationRequest { SupplierDebtId = firstDebtId, Amount = first },
            new SupplierPaymentDebtAllocationRequest { SupplierDebtId = secondDebtId, Amount = second }
        ],
        FundingAllocations =
        [
            new SupplierPaymentFundingAllocationRequest
            {
                FundingSourceId = ids.FundingSourceId,
                Amount = first + second
            }
        ]
    };

    private sealed class TestCurrentUser(Guid companyId, Guid userId, string role) : ICurrentUserContext
    {
        public bool IsAuthenticated => true;
        public Guid? UserId => userId;
        public Guid? CompanyId => companyId;
        public string? Role => role;
    }

    private sealed class FixtureIds
    {
        public Guid CompanyId { get; } = Guid.NewGuid();
        public string CompanyCode { get; } = $"W{Guid.NewGuid():N}"[..20].ToUpperInvariant();
        public Guid ManagerId { get; } = Guid.NewGuid();
        public Guid DeputyId { get; } = Guid.NewGuid();
        public Guid AccountantId { get; } = Guid.NewGuid();
        public Guid ExpenseCategoryId { get; } = Guid.NewGuid();
        public Guid FundingSourceId { get; } = Guid.NewGuid();
        public Guid ProjectA { get; set; }
        public Guid ProjectB { get; set; }
        public Guid LegacyProject { get; set; }
        public Guid SupplierId { get; set; }
    }
}
