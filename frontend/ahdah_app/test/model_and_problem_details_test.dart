import 'package:ahdah_app/core/errors/problem_details.dart';
import 'package:ahdah_app/features/authentication/domain/identity_models.dart';
import 'package:flutter_test/flutter_test.dart';

const _user = {
  'userId': '2e77a184-b59e-4e99-a276-37582a14b669',
  'fullName': 'Test User',
  'role': 'Worker',
  'status': 'Active',
  'identityVerificationStatus': 'NotRequired',
};
const _company = {
  'companyId': 'f29b18bb-9954-46c5-ad07-bd48db68bb55',
  'companyName': 'Test Company',
  'companyCode': 'TEST01',
  'status': 'Active',
};
const _authentication = {
  'accessToken': 'secret-access-token-value',
  'tokenType': 'Bearer',
  'accessTokenExpiresAtUtc': '2026-07-28T12:15:00Z',
  'user': _user,
  'company': _company,
  'role': 'Worker',
};

void main() {
  test('authentication response parses without token diagnostics', () {
    final result = AuthenticationResult.fromJson(_authentication);
    expect(result.user.fullName, 'Test User');
    expect(result.company.companyCode, 'TEST01');
    expect(result.toString(), isNot(contains('secret-access-token-value')));
  });

  test('current user response parses exact API shape', () {
    final result = CurrentSessionResult.fromJson({
      'user': _user,
      'company': _company,
      'role': 'Worker',
    });
    expect(result.role, 'Worker');
    expect(result.company.companyName, 'Test Company');
  });

  test('pending invitation result parses nullable authentication', () {
    final result = InvitationAcceptanceResult.fromJson(const {
      'outcome': 'AcceptedPendingIdentityVerification',
      'userStatus': 'PendingApproval',
      'assignedRole': 'Accountant',
      'authentication': null,
    });
    expect(result.authentication, isNull);
    expect(result.requiresIdentityVerification, isTrue);
  });

  test('join request acknowledgement parses', () {
    final result = JoinRequestAcknowledgement.fromJson(const {
      'status': 'Pending',
    });
    expect(result.status, 'Pending');
  });

  test('ProblemDetails and validation errors parse safely', () {
    final problem = ProblemDetails.fromJson(const {
      'title': 'Validation failed',
      'status': 400,
      'code': 'validation.failed',
      'traceId': 'safe-correlation-id',
      'errors': {
        'PhoneNumber': ['Invalid phone.'],
      },
    });
    expect(problem.status, 400);
    expect(problem.code, 'validation.failed');
    expect(problem.fieldErrors.firstFor('phoneNumber'), 'Invalid phone.');
    expect(problem.toString(), isNot(contains('Invalid phone.')));
  });

  test('unknown and malformed ProblemDetails values do not crash', () {
    expect(ProblemDetails.fromJson('not-json').status, isNull);
    expect(ProblemDetails.fromJson(const {'status': '409'}).status, 409);
  });
}
