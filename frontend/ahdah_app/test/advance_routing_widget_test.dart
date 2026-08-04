import 'package:ahdah_app/app/app.dart';
import 'package:ahdah_app/app/providers.dart';
import 'package:ahdah_app/core/config/app_config.dart';
import 'package:ahdah_app/features/access/domain/role_capabilities.dart';
import 'package:ahdah_app/features/advances/domain/advance_models.dart';
import 'package:ahdah_app/features/advances/presentation/create/advance_create_page.dart';
import 'package:ahdah_app/features/advances/presentation/distribute/advance_distribution_page.dart';
import 'package:ahdah_app/features/advances/presentation/list/advances_page.dart';
import 'package:ahdah_app/features/advances/presentation/movements/advance_movements_page.dart';
import 'package:ahdah_app/features/advances/presentation/return_money/advance_return_page.dart';
import 'package:ahdah_app/features/authenticated_home/presentation/home_page.dart';
import 'package:ahdah_app/features/authentication/domain/identity_models.dart';
import 'package:ahdah_app/features/company_members/domain/company_member_models.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';

import 'helpers/fakes.dart';

void main() {
  test('advance capabilities use exact conservative role values', () {
    final manager = RoleCapabilities.forRole('Manager');
    final deputy = RoleCapabilities.forRole('Deputy');
    final accountant = RoleCapabilities.forRole('Accountant');
    final supervisor = RoleCapabilities.forRole('Supervisor');
    final worker = RoleCapabilities.forRole('Worker');
    final unknown = RoleCapabilities.forRole('manager');

    expect(manager.canCreateTopLevelAdvance, isTrue);
    expect(manager.canDistributeAdvances, isTrue);
    expect(deputy.canDistributeAdvances, isTrue);
    expect(accountant.canViewAdvances, isTrue);
    expect(accountant.canConfirmReceivedAdvance, isFalse);
    expect(supervisor.canReturnHeldBalance, isTrue);
    expect(worker.canRejectReceivedTransfer, isTrue);
    expect(unknown.canViewAdvances, isFalse);
    expect(unknown.canCreateTopLevelAdvance, isFalse);
  });

  testWidgets('all supported roles see Advances and unknown role does not', (
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
      expect(find.text('Advances'), findsWidgets, reason: role);
    }
    await _pump(tester, role: 'FutureRole');
    expect(find.text('Advances'), findsNothing);
  });

  testWidgets('manager create route is guarded from every non-manager', (
    tester,
  ) async {
    await _pump(tester, role: 'Manager');
    _go(tester, '/advances');
    await tester.pumpAndSettle();
    expect(find.byType(AdvancesPage), findsOneWidget);
    expect(find.byKey(const Key('create-advance-action')), findsOneWidget);
    _go(tester, '/advances/new');
    await tester.pumpAndSettle();
    expect(find.byType(AdvanceCreatePage), findsOneWidget);

    await _pump(tester, role: 'Deputy');
    _go(tester, '/advances/new');
    await tester.pumpAndSettle();
    expect(find.byType(HomePage), findsOneWidget);
    expect(find.byType(AdvanceCreatePage), findsNothing);
  });

  testWidgets('distribution route is Manager/Deputy only', (tester) async {
    await _pump(tester, role: 'Deputy');
    _go(tester, '/advances/advance-1/distribute');
    await tester.pumpAndSettle();
    expect(find.byType(AdvanceDistributionPage), findsOneWidget);

    await _pump(tester, role: 'Accountant');
    _go(tester, '/advances/advance-1/distribute');
    await tester.pumpAndSettle();
    expect(find.byType(HomePage), findsOneWidget);
  });

  testWidgets('participant empty state explains personal visibility', (
    tester,
  ) async {
    await _pump(tester, role: 'Worker');
    _go(tester, '/advances');
    await tester.pumpAndSettle();

    expect(find.byType(AdvancesPage), findsOneWidget);
    expect(
      find.text('Only advances involving you appear here.'),
      findsOneWidget,
    );
  });

  testWidgets('initial delivery can be confirmed but never rejected', (
    tester,
  ) async {
    final advances = FakeAdvanceRepository()
      ..movementPage = AdvanceMovementPage(
        items: [
          AdvanceMovement(
            movementId: 'delivery-1',
            operationType: 'AdvanceDelivery',
            amount: '100.00',
            actor: const AdvanceUserSummary(
              userId: 'manager-1',
              fullName: 'Manager',
              role: 'Manager',
            ),
            sender: const AdvanceUserSummary(
              userId: 'manager-1',
              fullName: 'Manager',
              role: 'Manager',
            ),
            recipient: const AdvanceUserSummary(
              userId: 'user-Worker',
              fullName: 'Worker User',
              role: 'Worker',
            ),
            status: 'PendingConfirmation',
            occurredAtUtc: DateTime.utc(2026, 8, 4),
          ),
        ],
        page: 1,
        pageSize: 20,
        totalCount: 1,
        totalPages: 1,
      );
    await _pump(tester, role: 'Worker', advances: advances);
    _go(tester, '/advances/advance-1/movements');
    await tester.pumpAndSettle();

    expect(find.byType(AdvanceMovementsPage), findsOneWidget);
    expect(
      find.byKey(const Key('confirm-transfer-delivery-1')),
      findsOneWidget,
    );
    expect(find.byKey(const Key('reject-transfer-delivery-1')), findsNothing);
  });

  testWidgets('return page exposes no arbitrary recipient selector', (
    tester,
  ) async {
    await _pump(tester, role: 'Worker');
    _go(tester, '/advances/advance-1/return');
    await tester.pumpAndSettle();

    expect(find.byType(AdvanceReturnPage), findsOneWidget);
    expect(find.byKey(const Key('distribution-recipient-field')), findsNothing);
    expect(find.text('Select recipient'), findsNothing);
    expect(
      find.textContaining('derived automatically from confirmed upstream'),
      findsOneWidget,
    );
  });

  testWidgets('creation selector retains Active Deputy candidates only', (
    tester,
  ) async {
    final members = FakeCompanyMemberRepository()
      ..memberPage = const CompanyMemberPage(
        items: [
          CompanyMemberSummary(
            id: 'deputy-1',
            fullName: 'Eligible Deputy',
            role: 'Deputy',
            status: 'Active',
            identityVerificationStatus: 'Verified',
          ),
          CompanyMemberSummary(
            id: 'worker-1',
            fullName: 'Unsafe Worker',
            role: 'Worker',
            status: 'Active',
            identityVerificationStatus: 'NotRequired',
          ),
          CompanyMemberSummary(
            id: 'deputy-2',
            fullName: 'Suspended Deputy',
            role: 'Deputy',
            status: 'Suspended',
            identityVerificationStatus: 'Verified',
          ),
        ],
        page: 1,
        pageSize: 20,
        totalCount: 3,
        totalPages: 1,
      );
    await _pump(tester, role: 'Manager', members: members);
    _go(tester, '/advances/new');
    await tester.pumpAndSettle();
    await tester.tap(find.byKey(const Key('advance-recipient-field')));
    await tester.pumpAndSettle();

    expect(find.text('Eligible Deputy'), findsOneWidget);
    expect(find.text('Unsafe Worker'), findsNothing);
    expect(find.text('Suspended Deputy'), findsNothing);
    expect(members.lastRole, 'Deputy');
    expect(members.lastStatus, 'Active');
  });
}

Future<void> _pump(
  WidgetTester tester, {
  required String role,
  FakeAdvanceRepository? advances,
  FakeCompanyMemberRepository? members,
}) async {
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
  final repository = advances ?? FakeAdvanceRepository();
  repository.details = AdvanceDetails(
    advance: testAdvanceSummary,
    fundings: const [],
    balances: [
      AdvanceBalanceSummary(
        advanceId: 'advance-1',
        advanceNumber: 'ADV-TEST',
        holder: AdvanceUserSummary(
          userId: user.userId,
          fullName: user.fullName,
          role: role,
        ),
        totalReceivedAmount: '100.00',
        totalRestoredAmount: '0.00',
        totalExpensedAmount: '0.00',
        totalTransferredOutAmount: '0.00',
        totalReturnedAmount: '0.00',
        availableAmount: '80.00',
        reservedAmount: '20.00',
        currencyCode: 'LYD',
        status: 'Active',
        versionNumber: 1,
        updatedAtUtc: DateTime.utc(2026, 8, 4),
      ),
    ],
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
          members ?? FakeCompanyMemberRepository(),
        ),
        advanceRepositoryProvider.overrideWithValue(repository),
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
