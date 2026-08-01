import '../../../core/network/api_client.dart';
import '../domain/identity_models.dart';
import '../domain/identity_requests.dart';

abstract interface class AuthRepository {
  Future<AuthenticationResult> login(LoginRequest request);
  Future<RegisterCompanyResult> registerCompany(RegisterCompanyRequest request);
  Future<CurrentSessionResult> currentSession();
  Future<InvitationAcceptanceResult> acceptInvitation(
    AcceptInvitationRequest request,
  );
  Future<JoinRequestAcknowledgement> submitJoinRequest(
    SubmitJoinRequest request,
  );
  Future<bool> health();
}

final class ApiAuthRepository implements AuthRepository {
  const ApiAuthRepository(this._client);
  final ApiClient _client;

  @override
  Future<AuthenticationResult> login(LoginRequest request) =>
      _client.login(request);
  @override
  Future<RegisterCompanyResult> registerCompany(
    RegisterCompanyRequest request,
  ) => _client.registerCompany(request);
  @override
  Future<CurrentSessionResult> currentSession() => _client.currentSession();
  @override
  Future<InvitationAcceptanceResult> acceptInvitation(
    AcceptInvitationRequest request,
  ) => _client.acceptInvitation(request);
  @override
  Future<JoinRequestAcknowledgement> submitJoinRequest(
    SubmitJoinRequest request,
  ) => _client.submitJoinRequest(request);
  @override
  Future<bool> health() => _client.health();
}
