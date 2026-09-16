import '../../../core/network/api_client.dart';
import '../domain/settlement_models.dart';
import '../domain/settlement_repository.dart';

final class ApiSettlementRepository implements SettlementRepository {
  const ApiSettlementRepository(this._client);
  final ApiClient _client;
  @override
  Future<ProjectSettlement> getProjectSettlement(String projectId) =>
      _client.getProjectSettlement(projectId);
}
