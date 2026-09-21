using Ahdah.Application.Access.Models;
using Ahdah.Application.Advances;
using Ahdah.Application.Custody;
using Ahdah.Application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ahdah.Api.Controllers;

[ApiController]
[Route("api/v1")]
public sealed class CustodyController(ICustodyService service) : ControllerBase
{
    [Authorize(Policy = AhdahAuthorizationPolicies.CustodyViewer)]
    [HttpGet("advance-balances/{balanceId:guid}/custody")]
    public async Task<ActionResult<BalanceCustodyDetails>> GetBalance(Guid balanceId, CancellationToken ct) =>
        Result(await service.GetBalanceAsync(balanceId, ct));

    [Authorize(Policy = AhdahAuthorizationPolicies.CustodyViewer)]
    [HttpGet("projects/{projectId:guid}/custody")]
    public async Task<ActionResult<ProjectCustodyDetails>> GetProject(Guid projectId, CancellationToken ct) =>
        Result(await service.GetProjectAsync(projectId, ct));

    [Authorize(Policy = AhdahAuthorizationPolicies.CustodyPreparer)]
    [HttpPost("custody/designations")]
    public async Task<ActionResult<CustodyOperationDetails>> RequestDesignation(
        CreateDesignationRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken ct)
    {
        if (!AdvanceRules.IsValidIdempotencyKey(idempotencyKey)) return InvalidKey();
        var result = await service.RequestDesignationAsync(request, idempotencyKey!, ct);
        return result.Status == AccessResultStatus.Success ? StatusCode(201, result.Value) : Result(result);
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.CustodyApprover)]
    [HttpPost("custody/designations/{operationId:guid}/approve")]
    public async Task<ActionResult<CustodyOperationDetails>> ApproveDesignation(
        Guid operationId, ApproveDesignationRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken ct)
    {
        if (!AdvanceRules.IsValidIdempotencyKey(idempotencyKey)) return InvalidKey();
        return Result(await service.ApproveDesignationAsync(operationId, request, idempotencyKey!, ct));
    }

    private ActionResult<T> Result<T>(AccessResult<T> result) => result.Status switch
    {
        AccessResultStatus.Success => Ok(result.Value),
        AccessResultStatus.Invalid => ProblemResult(400, "Invalid custody operation", "custody.invalid_operation"),
        AccessResultStatus.NotFound => ProblemResult(404, "Custody operation unavailable", "custody.not_found"),
        AccessResultStatus.Conflict => ProblemResult(409, "Custody state conflict", "custody.conflict"),
        AccessResultStatus.Forbidden => ProblemResult(403, "Forbidden", "custody.forbidden"),
        AccessResultStatus.Unauthorized => ProblemResult(401, "Unauthorized", "authentication.stale_token"),
        _ => ProblemResult(500, "Unexpected server error", "server.unexpected_error")
    };

    private ObjectResult InvalidKey() => ProblemResult(400, "Invalid idempotency key", "custody.invalid_idempotency_key");
    private ObjectResult ProblemResult(int status, string title, string code) => Problem(
        statusCode: status, title: title, extensions: new Dictionary<string, object?> { ["code"] = code });
}
