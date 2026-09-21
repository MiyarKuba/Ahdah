using Ahdah.Application.Access.Models;

namespace Ahdah.Application.Custody;

public interface ICustodyService
{
    Task<AccessResult<BalanceCustodyDetails>> GetBalanceAsync(Guid balanceId, CancellationToken cancellationToken);
    Task<AccessResult<ProjectCustodyDetails>> GetProjectAsync(Guid projectId, CancellationToken cancellationToken);
    Task<AccessResult<CustodyOperationDetails>> RequestDesignationAsync(
        CreateDesignationRequest request, string idempotencyKey, CancellationToken cancellationToken);
    Task<AccessResult<CustodyOperationDetails>> ApproveDesignationAsync(
        Guid operationId, ApproveDesignationRequest request, string idempotencyKey, CancellationToken cancellationToken);
}
