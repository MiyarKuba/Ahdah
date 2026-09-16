import 'settlement_models.dart';

abstract interface class SettlementRepository {
  Future<ProjectSettlement> getProjectSettlement(String projectId);
}
