using Ahdah.Application.Abstractions.Context;
using Ahdah.Application.Access;
using Ahdah.Application.Access.Models;
using Ahdah.Application.CompanyMembers.Contracts;
using Ahdah.Application.CompanyMembers.Models;
using Ahdah.Application.CompanyMembers.Services;
using Ahdah.Application.Identity;
using Ahdah.Infrastructure.Persistence.Generated.Context;
using Microsoft.EntityFrameworkCore;

namespace Ahdah.Infrastructure.CompanyMembers;

public sealed class CompanyMemberService(
    AhdahDbContext dbContext,
    ICurrentUserContext currentUserContext) : ICompanyMemberService
{
    public async Task<AccessResult<PagedResult<CompanyMemberSummary>>> ListAsync(
        CompanyMemberQuery query,
        CancellationToken cancellationToken)
    {
        var caller = await GetDirectoryCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<PagedResult<CompanyMemberSummary>>.Failure(
                AccessResultStatus.Unauthorized);
        }

        if (!IsValid(query))
        {
            return AccessResult<PagedResult<CompanyMemberSummary>>.Failure(
                AccessResultStatus.Invalid);
        }

        var members = dbContext.AppUsers
            .AsNoTracking()
            .Where(user => user.CompanyId == caller.Value.CompanyId);

        if (query.Role is not null)
        {
            members = members.Where(user => user.Role == query.Role);
        }

        if (query.Status is not null)
        {
            members = members.Where(user => user.Status == query.Status);
        }

        if (NormalizeOptional(query.Search) is { } search)
        {
            var normalizedSearch = search.ToLower();
            members = members.Where(user =>
                user.FullName.ToLower().StartsWith(normalizedSearch)
                || user.PhoneNumber.StartsWith(search));
        }

        var totalCount = await members.CountAsync(cancellationToken);
        var rows = await members
            .OrderBy(user => user.FullName)
            .ThenBy(user => user.UserId)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(user => new CompanyMemberSummary(
                user.UserId,
                user.FullName,
                user.Role,
                user.Status,
                user.IdentityVerificationStatus,
                ToUtc(user.CreatedAt)))
            .ToArrayAsync(cancellationToken);

        return AccessResult<PagedResult<CompanyMemberSummary>>.Success(
            new PagedResult<CompanyMemberSummary>(
                rows,
                query.Page,
                query.PageSize,
                totalCount,
                CalculateTotalPages(totalCount, query.PageSize)));
    }

    public async Task<AccessResult<CompanyMemberDetails>> GetAsync(
        Guid memberId,
        CancellationToken cancellationToken)
    {
        var caller = await GetDirectoryCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<CompanyMemberDetails>.Failure(AccessResultStatus.Unauthorized);
        }

        var member = await dbContext.AppUsers
            .AsNoTracking()
            .Where(user => user.CompanyId == caller.Value.CompanyId && user.UserId == memberId)
            .Select(user => new CompanyMemberDetails(
                user.UserId,
                user.FullName,
                user.Role,
                user.Status,
                user.IdentityVerificationStatus,
                user.PhoneNumber,
                user.Email,
                ToUtc(user.CreatedAt),
                ToUtc(user.UpdatedAt)))
            .SingleOrDefaultAsync(cancellationToken);

        return member is null
            ? AccessResult<CompanyMemberDetails>.Failure(AccessResultStatus.NotFound)
            : AccessResult<CompanyMemberDetails>.Success(member);
    }

    private async Task<(Guid CompanyId, Guid UserId)?> GetDirectoryCallerAsync(
        CancellationToken cancellationToken)
    {
        if (!currentUserContext.IsAuthenticated
            || currentUserContext.CompanyId is not { } companyId
            || currentUserContext.UserId is not { } userId
            || currentUserContext.Role is not { } claimRole
            || claimRole is not (IdentityConstants.ManagerRole or AccessConstants.DeputyRole))
        {
            return null;
        }

        var isValid = await dbContext.AppUsers
            .AsNoTracking()
            .AnyAsync(user =>
                user.CompanyId == companyId
                && user.UserId == userId
                && user.Role == claimRole
                && user.Status == IdentityConstants.ActiveStatus
                && user.Company.Status == IdentityConstants.ActiveStatus,
                cancellationToken);

        return isValid ? (companyId, userId) : null;
    }

    private static bool IsValid(CompanyMemberQuery query) =>
        query.Page >= 1
        && query.PageSize is >= 1 and <= AccessConstants.MaximumPageSize
        && IsKnownRole(query.Role)
        && IsKnownStatus(query.Status)
        && (query.Search is null || NormalizeOptional(query.Search) is { Length: >= 2 and <= 100 });

    private static bool IsKnownRole(string? role) => role is null
        or IdentityConstants.ManagerRole
        or AccessConstants.DeputyRole
        or AccessConstants.AccountantRole
        or AccessConstants.SupervisorRole
        or AccessConstants.WorkerRole;

    private static bool IsKnownStatus(string? status) => status is null
        or AccessConstants.PendingApprovalUserStatus
        or AccessConstants.ActiveUserStatus
        or "Suspended"
        or "Inactive"
        or AccessConstants.RejectedUserStatus;

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static DateTimeOffset ToUtc(DateTime value) =>
        new(DateTime.SpecifyKind(value, DateTimeKind.Utc));

    private static int CalculateTotalPages(int totalCount, int pageSize) =>
        totalCount == 0 ? 0 : (totalCount + pageSize - 1) / pageSize;
}
