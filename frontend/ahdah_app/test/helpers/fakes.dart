import 'dart:async';
import 'dart:convert';
import 'dart:typed_data';

import 'package:ahdah_app/core/errors/app_exception.dart';
import 'package:ahdah_app/core/security/access_token_store.dart';
import 'package:ahdah_app/features/authentication/data/auth_repository.dart';
import 'package:ahdah_app/features/authentication/domain/identity_models.dart';
import 'package:ahdah_app/features/authentication/domain/identity_requests.dart';
import 'package:dio/dio.dart';

const testUser = AuthenticatedUser(
  userId: '2e77a184-b59e-4e99-a276-37582a14b669',
  fullName: 'Test User',
  role: 'Worker',
  status: 'Active',
);
const testCompany = CompanySummary(
  companyId: 'f29b18bb-9954-46c5-ad07-bd48db68bb55',
  companyName: 'Test Company',
  companyCode: 'TEST01',
  status: 'Active',
);
const testCurrentSession = CurrentSessionResult(
  user: testUser,
  company: testCompany,
  role: 'Worker',
);
final testAuthentication = AuthenticationResult(
  accessToken: 'test-token-never-logged',
  tokenType: 'Bearer',
  accessTokenExpiresAtUtc: DateTime.utc(2026, 7, 28, 12, 15),
  user: testUser,
  company: testCompany,
  role: 'Worker',
);

final class FakeAccessTokenStore implements AccessTokenStore {
  FakeAccessTokenStore([this.token]);
  String? token;
  int clearCalls = 0;

  @override
  Future<String?> read() async => token;
  @override
  Future<void> write(String token) async => this.token = token;
  @override
  Future<void> clear() async {
    clearCalls++;
    token = null;
  }
}

final class FakeAuthRepository implements AuthRepository {
  CurrentSessionResult current = testCurrentSession;
  AppException? currentError;
  Completer<CurrentSessionResult>? currentCompleter;
  Completer<AuthenticationResult>? loginCompleter;
  int loginCalls = 0;
  int currentCalls = 0;
  InvitationAcceptanceResult invitationResult =
      const InvitationAcceptanceResult(
        outcome: 'AcceptedPendingIdentityVerification',
        userStatus: 'PendingApproval',
        assignedRole: 'Accountant',
      );

  @override
  Future<AuthenticationResult> login(LoginRequest request) {
    loginCalls++;
    return loginCompleter?.future ?? Future.value(testAuthentication);
  }

  @override
  Future<RegisterCompanyResult> registerCompany(
    RegisterCompanyRequest request,
  ) async => RegisterCompanyResult(
    company: testCompany,
    manager: testUser,
    authentication: testAuthentication,
  );

  @override
  Future<CurrentSessionResult> currentSession() async {
    currentCalls++;
    if (currentError != null) throw currentError!;
    return currentCompleter?.future ?? current;
  }

  @override
  Future<InvitationAcceptanceResult> acceptInvitation(
    AcceptInvitationRequest request,
  ) async => invitationResult;

  @override
  Future<JoinRequestAcknowledgement> submitJoinRequest(
    SubmitJoinRequest request,
  ) async => const JoinRequestAcknowledgement(status: 'Pending');

  @override
  Future<bool> health() async => true;
}

final class RecordingAdapter implements HttpClientAdapter {
  RecordingAdapter({this.statusCode = 200, this.body = const {}});
  final int statusCode;
  final Map<String, Object?> body;
  RequestOptions? lastRequest;

  @override
  Future<ResponseBody> fetch(
    RequestOptions options,
    Stream<Uint8List>? requestStream,
    Future<void>? cancelFuture,
  ) async {
    lastRequest = options;
    return ResponseBody.fromString(
      jsonEncode(body),
      statusCode,
      headers: {
        Headers.contentTypeHeader: [Headers.jsonContentType],
      },
    );
  }

  @override
  void close({bool force = false}) {}
}
