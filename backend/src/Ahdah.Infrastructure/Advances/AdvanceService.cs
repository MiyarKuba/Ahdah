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
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Ahdah.Infrastructure.Advances;

public sealed class AdvanceService(
    AhdahDbContext dbContext,
    ICurrentUserContext currentUserContext,
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

            var balance = await LockBalanceAsync(
                caller.CompanyId,
                advanceId,
                caller.UserId,
                cancellationToken);
            if (balance is null
                || balance.Status != AdvanceConstants.ActiveBalanceStatus
                || balance.AvailableAmount < request.Amount)
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

            balance.AvailableAmount -= request.Amount;
            balance.ReservedAmount += request.Amount;
            balance.VersionNumber++;
            AddBalanceLedger(
                balance,
                "TransferReserved",
                0m,
                0m,
                0m,
                0m,
                0m,
                -request.Amount,
                request.Amount,
                caller.UserId,
                transferId,
                "Reserved for transfer confirmation.");

            dbContext.TransferAdvanceAllocations.Add(new TransferAdvanceAllocation
            {
                CompanyId = caller.CompanyId,
                MoneyTransferId = transferId,
                AdvanceId = advanceId,
                AllocatedAmount = request.Amount,
                AllocatedByUserId = caller.UserId,
                Notes = NormalizeOptional(request.Notes)
            });

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

            var balance = await LockBalanceAsync(
                caller.CompanyId,
                advanceId,
                caller.UserId,
                cancellationToken);
            if (balance is null
                || balance.Status != AdvanceConstants.ActiveBalanceStatus
                || balance.AvailableAmount < request.Amount)
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

            balance.AvailableAmount -= request.Amount;
            balance.ReservedAmount += request.Amount;
            balance.VersionNumber++;
            AddBalanceLedger(
                balance,
                "BalanceReturnReserved",
                0m,
                0m,
                0m,
                0m,
                0m,
                -request.Amount,
                request.Amount,
                caller.UserId,
                transferId,
                "Reserved for unused-balance return confirmation.");

            dbContext.TransferAdvanceAllocations.Add(new TransferAdvanceAllocation
            {
                CompanyId = caller.CompanyId,
                MoneyTransferId = transferId,
                AdvanceId = advanceId,
                AllocatedAmount = request.Amount,
                AllocatedByUserId = caller.UserId,
                Notes = NormalizeOptional(request.Notes)
            });

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
                    cancellationToken),
                AdvanceConstants.InternalTransferType => await ConfirmAllocatedTransferAsync(
                    caller,
                    transfer,
                    isReturn: false,
                    cancellationToken),
                AdvanceConstants.BalanceReturnTransferType => await ConfirmAllocatedTransferAsync(
                    caller,
                    transfer,
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

            var senderBalance = await LockBalanceAsync(
                caller.CompanyId,
                allocation.AdvanceId,
                transfer.SenderUserId,
                cancellationToken);
            if (senderBalance is null || senderBalance.ReservedAmount < transfer.TransferAmount)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<AdvanceTransferDetails>.Failure(AccessResultStatus.Conflict);
            }

            senderBalance.AvailableAmount += transfer.TransferAmount;
            senderBalance.ReservedAmount -= transfer.TransferAmount;
            senderBalance.VersionNumber++;
            AddBalanceLedger(
                senderBalance,
                "TransferReservationReleased",
                0m,
                0m,
                0m,
                0m,
                0m,
                transfer.TransferAmount,
                -transfer.TransferAmount,
                caller.UserId,
                transferId,
                "Reservation released after recipient rejection.");

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
        AddBalanceLedger(
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

        var senderBalance = await LockBalanceAsync(
            caller.CompanyId,
            allocation.AdvanceId,
            transfer.SenderUserId,
            cancellationToken);
        if (senderBalance is null
            || senderBalance.Status != AdvanceConstants.ActiveBalanceStatus
            || senderBalance.ReservedAmount < transfer.TransferAmount)
        {
            return false;
        }

        var recipientBalance = await LockBalanceAsync(
            caller.CompanyId,
            allocation.AdvanceId,
            caller.UserId,
            cancellationToken);
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
        else
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
        AddBalanceLedger(
            senderBalance,
            isReturn ? "BalanceReturnConfirmed" : "TransferOutConfirmed",
            0m,
            0m,
            0m,
            isReturn ? 0m : transfer.TransferAmount,
            isReturn ? transfer.TransferAmount : 0m,
            0m,
            -transfer.TransferAmount,
            caller.UserId,
            transfer.MoneyTransferId,
            isReturn ? "Unused balance returned." : "Transfer confirmed by recipient.");
        AddBalanceLedger(
            recipientBalance,
            isReturn ? "AmountRestored" : "TransferInConfirmed",
            isReturn ? 0m : transfer.TransferAmount,
            isReturn ? transfer.TransferAmount : 0m,
            0m,
            0m,
            0m,
            transfer.TransferAmount,
            0m,
            caller.UserId,
            transfer.MoneyTransferId,
            isReturn ? "Returned balance restored to upstream holder." : "Incoming transfer confirmed.");
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

    private void AddBalanceLedger(
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
        dbContext.BalanceLedgerEntries.Add(new BalanceLedgerEntry
        {
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
        });
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

    private sealed record Caller(
        Guid CompanyId,
        Guid UserId,
        string Role,
        AdvanceRoleCapabilities Capabilities);

    private sealed record IdempotencyAttempt(
        IdempotencyRecord? Record,
        Guid? ReplayResourceId,
        bool Conflict);

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
