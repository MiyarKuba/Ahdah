import 'dart:async';
import 'dart:convert';
import 'dart:typed_data';

import 'package:ahdah_app/core/network/api_client.dart';
import 'package:ahdah_app/core/network/auth_interceptor.dart';
import 'package:ahdah_app/features/advances/domain/advance_models.dart';
import 'package:ahdah_app/features/advances/domain/advance_requests.dart';
import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';

import 'helpers/fakes.dart';

const _user = {'userId': 'user-1', 'fullName': 'User One', 'role': 'Deputy'};

const _summary = {
  'advanceId': 'advance-1',
  'advanceNumber': 'ADV-ONE',
  'advanceAmount': 100.2,
  'availableAmount': 80,
  'reservedAmount': 20.20,
  'currencyCode': 'LYD',
  'issueDate': '2026-08-04',
  'settlementDueDate': null,
  'purpose': 'Custody',
  'notes': null,
  'status': 'FutureStatus',
  'recipient': _user,
  'versionNumber': 2,
  'createdAtUtc': '2026-08-04T09:00:00Z',
  'confirmedAtUtc': null,
};

const _transfer = {
  'transferId': 'transfer-1',
  'transferNumber': 'TRF-ONE',
  'transferType': 'InternalTransfer',
  'advanceId': 'advance-1',
  'amount': 10.25,
  'currencyCode': 'LYD',
  'sender': _user,
  'recipient': _user,
  'transferMethod': 'FutureMethod',
  'status': 'PendingConfirmation',
  'versionNumber': 1,
  'createdAtUtc': '2026-08-04T10:00:00Z',
  'confirmedAtUtc': null,
  'rejectedAtUtc': null,
};

void main() {
  test(
    'advance list preserves decimal text, UTC dates, and unknown values',
    () {
      final page = AdvancePage.fromJson(const {
        'items': [_summary],
        'page': 1,
        'pageSize': 20,
        'totalCount': 1,
        'totalPages': 1,
      });

      expect(page.items.single.advanceAmount, '100.20');
      expect(page.items.single.availableAmount, '80.00');
      expect(page.items.single.status, 'FutureStatus');
      expect(page.items.single.createdAtUtc, DateTime.utc(2026, 8, 4, 9));
    },
  );

  test(
    'detail, funding, balance, and movement parse exact safe DTO fields',
    () {
      final details = AdvanceDetails.fromJson(const {
        'advance': _summary,
        'fundings': [
          {
            'sourceType': 'CompanyCashbox',
            'allocatedAmount': 100,
            'fundingStatus': 'Available',
            'paymentMethods': ['Cash'],
            'allocatedAtUtc': '2026-08-04T09:00:00Z',
          },
        ],
        'balances': [
          {
            'advanceId': 'advance-1',
            'advanceNumber': 'ADV-ONE',
            'holder': _user,
            'totalReceivedAmount': 100,
            'totalRestoredAmount': 0,
            'totalExpensedAmount': 0,
            'totalTransferredOutAmount': 0,
            'totalReturnedAmount': 0,
            'availableAmount': 80,
            'reservedAmount': 20,
            'currencyCode': 'LYD',
            'status': 'Active',
            'versionNumber': 2,
            'updatedAtUtc': '2026-08-04T10:00:00Z',
          },
        ],
        'closureStatus': null,
      });
      final movement = AdvanceMovement.fromJson(const {
        'movementId': 'transfer-1',
        'operationType': 'InternalTransfer',
        'amount': 10,
        'actor': _user,
        'sender': _user,
        'recipient': _user,
        'status': 'PendingConfirmation',
        'occurredAtUtc': '2026-08-04T10:00:00Z',
      });

      expect(details.fundings.single.paymentMethods, ['Cash']);
      expect(details.balances.single.totalExpensedAmount, '0.00');
      expect(details.balanceFor('user-1'), isNotNull);
      expect(movement.canBeRejected, isTrue);
    },
  );

  test('no project relationship is invented in advance models', () {
    final details = AdvanceDetails.fromJson(const {
      'advance': _summary,
      'fundings': [],
      'balances': [],
      'projectId': 'must-be-ignored',
    });
    expect(details.advance.advanceId, 'advance-1');
    expect(details.toString(), isNot(contains('projectId')));
  });

  test(
    'financial response decoder preserves maximum decimal token text',
    () async {
      final adapter = _RawAdapter(
        '{"items":[${jsonEncode(_summary).replaceFirst('100.2', '9999999999999999.99')}],'
        '"page":1,"pageSize":20,"totalCount":1,"totalPages":1}',
      );
      final result = await _client(adapter).listAdvances(page: 1, pageSize: 20);
      expect(result.items.single.advanceAmount, '9999999999999999.99');
    },
  );

  test(
    'advance reads send exact routes, filters, and no idempotency header',
    () async {
      final adapter = RecordingAdapter(
        body: const {
          'items': [],
          'page': 2,
          'pageSize': 20,
          'totalCount': 0,
          'totalPages': 0,
        },
      );
      await _client(adapter).listAdvances(
        page: 2,
        pageSize: 20,
        status: 'Open',
        userId: 'user-1',
        reference: 'ADV',
      );

      expect(adapter.lastRequest!.path, '/api/v1/advances');
      expect(adapter.lastRequest!.queryParameters, {
        'page': 2,
        'pageSize': 20,
        'status': 'Open',
        'userId': 'user-1',
        'reference': 'ADV',
      });
      expect(adapter.lastRequest!.headers, isNot(contains('Idempotency-Key')));
    },
  );

  test('detail, movement, funding, and balance routes are exact', () async {
    final detailAdapter = RecordingAdapter(
      body: const {'advance': _summary, 'fundings': [], 'balances': []},
    );
    await _client(detailAdapter).getAdvance('advance-1');
    expect(detailAdapter.lastRequest!.path, '/api/v1/advances/advance-1');

    final movementAdapter = RecordingAdapter(
      body: const {
        'items': [],
        'page': 1,
        'pageSize': 20,
        'totalCount': 0,
        'totalPages': 0,
      },
    );
    await _client(
      movementAdapter,
    ).listAdvanceMovements('advance-1', page: 1, pageSize: 20);
    expect(
      movementAdapter.lastRequest!.path,
      '/api/v1/advances/advance-1/movements',
    );

    final fundingAdapter = RecordingAdapter(body: movementAdapter.body);
    await _client(
      fundingAdapter,
    ).listAdvanceFundingSources(page: 1, pageSize: 20);
    expect(fundingAdapter.lastRequest!.path, '/api/v1/advance-funding-sources');

    final balanceAdapter = RecordingAdapter(
      body: const {
        'page': {
          'items': [],
          'page': 1,
          'pageSize': 20,
          'totalCount': 0,
          'totalPages': 0,
        },
        'totalAvailableAmount': 0,
      },
    );
    await _client(balanceAdapter).getMyAdvanceBalances(page: 1, pageSize: 20);
    expect(balanceAdapter.lastRequest!.path, '/api/v1/advance-balances/me');
    await _client(
      balanceAdapter,
    ).getUserAdvanceBalances('user-1', page: 1, pageSize: 20);
    expect(
      balanceAdapter.lastRequest!.path,
      '/api/v1/advance-balances/users/user-1',
    );
  });

  test(
    'creation sends exact numeric fields and ephemeral command header',
    () async {
      final adapter = RecordingAdapter(
        statusCode: 201,
        body: const {'advance': _summary, 'fundings': [], 'balances': []},
      );
      final input = CreateAdvanceInput(
        recipientUserId: 'user-1',
        issueDate: DateTime(2026, 8, 4),
        purpose: 'Custody',
        fundings: const [
          FundingAllocationInput(fundingSourceId: 'source-1', amount: '100.00'),
        ],
        transfer: AdvanceTransferFields(
          amount: '100.00',
          transferDate: DateTime(2026, 8, 4),
          transferMethod: 'Cash',
        ),
      );
      await _client(
        adapter,
      ).createAdvance(input, 'secure-operation-key-123456');
      final body = adapter.lastRequest!.data as String;

      expect(body, contains('"amount":100.00'));
      expect(
        body,
        contains('"fundings":[{"fundingSourceId":"source-1","amount":100.00'),
      );
      expect(body, isNot(contains('companyId')));
      expect(body, isNot(contains('projectId')));
      expect(body, isNot(contains('balance')));
      expect(
        adapter.lastRequest!.headers['Idempotency-Key'],
        'secure-operation-key-123456',
      );
    },
  );

  test(
    'distribution, return, confirmation, and rejection contracts are exact',
    () async {
      final transferFields = AdvanceTransferFields(
        amount: '10.00',
        transferDate: DateTime(2026, 8, 4),
        transferMethod: 'Cash',
      );
      final distributionAdapter = RecordingAdapter(
        statusCode: 201,
        body: _transfer,
      );
      await _client(distributionAdapter).distributeAdvance(
        'advance-1',
        CreateAdvanceDistributionInput(
          recipientUserId: 'user-2',
          transfer: transferFields,
        ),
        'secure-operation-key-123456',
      );
      expect(
        distributionAdapter.lastRequest!.path,
        '/api/v1/advances/advance-1/distributions',
      );
      expect(
        distributionAdapter.lastRequest!.data,
        contains('"recipientUserId":"user-2"'),
      );

      final returnAdapter = RecordingAdapter(statusCode: 201, body: _transfer);
      await _client(returnAdapter).returnAdvanceMoney(
        'advance-1',
        CreateAdvanceReturnInput(transfer: transferFields),
        'secure-operation-key-123456',
      );
      expect(
        returnAdapter.lastRequest!.path,
        '/api/v1/advances/advance-1/returns',
      );
      expect(
        returnAdapter.lastRequest!.data,
        isNot(contains('recipientUserId')),
      );

      final confirmAdapter = RecordingAdapter(body: _transfer);
      await _client(
        confirmAdapter,
      ).confirmAdvanceTransfer('transfer-1', 'secure-operation-key-123456');
      expect(
        confirmAdapter.lastRequest!.path,
        '/api/v1/advance-transfers/transfer-1/confirm',
      );
      expect(confirmAdapter.lastRequest!.data, isNull);

      final rejectAdapter = RecordingAdapter(body: _transfer);
      await _client(rejectAdapter).rejectAdvanceTransfer(
        'transfer-1',
        const RejectAdvanceTransferInput(reason: 'Not received'),
        'secure-operation-key-123456',
      );
      expect(
        rejectAdapter.lastRequest!.path,
        '/api/v1/advance-transfers/transfer-1/reject',
      );
      expect(rejectAdapter.lastRequest!.data, {'reason': 'Not received'});
    },
  );
}

ApiClient _client(HttpClientAdapter adapter) {
  final dio = Dio(BaseOptions(baseUrl: 'http://localhost:5231'))
    ..httpClientAdapter = adapter
    ..interceptors.add(AuthInterceptor(FakeAccessTokenStore('test-token')));
  return ApiClient(dio);
}

final class _RawAdapter implements HttpClientAdapter {
  _RawAdapter(this.body);
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
