import 'expense_filters.dart';
import 'expense_models.dart';
import 'expense_requests.dart';

abstract interface class ExpenseRepository {
  Future<ExpenseCategoryPage> listCategories({
    required int page,
    required int pageSize,
    String? categoryGroup,
    String? expenseScope,
  });

  Future<ExpenseCategory> createCategory(CreateExpenseCategoryInput input);

  Future<ExpensePage> listExpenses({
    required int page,
    required int pageSize,
    required ExpenseFilters filters,
  });

  Future<ExpenseDetails> getExpense(String expenseId);
  Future<ExpenseAllocationPage> listAllocations(
    String expenseId, {
    required int page,
    required int pageSize,
  });
  Future<ExpenseDocumentPage> listDocuments(
    String expenseId, {
    required int page,
    required int pageSize,
  });
  Future<ExpenseHistoryPage> listHistory(
    String expenseId, {
    required int page,
    required int pageSize,
  });
  Future<ExpenseDetails> createExpense(
    CreateExpenseInput input,
    String idempotencyKey,
  );
  Future<ExpenseDetails> approveExpense(
    String expenseId,
    ApproveExpenseInput input,
    String idempotencyKey,
  );
  Future<ExpenseDetails> rejectExpense(
    String expenseId,
    RejectExpenseInput input,
    String idempotencyKey,
  );
  Future<ReimbursementPage> listReimbursements({
    required int page,
    required int pageSize,
    String? status,
    String? claimantUserId,
  });
  Future<ReimbursementSummary> getReimbursement(String reimbursementId);
}
