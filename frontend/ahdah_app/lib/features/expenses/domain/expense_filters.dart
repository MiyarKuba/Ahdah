final class ExpenseFilters {
  const ExpenseFilters({
    this.status,
    this.categoryId,
    this.projectId,
    this.incurredByUserId,
    this.submittedByUserId,
    this.paymentMode,
    this.reference,
  });

  final String? status;
  final String? categoryId;
  final String? projectId;
  final String? incurredByUserId;
  final String? submittedByUserId;
  final String? paymentMode;
  final String? reference;
}
