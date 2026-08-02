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
import '../../features/projects/presentation/create/project_create_page.dart';
import '../../features/projects/presentation/detail/project_details_page.dart';
import '../../features/projects/presentation/edit/project_edit_page.dart';
import '../../features/projects/presentation/list/projects_page.dart';
import '../../features/projects/presentation/members/project_members_page.dart';
import '../../features/projects/presentation/supervisor/project_supervisor_page.dart';
import '../../features/company_members/presentation/detail/company_member_details_page.dart';
import '../../features/company_members/presentation/list/company_members_page.dart';
import 'app_routes.dart';

final appRouterProvider = Provider<GoRouter>((ref) {
  final session = ref.watch(sessionControllerProvider);
  return GoRouter(
    initialLocation: AppRoutes.splashPath,
    redirect: (context, state) {
      final location = state.matchedLocation;
      final isSplash = location == AppRoutes.splashPath;
      final isProjectPath =
          location == AppRoutes.projectsPath ||
          location.startsWith('${AppRoutes.projectsPath}/');
      final isCompanyDirectoryPath =
          location == AppRoutes.companyMembersPath ||
          location.startsWith('${AppRoutes.companyMembersPath}/');
      final isAccessManagerPath =
          location == AppRoutes.invitationsPath ||
          location == AppRoutes.joinRequestsPath;
      final isProjectManagementPath =
          location == AppRoutes.projectCreatePath ||
          location.endsWith('/edit') ||
          location.endsWith('/supervisor');
      final isProjectMembersPath = location.endsWith('/members');
      final isAuthenticatedPath =
          location == AppRoutes.homePath ||
          location == AppRoutes.accountPath ||
          isAccessManagerPath ||
          isProjectPath ||
          isCompanyDirectoryPath;
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
        SessionStatus.authenticated => () {
          final capabilities = RoleCapabilities.forRole(session.current?.role);
          if (isAccessManagerPath && !capabilities.canManageAccess ||
              isProjectPath && !capabilities.canViewProjects ||
              isProjectManagementPath && !capabilities.canManageProjects ||
              isProjectMembersPath && !capabilities.canViewProjectMembers ||
              isCompanyDirectoryPath && !capabilities.canViewCompanyDirectory) {
            return AppRoutes.homePath;
          }
          return isOnboarding || isSplash || isUnavailable
              ? AppRoutes.homePath
              : null;
        }(),
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
            path: AppRoutes.projectsPath,
            name: AppRoutes.projects,
            builder: (context, state) => const ProjectsPage(),
          ),
          GoRoute(
            path: AppRoutes.projectCreatePath,
            name: AppRoutes.projectCreate,
            builder: (context, state) => const ProjectCreatePage(),
          ),
          GoRoute(
            path: '/projects/:projectId',
            name: AppRoutes.projectDetails,
            builder: (context, state) => ProjectDetailsPage(
              projectId: state.pathParameters['projectId']!,
            ),
          ),
          GoRoute(
            path: '/projects/:projectId/edit',
            name: AppRoutes.projectEdit,
            builder: (context, state) =>
                ProjectEditPage(projectId: state.pathParameters['projectId']!),
          ),
          GoRoute(
            path: '/projects/:projectId/supervisor',
            name: AppRoutes.projectSupervisor,
            builder: (context, state) => ProjectSupervisorPage(
              projectId: state.pathParameters['projectId']!,
            ),
          ),
          GoRoute(
            path: '/projects/:projectId/members',
            name: AppRoutes.projectMembers,
            builder: (context, state) => ProjectMembersPage(
              projectId: state.pathParameters['projectId']!,
            ),
          ),
          GoRoute(
            path: AppRoutes.companyMembersPath,
            name: AppRoutes.companyMembers,
            builder: (context, state) => const CompanyMembersPage(),
          ),
          GoRoute(
            path: '/company/members/:memberId',
            name: AppRoutes.companyMemberDetails,
            builder: (context, state) => CompanyMemberDetailsPage(
              memberId: state.pathParameters['memberId']!,
            ),
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
