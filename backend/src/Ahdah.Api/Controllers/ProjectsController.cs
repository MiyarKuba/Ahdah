using Ahdah.Application.Access.Models;
using Ahdah.Application.Identity;
using Ahdah.Application.Projects.Contracts;
using Ahdah.Application.Projects.Models;
using Ahdah.Application.Projects.Services;
using Ahdah.Application.Settlements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ahdah.Api.Controllers;

[ApiController]
[Route("api/v1/projects")]
public sealed class ProjectsController(IProjectService projectService) : ControllerBase
{
    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierViewer)]
    [HttpGet("{projectId:guid}/settlement")]
    [ProducesResponseType<ProjectSettlementSummary>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectSettlementSummary>> Settlement(
        Guid projectId,
        [FromServices] IProjectSettlementService settlementService,
        CancellationToken cancellationToken)
    {
        return ProjectResult(await settlementService.GetAsync(projectId, cancellationToken),
            "projects.settlement.invalid_request");
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.ProjectViewer)]
    [HttpGet]
    [ProducesResponseType<PagedResult<ProjectDetails>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<ProjectDetails>>> List(
        [FromQuery] ProjectQuery query,
        CancellationToken cancellationToken)
    {
        var result = await projectService.ListAsync(query, cancellationToken);
        return ProjectResult(result, "projects.invalid_query");
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.ProjectViewer)]
    [HttpGet("{projectId:guid}")]
    [ProducesResponseType<ProjectDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDetails>> Get(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var result = await projectService.GetAsync(projectId, cancellationToken);
        return ProjectResult(result, "projects.invalid_request");
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.ManagerOnly)]
    [HttpPost]
    [ProducesResponseType<ProjectDetails>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProjectDetails>> Create(
        CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var result = await projectService.CreateAsync(request, cancellationToken);
        if (result.Status == AccessResultStatus.Success)
        {
            return StatusCode(StatusCodes.Status201Created, result.Value);
        }

        return ProjectResult(result, "projects.invalid_creation");
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.ManagerOnly)]
    [HttpPatch("{projectId:guid}")]
    [ProducesResponseType<ProjectDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProjectDetails>> Update(
        Guid projectId,
        UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var result = await projectService.UpdateAsync(projectId, request, cancellationToken);
        return ProjectResult(result, "projects.invalid_update");
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.ManagerOnly)]
    [HttpPut("{projectId:guid}/supervisor")]
    [ProducesResponseType<ProjectDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProjectDetails>> AssignSupervisor(
        Guid projectId,
        AssignProjectSupervisorRequest request,
        CancellationToken cancellationToken)
    {
        var result = await projectService.AssignSupervisorAsync(
            projectId,
            request,
            cancellationToken);
        return ProjectResult(result, "projects.invalid_supervisor_assignment");
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.ProjectViewer)]
    [HttpGet("{projectId:guid}/members")]
    [ProducesResponseType<PagedResult<ProjectMemberSummary>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<ProjectMemberSummary>>> ListMembers(
        Guid projectId,
        [FromQuery] ProjectMemberQuery query,
        CancellationToken cancellationToken)
    {
        var result = await projectService.ListMembersAsync(projectId, query, cancellationToken);
        return result.Status switch
        {
            AccessResultStatus.Success => Ok(result.Value),
            AccessResultStatus.Invalid => StructureProblem(
                StatusCodes.Status400BadRequest,
                "Invalid project-member query",
                "The project-member query is invalid.",
                "projects.members.invalid_query"),
            AccessResultStatus.Forbidden => StructureProblem(
                StatusCodes.Status403Forbidden,
                "Forbidden",
                "You are not authorized to view project members.",
                "projects.members.forbidden"),
            AccessResultStatus.NotFound => NotFoundProblem(),
            AccessResultStatus.Unauthorized => StaleTokenProblem(),
            _ => UnexpectedProblem()
        };
    }

    private ActionResult<T> ProjectResult<T>(AccessResult<T> result, string invalidCode) =>
        result.Status switch
        {
            AccessResultStatus.Success => Ok(result.Value),
            AccessResultStatus.Invalid => StructureProblem(
                StatusCodes.Status400BadRequest,
                "Invalid project operation",
                "The project operation is invalid or unsupported in the current lifecycle state.",
                invalidCode),
            AccessResultStatus.NotFound => NotFoundProblem(),
            AccessResultStatus.Conflict => StructureProblem(
                StatusCodes.Status409Conflict,
                "Project conflict",
                "The project or assignment was changed by another request or conflicts with existing data.",
                "projects.concurrency_conflict"),
            AccessResultStatus.Forbidden => StructureProblem(
                StatusCodes.Status403Forbidden,
                "Forbidden",
                "You are not authorized to perform this project operation.",
                "projects.forbidden"),
            AccessResultStatus.Unauthorized => StaleTokenProblem(),
            _ => UnexpectedProblem()
        };

    private ObjectResult NotFoundProblem() => StructureProblem(
        StatusCodes.Status404NotFound,
        "Project not found",
        "The project was not found.",
        "projects.not_found");

    private ObjectResult StaleTokenProblem() => StructureProblem(
        StatusCodes.Status401Unauthorized,
        "Unauthorized",
        "The access token is no longer valid for an active company member.",
        "authentication.stale_token");

    private ObjectResult UnexpectedProblem() => StructureProblem(
        StatusCodes.Status500InternalServerError,
        "Unexpected server error",
        "An unexpected error occurred.",
        "server.unexpected_error");

    private ObjectResult StructureProblem(int status, string title, string detail, string code) =>
        Problem(
            statusCode: status,
            title: title,
            detail: detail,
            extensions: new Dictionary<string, object?> { ["code"] = code });
}
