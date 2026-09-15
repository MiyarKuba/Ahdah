import 'package:ahdah_app/core/network/api_client.dart';
import 'package:ahdah_app/features/suppliers/domain/supplier_models.dart';
import 'package:ahdah_app/features/suppliers/domain/supplier_requests.dart';
import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';

import 'helpers/fakes.dart';

const _supplier = {
  'supplierId': 'supplier-1',
  'supplierCode': 'SUP-1',
  'supplierName': 'Supplier One',
  'supplierType': 'FutureSupplierType',
  'contactPersonName': 'Contact',
  'phoneNumber': '+218910000000',
  'email': 'supplier@example.test',
  'city': 'Tripoli',
  'defaultCurrencyCode': 'LYD',
  'transactionMode': 'CashAndCredit',
  'defaultPaymentTermsDays': 30,
  'creditLimit': 1000,
  'preferredPaymentMethod': 'Cash',
  'isActive': true,
  'versionNumber': 2,
  'createdAtUtc': '2026-08-04T10:00:00Z',
  'updatedAtUtc': '2026-08-04T10:00:00Z',
};
const _invoice = {
  'supplierDebtId': 'debt-1',
  'debtNumber': 'SDEBT-1',
  'expenseId': 'expense-1',
  'expenseNumber': 'EXP-1',
  'invoiceNumber': 'INV-1',
  'supplier': _supplier,
  'project': {'projectId': 'project-1', 'projectName': 'Project One'},
  'invoiceDate': '2026-08-04',
  'dueDate': '2026-09-04',
  'amount': 100,
  'paidAmount': 20,
  'creditNoteAmount': 5,
  'writtenOffAmount': 0,
  'outstandingAmount': 75,
  'currencyCode': 'LYD',
  'expenseStatus': 'Approved',
  'debtStatus': 'FutureDebtStatus',
  'description': 'Materials',
  'expenseVersionNumber': 2,
  'debtVersionNumber': 3,
  'createdAtUtc': '2026-08-04T10:00:00Z',
};
const _payment = {
  'supplierPaymentId': 'payment-1',
  'paymentNumber': 'SPAY-1',
  'supplierId': 'supplier-1',
  'supplierName': 'Supplier One',
  'paymentDate': '2026-08-04',
  'paymentAmount': 75,
  'currencyCode': 'LYD',
  'paymentMethod': 'Cash',
  'referenceNumber': null,
  'hasProof': false,
  'status': 'PendingApproval',
  'createdBy': {'userId': 'user-1', 'fullName': 'Manager', 'role': 'Manager'},
  'confirmedBy': null,
  'versionNumber': 1,
  'createdAtUtc': '2026-08-04T10:00:00Z',
  'confirmedAtUtc': null,
};
const _credit = {
  'supplierCreditNoteId': 'credit-1',
  'creditNoteNumber': 'SCN-1',
  'supplierId': 'supplier-1',
  'supplierName': 'Supplier One',
  'supplierReferenceNumber': null,
  'creditNoteDate': '2026-08-04',
  'creditNoteAmount': 10,
  'appliedAmount': 2,
  'availableAmount': 8,
  'currencyCode': 'LYD',
  'reasonType': 'AdditionalDiscount',
  'description': 'Discount',
  'status': 'Approved',
  'versionNumber': 2,
  'createdAtUtc': '2026-08-04T10:00:00Z',
};

void main() {
  test('supplier detail preserves decimals, unknown enums, and UTC dates', () {
    final details = SupplierDetails.fromJson(const {
      'supplier': _supplier,
      'secondaryPhoneNumber': null,
      'commercialRegistrationNumber': null,
      'taxRegistrationNumber': null,
      'address': 'Tripoli',
      'notes': null,
      'balances': [
        {'currencyCode': 'LYD', 'outstandingAmount': 75, 'openDebtCount': 1},
      ],
    });
    expect(details.supplier.supplierType, 'FutureSupplierType');
    expect(details.supplier.creditLimit, '1000.00');
    expect(details.balances.single.outstandingAmount, '75.00');
    expect(details.supplier.createdAtUtc, DateTime.utc(2026, 8, 4, 10));
  });

  test(
    'invoice detail parses authoritative debt amounts and 3-decimal quantity',
    () {
      final details = SupplierInvoiceDetails.fromJson(const {
        'invoice': _invoice,
        'adjustmentAmount': 0,
        'notes': null,
        'items': [
          {
            'expenseItemId': 'item-1',
            'lineNumber': 1,
            'itemName': 'Cement',
            'itemCode': null,
            'itemDescription': null,
            'quantity': 1.250,
            'unitCode': 'Bag',
            'customUnitName': null,
            'unitPrice': 80,
            'subtotalAmount': 100,
            'discountAmount': 0,
            'taxAmount': 0,
            'totalAmount': 100,
            'notes': null,
          },
        ],
      });
      expect(details.invoice.outstandingAmount, '75.00');
      expect(details.invoice.debtStatus, 'FutureDebtStatus');
      expect(details.items.single.quantity, '1.25');
      expect(details.items.single.unitPrice, '80.00');
    },
  );

  test('payment, funding, credit, refund, and statement projections parse', () {
    final payment = SupplierPaymentDetails.fromJson(const {
      'payment': _payment,
      'supplierPaymentAccountId': null,
      'payerBankName': null,
      'description': null,
      'notes': null,
      'debtAllocations': [
        {
          'supplierPaymentDebtAllocationId': 'allocation-1',
          'supplierDebtId': 'debt-1',
          'debtNumber': 'SDEBT-1',
          'allocatedAmount': 75,
          'createdAtUtc': '2026-08-04T10:00:00Z',
        },
      ],
      'fundingSources': [
        {
          'supplierPaymentFundingSourceId': 'funding-allocation-1',
          'fundingSourceId': 'funding-1',
          'sourceType': 'ManagerContribution',
          'allocatedAmount': 75,
          'createdAtUtc': '2026-08-04T10:00:00Z',
        },
      ],
    });
    final credit = SupplierCreditNoteDetails.fromJson(const {
      'creditNote': _credit,
      'notes': null,
      'allocations': [],
    });
    final refund = SupplierRefundSummary.fromJson(const {
      'supplierRefundId': 'refund-1',
      'refundNumber': 'SRF-1',
      'supplierId': 'supplier-1',
      'supplierName': 'Supplier One',
      'expenseReturnId': 'return-1',
      'refundDate': '2026-08-04',
      'refundAmount': 10,
      'feeAmount': 0,
      'netReceivedAmount': 10,
      'currencyCode': 'LYD',
      'refundMethod': 'Cash',
      'supplierReferenceNumber': null,
      'transactionReferenceNumber': null,
      'hasProof': false,
      'description': 'Return',
      'status': 'Confirmed',
      'versionNumber': 1,
      'createdAtUtc': '2026-08-04T10:00:00Z',
    });
    final entry = SupplierStatementEntry.fromJson(const {
      'eventId': 'event-1',
      'eventType': 'PaymentApplied',
      'reference': 'SPAY-1',
      'eventDate': '2026-08-04',
      'amount': 75,
      'currencyCode': 'LYD',
      'status': 'Confirmed',
      'description': 'Payment applied',
      'occurredAtUtc': '2026-08-04T10:00:00Z',
    });
    expect(payment.debtAllocations.single.allocatedAmount, '75.00');
    expect(payment.fundingSources.single.sourceType, 'ManagerContribution');
    expect(credit.creditNote.availableAmount, '8.00');
    expect(refund.feeAmount, '0.00');
    expect(entry.eventType, 'PaymentApplied');
  });

  test('money and quantity validators reject malformed precision exactly', () {
    expect(DecimalQuantity.canonicalize('0.001'), '0.001');
    expect(DecimalQuantity.canonicalize('1'), '1.000');
    expect(DecimalQuantity.canonicalize('1.250'), '1.250');
    expect(DecimalQuantity.canonicalize('1.0001'), isNull);
    expect(DecimalQuantity.canonicalize('1,25'), isNull);
  });

  test('invoice request emits numeric decimal tokens and exact item total', () {
    const item = SupplierInvoiceItemInput(
      itemName: 'Cement',
      quantity: '2.500',
      unitCode: 'Bag',
      unitPrice: '10.00',
      discountAmount: '1.00',
      taxAmount: '1.00',
    );
    final input = SupplierInvoiceCreateInput(
      supplierId: 'supplier-1',
      expenseCategoryId: 'category-1',
      invoiceDate: DateTime(2026, 8, 4),
      dueDate: DateTime(2026, 9, 4),
      amount: '25.00',
      currencyCode: 'lyd',
      description: 'Materials',
      items: const [item],
    );
    expect(input.itemsMatchAmount, isTrue);
    expect(input.toJsonBody(), contains('"quantity":2.500'));
    expect(input.toJsonBody(), contains('"amount":25.00'));
    expect(input.toJsonBody(), isNot(contains('companyId')));
  });

  test('payment and credit allocations use exact minor-unit totals', () {
    final payment = SupplierPaymentCreateInput(
      supplierId: 'supplier-1',
      paymentDate: DateTime(2026, 8, 4),
      paymentAmount: '100.00',
      currencyCode: 'LYD',
      paymentMethod: 'Cash',
      debtAllocations: const [
        SupplierPaymentDebtAllocationInput(
          supplierDebtId: 'debt-1',
          amount: '40.00',
        ),
        SupplierPaymentDebtAllocationInput(
          supplierDebtId: 'debt-2',
          amount: '60.00',
        ),
      ],
      fundingAllocations: const [
        SupplierPaymentFundingAllocationInput(
          fundingSourceId: 'source-1',
          amount: '99.99',
        ),
        SupplierPaymentFundingAllocationInput(
          fundingSourceId: 'source-2',
          amount: '0.01',
        ),
      ],
    );
    expect(payment.allocationsMatch, isTrue);
    final mismatch = SupplierPaymentCreateInput(
      supplierId: payment.supplierId,
      paymentDate: payment.paymentDate,
      paymentAmount: payment.paymentAmount,
      currencyCode: payment.currencyCode,
      paymentMethod: payment.paymentMethod,
      debtAllocations: payment.debtAllocations,
      fundingAllocations: const [
        SupplierPaymentFundingAllocationInput(
          fundingSourceId: 'source-1',
          amount: '99.99',
        ),
      ],
    );
    expect(mismatch.allocationsMatch, isFalse);
    const credit = SupplierCreditApplyInput(
      expectedVersion: 1,
      allocations: [
        SupplierCreditAllocationInput(
          supplierDebtId: 'debt-1',
          amount: '10.00',
        ),
      ],
    );
    expect(credit.totalWithin('10.00'), isTrue);
    expect(credit.totalWithin('9.99'), isFalse);
  });

  test(
    'supplier list and funding-source reads use exact routes without idempotency',
    () async {
      final listAdapter = RecordingAdapter(
        body: const {
          'items': [_supplier],
          'page': 1,
          'pageSize': 20,
          'totalCount': 1,
          'totalPages': 1,
        },
      );
      await _client(listAdapter).listSuppliers(
        page: 1,
        pageSize: 20,
        filters: const SupplierFilters(
          isActive: true,
          supplierType: 'GeneralSupplier',
          search: 'Su',
        ),
      );
      expect(listAdapter.lastRequest!.path, '/api/v1/suppliers');
      expect(
        listAdapter.lastRequest!.headers,
        isNot(contains('Idempotency-Key')),
      );
      expect(
        listAdapter.lastRequest!.queryParameters,
        isNot(contains('companyId')),
      );

      final fundingAdapter = RecordingAdapter(
        body: const {
          'items': [],
          'page': 1,
          'pageSize': 20,
          'totalCount': 0,
          'totalPages': 0,
        },
      );
      await _client(fundingAdapter).listSupplierFundingSources(
        page: 1,
        pageSize: 20,
        currencyCode: 'LYD',
        paymentMethod: 'Cash',
      );
      expect(
        fundingAdapter.lastRequest!.path,
        '/api/v1/supplier-payments/funding-sources',
      );
      expect(
        fundingAdapter.lastRequest!.queryParameters['currencyCode'],
        'LYD',
      );
    },
  );

  test(
    'every supplier financial command sends idempotency header on exact route',
    () async {
      const key = 'secure-key-123456789';
      final invoiceAdapter = RecordingAdapter(
        statusCode: 201,
        body: const {
          'invoice': _invoice,
          'adjustmentAmount': 0,
          'notes': null,
          'items': [],
        },
      );
      await _client(invoiceAdapter).createSupplierInvoice(
        SupplierInvoiceCreateInput(
          supplierId: 'supplier-1',
          expenseCategoryId: 'category-1',
          invoiceDate: DateTime(2026, 8, 4),
          dueDate: DateTime(2026, 9, 4),
          amount: '100.00',
          currencyCode: 'LYD',
          description: 'Materials',
        ),
        key,
      );
      expect(invoiceAdapter.lastRequest!.path, '/api/v1/supplier-invoices');
      expect(invoiceAdapter.lastRequest!.headers['Idempotency-Key'], key);

      final paymentBody = const {
        'payment': _payment,
        'supplierPaymentAccountId': null,
        'payerBankName': null,
        'description': null,
        'notes': null,
        'debtAllocations': [],
        'fundingSources': [],
      };
      final confirmAdapter = RecordingAdapter(body: paymentBody);
      await _client(confirmAdapter).confirmSupplierPayment(
        'payment-1',
        const SupplierPaymentReviewInput(expectedVersion: 1),
        key,
      );
      expect(
        confirmAdapter.lastRequest!.path,
        '/api/v1/supplier-payments/payment-1/confirm',
      );
      expect(confirmAdapter.lastRequest!.headers['Idempotency-Key'], key);

      final creditAdapter = RecordingAdapter(
        body: const {'creditNote': _credit, 'notes': null, 'allocations': []},
      );
      await _client(creditAdapter).applySupplierCreditNote(
        'credit-1',
        const SupplierCreditApplyInput(
          expectedVersion: 2,
          allocations: [
            SupplierCreditAllocationInput(
              supplierDebtId: 'debt-1',
              amount: '8.00',
            ),
          ],
        ),
        key,
      );
      expect(
        creditAdapter.lastRequest!.path,
        '/api/v1/supplier-credit-notes/credit-1/allocations',
      );
      expect(creditAdapter.lastRequest!.headers['Idempotency-Key'], key);
    },
  );
}

ApiClient _client(HttpClientAdapter adapter) {
  final dio = Dio(BaseOptions(baseUrl: 'https://api.example.test'))
    ..httpClientAdapter = adapter;
  return ApiClient(dio);
}
