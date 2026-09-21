using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Ahdah.Application.Abstractions.Context;
using Ahdah.Application.Access;
using Ahdah.Application.Access.Models;
using Ahdah.Application.Custody;
using Ahdah.Application.Identity;
using Ahdah.Infrastructure.FinancialControl;
using Ahdah.Infrastructure.Persistence.Generated.Context;
using Ahdah.Infrastructure.Persistence.Generated.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Ahdah.Infrastructure.Custody;

public sealed class CustodyService(
    AhdahDbContext db,
    ICurrentUserContext currentUser,
    ProjectFinancialGate financialGate,
    TimeProvider timeProvider) : ICustodyService
{
    public async Task<AccessResult<BalanceCustodyDetails>> GetBalanceAsync(
        Guid balanceId, CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return AccessResult<BalanceCustodyDetails>.Failure(AccessResultStatus.Unauthorized);
        if (!CanView(caller.Role)) return AccessResult<BalanceCustodyDetails>.Failure(AccessResultStatus.Forbidden);

        var balance = await db.UserAdvanceBalances.AsNoTracking()
            .Where(x => x.CompanyId == caller.CompanyId && x.UserAdvanceBalanceId == balanceId)
            .Select(x => new { x.UserAdvanceBalanceId, x.AdvanceId, x.Advance.CurrencyCode, x.AvailableAmount, x.ReservedAmount })
            .SingleOrDefaultAsync(cancellationToken);
        if (balance is null) return AccessResult<BalanceCustodyDetails>.Failure(AccessResultStatus.NotFound);

        var positions = await db.ProjectCustodyPositions.AsNoTracking()
            .Where(x => x.CompanyId == caller.CompanyId && x.UserAdvanceBalanceId == balanceId)
            .OrderBy(x => x.ProjectId == null ? 0 : 1).ThenBy(x => x.ProjectId)
            .Select(x => new CustodyPositionDetails(x.ProjectCustodyPositionId, x.UserAdvanceBalanceId,
                x.ProjectId, x.Project == null ? null : x.Project.ProjectName, x.AvailableAmount,
                x.ReservedAmount, x.VersionNumber))
            .ToArrayAsync(cancellationToken);
        var reconciled = positions.Count(x => x.ProjectId is null) == 1
            && positions.Sum(x => x.AvailableAmount) == balance.AvailableAmount
            && positions.Sum(x => x.ReservedAmount) == balance.ReservedAmount;
        return AccessResult<BalanceCustodyDetails>.Success(new(balance.UserAdvanceBalanceId,
            balance.AdvanceId, balance.CurrencyCode, balance.AvailableAmount, balance.ReservedAmount,
            positions, reconciled));
    }

    public async Task<AccessResult<ProjectCustodyDetails>> GetProjectAsync(
        Guid projectId, CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return AccessResult<ProjectCustodyDetails>.Failure(AccessResultStatus.Unauthorized);
        if (!CanView(caller.Role)) return AccessResult<ProjectCustodyDetails>.Failure(AccessResultStatus.Forbidden);

        var project = await db.Projects.AsNoTracking()
            .Where(x => x.CompanyId == caller.CompanyId && x.ProjectId == projectId)
            .Select(x => new { x.ProjectId, x.ProjectName })
            .SingleOrDefaultAsync(cancellationToken);
        if (project is null) return AccessResult<ProjectCustodyDetails>.Failure(AccessResultStatus.NotFound);
        var control = await financialGate.GetAsync(caller.CompanyId, projectId, cancellationToken);
        if (control is null) return AccessResult<ProjectCustodyDetails>.Failure(AccessResultStatus.Conflict);

        var positions = await db.ProjectCustodyPositions.AsNoTracking()
            .Where(x => x.CompanyId == caller.CompanyId && x.ProjectId == projectId)
            .OrderBy(x => x.UserAdvanceBalanceId)
            .Select(x => new CustodyPositionDetails(x.ProjectCustodyPositionId, x.UserAdvanceBalanceId,
                x.ProjectId, project.ProjectName, x.AvailableAmount, x.ReservedAmount, x.VersionNumber))
            .ToArrayAsync(cancellationToken);
        return AccessResult<ProjectCustodyDetails>.Success(new(project.ProjectId, project.ProjectName,
            control.FinancialState, control.FinancialRevision, control.AttributionStatus, positions));
    }

    public async Task<AccessResult<CustodyOperationDetails>> RequestDesignationAsync(
        CreateDesignationRequest request, string idempotencyKey, CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return AccessResult<CustodyOperationDetails>.Failure(AccessResultStatus.Unauthorized);
        if (caller.Role is not (AccessConstants.AccountantRole or AccessConstants.DeputyRole))
            return AccessResult<CustodyOperationDetails>.Failure(AccessResultStatus.Forbidden);
        var normalized = request with { Reason = request.Reason?.Trim() ?? string.Empty };
        if (!Valid(normalized) || !ValidKey(idempotencyKey))
            return AccessResult<CustodyOperationDetails>.Failure(AccessResultStatus.Invalid);

        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var fingerprint = Fingerprint("custody.request_designation", normalized);
            var attempt = await BeginIdempotencyAsync(caller, idempotencyKey,
                "custody.request_designation", "/api/v1/custody/designations", fingerprint, cancellationToken);
            if (attempt.Conflict) return await Rollback<CustodyOperationDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
            if (attempt.Replay)
            {
                var replay = Deserialize<CustodyOperationDetails>(attempt.Record.ResponsePayload);
                if (replay is null) return await Rollback<CustodyOperationDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return AccessResult<CustodyOperationDetails>.Success(replay);
            }

            var exists = await db.UserAdvanceBalances.AsNoTracking().AnyAsync(x =>
                x.CompanyId == caller.CompanyId && x.UserAdvanceBalanceId == normalized.BalanceId, cancellationToken)
                && await db.Projects.AsNoTracking().AnyAsync(x =>
                    x.CompanyId == caller.CompanyId && x.ProjectId == normalized.ProjectId, cancellationToken);
            if (!exists) return await Rollback<CustodyOperationDetails>(transaction, AccessResultStatus.NotFound, cancellationToken);
            if (await financialGate.GetAsync(caller.CompanyId, normalized.ProjectId, cancellationToken) is null)
                return await Rollback<CustodyOperationDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);

            var now = timeProvider.GetUtcNow().UtcDateTime;
            var operation = new CustodyOperation
            {
                CustodyOperationId = Guid.NewGuid(), CompanyId = caller.CompanyId,
                OperationType = "Designate", Status = "PendingApproval",
                RequestedByUserId = caller.UserId, RequestedAt = now,
                Reason = normalized.Reason, IdempotencyRecordId = attempt.Record.IdempotencyRecordId,
                VersionNumber = 1
            };
            db.CustodyOperations.Add(operation);
            var result = Details(operation, normalized.ProjectId, normalized.BalanceId, normalized.Amount, null, null);
            Complete(attempt.Record, "CustodyOperation", operation.CustodyOperationId, operation.VersionNumber, result);
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return AccessResult<CustodyOperationDetails>.Success(result);
        }
        catch (DbUpdateException ex) when (IsConflict(ex))
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<CustodyOperationDetails>.Failure(AccessResultStatus.Conflict);
        }
    }

    public async Task<AccessResult<CustodyOperationDetails>> ApproveDesignationAsync(
        Guid operationId, ApproveDesignationRequest request, string idempotencyKey,
        CancellationToken cancellationToken)
    {
        var caller = await GetCallerAsync(cancellationToken);
        if (caller is null) return AccessResult<CustodyOperationDetails>.Failure(AccessResultStatus.Unauthorized);
        if (caller.Role != IdentityConstants.ManagerRole)
            return AccessResult<CustodyOperationDetails>.Failure(AccessResultStatus.Forbidden);
        var normalized = request with { Reason = request.Reason?.Trim() ?? string.Empty };
        if (operationId == Guid.Empty || !Valid(new(normalized.BalanceId, normalized.ProjectId, normalized.Amount, normalized.Reason))
            || normalized.ExpectedOperationVersion < 1 || !ValidKey(idempotencyKey))
            return AccessResult<CustodyOperationDetails>.Failure(AccessResultStatus.Invalid);

        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var approvalFingerprint = Fingerprint("custody.approve_designation", new { operationId, request = normalized });
            var attempt = await BeginIdempotencyAsync(caller, idempotencyKey,
                "custody.approve_designation", $"/api/v1/custody/designations/{operationId}/approve",
                approvalFingerprint, cancellationToken);
            if (attempt.Conflict) return await Rollback<CustodyOperationDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
            if (attempt.Replay)
            {
                var replay = Deserialize<CustodyOperationDetails>(attempt.Record.ResponsePayload);
                if (replay is null) return await Rollback<CustodyOperationDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return AccessResult<CustodyOperationDetails>.Success(replay);
            }

            var operation = await db.CustodyOperations
                .FromSqlInterpolated($"SELECT * FROM ahdah.custody_operations WHERE company_id = {caller.CompanyId} AND custody_operation_id = {operationId} FOR UPDATE")
                .SingleOrDefaultAsync(cancellationToken);
            if (operation is null) return await Rollback<CustodyOperationDetails>(transaction, AccessResultStatus.NotFound, cancellationToken);
            if (operation.OperationType != "Designate" || operation.Status != "PendingApproval"
                || operation.VersionNumber != normalized.ExpectedOperationVersion)
                return await Rollback<CustodyOperationDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);

            var original = await db.IdempotencyRecords.AsNoTracking().SingleAsync(x =>
                x.CompanyId == caller.CompanyId && x.IdempotencyRecordId == operation.IdempotencyRecordId, cancellationToken);
            var originalRequest = new CreateDesignationRequest(normalized.BalanceId, normalized.ProjectId,
                normalized.Amount, normalized.Reason);
            if (original.OperationName != "custody.request_designation"
                || original.RequestFingerprintHash != Fingerprint("custody.request_designation", originalRequest))
                return await Rollback<CustodyOperationDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);

            var controls = await financialGate.LockAsync(caller.CompanyId, [normalized.ProjectId], cancellationToken);
            var control = controls?.SingleOrDefault();
            if (control is null || !ProjectFinancialGate.AllowsOrdinaryFinancialMutation(control))
                return await Rollback<CustodyOperationDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);

            var balance = await db.UserAdvanceBalances
                .FromSqlInterpolated($"SELECT * FROM ahdah.user_advance_balances WHERE company_id = {caller.CompanyId} AND user_advance_balance_id = {normalized.BalanceId} FOR UPDATE")
                .SingleOrDefaultAsync(cancellationToken);
            if (balance is null) return await Rollback<CustodyOperationDetails>(transaction, AccessResultStatus.NotFound, cancellationToken);
            var advance = await db.Advances.AsNoTracking().SingleAsync(x =>
                x.CompanyId == caller.CompanyId && x.AdvanceId == balance.AdvanceId, cancellationToken);

            var positions = await db.ProjectCustodyPositions
                .FromSqlInterpolated($"SELECT * FROM ahdah.project_custody_positions WHERE company_id = {caller.CompanyId} AND user_advance_balance_id = {normalized.BalanceId} ORDER BY project_custody_position_id FOR UPDATE")
                .ToArrayAsync(cancellationToken);
            var unassigned = positions.SingleOrDefault(x => x.ProjectId == null);
            if (unassigned is null || unassigned.AvailableAmount < normalized.Amount)
                return await Rollback<CustodyOperationDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);
            var projectPosition = positions.SingleOrDefault(x => x.ProjectId == normalized.ProjectId);

            var policy = await db.CompanySettlementPolicyBindings.AsNoTracking()
                .Where(x => x.CompanyId == caller.CompanyId
                    && x.SettlementPolicyVersion.EffectiveFrom <= timeProvider.GetUtcNow().UtcDateTime)
                .Select(x => x.SettlementPolicyVersion)
                .SingleOrDefaultAsync(cancellationToken);
            if (policy is null) return await Rollback<CustodyOperationDetails>(transaction, AccessResultStatus.Conflict, cancellationToken);

            var now = timeProvider.GetUtcNow().UtcDateTime;
            var requestHash = SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(originalRequest)));
            var decision = new ProjectAuthorizationDecision
            {
                ProjectAuthorizationDecisionId = Guid.NewGuid(), CompanyId = caller.CompanyId,
                ProjectId = normalized.ProjectId, ActionCode = "Designate", ActorUserId = caller.UserId,
                ActorRole = caller.Role, DecidedAt = now, Decision = "Allowed",
                PolicyVersionId = policy.SettlementPolicyVersionId,
                PreparerUserId = operation.RequestedByUserId, SubmitterUserId = operation.RequestedByUserId,
                BeneficiaryUserId = balance.UserId, ConflictDetected = false, RequestHash = requestHash,
                Details = JsonSerializer.Serialize(new { custodyOperationId = operationId, normalized.BalanceId, normalized.ProjectId,
                    normalized.Amount, CurrencyCode = advance.CurrencyCode })
            };
            db.ProjectAuthorizationDecisions.Add(decision);

            var unassignedBefore = unassigned.VersionNumber;
            unassigned.AvailableAmount -= normalized.Amount;
            unassigned.VersionNumber++;
            unassigned.UpdatedAt = now;
            var projectBefore = projectPosition?.VersionNumber ?? 1;
            if (projectPosition is null)
            {
                projectPosition = new ProjectCustodyPosition
                {
                    ProjectCustodyPositionId = Guid.NewGuid(), CompanyId = caller.CompanyId,
                    UserAdvanceBalanceId = balance.UserAdvanceBalanceId, ProjectId = normalized.ProjectId,
                    AvailableAmount = normalized.Amount, ReservedAmount = 0m, VersionNumber = 2
                };
                db.ProjectCustodyPositions.Add(projectPosition);
            }
            else
            {
                projectPosition.AvailableAmount += normalized.Amount;
                projectPosition.VersionNumber++;
                projectPosition.UpdatedAt = now;
            }
            var revision = financialGate.AdvanceRevision(control);
            db.CustodyMovementEntries.AddRange(
                Movement(operation, unassigned, 1, -normalized.Amount, caller.UserId,
                    unassignedBefore, unassigned.VersionNumber, null, now),
                Movement(operation, projectPosition, 2, normalized.Amount, caller.UserId,
                    projectBefore, projectPosition.VersionNumber, revision, now));

            await db.SaveChangesAsync(cancellationToken);
            operation.Status = "Posted";
            operation.ApprovedByUserId = caller.UserId;
            operation.ApprovedAt = now;
            operation.AuthorizationDecisionId = decision.ProjectAuthorizationDecisionId;
            operation.VersionNumber++;
            operation.UpdatedAt = now;
            var result = Details(operation, normalized.ProjectId, normalized.BalanceId, normalized.Amount,
                advance.CurrencyCode, revision);
            Complete(attempt.Record, "CustodyOperation", operation.CustodyOperationId, operation.VersionNumber, result);
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return AccessResult<CustodyOperationDetails>.Success(result);
        }
        catch (DbUpdateException ex) when (IsConflict(ex))
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<CustodyOperationDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<CustodyOperationDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch (PostgresException ex) when (ex.SqlState is PostgresErrorCodes.CheckViolation
            or PostgresErrorCodes.SerializationFailure or PostgresErrorCodes.DeadlockDetected
            or PostgresErrorCodes.RaiseException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<CustodyOperationDetails>.Failure(AccessResultStatus.Conflict);
        }
    }

    private static CustodyMovementEntry Movement(CustodyOperation operation, ProjectCustodyPosition position,
        int number, decimal delta, Guid actor, int before, int after, long? revision, DateTime now) => new()
    {
        CustodyMovementEntryId = Guid.NewGuid(), CompanyId = operation.CompanyId,
        CustodyOperation = operation, ProjectCustodyPosition = position, EntryNumber = number,
        DeltaAvailableAmount = delta, DeltaReservedAmount = 0m, OccurredAt = now,
        ActorUserId = actor, PositionVersionBefore = before, PositionVersionAfter = after,
        FinancialRevisionAfter = revision
    };

    private async Task<Caller?> GetCallerAsync(CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.CompanyId is not { } companyId
            || currentUser.UserId is not { } userId || currentUser.Role is not { } role) return null;
        var actual = await db.AppUsers.AsNoTracking().Where(x => x.CompanyId == companyId && x.UserId == userId
                && x.Status == IdentityConstants.ActiveStatus && x.Company.Status == IdentityConstants.ActiveStatus)
            .Select(x => x.Role).SingleOrDefaultAsync(cancellationToken);
        return actual == role ? new Caller(companyId, userId, role) : null;
    }

    private async Task<Attempt> BeginIdempotencyAsync(Caller caller, string key, string operationName,
        string path, string fingerprint, CancellationToken cancellationToken)
    {
        var existing = await db.IdempotencyRecords.FromSqlInterpolated(
            $"SELECT * FROM ahdah.idempotency_records WHERE company_id = {caller.CompanyId} AND idempotency_key = {key} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);
        if (existing is not null)
        {
            var conflict = existing.OperationName != operationName || existing.RequestFingerprintHash != fingerprint
                || existing.ActorUserId != caller.UserId || existing.Status != "Completed" || existing.ResourceId is null;
            if (!conflict) { existing.ReplayCount++; existing.LastReplayedAt = timeProvider.GetUtcNow().UtcDateTime; }
            return new(existing, !conflict, conflict);
        }
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var record = new IdempotencyRecord
        {
            IdempotencyRecordId = Guid.NewGuid(), CompanyId = caller.CompanyId, IdempotencyKey = key,
            OperationName = operationName, RequestMethod = "POST", RequestPath = path,
            RequestSource = "Application", RequestFingerprintHash = fingerprint, RequestPayloadHash = fingerprint,
            ActorType = "User", ActorUserId = caller.UserId, Status = "InProgress", AttemptCount = 1,
            LockToken = Guid.NewGuid(), LockedBy = "Ahdah.Api", LockAcquiredAt = now,
            LeaseExpiresAt = now.AddMinutes(5), CorrelationId = Guid.NewGuid(), StartedAt = now,
            ExpiresAt = now.AddHours(24)
        };
        db.IdempotencyRecords.Add(record);
        return new(record, false, false);
    }

    private void Complete(IdempotencyRecord record, string type, Guid id, int version, CustodyOperationDetails result)
    {
        record.Status = "Completed"; record.LockToken = null; record.LockedBy = null;
        record.LockAcquiredAt = null; record.LeaseExpiresAt = null; record.ResponseHttpStatus = 200;
        record.ResponseContentType = "application/json"; record.ResponsePayload = JsonSerializer.Serialize(result);
        record.ResourceType = type; record.ResourceId = id; record.ResourceVersionNumber = version;
        record.CompletedAt = timeProvider.GetUtcNow().UtcDateTime;
    }

    private static CustodyOperationDetails Details(CustodyOperation operation, Guid? projectId,
        Guid? balanceId, decimal? amount, string? currency, long? revision) => new(
        operation.CustodyOperationId, operation.OperationType, operation.Status,
        operation.RequestedByUserId, new DateTimeOffset(operation.RequestedAt, TimeSpan.Zero),
        operation.ApprovedByUserId,
        operation.ApprovedAt is { } approved ? new DateTimeOffset(approved, TimeSpan.Zero) : null,
        operation.VersionNumber, projectId, balanceId, amount, currency, revision);

    private static bool Valid(CreateDesignationRequest request) => request.BalanceId != Guid.Empty
        && request.ProjectId != Guid.Empty && request.Amount >= 0.01m
        && decimal.Round(request.Amount, 2) == request.Amount
        && request.Reason.Length is >= 1 and <= 1000;
    private static bool ValidKey(string key) => key.Length is >= 16 and <= 200;
    private static bool CanView(string role) => role is IdentityConstants.ManagerRole
        or AccessConstants.DeputyRole or AccessConstants.AccountantRole;
    private static string Fingerprint(string operation, object value) => Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes($"{operation}|{JsonSerializer.Serialize(value)}"))).ToLowerInvariant();
    private static T? Deserialize<T>(string? json) => string.IsNullOrWhiteSpace(json) ? default : JsonSerializer.Deserialize<T>(json);
    private static bool IsConflict(DbUpdateException ex) => ex.InnerException is PostgresException p
        && p.SqlState is PostgresErrorCodes.UniqueViolation or PostgresErrorCodes.CheckViolation
            or PostgresErrorCodes.ForeignKeyViolation or PostgresErrorCodes.RaiseException;
    private static async Task<AccessResult<T>> Rollback<T>(Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction tx,
        AccessResultStatus status, CancellationToken cancellationToken)
    { await tx.RollbackAsync(cancellationToken); return AccessResult<T>.Failure(status); }

    private sealed record Caller(Guid CompanyId, Guid UserId, string Role);
    private sealed record Attempt(IdempotencyRecord Record, bool Replay, bool Conflict);
}
