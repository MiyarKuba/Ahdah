import 'dart:async';

import 'package:ahdah_app/core/errors/app_exception.dart';
import 'package:ahdah_app/features/access/domain/access_list_state.dart';
import 'package:ahdah_app/features/access/domain/access_models.dart';
import 'package:ahdah_app/features/access/presentation/controllers/access_list_controllers.dart';
import 'package:ahdah_app/features/access/presentation/controllers/access_write_controllers.dart';
import 'package:flutter_test/flutter_test.dart';

import 'helpers/fakes.dart';

const _firstInvitation = Invitation(
  id: 'invitation-1',
  invitedPhoneNumber: '+218912345678',
  assignedRole: 'Worker',
  status: 'Pending',
);
const _secondInvitation = Invitation(
  id: 'invitation-2',
  invitedPhoneNumber: '+218912345679',
  assignedRole: 'Supervisor',
  status: 'Pending',
);
const _joinRequest = JoinRequest(
  id: 'request-1',
  fullName: 'Applicant',
  phoneNumber: '+218912345680',
  requestedRole: 'Worker',
  status: 'Pending',
);

void main() {
  test('invitation initial load and refresh preserve filter', () async {
    final repository = FakeAccessRepository()
      ..invitationPage = const AccessPage(
        items: [_firstInvitation],
        page: 1,
        pageSize: 20,
        totalCount: 1,
        totalPages: 1,
      );
    final controller = InvitationListController(
      repository,
      onUnauthorized: _noOp,
    );

    await controller.setFilter('Pending');
    await controller.refresh();

    expect(controller.state.phase, AccessListPhase.data);
    expect(controller.state.items, [_firstInvitation]);
    expect(controller.state.filter, 'Pending');
    expect(repository.invitationListCalls, 2);
    expect(repository.lastPage, 1);
  });

  test('invitation pagination appends and deduplicates stable ids', () async {
    final repository = FakeAccessRepository()
      ..invitationPage = const AccessPage(
        items: [_firstInvitation],
        page: 1,
        pageSize: 20,
        totalCount: 2,
        totalPages: 2,
      );
    final controller = InvitationListController(
      repository,
      onUnauthorized: _noOp,
    );
    await controller.load();
    repository.invitationPage = const AccessPage(
      items: [_firstInvitation, _secondInvitation],
      page: 2,
      pageSize: 20,
      totalCount: 2,
      totalPages: 2,
    );

    await controller.loadMore();

    expect(controller.state.items, [_firstInvitation, _secondInvitation]);
    expect(controller.state.page, 2);
    expect(controller.state.hasMore, isFalse);
  });

  test('status filter resets pagination to page one', () async {
    final repository = FakeAccessRepository()
      ..invitationPage = const AccessPage(
        items: [_firstInvitation],
        page: 1,
        pageSize: 20,
        totalCount: 1,
        totalPages: 1,
      );
    final controller = InvitationListController(
      repository,
      onUnauthorized: _noOp,
    );

    await controller.setFilter('Cancelled');

    expect(repository.lastPage, 1);
    expect(repository.lastFilter, 'Cancelled');
    expect(controller.state.filter, 'Cancelled');
  });

  test('network list failure stays retryable', () async {
    final repository = FakeAccessRepository()
      ..invitationListError = const AppException(AppExceptionKind.network);
    final controller = InvitationListController(
      repository,
      onUnauthorized: _noOp,
    );
    await controller.load();
    expect(controller.state.phase, AccessListPhase.failure);

    repository.invitationListError = null;
    await controller.load();
    expect(controller.state.phase, AccessListPhase.empty);
  });

  test('401 list failure invokes centralized session expiry', () async {
    var expired = false;
    final repository = FakeAccessRepository()
      ..invitationListError = const AppException(AppExceptionKind.unauthorized);
    final controller = InvitationListController(
      repository,
      onUnauthorized: () async => expired = true,
    );

    await controller.load();

    expect(expired, isTrue);
  });

  test('join list load and pagination use exact filter', () async {
    final repository = FakeAccessRepository()
      ..joinRequestPage = const AccessPage(
        items: [_joinRequest],
        page: 1,
        pageSize: 20,
        totalCount: 1,
        totalPages: 1,
      );
    final controller = JoinRequestListController(
      repository,
      onUnauthorized: _noOp,
    );

    await controller.setFilter('Pending');

    expect(controller.state.items, [_joinRequest]);
    expect(repository.lastFilter, 'Pending');
    expect(repository.lastPageSize, 20);
  });

  test(
    'invitation creation prevents duplicate submit and stores no token',
    () async {
      final repository = FakeAccessRepository()
        ..createCompleter = Completer<CreatedInvitation>();
      final controller = InvitationCreationController(
        repository,
        onUnauthorized: _noOp,
      );
      const input = CreateInvitationInput(
        phoneNumber: '+218912345678',
        assignedRole: 'Worker',
      );

      final first = controller.create(input);
      final duplicate = await controller.create(input);
      expect(repository.createCalls, 1);
      expect(duplicate, isNull);
      expect(controller.state.phase, AccessSubmitPhase.submitting);

      repository.createCompleter!.complete(
        const CreatedInvitation(
          invitation: _firstInvitation,
          token: 'ephemeral-raw-token',
        ),
      );
      final result = await first;
      expect(result!.token, 'ephemeral-raw-token');
      expect(controller.state.phase, AccessSubmitPhase.success);
      expect(
        controller.state.toString(),
        isNot(contains('ephemeral-raw-token')),
      );
    },
  );

  test(
    'cancellation, approval, and rejection expose server outcomes',
    () async {
      final repository = FakeAccessRepository();
      final cancellation = InvitationCancellationController(
        repository,
        onUnauthorized: _noOp,
      );
      final approval = JoinApprovalController(
        repository,
        onUnauthorized: _noOp,
      );
      final rejection = JoinRejectionController(
        repository,
        onUnauthorized: _noOp,
      );

      expect((await cancellation.cancel('invitation-1'))!.status, 'Cancelled');
      final approved = await approval.approve(
        'request-1',
        const ApproveJoinRequestInput(assignedRole: 'Accountant'),
      );
      expect(approved!.outcome, 'ApprovedPendingIdentityVerification');
      final rejected = await rejection.reject(
        'request-1',
        const RejectJoinRequestInput(reason: 'Not eligible'),
      );
      expect(rejected!.outcome, 'Rejected');
    },
  );

  test('409 write failure leaves controller recoverable', () async {
    final repository = FakeAccessRepository()
      ..writeError = const AppException(AppExceptionKind.conflict);
    final controller = JoinApprovalController(
      repository,
      onUnauthorized: _noOp,
    );

    final failed = await controller.approve(
      'request-1',
      const ApproveJoinRequestInput(assignedRole: 'Worker'),
    );
    expect(failed, isNull);
    expect(controller.state.phase, AccessSubmitPhase.failure);

    repository.writeError = null;
    final retried = await controller.approve(
      'request-1',
      const ApproveJoinRequestInput(assignedRole: 'Worker'),
    );
    expect(retried, isNotNull);
    expect(controller.state.phase, AccessSubmitPhase.success);
  });
}

Future<void> _noOp() async {}
