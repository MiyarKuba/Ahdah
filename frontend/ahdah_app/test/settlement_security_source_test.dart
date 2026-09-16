import 'dart:io';
import 'package:flutter_test/flutter_test.dart';
import 'package:ahdah_app/features/settlements/presentation/settlement_labels.dart';
import 'package:ahdah_app/l10n/app_localizations_en.dart';
import 'package:ahdah_app/l10n/app_localizations_ar.dart';

void main() {
  final source = Directory('lib/features/settlements')
      .listSync(recursive: true)
      .whereType<File>()
      .where((f) => f.path.endsWith('.dart'))
      .map((f) => f.readAsStringSync())
      .join('\n');
  test(
    'read-only settlement has no tenant authority, direct database, persistence or financial writes',
    () {
      for (final forbidden in [
        "'companyId'",
        "'company_id'",
        "'actorId'",
        'PostgreSQL',
        'postgres://',
        'SharedPreferences',
        'SecureStorage',
        'debugPrint(',
        'print(',
        'Idempotency-Key',
        'double.parse',
        '.post(',
        '.patch(',
        '.put(',
        '.delete(',
        'finalizeSettlement',
        'financialClose',
      ]) {
        expect(source.contains(forbidden), isFalse, reason: forbidden);
      }
      expect(RegExp(r'\bdouble\b').hasMatch(source), isFalse);
      expect(source.contains('.fold('), isFalse);
      expect(source.contains('.reduce('), isFalse);
    },
  );
  test('navigation never consumes backend resourcePath', () {
    final navigation = File(
      'lib/features/settlements/presentation/settlement_navigation.dart',
    ).readAsStringSync();
    expect(navigation.contains('resourcePath'), isFalse);
    expect(navigation.contains('go('), isFalse);
  });
  test(
    'all currently emitted reason and gap codes have English and Arabic labels',
    () {
      final rules = File(
        '../../backend/src/Ahdah.Application/Settlements/SettlementRules.cs',
      ).readAsStringSync();
      final service = File(
        '../../backend/src/Ahdah.Infrastructure/Settlements/ProjectSettlementService.cs',
      ).readAsStringSync();
      final queries = File(
        '../../backend/src/Ahdah.Infrastructure/Settlements/ProjectSettlementQueries.cs',
      ).readAsStringSync();
      final codes = RegExp(r'"([A-Za-z]+)"')
          .allMatches('$rules\n$service\n$queries')
          .map((m) => m[1]!)
          .where(
            (code) =>
                code.startsWith('Unknown') ||
                code.startsWith('Project') &&
                    code != 'ProjectLifecycle' &&
                    code != 'ProjectContractChange' ||
                code.startsWith('Financial') && code != 'FinanciallyClosed' ||
                code.startsWith('Outstanding') ||
                code.startsWith('Unsettled') ||
                code.startsWith('Unresolved') ||
                code.startsWith('MissingExpense') ||
                code.startsWith('ExpenseDraft') ||
                code.startsWith('ExpensePending') ||
                code.startsWith('ExpenseCorrection') ||
                code.startsWith('KnownFinancial') ||
                code.startsWith('ApprovedReturn') ||
                code.startsWith('UnallocatedCredit') ||
                code.startsWith('ActiveDocument') ||
                code.startsWith('DocumentPolicy') ||
                code.startsWith('Pending') &&
                    ![
                      'PendingReview',
                      'PendingApproval',
                      'PendingVerification',
                    ].contains(code),
          )
          .toSet();
      for (final code in codes.where((c) => c != 'Unknown')) {
        expect(
          SettlementLabels.code(AppLocalizationsEn(), code),
          isNot(AppLocalizationsEn().settlementUnknown),
          reason: code,
        );
        expect(
          SettlementLabels.code(AppLocalizationsAr(), code),
          isNot(AppLocalizationsAr().settlementUnknown),
          reason: code,
        );
      }
    },
  );
}
