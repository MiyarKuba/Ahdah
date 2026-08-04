import 'advance_models.dart';
import 'advance_requests.dart';

abstract interface class AdvanceRepository {
  Future<AdvancePage> listAdvances({
    required int page,
    required int pageSize,
    String? status,
    String? userId,
    String? reference,
  });

  Future<AdvanceDetails> getAdvance(String advanceId);

  Future<AdvanceMovementPage> listMovements(
    String advanceId, {
    required int page,
    required int pageSize,
  });

  Future<FundingSourcePage> listFundingSources({
    required int page,
    required int pageSize,
  });

  Future<AdvanceBalancePage> getMyBalances({
    required int page,
    required int pageSize,
  });

  Future<AdvanceBalancePage> getUserBalances(
    String userId, {
    required int page,
    required int pageSize,
  });

  Future<AdvanceDetails> createAdvance(
    CreateAdvanceInput input,
    String idempotencyKey,
  );

  Future<AdvanceTransferDetails> distribute(
    String advanceId,
    CreateAdvanceDistributionInput input,
    String idempotencyKey,
  );

  Future<AdvanceTransferDetails> returnMoney(
    String advanceId,
    CreateAdvanceReturnInput input,
    String idempotencyKey,
  );

  Future<AdvanceTransferDetails> confirmTransfer(
    String transferId,
    String idempotencyKey,
  );

  Future<AdvanceTransferDetails> rejectTransfer(
    String transferId,
    RejectAdvanceTransferInput input,
    String idempotencyKey,
  );
}
