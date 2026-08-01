using Ahdah.Application.Access.Models;
using Ahdah.Application.CompanyMembers.Contracts;
using Ahdah.Application.CompanyMembers.Models;
using Ahdah.Application.CompanyMembers.Services;
using Ahdah.Application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ahdah.Api.Controllers;

[ApiController]
[Route("api/v1/company/members")]
[Authorize(Policy = AhdahAuthorizationPolicies.CompanyDirectoryViewer)]
public sealed class CompanyMembersController(ICompanyMemberService companyMemberService)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<CompanyMemberSummary>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<CompanyMemberSummary>>> List(
        [FromQuery] CompanyMemberQuery query,
        CancellationToken cancellationToken)
    {
        var result = await companyMemberService.ListAsync(query, cancellationToken);
        return result.Status switch
        {
            AccessResultStatus.Success => Ok(result.Value),
            AccessResultStatus.Invalid => StructureProblem(
                StatusCodes.Status400BadRequest,
                "Invalid member query",
                "The company-member query is invalid.",
                "company_members.invalid_query"),
            AccessResultStatus.Unauthorized => StaleTokenProblem(),
            _ => UnexpectedProblem()
        };
    }

    [HttpGet("{memberId:guid}")]
    [ProducesResponseType<CompanyMemberDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyMemberDetails>> Get(
        Guid memberId,
        CancellationToken cancellationToken)
    {
        var result = await companyMemberService.GetAsync(memberId, cancellationToken);
        return result.Status switch
        {
            AccessResultStatus.Success => Ok(result.Value),
            AccessResultStatus.NotFound => StructureProblem(
                StatusCodes.Status404NotFound,
                "Company member not found",
                "The company member was not found.",
                "company_members.not_found"),
            AccessResultStatus.Unauthorized => StaleTokenProblem(),
            _ => UnexpectedProblem()
        };
    }

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
