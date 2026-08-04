import '../../../core/network/api_client.dart';
import '../domain/expense_filters.dart';
import '../domain/expense_models.dart';
import '../domain/expense_repository.dart';
import '../domain/expense_requests.dart';

final class ApiExpenseRepository implements ExpenseRepository {
  const ApiExpenseRepository(this._client);
  final ApiClient _client;

  @override
  Future<ExpenseCategoryPage> listCategories({
    required int page,
    required int pageSize,
    String? categoryGroup,
    String? expenseScope,
  }) => _client.listExpenseCategories(
    page: page,
    pageSize: pageSize,
    categoryGroup: categoryGroup,
    expenseScope: expenseScope,
  );

  @override
  Future<ExpenseCategory> createCategory(CreateExpenseCategoryInput input) =>
      _client.createExpenseCategory(input);

  @override
  Future<ExpensePage> listExpenses({
    required int page,
    required int pageSize,
    required ExpenseFilters filters,
  }) => _client.listExpenses(page: page, pageSize: pageSize, filters: filters);

  @override
  Future<ExpenseDetails> getExpense(String expenseId) =>
      _client.getExpense(expenseId);

  @override
  Future<ExpenseAllocationPage> listAllocations(
    String expenseId, {
    required int page,
    required int pageSize,
  }) =>
      _client.listExpenseAllocations(expenseId, page: page, pageSize: pageSize);

  @override
  Future<ExpenseDocumentPage> listDocuments(
    String expenseId, {
    required int page,
    required int pageSize,
  }) => _client.listExpenseDocuments(expenseId, page: page, pageSize: pageSize);

  @override
  Future<ExpenseHistoryPage> listHistory(
    String expenseId, {
    required int page,
    required int pageSize,
  }) => _client.listExpenseHistory(expenseId, page: page, pageSize: pageSize);

  @override
  Future<ExpenseDetails> createExpense(
    CreateExpenseInput input,
    String idempotencyKey,
  ) => _client.createExpense(input, idempotencyKey);

  @override
  Future<ExpenseDetails> approveExpense(
    String expenseId,
    ApproveExpenseInput input,
    String idempotencyKey,
  ) => _client.approveExpense(expenseId, input, idempotencyKey);

  @override
  Future<ExpenseDetails> rejectExpense(
    String expenseId,
    RejectExpenseInput input,
    String idempotencyKey,
  ) => _client.rejectExpense(expenseId, input, idempotencyKey);

  @override
  Future<ReimbursementPage> listReimbursements({
    required int page,
    required int pageSize,
    String? status,
    String? claimantUserId,
  }) => _client.listReimbursements(
    page: page,
    pageSize: pageSize,
    status: status,
    claimantUserId: claimantUserId,
  );

  @override
  Future<ReimbursementSummary> getReimbursement(String reimbursementId) =>
      _client.getReimbursement(reimbursementId);
}
