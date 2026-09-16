import 'dart:async';
import 'package:ahdah_app/core/errors/app_exception.dart';
import 'package:ahdah_app/features/settlements/domain/settlement_models.dart';
import 'package:ahdah_app/features/settlements/domain/settlement_repository.dart';

Map<String, Object?> settlementJson({String id = 'project-1'}) => {
  'projectId': id,
  'projectStatus': 'Completed',
  'projectVersion': 7,
  'evaluatedAt': '2026-09-16T10:15:00Z',
  'visibilityScope': 'CompanyFinancial',
  'hasKnownFinancialBlockers': false,
  'canSettle': null,
  'canClose': null,
  'settlementReadiness': 'Indeterminate',
  'closureReadiness': 'Indeterminate',
  'totalBlockerCount': 0,
  'categories': <Object?>[],
  'evaluationGaps': [
    {'category': 'Advances', 'code': 'ProjectAdvanceAttributionUnavailable'},
  ],
  'settlementImpediments': <String>[],
  'closureImpediments': <String>[],
};
Map<String, Object?> settlementCategory({
  String name = 'SupplierDebt',
  String evaluation = 'Evaluated',
}) => {
  'category': name,
  'evaluationStatus': evaluation,
  'amountMeaning': 'OutstandingLiability',
  'blockerCount': 1,
  'totals': [
    {'currencyCode': 'LYD', 'amount': '8500.01'},
    {'currencyCode': 'USD', 'amount': '300.09'},
  ],
  'blockers': [settlementBlocker()],
};
Map<String, Object?> settlementBlocker({
  String type = 'SupplierDebt',
  String id = 'debt-1',
}) => {
  'category': 'SupplierDebt',
  'code': 'OutstandingSupplierDebt',
  'recordType': type,
  'recordId': id,
  'status': 'Open',
  'currencyCode': 'LYD',
  'amount': '8500.01',
  'resourcePath': '/api/v1/supplier-debts/$id',
};

final class FakeSettlementRepository implements SettlementRepository {
  Map<String, Object?> json = settlementJson();
  int calls = 0;
  String? lastProjectId;
  AppException? error;
  Completer<ProjectSettlement>? completer;
  @override
  Future<ProjectSettlement> getProjectSettlement(String projectId) async {
    calls++;
    lastProjectId = projectId;
    if (error != null) throw error!;
    return completer?.future ??
        ProjectSettlement.fromJson({...json, 'projectId': projectId});
  }
}
