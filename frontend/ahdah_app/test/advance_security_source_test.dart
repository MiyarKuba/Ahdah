import 'dart:io';

import 'package:ahdah_app/features/advances/domain/advance_requests.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  test(
    'financial request contracts accept no tenant, project, or balance input',
    () {
      final create = CreateAdvanceInput(
        recipientUserId: 'user-1',
        issueDate: DateTime(2026, 8, 4),
        purpose: 'Custody',
        fundings: const [
          FundingAllocationInput(fundingSourceId: 'source-1', amount: '10.00'),
        ],
        transfer: AdvanceTransferFields(
          amount: '10.00',
          transferDate: DateTime(2026, 8, 4),
          transferMethod: 'Cash',
        ),
      ).toJsonBody();

      for (final forbidden in [
        'company_id',
        'companyId',
        'projectId',
        'availableAmount',
        'reservedAmount',
        'balance',
        'createdByUserId',
        'expectedVersion',
      ]) {
        expect(create, isNot(contains(forbidden)));
      }
    },
  );

  test('advance money requests contain no binary floating-point parsing', () {
    final source = _sources('lib/features/advances');
    expect(source, isNot(contains('double.parse')));
    expect(source, isNot(contains('double.tryParse')));
    expect(source, isNot(contains('toDouble()')));
  });

  test('idempotency keys are not persisted, displayed, or logged', () {
    final source = _sources('lib/features/advances');
    expect(source, isNot(contains('SharedPreferences')));
    expect(source, isNot(contains('flutter_secure_storage')));
    expect(source, isNot(contains('localStorage')));
    expect(source, isNot(contains('print(')));
    expect(source, isNot(contains('debugPrint(')));
  });

  test(
    'still-deferred financial workflows and advance project link are absent',
    () {
      final endpointSource = File(
        'lib/core/network/api_endpoints.dart',
      ).readAsStringSync();
      for (final endpoint in [
        '/receipts',
        '/settlements',
        '/advance-closures',
        '/claims',
      ]) {
        expect(endpointSource, isNot(contains(endpoint)));
      }
      final requests = File(
        'lib/features/advances/domain/advance_requests.dart',
      ).readAsStringSync();
      expect(requests, isNot(contains('projectId')));
      expect(requests, isNot(contains('returnRecipient')));
    },
  );

  test('financial command layer contains no automatic retry mechanism', () {
    final client = File('lib/core/network/api_client.dart').readAsStringSync();
    expect(client, isNot(contains('RetryInterceptor')));
    expect(client, isNot(contains('dio_smart_retry')));
    expect(client, isNot(contains('retryWhen')));
  });
}

String _sources(String path) => Directory(path)
    .listSync(recursive: true)
    .whereType<File>()
    .where((file) => file.path.endsWith('.dart'))
    .map((file) => file.readAsStringSync())
    .join('\n');
