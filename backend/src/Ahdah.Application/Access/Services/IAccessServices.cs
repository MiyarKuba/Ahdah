using Ahdah.Application.Access.Contracts;
using Ahdah.Application.Access.Models;

namespace Ahdah.Application.Access.Services;

public interface IInvitationService
{
    Task<AccessResult<CreateInvitationResult>> CreateAsync(
        CreateInvitationRequest request,
        CancellationToken cancellationToken);

    Task<AccessResult<PagedResult<InvitationSummary>>> ListAsync(
        InvitationQuery query,
        CancellationToken cancellationToken);

    Task<AccessResult<InvitationAcceptanceResult>> AcceptAsync(
        AcceptInvitationRequest request,
        CancellationToken cancellationToken);

    Task<AccessResult<InvitationSummary>> CancelAsync(
        Guid invitationId,
        CancellationToken cancellationToken);
}

public interface IJoinRequestService
{
    Task<AccessResult<JoinRequestSubmissionResult>> SubmitAsync(
        SubmitJoinRequestRequest request,
        CancellationToken cancellationToken);

    Task<AccessResult<PagedResult<JoinRequestSummary>>> ListAsync(
        JoinRequestQuery query,
        CancellationToken cancellationToken);

    Task<AccessResult<JoinRequestDecisionResult>> ApproveAsync(
        Guid joinRequestId,
        ApproveJoinRequestRequest request,
        CancellationToken cancellationToken);

    Task<AccessResult<JoinRequestDecisionResult>> RejectAsync(
        Guid joinRequestId,
        RejectJoinRequestRequest request,
        CancellationToken cancellationToken);
}
