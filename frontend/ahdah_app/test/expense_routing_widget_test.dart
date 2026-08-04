import 'package:ahdah_app/app/app.dart';
import 'package:ahdah_app/app/providers.dart';
import 'package:ahdah_app/core/config/app_config.dart';
import 'package:ahdah_app/features/authenticated_home/presentation/home_page.dart';
import 'package:ahdah_app/features/authentication/domain/identity_models.dart';
import 'package:ahdah_app/features/expenses/domain/expense_filters.dart';
import 'package:ahdah_app/features/expenses/domain/expense_models.dart';
import 'package:ahdah_app/features/expenses/domain/expense_repository.dart';
import 'package:ahdah_app/features/expenses/domain/expense_requests.dart';
import 'package:ahdah_app/features/expenses/presentation/categories/expense_categories_page.dart';
import 'package:ahdah_app/features/expenses/presentation/create/expense_create_page.dart';
import 'package:ahdah_app/features/expenses/presentation/documents/expense_documents_page.dart';
import 'package:ahdah_app/features/expenses/presentation/list/expenses_page.dart';
import 'package:ahdah_app/features/expenses/presentation/review/expense_review_page.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';

import 'helpers/fakes.dart';

void main() {
  testWidgets('supported roles see Expenses and unknown role does not', (
    tester,
  ) async {
    for (final role in [
      'Manager',
      'Deputy',
      'Accountant',
      'Supervisor',
      'Worker',
    ]) {
      await _pump(tester, role: role);
      expect(find.text('Expenses'), findsWidgets, reason: role);
    }
    await _pump(tester, role: 'FutureRole');
    expect(find.text('Expenses'), findsNothing);
  });

  testWidgets('accountant cannot create and worker has no project selector', (
    tester,
  ) async {
    await _pump(tester, role: 'Accountant');
    _go(tester, '/expenses/new');
    await tester.pumpAndSettle();
    expect(find.byType(HomePage), findsOneWidget);

    await _pump(tester, role: 'Worker');
    _go(tester, '/expenses/new');
    await tester.pumpAndSettle();
    expect(find.byType(ExpenseCreatePage), findsOneWidget);
    expect(find.text('Project (optional)'), findsNothing);
    expect(find.text('Supplier credit'), findsNothing);
    expect(find.text('Mixed payment'), findsNothing);
  });

  testWidgets('category creation is manager-only', (tester) async {
    await _pump(tester, role: 'Manager');
    _go(tester, '/expense-categories');
    await tester.pumpAndSettle();
    expect(find.byType(ExpenseCategoriesPage), findsOneWidget);
    expect(
      find.byKey(const Key('create-expense-category-action')),
      findsOneWidget,
    );
    _go(tester, '/expense-categories/new');
    await tester.pumpAndSettle();
    expect(find.byType(ExpenseCategoryCreatePage), findsOneWidget);

    await _pump(tester, role: 'Deputy');
    _go(tester, '/expense-categories/new');
    await tester.pumpAndSettle();
    expect(find.byType(HomePage), findsOneWidget);
  });

  testWidgets('review route is reviewer-only and metadata is read-only', (
    tester,
  ) async {
    await _pump(tester, role: 'Manager');
    _go(tester, '/expenses/expense-1/review');
    await tester.pumpAndSettle();
    expect(find.byType(ExpenseReviewPage), findsOneWidget);
    expect(find.byKey(const Key('approve-expense-action')), findsOneWidget);

    _go(tester, '/expenses/expense-1/documents');
    await tester.pumpAndSettle();
    expect(find.byType(ExpenseDocumentsPage), findsOneWidget);
    expect(
      find.textContaining('File upload is not implemented'),
      findsOneWidget,
    );
    expect(find.byIcon(Icons.upload_file), findsNothing);

    await _pump(tester, role: 'Worker');
    _go(tester, '/expenses/expense-1/review');
    await tester.pumpAndSettle();
    expect(find.byType(HomePage), findsOneWidget);
  });

  testWidgets('expense list has no unsupported financial actions', (
    tester,
  ) async {
    await _pump(tester, role: 'Manager');
    _go(tester, '/expenses');
    await tester.pumpAndSettle();
    expect(find.byType(ExpensesPage), findsOneWidget);
    expect(find.text('Settlement'), findsNothing);
    expect(find.text('Pay claim'), findsNothing);
    expect(find.text('Paper custody'), findsNothing);
  });
}

Future<void> _pump(WidgetTester tester, {required String role}) async {
  await tester.pumpWidget(const SizedBox.shrink());
  await tester.pump();
  SharedPreferences.setMockInitialValues({'ahdah.locale': 'en'});
  final preferences = await SharedPreferences.getInstance();
  final user = AuthenticatedUser(
    userId: 'user-$role',
    fullName: '$role User',
    role: role,
    status: 'Active',
    identityVerificationStatus: role == 'Manager' ? 'Verified' : 'NotRequired',
  );
  final auth = FakeAuthRepository()
    ..current = CurrentSessionResult(
      user: user,
      company: testCompany,
      role: role,
    );
  await tester.pumpWidget(
    ProviderScope(
      overrides: [
        appConfigProvider.overrideWithValue(
          AppConfig.parse('http://localhost:5231'),
        ),
        sharedPreferencesProvider.overrideWithValue(preferences),
        accessTokenStoreProvider.overrideWithValue(
          FakeAccessTokenStore('valid-token'),
        ),
        authRepositoryProvider.overrideWithValue(auth),
        accessRepositoryProvider.overrideWithValue(FakeAccessRepository()),
        projectRepositoryProvider.overrideWithValue(FakeProjectRepository()),
        companyMemberRepositoryProvider.overrideWithValue(
          FakeCompanyMemberRepository(),
        ),
        advanceRepositoryProvider.overrideWithValue(FakeAdvanceRepository()),
        expenseRepositoryProvider.overrideWithValue(_FakeExpenseRepository()),
      ],
      child: const AhdahApp(),
    ),
  );
  await tester.pump();
  await tester.pump(const Duration(milliseconds: 500));
}

void _go(WidgetTester tester, String location) {
  final context = tester.element(find.byType(Scaffold).first);
  GoRouter.of(context).go(location);
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

final class _FakeExpenseRepository implements ExpenseRepository {
  @override
  Future<ExpensePage> listExpenses({
    required int page,
    required int pageSize,
    required ExpenseFilters filters,
  }) async => ExpensePage(
    items: [_details.expense],
    page: 1,
    pageSize: 20,
    totalCount: 1,
    totalPages: 1,
  );

  @override
  Future<ExpenseDetails> getExpense(String expenseId) async => _details;

  @override
  Future<ExpenseCategoryPage> listCategories({
    required int page,
    required int pageSize,
    String? categoryGroup,
    String? expenseScope,
  }) async => ExpenseCategoryPage(
    items: [_details.expense.category],
    page: 1,
    pageSize: 20,
    totalCount: 1,
    totalPages: 1,
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
  Future<ExpenseDetails> createExpense(
    CreateExpenseInput input,
    String idempotencyKey,
  ) async => _details;
  @override
  Future<ExpenseDetails> approveExpense(
    String expenseId,
    ApproveExpenseInput input,
    String idempotencyKey,
  ) async => _details;
  @override
  Future<ExpenseDetails> rejectExpense(
    String expenseId,
    RejectExpenseInput input,
    String idempotencyKey,
  ) async => _details;
  @override
  Future<ExpenseCategory> createCategory(CreateExpenseCategoryInput input) =>
      throw UnimplementedError();
  @override
  Future<ReimbursementSummary> getReimbursement(String reimbursementId) =>
      throw UnimplementedError();
}
