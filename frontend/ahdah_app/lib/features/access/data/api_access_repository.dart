import '../../../core/network/api_client.dart';
import '../domain/access_models.dart';
import '../domain/access_repository.dart';

final class ApiAccessRepository implements AccessRepository {
  const ApiAccessRepository(this._client);

  final ApiClient _client;

  @override
  Future<AccessPage<Invitation>> listInvitations({
    required int page,
    required int pageSize,
    String? status,
  }) => _client.listInvitations(page: page, pageSize: pageSize, status: status);

  @override
  Future<CreatedInvitation> createInvitation(CreateInvitationInput input) =>
      _client.createInvitation(input);

  @override
  Future<Invitation> cancelInvitation(String invitationId) =>
      _client.cancelInvitation(invitationId);

  @override
  Future<AccessPage<JoinRequest>> listJoinRequests({
    required int page,
    required int pageSize,
    String? status,
  }) =>
      _client.listJoinRequests(page: page, pageSize: pageSize, status: status);

  @override
  Future<JoinRequestDecision> approveJoinRequest(
    String joinRequestId,
    ApproveJoinRequestInput input,
  ) => _client.approveJoinRequest(joinRequestId, input);

  @override
  Future<JoinRequestDecision> rejectJoinRequest(
    String joinRequestId,
    RejectJoinRequestInput input,
  ) => _client.rejectJoinRequest(joinRequestId, input);
}
