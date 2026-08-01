import 'dart:io';

import 'package:ahdah_app/features/access/domain/access_models.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  test('access request models never send tenant or reviewer identifiers', () {
    const create = CreateInvitationInput(
      phoneNumber: '+218912345678',
      assignedRole: 'Worker',
    );
    const approve = ApproveJoinRequestInput(
      assignedRole: 'Supervisor',
      reviewNotes: null,
    );
    const reject = RejectJoinRequestInput(
      reason: 'Not eligible',
      reviewNotes: null,
    );

    for (final body in [create.toJson(), approve.toJson(), reject.toJson()]) {
      expect(body, isNot(contains('company_id')));
      expect(body, isNot(contains('companyId')));
      expect(body, isNot(contains('userId')));
      expect(body, isNot(contains('reviewerId')));
    }
  });

  test('web token store uses no browser persistence APIs', () {
    final source = File(
      'lib/core/security/web_access_token_store.dart',
    ).readAsStringSync();
    expect(source, isNot(contains('localStorage')));
    expect(source, isNot(contains('sessionStorage')));
    expect(source, isNot(contains('SharedPreferences')));
    expect(source, isNot(contains('IndexedDB')));
  });

  test('access feature has no persistence or password storage dependency', () {
    final sources = Directory('lib/features/access')
        .listSync(recursive: true)
        .whereType<File>()
        .where((file) => file.path.endsWith('.dart'))
        .map((file) => file.readAsStringSync())
        .join('\n');
    expect(sources, isNot(contains('SharedPreferences')));
    expect(sources, isNot(contains('flutter_secure_storage')));
    expect(sources, isNot(contains('localStorage')));
    expect(sources, isNot(contains('passwordHash')));
    expect(sources, isNot(contains('tokenHash')));
  });

  test('backend source contains no automatic schema mutation calls', () {
    final backend = Directory('../../backend/src')
        .listSync(recursive: true)
        .whereType<File>()
        .where((file) => file.path.endsWith('.cs'))
        .map((file) => file.readAsStringSync())
        .join('\n');
    expect(backend, isNot(contains('EnsureCreated(')));
    expect(backend, isNot(contains('EnsureDeleted(')));
    expect(backend, isNot(contains('Database.Migrate(')));
  });
}
