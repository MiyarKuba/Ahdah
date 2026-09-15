import 'package:ahdah_app/app/app.dart';
import 'package:ahdah_app/app/providers.dart';
import 'package:ahdah_app/core/config/app_config.dart';
import 'package:ahdah_app/features/authenticated_home/presentation/home_page.dart';
import 'package:ahdah_app/features/authentication/domain/identity_models.dart';
import 'package:ahdah_app/features/suppliers/domain/supplier_models.dart';
import 'package:ahdah_app/features/suppliers/domain/supplier_repository.dart';
import 'package:ahdah_app/features/suppliers/domain/supplier_requests.dart';
import 'package:ahdah_app/features/suppliers/presentation/supplier_forms.dart';
import 'package:ahdah_app/features/suppliers/presentation/supplier_pages.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';

import 'helpers/fakes.dart';

void main() {
  testWidgets('supported roles see Suppliers while worker and unknown do not', (
    tester,
  ) async {
    for (final role in ['Manager', 'Deputy', 'Accountant', 'Supervisor']) {
      await _pump(tester, role: role);
      expect(find.text('Suppliers'), findsWidgets, reason: role);
    }
    for (final role in ['Worker', 'FutureRole']) {
      await _pump(tester, role: role);
      expect(find.text('Suppliers'), findsNothing, reason: role);
    }
  });

  testWidgets('manager sees supplier management while deputy is invoice-only', (
    tester,
  ) async {
    await _pump(tester, role: 'Manager');
    _go(tester, '/suppliers');
    await tester.pumpAndSettle();
    expect(find.text('Add supplier'), findsOneWidget);
    _go(tester, '/suppliers/supplier-1/edit');
    await tester.pumpAndSettle();
    expect(find.byType(SupplierFormPage), findsOneWidget);
    for (var index = 0; index < 4; index++) {
      await tester.drag(find.byType(ListView).last, const Offset(0, -500));
      await tester.pump();
    }
    await tester.tap(find.text('Deactivate supplier'));
    await tester.pumpAndSettle();
    expect(find.textContaining('history is preserved'), findsOneWidget);
    await tester.tap(find.text('Cancel'));

    await _pump(tester, role: 'Deputy');
    _go(tester, '/supplier-invoices/new');
    await tester.pumpAndSettle();
    expect(find.byType(SupplierInvoiceCreatePage), findsOneWidget);
    _go(tester, '/supplier-payments/new');
    await tester.pumpAndSettle();
    expect(find.byType(HomePage), findsOneWidget);
  });

  testWidgets(
    'accountant sees payment and credit actions but no supplier edit',
    (tester) async {
      await _pump(tester, role: 'Accountant');
      _go(tester, '/supplier-payments');
      await tester.pumpAndSettle();
      expect(find.text('Record payment'), findsOneWidget);
      _go(tester, '/supplier-credit-notes');
      await tester.pumpAndSettle();
      expect(find.text('Create credit note'), findsOneWidget);
      _go(tester, '/suppliers/supplier-1/edit');
      await tester.pumpAndSettle();
      expect(find.byType(HomePage), findsOneWidget);
    },
  );

  testWidgets('supervisor has read-only constrained supplier access', (
    tester,
  ) async {
    await _pump(tester, role: 'Supervisor');
    _go(tester, '/suppliers');
    await tester.pumpAndSettle();
    expect(find.byType(SuppliersPage), findsOneWidget);
    expect(find.text('Add supplier'), findsNothing);
    _go(tester, '/supplier-payments');
    await tester.pumpAndSettle();
    expect(find.byType(HomePage), findsOneWidget);
  });

  testWidgets('payment accounts are masked and refunds remain read-only', (
    tester,
  ) async {
    await _pump(tester, role: 'Manager');
    _go(tester, '/suppliers/supplier-1/payment-accounts');
    await tester.pumpAndSettle();
    expect(find.textContaining('****3456'), findsWidgets);
    expect(find.textContaining('1234567890123456'), findsNothing);
    _go(tester, '/supplier-refunds');
    await tester.pumpAndSettle();
    expect(find.textContaining('read-only'), findsOneWidget);
    expect(find.textContaining('Create refund'), findsNothing);
  });

  testWidgets('statement warns that currencies are never totaled together', (
    tester,
  ) async {
    await _pump(tester, role: 'Manager');
    _go(tester, '/suppliers/supplier-1/statement');
    await tester.pumpAndSettle();
    expect(find.textContaining('never totaled together'), findsOneWidget);
    expect(find.text('LYD'), findsWidgets);
    expect(find.text('USD'), findsWidgets);
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
        supplierRepositoryProvider.overrideWithValue(_FakeSupplierRepository()),
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

final _supplier = SupplierSummary.fromJson(const {
  'supplierId': 'supplier-1',
  'supplierCode': 'SUP-1',
  'supplierName': 'Supplier One',
  'supplierType': 'GeneralSupplier',
  'defaultCurrencyCode': 'LYD',
  'transactionMode': 'CashAndCredit',
  'defaultPaymentTermsDays': 30,
  'isActive': true,
  'versionNumber': 1,
});
final _details = SupplierDetails(
  supplier: _supplier,
  balances: const [
    SupplierBalanceSummary(
      currencyCode: 'LYD',
      outstandingAmount: '100.00',
      openDebtCount: 1,
    ),
    SupplierBalanceSummary(
      currencyCode: 'USD',
      outstandingAmount: '20.00',
      openDebtCount: 1,
    ),
  ],
);

final class _FakeSupplierRepository implements SupplierRepository {
  @override
  Future<SupplierPage<SupplierSummary>> listSuppliers({
    required int page,
    required int pageSize,
    required SupplierFilters filters,
  }) async => SupplierPage(
    items: [_supplier],
    page: 1,
    pageSize: 20,
    totalCount: 1,
    totalPages: 1,
  );
  @override
  Future<SupplierDetails> getSupplier(String supplierId) async => _details;
  @override
  Future<SupplierDetails> createSupplier(SupplierCreateInput input) async =>
      _details;
  @override
  Future<SupplierDetails> updateSupplier(
    String supplierId,
    SupplierUpdateInput input,
  ) async => _details;
  @override
  Future<SupplierPage<SupplierPaymentAccount>> listPaymentAccounts(
    String supplierId, {
    required int page,
    required int pageSize,
  }) async => SupplierPage(
    items: [
      SupplierPaymentAccount.fromJson(const {
        'supplierPaymentAccountId': 'account-1',
        'accountType': 'BankAccount',
        'accountLabel': 'Primary',
        'maskedAccountNumber': '****3456',
        'currencyCode': 'LYD',
        'isDefault': false,
        'verificationStatus': 'Verified',
        'isActive': true,
        'versionNumber': 1,
      }),
    ],
    page: 1,
    pageSize: 20,
    totalCount: 1,
    totalPages: 1,
  );
  @override
  Future<SupplierPaymentAccount> createPaymentAccount(
    String supplierId,
    SupplierPaymentAccountInput input,
    String idempotencyKey,
  ) => throw UnimplementedError();
  @override
  Future<SupplierPage<SupplierInvoiceSummary>> listInvoices({
    required int page,
    required int pageSize,
    required SupplierInvoiceFilters filters,
  }) async => SupplierPage(
    items: const [],
    page: page,
    pageSize: pageSize,
    totalCount: 0,
    totalPages: 0,
  );
  @override
  Future<SupplierInvoiceDetails> getInvoice(String debtId) =>
      throw UnimplementedError();
  @override
  Future<SupplierInvoiceDetails> createInvoice(
    SupplierInvoiceCreateInput input,
    String idempotencyKey,
  ) => throw UnimplementedError();
  @override
  Future<SupplierPage<SupplierPaymentSummary>> listPayments({
    required int page,
    required int pageSize,
    required SupplierPaymentFilters filters,
  }) async => SupplierPage(
    items: const [],
    page: page,
    pageSize: pageSize,
    totalCount: 0,
    totalPages: 0,
  );
  @override
  Future<SupplierPaymentDetails> getPayment(String paymentId) =>
      throw UnimplementedError();
  @override
  Future<SupplierPage<SupplierFundingSource>> listFundingSources({
    required int page,
    required int pageSize,
    String? currencyCode,
    String? paymentMethod,
  }) async => SupplierPage(
    items: const [],
    page: page,
    pageSize: pageSize,
    totalCount: 0,
    totalPages: 0,
  );
  @override
  Future<SupplierPaymentDetails> createPayment(
    SupplierPaymentCreateInput input,
    String idempotencyKey,
  ) => throw UnimplementedError();
  @override
  Future<SupplierPaymentDetails> confirmPayment(
    String paymentId,
    SupplierPaymentReviewInput input,
    String idempotencyKey,
  ) => throw UnimplementedError();
  @override
  Future<SupplierPaymentDetails> rejectPayment(
    String paymentId,
    SupplierPaymentReviewInput input,
    String idempotencyKey,
  ) => throw UnimplementedError();
  @override
  Future<SupplierPage<SupplierCreditNoteSummary>> listCreditNotes({
    required int page,
    required int pageSize,
    String? supplierId,
    String? status,
    String? currencyCode,
  }) async => SupplierPage(
    items: const [],
    page: page,
    pageSize: pageSize,
    totalCount: 0,
    totalPages: 0,
  );
  @override
  Future<SupplierCreditNoteDetails> getCreditNote(String creditNoteId) =>
      throw UnimplementedError();
  @override
  Future<SupplierCreditNoteDetails> createCreditNote(
    SupplierCreditCreateInput input,
    String idempotencyKey,
  ) => throw UnimplementedError();
  @override
  Future<SupplierCreditNoteDetails> approveCreditNote(
    String creditNoteId,
    SupplierCreditApproveInput input,
    String idempotencyKey,
  ) => throw UnimplementedError();
  @override
  Future<SupplierCreditNoteDetails> applyCreditNote(
    String creditNoteId,
    SupplierCreditApplyInput input,
    String idempotencyKey,
  ) => throw UnimplementedError();
  @override
  Future<SupplierPage<SupplierRefundSummary>> listRefunds({
    required int page,
    required int pageSize,
    String? supplierId,
    String? status,
    String? currencyCode,
  }) async => SupplierPage(
    items: const [],
    page: page,
    pageSize: pageSize,
    totalCount: 0,
    totalPages: 0,
  );
  @override
  Future<SupplierStatement> getStatement(
    String supplierId, {
    required int page,
    required int pageSize,
    String? currencyCode,
  }) async => SupplierStatement(
    supplier: _supplier,
    balances: _details.balances,
    entries: SupplierPage(
      items: const [],
      page: 1,
      pageSize: 20,
      totalCount: 0,
      totalPages: 0,
    ),
  );
}
