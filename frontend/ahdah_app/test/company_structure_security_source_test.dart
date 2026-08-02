import 'dart:io';

import 'package:ahdah_app/features/projects/domain/project_requests.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  test('project request contracts never carry tenant or actor scope', () {
    final create = ProjectCreateInput(
      projectName: 'Project',
      siteAddress: 'Site',
      contractValue: '10.00',
      contractDate: DateTime(2026, 8, 1),
      startDate: DateTime(2026, 8, 1),
      newOwner: const NewProjectOwnerInput(
        ownerName: 'Owner',
        phoneNumber: '+218912345678',
      ),
    ).toJson();
    const update = ProjectUpdateInput(
      expectedVersion: 1,
      projectName: 'Updated',
    );
    const supervisor = SupervisorAssignmentInput(
      supervisorUserId: 'supervisor-1',
      expectedVersion: 1,
    );

    for (final body in [create, update.toJson(), supervisor.toJson()]) {
      expect(body, isNot(contains('company_id')));
      expect(body, isNot(contains('companyId')));
      expect(body, isNot(contains('creatorId')));
      expect(body, isNot(contains('createdByUserId')));
    }
  });

  test('project feature performs no binary floating-point calculations', () {
    final source = _dartSources('lib/features/projects');
    expect(source, isNot(contains('double.parse')));
    expect(source, isNot(contains('double.tryParse')));
    expect(source, isNot(contains('remainingContractValue')));
    expect(source, isNot(contains('contractValue +')));
    expect(source, isNot(contains('contractValue -')));
  });

  test('company structure features contain no secret persistence', () {
    final source = [
      _dartSources('lib/features/projects'),
      _dartSources('lib/features/company_members'),
    ].join('\n');
    expect(source, isNot(contains('SharedPreferences')));
    expect(source, isNot(contains('flutter_secure_storage')));
    expect(source, isNot(contains('passwordHash')));
    expect(source, isNot(contains('tokenHash')));
    expect(source, isNot(contains('accessToken')));
  });

  test('no financial endpoint or deferred lifecycle write is present', () {
    final source = _dartSources('lib/features/projects');
    for (final endpoint in [
      '/advances',
      '/expenses',
      '/settlements',
      '/suppliers',
      '/debts',
      '/reports',
    ]) {
      expect(source, isNot(contains(endpoint)));
    }
    expect(source, isNot(contains('completeProject')));
    expect(source, isNot(contains('cancelProject')));
    expect(source, isNot(contains('financiallyCloseProject')));
  });

  test('company directory domain remains read-only', () {
    final repository = File(
      'lib/features/company_members/domain/company_member_repository.dart',
    ).readAsStringSync();
    expect(repository, contains('listMembers'));
    expect(repository, contains('getMember'));
    expect(repository, isNot(contains('updateMember')));
    expect(repository, isNot(contains('deleteMember')));
    expect(repository, isNot(contains('suspendMember')));
    expect(repository, isNot(contains('verifyIdentity')));
  });
}

String _dartSources(String path) => Directory(path)
    .listSync(recursive: true)
    .whereType<File>()
    .where((file) => file.path.endsWith('.dart'))
    .map((file) => file.readAsStringSync())
    .join('\n');
