using Ahdah.Application.Abstractions.Authentication;
using Ahdah.Application.Abstractions.Context;
using Ahdah.Application.Identity;
using Ahdah.Application.Identity.Contracts;
using Ahdah.Application.Identity.Models;
using Ahdah.Application.Identity.Services;
using Ahdah.Infrastructure.Persistence.Generated.Context;
using Ahdah.Infrastructure.Persistence.Generated.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Ahdah.Infrastructure.Identity;

public sealed class IdentityService(
    AhdahDbContext dbContext,
    IPasswordHashingService passwordHashingService,
    IAccessTokenService accessTokenService,
    ICurrentUserContext currentUserContext,
    TimeProvider timeProvider) : IIdentityService
{
    public async Task<IdentityResult<RegisterCompanyResult>> RegisterCompanyAsync(
        RegisterCompanyRequest request,
        CancellationToken cancellationToken)
    {
        var companyName = request.CompanyName.Trim();
        var companyCode = request.CompanyCode.Trim().ToUpperInvariant();
        var managerName = request.ManagerFullName.Trim();
        var managerPhone = request.ManagerPhone.Trim();
        var managerEmail = NormalizeOptional(request.ManagerEmail);
        var passwordHash = passwordHashingService.HashPassword(request.Password);
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var hasConflict = await dbContext.Companies
                    .AsNoTracking()
                    .AnyAsync(company => company.CompanyCode == companyCode, cancellationToken)
                || await dbContext.AppUsers
                    .AsNoTracking()
                    .AnyAsync(user => user.PhoneNumber == managerPhone, cancellationToken);

            if (hasConflict)
            {
                await transaction.RollbackAsync(cancellationToken);
                return IdentityResult<RegisterCompanyResult>.Failure(IdentityResultStatus.Conflict);
            }

            var company = new Company
            {
                CompanyName = companyName,
                CompanyCode = companyCode,
                Status = IdentityConstants.ActiveStatus
            };

            var manager = new AppUser
            {
                Company = company,
                FullName = managerName,
                PhoneNumber = managerPhone,
                Email = managerEmail,
                PasswordHash = passwordHash,
                Role = IdentityConstants.ManagerRole,
                Status = IdentityConstants.ActiveStatus,
                IdentityVerificationStatus = IdentityConstants.VerifiedIdentityStatus,
                IdentityVerifiedAt = nowUtc,
                FailedLoginAttempts = 0,
                MustChangePassword = false
            };

            dbContext.Companies.Add(company);
            dbContext.AppUsers.Add(manager);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var companySummary = MapCompany(company);
            var managerSummary = MapUser(manager);
            var authentication = CreateAuthentication(managerSummary, companySummary);

            return IdentityResult<RegisterCompanyResult>.Success(
                new RegisterCompanyResult(companySummary, managerSummary, authentication));
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            return IdentityResult<RegisterCompanyResult>.Failure(IdentityResultStatus.Conflict);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<IdentityResult<AuthenticationResult>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var phoneNumber = request.PhoneNumber.Trim();
        var user = await dbContext.AppUsers
            .Include(candidate => candidate.Company)
            .SingleOrDefaultAsync(candidate => candidate.PhoneNumber == phoneNumber, cancellationToken);

        var nowUtc = timeProvider.GetUtcNow();
        if (user is null
            || user.Status != IdentityConstants.ActiveStatus
            || user.Company.Status != IdentityConstants.ActiveStatus
            || user.LockedUntil is not null && user.LockedUntil.Value > nowUtc.UtcDateTime)
        {
            return IdentityResult<AuthenticationResult>.Failure(IdentityResultStatus.InvalidCredentials);
        }

        var verificationResult = passwordHashingService.VerifyPassword(user.PasswordHash, request.Password);
        if (verificationResult == Ahdah.Application.Abstractions.Authentication.PasswordVerificationResult.Failed)
        {
            return IdentityResult<AuthenticationResult>.Failure(IdentityResultStatus.InvalidCredentials);
        }

        if (verificationResult == Ahdah.Application.Abstractions.Authentication.PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHashingService.HashPassword(request.Password);
        }

        user.LastLoginAt = nowUtc.UtcDateTime;
        user.UpdatedAt = nowUtc.UtcDateTime;
        user.VersionNumber++;
        await dbContext.SaveChangesAsync(cancellationToken);

        var userSummary = MapUser(user);
        var companySummary = MapCompany(user.Company);
        return IdentityResult<AuthenticationResult>.Success(
            CreateAuthentication(userSummary, companySummary));
    }

    public async Task<IdentityResult<CurrentUserResult>> GetCurrentUserAsync(
        CancellationToken cancellationToken)
    {
        if (!currentUserContext.IsAuthenticated
            || currentUserContext.UserId is not { } userId
            || currentUserContext.CompanyId is not { } companyId)
        {
            return IdentityResult<CurrentUserResult>.Failure(IdentityResultStatus.Unauthorized);
        }

        var currentUser = await dbContext.AppUsers
            .AsNoTracking()
            .Where(user => user.CompanyId == companyId && user.UserId == userId)
            .Select(user => new CurrentUserResult(
                new UserSummary(user.UserId, user.FullName, user.Role, user.Status),
                new CompanySummary(
                    user.Company.CompanyId,
                    user.Company.CompanyName,
                    user.Company.CompanyCode,
                    user.Company.Status),
                user.Role))
            .SingleOrDefaultAsync(cancellationToken);

        if (currentUser is null
            || currentUser.User.Status != IdentityConstants.ActiveStatus
            || currentUser.Company.Status != IdentityConstants.ActiveStatus)
        {
            return IdentityResult<CurrentUserResult>.Failure(IdentityResultStatus.Unauthorized);
        }

        return IdentityResult<CurrentUserResult>.Success(currentUser);
    }

    private AuthenticationResult CreateAuthentication(
        UserSummary user,
        CompanySummary company)
    {
        var token = accessTokenService.CreateToken(
            new AccessTokenSubject(user.UserId, company.CompanyId, user.Role, user.FullName));

        return new AuthenticationResult(
            token.Token,
            "Bearer",
            token.ExpiresAtUtc,
            user,
            company,
            user.Role);
    }

    private static UserSummary MapUser(AppUser user) =>
        new(user.UserId, user.FullName, user.Role, user.Status);

    private static CompanySummary MapCompany(Company company) =>
        new(company.CompanyId, company.CompanyName, company.CompanyCode, company.Status);

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static bool IsUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
}
