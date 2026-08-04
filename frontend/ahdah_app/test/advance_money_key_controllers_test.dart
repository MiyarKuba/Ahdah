import 'dart:async';
import 'dart:math';

import 'package:ahdah_app/core/errors/app_exception.dart';
import 'package:ahdah_app/features/advances/domain/advance_models.dart';
import 'package:ahdah_app/features/advances/domain/advance_requests.dart';
import 'package:ahdah_app/features/advances/domain/decimal_money.dart';
import 'package:ahdah_app/features/advances/domain/financial_operation_key.dart';
import 'package:ahdah_app/features/advances/presentation/controllers/advance_controllers.dart';
import 'package:ahdah_app/features/company_members/domain/company_member_models.dart';
import 'package:flutter_test/flutter_test.dart';

import 'helpers/fakes.dart';

void main() {
  group('decimal money', () {
    test('canonicalizes exact accepted inputs without double arithmetic', () {
      expect(DecimalMoney.canonicalize('0.01'), '0.01');
      expect(DecimalMoney.canonicalize('1'), '1.00');
      expect(DecimalMoney.canonicalize('1.2'), '1.20');
      expect(DecimalMoney.canonicalize('1.20'), '1.20');
      expect(
        DecimalMoney.canonicalize('9999999999999999.99'),
        '9999999999999999.99',
      );
      expect(DecimalMoney.canonicalize('١٫٢٠'), '1.20');
    });

    test(
      'rejects zero, negative, excessive precision, and malformed separators',
      () {
        expect(DecimalMoney.canonicalize('0'), isNull);
        expect(DecimalMoney.canonicalize('-1'), isNull);
        expect(DecimalMoney.canonicalize('1.001'), isNull);
        expect(DecimalMoney.canonicalize('1,20'), isNull);
        expect(DecimalMoney.canonicalize('1..2'), isNull);
      },
    );

    test('exact minor-unit sum detects a one-cent mismatch', () {
      expect(DecimalMoney.sumEquals(['50.00', '50.00'], '100.00'), isTrue);
      expect(DecimalMoney.sumEquals(['50.00', '49.99'], '100.00'), isFalse);
    });
  });

  group('financial operation keys', () {
    test(
      'secure factory creates distinct URL-safe keys of suitable length',
      () {
        final factory = SecureFinancialOperationKeyFactory(random: Random(42));
        final first = factory.create();
        final second = factory.create();

        expect(first.length, inInclusiveRange(32, 200));
        expect(first, matches(RegExp(r'^[A-Za-z0-9_-]+$')));
        expect(second, isNot(first));
        expect(first, isNot(matches(RegExp(r'^\d{10,}$'))));
      },
    );

    test('session reuses exact payload and invalidates on change or clear', () {
      final factory = _SequenceKeyFactory();
      final session = FinancialOperationKeySession(factory);
      final first = session.keyFor('payload-a');

      expect(session.keyFor('payload-a'), first);
      expect(session.keyFor('payload-b'), isNot(first));
      session.clear();
      expect(session.hasPendingOperation, isFalse);
      expect(session.keyFor('payload-b'), isNot(first));
    });
  });

  group('advance controllers', () {
    test(
      'list loads, applies exact filter, resets page, and deduplicates',
      () async {
        final repository = FakeAdvanceRepository()
          ..advancePage = AdvancePage(
            items: [testAdvanceSummary],
            page: 1,
            pageSize: 20,
            totalCount: 2,
            totalPages: 2,
          );
        final controller = AdvanceListController(
          repository,
          onUnauthorized: () async {},
        );

        await controller.load();
        expect(controller.state.items, hasLength(1));
        await controller.setStatus('Open');
        expect(repository.lastStatus, 'Open');
        expect(repository.lastPage, 1);

        repository.advancePage = AdvancePage(
          items: [testAdvanceSummary],
          page: 2,
          pageSize: 20,
          totalCount: 2,
          totalPages: 2,
        );
        await controller.loadMore();
        expect(controller.state.items, hasLength(1));
        expect(controller.state.page, 2);
      },
    );

    test('load-more failure preserves existing financial records', () async {
      final repository = FakeAdvanceRepository()
        ..advancePage = AdvancePage(
          items: [testAdvanceSummary],
          page: 1,
          pageSize: 20,
          totalCount: 2,
          totalPages: 2,
        );
      final controller = AdvanceListController(
        repository,
        onUnauthorized: () async {},
      );
      await controller.load();
      repository.readError = const AppException(AppExceptionKind.network);

      await controller.loadMore();

      expect(controller.state.items.single.advanceId, 'advance-1');
      expect(controller.state.loadMoreError?.kind, AppExceptionKind.network);
    });

    test(
      'recipient lookup uses bounded server search and pagination',
      () async {
        final members = FakeCompanyMemberRepository()
          ..memberPage = const CompanyMemberPage(
            items: [
              CompanyMemberSummary(
                id: 'deputy-1',
                fullName: 'Deputy One',
                role: 'Deputy',
                status: 'Active',
                identityVerificationStatus: 'Verified',
              ),
            ],
            page: 1,
            pageSize: 20,
            totalCount: 2,
            totalPages: 2,
          );
        final controller = AdvanceRecipientController(
          members,
          'Manager',
          onUnauthorized: () async {},
        );
        await controller.load();
        expect(members.lastPageSize, 20);
        expect(members.lastRole, 'Deputy');
        expect(members.lastStatus, 'Active');

        controller.setSearch('Dep', debounce: Duration.zero);
        await Future<void>.delayed(Duration.zero);
        await Future<void>.delayed(Duration.zero);
        expect(members.lastSearch, 'Dep');

        members.memberPage = const CompanyMemberPage(
          items: [
            CompanyMemberSummary(
              id: 'deputy-2',
              fullName: 'Deputy Two',
              role: 'Deputy',
              status: 'Active',
              identityVerificationStatus: 'Verified',
            ),
          ],
          page: 2,
          pageSize: 20,
          totalCount: 2,
          totalPages: 2,
        );
        await controller.loadMore();
        expect(controller.state.items, hasLength(2));
        expect(members.lastPage, 2);
      },
    );

    test('duplicate create submissions are suppressed', () async {
      final repository = FakeAdvanceRepository();
      final completer = Completer<AdvanceDetails>();
      repository.createCompleter = completer;
      final controller = AdvanceCreateController(
        repository,
        _SequenceKeyFactory(),
        onUnauthorized: () async {},
      );
      final input = _createInput('100.00');

      final first = controller.create(input);
      final duplicate = await controller.create(input);
      expect(duplicate, isNull);
      expect(repository.createCalls, 1);
      completer.complete(testAdvanceDetails);
      expect(await first, testAdvanceDetails);
    });

    test(
      'ambiguous timeout preserves exact payload and key for explicit retry',
      () async {
        final repository = FakeAdvanceRepository()
          ..writeError = const AppException(AppExceptionKind.timeout);
        final factory = _SequenceKeyFactory();
        final controller = AdvanceCreateController(
          repository,
          factory,
          onUnauthorized: () async {},
        );

        expect(await controller.create(_createInput('100.00')), isNull);
        final firstKey = repository.lastIdempotencyKey;
        expect(
          controller.state.phase,
          FinancialCommandPhase.uncertainSubmission,
        );

        repository.writeError = null;
        expect(await controller.retrySameOperation(), testAdvanceDetails);
        expect(repository.lastIdempotencyKey, firstKey);
        expect(repository.createCalls, 2);
      },
    );

    test(
      'definitive 400 clears pending key and later operation gets a new key',
      () async {
        final repository = FakeAdvanceRepository()
          ..writeError = const AppException(AppExceptionKind.validation);
        final controller = AdvanceCreateController(
          repository,
          _SequenceKeyFactory(),
          onUnauthorized: () async {},
        );
        await controller.create(_createInput('100.00'));
        final firstKey = repository.lastIdempotencyKey;
        expect(controller.state.phase, FinancialCommandPhase.failure);

        repository.writeError = null;
        await controller.create(_createInput('100.00'));
        expect(repository.lastIdempotencyKey, isNot(firstKey));
      },
    );

    test('cancel clears uncertain local retry state', () async {
      final repository = FakeAdvanceRepository()
        ..writeError = const AppException(AppExceptionKind.network);
      final controller = AdvanceConfirmationController(
        repository,
        _SequenceKeyFactory(),
        onUnauthorized: () async {},
      );
      await controller.confirm('transfer-1');
      expect(controller.state.isUncertain, isTrue);

      controller.cancelPending();

      expect(controller.state.phase, FinancialCommandPhase.idle);
      expect(await controller.retrySameOperation(), isNull);
    });
  });
}

CreateAdvanceInput _createInput(String amount) => CreateAdvanceInput(
  recipientUserId: 'user-1',
  issueDate: DateTime(2026, 8, 4),
  purpose: 'Custody',
  fundings: [
    FundingAllocationInput(fundingSourceId: 'source-1', amount: amount),
  ],
  transfer: AdvanceTransferFields(
    amount: amount,
    transferDate: DateTime(2026, 8, 4),
    transferMethod: 'Cash',
  ),
);

final class _SequenceKeyFactory implements FinancialOperationKeyFactory {
  var _next = 0;

  @override
  String create() => 'test-secure-operation-key-${++_next}-xxxxxxxx';
}
