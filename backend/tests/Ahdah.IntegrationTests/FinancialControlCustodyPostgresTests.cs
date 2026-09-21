using Ahdah.Application.Abstractions.Context;
using Ahdah.Application.Access.Models;
using Ahdah.Application.Advances;
using Ahdah.Application.Custody;
using Ahdah.Application.Projects.Contracts;
using Ahdah.Infrastructure.Advances;
using Ahdah.Infrastructure.Custody;
using Ahdah.Infrastructure.FinancialControl;
using Ahdah.Infrastructure.Persistence.Generated.Context;
using Ahdah.Infrastructure.Persistence.Generated.Entities;
using Ahdah.Infrastructure.Projects;
using Microsoft.EntityFrameworkCore;

namespace Ahdah.IntegrationTests;

[Collection("PostgreSQL custody serial")]
public sealed class FinancialControlCustodyPostgresTests
{
    [Fact]
    public async Task Real_postgresql_project_opening_designate_concurrency_and_replay_are_atomic()
    {
        var connectionString = Environment.GetEnvironmentVariable("AHDAH_CUSTODY_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        var ids = new FixtureIds();
        await using (var setup = Db(connectionString))
        {
            await SeedIdentityAsync(setup, ids);
        }

        var manager = new TestCurrentUser(ids.CompanyId, ids.ManagerId, "Manager");
        await using (var projectDb = Db(connectionString))
        {
            var projects = new ProjectService(projectDb, manager, TimeProvider.System);
            var created = await projects.CreateAsync(new CreateProjectRequest
            {
                ProjectName = "Custody Integration Project",
                SiteAddress = "Tripoli",
                ContractValue = 2500.00m,
                ContractDate = DateOnly.FromDateTime(DateTime.UtcNow),
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                NewOwner = new NewProjectOwnerRequest
                {
                    OwnerName = "Integration Owner",
                    PhoneNumber = $"+2189{Random.Shared.Next(1000000, 9999999)}"
                }
            }, default);
            Assert.Equal(AccessResultStatus.Success, created.Status);
            ids.ProjectId = created.Value!.ProjectId;
            var control = await projectDb.ProjectFinancialControls.AsNoTracking().SingleAsync(x =>
                x.CompanyId == ids.CompanyId && x.ProjectId == ids.ProjectId);
            Assert.Equal((1L, "Open", 1, "PendingReview"),
                (control.FinancialRevision, control.FinancialState, control.NextCycleNumber, control.AttributionStatus));
        }

        await using (var setup = Db(connectionString))
        {
            await SeedAdvanceAndPolicyAsync(setup, ids);
        }

        var deputy = new TestCurrentUser(ids.CompanyId, ids.DeputyId, "Deputy");
        var openingKey = $"opening-{Guid.NewGuid():N}";
        await using (var openingDb = Db(connectionString))
        {
            var advances = new AdvanceService(openingDb, deputy, TimeProvider.System);
            var confirmed = await advances.ConfirmTransferAsync(ids.TransferId, openingKey, default);
            Assert.Equal(AccessResultStatus.Success, confirmed.Status);
            var position = await openingDb.ProjectCustodyPositions.AsNoTracking().SingleAsync(x =>
                x.CompanyId == ids.CompanyId && x.UserAdvanceBalance.AdvanceId == ids.AdvanceId);
            Assert.Null(position.ProjectId);
            Assert.Equal(100.00m, position.AvailableAmount);
            var opening = await openingDb.CustodyOperations.AsNoTracking().Include(x => x.CustodyMovementEntries)
                .SingleAsync(x => x.CompanyId == ids.CompanyId && x.MoneyTransferId == ids.TransferId);
            Assert.Equal(("Opening", "Posted", 1),
                (opening.OperationType, opening.Status, opening.CustodyMovementEntries.Count));
            Assert.Null(opening.AuthorizationDecisionId);
            Assert.Equal(100.00m, opening.CustodyMovementEntries.Single().ReceivedAmount);
            ids.BalanceId = position.UserAdvanceBalanceId;
        }
        await using (var openingReplayDb = Db(connectionString))
        {
            var replay = await new AdvanceService(openingReplayDb, deputy, TimeProvider.System)
                .ConfirmTransferAsync(ids.TransferId, openingKey, default);
            Assert.Equal(AccessResultStatus.Success, replay.Status);
            Assert.Equal(1, await openingReplayDb.CustodyOperations.AsNoTracking().CountAsync(x =>
                x.CompanyId == ids.CompanyId && x.MoneyTransferId == ids.TransferId && x.OperationType == "Opening"));
            Assert.Empty(await openingReplayDb.CustodyMovementEntries.AsNoTracking().Where(x =>
                x.CompanyId == ids.CompanyId && x.FinancialRevisionAfter != null
                && x.CustodyOperation.MoneyTransferId == ids.TransferId).ToArrayAsync());
        }

        var guardedTransferIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        await using (var transferSetup = Db(connectionString))
        {
            for (var index = 0; index < guardedTransferIds.Length; index++)
            {
                var transferId = guardedTransferIds[index];
                transferSetup.MoneyTransfers.Add(new MoneyTransfer
                {
                    MoneyTransferId = transferId, CompanyId = ids.CompanyId,
                    TransferNumber = $"GUARD-{transferId:N}",
                    TransferType = index == 0 ? AdvanceConstants.InternalTransferType : AdvanceConstants.BalanceReturnTransferType,
                    SenderUserId = ids.DeputyId, RecipientUserId = ids.ManagerId,
                    TransferAmount = 1m, CurrencyCode = "LYD", TransferDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    TransferMethod = "BalanceTransfer", Status = AdvanceConstants.PendingConfirmationStatus,
                    InitiatedByUserId = ids.DeputyId, VersionNumber = 1
                });
                transferSetup.TransferAdvanceAllocations.Add(new TransferAdvanceAllocation
                {
                    TransferAdvanceAllocationId = Guid.NewGuid(), CompanyId = ids.CompanyId,
                    MoneyTransferId = transferId, AdvanceId = ids.AdvanceId, AllocatedAmount = 1m,
                    AllocatedByUserId = ids.DeputyId
                });
            }
            await transferSetup.SaveChangesAsync();
        }
        foreach (var transferId in guardedTransferIds)
        {
            await using var guardedDb = Db(connectionString);
            var guarded = await new AdvanceService(guardedDb, manager, TimeProvider.System)
                .ConfirmTransferAsync(transferId, $"guard-{Guid.NewGuid():N}", default);
            Assert.Equal(AccessResultStatus.Conflict, guarded.Status);
            Assert.Equal(AdvanceConstants.PendingConfirmationStatus,
                (await guardedDb.MoneyTransfers.AsNoTracking().SingleAsync(x => x.MoneyTransferId == transferId)).Status);
        }

        var accountant = new TestCurrentUser(ids.CompanyId, ids.AccountantId, "Accountant");
        var firstRequest = new CreateDesignationRequest(ids.BalanceId, ids.ProjectId, 25.15m, "Initial project designation");
        CustodyOperationDetails pending;
        await using (var requestDb = Db(connectionString))
        {
            var custody = Service(requestDb, accountant);
            var requested = await custody.RequestDesignationAsync(firstRequest,
                $"request-{Guid.NewGuid():N}", default);
            Assert.Equal(AccessResultStatus.Success, requested.Status);
            Assert.Equal("PendingApproval", requested.Value!.Status);
            pending = requested.Value;
        }

        var approval = new ApproveDesignationRequest(ids.BalanceId, ids.ProjectId, 25.15m,
            firstRequest.Reason, pending.Version);
        var approvalKey = $"approve-{Guid.NewGuid():N}";
        await using (var approveDb = Db(connectionString))
        {
            var custody = Service(approveDb, manager);
            var approved = await custody.ApproveDesignationAsync(pending.OperationId, approval, approvalKey, default);
            Assert.Equal(AccessResultStatus.Success, approved.Status);
            Assert.Equal(("Posted", 2L, 25.15m),
                (approved.Value!.Status, approved.Value.FinancialRevision, approved.Value.Amount));
        }

        await using (var replayDb = Db(connectionString))
        {
            var replayed = await Service(replayDb, manager)
                .ApproveDesignationAsync(pending.OperationId, approval, approvalKey, default);
            Assert.Equal(AccessResultStatus.Success, replayed.Status);
            var control = await replayDb.ProjectFinancialControls.AsNoTracking().SingleAsync(x =>
                x.CompanyId == ids.CompanyId && x.ProjectId == ids.ProjectId);
            Assert.Equal(2, control.FinancialRevision);
            var movements = await replayDb.CustodyMovementEntries.AsNoTracking().Where(x =>
                x.CompanyId == ids.CompanyId && x.CustodyOperationId == pending.OperationId).ToArrayAsync();
            Assert.Equal(2, movements.Length);
            Assert.Equal(0m, movements.Sum(x => x.DeltaAvailableAmount));

            var balanceRead = await Service(replayDb, manager).GetBalanceAsync(ids.BalanceId, default);
            Assert.Equal(AccessResultStatus.Success, balanceRead.Status);
            Assert.Equal("LYD", balanceRead.Value!.CurrencyCode);
            Assert.True(balanceRead.Value.IsReconciled);
            Assert.Equal(2, balanceRead.Value.Positions.Count);
            Assert.Contains(balanceRead.Value.Positions, x => x.ProjectId is null && x.AvailableAmount == 74.85m);
            Assert.Contains(balanceRead.Value.Positions, x => x.ProjectId == ids.ProjectId && x.AvailableAmount == 25.15m);
            var projectRead = await Service(replayDb, manager).GetProjectAsync(ids.ProjectId, default);
            Assert.Equal(AccessResultStatus.Success, projectRead.Status);
            Assert.Equal(2, projectRead.Value!.FinancialRevision);
            Assert.Equal(AccessResultStatus.NotFound,
                (await Service(replayDb, manager).GetBalanceAsync(Guid.NewGuid(), default)).Status);

            var authority = await replayDb.ProjectAuthorizationDecisions.AsNoTracking().SingleAsync(x =>
                x.CompanyId == ids.CompanyId && x.ProjectId == ids.ProjectId
                && x.ActionCode == "Designate" && x.ActorUserId == ids.ManagerId);
            Assert.Equal(("Allowed", "Manager"), (authority.Decision, authority.ActorRole));
        }

        await using (var crossTenantDb = Db(connectionString))
        {
            var otherTenant = new TestCurrentUser(Guid.NewGuid(), ids.ManagerId, "Manager");
            Assert.Equal(AccessResultStatus.Unauthorized,
                (await Service(crossTenantDb, otherTenant).GetBalanceAsync(ids.BalanceId, default)).Status);
            Assert.Equal(AccessResultStatus.Unauthorized,
                (await Service(crossTenantDb, otherTenant).GetProjectAsync(ids.ProjectId, default)).Status);
        }

        await using (var roleDb = Db(connectionString))
        {
            Assert.Equal(AccessResultStatus.Forbidden,
                (await Service(roleDb, manager).RequestDesignationAsync(
                    new(ids.BalanceId, ids.ProjectId, 1m, "Manager cannot prepare"),
                    $"request-{Guid.NewGuid():N}", default)).Status);
            Assert.Equal(AccessResultStatus.Forbidden,
                (await Service(roleDb, accountant).ApproveDesignationAsync(pending.OperationId, approval,
                    $"approve-{Guid.NewGuid():N}", default)).Status);
        }


        CustodyOperationDetails guardedOperation;
        var guardedRequest = new CreateDesignationRequest(ids.BalanceId, ids.ProjectId, 1m,
            "Project and closure guards");
        await using (var requestDb = Db(connectionString))
        {
            guardedOperation = (await Service(requestDb, accountant).RequestDesignationAsync(
                guardedRequest, $"request-{Guid.NewGuid():N}", default)).Value!;
        }
        await using (var mismatchedDb = Db(connectionString))
        {
            var mismatched = await Service(mismatchedDb, manager).ApproveDesignationAsync(
                guardedOperation.OperationId,
                new(ids.BalanceId, Guid.NewGuid(), guardedRequest.Amount, guardedRequest.Reason,
                    guardedOperation.Version), $"approve-{Guid.NewGuid():N}", default);
            Assert.Equal(AccessResultStatus.Conflict, mismatched.Status);
            Assert.Empty(await mismatchedDb.CustodyMovementEntries.AsNoTracking().Where(x =>
                x.CompanyId == ids.CompanyId && x.CustodyOperationId == guardedOperation.OperationId).ToArrayAsync());
        }
        CustodyOperationDetails insufficientOperation;
        var insufficientRequest = new CreateDesignationRequest(ids.BalanceId, ids.ProjectId, 999m,
            "Insufficient custody must roll back");
        await using (var requestDb = Db(connectionString))
        {
            insufficientOperation = (await Service(requestDb, accountant).RequestDesignationAsync(
                insufficientRequest, $"request-{Guid.NewGuid():N}", default)).Value!;
        }
        await using (var insufficientDb = Db(connectionString))
        {
            var rejected = await Service(insufficientDb, manager).ApproveDesignationAsync(
                insufficientOperation.OperationId,
                new(ids.BalanceId, ids.ProjectId, insufficientRequest.Amount, insufficientRequest.Reason,
                    insufficientOperation.Version), $"approve-{Guid.NewGuid():N}", default);
            Assert.Equal(AccessResultStatus.Conflict, rejected.Status);
            Assert.Equal(2, (await insufficientDb.ProjectFinancialControls.AsNoTracking().SingleAsync(x =>
                x.CompanyId == ids.CompanyId && x.ProjectId == ids.ProjectId)).FinancialRevision);
            Assert.Empty(await insufficientDb.CustodyMovementEntries.AsNoTracking().Where(x =>
                x.CompanyId == ids.CompanyId && x.CustodyOperationId == insufficientOperation.OperationId).ToArrayAsync());
        }

        var concurrentRequests = new[]
        {
            new CreateDesignationRequest(ids.BalanceId, ids.ProjectId, 50m, "Concurrent designation A"),
            new CreateDesignationRequest(ids.BalanceId, ids.ProjectId, 50m, "Concurrent designation B")
        };
        var pendingOperations = new List<(CustodyOperationDetails Operation, CreateDesignationRequest Request)>();
        foreach (var request in concurrentRequests)
        {
            await using var requestDb = Db(connectionString);
            var result = await Service(requestDb, accountant).RequestDesignationAsync(request,
                $"request-{Guid.NewGuid():N}", default);
            Assert.Equal(AccessResultStatus.Success, result.Status);
            pendingOperations.Add((result.Value!, request));
        }

        async Task<AccessResult<CustodyOperationDetails>> Approve(
            (CustodyOperationDetails Operation, CreateDesignationRequest Request) item)
        {
            await using var context = Db(connectionString);
            return await Service(context, manager).ApproveDesignationAsync(item.Operation.OperationId,
                new(ids.BalanceId, ids.ProjectId, item.Request.Amount, item.Request.Reason, item.Operation.Version),
                $"approve-{Guid.NewGuid():N}", default);
        }
        var concurrentResults = await Task.WhenAll(pendingOperations.Select(Approve));
        Assert.Single(concurrentResults, x => x.Status == AccessResultStatus.Success);
        Assert.Single(concurrentResults, x => x.Status == AccessResultStatus.Conflict);

        await using (var verify = Db(connectionString))
        {
            var positions = await verify.ProjectCustodyPositions.AsNoTracking()
                .Where(x => x.CompanyId == ids.CompanyId && x.UserAdvanceBalanceId == ids.BalanceId).ToArrayAsync();
            Assert.Equal(100m, positions.Sum(x => x.AvailableAmount));
            Assert.Equal(24.85m, positions.Single(x => x.ProjectId is null).AvailableAmount);
            Assert.Equal(3, (await verify.ProjectFinancialControls.AsNoTracking().SingleAsync(x =>
                x.CompanyId == ids.CompanyId && x.ProjectId == ids.ProjectId)).FinancialRevision);

            var ownerId = await verify.ProjectOwners.AsNoTracking().Where(x => x.CompanyId == ids.CompanyId)
                .Select(x => x.ProjectOwnerId).FirstAsync();
            var legacyProject = new Project
            {
                ProjectId = Guid.NewGuid(), CompanyId = ids.CompanyId, ProjectOwnerId = ownerId,
                ProjectName = "Legacy project without control", SiteAddress = "Tripoli",
                ContractValue = 1m, ContractDate = DateOnly.FromDateTime(DateTime.UtcNow),
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow), Status = "Active",
                CreatedByUserId = ids.ManagerId, VersionNumber = 1
            };
            verify.Projects.Add(legacyProject);
            await verify.SaveChangesAsync();
            Assert.Equal(AccessResultStatus.Conflict,
                (await Service(verify, manager).GetProjectAsync(legacyProject.ProjectId, default)).Status);
        }
    }

    [Fact]
    public void Financially_closed_gate_fails_closed_without_mutating_revision()
    {
        var control = new ProjectFinancialControl { FinancialState = "FinanciallyClosed", FinancialRevision = 9 };
        Assert.False(ProjectFinancialGate.AllowsOrdinaryFinancialMutation(control));
        Assert.Equal(9, control.FinancialRevision);
    }

    private static CustodyService Service(AhdahDbContext db, ICurrentUserContext user) =>
        new(db, user, new ProjectFinancialGate(db, TimeProvider.System), TimeProvider.System);

    private static AhdahDbContext Db(string connection) => new(
        new DbContextOptionsBuilder<AhdahDbContext>().UseNpgsql(connection).Options);

    private static async Task SeedIdentityAsync(AhdahDbContext db, FixtureIds ids)
    {
        await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO ahdah.companies(company_id,company_name,company_code,status) VALUES ({ids.CompanyId}, {"Custody Integration Company"}, {ids.CompanyCode}, {"Active"})");
        await InsertUser(db, ids.CompanyId, ids.ManagerId, "Manager", $"+21891{Random.Shared.Next(1000000, 9999999)}");
        await InsertUser(db, ids.CompanyId, ids.DeputyId, "Deputy", $"+21892{Random.Shared.Next(1000000, 9999999)}");
        await InsertUser(db, ids.CompanyId, ids.AccountantId, "Accountant", $"+21893{Random.Shared.Next(1000000, 9999999)}");
    }

    private static Task<int> InsertUser(AhdahDbContext db, Guid company, Guid user, string role, string phone) =>
        db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO ahdah.app_users(user_id,company_id,full_name,phone_number,password_hash,role,status,identity_verification_status) VALUES ({user},{company},{role + " Integration"},{phone},{"integration-only-hash"},{role},{"Active"},{"Verified"})");

    private static async Task SeedAdvanceAndPolicyAsync(AhdahDbContext db, FixtureIds ids)
    {
        var now = DateTime.UtcNow;
        db.Advances.Add(new Advance
        {
            AdvanceId = ids.AdvanceId, CompanyId = ids.CompanyId, AdvanceNumber = $"ADV-{ids.AdvanceId:N}",
            DeputyUserId = ids.DeputyId, AdvanceAmount = 100m, CurrencyCode = "LYD",
            IssueDate = DateOnly.FromDateTime(DateTime.UtcNow), Purpose = "Custody integration",
            Status = AdvanceConstants.PendingConfirmationStatus, CreatedByUserId = ids.ManagerId, VersionNumber = 1
        });
        db.FundingSources.Add(new FundingSource
        {
            FundingSourceId = ids.FundingSourceId, CompanyId = ids.CompanyId, SourceType = "ManagerContribution",
            SourceDate = DateOnly.FromDateTime(DateTime.UtcNow), CurrencyCode = "LYD", GrossAmount = 100m,
            NetAmount = 100m, AvailableAmount = 0m, ReservedAmount = 100m, Status = "Available",
            CreatedByUserId = ids.ManagerId, VerifiedByUserId = ids.ManagerId,
            VerifiedAt = now, VersionNumber = 1, CreatedAt = now, UpdatedAt = now
        });
        db.AdvanceFundingSources.Add(new AdvanceFundingSource
        {
            AdvanceFundingSourceId = Guid.NewGuid(), CompanyId = ids.CompanyId, AdvanceId = ids.AdvanceId,
            FundingSourceId = ids.FundingSourceId, AllocatedAmount = 100m, AllocatedByUserId = ids.ManagerId
        });
        db.MoneyTransfers.Add(new MoneyTransfer
        {
            MoneyTransferId = ids.TransferId, CompanyId = ids.CompanyId,
            TransferNumber = $"TR-{ids.TransferId:N}", TransferType = AdvanceConstants.AdvanceDeliveryTransferType,
            AdvanceId = ids.AdvanceId, SenderUserId = ids.ManagerId, RecipientUserId = ids.DeputyId,
            TransferAmount = 100m, CurrencyCode = "LYD", TransferDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TransferMethod = "Cash", Status = AdvanceConstants.PendingConfirmationStatus,
            InitiatedByUserId = ids.ManagerId, VersionNumber = 1
        });
        db.SettlementPolicyVersions.Add(new SettlementPolicyVersion
        {
            SettlementPolicyVersionId = ids.PolicyId, CompanyId = ids.CompanyId, PolicyCode = "POLICY_V1",
            MajorVersion = 1, ConfigurationVersion = 1, PolicyText = "Integration Policy v1",
            CanonicalRules = [1], Rules = "{}", ContentHash = new byte[32],
            ApprovedByUserId = ids.ManagerId, CreatedAt = now.AddMinutes(-2),
            ApprovedAt = now.AddMinutes(-1), EffectiveFrom = now.AddSeconds(-30)
        });
        db.CompanySettlementPolicyBindings.Add(new CompanySettlementPolicyBinding
        {
            CompanyId = ids.CompanyId, ActivePolicyVersionId = ids.PolicyId,
            ActivatedByUserId = ids.ManagerId, ActivatedAt = DateTime.UtcNow, VersionNumber = 1
        });
        await db.SaveChangesAsync();
    }

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
        public string CompanyCode { get; } = $"C{Guid.NewGuid():N}"[..20].ToUpperInvariant();
        public Guid ManagerId { get; } = Guid.NewGuid();
        public Guid DeputyId { get; } = Guid.NewGuid();
        public Guid AccountantId { get; } = Guid.NewGuid();
        public Guid AdvanceId { get; } = Guid.NewGuid();
        public Guid FundingSourceId { get; } = Guid.NewGuid();
        public Guid TransferId { get; } = Guid.NewGuid();
        public Guid PolicyId { get; } = Guid.NewGuid();
        public Guid ProjectId { get; set; }
        public Guid BalanceId { get; set; }
    }
}

[CollectionDefinition("PostgreSQL custody serial", DisableParallelization = true)]
public sealed class PostgreSqlCustodyCollection;
