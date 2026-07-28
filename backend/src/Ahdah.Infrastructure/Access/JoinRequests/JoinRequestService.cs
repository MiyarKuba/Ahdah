using System.Data;
using Ahdah.Application.Abstractions.Authentication;
using Ahdah.Application.Abstractions.Context;
using Ahdah.Application.Access;
using Ahdah.Application.Access.Contracts;
using Ahdah.Application.Access.Models;
using Ahdah.Application.Access.Services;
using Ahdah.Application.Identity;
using Ahdah.Infrastructure.Persistence.Generated.Context;
using Ahdah.Infrastructure.Persistence.Generated.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Ahdah.Infrastructure.Access.JoinRequests;

public sealed class JoinRequestService(
    AhdahDbContext dbContext,
    IPasswordHashingService passwordHashingService,
    ICurrentUserContext currentUserContext,
    TimeProvider timeProvider) : IJoinRequestService
{
    public async Task<AccessResult<JoinRequestSubmissionResult>> SubmitAsync(
        SubmitJoinRequestRequest request,
        CancellationToken cancellationToken)
    {
        if (!AccessRoleRules.IsAssignable(request.RequestedRole))
        {
            return AccessResult<JoinRequestSubmissionResult>.Failure(AccessResultStatus.Invalid);
        }

        var companyCode = request.CompanyCode.Trim().ToUpperInvariant();
        var phoneNumber = request.PhoneNumber.Trim();
        var email = NormalizeOptional(request.Email);
        var requestedRole = AccessRoleRules.Normalize(request.RequestedRole);
        var passwordHash = passwordHashingService.HashPassword(request.Password);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        try
        {
            var company = await dbContext.Companies
                .FromSqlInterpolated(
                    $"SELECT * FROM ahdah.companies WHERE company_code = {companyCode} FOR SHARE")
                .SingleOrDefaultAsync(cancellationToken);

            if (company is null || company.Status != IdentityConstants.ActiveStatus)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<JoinRequestSubmissionResult>.Failure(AccessResultStatus.Invalid);
            }

            var hasConflict = await dbContext.AppUsers
                    .AsNoTracking()
                    .AnyAsync(user => user.PhoneNumber == phoneNumber, cancellationToken)
                || email is not null && await dbContext.AppUsers
                    .AsNoTracking()
                    .AnyAsync(
                        user => user.CompanyId == company.CompanyId
                            && user.Email != null
                            && user.Email.ToLower() == email.ToLower(),
                        cancellationToken);

            if (hasConflict)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<JoinRequestSubmissionResult>.Failure(AccessResultStatus.Conflict);
            }

            var identityStatus = AccessRoleRules.InitialIdentityStatus(requestedRole);
            var applicant = new AppUser
            {
                CompanyId = company.CompanyId,
                FullName = request.FullName.Trim(),
                PhoneNumber = phoneNumber,
                Email = email,
                PasswordHash = passwordHash,
                Role = requestedRole,
                Status = AccessConstants.PendingApprovalUserStatus,
                IdentityVerificationStatus = identityStatus,
                FailedLoginAttempts = 0,
                MustChangePassword = false
            };

            dbContext.AppUsers.Add(applicant);
            await dbContext.SaveChangesAsync(cancellationToken);

            var joinRequest = new JoinRequest
            {
                CompanyId = company.CompanyId,
                UserId = applicant.UserId,
                RequestedRole = requestedRole,
                Status = AccessConstants.PendingJoinRequestStatus,
                RequestMessage = NormalizeOptional(request.RequestMessage)
            };

            dbContext.JoinRequests.Add(joinRequest);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return AccessResult<JoinRequestSubmissionResult>.Success(
                new JoinRequestSubmissionResult(AccessConstants.PendingJoinRequestStatus));
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<JoinRequestSubmissionResult>.Failure(AccessResultStatus.Conflict);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<AccessResult<PagedResult<JoinRequestSummary>>> ListAsync(
        JoinRequestQuery query,
        CancellationToken cancellationToken)
    {
        var tenant = await GetActiveManagerTenantAsync(cancellationToken);
        if (tenant is null)
        {
            return AccessResult<PagedResult<JoinRequestSummary>>.Failure(AccessResultStatus.Unauthorized);
        }

        if (query.Page < 1
            || query.PageSize is < 1 or > AccessConstants.MaximumPageSize
            || !IsKnownStatus(query.Status))
        {
            return AccessResult<PagedResult<JoinRequestSummary>>.Failure(AccessResultStatus.Invalid);
        }

        var requests = dbContext.JoinRequests
            .AsNoTracking()
            .Where(request => request.CompanyId == tenant.Value.CompanyId);

        if (query.Status is not null)
        {
            requests = requests.Where(request => request.Status == query.Status);
        }

        var totalCount = await requests.CountAsync(cancellationToken);
        var rows = await requests
            .Include(request => request.AppUserNavigation)
            .OrderByDescending(request => request.RequestedAt)
            .ThenByDescending(request => request.JoinRequestId)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return AccessResult<PagedResult<JoinRequestSummary>>.Success(new PagedResult<JoinRequestSummary>(
            rows.Select(request => Map(request, request.AppUserNavigation)).ToArray(),
            query.Page,
            query.PageSize,
            totalCount,
            CalculateTotalPages(totalCount, query.PageSize)));
    }

    public async Task<AccessResult<JoinRequestDecisionResult>> ApproveAsync(
        Guid joinRequestId,
        ApproveJoinRequestRequest request,
        CancellationToken cancellationToken)
    {
        var tenant = await GetActiveManagerTenantAsync(cancellationToken);
        if (tenant is null)
        {
            return AccessResult<JoinRequestDecisionResult>.Failure(AccessResultStatus.Unauthorized);
        }

        if (!AccessRoleRules.IsAssignable(request.AssignedRole))
        {
            return AccessResult<JoinRequestDecisionResult>.Failure(AccessResultStatus.Invalid);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        try
        {
            var joinRequest = await LockTenantRequestAsync(
                tenant.Value.CompanyId,
                joinRequestId,
                cancellationToken);

            if (joinRequest is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<JoinRequestDecisionResult>.Failure(AccessResultStatus.NotFound);
            }

            if (joinRequest.Status != AccessConstants.PendingJoinRequestStatus)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<JoinRequestDecisionResult>.Failure(AccessResultStatus.Invalid);
            }

            var applicant = await LockTenantApplicantAsync(
                tenant.Value.CompanyId,
                joinRequest.UserId,
                cancellationToken);
            if (applicant is null
                || applicant.Status != AccessConstants.PendingApprovalUserStatus)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<JoinRequestDecisionResult>.Failure(AccessResultStatus.Conflict);
            }

            var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
            var assignedRole = AccessRoleRules.Normalize(request.AssignedRole);
            var activation = AccessRoleRules.ForJoinApproval(
                assignedRole,
                applicant.IdentityVerificationStatus);
            joinRequest.AssignedRole = assignedRole;
            joinRequest.Status = AccessConstants.ApprovedJoinRequestStatus;
            joinRequest.ReviewedByUserId = tenant.Value.UserId;
            joinRequest.ReviewedAt = nowUtc;
            joinRequest.ReviewNotes = NormalizeOptional(request.ReviewNotes);
            joinRequest.RejectionReason = null;
            joinRequest.CancelledAt = null;
            joinRequest.UpdatedAt = nowUtc;

            applicant.Role = assignedRole;
            applicant.Status = activation.UserStatus;
            applicant.IdentityVerificationStatus = activation.IdentityStatus;
            applicant.ApprovedByUserId = tenant.Value.UserId;
            applicant.ApprovedAt = nowUtc;
            applicant.UpdatedAt = nowUtc;
            applicant.VersionNumber++;

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return AccessResult<JoinRequestDecisionResult>.Success(
                new JoinRequestDecisionResult(
                    activation.Outcome,
                    applicant.Status,
                    Map(joinRequest, applicant)));
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<JoinRequestDecisionResult>.Failure(AccessResultStatus.Conflict);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<AccessResult<JoinRequestDecisionResult>> RejectAsync(
        Guid joinRequestId,
        RejectJoinRequestRequest request,
        CancellationToken cancellationToken)
    {
        var tenant = await GetActiveManagerTenantAsync(cancellationToken);
        if (tenant is null)
        {
            return AccessResult<JoinRequestDecisionResult>.Failure(AccessResultStatus.Unauthorized);
        }

        var rejectionReason = request.Reason.Trim();
        if (string.IsNullOrWhiteSpace(rejectionReason) || rejectionReason.Length > 500)
        {
            return AccessResult<JoinRequestDecisionResult>.Failure(AccessResultStatus.Invalid);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        try
        {
            var joinRequest = await LockTenantRequestAsync(
                tenant.Value.CompanyId,
                joinRequestId,
                cancellationToken);

            if (joinRequest is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<JoinRequestDecisionResult>.Failure(AccessResultStatus.NotFound);
            }

            if (joinRequest.Status != AccessConstants.PendingJoinRequestStatus)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<JoinRequestDecisionResult>.Failure(AccessResultStatus.Invalid);
            }

            var applicant = await LockTenantApplicantAsync(
                tenant.Value.CompanyId,
                joinRequest.UserId,
                cancellationToken);
            if (applicant is null
                || applicant.Status != AccessConstants.PendingApprovalUserStatus)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<JoinRequestDecisionResult>.Failure(AccessResultStatus.Conflict);
            }

            var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
            joinRequest.AssignedRole = null;
            joinRequest.Status = AccessConstants.RejectedJoinRequestStatus;
            joinRequest.ReviewedByUserId = tenant.Value.UserId;
            joinRequest.ReviewedAt = nowUtc;
            joinRequest.ReviewNotes = NormalizeOptional(request.ReviewNotes);
            joinRequest.RejectionReason = rejectionReason;
            joinRequest.CancelledAt = null;
            joinRequest.UpdatedAt = nowUtc;

            applicant.Status = AccessRoleRules.UserStatusForJoinRejection();
            applicant.UpdatedAt = nowUtc;
            applicant.VersionNumber++;

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return AccessResult<JoinRequestDecisionResult>.Success(
                new JoinRequestDecisionResult(
                    AccessConstants.RejectedOutcome,
                    applicant.Status,
                    Map(joinRequest, applicant)));
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<JoinRequestDecisionResult>.Failure(AccessResultStatus.Conflict);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private Task<JoinRequest?> LockTenantRequestAsync(
        Guid companyId,
        Guid joinRequestId,
        CancellationToken cancellationToken) =>
        dbContext.JoinRequests
            .FromSqlInterpolated(
                $"SELECT * FROM ahdah.join_requests WHERE company_id = {companyId} AND join_request_id = {joinRequestId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);

    private Task<AppUser?> LockTenantApplicantAsync(
        Guid companyId,
        Guid userId,
        CancellationToken cancellationToken) =>
        dbContext.AppUsers
            .FromSqlInterpolated(
                $"SELECT * FROM ahdah.app_users WHERE company_id = {companyId} AND user_id = {userId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);

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

    private static JoinRequestSummary Map(JoinRequest request, AppUser applicant) => new(
        request.JoinRequestId,
        applicant.UserId,
        applicant.FullName,
        applicant.PhoneNumber,
        applicant.Email,
        request.RequestedRole,
        request.AssignedRole,
        request.Status,
        request.RequestMessage,
        request.ReviewNotes,
        request.RejectionReason,
        ToUtc(request.RequestedAt),
        ToNullableUtc(request.ReviewedAt),
        ToNullableUtc(request.CancelledAt),
        ToUtc(request.CreatedAt),
        ToUtc(request.UpdatedAt));

    private static bool IsKnownStatus(string? status) =>
        status is null
            or AccessConstants.PendingJoinRequestStatus
            or AccessConstants.ApprovedJoinRequestStatus
            or AccessConstants.RejectedJoinRequestStatus
            or AccessConstants.CancelledJoinRequestStatus;

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
