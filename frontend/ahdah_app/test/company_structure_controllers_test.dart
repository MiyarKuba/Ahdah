import 'dart:async';

import 'package:ahdah_app/core/errors/app_exception.dart';
import 'package:ahdah_app/features/company_members/domain/company_member_list_state.dart';
import 'package:ahdah_app/features/company_members/domain/company_member_models.dart';
import 'package:ahdah_app/features/company_members/presentation/controllers/company_member_controllers.dart';
import 'package:ahdah_app/features/projects/domain/project_list_state.dart';
import 'package:ahdah_app/features/projects/domain/project_models.dart';
import 'package:ahdah_app/features/projects/domain/project_requests.dart';
import 'package:ahdah_app/features/projects/presentation/controllers/project_controllers.dart';
import 'package:flutter_test/flutter_test.dart';

import 'helpers/fakes.dart';

final _secondProject = ProjectDetails(
  id: 'project-2',
  projectName: 'Project Two',
  owner: testProject.owner,
  siteAddress: 'Benghazi',
  contractDate: DateTime(2026, 8, 1),
  startDate: DateTime(2026, 8, 2),
  status: 'Paused',
  assignedSupervisors: const [],
  versionNumber: 1,
);

const _memberOne = CompanyMemberSummary(
  id: 'member-1',
  fullName: 'Supervisor One',
  role: 'Supervisor',
  status: 'Active',
  identityVerificationStatus: 'NotRequired',
);

void main() {
  test('project initial load and refresh preserve filters', () async {
    final repository = FakeProjectRepository()
      ..projectPage = ProjectPage(
        items: [testProject],
        page: 1,
        pageSize: 20,
        totalCount: 1,
        totalPages: 1,
      );
    final controller = ProjectListController(repository, onUnauthorized: _noOp);

    await controller.setStatus('Active');
    await controller.refresh();

    expect(controller.state.phase, ProjectListPhase.data);
    expect(controller.state.status, 'Active');
    expect(repository.listCalls, 2);
    expect(repository.lastPage, 1);
  });

  test('project pagination appends and deduplicates by project id', () async {
    final repository = FakeProjectRepository()
      ..projectPage = ProjectPage(
        items: [testProject],
        page: 1,
        pageSize: 20,
        totalCount: 2,
        totalPages: 2,
      );
    final controller = ProjectListController(repository, onUnauthorized: _noOp);
    await controller.load();
    repository.projectPage = ProjectPage(
      items: [testProject, _secondProject],
      page: 2,
      pageSize: 20,
      totalCount: 2,
      totalPages: 2,
    );

    await controller.loadMore();

    expect(controller.state.items.map((item) => item.id), [
      'project-1',
      'project-2',
    ]);
    expect(controller.state.hasMore, isFalse);
  });

  test('one-character project search never reaches the repository', () async {
    final repository = FakeProjectRepository();
    final controller = ProjectListController(repository, onUnauthorized: _noOp);

    controller.setSearch('a', debounce: Duration.zero);
    await Future<void>.delayed(Duration.zero);

    expect(repository.listCalls, 0);
    expect(controller.state.phase, ProjectListPhase.empty);
  });

  test('project search resets page and sends bounded server search', () async {
    final repository = FakeProjectRepository();
    final controller = ProjectListController(repository, onUnauthorized: _noOp);

    controller.setSearch('Site', debounce: Duration.zero);
    await Future<void>.delayed(Duration.zero);

    expect(repository.lastSearch, 'Site');
    expect(repository.lastPage, 1);
  });

  test('stale project response cannot overwrite newer filter', () async {
    final repository = FakeProjectRepository();
    final first = Completer<ProjectPage>();
    final second = Completer<ProjectPage>();
    repository.listCompleter = first;
    final controller = ProjectListController(repository, onUnauthorized: _noOp);
    final firstLoad = controller.load();
    repository.listCompleter = second;
    final secondLoad = controller.setStatus('Paused');
    second.complete(
      ProjectPage(
        items: [_secondProject],
        page: 1,
        pageSize: 20,
        totalCount: 1,
        totalPages: 1,
      ),
    );
    await secondLoad;
    first.complete(
      ProjectPage(
        items: [testProject],
        page: 1,
        pageSize: 20,
        totalCount: 1,
        totalPages: 1,
      ),
    );
    await firstLoad;

    expect(controller.state.status, 'Paused');
    expect(controller.state.items.single.id, 'project-2');
  });

  test(
    'recoverable project load-more failure preserves loaded items',
    () async {
      final repository = FakeProjectRepository()
        ..projectPage = ProjectPage(
          items: [testProject],
          page: 1,
          pageSize: 20,
          totalCount: 2,
          totalPages: 2,
        );
      final controller = ProjectListController(
        repository,
        onUnauthorized: _noOp,
      );
      await controller.load();
      repository.listError = const AppException(AppExceptionKind.network);

      await controller.loadMore();

      expect(controller.state.items.single.id, 'project-1');
      expect(controller.state.loadMoreError?.kind, AppExceptionKind.network);
    },
  );

  test('project create prevents duplicate submissions', () async {
    final repository = FakeProjectRepository()
      ..createCompleter = Completer<ProjectDetails>();
    final controller = ProjectCreateController(
      repository,
      onUnauthorized: _noOp,
    );
    final input = ProjectCreateInput(
      projectName: 'Project',
      siteAddress: 'Site',
      contractValue: '10.00',
      contractDate: DateTime(2026, 8, 1),
      startDate: DateTime(2026, 8, 1),
      newOwner: const NewProjectOwnerInput(
        ownerName: 'Owner',
        phoneNumber: '+218912345678',
      ),
    );

    final first = controller.create(input);
    final duplicate = await controller.create(input);

    expect(repository.createCalls, 1);
    expect(duplicate, isNull);
    repository.createCompleter!.complete(testProject);
    expect(await first, testProject);
  });

  test(
    'update and supervisor conflicts remain explicit and reloadable',
    () async {
      final repository = FakeProjectRepository()
        ..writeError = const AppException(AppExceptionKind.conflict);
      final update = ProjectUpdateController(repository, onUnauthorized: _noOp);
      final assignment = SupervisorAssignmentController(
        repository,
        onUnauthorized: _noOp,
      );

      expect(
        await update.update(
          'project-1',
          const ProjectUpdateInput(expectedVersion: 3, status: 'Paused'),
        ),
        isNull,
      );
      expect(update.state.isConflict, isTrue);
      expect(
        await assignment.assign(
          'project-1',
          const SupervisorAssignmentInput(
            supervisorUserId: 'supervisor-1',
            expectedVersion: 3,
          ),
        ),
        isNull,
      );
      expect(assignment.state.isConflict, isTrue);
    },
  );

  test(
    'company member filters reset pagination and remain server-side',
    () async {
      final repository = FakeCompanyMemberRepository()
        ..memberPage = const CompanyMemberPage(
          items: [_memberOne],
          page: 1,
          pageSize: 20,
          totalCount: 1,
          totalPages: 1,
        );
      final controller = CompanyMemberListController(
        repository,
        onUnauthorized: _noOp,
      );

      await controller.setRole('Supervisor');
      await controller.setStatus('Active');
      controller.setSearch('Su', debounce: Duration.zero);
      await Future<void>.delayed(Duration.zero);

      expect(repository.lastPage, 1);
      expect(repository.lastRole, 'Supervisor');
      expect(repository.lastStatus, 'Active');
      expect(repository.lastSearch, 'Su');
    },
  );

  test('company member list invokes session expiry only on 401', () async {
    var expired = 0;
    final repository = FakeCompanyMemberRepository()
      ..listError = const AppException(AppExceptionKind.unauthorized);
    final controller = CompanyMemberListController(
      repository,
      onUnauthorized: () async => expired++,
    );

    await controller.load();

    expect(expired, 1);
    expect(controller.state.phase, CompanyMemberListPhase.failure);
  });

  test('project members paginate active assignment summaries', () async {
    final member = ProjectMemberSummary(
      id: 'member-1',
      fullName: 'Supervisor One',
      role: 'Supervisor',
      status: 'Active',
      identityVerificationStatus: 'NotRequired',
      phoneNumber: '+218912345679',
      assignedAtUtc: DateTime.utc(2026, 8, 1),
    );
    final repository = FakeProjectRepository()
      ..memberPage = ProjectMemberPage(
        items: [member],
        page: 1,
        pageSize: 20,
        totalCount: 1,
        totalPages: 1,
      );
    final controller = ProjectMembersController(
      repository,
      'project-1',
      onUnauthorized: _noOp,
    );

    await controller.load();

    expect(controller.state.phase, ProjectMembersPhase.data);
    expect(controller.state.items.single.role, 'Supervisor');
    expect(repository.lastPageSize, 20);
  });
}

Future<void> _noOp() async {}
