using System.Data;
using Ahdah.Application.Abstractions.Authentication;
using Ahdah.Application.Abstractions.Context;
using Ahdah.Application.Abstractions.Security;
using Ahdah.Application.Access;
using Ahdah.Application.Access.Contracts;
using Ahdah.Application.Access.Models;
using Ahdah.Application.Access.Services;
using Ahdah.Application.Identity;
using Ahdah.Application.Identity.Models;
using Ahdah.Infrastructure.Persistence.Generated.Context;
using Ahdah.Infrastructure.Persistence.Generated.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Ahdah.Infrastructure.Access.Invitations;

public sealed class InvitationService(
    AhdahDbContext dbContext,
    IInvitationTokenService invitationTokenService,
    IPasswordHashingService passwordHashingService,
    IAccessTokenService accessTokenService,
    ICurrentUserContext currentUserContext,
    TimeProvider timeProvider,
    IOptions<InvitationOptions> invitationOptions) : IInvitationService
{
    public async Task<AccessResult<CreateInvitationResult>> CreateAsync(
        CreateInvitationRequest request,
        CancellationToken cancellationToken)
    {
        var tenant = await GetActiveManagerTenantAsync(cancellationToken);
        if (tenant is null)
        {
            return AccessResult<CreateInvitationResult>.Failure(AccessResultStatus.Unauthorized);
        }

        var phoneNumber = request.PhoneNumber.Trim();
        if (!AccessRoleRules.IsAssignable(request.AssignedRole))
        {
            return AccessResult<CreateInvitationResult>.Failure(AccessResultStatus.Invalid);
        }

        var assignedRole = AccessRoleRules.Normalize(request.AssignedRole);
        var hasConflict = await dbContext.AppUsers
                .AsNoTracking()
                .AnyAsync(user => user.PhoneNumber == phoneNumber, cancellationToken)
            || await dbContext.Invitations
                .AsNoTracking()
                .AnyAsync(
                    invitation => invitation.CompanyId == tenant.Value.CompanyId
                        && invitation.InvitedPhoneNumber == phoneNumber
                        && invitation.Status == AccessConstants.PendingInvitationStatus,
                    cancellationToken);

        if (hasConflict)
        {
            return AccessResult<CreateInvitationResult>.Failure(AccessResultStatus.Conflict);
        }

        var token = invitationTokenService.Generate();
        var invitation = new Invitation
        {
            CompanyId = tenant.Value.CompanyId,
            InvitedByUserId = tenant.Value.UserId,
            InvitedPhoneNumber = phoneNumber,
            AssignedRole = assignedRole,
            InvitationCodeHash = token.TokenHash,
            Status = AccessConstants.PendingInvitationStatus,
            ExpiresAt = InvitationExpiryCalculator.Calculate(timeProvider, invitationOptions.Value)
        };

        try
        {
            dbContext.Invitations.Add(invitation);
            await dbContext.SaveChangesAsync(cancellationToken);

            return AccessResult<CreateInvitationResult>.Success(
                new CreateInvitationResult(Map(invitation), token.RawToken));
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return AccessResult<CreateInvitationResult>.Failure(AccessResultStatus.Conflict);
        }
    }

    public async Task<AccessResult<PagedResult<InvitationSummary>>> ListAsync(
        InvitationQuery query,
        CancellationToken cancellationToken)
    {
        var tenant = await GetActiveManagerTenantAsync(cancellationToken);
        if (tenant is null)
        {
            return AccessResult<PagedResult<InvitationSummary>>.Failure(AccessResultStatus.Unauthorized);
        }

        if (query.Page < 1
            || query.PageSize is < 1 or > AccessConstants.MaximumPageSize
            || !IsKnownStatus(query.Status))
        {
            return AccessResult<PagedResult<InvitationSummary>>.Failure(AccessResultStatus.Invalid);
        }

        var invitations = dbContext.Invitations
            .AsNoTracking()
            .Where(invitation => invitation.CompanyId == tenant.Value.CompanyId);

        if (query.Status is not null)
        {
            invitations = invitations.Where(invitation => invitation.Status == query.Status);
        }

        var totalCount = await invitations.CountAsync(cancellationToken);
        var rows = await invitations
            .OrderByDescending(invitation => invitation.CreatedAt)
            .ThenByDescending(invitation => invitation.InvitationId)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return AccessResult<PagedResult<InvitationSummary>>.Success(new PagedResult<InvitationSummary>(
            rows.Select(Map).ToArray(),
            query.Page,
            query.PageSize,
            totalCount,
            CalculateTotalPages(totalCount, query.PageSize)));
    }

    public async Task<AccessResult<InvitationAcceptanceResult>> AcceptAsync(
        AcceptInvitationRequest request,
        CancellationToken cancellationToken)
    {
        var token = request.Token.Trim();
        if (string.IsNullOrWhiteSpace(token)
            || string.IsNullOrWhiteSpace(request.FullName)
            || string.IsNullOrWhiteSpace(request.Password))
        {
            return AccessResult<InvitationAcceptanceResult>.Failure(AccessResultStatus.Invalid);
        }

        var tokenHash = invitationTokenService.Hash(token);
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        try
        {
            var invitation = await dbContext.Invitations
                .FromSqlInterpolated(
                    $"SELECT * FROM ahdah.invitations WHERE invitation_code_hash = {tokenHash} FOR UPDATE")
                .SingleOrDefaultAsync(cancellationToken);

            if (invitation is null
                || invitation.Status != AccessConstants.PendingInvitationStatus
                || invitation.AcceptedByUserId is not null
                || invitation.AcceptedAt is not null
                || invitation.CancelledAt is not null
                || invitation.ExpiresAt <= nowUtc
                || !AccessRoleRules.IsAssignable(invitation.AssignedRole))
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<InvitationAcceptanceResult>.Failure(AccessResultStatus.Invalid);
            }

            var company = await dbContext.Companies
                .FromSqlInterpolated(
                    $"SELECT * FROM ahdah.companies WHERE company_id = {invitation.CompanyId} FOR SHARE")
                .SingleOrDefaultAsync(cancellationToken);

            if (company is null || company.Status != IdentityConstants.ActiveStatus)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<InvitationAcceptanceResult>.Failure(AccessResultStatus.Invalid);
            }

            var email = NormalizeOptional(request.Email);
            var recipientConflict = await dbContext.AppUsers
                    .AsNoTracking()
                    .AnyAsync(user => user.PhoneNumber == invitation.InvitedPhoneNumber, cancellationToken)
                || email is not null && await dbContext.AppUsers
                    .AsNoTracking()
                    .AnyAsync(
                        user => user.CompanyId == invitation.CompanyId
                            && user.Email != null
                            && user.Email.ToLower() == email.ToLower(),
                        cancellationToken);

            if (recipientConflict)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<InvitationAcceptanceResult>.Failure(AccessResultStatus.Conflict);
            }

            var assignedRole = AccessRoleRules.Normalize(invitation.AssignedRole);
            var activation = AccessRoleRules.ForInvitationAcceptance(assignedRole);
            var user = new AppUser
            {
                CompanyId = invitation.CompanyId,
                FullName = request.FullName.Trim(),
                PhoneNumber = invitation.InvitedPhoneNumber,
                Email = email,
                PasswordHash = passwordHashingService.HashPassword(request.Password),
                Role = assignedRole,
                Status = activation.UserStatus,
                IdentityVerificationStatus = activation.IdentityStatus,
                FailedLoginAttempts = 0,
                MustChangePassword = false
            };

            dbContext.AppUsers.Add(user);
            await dbContext.SaveChangesAsync(cancellationToken);

            invitation.Status = AccessConstants.AcceptedInvitationStatus;
            invitation.AcceptedByUserId = user.UserId;
            invitation.AcceptedAt = nowUtc;
            invitation.UpdatedAt = nowUtc;
            await dbContext.SaveChangesAsync(cancellationToken);

            AuthenticationResult? authentication = null;
            if (user.Status == AccessConstants.ActiveUserStatus)
            {
                authentication = CreateAuthentication(user, company);
            }

            await transaction.CommitAsync(cancellationToken);

            return AccessResult<InvitationAcceptanceResult>.Success(
                new InvitationAcceptanceResult(
                    activation.Outcome,
                    user.Status,
                    user.Role,
                    authentication));
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<InvitationAcceptanceResult>.Failure(AccessResultStatus.Conflict);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<InvitationAcceptanceResult>.Failure(AccessResultStatus.Conflict);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<AccessResult<InvitationSummary>> CancelAsync(
        Guid invitationId,
        CancellationToken cancellationToken)
    {
        var tenant = await GetActiveManagerTenantAsync(cancellationToken);
        if (tenant is null)
        {
            return AccessResult<InvitationSummary>.Failure(AccessResultStatus.Unauthorized);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        try
        {
            var invitation = await dbContext.Invitations
                .FromSqlInterpolated(
                    $"SELECT * FROM ahdah.invitations WHERE company_id = {tenant.Value.CompanyId} AND invitation_id = {invitationId} FOR UPDATE")
                .SingleOrDefaultAsync(cancellationToken);

            if (invitation is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<InvitationSummary>.Failure(AccessResultStatus.NotFound);
            }

            if (invitation.Status == AccessConstants.CancelledInvitationStatus)
            {
                await transaction.CommitAsync(cancellationToken);
                return AccessResult<InvitationSummary>.Success(Map(invitation));
            }

            if (invitation.Status != AccessConstants.PendingInvitationStatus
                || invitation.AcceptedByUserId is not null
                || invitation.AcceptedAt is not null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<InvitationSummary>.Failure(AccessResultStatus.Invalid);
            }

            var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
            invitation.Status = AccessConstants.CancelledInvitationStatus;
            invitation.CancelledAt = nowUtc;
            invitation.UpdatedAt = nowUtc;
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return AccessResult<InvitationSummary>.Success(Map(invitation));
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<InvitationSummary>.Failure(AccessResultStatus.Conflict);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<(Guid CompanyId, Guid UserId)?> GetActiveManagerTenantAsync(
        CancellationToken cancellationToken)
    {
        if (!currentUserContext.IsAuthenticated
            || currentUserContext.CompanyId is not { } companyId
            || currentUserContext.UserId is not { } userId
            || currentUserContext.Role != IdentityConstants.ManagerRole)
        {
            return null;
        }

        var validManager = await dbContext.AppUsers
            .AsNoTracking()
            .AnyAsync(
                user => user.CompanyId == companyId
                    && user.UserId == userId
                    && user.Role == IdentityConstants.ManagerRole
                    && user.Status == IdentityConstants.ActiveStatus
                    && user.Company.Status == IdentityConstants.ActiveStatus,
                cancellationToken);

        return validManager ? (companyId, userId) : null;
    }

    private static bool IsKnownStatus(string? status) =>
        status is null
            or AccessConstants.PendingInvitationStatus
            or AccessConstants.AcceptedInvitationStatus
            or AccessConstants.ExpiredInvitationStatus
            or AccessConstants.CancelledInvitationStatus;

    private static InvitationSummary Map(Invitation invitation) => new(
        invitation.InvitationId,
        invitation.InvitedPhoneNumber,
        invitation.AssignedRole,
        invitation.Status,
        ToUtc(invitation.ExpiresAt),
        ToNullableUtc(invitation.AcceptedAt),
        ToNullableUtc(invitation.CancelledAt),
        ToUtc(invitation.CreatedAt),
        ToUtc(invitation.UpdatedAt));

    private AuthenticationResult CreateAuthentication(AppUser user, Company company)
    {
        var userSummary = new UserSummary(
            user.UserId,
            user.FullName,
            user.Role,
            user.Status,
            user.IdentityVerificationStatus);
        var companySummary = new CompanySummary(
            company.CompanyId,
            company.CompanyName,
            company.CompanyCode,
            company.Status);
        var token = accessTokenService.CreateToken(
            new AccessTokenSubject(user.UserId, company.CompanyId, user.Role, user.FullName));

        return new AuthenticationResult(
            token.Token,
            "Bearer",
            token.ExpiresAtUtc,
            userSummary,
            companySummary,
            user.Role);
    }

    private static DateTimeOffset ToUtc(DateTime value) =>
        new(DateTime.SpecifyKind(value, DateTimeKind.Utc));

    private static DateTimeOffset? ToNullableUtc(DateTime? value) =>
        value is null ? null : ToUtc(value.Value);

    private static int CalculateTotalPages(int totalCount, int pageSize) =>
        totalCount == 0 ? 0 : (totalCount + pageSize - 1) / pageSize;

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static bool IsUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
}
