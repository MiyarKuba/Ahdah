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
[Route("api/v1/supplier-credit-notes")]
public sealed class SupplierCreditNotesController(ISupplierService supplierService) : SupplierControllerBase
{
    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierFinancialViewer)]
    [HttpGet]
    public async Task<ActionResult<PagedResult<SupplierCreditNoteSummary>>> List(
        [FromQuery] SupplierCreditNoteQuery query, CancellationToken cancellationToken) =>
        SupplierResult(await supplierService.ListCreditNotesAsync(query, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierFinancialViewer)]
    [HttpGet("{creditNoteId:guid}")]
    public async Task<ActionResult<SupplierCreditNoteDetails>> Get(
        Guid creditNoteId, CancellationToken cancellationToken) =>
        SupplierResult(await supplierService.GetCreditNoteAsync(creditNoteId, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierCreditManager)]
    [HttpPost]
    public async Task<ActionResult<SupplierCreditNoteDetails>> Create(
        CreateSupplierCreditNoteRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (!SupplierRules.IsValidIdempotencyKey(idempotencyKey)) return InvalidSupplierIdempotencyKey();
        var result = await supplierService.CreateCreditNoteAsync(request, idempotencyKey, cancellationToken);
        return result.Status == AccessResultStatus.Success ? StatusCode(201, result.Value) : SupplierResult(result);
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierCreditManager)]
    [HttpPost("{creditNoteId:guid}/approve")]
    public async Task<ActionResult<SupplierCreditNoteDetails>> Approve(
        Guid creditNoteId, ApproveSupplierCreditNoteRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (!SupplierRules.IsValidIdempotencyKey(idempotencyKey)) return InvalidSupplierIdempotencyKey();
        return SupplierResult(await supplierService.ApproveCreditNoteAsync(
            creditNoteId, request, idempotencyKey, cancellationToken));
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierCreditManager)]
    [HttpPost("{creditNoteId:guid}/allocations")]
    public async Task<ActionResult<SupplierCreditNoteDetails>> Allocate(
        Guid creditNoteId, ApplySupplierCreditNoteRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (!SupplierRules.IsValidIdempotencyKey(idempotencyKey)) return InvalidSupplierIdempotencyKey();
        return SupplierResult(await supplierService.ApplyCreditNoteAsync(
            creditNoteId, request, idempotencyKey, cancellationToken));
    }
}

[ApiController]
[Route("api/v1/supplier-refunds")]
[Authorize(Policy = AhdahAuthorizationPolicies.SupplierFinancialViewer)]
public sealed class SupplierRefundsController(ISupplierService supplierService) : SupplierControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<SupplierRefundSummary>>> List(
        [FromQuery] SupplierRefundQuery query, CancellationToken cancellationToken) =>
        SupplierResult(await supplierService.ListRefundsAsync(query, cancellationToken));
}
