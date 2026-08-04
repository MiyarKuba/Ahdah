import 'dart:async';

import 'package:ahdah_app/core/errors/app_exception.dart';
import 'package:ahdah_app/features/access/domain/role_capabilities.dart';
import 'package:ahdah_app/features/advances/domain/financial_operation_key.dart';
import 'package:ahdah_app/features/expenses/domain/expense_filters.dart';
import 'package:ahdah_app/features/expenses/domain/expense_models.dart';
import 'package:ahdah_app/features/expenses/domain/expense_repository.dart';
import 'package:ahdah_app/features/expenses/domain/expense_requests.dart';
import 'package:ahdah_app/features/expenses/presentation/controllers/expense_controllers.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  group('expense capabilities', () {
    test('exact roles receive conservative expense capabilities', () {
      final manager = RoleCapabilities.forRole('Manager');
      expect(manager.canViewExpenses, isTrue);
      expect(manager.canCreateExpenses, isTrue);
      expect(manager.canReviewExpenses, isTrue);
      expect(manager.canManageExpenseCategories, isTrue);
      expect(manager.canCreateProjectLinkedExpense, isTrue);

      final deputy = RoleCapabilities.forRole('Deputy');
      expect(deputy.canCreateExpenses, isTrue);
      expect(deputy.canReviewExpenses, isTrue);
      expect(deputy.canManageExpenseCategories, isFalse);

      final accountant = RoleCapabilities.forRole('Accountant');
      expect(accountant.canViewExpenses, isTrue);
      expect(accountant.canCreateExpenses, isFalse);
      expect(accountant.canReviewExpenses, isTrue);

      final supervisor = RoleCapabilities.forRole('Supervisor');
      expect(supervisor.canCreateProjectLinkedExpense, isTrue);
      expect(supervisor.canReviewExpenses, isFalse);

      final worker = RoleCapabilities.forRole('Worker');
      expect(worker.canCreateExpenses, isTrue);
      expect(worker.canCreateProjectLinkedExpense, isFalse);
      expect(worker.canViewReimbursements, isTrue);

      final unknown = RoleCapabilities.forRole('FutureRole');
      expect(unknown.canViewExpenses, isFalse);
      expect(unknown.canCreateExpenses, isFalse);
      expect(unknown.canReviewExpenses, isFalse);
    });
  });

  group('expense controllers', () {
    test(
      'list loads, resets filtered page and deduplicates load more',
      () async {
        final repository = _FakeExpenseRepository()
          ..expensePage = ExpensePage(
            items: [_summary],
            page: 1,
            pageSize: 20,
            totalCount: 2,
            totalPages: 2,
          );
        final controller = ExpenseListController(
          repository,
          onUnauthorized: () async {},
        );
        await controller.load();
        expect(controller.state.items, hasLength(1));

        await controller.setFilters(
          const ExpenseFilters(status: 'PendingReview'),
        );
        expect(repository.lastFilters.status, 'PendingReview');
        expect(repository.lastPage, 1);

        repository.expensePage = ExpensePage(
          items: [_summary],
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

    test('load-more failure preserves loaded expense records', () async {
      final repository = _FakeExpenseRepository()
        ..expensePage = ExpensePage(
          items: [_summary],
          page: 1,
          pageSize: 20,
          totalCount: 2,
          totalPages: 2,
        );
      final controller = ExpenseListController(
        repository,
        onUnauthorized: () async {},
      );
      await controller.load();
      repository.readError = const AppException(AppExceptionKind.network);
      await controller.loadMore();
      expect(controller.state.items.single.expenseId, 'expense-1');
      expect(controller.state.loadMoreError?.kind, AppExceptionKind.network);
    });

    test('duplicate expense create submissions are suppressed', () async {
      final repository = _FakeExpenseRepository();
      final completer = Completer<ExpenseDetails>();
      repository.createCompleter = completer;
      final controller = ExpenseCreateController(
        repository,
        _SequenceKeyFactory(),
        onUnauthorized: () async {},
      );
      final first = controller.create(_createInput('25.20'));
      expect(await controller.create(_createInput('25.20')), isNull);
      expect(repository.createCalls, 1);
      completer.complete(_details);
      expect(await first, _details);
    });

    test(
      'ambiguous create retains payload and key for explicit retry',
      () async {
        final repository = _FakeExpenseRepository()
          ..writeError = const AppException(AppExceptionKind.timeout);
        final controller = ExpenseCreateController(
          repository,
          _SequenceKeyFactory(),
          onUnauthorized: () async {},
        );
        expect(await controller.create(_createInput('25.20')), isNull);
        final key = repository.lastIdempotencyKey;
        expect(controller.state.phase, ExpenseCommandPhase.uncertainSubmission);

        repository.writeError = null;
        expect(await controller.retrySameOperation(), _details);
        expect(repository.lastIdempotencyKey, key);
        expect(repository.createCalls, 2);
      },
    );

    test('changed payload invalidates uncertain operation key', () async {
      final repository = _FakeExpenseRepository()
        ..writeError = const AppException(AppExceptionKind.network);
      final controller = ExpenseCreateController(
        repository,
        _SequenceKeyFactory(),
        onUnauthorized: () async {},
      );
      await controller.create(_createInput('25.20'));
      final firstKey = repository.lastIdempotencyKey;
      repository.writeError = null;
      await controller.create(_createInput('25.21'));
      expect(repository.lastIdempotencyKey, isNot(firstKey));
    });

    test('approval and rejection use focused command payloads', () async {
      final repository = _FakeExpenseRepository();
      final keys = _SequenceKeyFactory();
      final approval = ExpenseApprovalController(
        repository,
        keys,
        onUnauthorized: () async {},
      );
      await approval.approve(
        'expense-1',
        const ApproveExpenseInput(expectedVersion: 3),
      );
      expect(repository.approvalInput?.expectedVersion, 3);
      expect(repository.lastIdempotencyKey, isNotEmpty);

      final rejection = ExpenseRejectionController(
        repository,
        keys,
        onUnauthorized: () async {},
      );
      await rejection.reject(
        'expense-1',
        const RejectExpenseInput(expectedVersion: 3, reason: 'Duplicate'),
      );
      expect(repository.rejectionInput?.reason, 'Duplicate');
    });
  });
}

final _details = ExpenseDetails.fromJson(const {
  'expense': {
    'expenseId': 'expense-1',
    'expenseNumber': 'EXP-ONE',
    'expenseDate': '2026-08-04',
    'totalAmount': '25.20',
    'currencyCode': 'LYD',
    'paymentMode': 'PersonalFunds',
    'description': 'Purchase',
    'status': 'PendingReview',
    'category': {
      'expenseCategoryId': 'category-1',
      'categoryName': 'Materials',
      'categoryGroup': 'Materials',
      'expenseScope': 'Both',
      'requiresSupplier': false,
      'requiresReceipt': true,
      'supportsQuantityDetails': false,
      'isActive': true,
      'displayOrder': 0,
      'versionNumber': 1,
    },
    'incurredBy': {'userId': 'user-1', 'fullName': 'User', 'role': 'Worker'},
    'submittedBy': {'userId': 'user-1', 'fullName': 'User', 'role': 'Worker'},
    'hasReceiptDocument': false,
    'versionNumber': 3,
    'createdAtUtc': '2026-08-04T09:00:00Z',
  },
  'subtotalAmount': '25.20',
  'discountAmount': '0.00',
  'taxAmount': '0.00',
  'advanceAllocations': [],
  'documents': [],
  'items': [],
});

final _summary = _details.expense;

CreateExpenseInput _createInput(String amount) => CreateExpenseInput(
  expenseCategoryId: 'category-1',
  expenseDate: DateTime(2026, 8, 4),
  amount: amount,
  currencyCode: 'LYD',
  paymentMode: 'PersonalFunds',
  description: 'Purchase',
);

final class _SequenceKeyFactory implements FinancialOperationKeyFactory {
  int _next = 0;
  @override
  String create() => 'expense-operation-key-${++_next}-123456789';
}

final class _FakeExpenseRepository implements ExpenseRepository {
  ExpensePage expensePage = const ExpensePage(
    items: [],
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,
  );
  AppException? readError;
  AppException? writeError;
  Completer<ExpenseDetails>? createCompleter;
  int createCalls = 0;
  int lastPage = 0;
  ExpenseFilters lastFilters = const ExpenseFilters();
  String lastIdempotencyKey = '';
  ApproveExpenseInput? approvalInput;
  RejectExpenseInput? rejectionInput;

  @override
  Future<ExpensePage> listExpenses({
    required int page,
    required int pageSize,
    required ExpenseFilters filters,
  }) async {
    lastPage = page;
    lastFilters = filters;
    if (readError != null) throw readError!;
    return expensePage;
  }

  @override
  Future<ExpenseDetails> createExpense(
    CreateExpenseInput input,
    String idempotencyKey,
  ) async {
    createCalls++;
    lastIdempotencyKey = idempotencyKey;
    if (writeError != null) throw writeError!;
    return createCompleter?.future ?? _details;
  }

  @override
  Future<ExpenseDetails> approveExpense(
    String expenseId,
    ApproveExpenseInput input,
    String idempotencyKey,
  ) async {
    approvalInput = input;
    lastIdempotencyKey = idempotencyKey;
    if (writeError != null) throw writeError!;
    return _details;
  }

  @override
  Future<ExpenseDetails> rejectExpense(
    String expenseId,
    RejectExpenseInput input,
    String idempotencyKey,
  ) async {
    rejectionInput = input;
    lastIdempotencyKey = idempotencyKey;
    if (writeError != null) throw writeError!;
    return _details;
  }

  @override
  Future<ExpenseCategoryPage> listCategories({
    required int page,
    required int pageSize,
    String? categoryGroup,
    String? expenseScope,
  }) async => const ExpenseCategoryPage(
    items: [],
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,
  );

  @override
  Future<ExpenseCategory> createCategory(CreateExpenseCategoryInput input) =>
      throw UnimplementedError();
  @override
  Future<ExpenseDetails> getExpense(String expenseId) async => _details;
  @override
  Future<ExpenseAllocationPage> listAllocations(
    String expenseId, {
    required int page,
    required int pageSize,
  }) async => const ExpenseAllocationPage(
    items: [],
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,
  );
  @override
  Future<ExpenseDocumentPage> listDocuments(
    String expenseId, {
    required int page,
    required int pageSize,
  }) async => const ExpenseDocumentPage(
    items: [],
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,
  );
  @override
  Future<ExpenseHistoryPage> listHistory(
    String expenseId, {
    required int page,
    required int pageSize,
  }) async => const ExpenseHistoryPage(
    items: [],
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,
  );
  @override
  Future<ReimbursementPage> listReimbursements({
    required int page,
    required int pageSize,
    String? status,
    String? claimantUserId,
  }) async => const ReimbursementPage(
    items: [],
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,
  );
  @override
  Future<ReimbursementSummary> getReimbursement(String reimbursementId) =>
      throw UnimplementedError();
}
