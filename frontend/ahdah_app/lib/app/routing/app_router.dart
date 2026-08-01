import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../features/authenticated_home/presentation/home_page.dart';
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
      final isHome = location == AppRoutes.homePath;
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
          isHome
              ? null
              : (isOnboarding || isSplash || isUnavailable)
              ? AppRoutes.homePath
              : null,
        SessionStatus.sessionExpired =>
          isHome || isSplash || isUnavailable ? AppRoutes.loginPath : null,
        SessionStatus.unauthenticated =>
          isHome || isSplash || isUnavailable ? AppRoutes.welcomePath : null,
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
      GoRoute(
        path: AppRoutes.homePath,
        name: AppRoutes.home,
        builder: (context, state) => const HomePage(),
      ),
      GoRoute(
        path: AppRoutes.unavailablePath,
        name: AppRoutes.unavailable,
        builder: (context, state) => const SessionUnavailablePage(),
      ),
    ],
  );
});
