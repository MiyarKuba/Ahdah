import 'package:ahdah_app/core/network/api_client.dart';
import 'package:ahdah_app/core/network/auth_interceptor.dart';
import 'package:ahdah_app/features/access/domain/access_models.dart';
import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';

import 'helpers/fakes.dart';

const _invitationJson = {
  'invitationId': 'invitation-1',
  'invitedPhoneNumber': '+218912345678',
  'assignedRole': 'Worker',
  'status': 'Pending',
  'expiresAtUtc': '2026-08-08T12:00:00Z',
  'acceptedAtUtc': null,
  'cancelledAtUtc': null,
  'createdAtUtc': '2026-08-01T12:00:00Z',
  'updatedAtUtc': '2026-08-01T12:00:00Z',
};

const _joinRequestJson = {
  'joinRequestId': 'request-1',
  'userId': 'internal-user-not-mapped',
  'fullName': 'Applicant',
  'phoneNumber': '+218912345679',
  'email': 'applicant@example.com',
  'requestedRole': 'Worker',
  'assignedRole': null,
  'status': 'Pending',
  'requestMessage': 'Please add me',
  'reviewNotes': null,
  'rejectionReason': null,
  'requestedAtUtc': '2026-08-01T10:00:00Z',
  'reviewedAtUtc': null,
  'cancelledAtUtc': null,
  'createdAtUtc': '2026-08-01T10:00:00Z',
  'updatedAtUtc': '2026-08-01T10:00:00Z',
};

void main() {
  test('invitation page parses exact pagination and safe list fields', () {
    final page = AccessPage.fromJson(const {
      'items': [_invitationJson],
      'page': 1,
      'pageSize': 20,
      'totalCount': 21,
      'totalPages': 2,
    }, Invitation.fromJson);

    expect(page.items.single.invitedPhoneNumber, '+218912345678');
    expect(page.items.single.expiresAtUtc, DateTime.utc(2026, 8, 8, 12));
    expect(page.hasMore, isTrue);
    expect(page.totalCount, 21);
    expect(page.items.single.toString(), isNot(contains('token')));
  });

  test('created invitation parses token but redacts diagnostics', () {
    final result = CreatedInvitation.fromJson(const {
      'invitation': _invitationJson,
      'token': 'raw-one-time-secret',
    });

    expect(result.token, 'raw-one-time-secret');
    expect(result.toString(), isNot(contains('raw-one-time-secret')));
  });

  test('join request parser ignores internal user id and handles UTC', () {
    final request = JoinRequest.fromJson(_joinRequestJson);

    expect(request.fullName, 'Applicant');
    expect(request.requestedAtUtc, DateTime.utc(2026, 8, 1, 10));
    expect(request.isPending, isTrue);
  });

  test('decision result parses both approval and rejection fields', () {
    final request = Map<String, Object?>.of(_joinRequestJson)
      ..['status'] = 'Approved';
    final result = JoinRequestDecision.fromJson({
      'outcome': 'ApprovedAndActivated',
      'userStatus': 'Active',
      'request': request,
    });

    expect(result.outcome, 'ApprovedAndActivated');
    expect(result.request.status, 'Approved');
  });

  test('unknown role/status and malformed optional dates do not crash', () {
    final json = Map<String, Object?>.of(_invitationJson)
      ..['assignedRole'] = 'FutureRole'
      ..['status'] = 'FutureStatus'
      ..['expiresAtUtc'] = 'not-a-date';
    final invitation = Invitation.fromJson(json);

    expect(invitation.assignedRole, 'FutureRole');
    expect(invitation.status, 'FutureStatus');
    expect(invitation.expiresAtUtc, isNull);
  });

  test(
    'invitation list sends exact page/filter and authenticated header',
    () async {
      final adapter = RecordingAdapter(
        body: const {
          'items': [],
          'page': 2,
          'pageSize': 20,
          'totalCount': 0,
          'totalPages': 0,
        },
      );
      final client = _client(adapter);

      await client.listInvitations(page: 2, pageSize: 20, status: 'Pending');

      expect(adapter.lastRequest!.path, '/api/v1/invitations');
      expect(adapter.lastRequest!.queryParameters, {
        'page': 2,
        'pageSize': 20,
        'status': 'Pending',
      });
      expect(
        adapter.lastRequest!.headers['Authorization'],
        'Bearer test-token',
      );
    },
  );

  test('invitation creation sends phone and assigned role only', () async {
    final adapter = RecordingAdapter(
      statusCode: 201,
      body: const {'invitation': _invitationJson, 'token': 'secret'},
    );
    final client = _client(adapter);

    await client.createInvitation(
      const CreateInvitationInput(
        phoneNumber: '+218912345678',
        assignedRole: 'Worker',
      ),
    );

    expect(adapter.lastRequest!.data, {
      'phoneNumber': '+218912345678',
      'assignedRole': 'Worker',
    });
    expect(adapter.lastRequest!.data, isNot(contains('company_id')));
    expect(adapter.lastRequest!.data, isNot(contains('companyId')));
  });

  test('cancellation and join decisions use exact routes and bodies', () async {
    final cancelAdapter = RecordingAdapter(body: _invitationJson);
    await _client(cancelAdapter).cancelInvitation('invitation-1');
    expect(
      cancelAdapter.lastRequest!.path,
      '/api/v1/invitations/invitation-1/cancel',
    );
    expect(cancelAdapter.lastRequest!.data, isNull);

    final approvedRequest = Map<String, Object?>.of(_joinRequestJson)
      ..['status'] = 'Approved';
    final approvalAdapter = RecordingAdapter(
      body: {
        'outcome': 'ApprovedAndActivated',
        'userStatus': 'Active',
        'request': approvedRequest,
      },
    );
    await _client(approvalAdapter).approveJoinRequest(
      'request-1',
      const ApproveJoinRequestInput(
        assignedRole: 'Supervisor',
        reviewNotes: 'Reviewed',
      ),
    );
    expect(
      approvalAdapter.lastRequest!.path,
      '/api/v1/join-requests/request-1/approve',
    );
    expect(approvalAdapter.lastRequest!.data, {
      'assignedRole': 'Supervisor',
      'reviewNotes': 'Reviewed',
    });

    final rejectedRequest = Map<String, Object?>.of(_joinRequestJson)
      ..['status'] = 'Rejected'
      ..['rejectionReason'] = 'Not eligible';
    final rejectionAdapter = RecordingAdapter(
      body: {
        'outcome': 'Rejected',
        'userStatus': 'Rejected',
        'request': rejectedRequest,
      },
    );
    await _client(rejectionAdapter).rejectJoinRequest(
      'request-1',
      const RejectJoinRequestInput(reason: 'Not eligible', reviewNotes: null),
    );
    expect(rejectionAdapter.lastRequest!.data, {
      'reason': 'Not eligible',
      'reviewNotes': null,
    });
    expect(rejectionAdapter.lastRequest!.data, isNot(contains('company_id')));
  });
}

ApiClient _client(RecordingAdapter adapter) {
  final dio = Dio(BaseOptions(baseUrl: 'http://localhost:5231'))
    ..httpClientAdapter = adapter
    ..interceptors.add(AuthInterceptor(FakeAccessTokenStore('test-token')));
  return ApiClient(dio);
}
