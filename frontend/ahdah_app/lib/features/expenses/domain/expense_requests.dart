import 'dart:convert';

final class ExpenseAdvanceAllocationInput {
  const ExpenseAdvanceAllocationInput({
    required this.userAdvanceBalanceId,
    required this.amount,
    this.notes,
  });

  final String userAdvanceBalanceId;
  final String amount;
  final String? notes;
}

final class CreateExpenseInput {
  const CreateExpenseInput({
    required this.expenseCategoryId,
    required this.expenseDate,
    required this.amount,
    required this.currencyCode,
    required this.paymentMode,
    required this.description,
    this.projectId,
    this.invoiceNumber,
    this.receiptNumber,
    this.merchantName,
    this.expenseLocation,
    this.notes,
    this.advanceAllocations = const [],
  });

  final String expenseCategoryId;
  final String? projectId;
  final DateTime expenseDate;
  final String amount;
  final String currencyCode;
  final String paymentMode;
  final String description;
  final String? invoiceNumber;
  final String? receiptNumber;
  final String? merchantName;
  final String? expenseLocation;
  final String? notes;
  final List<ExpenseAdvanceAllocationInput> advanceAllocations;

  String get payloadFingerprint => toJsonBody();

  String toJsonBody() {
    const amountMarker = '__AHDAH_EXPENSE_AMOUNT__';
    final allocationMarkers = List.generate(
      advanceAllocations.length,
      (index) =>
          '__AHDAH_EXPENSE_ALLOCATION_$index'
          '__',
    );
    final encoded = jsonEncode({
      'expenseCategoryId': expenseCategoryId,
      'projectId': _optional(projectId),
      'expenseDate': _date(expenseDate),
      'amount': amountMarker,
      'currencyCode': currencyCode.trim().toUpperCase(),
      'paymentMode': paymentMode,
      'description': description.trim(),
      'invoiceNumber': _optional(invoiceNumber),
      'receiptNumber': _optional(receiptNumber),
      'merchantName': _optional(merchantName),
      'expenseLocation': _optional(expenseLocation),
      'notes': _optional(notes),
      'advanceAllocations': [
        for (var index = 0; index < advanceAllocations.length; index++)
          {
            'userAdvanceBalanceId':
                advanceAllocations[index].userAdvanceBalanceId,
            'amount': allocationMarkers[index],
            'notes': _optional(advanceAllocations[index].notes),
          },
      ],
    });
    var result = encoded.replaceFirst('"$amountMarker"', amount);
    for (var index = 0; index < allocationMarkers.length; index++) {
      result = result.replaceFirst(
        '"${allocationMarkers[index]}"',
        advanceAllocations[index].amount,
      );
    }
    return result;
  }
}

final class CreateExpenseCategoryInput {
  const CreateExpenseCategoryInput({
    required this.categoryName,
    required this.categoryGroup,
    required this.expenseScope,
    this.parentExpenseCategoryId,
    this.categoryCode,
    this.description,
    this.requiresSupplier = false,
    this.requiresReceipt = true,
    this.supportsQuantityDetails = false,
    this.displayOrder = 0,
  });

  final String? parentExpenseCategoryId;
  final String? categoryCode;
  final String categoryName;
  final String categoryGroup;
  final String expenseScope;
  final String? description;
  final bool requiresSupplier;
  final bool requiresReceipt;
  final bool supportsQuantityDetails;
  final int displayOrder;

  Map<String, Object?> toJson() => {
    'parentExpenseCategoryId': _optional(parentExpenseCategoryId),
    'categoryCode': _optional(categoryCode)?.toUpperCase(),
    'categoryName': categoryName.trim(),
    'categoryGroup': categoryGroup,
    'expenseScope': expenseScope,
    'description': _optional(description),
    'requiresSupplier': requiresSupplier,
    'requiresReceipt': requiresReceipt,
    'supportsQuantityDetails': supportsQuantityDetails,
    'displayOrder': displayOrder,
  };
}

final class ApproveExpenseInput {
  const ApproveExpenseInput({required this.expectedVersion});
  final int expectedVersion;
  String get payloadFingerprint => jsonEncode(toJson());
  Map<String, Object?> toJson() => {'expectedVersion': expectedVersion};
}

final class RejectExpenseInput {
  const RejectExpenseInput({
    required this.expectedVersion,
    required this.reason,
  });

  final int expectedVersion;
  final String reason;
  String get payloadFingerprint => jsonEncode(toJson());
  Map<String, Object?> toJson() => {
    'expectedVersion': expectedVersion,
    'reason': reason.trim(),
  };
}

String _date(DateTime value) =>
    '${value.year.toString().padLeft(4, '0')}-'
    '${value.month.toString().padLeft(2, '0')}-'
    '${value.day.toString().padLeft(2, '0')}';

String? _optional(String? value) {
  final normalized = value?.trim();
  return normalized == null || normalized.isEmpty ? null : normalized;
}
