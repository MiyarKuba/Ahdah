using Ahdah.Application.Access.Models;
using Ahdah.Application.Identity;
using Ahdah.Application.Suppliers;
using Ahdah.Application.Suppliers.Contracts;
using Ahdah.Application.Suppliers.Models;
using Ahdah.Application.Suppliers.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ahdah.Api.Controllers;

[ApiController]
[Route("api/v1/supplier-payments")]
public sealed class SupplierPaymentsController(ISupplierService supplierService) : SupplierControllerBase
{
    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierFinancialViewer)]
    [HttpGet]
    public async Task<ActionResult<PagedResult<SupplierPaymentSummary>>> List(
        [FromQuery] SupplierPaymentQuery query, CancellationToken cancellationToken) =>
        SupplierResult(await supplierService.ListPaymentsAsync(query, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierFinancialViewer)]
    [HttpGet("{paymentId:guid}")]
    public async Task<ActionResult<SupplierPaymentDetails>> Get(Guid paymentId, CancellationToken cancellationToken) =>
        SupplierResult(await supplierService.GetPaymentAsync(paymentId, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierPaymentRecorder)]
    [HttpPost]
    public async Task<ActionResult<SupplierPaymentDetails>> Create(
        CreateSupplierPaymentRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (!SupplierRules.IsValidIdempotencyKey(idempotencyKey)) return InvalidSupplierIdempotencyKey();
        var result = await supplierService.CreatePaymentAsync(request, idempotencyKey, cancellationToken);
        return result.Status == AccessResultStatus.Success ? StatusCode(201, result.Value) : SupplierResult(result);
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierPaymentRecorder)]
    [HttpPost("{paymentId:guid}/confirm")]
    public async Task<ActionResult<SupplierPaymentDetails>> Confirm(
        Guid paymentId, ReviewSupplierPaymentRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (!SupplierRules.IsValidIdempotencyKey(idempotencyKey)) return InvalidSupplierIdempotencyKey();
        return SupplierResult(await supplierService.ConfirmPaymentAsync(
            paymentId, request, idempotencyKey, cancellationToken));
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierPaymentRecorder)]
    [HttpPost("{paymentId:guid}/reject")]
    public async Task<ActionResult<SupplierPaymentDetails>> Reject(
        Guid paymentId, ReviewSupplierPaymentRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (!SupplierRules.IsValidIdempotencyKey(idempotencyKey)) return InvalidSupplierIdempotencyKey();
        return SupplierResult(await supplierService.RejectPaymentAsync(
            paymentId, request, idempotencyKey, cancellationToken));
    }
}
