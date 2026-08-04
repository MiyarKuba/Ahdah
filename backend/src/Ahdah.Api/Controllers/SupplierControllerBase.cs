using Ahdah.Application.Access.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ahdah.Api.Controllers;

public abstract class SupplierControllerBase : ControllerBase
{
    protected ActionResult<T> SupplierResult<T>(AccessResult<T> result) => result.Status switch
    {
        AccessResultStatus.Success => Ok(result.Value),
        AccessResultStatus.Invalid => SupplierProblem(400, "Invalid supplier operation",
            "The supplier operation is invalid or unsupported in the current lifecycle state.",
            "suppliers.invalid_operation"),
        AccessResultStatus.NotFound => SupplierProblem(404, "Supplier operation unavailable",
            "The requested supplier record was not found.", "suppliers.not_found"),
        AccessResultStatus.Conflict => SupplierProblem(409, "Supplier financial conflict",
            "The operation conflicts with current financial allocations, balances, or lifecycle state.",
            "suppliers.conflict"),
        AccessResultStatus.Forbidden => SupplierProblem(403, "Forbidden",
            "You are not authorized to perform this supplier operation.", "suppliers.forbidden"),
        AccessResultStatus.Unauthorized => SupplierProblem(401, "Unauthorized",
            "The access token is no longer valid for an active company member.",
            "authentication.stale_token"),
        _ => SupplierProblem(500, "Unexpected server error", "An unexpected error occurred.",
            "server.unexpected_error")
    };

    protected ObjectResult InvalidSupplierIdempotencyKey() => SupplierProblem(400,
        "Invalid idempotency key",
        "A 16 to 200 character Idempotency-Key header is required for this supplier financial command.",
        "suppliers.invalid_idempotency_key");

    private ObjectResult SupplierProblem(int status, string title, string detail, string code) =>
        Problem(statusCode: status, title: title, detail: detail,
            extensions: new Dictionary<string, object?> { ["code"] = code });
}
