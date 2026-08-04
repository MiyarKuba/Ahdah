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
[Route("api/v1/suppliers")]
public sealed class SuppliersController(ISupplierService supplierService) : SupplierControllerBase
{
    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierViewer)]
    [HttpGet]
    public async Task<ActionResult<PagedResult<SupplierSummary>>> List(
        [FromQuery] SupplierQuery query, CancellationToken cancellationToken) =>
        SupplierResult(await supplierService.ListSuppliersAsync(query, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierViewer)]
    [HttpGet("{supplierId:guid}")]
    public async Task<ActionResult<SupplierDetails>> Get(Guid supplierId, CancellationToken cancellationToken) =>
        SupplierResult(await supplierService.GetSupplierAsync(supplierId, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierManager)]
    [HttpPost]
    public async Task<ActionResult<SupplierDetails>> Create(
        CreateSupplierRequest request, CancellationToken cancellationToken)
    {
        var result = await supplierService.CreateSupplierAsync(request, cancellationToken);
        return result.Status == AccessResultStatus.Success ? StatusCode(201, result.Value) : SupplierResult(result);
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierManager)]
    [HttpPatch("{supplierId:guid}")]
    public async Task<ActionResult<SupplierDetails>> Update(
        Guid supplierId, UpdateSupplierRequest request, CancellationToken cancellationToken) =>
        SupplierResult(await supplierService.UpdateSupplierAsync(supplierId, request, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierFinancialViewer)]
    [HttpGet("{supplierId:guid}/payment-accounts")]
    public async Task<ActionResult<PagedResult<SupplierPaymentAccountSummary>>> ListAccounts(
        Guid supplierId, [FromQuery] SupplierSubresourceQuery query, CancellationToken cancellationToken) =>
        SupplierResult(await supplierService.ListPaymentAccountsAsync(supplierId, query, cancellationToken));

    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierPaymentRecorder)]
    [HttpPost("{supplierId:guid}/payment-accounts")]
    public async Task<ActionResult<SupplierPaymentAccountSummary>> CreateAccount(
        Guid supplierId, CreateSupplierPaymentAccountRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (!SupplierRules.IsValidIdempotencyKey(idempotencyKey)) return InvalidSupplierIdempotencyKey();
        var result = await supplierService.CreatePaymentAccountAsync(
            supplierId, request, idempotencyKey, cancellationToken);
        return result.Status == AccessResultStatus.Success ? StatusCode(201, result.Value) : SupplierResult(result);
    }

    [Authorize(Policy = AhdahAuthorizationPolicies.SupplierFinancialViewer)]
    [HttpGet("{supplierId:guid}/statement")]
    public async Task<ActionResult<SupplierStatement>> Statement(
        Guid supplierId, [FromQuery] SupplierStatementQuery query, CancellationToken cancellationToken) =>
        SupplierResult(await supplierService.GetStatementAsync(supplierId, query, cancellationToken));
}
