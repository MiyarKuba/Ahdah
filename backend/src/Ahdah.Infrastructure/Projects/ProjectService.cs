using System.ComponentModel.DataAnnotations;
using System.Data;
using Ahdah.Application.Abstractions.Context;
using Ahdah.Application.Access;
using Ahdah.Application.Access.Models;
using Ahdah.Application.Identity;
using Ahdah.Application.Projects;
using Ahdah.Application.Projects.Contracts;
using Ahdah.Application.Projects.Models;
using Ahdah.Application.Projects.Services;
using Ahdah.Infrastructure.Persistence.Generated.Context;
using Ahdah.Infrastructure.Persistence.Generated.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Ahdah.Infrastructure.Projects;

public sealed class ProjectService(
    AhdahDbContext dbContext,
    ICurrentUserContext currentUserContext,
    TimeProvider timeProvider) : IProjectService
{
    public async Task<AccessResult<PagedResult<ProjectDetails>>> ListAsync(
        ProjectQuery query,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<PagedResult<ProjectDetails>>.Failure(AccessResultStatus.Unauthorized);
        }

        if (!IsValid(query))
        {
            return AccessResult<PagedResult<ProjectDetails>>.Failure(AccessResultStatus.Invalid);
        }

        var projects = ApplyVisibility(
            dbContext.Projects
                .AsNoTracking()
                .Where(project => project.CompanyId == caller.CompanyId),
            caller);

        if (query.Status is not null)
        {
            projects = projects.Where(project => project.Status == query.Status);
        }

        if (NormalizeOptional(query.Search) is { } search)
        {
            var normalizedSearch = search.ToLower();
            projects = projects.Where(project =>
                project.ProjectName.ToLower().StartsWith(normalizedSearch)
                || project.SiteAddress.ToLower().StartsWith(normalizedSearch));
        }

        var totalCount = await projects.CountAsync(cancellationToken);
        var rows = await SelectProjectRows(projects, caller.CanViewContractValue)
            .OrderByDescending(project => project.CreatedAt)
            .ThenByDescending(project => project.ProjectId)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToArrayAsync(cancellationToken);
        var supervisors = await LoadSupervisorLookupAsync(
            caller.CompanyId,
            rows.Select(row => row.ProjectId).ToArray(),
            cancellationToken);

        return AccessResult<PagedResult<ProjectDetails>>.Success(
            new PagedResult<ProjectDetails>(
                rows.Select(row => Map(row, supervisors)).ToArray(),
                query.Page,
                query.PageSize,
                totalCount,
                CalculateTotalPages(totalCount, query.PageSize)));
    }

    public async Task<AccessResult<ProjectDetails>> GetAsync(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Unauthorized);
        }

        return await LoadVisibleProjectAsync(caller, projectId, cancellationToken);
    }

    public async Task<AccessResult<ProjectDetails>> CreateAsync(
        CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Unauthorized);
        }

        if (!caller.Capabilities.CanCreate || !IsValid(request))
        {
            return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Invalid);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        try
        {
            ProjectOwner owner;
            if (request.ProjectOwnerId is { } ownerId)
            {
                var existingOwner = await dbContext.ProjectOwners
                    .SingleOrDefaultAsync(candidate =>
                        candidate.CompanyId == caller.CompanyId
                        && candidate.ProjectOwnerId == ownerId
                        && candidate.IsActive,
                        cancellationToken);

                if (existingOwner is null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Invalid);
                }

                owner = existingOwner;
            }
            else
            {
                var newOwner = request.NewOwner!;
                var ownerPhone = newOwner.PhoneNumber.Trim();
                var ownerEmail = NormalizeOptional(newOwner.Email);
                var ownerConflict = await dbContext.ProjectOwners
                    .AsNoTracking()
                    .AnyAsync(candidate =>
                        candidate.CompanyId == caller.CompanyId
                        && (candidate.PhoneNumber == ownerPhone
                            || ownerEmail != null
                            && candidate.Email != null
                            && candidate.Email.ToLower() == ownerEmail.ToLower()),
                        cancellationToken);

                if (ownerConflict)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Conflict);
                }

                owner = new ProjectOwner
                {
                    CompanyId = caller.CompanyId,
                    OwnerName = newOwner.OwnerName.Trim(),
                    PhoneNumber = ownerPhone,
                    Email = ownerEmail,
                    Address = NormalizeOptional(newOwner.Address),
                    Notes = NormalizeOptional(newOwner.Notes),
                    IsActive = true,
                    CreatedByUserId = caller.UserId
                };
                dbContext.ProjectOwners.Add(owner);
            }

            AppUser? supervisor = null;
            if (request.SupervisorUserId is { } supervisorUserId)
            {
                supervisor = await LoadEligibleSupervisorAsync(
                    caller.CompanyId,
                    supervisorUserId,
                    cancellationToken);
                if (supervisor is null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Invalid);
                }
            }

            var project = new Project
            {
                CompanyId = caller.CompanyId,
                ProjectOwner = owner,
                ProjectName = request.ProjectName.Trim(),
                SiteAddress = request.SiteAddress.Trim(),
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                ContactPhoneNumber = NormalizeOptional(request.ContactPhoneNumber),
                ContractValue = request.ContractValue,
                ContractDate = request.ContractDate,
                StartDate = request.StartDate,
                ExpectedEndDate = request.ExpectedEndDate,
                Status = ProjectConstants.ActiveStatus,
                Description = NormalizeOptional(request.Description),
                Notes = NormalizeOptional(request.Notes),
                CreatedByUserId = caller.UserId
            };
            dbContext.Projects.Add(project);

            if (supervisor is not null)
            {
                dbContext.ProjectSupervisors.Add(new ProjectSupervisor
                {
                    CompanyId = caller.CompanyId,
                    Project = project,
                    SupervisorUserId = supervisor.UserId,
                    AssignedByUserId = caller.UserId,
                    IsActive = true
                });
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return await LoadVisibleProjectAsync(caller, project.ProjectId, cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<AccessResult<ProjectDetails>> UpdateAsync(
        Guid projectId,
        UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Unauthorized);
        }

        if (!caller.Capabilities.CanUpdate || !IsValid(request))
        {
            return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Invalid);
        }

        var project = await dbContext.Projects
            .SingleOrDefaultAsync(candidate =>
                candidate.CompanyId == caller.CompanyId
                && candidate.ProjectId == projectId,
                cancellationToken);

        if (project is null)
        {
            return AccessResult<ProjectDetails>.Failure(AccessResultStatus.NotFound);
        }

        if (project.VersionNumber != request.ExpectedVersion)
        {
            return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Conflict);
        }

        if (!ProjectLifecycleRules.IsMutableStatus(project.Status))
        {
            return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Invalid);
        }

        if (request.ProjectOwnerId is { } ownerId)
        {
            var ownerExists = await dbContext.ProjectOwners
                .AsNoTracking()
                .AnyAsync(owner =>
                    owner.CompanyId == caller.CompanyId
                    && owner.ProjectOwnerId == ownerId
                    && owner.IsActive,
                    cancellationToken);
            if (!ownerExists)
            {
                return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Invalid);
            }

            project.ProjectOwnerId = ownerId;
        }

        var effectiveStartDate = request.StartDate ?? project.StartDate;
        var effectiveExpectedEndDate = request.ExpectedEndDate ?? project.ExpectedEndDate;
        if (!ProjectRequestRules.AreDatesValid(effectiveStartDate, effectiveExpectedEndDate)
            || project.ActualEndDate is { } actualEndDate && actualEndDate < effectiveStartDate)
        {
            return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Invalid);
        }

        project.ProjectName = request.ProjectName?.Trim() ?? project.ProjectName;
        project.SiteAddress = request.SiteAddress?.Trim() ?? project.SiteAddress;
        project.ContactPhoneNumber = request.ContactPhoneNumber is null
            ? project.ContactPhoneNumber
            : NormalizeOptional(request.ContactPhoneNumber);
        project.ContractDate = request.ContractDate ?? project.ContractDate;
        project.StartDate = effectiveStartDate;
        project.ExpectedEndDate = effectiveExpectedEndDate;
        project.Status = request.Status ?? project.Status;
        project.Description = request.Description is null
            ? project.Description
            : NormalizeOptional(request.Description);
        project.Notes = request.Notes is null ? project.Notes : NormalizeOptional(request.Notes);
        project.UpdatedAt = timeProvider.GetUtcNow().UtcDateTime;
        project.VersionNumber++;

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return await LoadVisibleProjectAsync(caller, projectId, cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Conflict);
        }
    }

    public async Task<AccessResult<ProjectDetails>> AssignSupervisorAsync(
        Guid projectId,
        AssignProjectSupervisorRequest request,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Unauthorized);
        }

        if (!caller.Capabilities.CanAssignSupervisor
            || request.SupervisorUserId == Guid.Empty
            || request.ExpectedVersion < 1)
        {
            return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Invalid);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        try
        {
            var project = await dbContext.Projects
                .FromSqlInterpolated(
                    $"SELECT * FROM ahdah.projects WHERE company_id = {caller.CompanyId} AND project_id = {projectId} FOR UPDATE")
                .SingleOrDefaultAsync(cancellationToken);
            if (project is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ProjectDetails>.Failure(AccessResultStatus.NotFound);
            }

            if (project.VersionNumber != request.ExpectedVersion)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Conflict);
            }

            if (!ProjectLifecycleRules.IsMutableStatus(project.Status))
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Invalid);
            }

            var supervisor = await LoadEligibleSupervisorAsync(
                caller.CompanyId,
                request.SupervisorUserId,
                cancellationToken);
            if (supervisor is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Invalid);
            }

            var activeAssignments = await dbContext.ProjectSupervisors
                .Where(assignment =>
                    assignment.CompanyId == caller.CompanyId
                    && assignment.ProjectId == projectId
                    && assignment.IsActive)
                .ToArrayAsync(cancellationToken);
            var selectedAssignment = activeAssignments.SingleOrDefault(
                assignment => assignment.SupervisorUserId == supervisor.UserId);
            var assignmentsToRemove = activeAssignments
                .Where(assignment => assignment.SupervisorUserId != supervisor.UserId)
                .ToArray();

            if (selectedAssignment is not null && assignmentsToRemove.Length == 0)
            {
                await transaction.CommitAsync(cancellationToken);
                return await LoadVisibleProjectAsync(caller, projectId, cancellationToken);
            }

            var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
            foreach (var assignment in assignmentsToRemove)
            {
                assignment.IsActive = false;
                assignment.RemovedByUserId = caller.UserId;
                assignment.RemovedAt = nowUtc;
                assignment.RemovalReason = "Replaced by manager.";
                assignment.UpdatedAt = nowUtc;
            }

            if (selectedAssignment is null)
            {
                dbContext.ProjectSupervisors.Add(new ProjectSupervisor
                {
                    CompanyId = caller.CompanyId,
                    ProjectId = projectId,
                    SupervisorUserId = supervisor.UserId,
                    AssignedByUserId = caller.UserId,
                    AssignedAt = nowUtc,
                    IsActive = true
                });
            }

            project.UpdatedAt = nowUtc;
            project.VersionNumber++;
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return await LoadVisibleProjectAsync(caller, projectId, cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            return AccessResult<ProjectDetails>.Failure(AccessResultStatus.Conflict);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<AccessResult<PagedResult<ProjectMemberSummary>>> ListMembersAsync(
        Guid projectId,
        ProjectMemberQuery query,
        CancellationToken cancellationToken)
    {
        var caller = await GetActiveCallerAsync(cancellationToken);
        if (caller is null)
        {
            return AccessResult<PagedResult<ProjectMemberSummary>>.Failure(
                AccessResultStatus.Unauthorized);
        }

        if (query.Page < 1 || query.PageSize is < 1 or > AccessConstants.MaximumPageSize)
        {
            return AccessResult<PagedResult<ProjectMemberSummary>>.Failure(AccessResultStatus.Invalid);
        }

        if (!caller.Capabilities.CanViewProjectMembers)
        {
            return AccessResult<PagedResult<ProjectMemberSummary>>.Failure(
                caller.Role == AccessConstants.AccountantRole
                    ? AccessResultStatus.Forbidden
                    : AccessResultStatus.NotFound);
        }

        var visibleProject = ApplyVisibility(
            dbContext.Projects
                .AsNoTracking()
                .Where(project =>
                    project.CompanyId == caller.CompanyId
                    && project.ProjectId == projectId),
            caller);
        if (!await visibleProject.AnyAsync(cancellationToken))
        {
            return AccessResult<PagedResult<ProjectMemberSummary>>.Failure(
                AccessResultStatus.NotFound);
        }

        var members = dbContext.ProjectSupervisors
            .AsNoTracking()
            .Where(assignment =>
                assignment.CompanyId == caller.CompanyId
                && assignment.ProjectId == projectId
                && assignment.IsActive
                && assignment.AppUser1.CompanyId == caller.CompanyId);
        var totalCount = await members.CountAsync(cancellationToken);
        var rows = await members
            .OrderBy(assignment => assignment.AppUser1.FullName)
            .ThenBy(assignment => assignment.SupervisorUserId)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(assignment => new ProjectMemberSummary(
                assignment.AppUser1.UserId,
                assignment.AppUser1.FullName,
                assignment.AppUser1.Role,
                assignment.AppUser1.Status,
                assignment.AppUser1.IdentityVerificationStatus,
                assignment.AppUser1.PhoneNumber,
                assignment.AppUser1.Email,
                ToUtc(assignment.AssignedAt)))
            .ToArrayAsync(cancellationToken);

        return AccessResult<PagedResult<ProjectMemberSummary>>.Success(
            new PagedResult<ProjectMemberSummary>(
                rows,
                query.Page,
                query.PageSize,
                totalCount,
                CalculateTotalPages(totalCount, query.PageSize)));
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

        var caller = await dbContext.AppUsers
            .AsNoTracking()
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

        return new Caller(
            companyId,
            userId,
            caller.Role,
            ProjectRoleCapabilities.For(caller.Role));
    }

    private static IQueryable<Project> ApplyVisibility(
        IQueryable<Project> projects,
        Caller caller)
    {
        if (caller.Capabilities.CanViewAllCompanyProjects)
        {
            return projects;
        }

        if (caller.Capabilities.RequiresSupervisorAssignment)
        {
            return projects.Where(project => project.ProjectSupervisors.Any(assignment =>
                assignment.CompanyId == caller.CompanyId
                && assignment.SupervisorUserId == caller.UserId
                && assignment.IsActive));
        }

        return projects.Where(_ => false);
    }

    private async Task<AccessResult<ProjectDetails>> LoadVisibleProjectAsync(
        Caller caller,
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var projects = ApplyVisibility(
            dbContext.Projects
                .AsNoTracking()
                .Where(project =>
                    project.CompanyId == caller.CompanyId
                    && project.ProjectId == projectId),
            caller);
        var row = await SelectProjectRows(projects, caller.CanViewContractValue)
            .SingleOrDefaultAsync(cancellationToken);
        if (row is null)
        {
            return AccessResult<ProjectDetails>.Failure(AccessResultStatus.NotFound);
        }

        var supervisors = await LoadSupervisorLookupAsync(
            caller.CompanyId,
            [projectId],
            cancellationToken);
        return AccessResult<ProjectDetails>.Success(Map(row, supervisors));
    }

    private static IQueryable<ProjectRow> SelectProjectRows(
        IQueryable<Project> projects,
        bool canViewContractValue) =>
        projects.Select(project => new ProjectRow(
            project.ProjectId,
            project.ProjectName,
            project.ProjectOwner.ProjectOwnerId,
            project.ProjectOwner.OwnerName,
            project.ProjectOwner.PhoneNumber,
            project.ProjectOwner.Email,
            project.ProjectOwner.Address,
            project.SiteAddress,
            project.Latitude,
            project.Longitude,
            project.ContactPhoneNumber,
            canViewContractValue ? project.ContractValue : null,
            project.ContractDate,
            project.StartDate,
            project.ExpectedEndDate,
            project.ActualEndDate,
            project.Status,
            project.Description,
            project.Notes,
            project.VersionNumber,
            project.CreatedAt,
            project.UpdatedAt));

    private async Task<IReadOnlyDictionary<Guid, IReadOnlyList<ProjectSupervisorSummary>>>
        LoadSupervisorLookupAsync(
            Guid companyId,
            Guid[] projectIds,
            CancellationToken cancellationToken)
    {
        if (projectIds.Length == 0)
        {
            return new Dictionary<Guid, IReadOnlyList<ProjectSupervisorSummary>>();
        }

        var rows = await dbContext.ProjectSupervisors
            .AsNoTracking()
            .Where(assignment =>
                assignment.CompanyId == companyId
                && projectIds.Contains(assignment.ProjectId)
                && assignment.IsActive
                && assignment.AppUser1.CompanyId == companyId)
            .OrderBy(assignment => assignment.AssignedAt)
            .ThenBy(assignment => assignment.ProjectSupervisorId)
            .Select(assignment => new SupervisorRow(
                assignment.ProjectId,
                assignment.SupervisorUserId,
                assignment.AppUser1.FullName,
                assignment.AppUser1.Status,
                assignment.AssignedAt))
            .ToArrayAsync(cancellationToken);

        return rows
            .GroupBy(row => row.ProjectId)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<ProjectSupervisorSummary>)group
                    .Select(row => new ProjectSupervisorSummary(
                        row.UserId,
                        row.FullName,
                        row.Status,
                        ToUtc(row.AssignedAt)))
                    .ToArray());
    }

    private Task<AppUser?> LoadEligibleSupervisorAsync(
        Guid companyId,
        Guid userId,
        CancellationToken cancellationToken) =>
        dbContext.AppUsers.SingleOrDefaultAsync(user =>
            user.CompanyId == companyId
            && user.UserId == userId
            && user.Role == AccessConstants.SupervisorRole
            && user.Status == AccessConstants.ActiveUserStatus,
            cancellationToken);

    private static ProjectDetails Map(
        ProjectRow row,
        IReadOnlyDictionary<Guid, IReadOnlyList<ProjectSupervisorSummary>> supervisors) =>
        new(
            row.ProjectId,
            row.ProjectName,
            new ProjectOwnerSummary(
                row.ProjectOwnerId,
                row.OwnerName,
                row.OwnerPhoneNumber,
                row.OwnerEmail,
                row.OwnerAddress),
            row.SiteAddress,
            row.Latitude,
            row.Longitude,
            row.ContactPhoneNumber,
            row.ContractValue,
            row.ContractDate,
            row.StartDate,
            row.ExpectedEndDate,
            row.ActualEndDate,
            row.Status,
            row.Description,
            row.Notes,
            supervisors.GetValueOrDefault(row.ProjectId, []),
            row.VersionNumber,
            ToUtc(row.CreatedAt),
            ToUtc(row.UpdatedAt));

    private static bool IsValid(ProjectQuery query) =>
        query.Page >= 1
        && query.PageSize is >= 1 and <= AccessConstants.MaximumPageSize
        && (query.Status is null || ProjectLifecycleRules.IsKnownStatus(query.Status))
        && ProjectRequestRules.IsSearchValid(query.Search)
        && (query.Search is null || NormalizeOptional(query.Search) is not null);

    private static bool IsValid(CreateProjectRequest request) =>
        Validate(request)
        && (request.NewOwner is null || Validate(request.NewOwner));

    private static bool IsValid(UpdateProjectRequest request) => Validate(request);

    private static bool Validate(object value)
    {
        var results = new List<ValidationResult>();
        return Validator.TryValidateObject(value, new ValidationContext(value), results, true);
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static DateTimeOffset ToUtc(DateTime value) =>
        new(DateTime.SpecifyKind(value, DateTimeKind.Utc));

    private static int CalculateTotalPages(int totalCount, int pageSize) =>
        totalCount == 0 ? 0 : (totalCount + pageSize - 1) / pageSize;

    private static bool IsUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };

    private sealed record Caller(
        Guid CompanyId,
        Guid UserId,
        string Role,
        ProjectRoleCapabilities Capabilities)
    {
        public bool CanViewContractValue => Capabilities.CanViewContractValue;
    }

    private sealed record ProjectRow(
        Guid ProjectId,
        string ProjectName,
        Guid ProjectOwnerId,
        string OwnerName,
        string OwnerPhoneNumber,
        string? OwnerEmail,
        string? OwnerAddress,
        string SiteAddress,
        decimal? Latitude,
        decimal? Longitude,
        string? ContactPhoneNumber,
        decimal? ContractValue,
        DateOnly ContractDate,
        DateOnly StartDate,
        DateOnly? ExpectedEndDate,
        DateOnly? ActualEndDate,
        string Status,
        string? Description,
        string? Notes,
        int VersionNumber,
        DateTime CreatedAt,
        DateTime UpdatedAt);

    private sealed record SupervisorRow(
        Guid ProjectId,
        Guid UserId,
        string FullName,
        string Status,
        DateTime AssignedAt);
}
