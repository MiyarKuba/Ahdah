import 'package:ahdah_app/app/app.dart';
import 'package:ahdah_app/app/providers.dart';
import 'package:ahdah_app/core/config/app_config.dart';
import 'package:ahdah_app/features/access/domain/role_capabilities.dart';
import 'package:ahdah_app/features/authentication/domain/identity_models.dart';
import 'package:ahdah_app/features/authenticated_home/presentation/home_page.dart';
import 'package:ahdah_app/features/company_members/domain/company_member_models.dart';
import 'package:ahdah_app/features/company_members/presentation/detail/company_member_details_page.dart';
import 'package:ahdah_app/features/company_members/presentation/list/company_members_page.dart';
import 'package:ahdah_app/features/projects/domain/project_models.dart';
import 'package:ahdah_app/features/projects/presentation/create/project_create_page.dart';
import 'package:ahdah_app/features/projects/presentation/detail/project_details_page.dart';
import 'package:ahdah_app/features/projects/presentation/edit/project_edit_page.dart';
import 'package:ahdah_app/features/projects/presentation/list/projects_page.dart';
import 'package:ahdah_app/features/projects/presentation/members/project_members_page.dart';
import 'package:ahdah_app/features/projects/presentation/supervisor/project_supervisor_page.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';

import 'helpers/fakes.dart';

void main() {
  test('role capabilities follow the conservative visibility matrix', () {
    final manager = RoleCapabilities.forRole('Manager');
    final deputy = RoleCapabilities.forRole('Deputy');
    final accountant = RoleCapabilities.forRole('Accountant');
    final supervisor = RoleCapabilities.forRole('Supervisor');
    final worker = RoleCapabilities.forRole('Worker');
    final unknown = RoleCapabilities.forRole('FutureRole');

    expect(manager.canManageProjects, isTrue);
    expect(manager.canViewContractValue, isTrue);
    expect(deputy.canViewCompanyDirectory, isTrue);
    expect(deputy.canManageProjects, isFalse);
    expect(accountant.canViewProjects, isTrue);
    expect(accountant.canViewProjectMembers, isFalse);
    expect(supervisor.requiresAssignedProjects, isTrue);
    expect(supervisor.canViewProjectMembers, isTrue);
    expect(worker.canViewProjects, isFalse);
    expect(unknown.canViewProjects, isFalse);
    expect(unknown.canViewCompanyDirectory, isFalse);
  });

  testWidgets('manager navigation exposes projects directory and create', (
    tester,
  ) async {
    await _pump(tester, role: 'Manager');

    expect(find.text('Projects'), findsWidgets);
    expect(find.text('Company members'), findsWidgets);
    _go(tester, '/projects');
    await tester.pumpAndSettle();
    expect(find.byType(ProjectsPage), findsOneWidget);
    expect(find.byKey(const Key('create-project-action')), findsOneWidget);

    _go(tester, '/projects/new');
    await tester.pumpAndSettle();
    expect(find.byType(ProjectCreatePage), findsOneWidget);
  });

  testWidgets('deputy receives read routes but manager routes are guarded', (
    tester,
  ) async {
    await _pump(tester, role: 'Deputy');

    expect(find.text('Projects'), findsWidgets);
    expect(find.text('Company members'), findsWidgets);
    _go(tester, '/projects/new');
    await tester.pumpAndSettle();
    expect(find.byType(HomePage), findsOneWidget);
    expect(find.byType(ProjectCreatePage), findsNothing);

    _go(tester, '/company/members');
    await tester.pumpAndSettle();
    expect(find.byType(CompanyMembersPage), findsOneWidget);
  });

  testWidgets('accountant sees projects but no directory or member route', (
    tester,
  ) async {
    await _pump(tester, role: 'Accountant');

    expect(find.text('Projects'), findsWidgets);
    expect(find.text('Company members'), findsNothing);
    _go(tester, '/projects/project-1/members');
    await tester.pumpAndSettle();
    expect(find.byType(HomePage), findsOneWidget);
    expect(find.byType(ProjectMembersPage), findsNothing);
  });

  testWidgets('supervisor empty state explains assigned-project visibility', (
    tester,
  ) async {
    await _pump(tester, role: 'Supervisor');
    _go(tester, '/projects');
    await tester.pumpAndSettle();

    expect(find.byType(ProjectsPage), findsOneWidget);
    expect(
      find.text('Only projects currently assigned to you appear here.'),
      findsOneWidget,
    );
    expect(find.text('Company members'), findsNothing);
  });

  testWidgets('worker and unknown roles have safe minimal navigation', (
    tester,
  ) async {
    await _pump(tester, role: 'Worker');
    expect(find.text('Projects'), findsNothing);
    _go(tester, '/projects');
    await tester.pumpAndSettle();
    expect(find.byType(HomePage), findsOneWidget);

    await _pump(tester, role: 'FutureRole');
    expect(find.text('Projects'), findsNothing);
    expect(find.text('Company members'), findsNothing);
  });

  testWidgets('contract value renders only with manager capability', (
    tester,
  ) async {
    await _pump(tester, role: 'Manager');
    _go(tester, '/projects/project-1');
    await tester.pumpAndSettle();
    expect(find.byType(ProjectDetailsPage), findsOneWidget);
    expect(find.textContaining('1,250.25 LYD'), findsOneWidget);

    await _pump(tester, role: 'Deputy');
    _go(tester, '/projects/project-1');
    await tester.pumpAndSettle();
    expect(find.textContaining('1,250.25 LYD'), findsNothing);
    expect(find.byKey(const Key('edit-project-action')), findsNothing);
  });

  testWidgets('edit UI offers only Active and Paused status values', (
    tester,
  ) async {
    await _pump(tester, role: 'Manager');
    _go(tester, '/projects/project-1/edit');
    await tester.pumpAndSettle();
    expect(find.byType(ProjectEditPage), findsOneWidget);

    await tester.tap(find.byKey(const Key('project-status-field')));
    await tester.pumpAndSettle();
    expect(find.text('Active'), findsWidgets);
    expect(find.text('Paused'), findsOneWidget);
    expect(find.text('Completed'), findsNothing);
    expect(find.text('Financially closed'), findsNothing);
    expect(find.text('Cancelled'), findsNothing);
  });

  testWidgets('project members are labelled as active supervision only', (
    tester,
  ) async {
    await _pump(tester, role: 'Manager');
    _go(tester, '/projects/project-1/members');
    await tester.pumpAndSettle();

    expect(find.byType(ProjectMembersPage), findsOneWidget);
    expect(
      find.textContaining('active supervision assignments only'),
      findsOneWidget,
    );
    expect(find.textContaining('complete project staff'), findsOneWidget);
  });

  testWidgets('company member detail is read-only and exposes no mutation', (
    tester,
  ) async {
    await _pump(tester, role: 'Manager');
    _go(tester, '/company/members/member-1');
    await tester.pumpAndSettle();

    expect(find.byType(CompanyMemberDetailsPage), findsOneWidget);
    expect(find.text('Read-only'), findsOneWidget);
    expect(find.text('Suspend'), findsNothing);
    expect(find.text('Delete'), findsNothing);
    expect(find.text('Change role'), findsNothing);
  });

  testWidgets('supervisor selector shows only active exact supervisors', (
    tester,
  ) async {
    final members = FakeCompanyMemberRepository()
      ..memberPage = const CompanyMemberPage(
        items: [
          CompanyMemberSummary(
            id: 'supervisor-1',
            fullName: 'Eligible Supervisor',
            role: 'Supervisor',
            status: 'Active',
            identityVerificationStatus: 'NotRequired',
          ),
          CompanyMemberSummary(
            id: 'manager-1',
            fullName: 'Unsafe Manager',
            role: 'Manager',
            status: 'Active',
            identityVerificationStatus: 'Verified',
          ),
          CompanyMemberSummary(
            id: 'supervisor-2',
            fullName: 'Suspended Supervisor',
            role: 'Supervisor',
            status: 'Suspended',
            identityVerificationStatus: 'NotRequired',
          ),
        ],
        page: 1,
        pageSize: 20,
        totalCount: 3,
        totalPages: 1,
      );
    await _pump(tester, role: 'Manager', members: members);
    _go(tester, '/projects/project-1/supervisor');
    await tester.pumpAndSettle();
    expect(find.byType(ProjectSupervisorPage), findsOneWidget);

    await tester.tap(find.widgetWithText(TextButton, 'Choose supervisor'));
    await tester.pumpAndSettle();
    expect(find.text('Eligible Supervisor'), findsOneWidget);
    expect(find.text('Unsafe Manager'), findsNothing);
    expect(find.text('Suspended Supervisor'), findsNothing);
    expect(members.lastRole, 'Supervisor');
    expect(members.lastStatus, 'Active');
  });
}

Future<void> _pump(
  WidgetTester tester, {
  required String role,
  FakeProjectRepository? projects,
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
  final projectRepository = projects ?? FakeProjectRepository();
  if (role != 'Supervisor') {
    projectRepository.projectPage = ProjectPage(
      items: [testProject],
      page: 1,
      pageSize: 20,
      totalCount: 1,
      totalPages: 1,
    );
  }
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
        projectRepositoryProvider.overrideWithValue(projectRepository),
        companyMemberRepositoryProvider.overrideWithValue(
          members ?? FakeCompanyMemberRepository(),
        ),
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
