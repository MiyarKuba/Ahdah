using Ahdah.Application.Access.Models;
using Ahdah.Application.Advances.Contracts;
using Ahdah.Application.Advances.Models;

namespace Ahdah.Application.Advances.Services;

public interface IAdvanceService
{
    Task<AccessResult<PagedResult<AdvanceSummary>>> ListAsync(AdvanceQuery query, CancellationToken cancellationToken);
    Task<AccessResult<AdvanceDetails>> GetAsync(Guid advanceId, CancellationToken cancellationToken);
    Task<AccessResult<PagedResult<AdvanceMovementSummary>>> ListMovementsAsync(Guid advanceId, AdvanceMovementQuery query, CancellationToken cancellationToken);
    Task<AccessResult<AdvanceDetails>> CreateAsync(CreateAdvanceRequest request, string? idempotencyKey, CancellationToken cancellationToken);
    Task<AccessResult<AdvanceTransferDetails>> DistributeAsync(Guid advanceId, CreateAdvanceDistributionRequest request, string? idempotencyKey, CancellationToken cancellationToken);
    Task<AccessResult<AdvanceTransferDetails>> ReturnAsync(Guid advanceId, CreateAdvanceReturnRequest request, string? idempotencyKey, CancellationToken cancellationToken);
    Task<AccessResult<AdvanceTransferDetails>> ConfirmTransferAsync(Guid transferId, string? idempotencyKey, CancellationToken cancellationToken);
    Task<AccessResult<AdvanceTransferDetails>> RejectTransferAsync(Guid transferId, RejectAdvanceTransferRequest request, string? idempotencyKey, CancellationToken cancellationToken);
    Task<AccessResult<AdvanceBalancePage>> GetMyBalancesAsync(AdvanceBalanceQuery query, CancellationToken cancellationToken);
    Task<AccessResult<AdvanceBalancePage>> GetUserBalancesAsync(Guid userId, AdvanceBalanceQuery query, CancellationToken cancellationToken);
    Task<AccessResult<PagedResult<AvailableFundingSourceSummary>>> ListAvailableFundingSourcesAsync(AvailableFundingSourceQuery query, CancellationToken cancellationToken);
}
