using Ahdah.Application.Access.Models;
using Ahdah.Application.Projects.Contracts;
using Ahdah.Application.Projects.Models;

namespace Ahdah.Application.Projects.Services;

public interface IProjectService
{
    Task<AccessResult<PagedResult<ProjectDetails>>> ListAsync(
        ProjectQuery query,
        CancellationToken cancellationToken);

    Task<AccessResult<ProjectDetails>> GetAsync(
        Guid projectId,
        CancellationToken cancellationToken);

    Task<AccessResult<ProjectDetails>> CreateAsync(
        CreateProjectRequest request,
        CancellationToken cancellationToken);

    Task<AccessResult<ProjectDetails>> UpdateAsync(
        Guid projectId,
        UpdateProjectRequest request,
        CancellationToken cancellationToken);

    Task<AccessResult<ProjectDetails>> AssignSupervisorAsync(
        Guid projectId,
        AssignProjectSupervisorRequest request,
        CancellationToken cancellationToken);

    Task<AccessResult<PagedResult<ProjectMemberSummary>>> ListMembersAsync(
        Guid projectId,
        ProjectMemberQuery query,
        CancellationToken cancellationToken);
}
