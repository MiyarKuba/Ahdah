import 'access_models.dart';

abstract interface class AccessRepository {
  Future<AccessPage<Invitation>> listInvitations({
    required int page,
    required int pageSize,
    String? status,
  });

  Future<CreatedInvitation> createInvitation(CreateInvitationInput input);

  Future<Invitation> cancelInvitation(String invitationId);

  Future<AccessPage<JoinRequest>> listJoinRequests({
    required int page,
    required int pageSize,
    String? status,
  });

  Future<JoinRequestDecision> approveJoinRequest(
    String joinRequestId,
    ApproveJoinRequestInput input,
  );

  Future<JoinRequestDecision> rejectJoinRequest(
    String joinRequestId,
    RejectJoinRequestInput input,
  );
}
