import '../../../core/network/api_client.dart';
import '../domain/advance_models.dart';
import '../domain/advance_repository.dart';
import '../domain/advance_requests.dart';

final class ApiAdvanceRepository implements AdvanceRepository {
  const ApiAdvanceRepository(this._client);

  final ApiClient _client;

  @override
  Future<AdvancePage> listAdvances({
    required int page,
    required int pageSize,
    String? status,
    String? userId,
    String? reference,
  }) => _client.listAdvances(
    page: page,
    pageSize: pageSize,
    status: status,
    userId: userId,
    reference: reference,
  );

  @override
  Future<AdvanceDetails> getAdvance(String advanceId) =>
      _client.getAdvance(advanceId);

  @override
  Future<AdvanceMovementPage> listMovements(
    String advanceId, {
    required int page,
    required int pageSize,
  }) => _client.listAdvanceMovements(advanceId, page: page, pageSize: pageSize);

  @override
  Future<FundingSourcePage> listFundingSources({
    required int page,
    required int pageSize,
  }) => _client.listAdvanceFundingSources(page: page, pageSize: pageSize);

  @override
  Future<AdvanceBalancePage> getMyBalances({
    required int page,
    required int pageSize,
  }) => _client.getMyAdvanceBalances(page: page, pageSize: pageSize);

  @override
  Future<AdvanceBalancePage> getUserBalances(
    String userId, {
    required int page,
    required int pageSize,
  }) => _client.getUserAdvanceBalances(userId, page: page, pageSize: pageSize);

  @override
  Future<AdvanceDetails> createAdvance(
    CreateAdvanceInput input,
    String idempotencyKey,
  ) => _client.createAdvance(input, idempotencyKey);

  @override
  Future<AdvanceTransferDetails> distribute(
    String advanceId,
    CreateAdvanceDistributionInput input,
    String idempotencyKey,
  ) => _client.distributeAdvance(advanceId, input, idempotencyKey);

  @override
  Future<AdvanceTransferDetails> returnMoney(
    String advanceId,
    CreateAdvanceReturnInput input,
    String idempotencyKey,
  ) => _client.returnAdvanceMoney(advanceId, input, idempotencyKey);

  @override
  Future<AdvanceTransferDetails> confirmTransfer(
    String transferId,
    String idempotencyKey,
  ) => _client.confirmAdvanceTransfer(transferId, idempotencyKey);

  @override
  Future<AdvanceTransferDetails> rejectTransfer(
    String transferId,
    RejectAdvanceTransferInput input,
    String idempotencyKey,
  ) => _client.rejectAdvanceTransfer(transferId, input, idempotencyKey);
}
