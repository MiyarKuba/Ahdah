import 'dart:convert';
import 'dart:typed_data';

import 'package:ahdah_app/core/network/api_client.dart';
import 'package:ahdah_app/features/expenses/domain/expense_filters.dart';
import 'package:ahdah_app/features/expenses/domain/expense_models.dart';
import 'package:ahdah_app/features/expenses/domain/expense_requests.dart';
import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';

import 'helpers/fakes.dart';

const _user = {'userId': 'user-1', 'fullName': 'User One', 'role': 'Worker'};
const _category = {
  'expenseCategoryId': 'category-1',
  'parentExpenseCategoryId': null,
  'categoryCode': 'MAT',
  'categoryName': 'Materials',
  'categoryGroup': 'Materials',
  'expenseScope': 'Both',
  'description': null,
  'requiresSupplier': false,
  'requiresReceipt': true,
  'supportsQuantityDetails': false,
  'isActive': true,
  'displayOrder': 1,
  'versionNumber': 2,
};
const _summary = {
  'expenseId': 'expense-1',
  'expenseNumber': 'EXP-ONE',
  'expenseDate': '2026-08-04',
  'totalAmount': 25.2,
  'currencyCode': 'LYD',
  'paymentMode': 'PersonalFunds',
  'description': 'Purchase',
  'status': 'FutureStatus',
  'category': _category,
  'project': {'projectId': 'project-1', 'projectName': 'Project One'},
  'incurredBy': _user,
  'submittedBy': _user,
  'receiptNumber': null,
  'invoiceNumber': 'INV-1',
  'hasReceiptDocument': true,
  'reimbursementStatus': 'Open',
  'versionNumber': 3,
  'createdAtUtc': '2026-08-04T09:00:00Z',
  'submittedAtUtc': '2026-08-04T09:00:00Z',
  'reviewedAtUtc': null,
};
const _claim = {
  'reimbursementId': 'claim-1',
  'reimbursementNumber': 'CLM-ONE',
  'expenseId': 'expense-1',
  'expenseNumber': 'EXP-ONE',
  'claimant': _user,
  'project': null,
  'claimDate': '2026-08-04',
  'dueDate': null,
  'claimAmount': 25.2,
  'outstandingAmount': 25.2,
  'currencyCode': 'LYD',
  'description': 'Purchase',
  'status': 'Open',
  'versionNumber': 1,
  'createdAtUtc': '2026-08-04T09:00:00Z',
};
const _details = {
  'expense': _summary,
  'subtotalAmount': 25.2,
  'discountAmount': 0,
  'taxAmount': 0,
  'merchantName': 'Shop',
  'expenseLocation': null,
  'notes': null,
  'correctionReason': null,
  'rejectionReason': null,
  'reviewedBy': null,
  'advanceAllocations': [
    {
      'expenseAdvanceAllocationId': 'allocation-1',
      'advanceId': 'advance-1',
      'advanceNumber': 'ADV-ONE',
      'allocatedAmount': 25.2,
      'createdAtUtc': '2026-08-04T09:00:00Z',
    },
  ],
  'documents': [
    {
      'expenseDocumentId': 'document-1',
      'documentType': 'Receipt',
      'documentNumber': 'R-1',
      'documentDate': '2026-08-04',
      'issuerName': 'Shop',
      'originalFileName': 'receipt.pdf',
      'mimeType': 'application/pdf',
      'fileSizeBytes': 123,
      'captureSource': 'FileUpload',
      'isPrimary': true,
      'verificationStatus': 'PendingVerification',
      'uploadedBy': _user,
      'verifiedBy': null,
      'createdAtUtc': '2026-08-04T09:00:00Z',
      'verifiedAtUtc': null,
      'rejectionReason': null,
      'notes': null,
      'versionNumber': 1,
    },
  ],
  'items': [
    {
      'expenseItemId': 'item-1',
      'lineNumber': 1,
      'itemName': 'Cement',
      'itemCode': null,
      'itemDescription': null,
      'quantity': 2,
      'unitCode': 'Bag',
      'customUnitName': null,
      'unitPrice': 12.6,
      'subtotalAmount': 25.2,
      'discountAmount': 0,
      'taxAmount': 0,
      'totalAmount': 25.2,
      'notes': null,
    },
  ],
  'reimbursement': _claim,
};

void main() {
  test(
    'expense category and list parse exact safe fields and unknown enum',
    () {
      final page = ExpensePage.fromJson(const {
        'items': [_summary],
        'page': 1,
        'pageSize': 20,
        'totalCount': 1,
        'totalPages': 1,
      });
      final expense = page.items.single;
      expect(expense.totalAmount, '25.20');
      expect(expense.status, 'FutureStatus');
      expect(expense.category.expenseScope, 'Both');
      expect(expense.project?.projectName, 'Project One');
      expect(expense.createdAtUtc, DateTime.utc(2026, 8, 4, 9));
    },
  );

  test('detail parses allocations documents items and reimbursement', () {
    final details = ExpenseDetails.fromJson(_details);
    expect(details.advanceAllocations.single.allocatedAmount, '25.20');
    expect(details.documents.single.originalFileName, 'receipt.pdf');
    expect(details.items.single.unitPrice, '12.60');
    expect(details.items.single.quantity, '2.00');
    expect(details.reimbursement?.outstandingAmount, '25.20');
    expect(details.toString(), isNot(contains('fileUrl')));
    expect(details.toString(), isNot(contains('sha256')));
  });

  test('history parser exposes safe projection without raw audit JSON', () {
    final history = ExpenseHistoryPage.fromJson(const {
      'items': [
        {
          'eventId': 'event-1',
          'eventName': 'ExpenseCreated',
          'eventAction': 'Create',
          'outcome': 'Success',
          'description': 'Expense submitted for review.',
          'actor': _user,
          'expenseVersionNumber': 1,
          'occurredAtUtc': '2026-08-04T09:00:00Z',
          'metadata': {'secret': true},
        },
      ],
      'page': 1,
      'pageSize': 20,
      'totalCount': 1,
      'totalPages': 1,
    });
    expect(history.items.single.eventName, 'ExpenseCreated');
    expect(history.items.single.toString(), isNot(contains('metadata')));
  });

  test(
    'creation body preserves exact decimals and excludes authority fields',
    () {
      final input = CreateExpenseInput(
        expenseCategoryId: 'category-1',
        projectId: 'project-1',
        expenseDate: DateTime(2026, 8, 4),
        amount: '100.00',
        currencyCode: 'lyd',
        paymentMode: 'AdvanceBalance',
        description: 'Purchase',
        advanceAllocations: const [
          ExpenseAdvanceAllocationInput(
            userAdvanceBalanceId: 'balance-1',
            amount: '60.00',
          ),
          ExpenseAdvanceAllocationInput(
            userAdvanceBalanceId: 'balance-2',
            amount: '40.00',
          ),
        ],
      );
      final body = input.toJsonBody();
      expect(body, contains('"amount":100.00'));
      expect(body, contains('"amount":60.00'));
      expect(body, contains('"amount":40.00'));
      expect(body, contains('"currencyCode":"LYD"'));
      expect(body, isNot(contains('companyId')));
      expect(body, isNot(contains('status')));
      expect(body, isNot(contains('availableAmount')));
      expect(body, isNot(contains('SupplierCredit')));
    },
  );

  test('expense list sends exact supported query parameters only', () async {
    final adapter = RecordingAdapter(
      body: const {
        'items': [],
        'page': 2,
        'pageSize': 20,
        'totalCount': 0,
        'totalPages': 0,
      },
    );
    await _client(adapter).listExpenses(
      page: 2,
      pageSize: 20,
      filters: const ExpenseFilters(
        status: 'PendingReview',
        categoryId: 'category-1',
        projectId: 'project-1',
        incurredByUserId: 'user-1',
        submittedByUserId: 'user-1',
        paymentMode: 'PersonalFunds',
        reference: 'EXP',
      ),
    );
    expect(adapter.lastRequest!.path, '/api/v1/expenses');
    expect(adapter.lastRequest!.queryParameters, {
      'page': 2,
      'pageSize': 20,
      'status': 'PendingReview',
      'categoryId': 'category-1',
      'projectId': 'project-1',
      'incurredByUserId': 'user-1',
      'submittedByUserId': 'user-1',
      'paymentMode': 'PersonalFunds',
      'reference': 'EXP',
    });
    expect(adapter.lastRequest!.headers, isNot(contains('Idempotency-Key')));
    expect(adapter.lastRequest!.queryParameters, isNot(contains('company_id')));
  });

  test(
    'expense creation approval and rejection send idempotency header',
    () async {
      final createAdapter = RecordingAdapter(statusCode: 201, body: _details);
      final input = CreateExpenseInput(
        expenseCategoryId: 'category-1',
        expenseDate: DateTime(2026, 8, 4),
        amount: '25.20',
        currencyCode: 'LYD',
        paymentMode: 'PersonalFunds',
        description: 'Purchase',
      );
      await _client(createAdapter).createExpense(input, 'secure-key-123456789');
      expect(createAdapter.lastRequest!.path, '/api/v1/expenses');
      expect(
        createAdapter.lastRequest!.headers['Idempotency-Key'],
        'secure-key-123456789',
      );

      final approveAdapter = RecordingAdapter(body: _details);
      await _client(approveAdapter).approveExpense(
        'expense-1',
        const ApproveExpenseInput(expectedVersion: 3),
        'secure-key-123456789',
      );
      expect(
        approveAdapter.lastRequest!.path,
        '/api/v1/expenses/expense-1/approve',
      );
      expect(approveAdapter.lastRequest!.data, {'expectedVersion': 3});

      final rejectAdapter = RecordingAdapter(body: _details);
      await _client(rejectAdapter).rejectExpense(
        'expense-1',
        const RejectExpenseInput(expectedVersion: 3, reason: 'Duplicate'),
        'secure-key-123456789',
      );
      expect(
        rejectAdapter.lastRequest!.path,
        '/api/v1/expenses/expense-1/reject',
      );
      expect(rejectAdapter.lastRequest!.data, {
        'expectedVersion': 3,
        'reason': 'Duplicate',
      });
    },
  );

  test(
    'category, detail, document, history and reimbursement routes are exact',
    () async {
      final categoryAdapter = RecordingAdapter(
        body: const {
          'items': [_category],
          'page': 1,
          'pageSize': 20,
          'totalCount': 1,
          'totalPages': 1,
        },
      );
      await _client(categoryAdapter).listExpenseCategories(
        page: 1,
        pageSize: 20,
        categoryGroup: 'Materials',
        expenseScope: 'Both',
      );
      expect(categoryAdapter.lastRequest!.path, '/api/v1/expense-categories');

      final createCategoryAdapter = RecordingAdapter(
        statusCode: 201,
        body: _category,
      );
      await _client(createCategoryAdapter).createExpenseCategory(
        const CreateExpenseCategoryInput(
          categoryName: 'Materials',
          categoryCode: 'MAT',
          categoryGroup: 'Materials',
          expenseScope: 'Both',
        ),
      );
      expect(
        createCategoryAdapter.lastRequest!.path,
        '/api/v1/expense-categories',
      );
      expect(
        createCategoryAdapter.lastRequest!.headers,
        isNot(contains('Idempotency-Key')),
      );

      final detailAdapter = RecordingAdapter(body: _details);
      await _client(detailAdapter).getExpense('expense-1');
      expect(detailAdapter.lastRequest!.path, '/api/v1/expenses/expense-1');

      final pageBody = const {
        'items': <Object?>[],
        'page': 1,
        'pageSize': 20,
        'totalCount': 0,
        'totalPages': 0,
      };
      final documentAdapter = RecordingAdapter(body: pageBody);
      await _client(
        documentAdapter,
      ).listExpenseDocuments('expense-1', page: 1, pageSize: 20);
      expect(
        documentAdapter.lastRequest!.path,
        '/api/v1/expenses/expense-1/attachments',
      );

      final historyAdapter = RecordingAdapter(body: pageBody);
      await _client(
        historyAdapter,
      ).listExpenseHistory('expense-1', page: 1, pageSize: 20);
      expect(
        historyAdapter.lastRequest!.path,
        '/api/v1/expenses/expense-1/history',
      );

      final reimbursementAdapter = RecordingAdapter(body: pageBody);
      await _client(reimbursementAdapter).listReimbursements(
        page: 1,
        pageSize: 20,
        status: 'Open',
        claimantUserId: 'user-1',
      );
      expect(reimbursementAdapter.lastRequest!.path, '/api/v1/reimbursements');
      expect(
        reimbursementAdapter.lastRequest!.headers,
        isNot(contains('Idempotency-Key')),
      );
    },
  );

  test('financial decoder preserves maximum expense decimal token', () async {
    final body = jsonEncode({
      'items': [_summary],
      'page': 1,
      'pageSize': 20,
      'totalCount': 1,
      'totalPages': 1,
    }).replaceFirst('25.2', '9999999999999999.99');
    final page = await _client(
      _RawAdapter(body),
    ).listExpenses(page: 1, pageSize: 20, filters: const ExpenseFilters());
    expect(page.items.single.totalAmount, '9999999999999999.99');
  });
}

ApiClient _client(HttpClientAdapter adapter) {
  final dio = Dio(
    BaseOptions(
      baseUrl: 'https://api.example.test',
      responseType: ResponseType.json,
    ),
  )..httpClientAdapter = adapter;
  return ApiClient(dio);
}

final class _RawAdapter implements HttpClientAdapter {
  const _RawAdapter(this.body);
  final String body;
  @override
  Future<ResponseBody> fetch(
    RequestOptions options,
    Stream<Uint8List>? requestStream,
    Future<void>? cancelFuture,
  ) async => ResponseBody.fromString(
    body,
    200,
    headers: {
      Headers.contentTypeHeader: [Headers.jsonContentType],
    },
  );
  @override
  void close({bool force = false}) {}
}
