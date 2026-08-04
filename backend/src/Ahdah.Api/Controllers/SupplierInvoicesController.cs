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
[Route("api/v1/supplier-invoices")]
public sealed class SupplierInvoicesController(ISupplierService supplierService) : SupplierControllerBase
{
    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierViewer)]
    [HttpGet]
    public async Task<ActionResult<PagedResult<SupplierInvoiceSummary>>> List(
        [FromQuery] SupplierInvoiceQuery query, CancellationToken cancellationToken) =>
        SupplierResult(await supplierService.ListInvoicesAsync(query, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierViewer)]
    [HttpGet("{invoiceId:guid}")]
    public async Task<ActionResult<SupplierInvoiceDetails>> Get(Guid invoiceId, CancellationToken cancellationToken) =>
        SupplierResult(await supplierService.GetInvoiceAsync(invoiceId, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierInvoiceCreator)]
    [HttpPost]
    public async Task<ActionResult<SupplierInvoiceDetails>> Create(
        CreateSupplierInvoiceRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (!SupplierRules.IsValidIdempotencyKey(idempotencyKey)) return InvalidSupplierIdempotencyKey();
        var result = await supplierService.CreateInvoiceAsync(request, idempotencyKey, cancellationToken);
        return result.Status == AccessResultStatus.Success ? StatusCode(201, result.Value) : SupplierResult(result);
    }
}

[ApiController]
[Route("api/v1/supplier-debts")]
[Authorize(Policy = AhdahAuthorizationPolicies.SupplierViewer)]
public sealed class SupplierDebtsController(ISupplierService supplierService) : SupplierControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<SupplierInvoiceSummary>>> List(
        [FromQuery] SupplierInvoiceQuery query, CancellationToken cancellationToken) =>
        SupplierResult(await supplierService.ListInvoicesAsync(query, cancellationToken));

    [HttpGet("{debtId:guid}")]
    public async Task<ActionResult<SupplierInvoiceDetails>> Get(Guid debtId, CancellationToken cancellationToken) =>
        SupplierResult(await supplierService.GetInvoiceAsync(debtId, cancellationToken));
}
