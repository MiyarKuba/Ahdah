import 'dart:io';

import 'package:ahdah_app/app/routing/app_routes.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  test('stable expense routes expose only supported workflows', () {
    expect(AppRoutes.expensesPath, '/expenses');
    expect(AppRoutes.expenseCreatePath, '/expenses/new');
    expect(AppRoutes.expenseCategoriesPath, '/expense-categories');
    expect(AppRoutes.reimbursementsPath, '/reimbursements');
  });

  test('expense creation offers no supplier credit or mixed payment', () {
    final source = _read(
      'lib/features/expenses/presentation/create/expense_create_page.dart',
    );
    expect(source, isNot(contains('SupplierCredit')));
    expect(source, isNot(contains('MixedPayment')));
    expect(source, isNot(contains('SplitPayment')));
    expect(source, contains("'AdvanceBalance'"));
    expect(source, contains("'PersonalFunds'"));
  });

  test('expense feature adds no binary picker or custody transition', () {
    final source = _expenseSources();
    expect(source, isNot(contains('image_picker')));
    expect(source, isNot(contains('file_picker')));
    expect(source, isNot(contains('original-status')));
    expect(source, isNot(contains('paperCustody')));
    expect(source, isNot(contains('FileUpload(')));
  });

  test('financial request source uses decimal strings and no double', () {
    final source = _read('lib/features/expenses/domain/expense_requests.dart');
    expect(source, isNot(contains('double')));
    expect(source, contains('String amount'));
    expect(source, isNot(contains('companyId')));
    expect(source, isNot(contains('availableAmount')));
  });

  test(
    'idempotency keys are not persisted or displayed by expense feature',
    () {
      final source = _expenseSources();
      expect(source, isNot(contains('SharedPreferences')));
      expect(source, isNot(contains('secure_storage')));
      expect(source, isNot(contains('print(')));
      expect(source, isNot(contains('Text(idempotencyKey')));
    },
  );

  test('no unsupported financial command route is introduced', () {
    final endpoints = _read('lib/core/network/api_endpoints.dart');
    expect(endpoints, isNot(contains('supplier-debts')));
    expect(endpoints, isNot(contains('settlements')));
    expect(endpoints, isNot(contains('claim-payment')));
    expect(endpoints, isNot(contains('original-status')));
  });
}

String _expenseSources() {
  final directory = Directory('lib/features/expenses');
  return directory
      .listSync(recursive: true)
      .whereType<File>()
      .where((file) => file.path.endsWith('.dart'))
      .map((file) => file.readAsStringSync())
      .join('\n');
}

String _read(String path) => File(path).readAsStringSync();
