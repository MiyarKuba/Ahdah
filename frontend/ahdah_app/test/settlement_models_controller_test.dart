import 'dart:async';
import 'dart:convert';
import 'dart:typed_data';
import 'package:ahdah_app/core/errors/app_exception.dart';
import 'package:ahdah_app/core/network/api_client.dart';
import 'package:ahdah_app/core/network/auth_interceptor.dart';
import 'package:ahdah_app/features/access/domain/role_capabilities.dart';
import 'package:ahdah_app/features/settlements/data/api_settlement_repository.dart';
import 'package:ahdah_app/features/settlements/domain/settlement_models.dart';
import 'package:ahdah_app/features/settlements/presentation/controllers/settlement_controller.dart';
import 'package:ahdah_app/features/settlements/presentation/settlement_navigation.dart';
import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'helpers/settlement_fakes.dart';

void main() {
  test(
    'full immutable contract preserves nullable booleans, exact money and references',
    () {
      final data = settlementJson()..['categories'] = [settlementCategory()];
      final value = ProjectSettlement.fromJson(data);
      expect(value.canSettle, isNull);
      expect(value.canClose, isNull);
      expect(value.projectVersion, 7);
      expect(value.evaluatedAt, DateTime.utc(2026, 9, 16, 10, 15));
      expect(value.visibilityScope, 'CompanyFinancial');
      expect(value.categories.single.totals.last.amount, '300.09');
      expect(
        value.categories.single.blockers.single.resourcePath,
        '/api/v1/supplier-debts/debt-1',
      );
      expect(
        value.evaluationGaps.single.code,
        'ProjectAdvanceAttributionUnavailable',
      );
      expect(() => value.categories.clear(), throwsUnsupportedError);
      expect(
        () => value.categories.single.blockers.clear(),
        throwsUnsupportedError,
      );
    },
  );
  for (final flag in [false, true, null]) {
    test('readiness boolean $flag is preserved without coercion', () {
      final value = ProjectSettlement.fromJson(
        settlementJson()
          ..['canSettle'] = flag
          ..['canClose'] = flag,
      );
      expect(value.canSettle, flag);
      expect(value.canClose, flag);
    });
  }
  test('unknown values and optional absence remain safe model data', () {
    final row = settlementBlocker()
      ..['amount'] = null
      ..['currencyCode'] = null
      ..['resourcePath'] = null
      ..['code'] = 'FutureReason';
    final data = settlementJson()
      ..['visibilityScope'] = 'FutureScope'
      ..['categories'] = [
        settlementCategory(
          name: 'FutureCategory',
          evaluation: 'FutureEvaluation',
        )..['blockers'] = [row],
      ];
    final model = ProjectSettlement.fromJson(data);
    expect(model.categories.single.category, 'FutureCategory');
    expect(model.categories.single.blockers.single.amount, isNull);
    expect(model.categories.single.blockers.single.code, 'FutureReason');
  });
  test(
    'binary floating amounts and malformed decimal strings are rejected',
    () {
      for (final amount in [1.23, '1.234', 'NaN', '1e3']) {
        expect(
          () => SettlementCurrencyTotal.fromJson({
            'currencyCode': 'LYD',
            'amount': amount,
          }),
          throwsFormatException,
        );
      }
      expect(
        SettlementCurrencyTotal.fromJson({
          'currencyCode': 'LYD',
          'amount': 10,
        }).amount,
        '10.00',
      );
    },
  );
  test(
    'repository protects nested decimal JSON tokens and sends only authenticated GET',
    () async {
      final json = settlementJson()..['categories'] = [settlementCategory()];
      final raw = jsonEncode(json).replaceAll('"8500.01"', '90071992547409.91');
      final adapter = _RawAdapter(raw);
      final dio = Dio(BaseOptions(baseUrl: 'https://example.test'))
        ..httpClientAdapter = adapter;
      final result = await ApiSettlementRepository(
        ApiClient(dio),
      ).getProjectSettlement('project-1');
      expect(result.categories.single.totals.first.amount, '90071992547409.91');
      expect(
        result.categories.single.blockers.single.amount,
        '90071992547409.91',
      );
      expect(adapter.request!.path, '/api/v1/projects/project-1/settlement');
      expect(adapter.request!.method, 'GET');
      expect(adapter.request!.data, isNull);
      expect(adapter.request!.queryParameters, isEmpty);
      expect(adapter.request!.extra[requiresAuthenticationKey], isTrue);
      expect(adapter.request!.headers.containsKey('Idempotency-Key'), isFalse);
    },
  );
  test('malformed successful payload becomes a safe server error', () async {
    final dio = Dio(BaseOptions(baseUrl: 'https://example.test'))
      ..httpClientAdapter = _RawAdapter('{}');
    await expectLater(
      ApiSettlementRepository(ApiClient(dio)).getProjectSettlement('id'),
      throwsA(
        isA<AppException>().having(
          (e) => e.kind,
          'kind',
          AppExceptionKind.server,
        ),
      ),
    );
  });
  for (final role in [
    'Manager',
    'Deputy',
    'Accountant',
    'Supervisor',
    'Worker',
    'FutureRole',
  ]) {
    test(
      '$role has the dedicated settlement capability expected by backend',
      () {
        expect(
          RoleCapabilities.forRole(role).canViewProjectSettlement,
          !['Worker', 'FutureRole'].contains(role),
        );
      },
    );
  }
  for (final type in [
    'Expense',
    'PersonalClaim',
    'SupplierDebt',
    'SupplierPayment',
    'SupplierCreditNote',
  ]) {
    test('$type maps to a known named route ignoring resourcePath', () {
      final row = SettlementBlocker.fromJson(
        settlementBlocker(type: type)
          ..['resourcePath'] = 'https://attacker.test',
      );
      final target = settlementDestination(
        row,
        RoleCapabilities.forRole('Manager'),
      );
      expect(target, isNotNull);
      expect(target!.parameters.values.single, 'debt-1');
      expect(target.name.contains('attacker'), isFalse);
    });
  }
  test('unsupported record types and unsafe IDs have no navigation', () {
    for (final type in [
      'ExpenseReturn',
      'SupplierRefund',
      'PersonalClaimPayment',
      'OwnerPaymentRefund',
      'ProjectContractChange',
      'FutureRecord',
    ]) {
      expect(
        settlementDestination(
          SettlementBlocker.fromJson(settlementBlocker(type: type)),
          RoleCapabilities.forRole('Manager'),
        ),
        isNull,
      );
    }
    for (final id in ['../admin', 'id?companyId=other', 'https://evil.test']) {
      expect(
        settlementDestination(
          SettlementBlocker.fromJson(settlementBlocker(id: id)),
          RoleCapabilities.forRole('Manager'),
        ),
        isNull,
      );
    }
  });
  test('Supervisor cannot navigate into company financial records', () {
    for (final type in [
      'PersonalClaim',
      'SupplierPayment',
      'SupplierCreditNote',
    ]) {
      expect(
        settlementDestination(
          SettlementBlocker.fromJson(settlementBlocker(type: type)),
          RoleCapabilities.forRole('Supervisor'),
        ),
        isNull,
      );
    }
    expect(
      settlementDestination(
        SettlementBlocker.fromJson(settlementBlocker()),
        RoleCapabilities.forRole('Worker'),
      ),
      isNull,
    );
  });
  test(
    'controller prevents duplicate loads and preserves loaded state only during refresh',
    () async {
      final repo = FakeSettlementRepository();
      final controller = SettlementController(
        repo,
        'project-1',
        onUnauthorized: () async {},
      );
      addTearDown(controller.dispose);
      await controller.load();
      expect(controller.state.value, isNotNull);
      repo.completer = Completer<ProjectSettlement>();
      final refresh = controller.load();
      await controller.load();
      expect(repo.calls, 2);
      expect(controller.state.loading, isTrue);
      expect(controller.state.value, isNotNull);
      repo.completer!.complete(ProjectSettlement.fromJson(settlementJson()));
      await refresh;
      expect(controller.state.loading, isFalse);
    },
  );
  for (final kind in [
    AppExceptionKind.unauthorized,
    AppExceptionKind.forbidden,
    AppExceptionKind.notFound,
    AppExceptionKind.network,
  ]) {
    test(
      '$kind refresh removes stale financial data and expires only 401',
      () async {
        final repo = FakeSettlementRepository();
        var expired = 0;
        final controller = SettlementController(
          repo,
          'project-1',
          onUnauthorized: () async {
            expired++;
          },
        );
        addTearDown(controller.dispose);
        await controller.load();
        repo.error = AppException(kind);
        await controller.load();
        expect(controller.state.value, isNull);
        expect(controller.state.error!.kind, kind);
        expect(expired, kind == AppExceptionKind.unauthorized ? 1 : 0);
        expect(repo.calls, 2);
      },
    );
  }
  test(
    'late response after dispose does not update state or expire another session',
    () async {
      final repo = FakeSettlementRepository()
        ..completer = Completer<ProjectSettlement>();
      var expired = false;
      final controller = SettlementController(
        repo,
        'project-1',
        onUnauthorized: () async {
          expired = true;
        },
      );
      final pending = controller.load();
      controller.dispose();
      repo.completer!.completeError(
        const AppException(AppExceptionKind.unauthorized),
      );
      await pending;
      expect(expired, isFalse);
    },
  );
}

final class _RawAdapter implements HttpClientAdapter {
  _RawAdapter(this.body);
  final String body;
  RequestOptions? request;
  @override
  Future<ResponseBody> fetch(
    RequestOptions options,
    Stream<Uint8List>? requestStream,
    Future<void>? cancelFuture,
  ) async {
    request = options;
    return ResponseBody.fromString(
      body,
      200,
      headers: {
        Headers.contentTypeHeader: [Headers.jsonContentType],
      },
    );
  }

  @override
  void close({bool force = false}) {}
}
