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
    public async Task Project_material_update_gates_revision_serializes_races_and_rejects_closed_state()
    {
        var connectionString = Environment.GetEnvironmentVariable("AHDAH_CUSTODY_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        var ids = new FixtureIds();
        await AssertRuntimeRoleAsync(connectionString);
        await SeedIdentityAsync(Db(SetupConnection(connectionString)), ids);
        var manager = User(ids.CompanyId, ids.ManagerId, "Manager");
        ids.ProjectA = await CreateProject(connectionString, manager, "Project Update Gate");

        Guid replacementOwnerId;
        await using (var ownerDb = Db(SetupConnection(connectionString)))
        {
            var replacementOwner = new ProjectOwner
            {
                CompanyId = ids.CompanyId,
                OwnerName = "Replacement owner",
                PhoneNumber = $"+21894{Random.Shared.Next(1000000, 9999999)}",
                IsActive = true,
                CreatedByUserId = ids.ManagerId
            };
            ownerDb.ProjectOwners.Add(replacementOwner);
            await ownerDb.SaveChangesAsync();
            replacementOwnerId = replacementOwner.ProjectOwnerId;
        }

        var changedContractDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        await using (var materialDb = Db(connectionString))
        {
            var updated = await Projects(materialDb, manager).UpdateAsync(ids.ProjectA,
                new UpdateProjectRequest
                {
                    ExpectedVersion = 1,
                    ContractDate = changedContractDate,
                    ProjectOwnerId = replacementOwnerId
                }, default);
            Assert.Equal(AccessResultStatus.Success, updated.Status);
            Assert.Equal(replacementOwnerId, updated.Value!.Owner.ProjectOwnerId);
            Assert.Equal(changedContractDate, updated.Value!.ContractDate);
            Assert.Equal(2, await Revision(materialDb, ids.CompanyId, ids.ProjectA));
        }

        await using (var replayDb = Db(connectionString))
        {
            var replay = await Projects(replayDb, manager).UpdateAsync(ids.ProjectA,
                new UpdateProjectRequest
                {
                    ExpectedVersion = 1,
                    ContractDate = changedContractDate,
                    ProjectOwnerId = replacementOwnerId
                }, default);
            Assert.Equal(AccessResultStatus.Conflict, replay.Status);
            Assert.Equal(2, await Revision(replayDb, ids.CompanyId, ids.ProjectA));
        }

        await using (var cosmeticDb = Db(connectionString))
        {
            var updated = await Projects(cosmeticDb, manager).UpdateAsync(ids.ProjectA,
                new UpdateProjectRequest
                {
                    ExpectedVersion = 2,
                    ProjectName = "Cosmetic project rename"
                }, default);
            Assert.Equal(AccessResultStatus.Success, updated.Status);
            Assert.Equal(2, await Revision(cosmeticDb, ids.CompanyId, ids.ProjectA));
        }

        async Task<AccessResult<Ahdah.Application.Projects.Models.ProjectDetails>> Race(
            UpdateProjectRequest request)
        {
            await using var context = Db(connectionString);
            return await Projects(context, manager).UpdateAsync(ids.ProjectA, request, default);
        }

        var races = await Task.WhenAll(
            Race(new UpdateProjectRequest { ExpectedVersion = 3, Status = "Paused" }),
            Race(new UpdateProjectRequest
            {
                ExpectedVersion = 3,
                ContractDate = changedContractDate.AddDays(1)
            }));
        Assert.Single(races, result => result.Status == AccessResultStatus.Success);
        Assert.Single(races, result => result.Status == AccessResultStatus.Conflict);

        await using (var closeDb = Db(SetupConnection(connectionString)))
        {
            Assert.Equal(3, await Revision(closeDb, ids.CompanyId, ids.ProjectA));
            await SeedStructurallyClosedControlAsync(closeDb, ids);
        }

        await using (var closedDb = Db(connectionString))
        {
            var projectVersion = await closedDb.Projects.AsNoTracking()
                .Where(value => value.CompanyId == ids.CompanyId && value.ProjectId == ids.ProjectA)
                .Select(value => value.VersionNumber)
                .SingleAsync();
            var rejected = await Projects(closedDb, manager).UpdateAsync(ids.ProjectA,
                new UpdateProjectRequest
                {
                    ExpectedVersion = projectVersion,
                    ContractDate = changedContractDate.AddDays(2)
                }, default);
            Assert.Equal(AccessResultStatus.Conflict, rejected.Status);
            Assert.Equal(3, await Revision(closedDb, ids.CompanyId, ids.ProjectA));
        }
    }

    [Fact]
    public async Task Expense_gate_is_atomic_replay_safe_and_serializes_concurrent_review()
    {
        var connectionString = Environment.GetEnvironmentVariable("AHDAH_CUSTODY_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        var ids = new FixtureIds();
        await AssertRuntimeRoleAsync(connectionString);
        await SeedIdentityAsync(Db(SetupConnection(connectionString)), ids);
        var manager = User(ids.CompanyId, ids.ManagerId, "Manager");
        var deputy = User(ids.CompanyId, ids.DeputyId, "Deputy");
        var accountant = User(ids.CompanyId, ids.AccountantId, "Accountant");
        ids.ProjectA = await CreateProject(connectionString, manager, "Expense Gate A");
        await SeedCategoryAsync(Db(SetupConnection(connectionString)), ids, requiresSupplier: false);

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

        Guid rejectedExpenseId;
        await using (var createRejectedDb = Db(connectionString))
        {
            var created = await Expenses(createRejectedDb, deputy).CreateAsync(request with
            {
                Description = "Project expense rejection integration"
            }, $"expense-create-{Guid.NewGuid():N}", default);
            Assert.Equal(AccessResultStatus.Success, created.Status);
            rejectedExpenseId = created.Value!.Expense.ExpenseId;
            Assert.Equal(4, await Revision(createRejectedDb, ids.CompanyId, ids.ProjectA));
        }
        await using (var rejectExpenseDb = Db(connectionString))
        {
            var rejected = await Expenses(rejectExpenseDb, manager).RejectAsync(rejectedExpenseId,
                new RejectExpenseRequest { ExpectedVersion = 1, Reason = "Invalid receipt" },
                $"expense-reject-{Guid.NewGuid():N}", default);
            Assert.Equal(AccessResultStatus.Success, rejected.Status);
            Assert.Equal(5, await Revision(rejectExpenseDb, ids.CompanyId, ids.ProjectA));
        }

        await using (var legacyDb = Db(SetupConnection(connectionString)))
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
            if (EnforcementEnabled)
            {
                await Assert.ThrowsAsync<DbUpdateException>(() => legacyDb.SaveChangesAsync());
                return;
            }
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
        await AssertRuntimeRoleAsync(connectionString);
        await SeedIdentityAsync(Db(SetupConnection(connectionString)), ids);
        await SeedCompanySettingsAsync(Db(SetupConnection(connectionString)), ids);
        var manager = User(ids.CompanyId, ids.ManagerId, "Manager");
        var accountant = User(ids.CompanyId, ids.AccountantId, "Accountant");
        ids.ProjectA = await CreateProject(connectionString, manager, "Supplier Gate A");
        ids.ProjectB = await CreateProject(connectionString, manager, "Supplier Gate B");
        await SeedCategoryAsync(Db(SetupConnection(connectionString)), ids, requiresSupplier: true);

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
        await using (var seededDb = Db(SetupConnection(connectionString)))
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

        Guid confirmedPaymentId;
        await using (var createConfirmedDb = Db(connectionString))
        {
            var created = await Suppliers(createConfirmedDb, manager).CreatePaymentAsync(
                Payment(ids, debtA.DebtId, debtB.DebtId, 1m, 1m),
                $"supplier-payment-confirm-create-{Guid.NewGuid():N}", default);
            Assert.Equal(AccessResultStatus.Success, created.Status);
            confirmedPaymentId = created.Value!.Payment.SupplierPaymentId;
            Assert.Equal(9, await Revision(createConfirmedDb, ids.CompanyId, ids.ProjectA));
            Assert.Equal(9, await Revision(createConfirmedDb, ids.CompanyId, ids.ProjectB));
        }
        await using (var confirmPaymentDb = Db(connectionString))
        {
            var confirmed = await Suppliers(confirmPaymentDb, accountant).ConfirmPaymentAsync(
                confirmedPaymentId, new ReviewSupplierPaymentRequest { ExpectedVersion = 1 },
                $"supplier-payment-confirm-{Guid.NewGuid():N}", default);
            Assert.Equal(AccessResultStatus.Success, confirmed.Status);
            Assert.Equal(10, await Revision(confirmPaymentDb, ids.CompanyId, ids.ProjectA));
            Assert.Equal(10, await Revision(confirmPaymentDb, ids.CompanyId, ids.ProjectB));
        }
    }

    private static ExpenseService Expenses(AhdahDbContext db, ICurrentUserContext user) =>
        new(db, user, new ProjectFinancialGate(db, TimeProvider.System), TimeProvider.System);

    private static SupplierService Suppliers(AhdahDbContext db, ICurrentUserContext user) =>
        new(db, user, new ProjectFinancialGate(db, TimeProvider.System), TimeProvider.System);

    private static ProjectService Projects(AhdahDbContext db, ICurrentUserContext user) =>
        new(db, user, new ProjectFinancialGate(db, TimeProvider.System), TimeProvider.System);

    private static TestCurrentUser User(Guid companyId, Guid userId, string role) =>
        new(companyId, userId, role);

    private static AhdahDbContext Db(string connection) => new(
        new DbContextOptionsBuilder<AhdahDbContext>().UseNpgsql(connection).Options);

    private static string SetupConnection(string fallback) =>
        Environment.GetEnvironmentVariable("AHDAH_CUSTODY_TEST_SETUP_CONNECTION") ?? fallback;

    private static bool EnforcementEnabled =>
        string.Equals(Environment.GetEnvironmentVariable("AHDAH_ENFORCEMENT_TEST"), "true",
            StringComparison.OrdinalIgnoreCase);

    private static async Task AssertRuntimeRoleAsync(string connection)
    {
        var expected = Environment.GetEnvironmentVariable("AHDAH_CUSTODY_TEST_RUNTIME_ROLE");
        if (string.IsNullOrWhiteSpace(expected)) return;
        await using var db = Db(connection);
        var actual = await db.Database.SqlQueryRaw<string>("SELECT current_user AS \"Value\"").SingleAsync();
        Assert.Equal(expected, actual);
    }

    private static async Task<long> Revision(AhdahDbContext db, Guid companyId, Guid projectId) =>
        await db.ProjectFinancialControls.AsNoTracking()
            .Where(value => value.CompanyId == companyId && value.ProjectId == projectId)
            .Select(value => value.FinancialRevision).SingleAsync();

    private static async Task<Guid> CreateProject(
        string connectionString, ICurrentUserContext manager, string name)
    {
        await using var db = Db(connectionString);
        var result = await new ProjectService(db, manager, new ProjectFinancialGate(db, TimeProvider.System), TimeProvider.System).CreateAsync(
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

    private static async Task SeedCompanySettingsAsync(AhdahDbContext db, FixtureIds ids)
    {
        await using (db)
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO ahdah.company_settings(
                    company_id,created_by_user_id,updated_by_user_id,
                    expense_document_mode,expense_document_threshold_amount,
                    payment_proof_mode,payment_proof_threshold_amount)
                VALUES ({ids.CompanyId},{ids.ManagerId},{ids.ManagerId},{"Optional"},NULL,{"Optional"},NULL)
                """);
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

    private static async Task SeedStructurallyClosedControlAsync(AhdahDbContext db, FixtureIds ids)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        db.Database.AutoSavepointsEnabled = false;
        var now = DateTime.UtcNow;
        var policyId = Guid.NewGuid();
        var authorizationId = Guid.NewGuid();
        var idempotencyId = Guid.NewGuid();
        var settlementId = Guid.NewGuid();
        var approvalSnapshotId = Guid.NewGuid();
        var closureSnapshotId = Guid.NewGuid();
        var certificateId = Guid.NewGuid();
        var approvalHash = Enumerable.Repeat((byte)0x2a, 32).ToArray();
        var closureHash = Enumerable.Repeat((byte)0x3b, 32).ToArray();

        var project = await db.Projects.SingleAsync(value =>
            value.CompanyId == ids.CompanyId && value.ProjectId == ids.ProjectA);
        var control = await db.ProjectFinancialControls.SingleAsync(value =>
            value.CompanyId == ids.CompanyId && value.ProjectId == ids.ProjectA);
        var postCloseVersion = project.VersionNumber + 1;

        db.SettlementPolicyVersions.Add(new SettlementPolicyVersion
        {
            SettlementPolicyVersionId = policyId,
            CompanyId = ids.CompanyId,
            PolicyCode = "TEST-CLOSED-GATE",
            MajorVersion = 1,
            ConfigurationVersion = 1,
            PolicyText = "Structurally valid closed-state integration fixture",
            CanonicalRules = [1],
            Rules = "{}",
            ContentHash = Enumerable.Repeat((byte)0x1a, 32).ToArray(),
            ApprovedByUserId = ids.ManagerId,
            ApprovedAt = now.AddMinutes(-1),
            EffectiveFrom = now.AddMinutes(-1),
            CreatedAt = now.AddMinutes(-2)
        });
        db.IdempotencyRecords.Add(new IdempotencyRecord
        {
            IdempotencyRecordId = idempotencyId,
            CompanyId = ids.CompanyId,
            IdempotencyKey = $"closed-fixture-{Guid.NewGuid():N}",
            OperationName = "project.close_fixture",
            RequestMethod = "POST",
            RequestPath = $"/integration/projects/{ids.ProjectA}/close-fixture",
            RequestSource = "Application",
            RequestFingerprintHash = new string('a', 64),
            RequestPayloadHash = new string('b', 64),
            ActorType = "User",
            ActorUserId = ids.ManagerId,
            Status = "Completed",
            AttemptCount = 1,
            ResponseHttpStatus = 200,
            ResponseContentType = "application/json",
            ResponsePayload = "{}",
            ResourceType = "ProjectClosureCertificate",
            ResourceId = certificateId,
            ResourceVersionNumber = 1,
            ReplayCount = 0,
            CorrelationId = Guid.NewGuid(),
            StartedAt = now.AddSeconds(-1),
            CompletedAt = now,
            ExpiresAt = now.AddHours(24),
            CreatedAt = now.AddSeconds(-1),
            UpdatedAt = now
        });
        db.ProjectAuthorizationDecisions.Add(new ProjectAuthorizationDecision
        {
            ProjectAuthorizationDecisionId = authorizationId,
            CompanyId = ids.CompanyId,
            ProjectId = ids.ProjectA,
            ActionCode = "Close",
            ActorUserId = ids.ManagerId,
            ActorRole = "Manager",
            DecidedAt = now,
            Decision = "Allowed",
            PolicyVersionId = policyId,
            ConflictDetected = false,
            RequestHash = Enumerable.Repeat((byte)0x4c, 32).ToArray(),
            Details = "{}",
            CreatedAt = now
        });
        var settlement = new ProjectSettlement
        {
            ProjectSettlementId = settlementId,
            CompanyId = ids.CompanyId,
            ProjectId = ids.ProjectA,
            CycleNumber = 1,
            Status = "Draft",
            OperationalBasis = "Completed",
            CapturedProjectVersion = postCloseVersion,
            CapturedFinancialRevision = control.FinancialRevision,
            PolicyVersionId = policyId,
            EvaluatorVersion = "integration-fixture",
            SnapshotSchemaVersion = 1,
            PreparedByUserId = ids.DeputyId,
            VersionNumber = 1,
            CreatedAt = now.AddSeconds(-1),
            UpdatedAt = now.AddSeconds(-1)
        };
        db.ProjectSettlements.Add(settlement);
        await db.SaveChangesAsync();

        db.ProjectSettlementSnapshotCategories.AddRange(
            SnapshotCategory(ids, approvalSnapshotId, "ApprovalFixture", now),
            SnapshotCategory(ids, closureSnapshotId, "ClosureFixture", now));
        await db.SaveChangesAsync();

        db.ProjectSettlementSnapshots.AddRange(
            Snapshot(ids, settlementId, approvalSnapshotId, policyId, 1, "ApprovalCheck",
                postCloseVersion, control.FinancialRevision, approvalHash, now),
            Snapshot(ids, settlementId, closureSnapshotId, policyId, 2, "ClosureCheck",
                postCloseVersion, control.FinancialRevision, closureHash, now));
        await db.SaveChangesAsync();

        settlement.Status = "Approved";
        settlement.EvaluatedAt = now;
        settlement.SubmittedByUserId = ids.AccountantId;
        settlement.SubmittedAt = now;
        settlement.ApprovedByUserId = ids.ManagerId;
        settlement.ApprovedAt = now;
        settlement.SnapshotId = approvalSnapshotId;
        settlement.SnapshotHash = approvalHash;
        settlement.VersionNumber++;
        settlement.UpdatedAt = now;
        await db.SaveChangesAsync();

        db.ProjectClosureCertificates.Add(new ProjectClosureCertificate
        {
            ProjectClosureCertificateId = certificateId,
            CompanyId = ids.CompanyId,
            ProjectId = ids.ProjectA,
            ProjectSettlementId = settlementId,
            ApprovedSnapshotId = approvalSnapshotId,
            ClosureSnapshotId = closureSnapshotId,
            FinancialRevision = control.FinancialRevision,
            PolicyVersionId = policyId,
            ClosureBasis = "Completed",
            ClosedByUserId = ids.ManagerId,
            ActorRole = "Manager",
            ClosedAt = now,
            Reason = "Closed-state writer rejection integration fixture",
            PreCloseProjectVersion = project.VersionNumber,
            PostCloseProjectVersion = postCloseVersion,
            ApprovedSnapshotHash = approvalHash,
            ClosureSnapshotHash = closureHash,
            ClosureVersion = 1,
            AuthorizationDecisionId = authorizationId,
            IdempotencyRecordId = idempotencyId,
            CreatedAt = now
        });
        project.Status = "FinanciallyClosed";
        project.ActualEndDate = DateOnly.FromDateTime(now);
        project.CompletedAt = now;
        project.VersionNumber = postCloseVersion;
        project.UpdatedAt = now;
        control.FinancialState = "FinanciallyClosed";
        control.ActiveSettlementId = settlementId;
        control.ActiveClosureId = certificateId;
        control.VersionNumber++;
        control.UpdatedAt = now;
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    private static ProjectSettlementSnapshotCategory SnapshotCategory(
        FixtureIds ids, Guid snapshotId, string category, DateTime now) => new()
        {
            ProjectSettlementSnapshotCategoryId = Guid.NewGuid(),
            CompanyId = ids.CompanyId,
            ProjectId = ids.ProjectA,
            SnapshotId = snapshotId,
            CategoryCode = category,
            EvaluationStatus = "Evaluated",
            AmountMeaning = "NotApplicable",
            BlockerCount = 0,
            GapCount = 0,
            Details = "[]",
            CreatedAt = now
        };

    private static ProjectSettlementSnapshot Snapshot(
        FixtureIds ids,
        Guid settlementId,
        Guid snapshotId,
        Guid policyId,
        int number,
        string purpose,
        int projectVersion,
        long financialRevision,
        byte[] hash,
        DateTime now) => new()
        {
            ProjectSettlementSnapshotId = snapshotId,
            CompanyId = ids.CompanyId,
            ProjectId = ids.ProjectA,
            ProjectSettlementId = settlementId,
            SnapshotNumber = number,
            Purpose = purpose,
            ProjectVersion = projectVersion,
            FinancialRevision = financialRevision,
            PolicyVersionId = policyId,
            EvaluatorVersion = "integration-fixture",
            SchemaVersion = 1,
            EvaluatedAt = now,
            CanonicalPayload = [(byte)number],
            Payload = "{}",
            HashAlgorithm = "SHA256",
            SnapshotHash = hash,
            EvaluatedByUserId = ids.ManagerId,
            BlockerCount = 0,
            MandatoryGapCount = 0,
            CreatedAt = now
        };

    private static CreateSupplierPaymentRequest Payment(
        FixtureIds ids, Guid firstDebtId, Guid secondDebtId, decimal first, decimal second) => new()
    {
        SupplierId = ids.SupplierId,
        PaymentDate = DateOnly.FromDateTime(DateTime.UtcNow),
        PaymentAmount = first + second,
        CurrencyCode = "LYD",
        PaymentMethod = "Cash",
        ProofFileUrl = "/integration/supplier-payment-proof.pdf",
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
