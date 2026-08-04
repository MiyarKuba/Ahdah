import 'dart:convert';

final class FundingAllocationInput {
  const FundingAllocationInput({
    required this.fundingSourceId,
    required this.amount,
    this.notes,
  });

  final String fundingSourceId;
  final String amount;
  final String? notes;
}

final class AdvanceTransferFields {
  const AdvanceTransferFields({
    required this.amount,
    required this.transferDate,
    required this.transferMethod,
    this.bankName,
    this.referenceNumber,
    this.description,
    this.notes,
  });

  final String amount;
  final DateTime transferDate;
  final String transferMethod;
  final String? bankName;
  final String? referenceNumber;
  final String? description;
  final String? notes;

  Map<String, Object?> jsonWithAmount(Object amountToken) => {
    'amount': amountToken,
    'transferDate': _date(transferDate),
    'transferMethod': transferMethod,
    'bankName': _optional(bankName),
    'referenceNumber': _optional(referenceNumber),
    'description': _optional(description),
    'notes': _optional(notes),
  };
}

final class CreateAdvanceInput {
  const CreateAdvanceInput({
    required this.recipientUserId,
    required this.issueDate,
    required this.purpose,
    required this.fundings,
    required this.transfer,
    this.settlementDueDate,
  });

  final String recipientUserId;
  final DateTime issueDate;
  final DateTime? settlementDueDate;
  final String purpose;
  final List<FundingAllocationInput> fundings;
  final AdvanceTransferFields transfer;

  String get payloadFingerprint => toJsonBody();

  String toJsonBody() {
    const amountMarker = '__AHDAH_ADVANCE_AMOUNT__';
    final fundingMarkers = List.generate(
      fundings.length,
      (index) =>
          '__AHDAH_FUNDING_AMOUNT_$index'
          '__',
    );
    final json = transfer.jsonWithAmount(amountMarker)
      ..addAll({
        'recipientUserId': recipientUserId,
        'issueDate': _date(issueDate),
        'settlementDueDate': settlementDueDate == null
            ? null
            : _date(settlementDueDate!),
        'purpose': purpose.trim(),
        'fundings': [
          for (var index = 0; index < fundings.length; index++)
            {
              'fundingSourceId': fundings[index].fundingSourceId,
              'amount': fundingMarkers[index],
              'notes': _optional(fundings[index].notes),
            },
        ],
      });
    var encoded = jsonEncode(
      json,
    ).replaceFirst('"$amountMarker"', transfer.amount);
    for (var index = 0; index < fundings.length; index++) {
      encoded = encoded.replaceFirst(
        '"${fundingMarkers[index]}"',
        fundings[index].amount,
      );
    }
    return encoded;
  }
}

final class CreateAdvanceDistributionInput {
  const CreateAdvanceDistributionInput({
    required this.recipientUserId,
    required this.transfer,
  });

  final String recipientUserId;
  final AdvanceTransferFields transfer;
  String get payloadFingerprint => toJsonBody();

  String toJsonBody() => _encodeTransfer(
    transfer.jsonWithAmount('__AHDAH_DISTRIBUTION_AMOUNT__')
      ..['recipientUserId'] = recipientUserId,
    '__AHDAH_DISTRIBUTION_AMOUNT__',
    transfer.amount,
  );
}

final class CreateAdvanceReturnInput {
  const CreateAdvanceReturnInput({required this.transfer});

  final AdvanceTransferFields transfer;
  String get payloadFingerprint => toJsonBody();

  String toJsonBody() => _encodeTransfer(
    transfer.jsonWithAmount('__AHDAH_RETURN_AMOUNT__'),
    '__AHDAH_RETURN_AMOUNT__',
    transfer.amount,
  );
}

final class RejectAdvanceTransferInput {
  const RejectAdvanceTransferInput({required this.reason});

  final String reason;
  String get payloadFingerprint => jsonEncode(toJson());
  Map<String, Object?> toJson() => {'reason': reason.trim()};
}

String _encodeTransfer(
  Map<String, Object?> json,
  String marker,
  String amount,
) => jsonEncode(json).replaceFirst('"$marker"', amount);

String _date(DateTime value) =>
    '${value.year.toString().padLeft(4, '0')}-'
    '${value.month.toString().padLeft(2, '0')}-'
    '${value.day.toString().padLeft(2, '0')}';

String? _optional(String? value) {
  final normalized = value?.trim();
  return normalized == null || normalized.isEmpty ? null : normalized;
}
