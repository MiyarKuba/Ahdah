using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Ahdah.Application.Abstractions.Context;
using Ahdah.Application.Access;
using Ahdah.Application.Access.Models;
using Ahdah.Application.Advances;
using Ahdah.Application.Advances.Contracts;
using Ahdah.Application.Advances.Models;
using Ahdah.Application.Advances.Services;
using Ahdah.Application.Identity;
using Ahdah.Infrastructure.Persistence.Generated.Context;
using Ahdah.Infrastructure.Persistence.Generated.Entities;
using Ahdah.Infrastructure.FinancialControl;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Ahdah.Infrastructure.Advances;

public sealed class AdvanceService(
    AhdahDbContext dbContext,
    ICurrentUserContext currentUserContext,
    ProjectFinancialGate financialGate,
    TimeProvider timeProvider) : IAdvanceService
{
    public async Task<AccessResult<PagedResult<AdvanceSummary>>> ListAsync(
        AdvanceQuery query,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<PagedResult<AdvanceSummary>>.Failure(AccessResultStatus.Unauthorized);
        }

        if (!IsValid(query))
        {
            return AccessResult<PagedResult<AdvanceSummary>>.Failure(AccessResultStatus.Invalid);
        }

        if (query.UserId is { } filteredUserId
            && !caller.Capabilities.CanViewOtherUserBalances
            && filteredUserId != caller.UserId)
        {
            return AccessResult<PagedResult<AdvanceSummary>>.Failure(AccessResultStatus.Forbidden);
        }

        var advances = ApplyVisibility(
            dbContext.Advances.AsNoTracking().Where(advance => advance.CompanyId == caller.CompanyId),
            caller);

        if (query.Status is not null)
        {
            advances = advances.Where(advance => advance.Status == query.Status);
        }

        if (query.UserId is { } userId)
        {
            advances = advances.Where(advance =>
                advance.DeputyUserId == userId
                || advance.UserAdvanceBalances.Any(balance =>
                    balance.CompanyId == caller.CompanyId && balance.UserId == userId)
                || advance.TransferAdvanceAllocations.Any(allocation =>
                    allocation.CompanyId == caller.CompanyId
                    && (allocation.MoneyTransfer.SenderUserId == userId
                        || allocation.MoneyTransfer.RecipientUserId == userId)));
        }

        if (NormalizeOptional(query.Reference) is { } reference)
        {
            var normalizedReference = reference.ToLower();
            advances = advances.Where(advance =>
                advance.AdvanceNumber.ToLower().StartsWith(normalizedReference));
        }

        var totalCount = await advances.CountAsync(cancellationToken);
        var rows = await SelectAdvanceRows(advances)
            .OrderByDescending(advance => advance.CreatedAt)
            .ThenByDescending(advance => advance.AdvanceId)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToArrayAsync(cancellationToken);

        return AccessResult<PagedResult<AdvanceSummary>>.Success(new PagedResult<AdvanceSummary>(
            rows.Select(MapAdvance).ToArray(),
            query.Page,
            query.PageSize,
            totalCount,
            CalculateTotalPages(totalCount, query.PageSize)));
    }

    public async Task<AccessResult<AdvanceDetails>> GetAsync(
        Guid advanceId,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<AdvanceDetails>.Failure(AccessResultStatus.Unauthorized);
        }

        return await LoadVisibleAdvanceAsync(caller, advanceId, cancellationToken);
    }

    public async Task<AccessResult<PagedResult<AdvanceMovementSummary>>> ListMovementsAsync(
        Guid advanceId,
        AdvanceMovementQuery query,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<PagedResult<AdvanceMovementSummary>>.Failure(
                AccessResultStatus.Unauthorized);
        }

        if (!IsValidPage(query.Page, query.PageSize))
        {
            return AccessResult<PagedResult<AdvanceMovementSummary>>.Failure(
                AccessResultStatus.Invalid);
        }

        var visibleAdvance = ApplyVisibility(
            dbContext.Advances.AsNoTracking().Where(advance =>
                advance.CompanyId == caller.CompanyId && advance.AdvanceId == advanceId),
            caller);
        if (!await visibleAdvance.AnyAsync(cancellationToken))
        {
            return AccessResult<PagedResult<AdvanceMovementSummary>>.Failure(
                AccessResultStatus.NotFound);
        }

        var fundingMovements = dbContext.AdvanceFundingSources
            .AsNoTracking()
            .Where(allocation =>
                allocation.CompanyId == caller.CompanyId && allocation.AdvanceId == advanceId)
            .Select(allocation => new MovementRow(
                allocation.AdvanceFundingSourceId,
                "FundingAllocation",
                allocation.AllocatedAmount,
                allocation.AllocatedByUserId,
                allocation.AppUser.FullName,
                allocation.AppUser.Role,
                (Guid?)null,
                null,
                null,
                allocation.Advance.DeputyUserId,
                allocation.Advance.AppUser2.FullName,
                allocation.Advance.AppUser2.Role,
                "Recorded",
                allocation.CreatedAt));

        var deliveryMovements = dbContext.MoneyTransfers
            .AsNoTracking()
            .Where(transfer =>
                transfer.CompanyId == caller.CompanyId
                && transfer.AdvanceId == advanceId
                && transfer.TransferType == AdvanceConstants.AdvanceDeliveryTransferType)
            .Select(transfer => new MovementRow(
                transfer.MoneyTransferId,
                transfer.TransferType,
                transfer.TransferAmount,
                transfer.InitiatedByUserId,
                transfer.AppUser1.FullName,
                transfer.AppUser1.Role,
                transfer.SenderUserId,
                transfer.AppUser5.FullName,
                transfer.AppUser5.Role,
                transfer.RecipientUserId,
                transfer.AppUser2.FullName,
                transfer.AppUser2.Role,
                transfer.Status,
                transfer.CreatedAt));

        var allocatedTransferMovements = dbContext.TransferAdvanceAllocations
            .AsNoTracking()
            .Where(allocation =>
                allocation.CompanyId == caller.CompanyId
                && allocation.AdvanceId == advanceId
                && allocation.MoneyTransfer.TransferType != AdvanceConstants.AdvanceDeliveryTransferType)
            .Select(allocation => new MovementRow(
                allocation.MoneyTransferId,
                allocation.MoneyTransfer.TransferType,
                allocation.AllocatedAmount,
                allocation.MoneyTransfer.InitiatedByUserId,
                allocation.MoneyTransfer.AppUser1.FullName,
                allocation.MoneyTransfer.AppUser1.Role,
                allocation.MoneyTransfer.SenderUserId,
                allocation.MoneyTransfer.AppUser5.FullName,
                allocation.MoneyTransfer.AppUser5.Role,
                allocation.MoneyTransfer.RecipientUserId,
                allocation.MoneyTransfer.AppUser2.FullName,
                allocation.MoneyTransfer.AppUser2.Role,
                allocation.MoneyTransfer.Status,
                allocation.MoneyTransfer.CreatedAt));

        var movements = fundingMovements.Concat(deliveryMovements).Concat(allocatedTransferMovements);
        var totalCount = await movements.CountAsync(cancellationToken);
        var rows = await movements
            .OrderBy(row => row.OccurredAt)
            .ThenBy(row => row.MovementId)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToArrayAsync(cancellationToken);

        return AccessResult<PagedResult<AdvanceMovementSummary>>.Success(
            new PagedResult<AdvanceMovementSummary>(
                rows.Select(MapMovement).ToArray(),
                query.Page,
                query.PageSize,
                totalCount,
                CalculateTotalPages(totalCount, query.PageSize)));
    }

    public async Task<AccessResult<AdvanceDetails>> CreateAsync(
        CreateAdvanceRequest request,
        string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<AdvanceDetails>.Failure(AccessResultStatus.Unauthorized);
        }

        if (!caller.Capabilities.CanCreateTopLevelAdvance
            || !Validate(request)
            || !AdvanceRules.IsValidIdempotencyKey(idempotencyKey))
        {
            return AccessResult<AdvanceDetails>.Failure(AccessResultStatus.Invalid);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        try
        {
            var idempotency = await BeginIdempotencyAsync(
                caller,
                idempotencyKey!,
                "advances.create",
                "/api/v1/advances",
                Fingerprint("advances.create", request),
                cancellationToken);
            if (idempotency.Conflict)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceDetails>.Failure(AccessResultStatus.Conflict);
            }

            if (idempotency.ReplayResourceId is not null)
            {
                var replay = DeserializeReplay<AdvanceDetails>(idempotency.Record!.ResponsePayload);
                if (replay is null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return AccessResult<AdvanceDetails>.Failure(AccessResultStatus.Conflict);
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return AccessResult<AdvanceDetails>.Success(replay);
            }

            var recipient = await dbContext.AppUsers.AsNoTracking().SingleOrDefaultAsync(user =>
                user.CompanyId == caller.CompanyId
                && user.UserId == request.RecipientUserId
                && user.Company.Status == IdentityConstants.ActiveStatus,
                cancellationToken);
            if (recipient is null
                || !AdvanceRules.IsEligibleTopLevelRecipient(recipient.Role, recipient.Status))
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceDetails>.Failure(AccessResultStatus.Invalid);
            }

            var correlationId = Guid.NewGuid();
            string? currencyCode = null;
            var lockedSources = new Dictionary<Guid, FundingSource>();

            foreach (var funding in request.Fundings.OrderBy(item => item.FundingSourceId))
            {
                var source = await dbContext.FundingSources
                    .FromSqlInterpolated(
                        $"SELECT * FROM ahdah.funding_sources WHERE company_id = {caller.CompanyId} AND funding_source_id = {funding.FundingSourceId} FOR UPDATE")
                    .SingleOrDefaultAsync(cancellationToken);
                if (source is null
                    || source.Status is not (AdvanceConstants.ActiveFundingStatus
                        or AdvanceConstants.PartiallyUsedFundingStatus)
                    || source.AvailableAmount < funding.Amount
                    || currencyCode is not null && currencyCode != source.CurrencyCode)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return AccessResult<AdvanceDetails>.Failure(AccessResultStatus.Conflict);
                }

                currencyCode ??= source.CurrencyCode;
                lockedSources.Add(source.FundingSourceId, source);
            }

            var advance = new Advance
            {
                CompanyId = caller.CompanyId,
                AdvanceNumber = CreateReference("ADV", Guid.NewGuid()),
                DeputyUserId = recipient.UserId,
                AdvanceAmount = request.Amount,
                CurrencyCode = currencyCode ?? AdvanceConstants.DefaultCurrencyCode,
                IssueDate = request.IssueDate,
                SettlementDueDate = request.SettlementDueDate,
                Purpose = request.Purpose.Trim(),
                Notes = NormalizeOptional(request.Notes),
                Status = AdvanceConstants.PendingConfirmationStatus,
                CreatedByUserId = caller.UserId
            };
            dbContext.Advances.Add(advance);
            await dbContext.SaveChangesAsync(cancellationToken);
            var advanceId = advance.AdvanceId;

            var delivery = NewTransfer(
                caller,
                AdvanceConstants.AdvanceDeliveryTransferType,
                advanceId,
                caller.UserId,
                recipient.UserId,
                request.Amount,
                advance.CurrencyCode,
                request);
            dbContext.MoneyTransfers.Add(delivery);

            foreach (var funding in request.Fundings)
            {
                var source = lockedSources[funding.FundingSourceId];
                source.AvailableAmount -= funding.Amount;
                source.ReservedAmount += funding.Amount;
                source.VersionNumber++;
                source.Status = FundingStatus(source);

                dbContext.AdvanceFundingSources.Add(new AdvanceFundingSource
                {
                    CompanyId = caller.CompanyId,
                    AdvanceId = advanceId,
                    FundingSourceId = source.FundingSourceId,
                    AllocatedAmount = funding.Amount,
                    AllocatedByUserId = caller.UserId,
                    Notes = NormalizeOptional(funding.Notes)
                });
                AddFundingLedger(
                    source,
                    "AmountReserved",
                    funding.Amount,
                    -funding.Amount,
                    funding.Amount,
                    0m,
                    caller.UserId,
                    "Advance",
                    advanceId,
                    correlationId,
                    "Reserved for advance delivery.");
            }

            CompleteIdempotency(
                idempotency.Record!,
                "Advance",
                advanceId,
                advance.VersionNumber,
                201);
            await dbContext.SaveChangesAsync(cancellationToken);
            var result = await LoadVisibleAdvanceAsync(caller, advanceId, cancellationToken);
            if (result.Status != AccessResultStatus.Success)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceDetails>.Failure(AccessResultStatus.Conflict);
            }

            SetResponsePayload(idempotency.Record!, result.Value!);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<AdvanceDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<AdvanceDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<AccessResult<AdvanceTransferDetails>> DistributeAsync(
        Guid advanceId,
        CreateAdvanceDistributionRequest request,
        string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Unauthorized);
        }

        if (!caller.Capabilities.CanDistributeHeldBalance
            || !Validate(request)
            || !AdvanceRules.IsValidIdempotencyKey(idempotencyKey)
            || !AdvanceRules.IsDifferentUser(caller.UserId, request.RecipientUserId))
        {
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Invalid);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        try
        {
            var idempotency = await BeginIdempotencyAsync(
                caller,
                idempotencyKey!,
                "advances.distribute",
                $"/api/v1/advances/{advanceId}/distributions",
                Fingerprint("advances.distribute", new { advanceId, request }),
                cancellationToken);
            if (idempotency.Conflict)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
            }

            if (idempotency.ReplayResourceId is not null)
            {
                var replay = DeserializeReplay<AdvanceTransferDetails>(
                    idempotency.Record!.ResponsePayload);
                if (replay is null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Success(replay);
            }

            var advance = await dbContext.Advances.AsNoTracking().SingleOrDefaultAsync(candidate =>
                candidate.CompanyId == caller.CompanyId && candidate.AdvanceId == advanceId,
                cancellationToken);
            if (advance is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.NotFound);
            }

            if (advance.Status != AdvanceConstants.OpenStatus)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Invalid);
            }

            var custodyPlan = await PrepareCustodyReservationAsync(
                caller, advanceId, request.Amount, request.CustodyAllocations, cancellationToken);
            var balance = custodyPlan?.Balance;
            if (balance is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
            }

            var recipient = await dbContext.AppUsers.AsNoTracking().SingleOrDefaultAsync(user =>
                user.CompanyId == caller.CompanyId
                && user.UserId == request.RecipientUserId
                && user.Company.Status == IdentityConstants.ActiveStatus,
                cancellationToken);
            if (recipient is null
                || !AdvanceRules.IsEligibleDistributionRecipient(
                    caller.Role,
                    recipient.Role,
                    recipient.Status))
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Invalid);
            }

            var transfer = NewTransfer(
                caller,
                AdvanceConstants.InternalTransferType,
                null,
                caller.UserId,
                recipient.UserId,
                request.Amount,
                advance.CurrencyCode,
                request);
            dbContext.MoneyTransfers.Add(transfer);
            await dbContext.SaveChangesAsync(cancellationToken);
            var transferId = transfer.MoneyTransferId;

            var transferAllocation = new TransferAdvanceAllocation
            {
                TransferAdvanceAllocationId = Guid.NewGuid(),
                CompanyId = caller.CompanyId,
                MoneyTransferId = transferId,
                AdvanceId = advanceId,
                AllocatedAmount = request.Amount,
                AllocatedByUserId = caller.UserId,
                Notes = NormalizeOptional(request.Notes)
            };
            dbContext.TransferAdvanceAllocations.Add(transferAllocation);
            var custodyOperation = ApplyCustodyReservation(
                caller, custodyPlan!, transfer, transferAllocation, idempotency.Record!, isReturn: false);
            await dbContext.SaveChangesAsync(cancellationToken);
            PostCustodyOperation(custodyOperation);

            CompleteIdempotency(
                idempotency.Record!,
                "MoneyTransfer",
                transferId,
                transfer.VersionNumber,
                201);
            await dbContext.SaveChangesAsync(cancellationToken);
            var result = await LoadTransferAsync(caller.CompanyId, transferId, cancellationToken);
            if (result.Status != AccessResultStatus.Success)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
            }

            SetResponsePayload(idempotency.Record!, result.Value!);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch (PostgresException exception) when (IsPostgresConflict(exception))
        {
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<AccessResult<AdvanceTransferDetails>> ReturnAsync(
        Guid advanceId,
        CreateAdvanceReturnRequest request,
        string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Unauthorized);
        }

        if (!caller.Capabilities.CanReturnHeldBalance
            || !Validate(request)
            || !AdvanceRules.IsValidIdempotencyKey(idempotencyKey))
        {
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Invalid);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        try
        {
            var idempotency = await BeginIdempotencyAsync(
                caller,
                idempotencyKey!,
                "advances.return",
                $"/api/v1/advances/{advanceId}/returns",
                Fingerprint("advances.return", new { advanceId, request }),
                cancellationToken);
            if (idempotency.Conflict)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
            }

            if (idempotency.ReplayResourceId is not null)
            {
                var replay = DeserializeReplay<AdvanceTransferDetails>(
                    idempotency.Record!.ResponsePayload);
                if (replay is null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Success(replay);
            }

            var advance = await dbContext.Advances.AsNoTracking().SingleOrDefaultAsync(candidate =>
                candidate.CompanyId == caller.CompanyId && candidate.AdvanceId == advanceId,
                cancellationToken);
            if (advance is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.NotFound);
            }

            if (advance.Status != AdvanceConstants.OpenStatus)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Invalid);
            }

            var custodyPlan = await PrepareCustodyReservationAsync(
                caller, advanceId, request.Amount, request.CustodyAllocations, cancellationToken);
            var balance = custodyPlan?.Balance;
            if (balance is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
            }

            var deliverySenders = dbContext.MoneyTransfers.AsNoTracking()
                .Where(transfer =>
                    transfer.CompanyId == caller.CompanyId
                    && transfer.AdvanceId == advanceId
                    && transfer.TransferType == AdvanceConstants.AdvanceDeliveryTransferType
                    && transfer.Status == AdvanceConstants.ConfirmedTransferStatus
                    && transfer.RecipientUserId == caller.UserId)
                .Select(transfer => transfer.SenderUserId);
            var distributionSenders = dbContext.TransferAdvanceAllocations.AsNoTracking()
                .Where(allocation =>
                    allocation.CompanyId == caller.CompanyId
                    && allocation.AdvanceId == advanceId
                    && allocation.MoneyTransfer.TransferType == AdvanceConstants.InternalTransferType
                    && allocation.MoneyTransfer.Status == AdvanceConstants.ConfirmedTransferStatus
                    && allocation.MoneyTransfer.RecipientUserId == caller.UserId)
                .Select(allocation => allocation.MoneyTransfer.SenderUserId);
            var upstreamSenders = await deliverySenders
                .Concat(distributionSenders)
                .Distinct()
                .Take(2)
                .ToArrayAsync(cancellationToken);
            if (upstreamSenders.Length != 1 || upstreamSenders[0] == caller.UserId)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Invalid);
            }

            var recipient = await dbContext.AppUsers.AsNoTracking().SingleOrDefaultAsync(user =>
                user.CompanyId == caller.CompanyId
                && user.UserId == upstreamSenders[0]
                && user.Status == AccessConstants.ActiveUserStatus
                && user.Company.Status == IdentityConstants.ActiveStatus,
                cancellationToken);
            if (recipient is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Invalid);
            }

            var transfer = NewTransfer(
                caller,
                AdvanceConstants.BalanceReturnTransferType,
                null,
                caller.UserId,
                recipient.UserId,
                request.Amount,
                advance.CurrencyCode,
                request);
            dbContext.MoneyTransfers.Add(transfer);
            await dbContext.SaveChangesAsync(cancellationToken);
            var transferId = transfer.MoneyTransferId;

            var transferAllocation = new TransferAdvanceAllocation
            {
                TransferAdvanceAllocationId = Guid.NewGuid(),
                CompanyId = caller.CompanyId,
                MoneyTransferId = transferId,
                AdvanceId = advanceId,
                AllocatedAmount = request.Amount,
                AllocatedByUserId = caller.UserId,
                Notes = NormalizeOptional(request.Notes)
            };
            dbContext.TransferAdvanceAllocations.Add(transferAllocation);
            var custodyOperation = ApplyCustodyReservation(
                caller, custodyPlan!, transfer, transferAllocation, idempotency.Record!, isReturn: true);
            await dbContext.SaveChangesAsync(cancellationToken);
            PostCustodyOperation(custodyOperation);

            CompleteIdempotency(
                idempotency.Record!,
                "MoneyTransfer",
                transferId,
                transfer.VersionNumber,
                201);
            await dbContext.SaveChangesAsync(cancellationToken);
            var result = await LoadTransferAsync(caller.CompanyId, transferId, cancellationToken);
            if (result.Status != AccessResultStatus.Success)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
            }

            SetResponsePayload(idempotency.Record!, result.Value!);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch (PostgresException exception) when (IsPostgresConflict(exception))
        {
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<AccessResult<AdvanceTransferDetails>> ConfirmTransferAsync(
        Guid transferId,
        string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Unauthorized);
        }

        if (!caller.Capabilities.CanConfirmReceipt
            || !AdvanceRules.IsValidIdempotencyKey(idempotencyKey))
        {
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Forbidden);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        try
        {
            var idempotency = await BeginIdempotencyAsync(
                caller,
                idempotencyKey!,
                "advances.confirm_transfer",
                $"/api/v1/advance-transfers/{transferId}/confirm",
                Fingerprint("advances.confirm_transfer", new { transferId }),
                cancellationToken);
            if (idempotency.Conflict)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
            }

            if (idempotency.ReplayResourceId is not null)
            {
                var replay = DeserializeReplay<AdvanceTransferDetails>(
                    idempotency.Record!.ResponsePayload);
                if (replay is null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Success(replay);
            }

            var affectedProjectIds = await DiscoverTransferProjectIdsAsync(
                caller.CompanyId, transferId, cancellationToken);
            var controls = await financialGate.LockOpenAsync(
                caller.CompanyId, affectedProjectIds, cancellationToken);
            if (controls is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
            }

            var transfer = await LockTransferAsync(caller.CompanyId, transferId, cancellationToken);
            if (transfer is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.NotFound);
            }

            if (transfer.RecipientUserId != caller.UserId)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.NotFound);
            }

            if (transfer.Status == AdvanceConstants.ConfirmedTransferStatus
                && transfer.ConfirmedByUserId == caller.UserId)
            {
                CompleteIdempotency(
                    idempotency.Record!,
                    "MoneyTransfer",
                    transferId,
                    transfer.VersionNumber,
                    200);
                await dbContext.SaveChangesAsync(cancellationToken);
                var existingResult = await LoadTransferAsync(caller.CompanyId, transferId, cancellationToken);
                if (existingResult.Status != AccessResultStatus.Success)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
                }

                SetResponsePayload(idempotency.Record!, existingResult.Value!);
                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return existingResult;
            }

            if (transfer.Status != AdvanceConstants.PendingConfirmationStatus)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
            }

            var actionSucceeded = transfer.TransferType switch
            {
                AdvanceConstants.AdvanceDeliveryTransferType => await ConfirmDeliveryAsync(
                    caller,
                    transfer,
                    idempotency.Record!,
                    cancellationToken),
                AdvanceConstants.InternalTransferType => await ConfirmAllocatedTransferAsync(
                    caller,
                    transfer,
                    idempotency.Record!,
                    controls,
                    isReturn: false,
                    cancellationToken),
                AdvanceConstants.BalanceReturnTransferType => await ConfirmAllocatedTransferAsync(
                    caller,
                    transfer,
                    idempotency.Record!,
                    controls,
                    isReturn: true,
                    cancellationToken),
                _ => false
            };
            if (!actionSucceeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
            }

            var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
            transfer.Status = AdvanceConstants.ConfirmedTransferStatus;
            transfer.ConfirmedByUserId = caller.UserId;
            transfer.ConfirmedAt = nowUtc;
            transfer.VersionNumber++;

            CompleteIdempotency(
                idempotency.Record!,
                "MoneyTransfer",
                transferId,
                transfer.VersionNumber,
                200);
            await dbContext.SaveChangesAsync(cancellationToken);
            var result = await LoadTransferAsync(caller.CompanyId, transferId, cancellationToken);
            if (result.Status != AccessResultStatus.Success)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
            }

            SetResponsePayload(idempotency.Record!, result.Value!);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch (PostgresException exception) when (IsPostgresConflict(exception))
        {
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<AccessResult<AdvanceTransferDetails>> RejectTransferAsync(
        Guid transferId,
        RejectAdvanceTransferRequest request,
        string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Unauthorized);
        }

        if (!caller.Capabilities.CanConfirmReceipt
            || !Validate(request)
            || !AdvanceRules.IsValidIdempotencyKey(idempotencyKey))
        {
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Forbidden);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        try
        {
            var idempotency = await BeginIdempotencyAsync(
                caller,
                idempotencyKey!,
                "advances.reject_transfer",
                $"/api/v1/advance-transfers/{transferId}/reject",
                Fingerprint("advances.reject_transfer", new { transferId, request }),
                cancellationToken);
            if (idempotency.Conflict)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
            }

            if (idempotency.ReplayResourceId is not null)
            {
                var replay = DeserializeReplay<AdvanceTransferDetails>(
                    idempotency.Record!.ResponsePayload);
                if (replay is null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Success(replay);
            }

            var affectedProjectIds = await DiscoverTransferProjectIdsAsync(
                caller.CompanyId, transferId, cancellationToken);
            var controls = await financialGate.LockOpenAsync(
                caller.CompanyId, affectedProjectIds, cancellationToken);
            if (controls is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
            }

            var transfer = await LockTransferAsync(caller.CompanyId, transferId, cancellationToken);
            if (transfer is null || transfer.RecipientUserId != caller.UserId)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.NotFound);
            }

            if (transfer.Status == AdvanceConstants.RejectedTransferStatus
                && transfer.RejectedByUserId == caller.UserId)
            {
                CompleteIdempotency(
                    idempotency.Record!,
                    "MoneyTransfer",
                    transferId,
                    transfer.VersionNumber,
                    200);
                await dbContext.SaveChangesAsync(cancellationToken);
                var existingResult = await LoadTransferAsync(caller.CompanyId, transferId, cancellationToken);
                if (existingResult.Status != AccessResultStatus.Success)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
                }

                SetResponsePayload(idempotency.Record!, existingResult.Value!);
                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return existingResult;
            }

            if (transfer.Status != AdvanceConstants.PendingConfirmationStatus
                || transfer.TransferType == AdvanceConstants.AdvanceDeliveryTransferType)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Invalid);
            }

            var allocation = await dbContext.TransferAdvanceAllocations.AsNoTracking()
                .SingleOrDefaultAsync(candidate =>
                    candidate.CompanyId == caller.CompanyId
                    && candidate.MoneyTransferId == transferId,
                    cancellationToken);
            if (allocation is null || allocation.AllocatedAmount != transfer.TransferAmount)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
            }

            if (!await RejectCustodyReservationAsync(
                    caller, transfer, allocation, idempotency.Record!, controls, cancellationToken))
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
            }

            transfer.Status = AdvanceConstants.RejectedTransferStatus;
            transfer.RejectedByUserId = caller.UserId;
            transfer.RejectedAt = timeProvider.GetUtcNow().UtcDateTime;
            transfer.RejectionReason = request.Reason.Trim();
            transfer.VersionNumber++;

            CompleteIdempotency(
                idempotency.Record!,
                "MoneyTransfer",
                transferId,
                transfer.VersionNumber,
                200);
            await dbContext.SaveChangesAsync(cancellationToken);
            var result = await LoadTransferAsync(caller.CompanyId, transferId, cancellationToken);
            if (result.Status != AccessResultStatus.Success)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
            }

            SetResponsePayload(idempotency.Record!, result.Value!);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch (PostgresException exception) when (IsPostgresConflict(exception))
        {
            return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<AccessResult<AdvanceBalancePage>> GetMyBalancesAsync(
        AdvanceBalanceQuery query,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<AdvanceBalancePage>.Failure(AccessResultStatus.Unauthorized);
        }

        return await LoadBalancePageAsync(caller, caller.UserId, query, cancellationToken);
    }

    public async Task<AccessResult<AdvanceBalancePage>> GetUserBalancesAsync(
        Guid userId,
        AdvanceBalanceQuery query,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<AdvanceBalancePage>.Failure(AccessResultStatus.Unauthorized);
        }

        if (!caller.Capabilities.CanViewOtherUserBalances)
        {
            return AccessResult<AdvanceBalancePage>.Failure(AccessResultStatus.Forbidden);
        }

        var userExists = await dbContext.AppUsers.AsNoTracking().AnyAsync(user =>
            user.CompanyId == caller.CompanyId && user.UserId == userId,
            cancellationToken);
        if (!userExists)
        {
            return AccessResult<AdvanceBalancePage>.Failure(AccessResultStatus.NotFound);
        }

        return await LoadBalancePageAsync(caller, userId, query, cancellationToken);
    }

    public async Task<AccessResult<PagedResult<AvailableFundingSourceSummary>>>
        ListAvailableFundingSourcesAsync(
            AvailableFundingSourceQuery query,
            CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<PagedResult<AvailableFundingSourceSummary>>.Failure(
                AccessResultStatus.Unauthorized);
        }

        if (!caller.Capabilities.CanViewAvailableFundingSources)
        {
            return AccessResult<PagedResult<AvailableFundingSourceSummary>>.Failure(
                AccessResultStatus.Forbidden);
        }

        if (!IsValidPage(query.Page, query.PageSize))
        {
            return AccessResult<PagedResult<AvailableFundingSourceSummary>>.Failure(
                AccessResultStatus.Invalid);
        }

        var sources = dbContext.FundingSources.AsNoTracking().Where(source =>
            source.CompanyId == caller.CompanyId
            && source.AvailableAmount > 0m
            && (source.Status == AdvanceConstants.ActiveFundingStatus
                || source.Status == AdvanceConstants.PartiallyUsedFundingStatus));
        var totalCount = await sources.CountAsync(cancellationToken);
        var rows = await sources
            .OrderBy(source => source.SourceDate)
            .ThenBy(source => source.FundingSourceId)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(source => new FundingSourceRow(
                source.FundingSourceId,
                source.SourceType,
                source.SourceDate,
                source.CurrencyCode,
                source.AvailableAmount,
                source.Status))
            .ToArrayAsync(cancellationToken);
        var sourceIds = rows.Select(row => row.FundingSourceId).ToArray();
        var paymentRows = await dbContext.FundingSourcePaymentMethods.AsNoTracking()
            .Where(method =>
                method.CompanyId == caller.CompanyId && sourceIds.Contains(method.FundingSourceId))
            .OrderBy(method => method.SequenceNumber)
            .Select(method => new { method.FundingSourceId, method.PaymentMethod })
            .ToArrayAsync(cancellationToken);
        var paymentLookup = paymentRows
            .GroupBy(row => row.FundingSourceId)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<string>)group.Select(row => row.PaymentMethod).ToArray());

        return AccessResult<PagedResult<AvailableFundingSourceSummary>>.Success(
            new PagedResult<AvailableFundingSourceSummary>(
                rows.Select(row => new AvailableFundingSourceSummary(
                    row.FundingSourceId,
                    row.SourceType,
                    row.SourceDate,
                    row.CurrencyCode,
                    row.AvailableAmount,
                    row.Status,
                    paymentLookup.GetValueOrDefault(row.FundingSourceId, [])))
                    .ToArray(),
                query.Page,
                query.PageSize,
                totalCount,
                CalculateTotalPages(totalCount, query.PageSize)));
    }

    private async Task<bool> ConfirmDeliveryAsync(
        Caller caller,
        MoneyTransfer transfer,
        IdempotencyRecord idempotencyRecord,
        CancellationToken cancellationToken)
    {
        if (transfer.AdvanceId is not { } advanceId)
        {
            return false;
        }

        var advance = await dbContext.Advances
            .FromSqlInterpolated(
                $"SELECT * FROM ahdah.advances WHERE company_id = {caller.CompanyId} AND advance_id = {advanceId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);
        if (advance is null
            || advance.Status != AdvanceConstants.PendingConfirmationStatus
            || advance.DeputyUserId != caller.UserId
            || advance.AdvanceAmount != transfer.TransferAmount)
        {
            return false;
        }

        var allocations = await dbContext.AdvanceFundingSources.AsNoTracking()
            .Where(allocation =>
                allocation.CompanyId == caller.CompanyId && allocation.AdvanceId == advanceId)
            .OrderBy(allocation => allocation.FundingSourceId)
            .ToArrayAsync(cancellationToken);
        if (allocations.Length == 0
            || allocations.Sum(allocation => allocation.AllocatedAmount) != advance.AdvanceAmount)
        {
            return false;
        }

        var correlationId = Guid.NewGuid();
        foreach (var allocation in allocations)
        {
            var source = await dbContext.FundingSources
                .FromSqlInterpolated(
                    $"SELECT * FROM ahdah.funding_sources WHERE company_id = {caller.CompanyId} AND funding_source_id = {allocation.FundingSourceId} FOR UPDATE")
                .SingleOrDefaultAsync(cancellationToken);
            if (source is null || source.ReservedAmount < allocation.AllocatedAmount)
            {
                return false;
            }

            source.ReservedAmount -= allocation.AllocatedAmount;
            source.UsedAmount += allocation.AllocatedAmount;
            source.VersionNumber++;
            source.Status = FundingStatus(source);
            AddFundingLedger(
                source,
                "AdvanceFunding",
                allocation.AllocatedAmount,
                0m,
                -allocation.AllocatedAmount,
                allocation.AllocatedAmount,
                caller.UserId,
                "Advance",
                advanceId,
                correlationId,
                "Advance delivery confirmed by recipient.");
        }

        var balanceExists = await dbContext.UserAdvanceBalances.AsNoTracking().AnyAsync(balance =>
            balance.CompanyId == caller.CompanyId
            && balance.AdvanceId == advanceId
            && balance.UserId == caller.UserId,
            cancellationToken);
        if (balanceExists)
        {
            return false;
        }

        var balance = NewBalance(
            caller.CompanyId,
            advanceId,
            caller.UserId,
            caller.UserId,
            receivedAmount: advance.AdvanceAmount,
            restoredAmount: 0m);
        dbContext.UserAdvanceBalances.Add(balance);
        var balanceLedgerEntry = AddBalanceLedger(
            balance,
            "BalanceCreated",
            advance.AdvanceAmount,
            0m,
            0m,
            0m,
            0m,
            advance.AdvanceAmount,
            0m,
            caller.UserId,
            advanceId,
            "Initial advance balance created after confirmation.",
            referenceType: "Advance");

        var openingPosition = new ProjectCustodyPosition
        {
            ProjectCustodyPositionId = Guid.NewGuid(),
            CompanyId = caller.CompanyId,
            UserAdvanceBalance = balance,
            ProjectId = null,
            AvailableAmount = advance.AdvanceAmount,
            ReservedAmount = 0m,
            VersionNumber = 2
        };
        var openingOperation = new CustodyOperation
        {
            CustodyOperationId = Guid.NewGuid(),
            CompanyId = caller.CompanyId,
            OperationType = "Opening",
            Status = "Draft",
            RequestedByUserId = caller.UserId,
            RequestedAt = timeProvider.GetUtcNow().UtcDateTime,
            Reason = "Advance delivery confirmed; custody opened from the authoritative balance receipt.",
            IdempotencyRecordId = idempotencyRecord.IdempotencyRecordId,
            MoneyTransferId = transfer.MoneyTransferId,
            VersionNumber = 1
        };
        dbContext.ProjectCustodyPositions.Add(openingPosition);
        dbContext.CustodyOperations.Add(openingOperation);
        dbContext.CustodyMovementEntries.Add(new CustodyMovementEntry
        {
            CustodyMovementEntryId = Guid.NewGuid(),
            CompanyId = caller.CompanyId,
            CustodyOperation = openingOperation,
            ProjectCustodyPosition = openingPosition,
            EntryNumber = 1,
            DeltaAvailableAmount = advance.AdvanceAmount,
            DeltaReservedAmount = 0m,
            ReceivedAmount = advance.AdvanceAmount,
            BalanceLedgerEntry = balanceLedgerEntry,
            OccurredAt = timeProvider.GetUtcNow().UtcDateTime,
            ActorUserId = caller.UserId,
            PositionVersionBefore = 1,
            PositionVersionAfter = 2
        });

        // Insert the immutable effect while the header is mutable, then post it in the same transaction.
        await dbContext.SaveChangesAsync(cancellationToken);
        openingOperation.Status = "Posted";
        openingOperation.UpdatedAt = timeProvider.GetUtcNow().UtcDateTime;
        openingOperation.VersionNumber++;

        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        advance.Status = AdvanceConstants.OpenStatus;
        advance.ConfirmedByUserId = caller.UserId;
        advance.ConfirmedAt = nowUtc;
        advance.VersionNumber++;
        return true;
    }

    private async Task<bool> ConfirmAllocatedTransferAsync(
        Caller caller,
        MoneyTransfer transfer,
        IdempotencyRecord idempotency,
        IReadOnlyList<ProjectFinancialControl> controls,
        bool isReturn,
        CancellationToken cancellationToken)
    {
        var allocation = await dbContext.TransferAdvanceAllocations.AsNoTracking()
            .SingleOrDefaultAsync(candidate =>
                candidate.CompanyId == caller.CompanyId
                && candidate.MoneyTransferId == transfer.MoneyTransferId,
                cancellationToken);
        if (allocation is null || allocation.AllocatedAmount != transfer.TransferAmount)
        {
            return false;
        }

        var balances = await LockBalancesAsync(caller.CompanyId, allocation.AdvanceId,
            [transfer.SenderUserId, caller.UserId], cancellationToken);
        var senderBalance = balances.SingleOrDefault(value => value.UserId == transfer.SenderUserId);
        if (senderBalance is null
            || senderBalance.Status != AdvanceConstants.ActiveBalanceStatus
            || senderBalance.ReservedAmount < transfer.TransferAmount)
        {
            return false;
        }

        var recipientBalance = balances.SingleOrDefault(value => value.UserId == caller.UserId);
        var recipientWasCreated = recipientBalance is null;
        if (recipientBalance is null)
        {
            recipientBalance = NewBalance(
                caller.CompanyId,
                allocation.AdvanceId,
                caller.UserId,
                caller.UserId,
                isReturn ? 0m : transfer.TransferAmount,
                isReturn ? transfer.TransferAmount : 0m);
            dbContext.UserAdvanceBalances.Add(recipientBalance);
        }
        else if (recipientBalance.Status != AdvanceConstants.ActiveBalanceStatus)
        {
            return false;
        }

        if (!recipientWasCreated)
        {
            if (isReturn)
            {
                recipientBalance.TotalRestoredAmount += transfer.TransferAmount;
            }
            else
            {
                recipientBalance.TotalReceivedAmount += transfer.TransferAmount;
            }

            recipientBalance.AvailableAmount += transfer.TransferAmount;
            recipientBalance.VersionNumber++;
        }

        senderBalance.ReservedAmount -= transfer.TransferAmount;
        if (isReturn)
        {
            senderBalance.TotalReturnedAmount += transfer.TransferAmount;
        }
        else
        {
            senderBalance.TotalTransferredOutAmount += transfer.TransferAmount;
        }

        senderBalance.VersionNumber++;

        var reservations = await dbContext.CustodyReservations
            .FromSqlInterpolated($"SELECT reservation.* FROM ahdah.custody_reservations reservation JOIN ahdah.transfer_advance_allocations allocation ON allocation.company_id = reservation.company_id AND allocation.transfer_advance_allocation_id = reservation.transfer_advance_allocation_id WHERE reservation.company_id = {caller.CompanyId} AND allocation.money_transfer_id = {transfer.MoneyTransferId} ORDER BY reservation.project_custody_position_id FOR UPDATE OF reservation")
            .ToArrayAsync(cancellationToken);
        if (reservations.Length == 0)
        {
            var attributedBalanceExists = await dbContext.ProjectCustodyPositions.AsNoTracking()
                .AnyAsync(position => position.CompanyId == caller.CompanyId
                    && position.UserAdvanceBalance.AdvanceId == allocation.AdvanceId
                    && (position.UserAdvanceBalance.UserId == transfer.SenderUserId
                        || position.UserAdvanceBalance.UserId == caller.UserId), cancellationToken);
            if (attributedBalanceExists || controls.Count != 0)
            {
                return false;
            }
        }
        else
        {
            if (reservations.Any(value => value.Status != "Reserved")
                || reservations.Sum(value => value.Amount) != transfer.TransferAmount)
            {
                return false;
            }

            var sourceIds = reservations.Select(value => value.ProjectCustodyPositionId)
                .OrderBy(value => value).ToArray();
            var sources = await dbContext.ProjectCustodyPositions
                .FromSqlInterpolated($"SELECT * FROM ahdah.project_custody_positions WHERE company_id = {caller.CompanyId} AND project_custody_position_id = ANY ({sourceIds}) ORDER BY project_custody_position_id FOR UPDATE")
                .ToArrayAsync(cancellationToken);
            if (sources.Length != reservations.Length
                || sources.Any(value => value.UserAdvanceBalanceId != senderBalance.UserAdvanceBalanceId))
            {
                return false;
            }

            var recipientPositions = await dbContext.ProjectCustodyPositions
                .FromSqlInterpolated($"SELECT * FROM ahdah.project_custody_positions WHERE company_id = {caller.CompanyId} AND user_advance_balance_id = {recipientBalance.UserAdvanceBalanceId} ORDER BY project_custody_position_id FOR UPDATE")
                .ToListAsync(cancellationToken);
            if (recipientPositions.All(value => value.ProjectId is not null))
            {
                var unassigned = new ProjectCustodyPosition
                {
                    ProjectCustodyPositionId = Guid.NewGuid(), CompanyId = caller.CompanyId,
                    UserAdvanceBalanceId = recipientBalance.UserAdvanceBalanceId,
                    ProjectId = null, AvailableAmount = 0m, ReservedAmount = 0m, VersionNumber = 1
                };
                recipientPositions.Add(unassigned);
                dbContext.ProjectCustodyPositions.Add(unassigned);
            }
            financialGate.AdvanceRevisions(controls);
            var revisions = controls.ToDictionary(value => value.ProjectId, value => value.FinancialRevision);
            var now = timeProvider.GetUtcNow().UtcDateTime;
            var operation = new CustodyOperation
            {
                CustodyOperationId = Guid.NewGuid(), CompanyId = caller.CompanyId,
                OperationType = "ConfirmTransfer", Status = "Draft",
                RequestedByUserId = caller.UserId, RequestedAt = now,
                Reason = isReturn ? "Confirmed balance return with preserved custody designation."
                    : "Confirmed internal transfer with preserved custody designation.",
                IdempotencyRecordId = idempotency.IdempotencyRecordId,
                MoneyTransferId = transfer.MoneyTransferId, VersionNumber = 1
            };
            dbContext.CustodyOperations.Add(operation);

            var sourceLedger = AddBalanceLedger(senderBalance,
                isReturn ? "BalanceReturnConfirmed" : "TransferOutConfirmed",
                0m, 0m, 0m, isReturn ? 0m : transfer.TransferAmount,
                isReturn ? transfer.TransferAmount : 0m, 0m, -transfer.TransferAmount,
                caller.UserId, transfer.MoneyTransferId,
                isReturn ? "Unused balance returned." : "Transfer confirmed by recipient.");
            var recipientLedger = AddBalanceLedger(recipientBalance,
                isReturn ? "AmountRestored" : "TransferInConfirmed",
                isReturn ? 0m : transfer.TransferAmount,
                isReturn ? transfer.TransferAmount : 0m,
                0m, 0m, 0m, transfer.TransferAmount, 0m,
                caller.UserId, transfer.MoneyTransferId,
                isReturn ? "Returned balance restored to upstream holder."
                    : "Incoming transfer confirmed.");

            var entryNumber = 1;
            foreach (var reservation in reservations)
            {
                var source = sources.Single(value =>
                    value.ProjectCustodyPositionId == reservation.ProjectCustodyPositionId);
                if (source.ReservedAmount < reservation.Amount)
                {
                    return false;
                }

                var target = recipientPositions.SingleOrDefault(value => value.ProjectId == source.ProjectId);
                if (target is null)
                {
                    target = new ProjectCustodyPosition
                    {
                        ProjectCustodyPositionId = Guid.NewGuid(), CompanyId = caller.CompanyId,
                        UserAdvanceBalanceId = recipientBalance.UserAdvanceBalanceId,
                        ProjectId = source.ProjectId, AvailableAmount = 0m,
                        ReservedAmount = 0m, VersionNumber = 1
                    };
                    recipientPositions.Add(target);
                    dbContext.ProjectCustodyPositions.Add(target);
                }

                var revision = source.ProjectId is { } projectId ? revisions[projectId] : (long?)null;
                var sourceBefore = source.VersionNumber;
                source.ReservedAmount -= reservation.Amount;
                source.VersionNumber++;
                source.UpdatedAt = now;
                var targetBefore = target.VersionNumber;
                target.AvailableAmount += reservation.Amount;
                target.VersionNumber++;
                target.UpdatedAt = now;
                dbContext.CustodyMovementEntries.AddRange(
                    TransferMovement(operation, source, entryNumber++, 0m, -reservation.Amount,
                        isReturn ? 0m : reservation.Amount, isReturn ? reservation.Amount : 0m,
                        sourceLedger, caller.UserId, sourceBefore, source.VersionNumber, revision, now),
                    TransferMovement(operation, target, entryNumber++, reservation.Amount, 0m,
                        0m, 0m, recipientLedger, caller.UserId, targetBefore,
                        target.VersionNumber, revision, now,
                        received: isReturn ? 0m : reservation.Amount,
                        restored: isReturn ? reservation.Amount : 0m));
                reservation.Status = "Consumed";
                reservation.ResolvedByOperationId = operation.CustodyOperationId;
                reservation.VersionNumber++;
                reservation.UpdatedAt = now;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            PostCustodyOperation(operation);
        }
        if (reservations.Length == 0)
        {
            AddBalanceLedger(senderBalance, isReturn ? "BalanceReturnConfirmed" : "TransferOutConfirmed",
                0m, 0m, 0m, isReturn ? 0m : transfer.TransferAmount,
                isReturn ? transfer.TransferAmount : 0m, 0m, -transfer.TransferAmount,
                caller.UserId, transfer.MoneyTransferId,
                isReturn ? "Unused balance returned." : "Transfer confirmed by recipient.");
            AddBalanceLedger(recipientBalance, isReturn ? "AmountRestored" : "TransferInConfirmed",
                isReturn ? 0m : transfer.TransferAmount, isReturn ? transfer.TransferAmount : 0m,
                0m, 0m, 0m, transfer.TransferAmount, 0m, caller.UserId,
                transfer.MoneyTransferId,
                isReturn ? "Returned balance restored to upstream holder."
                    : "Incoming transfer confirmed.");
        }
        return true;
    }

    private async Task<bool> RejectCustodyReservationAsync(
        Caller caller,
        MoneyTransfer transfer,
        TransferAdvanceAllocation allocation,
        IdempotencyRecord idempotency,
        IReadOnlyList<ProjectFinancialControl> controls,
        CancellationToken cancellationToken)
    {
        var senderBalance = await LockBalanceAsync(
            caller.CompanyId, allocation.AdvanceId, transfer.SenderUserId, cancellationToken);
        if (senderBalance is null || senderBalance.ReservedAmount < transfer.TransferAmount)
        {
            return false;
        }

        var reservations = await dbContext.CustodyReservations
            .FromSqlInterpolated($"SELECT reservation.* FROM ahdah.custody_reservations reservation JOIN ahdah.transfer_advance_allocations transfer_allocation ON transfer_allocation.company_id = reservation.company_id AND transfer_allocation.transfer_advance_allocation_id = reservation.transfer_advance_allocation_id WHERE reservation.company_id = {caller.CompanyId} AND transfer_allocation.money_transfer_id = {transfer.MoneyTransferId} ORDER BY reservation.project_custody_position_id FOR UPDATE OF reservation")
            .ToArrayAsync(cancellationToken);
        if (reservations.Length == 0)
        {
            if (controls.Count != 0 || await dbContext.ProjectCustodyPositions.AsNoTracking().AnyAsync(
                    value => value.CompanyId == caller.CompanyId
                        && value.UserAdvanceBalanceId == senderBalance.UserAdvanceBalanceId,
                    cancellationToken))
            {
                return false;
            }
        }
        else if (reservations.Any(value => value.Status != "Reserved")
                 || reservations.Sum(value => value.Amount) != transfer.TransferAmount)
        {
            return false;
        }

        senderBalance.AvailableAmount += transfer.TransferAmount;
        senderBalance.ReservedAmount -= transfer.TransferAmount;
        senderBalance.VersionNumber++;
        var ledger = AddBalanceLedger(senderBalance, "TransferReservationReleased",
            0m, 0m, 0m, 0m, 0m, transfer.TransferAmount, -transfer.TransferAmount,
            caller.UserId, transfer.MoneyTransferId,
            "Reservation released after recipient rejection.");
        if (reservations.Length == 0)
        {
            return true;
        }

        var positionIds = reservations.Select(value => value.ProjectCustodyPositionId)
            .OrderBy(value => value).ToArray();
        var positions = await dbContext.ProjectCustodyPositions
            .FromSqlInterpolated($"SELECT * FROM ahdah.project_custody_positions WHERE company_id = {caller.CompanyId} AND project_custody_position_id = ANY ({positionIds}) ORDER BY project_custody_position_id FOR UPDATE")
            .ToArrayAsync(cancellationToken);
        if (positions.Length != reservations.Length
            || positions.Any(value => value.UserAdvanceBalanceId != senderBalance.UserAdvanceBalanceId))
        {
            return false;
        }

        financialGate.AdvanceRevisions(controls);
        var revisions = controls.ToDictionary(value => value.ProjectId, value => value.FinancialRevision);
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var operation = new CustodyOperation
        {
            CustodyOperationId = Guid.NewGuid(), CompanyId = caller.CompanyId,
            OperationType = "RejectTransfer", Status = "Draft",
            RequestedByUserId = caller.UserId, RequestedAt = now,
            Reason = "Released explicit custody slices after transfer rejection.",
            IdempotencyRecordId = idempotency.IdempotencyRecordId,
            MoneyTransferId = transfer.MoneyTransferId, VersionNumber = 1
        };
        dbContext.CustodyOperations.Add(operation);
        var entryNumber = 1;
        foreach (var reservation in reservations)
        {
            var position = positions.Single(value =>
                value.ProjectCustodyPositionId == reservation.ProjectCustodyPositionId);
            if (position.ReservedAmount < reservation.Amount)
            {
                return false;
            }

            var before = position.VersionNumber;
            position.AvailableAmount += reservation.Amount;
            position.ReservedAmount -= reservation.Amount;
            position.VersionNumber++;
            position.UpdatedAt = now;
            dbContext.CustodyMovementEntries.Add(TransferMovement(
                operation, position, entryNumber++, reservation.Amount, -reservation.Amount,
                0m, 0m, ledger, caller.UserId, before, position.VersionNumber,
                position.ProjectId is { } projectId ? revisions[projectId] : null, now));
            reservation.Status = "Released";
            reservation.ResolvedByOperationId = operation.CustodyOperationId;
            reservation.VersionNumber++;
            reservation.UpdatedAt = now;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        PostCustodyOperation(operation);
        return true;
    }

    private async Task<AccessResult<AdvanceBalancePage>> LoadBalancePageAsync(
        Caller caller,
        Guid userId,
        AdvanceBalanceQuery query,
        CancellationToken cancellationToken)
    {
        if (!IsValidPage(query.Page, query.PageSize))
        {
            return AccessResult<AdvanceBalancePage>.Failure(AccessResultStatus.Invalid);
        }

        var balances = dbContext.UserAdvanceBalances.AsNoTracking().Where(balance =>
            balance.CompanyId == caller.CompanyId && balance.UserId == userId);
        var totalCount = await balances.CountAsync(cancellationToken);
        var totalAvailable = await balances.SumAsync(
            balance => (decimal?)balance.AvailableAmount,
            cancellationToken) ?? 0m;
        var rows = await SelectBalanceRows(balances)
            .OrderByDescending(balance => balance.UpdatedAt)
            .ThenByDescending(balance => balance.UserAdvanceBalanceId)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToArrayAsync(cancellationToken);
        var page = new PagedResult<AdvanceBalanceSummary>(
            rows.Select(MapBalance).ToArray(),
            query.Page,
            query.PageSize,
            totalCount,
            CalculateTotalPages(totalCount, query.PageSize));
        return AccessResult<AdvanceBalancePage>.Success(new AdvanceBalancePage(page, totalAvailable));
    }

    private async Task<AccessResult<AdvanceDetails>> LoadVisibleAdvanceAsync(
        Caller caller,
        Guid advanceId,
        CancellationToken cancellationToken)
    {
        var advances = ApplyVisibility(
            dbContext.Advances.AsNoTracking().Where(advance =>
                advance.CompanyId == caller.CompanyId && advance.AdvanceId == advanceId),
            caller);
        var row = await SelectAdvanceRows(advances).SingleOrDefaultAsync(cancellationToken);
        if (row is null)
        {
            return AccessResult<AdvanceDetails>.Failure(AccessResultStatus.NotFound);
        }

        var fundings = await dbContext.AdvanceFundingSources.AsNoTracking()
            .Where(allocation =>
                allocation.CompanyId == caller.CompanyId && allocation.AdvanceId == advanceId)
            .OrderBy(allocation => allocation.CreatedAt)
            .ThenBy(allocation => allocation.AdvanceFundingSourceId)
            .Select(allocation => new FundingRow(
                allocation.FundingSource.SourceType,
                allocation.AllocatedAmount,
                allocation.FundingSource.Status,
                allocation.CreatedAt,
                allocation.FundingSourceId))
            .ToArrayAsync(cancellationToken);
        var fundingSourceIds = fundings.Select(funding => funding.FundingSourceId).Distinct().ToArray();
        var paymentRows = await dbContext.FundingSourcePaymentMethods.AsNoTracking()
            .Where(method =>
                method.CompanyId == caller.CompanyId
                && fundingSourceIds.Contains(method.FundingSourceId))
            .OrderBy(method => method.SequenceNumber)
            .Select(method => new { method.FundingSourceId, method.PaymentMethod })
            .ToArrayAsync(cancellationToken);
        var paymentLookup = paymentRows.GroupBy(method => method.FundingSourceId).ToDictionary(
            group => group.Key,
            group => (IReadOnlyList<string>)group.Select(method => method.PaymentMethod).ToArray());

        var balanceRows = await SelectBalanceRows(dbContext.UserAdvanceBalances.AsNoTracking().Where(balance =>
                balance.CompanyId == caller.CompanyId && balance.AdvanceId == advanceId))
            .OrderBy(balance => balance.HolderName)
            .ThenBy(balance => balance.UserId)
            .ToArrayAsync(cancellationToken);
        var closureStatus = await dbContext.AdvanceClosures.AsNoTracking()
            .Where(closure =>
                closure.CompanyId == caller.CompanyId && closure.AdvanceId == advanceId)
            .OrderByDescending(closure => closure.CreatedAt)
            .Select(closure => closure.Status)
            .FirstOrDefaultAsync(cancellationToken);

        return AccessResult<AdvanceDetails>.Success(new AdvanceDetails(
            MapAdvance(row),
            fundings.Select(funding => new AdvanceFundingSummary(
                funding.SourceType,
                funding.AllocatedAmount,
                funding.Status,
                paymentLookup.GetValueOrDefault(funding.FundingSourceId, []),
                ToUtc(funding.CreatedAt))).ToArray(),
            balanceRows.Select(MapBalance).ToArray(),
            ClosureStatus: closureStatus));
    }

    private async Task<AccessResult<AdvanceTransferDetails>> LoadTransferAsync(
        Guid companyId,
        Guid transferId,
        CancellationToken cancellationToken)
    {
        var row = await dbContext.MoneyTransfers.AsNoTracking()
            .Where(transfer =>
                transfer.CompanyId == companyId && transfer.MoneyTransferId == transferId)
            .Select(transfer => new TransferRow(
                transfer.MoneyTransferId,
                transfer.TransferNumber,
                transfer.TransferType,
                transfer.AdvanceId ?? transfer.TransferAdvanceAllocations
                    .Select(allocation => allocation.AdvanceId)
                    .First(),
                transfer.TransferAmount,
                transfer.CurrencyCode,
                transfer.SenderUserId,
                transfer.AppUser5.FullName,
                transfer.AppUser5.Role,
                transfer.RecipientUserId,
                transfer.AppUser2.FullName,
                transfer.AppUser2.Role,
                transfer.TransferMethod,
                transfer.Status,
                transfer.VersionNumber,
                transfer.CreatedAt,
                transfer.ConfirmedAt,
                transfer.RejectedAt))
            .SingleOrDefaultAsync(cancellationToken);
        return row is null
            ? AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.NotFound)
            : AccessResult<AdvanceTransferDetails>.Success(MapTransfer(row));
    }

    private async Task<IdempotencyAttempt> BeginIdempotencyAsync(
        Caller caller,
        string idempotencyKey,
        string operationName,
        string requestPath,
        string fingerprint,
        CancellationToken cancellationToken)
    {
        var existing = await dbContext.IdempotencyRecords
            .FromSqlInterpolated(
                $"SELECT * FROM ahdah.idempotency_records WHERE company_id = {caller.CompanyId} AND idempotency_key = {idempotencyKey} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);
        if (existing is not null)
        {
            if (existing.OperationName != operationName
                || existing.RequestFingerprintHash != fingerprint
                || existing.ActorType != "User"
                || existing.ActorUserId != caller.UserId
                || existing.Status != "Completed"
                || existing.ResourceId is null)
            {
                return new IdempotencyAttempt(null, null, Conflict: true);
            }

            existing.ReplayCount++;
            existing.LastReplayedAt = timeProvider.GetUtcNow().UtcDateTime;
            return new IdempotencyAttempt(existing, existing.ResourceId, Conflict: false);
        }

        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        var record = new IdempotencyRecord
        {
            IdempotencyRecordId = Guid.NewGuid(),
            CompanyId = caller.CompanyId,
            IdempotencyKey = idempotencyKey,
            OperationName = operationName,
            RequestMethod = "POST",
            RequestPath = requestPath,
            RequestSource = "Application",
            RequestFingerprintHash = fingerprint,
            RequestPayloadHash = fingerprint,
            ActorType = "User",
            ActorUserId = caller.UserId,
            Status = "InProgress",
            AttemptCount = 1,
            LockToken = Guid.NewGuid(),
            LockedBy = "Ahdah.Api",
            LockAcquiredAt = nowUtc,
            LeaseExpiresAt = nowUtc.AddMinutes(5),
            ReplayCount = 0,
            CorrelationId = Guid.NewGuid(),
            StartedAt = nowUtc,
            ExpiresAt = nowUtc.AddHours(24)
        };
        dbContext.IdempotencyRecords.Add(record);
        return new IdempotencyAttempt(record, null, Conflict: false);
    }

    private void CompleteIdempotency(
        IdempotencyRecord record,
        string resourceType,
        Guid resourceId,
        int? resourceVersion,
        int responseStatus)
    {
        record.Status = "Completed";
        record.LockToken = null;
        record.LockedBy = null;
        record.LockAcquiredAt = null;
        record.LeaseExpiresAt = null;
        record.ResponseHttpStatus = responseStatus;
        record.ResponseContentType = "application/json";
        record.ResourceType = resourceType;
        record.ResourceId = resourceId;
        record.ResourceVersionNumber = resourceVersion;
        record.CompletedAt = timeProvider.GetUtcNow().UtcDateTime;
    }

    private static void SetResponsePayload<T>(IdempotencyRecord record, T value)
    {
        record.ResponsePayload = JsonSerializer.Serialize(value);
    }

    private static T? DeserializeReplay<T>(string? payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(payload);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    private static string Fingerprint(string operationName, object payload)
    {
        var serialized = JsonSerializer.Serialize(payload);
        var bytes = Encoding.UTF8.GetBytes($"{operationName}|{serialized}");
        return Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    }

    private async Task<Caller?> GetActiveCallerAsync(CancellationToken cancellationToken)
    {
        if (!currentUserContext.IsAuthenticated
            || currentUserContext.CompanyId is not { } companyId
            || currentUserContext.UserId is not { } userId
            || currentUserContext.Role is not { } claimRole)
        {
            return null;
        }

        var caller = await dbContext.AppUsers.AsNoTracking()
            .Where(user =>
                user.CompanyId == companyId
                && user.UserId == userId
                && user.Status == IdentityConstants.ActiveStatus
                && user.Company.Status == IdentityConstants.ActiveStatus)
            .Select(user => new { user.Role })
            .SingleOrDefaultAsync(cancellationToken);
        if (caller is null || caller.Role != claimRole)
        {
            return null;
        }

        return new Caller(companyId, userId, caller.Role, AdvanceRoleCapabilities.For(caller.Role));
    }

    private static IQueryable<Advance> ApplyVisibility(IQueryable<Advance> advances, Caller caller)
    {
        if (caller.Capabilities.CanViewAllCompanyAdvances)
        {
            return advances;
        }

        if (caller.Capabilities.CanConfirmReceipt || caller.Capabilities.CanReturnHeldBalance)
        {
            return advances.Where(advance =>
                advance.DeputyUserId == caller.UserId
                || advance.UserAdvanceBalances.Any(balance =>
                    balance.CompanyId == caller.CompanyId && balance.UserId == caller.UserId)
                || advance.TransferAdvanceAllocations.Any(allocation =>
                    allocation.CompanyId == caller.CompanyId
                    && (allocation.MoneyTransfer.SenderUserId == caller.UserId
                        || allocation.MoneyTransfer.RecipientUserId == caller.UserId)));
        }

        return advances.Where(_ => false);
    }

    private Task<UserAdvanceBalance?> LockBalanceAsync(
        Guid companyId,
        Guid advanceId,
        Guid userId,
        CancellationToken cancellationToken) =>
        dbContext.UserAdvanceBalances
            .FromSqlInterpolated(
                $"SELECT * FROM ahdah.user_advance_balances WHERE company_id = {companyId} AND advance_id = {advanceId} AND user_id = {userId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);

    private Task<UserAdvanceBalance[]> LockBalancesAsync(
        Guid companyId,
        Guid advanceId,
        Guid[] userIds,
        CancellationToken cancellationToken) =>
        dbContext.UserAdvanceBalances
            .FromSqlInterpolated(
                $"SELECT * FROM ahdah.user_advance_balances WHERE company_id = {companyId} AND advance_id = {advanceId} AND user_id = ANY ({userIds.Distinct().OrderBy(value => value).ToArray()}) ORDER BY user_advance_balance_id FOR UPDATE")
            .ToArrayAsync(cancellationToken);

    private Task<Guid[]> DiscoverTransferProjectIdsAsync(
        Guid companyId,
        Guid transferId,
        CancellationToken cancellationToken) =>
        dbContext.CustodyReservations.AsNoTracking()
            .Where(value => value.CompanyId == companyId
                && value.TransferAdvanceAllocation!.MoneyTransferId == transferId
                && value.ProjectCustodyPosition.ProjectId != null)
            .Select(value => value.ProjectCustodyPosition.ProjectId!.Value)
            .Distinct().OrderBy(value => value).ToArrayAsync(cancellationToken);

    private static CustodyMovementEntry TransferMovement(
        CustodyOperation operation,
        ProjectCustodyPosition position,
        int entryNumber,
        decimal availableDelta,
        decimal reservedDelta,
        decimal transferredOut,
        decimal returned,
        BalanceLedgerEntry ledger,
        Guid actor,
        int versionBefore,
        int versionAfter,
        long? revision,
        DateTime occurredAt,
        decimal received = 0m,
        decimal restored = 0m) => new()
        {
            CustodyMovementEntryId = Guid.NewGuid(), CompanyId = operation.CompanyId,
            CustodyOperation = operation, ProjectCustodyPosition = position,
            EntryNumber = entryNumber, DeltaAvailableAmount = availableDelta,
            DeltaReservedAmount = reservedDelta, ReceivedAmount = received,
            RestoredAmount = restored, TransferredOutAmount = transferredOut,
            ReturnedAmount = returned, BalanceLedgerEntry = ledger,
            OccurredAt = occurredAt, ActorUserId = actor,
            PositionVersionBefore = versionBefore, PositionVersionAfter = versionAfter,
            FinancialRevisionAfter = revision
        };

    private async Task<CustodyReservationPlan?> PrepareCustodyReservationAsync(
        Caller caller,
        Guid advanceId,
        decimal amount,
        IReadOnlyList<AdvanceCustodyAllocationRequest> requestedAllocations,
        CancellationToken cancellationToken)
    {
        var balanceId = await dbContext.UserAdvanceBalances.AsNoTracking()
            .Where(value => value.CompanyId == caller.CompanyId
                && value.AdvanceId == advanceId && value.UserId == caller.UserId)
            .Select(value => (Guid?)value.UserAdvanceBalanceId)
            .SingleOrDefaultAsync(cancellationToken);
        if (balanceId is null)
        {
            return null;
        }

        var hasCustody = await dbContext.ProjectCustodyPositions.AsNoTracking().AnyAsync(
            value => value.CompanyId == caller.CompanyId
                && value.UserAdvanceBalanceId == balanceId.Value, cancellationToken);
        if (!hasCustody)
        {
            if (requestedAllocations.Count != 0)
            {
                return null;
            }

            var legacyBalance = await LockBalanceAsync(
                caller.CompanyId, advanceId, caller.UserId, cancellationToken);
            return IsUsableBalance(legacyBalance, amount)
                ? new CustodyReservationPlan(legacyBalance!, [], [])
                : null;
        }

        if (requestedAllocations.Count == 0)
        {
            return null;
        }

        var requestedIds = requestedAllocations.Select(value => value.ProjectCustodyPositionId)
            .Distinct().OrderBy(value => value).ToArray();
        var discovered = await dbContext.ProjectCustodyPositions.AsNoTracking()
            .Where(value => value.CompanyId == caller.CompanyId
                && value.UserAdvanceBalanceId == balanceId.Value
                && requestedIds.Contains(value.ProjectCustodyPositionId))
            .Select(value => new { value.ProjectCustodyPositionId, value.ProjectId })
            .ToArrayAsync(cancellationToken);
        if (discovered.Length != requestedIds.Length)
        {
            return null;
        }

        var controls = await financialGate.LockOpenAsync(
            caller.CompanyId, discovered.Where(value => value.ProjectId != null)
                .Select(value => value.ProjectId!.Value), cancellationToken);
        if (controls is null)
        {
            return null;
        }

        var balance = await LockBalanceAsync(caller.CompanyId, advanceId, caller.UserId, cancellationToken);
        if (!IsUsableBalance(balance, amount))
        {
            return null;
        }

        var positions = await dbContext.ProjectCustodyPositions
            .FromSqlInterpolated($"SELECT * FROM ahdah.project_custody_positions WHERE company_id = {caller.CompanyId} AND user_advance_balance_id = {balanceId.Value} AND project_custody_position_id = ANY ({requestedIds}) ORDER BY project_custody_position_id FOR UPDATE")
            .ToArrayAsync(cancellationToken);
        var amounts = requestedAllocations.ToDictionary(
            value => value.ProjectCustodyPositionId, value => value.Amount);
        if (positions.Length != requestedIds.Length
            || positions.Any(value => value.AvailableAmount < amounts[value.ProjectCustodyPositionId]))
        {
            return null;
        }

        return new CustodyReservationPlan(balance!, positions.Select(value =>
            new CustodySlice(value, amounts[value.ProjectCustodyPositionId])).ToArray(), controls);
    }

    private CustodyOperation? ApplyCustodyReservation(
        Caller caller,
        CustodyReservationPlan plan,
        MoneyTransfer transfer,
        TransferAdvanceAllocation allocation,
        IdempotencyRecord idempotency,
        bool isReturn)
    {
        plan.Balance.AvailableAmount -= transfer.TransferAmount;
        plan.Balance.ReservedAmount += transfer.TransferAmount;
        plan.Balance.VersionNumber++;
        var ledger = AddBalanceLedger(plan.Balance,
            isReturn ? "BalanceReturnReserved" : "TransferReserved",
            0m, 0m, 0m, 0m, 0m, -transfer.TransferAmount, transfer.TransferAmount,
            caller.UserId, transfer.MoneyTransferId,
            isReturn ? "Reserved for unused-balance return confirmation."
                : "Reserved for transfer confirmation.");
        if (plan.Slices.Count == 0)
        {
            return null;
        }

        financialGate.AdvanceRevisions(plan.Controls);
        var revisions = plan.Controls.ToDictionary(value => value.ProjectId, value => value.FinancialRevision);
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var operation = new CustodyOperation
        {
            CustodyOperationId = Guid.NewGuid(), CompanyId = caller.CompanyId,
            OperationType = "ReserveTransfer", Status = "Draft",
            RequestedByUserId = caller.UserId, RequestedAt = now,
            Reason = isReturn ? "Reserved explicit custody slices for balance return."
                : "Reserved explicit custody slices for internal transfer.",
            IdempotencyRecordId = idempotency.IdempotencyRecordId,
            MoneyTransferId = transfer.MoneyTransferId, VersionNumber = 1
        };
        dbContext.CustodyOperations.Add(operation);
        var entryNumber = 1;
        foreach (var slice in plan.Slices)
        {
            var before = slice.Position.VersionNumber;
            slice.Position.AvailableAmount -= slice.Amount;
            slice.Position.ReservedAmount += slice.Amount;
            slice.Position.VersionNumber++;
            slice.Position.UpdatedAt = now;
            dbContext.CustodyMovementEntries.Add(new CustodyMovementEntry
            {
                CustodyMovementEntryId = Guid.NewGuid(), CompanyId = caller.CompanyId,
                CustodyOperation = operation, ProjectCustodyPosition = slice.Position,
                EntryNumber = entryNumber++, DeltaAvailableAmount = -slice.Amount,
                DeltaReservedAmount = slice.Amount, BalanceLedgerEntry = ledger,
                OccurredAt = now, ActorUserId = caller.UserId,
                PositionVersionBefore = before, PositionVersionAfter = slice.Position.VersionNumber,
                FinancialRevisionAfter = slice.Position.ProjectId is { } projectId
                    ? revisions[projectId] : null
            });
            dbContext.CustodyReservations.Add(new CustodyReservation
            {
                CustodyReservationId = Guid.NewGuid(), CompanyId = caller.CompanyId,
                CustodyOperation = operation, ProjectCustodyPosition = slice.Position,
                TransferAdvanceAllocation = allocation, Amount = slice.Amount,
                Status = "Reserved", VersionNumber = 1
            });
        }

        return operation;
    }

    private void PostCustodyOperation(CustodyOperation? operation)
    {
        if (operation is null) return;
        operation.Status = "Posted";
        operation.VersionNumber++;
        operation.UpdatedAt = timeProvider.GetUtcNow().UtcDateTime;
    }

    private static bool IsUsableBalance(UserAdvanceBalance? balance, decimal amount) =>
        balance is not null && balance.Status == AdvanceConstants.ActiveBalanceStatus
        && balance.AvailableAmount >= amount;

    private Task<MoneyTransfer?> LockTransferAsync(
        Guid companyId,
        Guid transferId,
        CancellationToken cancellationToken) =>
        dbContext.MoneyTransfers
            .FromSqlInterpolated(
                $"SELECT * FROM ahdah.money_transfers WHERE company_id = {companyId} AND money_transfer_id = {transferId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);

    private static MoneyTransfer NewTransfer(
        Caller caller,
        string transferType,
        Guid? advanceId,
        Guid senderUserId,
        Guid recipientUserId,
        decimal amount,
        string currencyCode,
        AdvanceTransferRequestBase request) => new()
        {
            CompanyId = caller.CompanyId,
            TransferNumber = CreateReference("TRF", Guid.NewGuid()),
            TransferType = transferType,
            AdvanceId = advanceId,
            SenderUserId = senderUserId,
            RecipientUserId = recipientUserId,
            TransferAmount = amount,
            CurrencyCode = currencyCode,
            TransferDate = request.TransferDate,
            TransferMethod = request.TransferMethod,
            BankName = NormalizeOptional(request.BankName),
            ReferenceNumber = NormalizeOptional(request.ReferenceNumber),
            ProofFileUrl = NormalizeOptional(request.ProofFileUrl),
            Description = NormalizeOptional(request.Description),
            Notes = NormalizeOptional(request.Notes),
            Status = AdvanceConstants.PendingConfirmationStatus,
            InitiatedByUserId = caller.UserId
        };

    private static UserAdvanceBalance NewBalance(
        Guid companyId,
        Guid advanceId,
        Guid userId,
        Guid createdByUserId,
        decimal receivedAmount,
        decimal restoredAmount) => new()
        {
            UserAdvanceBalanceId = Guid.NewGuid(),
            CompanyId = companyId,
            AdvanceId = advanceId,
            UserId = userId,
            TotalReceivedAmount = receivedAmount,
            TotalRestoredAmount = restoredAmount,
            AvailableAmount = receivedAmount + restoredAmount,
            Status = AdvanceConstants.ActiveBalanceStatus,
            CreatedByUserId = createdByUserId,
            VersionNumber = 1
        };

    private BalanceLedgerEntry AddBalanceLedger(
        UserAdvanceBalance balance,
        string entryType,
        decimal receivedDelta,
        decimal restoredDelta,
        decimal expensedDelta,
        decimal transferredDelta,
        decimal returnedDelta,
        decimal availableDelta,
        decimal reservedDelta,
        Guid performedByUserId,
        Guid referenceId,
        string description,
        string referenceType = "MoneyTransfer")
    {
        var entry = new BalanceLedgerEntry
        {
            BalanceLedgerEntryId = Guid.NewGuid(),
            CompanyId = balance.CompanyId,
            UserAdvanceBalance = balance,
            BalanceVersionNumber = balance.VersionNumber,
            EntryType = entryType,
            DeltaReceivedAmount = receivedDelta,
            DeltaRestoredAmount = restoredDelta,
            DeltaExpensedAmount = expensedDelta,
            DeltaTransferredOutAmount = transferredDelta,
            DeltaReturnedAmount = returnedDelta,
            DeltaAdjustmentOutAmount = 0m,
            DeltaAvailableAmount = availableDelta,
            DeltaReservedAmount = reservedDelta,
            ReceivedAfterAmount = balance.TotalReceivedAmount,
            RestoredAfterAmount = balance.TotalRestoredAmount,
            ExpensedAfterAmount = balance.TotalExpensedAmount,
            TransferredOutAfterAmount = balance.TotalTransferredOutAmount,
            ReturnedAfterAmount = balance.TotalReturnedAmount,
            AdjustmentOutAfterAmount = balance.TotalAdjustmentOutAmount,
            AvailableAfterAmount = balance.AvailableAmount,
            ReservedAfterAmount = balance.ReservedAmount,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            CorrelationId = Guid.NewGuid(),
            Description = description,
            PerformedByUserId = performedByUserId
        };
        dbContext.BalanceLedgerEntries.Add(entry);
        return entry;
    }

    private void AddFundingLedger(
        FundingSource source,
        string entryType,
        decimal amount,
        decimal availableDelta,
        decimal reservedDelta,
        decimal usedDelta,
        Guid performedByUserId,
        string referenceType,
        Guid referenceId,
        Guid correlationId,
        string description)
    {
        dbContext.FundingSourceLedgerEntries.Add(new FundingSourceLedgerEntry
        {
            CompanyId = source.CompanyId,
            FundingSourceId = source.FundingSourceId,
            SourceVersionNumber = source.VersionNumber,
            EntryType = entryType,
            Amount = amount,
            AvailableDelta = availableDelta,
            ReservedDelta = reservedDelta,
            UsedDelta = usedDelta,
            ReversedDelta = 0m,
            AvailableAfter = source.AvailableAmount,
            ReservedAfter = source.ReservedAmount,
            UsedAfter = source.UsedAmount,
            ReversedAfter = source.ReversedAmount,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            PerformedByUserId = performedByUserId,
            CorrelationId = correlationId,
            Description = description
        });
    }

    private static string FundingStatus(FundingSource source)
    {
        if (source.AvailableAmount == 0m && source.ReservedAmount == 0m
            && source.UsedAmount == source.NetAmount)
        {
            return AdvanceConstants.FullyUsedFundingStatus;
        }

        return source.UsedAmount == 0m
            ? AdvanceConstants.ActiveFundingStatus
            : AdvanceConstants.PartiallyUsedFundingStatus;
    }

    private static IQueryable<AdvanceRow> SelectAdvanceRows(IQueryable<Advance> advances) =>
        advances.Select(advance => new AdvanceRow(
            advance.AdvanceId,
            advance.AdvanceNumber,
            advance.AdvanceAmount,
            advance.UserAdvanceBalances.Sum(balance => (decimal?)balance.AvailableAmount) ?? 0m,
            advance.UserAdvanceBalances.Sum(balance => (decimal?)balance.ReservedAmount) ?? 0m,
            advance.CurrencyCode,
            advance.IssueDate,
            advance.SettlementDueDate,
            advance.Purpose,
            advance.Notes,
            advance.Status,
            advance.DeputyUserId,
            advance.AppUser2.FullName,
            advance.AppUser2.Role,
            advance.VersionNumber,
            advance.CreatedAt,
            advance.ConfirmedAt));

    private static IQueryable<BalanceRow> SelectBalanceRows(
        IQueryable<UserAdvanceBalance> balances) => balances.Select(balance => new BalanceRow(
            balance.UserAdvanceBalanceId,
            balance.AdvanceId,
            balance.Advance.AdvanceNumber,
            balance.UserId,
            balance.AppUserNavigation.FullName,
            balance.AppUserNavigation.Role,
            balance.TotalReceivedAmount,
            balance.TotalRestoredAmount,
            balance.TotalExpensedAmount,
            balance.TotalTransferredOutAmount,
            balance.TotalReturnedAmount,
            balance.AvailableAmount,
            balance.ReservedAmount,
            balance.Advance.CurrencyCode,
            balance.Status,
            balance.VersionNumber,
            balance.UpdatedAt));

    private static AdvanceSummary MapAdvance(AdvanceRow row) => new(
        row.AdvanceId,
        row.AdvanceNumber,
        row.AdvanceAmount,
        row.AvailableAmount,
        row.ReservedAmount,
        row.CurrencyCode,
        row.IssueDate,
        row.SettlementDueDate,
        row.Purpose,
        row.Notes,
        row.Status,
        new AdvanceUserSummary(row.RecipientUserId, row.RecipientName, row.RecipientRole),
        row.VersionNumber,
        ToUtc(row.CreatedAt),
        ToNullableUtc(row.ConfirmedAt));

    private static AdvanceBalanceSummary MapBalance(BalanceRow row) => new(
        row.UserAdvanceBalanceId,
        row.AdvanceId,
        row.AdvanceNumber,
        new AdvanceUserSummary(row.UserId, row.HolderName, row.HolderRole),
        row.TotalReceivedAmount,
        row.TotalRestoredAmount,
        row.TotalExpensedAmount,
        row.TotalTransferredOutAmount,
        row.TotalReturnedAmount,
        row.AvailableAmount,
        row.ReservedAmount,
        row.CurrencyCode,
        row.Status,
        row.VersionNumber,
        ToUtc(row.UpdatedAt));

    private static AdvanceMovementSummary MapMovement(MovementRow row) => new(
        row.MovementId,
        row.OperationType,
        row.Amount,
        new AdvanceUserSummary(row.ActorUserId, row.ActorName, row.ActorRole),
        row.SenderUserId is { } senderId
            ? new AdvanceUserSummary(senderId, row.SenderName!, row.SenderRole!)
            : null,
        row.RecipientUserId is { } recipientId
            ? new AdvanceUserSummary(recipientId, row.RecipientName!, row.RecipientRole!)
            : null,
        row.Status,
        ToUtc(row.OccurredAt));

    private static AdvanceTransferDetails MapTransfer(TransferRow row) => new(
        row.TransferId,
        row.TransferNumber,
        row.TransferType,
        row.AdvanceId,
        row.Amount,
        row.CurrencyCode,
        new AdvanceUserSummary(row.SenderUserId, row.SenderName, row.SenderRole),
        new AdvanceUserSummary(row.RecipientUserId, row.RecipientName, row.RecipientRole),
        row.TransferMethod,
        row.Status,
        row.VersionNumber,
        ToUtc(row.CreatedAt),
        ToNullableUtc(row.ConfirmedAt),
        ToNullableUtc(row.RejectedAt));

    private static bool IsValid(AdvanceQuery query) =>
        IsValidPage(query.Page, query.PageSize)
        && (query.Status is null || AdvanceRules.IsKnownAdvanceStatus(query.Status))
        && (query.UserId is null || query.UserId != Guid.Empty)
        && (query.Reference is null
            || NormalizeOptional(query.Reference) is { Length: >= 2 and <= AdvanceConstants.MaximumSearchLength });

    private static bool IsValidPage(int page, int pageSize) =>
        page >= 1 && pageSize is >= 1 and <= AccessConstants.MaximumPageSize;

    private static bool Validate(object value)
    {
        var results = new List<ValidationResult>();
        return Validator.TryValidateObject(value, new ValidationContext(value), results, true);
    }

    private static string CreateReference(string prefix, Guid id) => $"{prefix}-{id:N}";

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static int CalculateTotalPages(int totalCount, int pageSize) =>
        totalCount == 0 ? 0 : (totalCount + pageSize - 1) / pageSize;

    private static DateTimeOffset ToUtc(DateTime value) =>
        new(DateTime.SpecifyKind(value, DateTimeKind.Utc));

    private static DateTimeOffset? ToNullableUtc(DateTime? value) =>
        value is null ? null : ToUtc(value.Value);

    private static bool IsUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };

    private static bool IsPostgresConflict(PostgresException exception) =>
        exception.SqlState is PostgresErrorCodes.CheckViolation
            or PostgresErrorCodes.SerializationFailure
            or PostgresErrorCodes.DeadlockDetected
            or PostgresErrorCodes.RaiseException;

    private sealed record Caller(
        Guid CompanyId,
        Guid UserId,
        string Role,
        AdvanceRoleCapabilities Capabilities);

    private sealed record IdempotencyAttempt(
        IdempotencyRecord? Record,
        Guid? ReplayResourceId,
        bool Conflict);

    private sealed record CustodySlice(ProjectCustodyPosition Position, decimal Amount);

    private sealed record CustodyReservationPlan(
        UserAdvanceBalance Balance,
        IReadOnlyList<CustodySlice> Slices,
        IReadOnlyList<ProjectFinancialControl> Controls);

    private sealed record AdvanceRow(
        Guid AdvanceId,
        string AdvanceNumber,
        decimal AdvanceAmount,
        decimal AvailableAmount,
        decimal ReservedAmount,
        string CurrencyCode,
        DateOnly IssueDate,
        DateOnly? SettlementDueDate,
        string Purpose,
        string? Notes,
        string Status,
        Guid RecipientUserId,
        string RecipientName,
        string RecipientRole,
        int VersionNumber,
        DateTime CreatedAt,
        DateTime? ConfirmedAt);

    private sealed record BalanceRow(
        Guid UserAdvanceBalanceId,
        Guid AdvanceId,
        string AdvanceNumber,
        Guid UserId,
        string HolderName,
        string HolderRole,
        decimal TotalReceivedAmount,
        decimal TotalRestoredAmount,
        decimal TotalExpensedAmount,
        decimal TotalTransferredOutAmount,
        decimal TotalReturnedAmount,
        decimal AvailableAmount,
        decimal ReservedAmount,
        string CurrencyCode,
        string Status,
        int VersionNumber,
        DateTime UpdatedAt);

    private sealed record MovementRow(
        Guid MovementId,
        string OperationType,
        decimal Amount,
        Guid ActorUserId,
        string ActorName,
        string ActorRole,
        Guid? SenderUserId,
        string? SenderName,
        string? SenderRole,
        Guid? RecipientUserId,
        string? RecipientName,
        string? RecipientRole,
        string Status,
        DateTime OccurredAt);

    private sealed record FundingRow(
        string SourceType,
        decimal AllocatedAmount,
        string Status,
        DateTime CreatedAt,
        Guid FundingSourceId);

    private sealed record FundingSourceRow(
        Guid FundingSourceId,
        string SourceType,
        DateOnly SourceDate,
        string CurrencyCode,
        decimal AvailableAmount,
        string Status);

    private sealed record TransferRow(
        Guid TransferId,
        string TransferNumber,
        string TransferType,
        Guid AdvanceId,
        decimal Amount,
        string CurrencyCode,
        Guid SenderUserId,
        string SenderName,
        string SenderRole,
        Guid RecipientUserId,
        string RecipientName,
        string RecipientRole,
        string TransferMethod,
        string Status,
        int VersionNumber,
        DateTime CreatedAt,
        DateTime? ConfirmedAt,
        DateTime? RejectedAt);
}
