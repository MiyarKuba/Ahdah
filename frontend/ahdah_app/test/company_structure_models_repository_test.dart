import 'dart:convert';

import 'package:ahdah_app/core/network/api_client.dart';
import 'package:ahdah_app/core/network/auth_interceptor.dart';
import 'package:ahdah_app/features/company_members/domain/company_member_models.dart';
import 'package:ahdah_app/features/projects/domain/project_models.dart';
import 'package:ahdah_app/features/projects/domain/project_requests.dart';
import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';

import 'helpers/fakes.dart';

const _projectJson = {
  'projectId': 'project-1',
  'projectName': 'Project One',
  'owner': {
    'projectOwnerId': 'owner-1',
    'ownerName': 'Owner One',
    'phoneNumber': '+218912345678',
    'email': 'owner@example.com',
    'address': 'Tripoli',
  },
  'siteAddress': 'Tripoli site',
  'latitude': 32.8872,
  'longitude': 13.1913,
  'contactPhoneNumber': '+218912345679',
  'contractValue': 1250.25,
  'contractDate': '2026-08-01',
  'startDate': '2026-08-02',
  'expectedEndDate': '2027-08-02',
  'actualEndDate': null,
  'status': 'Active',
  'description': 'Site work',
  'notes': null,
  'assignedSupervisors': [
    {
      'userId': 'supervisor-1',
      'fullName': 'Supervisor One',
      'status': 'Active',
      'assignedAtUtc': '2026-08-01T10:30:00Z',
    },
  ],
  'versionNumber': 3,
  'createdAtUtc': '2026-08-01T09:00:00Z',
  'updatedAtUtc': '2026-08-01T10:30:00Z',
};

const _memberSummaryJson = {
  'memberId': 'member-1',
  'fullName': 'Supervisor One',
  'role': 'Supervisor',
  'status': 'Active',
  'identityVerificationStatus': 'NotRequired',
  'createdAtUtc': '2026-08-01T09:00:00Z',
};

void main() {
  test('project page parses exact pagination and nested response fields', () {
    final page = ProjectPage.fromJson(const {
      'items': [_projectJson],
      'page': 1,
      'pageSize': 20,
      'totalCount': 21,
      'totalPages': 2,
    });

    expect(page.hasMore, isTrue);
    expect(page.totalCount, 21);
    expect(page.items.single.owner.ownerName, 'Owner One');
    expect(
      page.items.single.assignedSupervisors.single.fullName,
      'Supervisor One',
    );
  });

  test(
    'contract decimal text and UTC timestamps are preserved deliberately',
    () {
      final project = ProjectDetails.fromJson(_projectJson);

      expect(project.contractValue, '1250.25');
      expect(project.createdAtUtc, DateTime.utc(2026, 8, 1, 9));
      expect(project.contractDate, DateTime(2026, 8, 1));
    },
  );

  test('missing contract value and malformed optional dates do not crash', () {
    final json = Map<String, Object?>.of(_projectJson)
      ..remove('contractValue')
      ..['expectedEndDate'] = 'not-a-date'
      ..['status'] = 'FutureStatus';
    final project = ProjectDetails.fromJson(json);

    expect(project.contractValue, isNull);
    expect(project.expectedEndDate, isNull);
    expect(project.status, 'FutureStatus');
  });

  test('project member response parses active assignment fields', () {
    final page = ProjectMemberPage.fromJson(const {
      'items': [
        {
          'memberId': 'member-1',
          'fullName': 'Supervisor One',
          'role': 'Supervisor',
          'status': 'Active',
          'identityVerificationStatus': 'NotRequired',
          'phoneNumber': '+218912345679',
          'email': null,
          'assignedAtUtc': '2026-08-01T10:30:00Z',
        },
      ],
      'page': 1,
      'pageSize': 20,
      'totalCount': 1,
      'totalPages': 1,
    });

    expect(page.items.single.role, 'Supervisor');
    expect(page.items.single.assignedAtUtc, DateTime.utc(2026, 8, 1, 10, 30));
  });

  test('company member list and detail use distinct safe shapes', () {
    final page = CompanyMemberPage.fromJson(const {
      'items': [_memberSummaryJson],
      'page': 1,
      'pageSize': 20,
      'totalCount': 1,
      'totalPages': 1,
    });
    final detail = CompanyMemberDetails.fromJson(const {
      ..._memberSummaryJson,
      'phoneNumber': '+218912345679',
      'email': 'supervisor@example.com',
      'updatedAtUtc': '2026-08-02T09:00:00Z',
    });

    expect(page.items.single.isEligibleSupervisor, isTrue);
    expect(detail.phoneNumber, '+218912345679');
    expect(detail.email, 'supervisor@example.com');
  });

  test('project list sends exact bounded server query', () async {
    final adapter = RecordingAdapter(
      body: const {
        'items': [],
        'page': 2,
        'pageSize': 20,
        'totalCount': 0,
        'totalPages': 0,
      },
    );
    await _client(
      adapter,
    ).listProjects(page: 2, pageSize: 20, status: 'Paused', search: 'Site');

    expect(adapter.lastRequest!.path, '/api/v1/projects');
    expect(adapter.lastRequest!.queryParameters, {
      'page': 2,
      'pageSize': 20,
      'status': 'Paused',
      'search': 'Site',
    });
  });

  test('project detail and members use exact authenticated routes', () async {
    final detailAdapter = RecordingAdapter(body: _projectJson);
    await _client(detailAdapter).getProject('project-1');
    expect(detailAdapter.lastRequest!.path, '/api/v1/projects/project-1');

    final membersAdapter = RecordingAdapter(
      body: const {
        'items': [],
        'page': 1,
        'pageSize': 20,
        'totalCount': 0,
        'totalPages': 0,
      },
    );
    await _client(
      membersAdapter,
    ).listProjectMembers('project-1', page: 1, pageSize: 20);
    expect(
      membersAdapter.lastRequest!.path,
      '/api/v1/projects/project-1/members',
    );
  });

  test(
    'creation emits validated contract decimal as JSON number only',
    () async {
      final adapter = RecordingAdapter(statusCode: 201, body: _projectJson);
      final input = ProjectCreateInput(
        projectName: 'Project One',
        siteAddress: 'Tripoli site',
        contractValue: '1250.25',
        contractDate: DateTime(2026, 8, 1),
        startDate: DateTime(2026, 8, 2),
        newOwner: const NewProjectOwnerInput(
          ownerName: 'Owner One',
          phoneNumber: '+218912345678',
        ),
      );
      await _client(adapter).createProject(input);

      final bodyText = adapter.lastRequest!.data as String;
      expect(bodyText, contains('"contractValue":1250.25'));
      expect(bodyText, isNot(contains('companyId')));
      expect(bodyText, isNot(contains('creator')));
      expect(
        (jsonDecode(bodyText) as Map)['newOwner']['ownerName'],
        'Owner One',
      );
    },
  );

  test(
    'update sends expected version and supported changed fields only',
    () async {
      final adapter = RecordingAdapter(body: _projectJson);
      await _client(adapter).updateProject(
        'project-1',
        const ProjectUpdateInput(
          expectedVersion: 3,
          projectName: 'Renamed',
          status: 'Paused',
        ),
      );

      expect(adapter.lastRequest!.method, 'PATCH');
      expect(adapter.lastRequest!.data, {
        'expectedVersion': 3,
        'projectName': 'Renamed',
        'status': 'Paused',
      });
    },
  );

  test('supervisor assignment sends exact concurrency body', () async {
    final adapter = RecordingAdapter(body: _projectJson);
    await _client(adapter).assignProjectSupervisor(
      'project-1',
      const SupervisorAssignmentInput(
        supervisorUserId: 'supervisor-1',
        expectedVersion: 3,
      ),
    );

    expect(adapter.lastRequest!.path, '/api/v1/projects/project-1/supervisor');
    expect(adapter.lastRequest!.data, {
      'supervisorUserId': 'supervisor-1',
      'expectedVersion': 3,
    });
  });

  test(
    'company directory sends exact role status and search filters',
    () async {
      final adapter = RecordingAdapter(
        body: const {
          'items': [],
          'page': 1,
          'pageSize': 20,
          'totalCount': 0,
          'totalPages': 0,
        },
      );
      await _client(adapter).listCompanyMembers(
        page: 1,
        pageSize: 20,
        role: 'Supervisor',
        status: 'Active',
        search: 'Su',
      );

      expect(adapter.lastRequest!.path, '/api/v1/company/members');
      expect(adapter.lastRequest!.queryParameters, {
        'page': 1,
        'pageSize': 20,
        'role': 'Supervisor',
        'status': 'Active',
        'search': 'Su',
      });
    },
  );

  test('company member detail uses exact route', () async {
    final adapter = RecordingAdapter(
      body: const {
        ..._memberSummaryJson,
        'phoneNumber': '+218912345679',
        'email': null,
        'updatedAtUtc': '2026-08-02T09:00:00Z',
      },
    );
    await _client(adapter).getCompanyMember('member-1');
    expect(adapter.lastRequest!.path, '/api/v1/company/members/member-1');
  });
}

ApiClient _client(RecordingAdapter adapter) {
  final dio = Dio(BaseOptions(baseUrl: 'http://localhost:5231'))
    ..httpClientAdapter = adapter
    ..interceptors.add(AuthInterceptor(FakeAccessTokenStore('test-token')));
  return ApiClient(dio);
}
