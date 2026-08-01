import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../features/authenticated_home/presentation/home_page.dart';
import '../../features/account/presentation/account_page.dart';
import '../../features/access/domain/role_capabilities.dart';
import '../../features/access/presentation/invitations/invitations_page.dart';
import '../../features/access/presentation/join_requests/join_requests_page.dart';
import '../../features/authenticated_shell/presentation/authenticated_shell.dart';
import '../../features/authentication/presentation/login_page.dart';
import '../../features/authentication/presentation/splash_page.dart';
import '../../features/authentication/presentation/welcome_page.dart';
import '../../features/company_registration/presentation/register_company_page.dart';
import '../../features/invitation_acceptance/presentation/accept_invitation_page.dart';
import '../../features/join_request/presentation/join_request_page.dart';
import '../../features/onboarding/domain/pending_result.dart';
import '../../features/onboarding/presentation/pending_page.dart';
import '../../features/session/presentation/session_controller.dart';
import '../../features/session/presentation/session_unavailable_page.dart';
import 'app_routes.dart';

final appRouterProvider = Provider<GoRouter>((ref) {
  final session = ref.watch(sessionControllerProvider);
  return GoRouter(
    initialLocation: AppRoutes.splashPath,
    redirect: (context, state) {
      final location = state.matchedLocation;
      final isSplash = location == AppRoutes.splashPath;
      final authenticatedPaths = {
        AppRoutes.homePath,
        AppRoutes.invitationsPath,
        AppRoutes.joinRequestsPath,
        AppRoutes.accountPath,
      };
      final isAuthenticatedPath = authenticatedPaths.contains(location);
      final isManagerPath =
          location == AppRoutes.invitationsPath ||
          location == AppRoutes.joinRequestsPath;
      final isUnavailable = location == AppRoutes.unavailablePath;
      final isOnboarding = {
        AppRoutes.welcomePath,
        AppRoutes.loginPath,
        AppRoutes.registerCompanyPath,
        AppRoutes.joinRequestPath,
        AppRoutes.acceptInvitationPath,
        AppRoutes.pendingPath,
      }.contains(location);

      return switch (session.status) {
        SessionStatus.bootstrapping => isSplash ? null : AppRoutes.splashPath,
        SessionStatus.temporarilyUnavailable =>
          isUnavailable ? null : AppRoutes.unavailablePath,
        SessionStatus.authenticated =>
          isManagerPath &&
                  !RoleCapabilities.forRole(
                    session.current?.role,
                  ).canManageAccess
              ? AppRoutes.homePath
              : (isOnboarding || isSplash || isUnavailable)
              ? AppRoutes.homePath
              : null,
        SessionStatus.sessionExpired =>
          isAuthenticatedPath || isSplash || isUnavailable
              ? AppRoutes.loginPath
              : null,
        SessionStatus.unauthenticated =>
          isAuthenticatedPath || isSplash || isUnavailable
              ? AppRoutes.welcomePath
              : null,
      };
    },
    routes: [
      GoRoute(
        path: AppRoutes.splashPath,
        name: AppRoutes.splash,
        builder: (context, state) => const SplashPage(),
      ),
      GoRoute(
        path: AppRoutes.welcomePath,
        name: AppRoutes.welcome,
        builder: (context, state) => const WelcomePage(),
      ),
      GoRoute(
        path: AppRoutes.loginPath,
        name: AppRoutes.login,
        builder: (context, state) => const LoginPage(),
      ),
      GoRoute(
        path: AppRoutes.registerCompanyPath,
        name: AppRoutes.registerCompany,
        builder: (context, state) => const RegisterCompanyPage(),
      ),
      GoRoute(
        path: AppRoutes.joinRequestPath,
        name: AppRoutes.joinRequest,
        builder: (context, state) => const JoinRequestPage(),
      ),
      GoRoute(
        path: AppRoutes.acceptInvitationPath,
        name: AppRoutes.acceptInvitation,
        builder: (context, state) => const AcceptInvitationPage(),
      ),
      GoRoute(
        path: AppRoutes.pendingPath,
        name: AppRoutes.pending,
        builder: (context, state) => PendingPage(
          result: state.extra is PendingOnboardingResult
              ? state.extra! as PendingOnboardingResult
              : null,
        ),
      ),
      ShellRoute(
        builder: (context, state, child) =>
            AuthenticatedShell(location: state.uri.path, child: child),
        routes: [
          GoRoute(
            path: AppRoutes.homePath,
            name: AppRoutes.home,
            builder: (context, state) => const HomePage(),
          ),
          GoRoute(
            path: AppRoutes.invitationsPath,
            name: AppRoutes.invitations,
            builder: (context, state) => const InvitationsPage(),
          ),
          GoRoute(
            path: AppRoutes.joinRequestsPath,
            name: AppRoutes.joinRequestsAdmin,
            builder: (context, state) => const JoinRequestsPage(),
          ),
          GoRoute(
            path: AppRoutes.accountPath,
            name: AppRoutes.account,
            builder: (context, state) => const AccountPage(),
          ),
        ],
      ),
      GoRoute(
        path: AppRoutes.unavailablePath,
        name: AppRoutes.unavailable,
        builder: (context, state) => const SessionUnavailablePage(),
      ),
    ],
  );
});
