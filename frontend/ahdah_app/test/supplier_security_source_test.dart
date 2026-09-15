import 'dart:io';

import 'package:flutter_test/flutter_test.dart';

void main() {
  final feature = Directory('lib/features/suppliers');
  final dartFiles = feature
      .listSync(recursive: true)
      .whereType<File>()
      .where((file) => file.path.endsWith('.dart'))
      .toList();

  test(
    'supplier feature contains no double financial arithmetic or tenant fields',
    () {
      final source = dartFiles
          .map((file) => file.readAsStringSync())
          .join('\n');
      expect(RegExp(r'\bdouble\b').hasMatch(source), isFalse);
      expect(source.contains("'companyId'"), isFalse);
      expect(source.contains("'company_id'"), isFalse);
      expect(source.contains('authoritativeBalance'), isFalse);
    },
  );

  test('unsupported supplier writes and advance funding are absent', () {
    final client = File('lib/core/network/api_client.dart').readAsStringSync();
    final endpoints = File(
      'lib/core/network/api_endpoints.dart',
    ).readAsStringSync();
    expect(client.contains('createSupplierRefund'), isFalse);
    expect(client.contains('verifySupplierRefund'), isFalse);
    expect(client.contains('cancelSupplierPayment'), isFalse);
    expect(client.contains('reverseSupplierPayment'), isFalse);
    expect(client.contains('advanceBalanceId'), isFalse);
    expect(endpoints.contains('supplier-refunds/verify'), isFalse);
  });

  test('idempotency values are neither persisted nor displayed', () {
    final source = dartFiles.map((file) => file.readAsStringSync()).join('\n');
    expect(source.contains('SharedPreferences'), isFalse);
    expect(source.contains('SecureStorage'), isFalse);
    expect(source.contains('print('), isFalse);
    expect(source.contains('debugPrint('), isFalse);
    expect(source.contains('Idempotency-Key'), isFalse);
  });

  test(
    'supplier financial writes use explicit key and have no Dio retry policy',
    () {
      final client = File(
        'lib/core/network/api_client.dart',
      ).readAsStringSync();
      expect(client.contains('createSupplierInvoice'), isTrue);
      expect(client.contains('createSupplierPayment'), isTrue);
      expect(client.contains('createSupplierCreditNote'), isTrue);
      expect(
        client.contains("headers: {'Idempotency-Key': idempotencyKey}"),
        isTrue,
      );
      expect(client.contains('RetryInterceptor'), isFalse);
    },
  );

  test('no schema migration or generated persistence file was added', () {
    final root = Directory('../../backend');
    final changedShapeFiles = root.existsSync()
        ? root
              .listSync(recursive: true)
              .whereType<File>()
              .where(
                (file) =>
                    file.path.contains('Migrations') ||
                    file.path.endsWith('.sql'),
              )
              .toList()
        : const <File>[];
    expect(changedShapeFiles, isEmpty);
  });
}
