import 'dart:async';

import 'package:ahdah_app/app/app.dart';
import 'package:ahdah_app/app/providers.dart';
import 'package:ahdah_app/core/config/app_config.dart';
import 'package:ahdah_app/core/localization/value_labels.dart';
import 'package:ahdah_app/features/access/domain/access_models.dart';
import 'package:ahdah_app/features/access/presentation/invitations/invitations_page.dart';
import 'package:ahdah_app/features/access/presentation/join_requests/join_requests_page.dart';
import 'package:ahdah_app/features/account/presentation/account_page.dart';
import 'package:ahdah_app/features/authentication/presentation/splash_page.dart';
import 'package:ahdah_app/features/authentication/presentation/welcome_page.dart';
import 'package:ahdah_app/features/authentication/domain/identity_models.dart';
import 'package:ahdah_app/features/company_registration/presentation/register_company_page.dart';
import 'package:ahdah_app/features/invitation_acceptance/presentation/accept_invitation_page.dart';
import 'package:ahdah_app/features/join_request/presentation/join_request_page.dart';
import 'package:ahdah_app/features/onboarding/presentation/pending_page.dart';
import 'package:ahdah_app/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';

import 'helpers/fakes.dart';

Future<void> pumpAhdah(
  WidgetTester tester, {
  required FakeAuthRepository repository,
  required FakeAccessTokenStore tokenStore,
  String locale = 'en',
  bool settle = true,
  FakeAccessRepository? accessRepository,
}) async {
  SharedPreferences.setMockInitialValues({'ahdah.locale': locale});
  final preferences = await SharedPreferences.getInstance();
  await tester.pumpWidget(
    ProviderScope(
      overrides: [
        appConfigProvider.overrideWithValue(
          AppConfig.parse('http://localhost:5231'),
        ),
        sharedPreferencesProvider.overrideWithValue(preferences),
        accessTokenStoreProvider.overrideWithValue(tokenStore),
        authRepositoryProvider.overrideWithValue(repository),
        accessRepositoryProvider.overrideWithValue(
          accessRepository ?? FakeAccessRepository(),
        ),
      ],
      child: const AhdahApp(),
    ),
  );
  if (settle) await tester.pumpAndSettle();
}

void goTo(WidgetTester tester, String location) {
  final context = tester.element(find.byType(Scaffold).first);
  GoRouter.of(context).go(location);
}

void main() {
  testWidgets('Arabic is RTL', (tester) async {
    await pumpAhdah(
      tester,
      repository: FakeAuthRepository(),
      tokenStore: FakeAccessTokenStore(),
      locale: 'ar',
    );
    expect(
      Directionality.of(tester.element(find.byType(WelcomePage))),
      TextDirection.rtl,
    );
  });

  testWidgets('English is LTR', (tester) async {
    await pumpAhdah(
      tester,
      repository: FakeAuthRepository(),
      tokenStore: FakeAccessTokenStore(),
      locale: 'en',
    );
    expect(
      Directionality.of(tester.element(find.byType(WelcomePage))),
      TextDirection.ltr,
    );
  });

  testWidgets('language switch changes direction and persists preference', (
    tester,
  ) async {
    await pumpAhdah(
      tester,
      repository: FakeAuthRepository(),
      tokenStore: FakeAccessTokenStore(),
      locale: 'ar',
    );

    await tester.tap(find.text('English'));
    await tester.pumpAndSettle();

    expect(
      Directionality.of(tester.element(find.byType(WelcomePage))),
      TextDirection.ltr,
    );
    expect(
      (await SharedPreferences.getInstance()).getString('ahdah.locale'),
      'en',
    );
  });

  testWidgets('unknown role and status values use localized fallback', (
    tester,
  ) async {
    await pumpAhdah(
      tester,
      repository: FakeAuthRepository(),
      tokenStore: FakeAccessTokenStore(),
    );
    final context = tester.element(find.byType(WelcomePage));
    final l10n = AppLocalizations.of(context);

    expect(ValueLabels.role(l10n, 'FutureRole'), l10n.unknownValue);
    expect(ValueLabels.userStatus(l10n, 'FutureStatus'), l10n.unknownValue);
    expect(
      ValueLabels.identityStatus(l10n, 'FutureIdentityStatus'),
      l10n.unknownValue,
    );
  });

  testWidgets('unauthenticated home redirects and onboarding stays public', (
    tester,
  ) async {
    await pumpAhdah(
      tester,
      repository: FakeAuthRepository(),
      tokenStore: FakeAccessTokenStore(),
    );
    goTo(tester, '/home');
    await tester.pumpAndSettle();
    expect(find.byType(WelcomePage), findsOneWidget);

    goTo(tester, '/join-request');
    await tester.pumpAndSettle();
    expect(find.byType(JoinRequestPage), findsOneWidget);
  });

  testWidgets('authenticated login redirects to home without token display', (
    tester,
  ) async {
    await pumpAhdah(
      tester,
      repository: FakeAuthRepository(),
      tokenStore: FakeAccessTokenStore('valid-token'),
    );
    expect(find.text('Welcome, Test User'), findsOneWidget);
    expect(find.textContaining('valid-token'), findsNothing);

    goTo(tester, '/login');
    await tester.pumpAndSettle();
    expect(find.text('Welcome, Test User'), findsOneWidget);
  });

  testWidgets('worker mobile shell has safe minimal navigation and guard', (
    tester,
  ) async {
    await pumpAhdah(
      tester,
      repository: FakeAuthRepository(),
      tokenStore: FakeAccessTokenStore('valid-token'),
    );

    expect(find.byType(NavigationBar), findsOneWidget);
    expect(find.text('Home'), findsWidgets);
    expect(find.text('Account'), findsOneWidget);
    expect(find.text('Invitations'), findsNothing);

    goTo(tester, '/access/invitations');
    await tester.pumpAndSettle();
    expect(find.text('Welcome, Test User'), findsOneWidget);
    expect(find.byType(InvitationsPage), findsNothing);
  });

  testWidgets('unknown role receives safe minimal shell', (tester) async {
    final repository = FakeAuthRepository()
      ..current = const CurrentSessionResult(
        user: AuthenticatedUser(
          userId: 'future-user',
          fullName: 'Future User',
          role: 'FutureRole',
          status: 'Active',
          identityVerificationStatus: 'Unknown',
        ),
        company: testCompany,
        role: 'FutureRole',
      );
    await pumpAhdah(
      tester,
      repository: repository,
      tokenStore: FakeAccessTokenStore('valid-token'),
    );

    expect(find.byType(NavigationBar), findsOneWidget);
    expect(find.text('Invitations'), findsNothing);
    expect(find.text('Join requests'), findsNothing);
  });

  testWidgets('manager can access both administration routes', (tester) async {
    final auth = FakeAuthRepository()..current = testManagerSession;
    await pumpAhdah(
      tester,
      repository: auth,
      tokenStore: FakeAccessTokenStore('valid-token'),
    );

    expect(find.text('Invitations'), findsWidgets);
    expect(find.text('Join requests'), findsWidgets);
    goTo(tester, '/access/invitations');
    await tester.pumpAndSettle();
    expect(find.byType(InvitationsPage), findsOneWidget);
    expect(find.text('No invitations found'), findsOneWidget);

    goTo(tester, '/access/join-requests');
    await tester.pumpAndSettle();
    expect(find.byType(JoinRequestsPage), findsOneWidget);
  });

  testWidgets('wide authenticated shell renders NavigationRail', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(1200, 900);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);
    final repository = FakeAuthRepository()..current = testManagerSession;

    await pumpAhdah(
      tester,
      repository: repository,
      tokenStore: FakeAccessTokenStore('valid-token'),
    );

    expect(find.byType(NavigationRail), findsOneWidget);
    expect(find.byType(NavigationBar), findsNothing);
  });

  testWidgets('Arabic authenticated shell is RTL', (tester) async {
    final repository = FakeAuthRepository()..current = testManagerSession;
    await pumpAhdah(
      tester,
      repository: repository,
      tokenStore: FakeAccessTokenStore('valid-token'),
      locale: 'ar',
    );

    expect(
      Directionality.of(tester.element(find.byType(NavigationBar))),
      TextDirection.rtl,
    );
  });

  testWidgets('one-time invitation token disappears after result closes', (
    tester,
  ) async {
    final auth = FakeAuthRepository()..current = testManagerSession;
    final access = FakeAccessRepository();
    await pumpAhdah(
      tester,
      repository: auth,
      tokenStore: FakeAccessTokenStore('valid-token'),
      accessRepository: access,
    );
    goTo(tester, '/access/invitations');
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 500));

    await tester.tap(find.text('Create invitation'));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 500));
    expect(find.text('Manager'), findsNothing);
    await tester.enterText(
      find.byKey(const Key('invitation-phone-field')),
      '+218912345678',
    );
    await tester.tap(find.byKey(const Key('create-invitation-submit')));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 500));

    expect(find.text('one-time-token-never-persisted'), findsOneWidget);
    await tester.tap(find.text('Finish'));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 500));
    expect(find.text('one-time-token-never-persisted'), findsNothing);
    expect(access.createCalls, 1);
  });

  testWidgets('invitation cancellation requires confirmation', (tester) async {
    final auth = FakeAuthRepository()..current = testManagerSession;
    final access = FakeAccessRepository()
      ..invitationPage = const AccessPage(
        items: [
          Invitation(
            id: 'invitation-1',
            invitedPhoneNumber: '+218912345678',
            assignedRole: 'Worker',
            status: 'Pending',
          ),
        ],
        page: 1,
        pageSize: 20,
        totalCount: 1,
        totalPages: 1,
      );
    await pumpAhdah(
      tester,
      repository: auth,
      tokenStore: FakeAccessTokenStore('valid-token'),
      accessRepository: access,
    );
    goTo(tester, '/access/invitations');
    await tester.pumpAndSettle();

    await tester.tap(find.text('Cancel invitation'));
    await tester.pumpAndSettle();
    expect(find.text('Cancel this invitation?'), findsOneWidget);
    expect(access.cancelCalls, 0);
  });

  testWidgets('approval excludes Manager and explains sensitive roles', (
    tester,
  ) async {
    final auth = FakeAuthRepository()..current = testManagerSession;
    final access = FakeAccessRepository()
      ..joinRequestPage = const AccessPage(
        items: [
          JoinRequest(
            id: 'request-1',
            fullName: 'Applicant',
            phoneNumber: '+218912345679',
            requestedRole: 'Accountant',
            status: 'Pending',
          ),
        ],
        page: 1,
        pageSize: 20,
        totalCount: 1,
        totalPages: 1,
      );
    await pumpAhdah(
      tester,
      repository: auth,
      tokenStore: FakeAccessTokenStore('valid-token'),
      accessRepository: access,
    );
    goTo(tester, '/access/join-requests');
    await tester.pumpAndSettle();

    await tester.tap(find.text('Approve'));
    await tester.pumpAndSettle();
    expect(
      find.text(
        'Deputy and accountant accounts may remain pending until identity verification is completed.',
      ),
      findsOneWidget,
    );
    await tester.tap(find.byKey(const Key('approval-role-field')));
    await tester.pumpAndSettle();
    expect(find.text('Manager'), findsNothing);
  });

  testWidgets('rejection validates required reason', (tester) async {
    final auth = FakeAuthRepository()..current = testManagerSession;
    final access = FakeAccessRepository()
      ..joinRequestPage = const AccessPage(
        items: [
          JoinRequest(
            id: 'request-1',
            fullName: 'Applicant',
            phoneNumber: '+218912345679',
            requestedRole: 'Worker',
            status: 'Pending',
          ),
        ],
        page: 1,
        pageSize: 20,
        totalCount: 1,
        totalPages: 1,
      );
    await pumpAhdah(
      tester,
      repository: auth,
      tokenStore: FakeAccessTokenStore('valid-token'),
      accessRepository: access,
    );
    goTo(tester, '/access/join-requests');
    await tester.pumpAndSettle();

    await tester.tap(find.text('Reject'));
    await tester.pumpAndSettle();
    await tester.tap(find.byKey(const Key('rejection-submit')));
    await tester.pump();
    expect(find.text('This field is required.'), findsOneWidget);
    expect(access.rejectCalls, 0);
  });

  testWidgets('account page exposes no token or internal ids', (tester) async {
    final repository = FakeAuthRepository()..current = testManagerSession;
    await pumpAhdah(
      tester,
      repository: repository,
      tokenStore: FakeAccessTokenStore('sensitive-token'),
    );
    goTo(tester, '/account');
    await tester.pumpAndSettle();

    expect(find.byType(AccountPage), findsOneWidget);
    expect(find.textContaining('sensitive-token'), findsNothing);
    expect(find.textContaining(testManager.userId), findsNothing);
    expect(find.textContaining(testCompany.companyId), findsNothing);
    expect(find.text('Identity verified'), findsOneWidget);
  });

  testWidgets('bootstrapping remains on splash without redirect loop', (
    tester,
  ) async {
    final repository = FakeAuthRepository()..currentCompleter = Completer();
    await pumpAhdah(
      tester,
      repository: repository,
      tokenStore: FakeAccessTokenStore('valid-token'),
      settle: false,
    );
    await tester.pump();
    await tester.pump();
    expect(find.byType(SplashPage), findsOneWidget);
  });

  testWidgets('login prevents invalid submission', (tester) async {
    final repository = FakeAuthRepository();
    await pumpAhdah(
      tester,
      repository: repository,
      tokenStore: FakeAccessTokenStore(),
    );
    goTo(tester, '/login');
    await tester.pumpAndSettle();

    await tester.tap(find.widgetWithText(FilledButton, 'Sign in'));
    await tester.pump();

    expect(repository.loginCalls, 0);
    expect(find.text('This field is required.'), findsWidgets);
  });

  testWidgets('company registration validates required fields', (tester) async {
    await pumpAhdah(
      tester,
      repository: FakeAuthRepository(),
      tokenStore: FakeAccessTokenStore(),
    );
    goTo(tester, '/register-company');
    await tester.pumpAndSettle();
    expect(find.byType(RegisterCompanyPage), findsOneWidget);

    final registerButton = find.widgetWithText(FilledButton, 'Create company');
    await tester.ensureVisible(registerButton);
    await tester.tap(registerButton);
    await tester.pump();

    expect(find.text('This field is required.'), findsWidgets);
  });

  testWidgets('join request never offers Manager role', (tester) async {
    await pumpAhdah(
      tester,
      repository: FakeAuthRepository(),
      tokenStore: FakeAccessTokenStore(),
    );
    goTo(tester, '/join-request');
    await tester.pumpAndSettle();

    final roleField = find.byKey(const Key('requested-role-field'));
    await tester.ensureVisible(roleField);
    await tester.tap(roleField);
    await tester.pumpAndSettle();

    expect(find.text('Manager'), findsNothing);
    expect(find.text('Worker'), findsWidgets);
  });

  testWidgets('invitation token is not retained after successful submission', (
    tester,
  ) async {
    const invitationToken = 'abcdefghijklmnopqrstuvwxyz_123456789';
    await pumpAhdah(
      tester,
      repository: FakeAuthRepository(),
      tokenStore: FakeAccessTokenStore(),
    );
    goTo(tester, '/accept-invitation');
    await tester.pumpAndSettle();
    expect(find.byType(AcceptInvitationPage), findsOneWidget);

    await tester.enterText(
      find.byKey(const Key('invitation-token-field')),
      invitationToken,
    );
    await tester.enterText(find.byType(TextFormField).at(1), 'Invited User');
    await tester.enterText(
      find.byKey(const Key('password-field')),
      'a sufficiently long password',
    );
    await tester.tap(find.widgetWithText(FilledButton, 'Accept invitation'));
    await tester.pumpAndSettle();

    expect(find.byType(PendingPage), findsOneWidget);
    expect(find.text(invitationToken), findsNothing);
    expect(find.text('Identity verification required'), findsOneWidget);
  });

  testWidgets('pending page renders a safe fallback message', (tester) async {
    await pumpAhdah(
      tester,
      repository: FakeAuthRepository(),
      tokenStore: FakeAccessTokenStore(),
    );
    goTo(tester, '/pending');
    await tester.pumpAndSettle();
    expect(find.byType(PendingPage), findsOneWidget);
    expect(
      find.text(
        'Your onboarding step was received and is awaiting completion.',
      ),
      findsOneWidget,
    );
  });

  testWidgets('loading prevents duplicate login submission', (tester) async {
    final repository = FakeAuthRepository()..loginCompleter = Completer();
    await pumpAhdah(
      tester,
      repository: repository,
      tokenStore: FakeAccessTokenStore(),
    );
    goTo(tester, '/login');
    await tester.pumpAndSettle();

    await tester.enterText(
      find.byKey(const Key('login-phone-field')),
      '+218912345678',
    );
    await tester.enterText(find.byKey(const Key('password-field')), 'password');
    final submitButton = find.widgetWithText(FilledButton, 'Sign in');
    final submitPosition = tester.getCenter(submitButton);
    await tester.tap(submitButton);
    await tester.pump();
    await tester.tapAt(submitPosition);
    await tester.pump();

    expect(repository.loginCalls, 1);
    repository.loginCompleter!.complete(testAuthentication);
    await tester.pumpAndSettle();
  });
}
