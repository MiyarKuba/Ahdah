final class AdvancePage {
  const AdvancePage({
    required this.items,
    required this.page,
    required this.pageSize,
    required this.totalCount,
    required this.totalPages,
  });

  final List<AdvanceSummary> items;
  final int page;
  final int pageSize;
  final int totalCount;
  final int totalPages;

  bool get hasMore => page < totalPages;

  factory AdvancePage.fromJson(Map<String, Object?> json) => AdvancePage(
    items: _items(json['items'], AdvanceSummary.fromJson),
    page: _integer(json['page'], fallback: 1),
    pageSize: _integer(json['pageSize'], fallback: 20),
    totalCount: _integer(json['totalCount']),
    totalPages: _integer(json['totalPages']),
  );
}

final class AdvanceUserSummary {
  const AdvanceUserSummary({
    required this.userId,
    required this.fullName,
    required this.role,
  });

  final String userId;
  final String fullName;
  final String role;

  factory AdvanceUserSummary.fromJson(Map<String, Object?> json) =>
      AdvanceUserSummary(
        userId: _string(json['userId']),
        fullName: _string(json['fullName']),
        role: _string(json['role']),
      );
}

final class AdvanceSummary {
  const AdvanceSummary({
    required this.advanceId,
    required this.advanceNumber,
    required this.advanceAmount,
    required this.availableAmount,
    required this.reservedAmount,
    required this.currencyCode,
    required this.issueDate,
    required this.purpose,
    required this.status,
    required this.recipient,
    required this.versionNumber,
    required this.createdAtUtc,
    this.settlementDueDate,
    this.notes,
    this.confirmedAtUtc,
  });

  final String advanceId;
  final String advanceNumber;
  final String advanceAmount;
  final String availableAmount;
  final String reservedAmount;
  final String currencyCode;
  final DateTime? issueDate;
  final DateTime? settlementDueDate;
  final String purpose;
  final String? notes;
  final String status;
  final AdvanceUserSummary recipient;
  final int versionNumber;
  final DateTime? createdAtUtc;
  final DateTime? confirmedAtUtc;

  factory AdvanceSummary.fromJson(Map<String, Object?> json) => AdvanceSummary(
    advanceId: _string(json['advanceId']),
    advanceNumber: _string(json['advanceNumber']),
    advanceAmount: decimalText(json['advanceAmount']),
    availableAmount: decimalText(json['availableAmount']),
    reservedAmount: decimalText(json['reservedAmount']),
    currencyCode: _string(json['currencyCode']),
    issueDate: _dateOnly(json['issueDate']),
    settlementDueDate: _dateOnly(json['settlementDueDate']),
    purpose: _string(json['purpose']),
    notes: _optionalString(json['notes']),
    status: _string(json['status']),
    recipient: AdvanceUserSummary.fromJson(_map(json['recipient'])),
    versionNumber: _integer(json['versionNumber']),
    createdAtUtc: _utcDate(json['createdAtUtc']),
    confirmedAtUtc: _utcDate(json['confirmedAtUtc']),
  );
}

final class AdvanceFundingSummary {
  const AdvanceFundingSummary({
    required this.sourceType,
    required this.allocatedAmount,
    required this.fundingStatus,
    required this.paymentMethods,
    required this.allocatedAtUtc,
  });

  final String sourceType;
  final String allocatedAmount;
  final String fundingStatus;
  final List<String> paymentMethods;
  final DateTime? allocatedAtUtc;

  factory AdvanceFundingSummary.fromJson(Map<String, Object?> json) =>
      AdvanceFundingSummary(
        sourceType: _string(json['sourceType']),
        allocatedAmount: decimalText(json['allocatedAmount']),
        fundingStatus: _string(json['fundingStatus']),
        paymentMethods: _strings(json['paymentMethods']),
        allocatedAtUtc: _utcDate(json['allocatedAtUtc']),
      );
}

final class AdvanceBalanceSummary {
  const AdvanceBalanceSummary({
    required this.advanceId,
    required this.advanceNumber,
    required this.holder,
    required this.totalReceivedAmount,
    required this.totalRestoredAmount,
    required this.totalExpensedAmount,
    required this.totalTransferredOutAmount,
    required this.totalReturnedAmount,
    required this.availableAmount,
    required this.reservedAmount,
    required this.currencyCode,
    required this.status,
    required this.versionNumber,
    required this.updatedAtUtc,
  });

  final String advanceId;
  final String advanceNumber;
  final AdvanceUserSummary holder;
  final String totalReceivedAmount;
  final String totalRestoredAmount;
  final String totalExpensedAmount;
  final String totalTransferredOutAmount;
  final String totalReturnedAmount;
  final String availableAmount;
  final String reservedAmount;
  final String currencyCode;
  final String status;
  final int versionNumber;
  final DateTime? updatedAtUtc;

  factory AdvanceBalanceSummary.fromJson(Map<String, Object?> json) =>
      AdvanceBalanceSummary(
        advanceId: _string(json['advanceId']),
        advanceNumber: _string(json['advanceNumber']),
        holder: AdvanceUserSummary.fromJson(_map(json['holder'])),
        totalReceivedAmount: decimalText(json['totalReceivedAmount']),
        totalRestoredAmount: decimalText(json['totalRestoredAmount']),
        totalExpensedAmount: decimalText(json['totalExpensedAmount']),
        totalTransferredOutAmount: decimalText(
          json['totalTransferredOutAmount'],
        ),
        totalReturnedAmount: decimalText(json['totalReturnedAmount']),
        availableAmount: decimalText(json['availableAmount']),
        reservedAmount: decimalText(json['reservedAmount']),
        currencyCode: _string(json['currencyCode']),
        status: _string(json['status']),
        versionNumber: _integer(json['versionNumber']),
        updatedAtUtc: _utcDate(json['updatedAtUtc']),
      );
}

final class AdvanceDetails {
  const AdvanceDetails({
    required this.advance,
    required this.fundings,
    required this.balances,
    this.closureStatus,
  });

  final AdvanceSummary advance;
  final List<AdvanceFundingSummary> fundings;
  final List<AdvanceBalanceSummary> balances;
  final String? closureStatus;

  AdvanceBalanceSummary? balanceFor(String userId) {
    for (final balance in balances) {
      if (balance.holder.userId == userId) return balance;
    }
    return null;
  }

  factory AdvanceDetails.fromJson(Map<String, Object?> json) => AdvanceDetails(
    advance: AdvanceSummary.fromJson(_map(json['advance'])),
    fundings: _items(json['fundings'], AdvanceFundingSummary.fromJson),
    balances: _items(json['balances'], AdvanceBalanceSummary.fromJson),
    closureStatus: _optionalString(json['closureStatus']),
  );
}

final class AdvanceMovementPage {
  const AdvanceMovementPage({
    required this.items,
    required this.page,
    required this.pageSize,
    required this.totalCount,
    required this.totalPages,
  });

  final List<AdvanceMovement> items;
  final int page;
  final int pageSize;
  final int totalCount;
  final int totalPages;

  bool get hasMore => page < totalPages;

  factory AdvanceMovementPage.fromJson(Map<String, Object?> json) =>
      AdvanceMovementPage(
        items: _items(json['items'], AdvanceMovement.fromJson),
        page: _integer(json['page'], fallback: 1),
        pageSize: _integer(json['pageSize'], fallback: 20),
        totalCount: _integer(json['totalCount']),
        totalPages: _integer(json['totalPages']),
      );
}

final class AdvanceMovement {
  const AdvanceMovement({
    required this.movementId,
    required this.operationType,
    required this.amount,
    required this.actor,
    required this.status,
    required this.occurredAtUtc,
    this.sender,
    this.recipient,
  });

  final String movementId;
  final String operationType;
  final String amount;
  final AdvanceUserSummary actor;
  final AdvanceUserSummary? sender;
  final AdvanceUserSummary? recipient;
  final String status;
  final DateTime? occurredAtUtc;

  bool get isTransfer => const {
    'AdvanceDelivery',
    'InternalTransfer',
    'BalanceReturn',
  }.contains(operationType);

  bool get canBeRejected =>
      const {'InternalTransfer', 'BalanceReturn'}.contains(operationType);

  factory AdvanceMovement.fromJson(Map<String, Object?> json) =>
      AdvanceMovement(
        movementId: _string(json['movementId']),
        operationType: _string(json['operationType']),
        amount: decimalText(json['amount']),
        actor: AdvanceUserSummary.fromJson(_map(json['actor'])),
        sender: json['sender'] is Map
            ? AdvanceUserSummary.fromJson(_map(json['sender']))
            : null,
        recipient: json['recipient'] is Map
            ? AdvanceUserSummary.fromJson(_map(json['recipient']))
            : null,
        status: _string(json['status']),
        occurredAtUtc: _utcDate(json['occurredAtUtc']),
      );
}

final class AdvanceTransferDetails {
  const AdvanceTransferDetails({
    required this.transferId,
    required this.transferNumber,
    required this.transferType,
    required this.advanceId,
    required this.amount,
    required this.currencyCode,
    required this.sender,
    required this.recipient,
    required this.transferMethod,
    required this.status,
    required this.versionNumber,
    required this.createdAtUtc,
    this.confirmedAtUtc,
    this.rejectedAtUtc,
  });

  final String transferId;
  final String transferNumber;
  final String transferType;
  final String advanceId;
  final String amount;
  final String currencyCode;
  final AdvanceUserSummary sender;
  final AdvanceUserSummary recipient;
  final String transferMethod;
  final String status;
  final int versionNumber;
  final DateTime? createdAtUtc;
  final DateTime? confirmedAtUtc;
  final DateTime? rejectedAtUtc;

  factory AdvanceTransferDetails.fromJson(Map<String, Object?> json) =>
      AdvanceTransferDetails(
        transferId: _string(json['transferId']),
        transferNumber: _string(json['transferNumber']),
        transferType: _string(json['transferType']),
        advanceId: _string(json['advanceId']),
        amount: decimalText(json['amount']),
        currencyCode: _string(json['currencyCode']),
        sender: AdvanceUserSummary.fromJson(_map(json['sender'])),
        recipient: AdvanceUserSummary.fromJson(_map(json['recipient'])),
        transferMethod: _string(json['transferMethod']),
        status: _string(json['status']),
        versionNumber: _integer(json['versionNumber']),
        createdAtUtc: _utcDate(json['createdAtUtc']),
        confirmedAtUtc: _utcDate(json['confirmedAtUtc']),
        rejectedAtUtc: _utcDate(json['rejectedAtUtc']),
      );
}

final class AvailableFundingSource {
  const AvailableFundingSource({
    required this.fundingSourceId,
    required this.sourceType,
    required this.sourceDate,
    required this.currencyCode,
    required this.availableAmount,
    required this.status,
    required this.paymentMethods,
  });

  final String fundingSourceId;
  final String sourceType;
  final DateTime? sourceDate;
  final String currencyCode;
  final String availableAmount;
  final String status;
  final List<String> paymentMethods;

  factory AvailableFundingSource.fromJson(Map<String, Object?> json) =>
      AvailableFundingSource(
        fundingSourceId: _string(json['fundingSourceId']),
        sourceType: _string(json['sourceType']),
        sourceDate: _dateOnly(json['sourceDate']),
        currencyCode: _string(json['currencyCode']),
        availableAmount: decimalText(json['availableAmount']),
        status: _string(json['status']),
        paymentMethods: _strings(json['paymentMethods']),
      );
}

final class FundingSourcePage {
  const FundingSourcePage({
    required this.items,
    required this.page,
    required this.pageSize,
    required this.totalCount,
    required this.totalPages,
  });

  final List<AvailableFundingSource> items;
  final int page;
  final int pageSize;
  final int totalCount;
  final int totalPages;

  bool get hasMore => page < totalPages;

  factory FundingSourcePage.fromJson(Map<String, Object?> json) =>
      FundingSourcePage(
        items: _items(json['items'], AvailableFundingSource.fromJson),
        page: _integer(json['page'], fallback: 1),
        pageSize: _integer(json['pageSize'], fallback: 20),
        totalCount: _integer(json['totalCount']),
        totalPages: _integer(json['totalPages']),
      );
}

final class AdvanceBalancePage {
  const AdvanceBalancePage({required this.page, this.totalAvailableAmount});

  final AdvanceBalanceResultPage page;
  final String? totalAvailableAmount;

  factory AdvanceBalancePage.fromJson(Map<String, Object?> json) =>
      AdvanceBalancePage(
        page: AdvanceBalanceResultPage.fromJson(_map(json['page'])),
        totalAvailableAmount: json.containsKey('totalAvailableAmount')
            ? decimalText(json['totalAvailableAmount'])
            : null,
      );
}

final class AdvanceBalanceResultPage {
  const AdvanceBalanceResultPage({
    required this.items,
    required this.page,
    required this.pageSize,
    required this.totalCount,
    required this.totalPages,
  });

  final List<AdvanceBalanceSummary> items;
  final int page;
  final int pageSize;
  final int totalCount;
  final int totalPages;

  bool get hasMore => page < totalPages;

  factory AdvanceBalanceResultPage.fromJson(Map<String, Object?> json) =>
      AdvanceBalanceResultPage(
        items: _items(json['items'], AdvanceBalanceSummary.fromJson),
        page: _integer(json['page'], fallback: 1),
        pageSize: _integer(json['pageSize'], fallback: 20),
        totalCount: _integer(json['totalCount']),
        totalPages: _integer(json['totalPages']),
      );
}

String decimalText(Object? value) {
  final text = switch (value) {
    final String value => value.trim(),
    final int value => value.toString(),
    final num value => value.toString(),
    _ => '0.00',
  };
  final match = RegExp(r'^(-?)(\d+)(?:\.(\d+))?$').firstMatch(text);
  if (match == null) return text;
  final sign = match.group(1)!;
  final whole = match.group(2)!.replaceFirst(RegExp(r'^0+(?=\d)'), '');
  final fraction = (match.group(3) ?? '').padRight(2, '0');
  if (fraction.length > 2) return text;
  final cents = fraction;
  return '$sign$whole.$cents';
}

Map<String, Object?> _map(Object? value) =>
    value is Map ? Map<String, Object?>.from(value) : const {};

List<T> _items<T>(Object? value, T Function(Map<String, Object?>) parse) =>
    value is List
    ? value
          .whereType<Map>()
          .map((item) => parse(Map<String, Object?>.from(item)))
          .toList(growable: false)
    : const [];

List<String> _strings(Object? value) => value is List
    ? value.whereType<String>().toList(growable: false)
    : const [];

String _string(Object? value) => value is String ? value : '';

String? _optionalString(Object? value) =>
    value is String && value.trim().isNotEmpty ? value : null;

DateTime? _utcDate(Object? value) =>
    value is String ? DateTime.tryParse(value)?.toUtc() : null;

DateTime? _dateOnly(Object? value) {
  if (value is! String) return null;
  final parsed = DateTime.tryParse(value);
  return parsed == null
      ? null
      : DateTime(parsed.year, parsed.month, parsed.day);
}

int _integer(Object? value, {int fallback = 0}) => switch (value) {
  final int value => value,
  final num value => value.toInt(),
  final String value => int.tryParse(value) ?? fallback,
  _ => fallback,
};
