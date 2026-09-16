import 'dart:async';
import 'package:ahdah_app/app/app.dart';
import 'package:ahdah_app/app/providers.dart';
import 'package:ahdah_app/core/config/app_config.dart';
import 'package:ahdah_app/core/errors/app_exception.dart';
import 'package:ahdah_app/features/authenticated_home/presentation/home_page.dart';
import 'package:ahdah_app/features/authentication/domain/identity_models.dart';
import 'package:ahdah_app/features/settlements/domain/settlement_models.dart';
import 'package:ahdah_app/features/settlements/presentation/project_settlement_page.dart';
import 'package:ahdah_app/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'helpers/fakes.dart';
import 'helpers/settlement_fakes.dart';
import 'package:ahdah_app/features/suppliers/domain/supplier_models.dart';
import 'package:ahdah_app/features/suppliers/domain/supplier_repository.dart';

void main() {
  for (final role in ['Manager', 'Deputy', 'Accountant', 'Supervisor']) {
    testWidgets('$role can open named settlement route with project ID', (
      tester,
    ) async {
      final repo = FakeSettlementRepository();
      await pumpSettlement(tester, repo, role: role);
      expect(find.byType(ProjectSettlementPage), findsOneWidget);
      expect(repo.lastProjectId, 'project-1');
      expect(find.text('Settlement readiness'), findsOneWidget);
      expect(find.text('Financial closure readiness'), findsOneWidget);
    });
  }
  for (final role in ['Worker', 'FutureRole']) {
    testWidgets('$role is redirected without settlement request', (
      tester,
    ) async {
      final repo = FakeSettlementRepository();
      await pumpSettlement(tester, repo, role: role);
      expect(find.byType(HomePage), findsOneWidget);
      expect(find.byType(ProjectSettlementPage), findsNothing);
      expect(repo.calls, 0);
    });
  }
  testWidgets(
    'project details exposes the settlement entry point and preserves project identity',
    (tester) async {
      final repo = FakeSettlementRepository();
      await pumpSettlement(tester, repo, path: '/projects/${testProject.id}');
      await tester.tap(find.byKey(const Key('project-settlement-action')));
      await tester.pumpAndSettle();
      expect(find.byType(ProjectSettlementPage), findsOneWidget);
      expect(repo.lastProjectId, testProject.id);
      expect(find.text(testProject.projectName), findsOneWidget);
    },
  );
  testWidgets(
    'zero known blockers never render ready and gaps explain uncertainty',
    (tester) async {
      await pumpSettlement(tester, FakeSettlementRepository());
      expect(find.text('Indeterminate'), findsNWidgets(2));
      expect(
        find.text('Settlement readiness cannot yet be fully determined.'),
        findsOneWidget,
      );
      expect(
        find.text(
          'Financial closure readiness cannot yet be fully determined.',
        ),
        findsOneWidget,
      );
      await reveal(
        tester,
        find.text(
          'No known blockers in the evaluated categories. This does not certify financial readiness.',
        ),
      );
      expect(find.text('Ready'), findsNothing);
      expect(find.text('Clear'), findsNothing);
      await reveal(tester, find.text('Evaluation gaps'));
      expect(
        find.textContaining('Advance balances cannot currently'),
        findsOneWidget,
      );
    },
  );
  testWidgets(
    'blocked settlement and separate closure impediments are localized',
    (tester) async {
      final repo = FakeSettlementRepository();
      repo.json.addAll({
        'canSettle': false,
        'canClose': false,
        'settlementReadiness': 'Blocked',
        'closureReadiness': 'Blocked',
        'hasKnownFinancialBlockers': true,
        'totalBlockerCount': 1,
        'settlementImpediments': ['KnownFinancialBlockers'],
        'closureImpediments': ['ProjectNotCompleted'],
      });
      await pumpSettlement(tester, repo);
      expect(find.text('Blocked'), findsNWidgets(2));
      expect(
        find.text('Known impediments prevent settlement.'),
        findsOneWidget,
      );
      await reveal(
        tester,
        find.text('The project is not operationally completed.'),
      );
      expect(find.text('Closure impediments'), findsOneWidget);
    },
  );
  testWidgets(
    'evaluated category displays distinct currencies and allowed record link',
    (tester) async {
      final repo = FakeSettlementRepository()
        ..json['categories'] = [settlementCategory()];
      await pumpSettlement(tester, repo);
      await expand(tester, 'SupplierDebt');
      expect(find.text('LYD 8500.01'), findsWidgets);
      expect(find.text('USD 300.09'), findsOneWidget);
      expect(find.textContaining('8800'), findsNothing);
      await reveal(tester, find.text('View record'));
      expect(
        tester
            .widget<TextButton>(
              find.ancestor(
                of: find.text('View record'),
                matching: find.byType(TextButton),
              ),
            )
            .onPressed,
        isNotNull,
      );
      await tester.tap(find.text('View record'));
      await tester.pumpAndSettle();
      final router = GoRouter.of(tester.element(find.byType(Scaffold).first));
      expect(
        router.routeInformationProvider.value.uri.path,
        '/supplier-debts/debt-1',
      );
      expect(find.byType(ProjectSettlementPage), findsNothing);
    },
  );
  testWidgets(
    'evaluated empty category remains distinct from unavailable evaluation',
    (tester) async {
      final repo = FakeSettlementRepository()
        ..json['categories'] = [
          settlementCategory()
            ..['blockerCount'] = 0
            ..['totals'] = []
            ..['blockers'] = [],
        ];
      await pumpSettlement(tester, repo);
      await expand(tester, 'SupplierDebt');
      expect(
        find.text('No known blockers in this evaluated category.'),
        findsOneWidget,
      );
      expect(find.text('View record'), findsNothing);
      expect(find.text('Ready'), findsNothing);
    },
  );
  for (final evaluation in [
    'NotVisible',
    'NotAttributable',
    'FutureEvaluation',
  ]) {
    testWidgets(
      '$evaluation suppresses unexpected counts totals IDs and links',
      (tester) async {
        final category = settlementCategory(evaluation: evaluation)
          ..['blockerCount'] = 987654;
        category['blockers'] = [settlementBlocker(id: 'hidden-record-secret')];
        final repo = FakeSettlementRepository()
          ..json['categories'] = [category];
        await pumpSettlement(tester, repo);
        await reveal(tester, find.text('Supplier debt'));
        await tester.drag(
          find.byKey(const Key('settlement-scroll')),
          const Offset(0, -600),
        );
        await tester.pumpAndSettle();
        expect(find.textContaining('987654'), findsNothing);
        expect(find.textContaining('8500.01'), findsNothing);
        expect(find.textContaining('hidden-record-secret'), findsNothing);
        expect(find.text('View record'), findsNothing);
        expect(
          find.textContaining(
            evaluation == 'NotVisible'
                ? 'outside your access'
                : evaluation == 'NotAttributable'
                ? 'not a zero balance'
                : 'not supported',
          ),
          findsWidgets,
        );
      },
    );
  }
  testWidgets(
    'Supervisor defense suppresses company financial records even if marked evaluated',
    (tester) async {
      final repo = FakeSettlementRepository()
        ..json['categories'] = [settlementCategory(name: 'SupplierPayments')];
      await pumpSettlement(tester, repo, role: 'Supervisor');
      await reveal(tester, find.text('Supplier payments'));
      expect(find.text('Not visible'), findsOneWidget);
      expect(find.text('LYD 8500.01'), findsNothing);
    },
  );
  testWidgets(
    'unsupported record has no navigation and unknown reason gets localized fallback',
    (tester) async {
      final row = settlementBlocker(type: 'ExpenseReturn')
        ..['code'] = 'SecretFutureCode'
        ..['status'] = 'FutureStatus';
      final repo = FakeSettlementRepository()
        ..json['categories'] = [
          settlementCategory()..['blockers'] = [row],
        ];
      await pumpSettlement(tester, repo);
      await expand(tester, 'SupplierDebt');
      await reveal(tester, find.textContaining('Record reference'));
      expect(find.text('View record'), findsNothing);
      expect(find.text('SecretFutureCode'), findsNothing);
      expect(find.text('Unrecognized value — review required'), findsOneWidget);
    },
  );
  testWidgets('unexpected future readiness does not claim clearance', (
    tester,
  ) async {
    final repo = FakeSettlementRepository()
      ..json.addAll({
        'canSettle': true,
        'canClose': true,
        'settlementReadiness': 'FutureReady',
        'closureReadiness': 'FutureReady',
      });
    await pumpSettlement(tester, repo);
    expect(find.text('Unrecognized value — review required'), findsNWidgets(2));
    expect(find.text('FutureReady'), findsNothing);
    expect(find.text('Ready'), findsNothing);
  });
  for (final locale in ['en', 'ar']) {
    testWidgets(
      '$locale narrow screen and long reasons wrap with correct direction',
      (tester) async {
        tester.view.physicalSize = const Size(360, 900);
        tester.view.devicePixelRatio = 1;
        addTearDown(tester.view.resetPhysicalSize);
        addTearDown(tester.view.resetDevicePixelRatio);
        final repo = FakeSettlementRepository()
          ..json['categories'] = [settlementCategory()];
        await pumpSettlement(tester, repo, locale: locale);
        final context = tester.element(find.byType(ProjectSettlementPage));
        expect(
          Directionality.of(context),
          locale == 'ar' ? TextDirection.rtl : TextDirection.ltr,
        );
        final l10n = AppLocalizations.of(context);
        expect(find.text(l10n.settlementTitle), findsOneWidget);
        await expand(tester, 'SupplierDebt');
        expect(find.text('USD 300.09'), findsOneWidget);
        expect(tester.takeException(), isNull);
      },
    );
  }
  for (final kind in [
    AppExceptionKind.network,
    AppExceptionKind.forbidden,
    AppExceptionKind.notFound,
  ]) {
    testWidgets(
      '$kind shows localized error and retry without claiming empty success',
      (tester) async {
        final repo = FakeSettlementRepository()..error = AppException(kind);
        await pumpSettlement(tester, repo);
        final l10n = AppLocalizations.of(
          tester.element(find.byType(ProjectSettlementPage)),
        );
        expect(
          find.text(switch (kind) {
            AppExceptionKind.network => l10n.networkError,
            AppExceptionKind.forbidden => l10n.permissionDenied,
            _ => l10n.notFoundError,
          }),
          findsOneWidget,
        );
        expect(find.text('Indeterminate'), findsNothing);
        repo.error = null;
        await tester.tap(find.text(l10n.retry));
        await tester.pumpAndSettle();
        expect(find.text('Indeterminate'), findsNWidgets(2));
        expect(repo.calls, 2);
      },
    );
  }
  testWidgets('loading and explicit refresh use the same controller', (
    tester,
  ) async {
    final repo = FakeSettlementRepository()
      ..completer = Completer<ProjectSettlement>();
    await pumpSettlement(tester, repo, settle: false);
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
    repo.completer!.complete(ProjectSettlement.fromJson(settlementJson()));
    await tester.pumpAndSettle();
    repo.completer = null;
    await tester.tap(find.byKey(const Key('settlement-refresh')));
    await tester.pumpAndSettle();
    expect(repo.calls, 2);
  });
}

Future<void> pumpSettlement(
  WidgetTester tester,
  FakeSettlementRepository repo, {
  String role = 'Manager',
  String locale = 'en',
  String path = '/projects/project-1/settlement',
  bool settle = true,
}) async {
  SharedPreferences.setMockInitialValues({'ahdah.locale': locale});
  final preferences = await SharedPreferences.getInstance();
  final user = AuthenticatedUser(
    userId: 'user-1',
    fullName: 'Test',
    role: role,
    status: 'Active',
    identityVerificationStatus: 'Verified',
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
        settlementRepositoryProvider.overrideWithValue(repo),
        supplierRepositoryProvider.overrideWithValue(
          _SettlementSupplierRepository(),
        ),
      ],
      child: const AhdahApp(),
    ),
  );
  await tester.pumpAndSettle();
  GoRouter.of(tester.element(find.byType(Scaffold).first)).go(path);
  await tester.pump();
  await tester.pump(const Duration(milliseconds: 100));
  if (settle) await tester.pumpAndSettle();
}

final class _SettlementSupplierRepository implements SupplierRepository {
  @override
  Future<SupplierInvoiceDetails> getInvoice(String debtId) async {
    expect(debtId, 'debt-1');
    throw const AppException(AppExceptionKind.notFound);
  }

  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}

Future<void> reveal(WidgetTester tester, Finder finder) async {
  await tester.scrollUntilVisible(
    finder,
    240,
    scrollable: find
        .descendant(
          of: find.byKey(const Key('settlement-scroll')),
          matching: find.byType(Scrollable),
        )
        .first,
    maxScrolls: 30,
  );
  await tester.pumpAndSettle();
}

Future<void> expand(WidgetTester tester, String category) async {
  final finder = find.byKey(Key('settlement-category-$category'));
  await reveal(tester, finder);
  await tester.tap(
    find.descendant(of: finder, matching: find.byType(ListTile)).first,
  );
  await tester.pumpAndSettle();
  await tester.drag(
    find.byKey(const Key('settlement-scroll')),
    const Offset(0, -200),
  );
  await tester.pumpAndSettle();
}
