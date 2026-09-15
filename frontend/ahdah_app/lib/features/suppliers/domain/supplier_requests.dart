import 'dart:convert';

import '../../advances/domain/decimal_money.dart';

abstract final class DecimalQuantity {
  static final _pattern = RegExp(r'^(?:0|[1-9][0-9]{0,14})(?:\.[0-9]{1,3})?$');
  static final maximumUnits = BigInt.parse('999999999999999999');

  static String? canonicalize(String? input) {
    if (input == null) return null;
    var text = input.trim();
    const arabicDigits = '٠١٢٣٤٥٦٧٨٩';
    const easternDigits = '۰۱۲۳۴۵۶۷۸۹';
    for (var index = 0; index < 10; index++) {
      text = text
          .replaceAll(arabicDigits[index], '$index')
          .replaceAll(easternDigits[index], '$index');
    }
    text = text.replaceAll('٫', '.');
    if (!_pattern.hasMatch(text)) return null;
    final parts = text.split('.');
    final whole = parts.first.replaceFirst(RegExp(r'^0+(?=\d)'), '');
    final fraction = parts.length == 1 ? '000' : parts[1].padRight(3, '0');
    final normalized = '$whole.$fraction';
    final units = toThousandths(normalized);
    if (units == null || units == BigInt.zero || units > maximumUnits) {
      return null;
    }
    return normalized;
  }

  static BigInt? toThousandths(String input) {
    final match = RegExp(r'^(\d{1,15})\.(\d{3})$').firstMatch(input);
    if (match == null) return null;
    return BigInt.tryParse('${match.group(1)}${match.group(2)}');
  }
}

final class SupplierFilters {
  const SupplierFilters({this.isActive, this.supplierType, this.search});
  final bool? isActive;
  final String? supplierType;
  final String? search;
}

final class SupplierInvoiceFilters {
  const SupplierInvoiceFilters({
    this.supplierId,
    this.projectId,
    this.status,
    this.currencyCode,
    this.dueFrom,
    this.dueTo,
    this.unpaidOnly,
    this.overdueOnly,
    this.reference,
  });
  final String? supplierId;
  final String? projectId;
  final String? status;
  final String? currencyCode;
  final DateTime? dueFrom;
  final DateTime? dueTo;
  final bool? unpaidOnly;
  final bool? overdueOnly;
  final String? reference;
}

final class SupplierPaymentFilters {
  const SupplierPaymentFilters({
    this.supplierId,
    this.supplierDebtId,
    this.fundingSourceId,
    this.status,
    this.paymentMethod,
    this.currencyCode,
    this.dateFrom,
    this.dateTo,
  });
  final String? supplierId;
  final String? supplierDebtId;
  final String? fundingSourceId;
  final String? status;
  final String? paymentMethod;
  final String? currencyCode;
  final DateTime? dateFrom;
  final DateTime? dateTo;
}

final class SupplierCreateInput {
  const SupplierCreateInput({
    required this.supplierName,
    required this.supplierType,
    required this.defaultCurrencyCode,
    required this.transactionMode,
    this.supplierCode,
    this.contactPersonName,
    this.phoneNumber,
    this.secondaryPhoneNumber,
    this.email,
    this.commercialRegistrationNumber,
    this.taxRegistrationNumber,
    this.address,
    this.city,
    this.defaultPaymentTermsDays = 0,
    this.creditLimit,
    this.preferredPaymentMethod,
    this.notes,
  });
  final String? supplierCode;
  final String supplierName;
  final String supplierType;
  final String? contactPersonName;
  final String? phoneNumber;
  final String? secondaryPhoneNumber;
  final String? email;
  final String? commercialRegistrationNumber;
  final String? taxRegistrationNumber;
  final String? address;
  final String? city;
  final String defaultCurrencyCode;
  final String transactionMode;
  final int defaultPaymentTermsDays;
  final String? creditLimit;
  final String? preferredPaymentMethod;
  final String? notes;

  String toJsonBody() => _numericJson(
    {
      'supplierCode': _optional(supplierCode)?.toUpperCase(),
      'supplierName': supplierName.trim(),
      'supplierType': supplierType,
      'contactPersonName': _optional(contactPersonName),
      'phoneNumber': _optional(phoneNumber),
      'secondaryPhoneNumber': _optional(secondaryPhoneNumber),
      'email': _optional(email)?.toLowerCase(),
      'commercialRegistrationNumber': _optional(commercialRegistrationNumber),
      'taxRegistrationNumber': _optional(taxRegistrationNumber),
      'address': _optional(address),
      'city': _optional(city),
      'defaultCurrencyCode': defaultCurrencyCode.trim().toUpperCase(),
      'transactionMode': transactionMode,
      'defaultPaymentTermsDays': defaultPaymentTermsDays,
      'creditLimit': creditLimit,
      'preferredPaymentMethod': _optional(preferredPaymentMethod),
      'notes': _optional(notes),
    },
    const {'creditLimit'},
  );
}

final class SupplierUpdateInput {
  const SupplierUpdateInput({
    required this.expectedVersion,
    this.contactPersonName,
    this.phoneNumber,
    this.secondaryPhoneNumber,
    this.email,
    this.address,
    this.city,
    this.notes,
    this.isActive,
    this.deactivationReason,
  });
  final int expectedVersion;
  final String? contactPersonName;
  final String? phoneNumber;
  final String? secondaryPhoneNumber;
  final String? email;
  final String? address;
  final String? city;
  final String? notes;
  final bool? isActive;
  final String? deactivationReason;
  Map<String, Object?> toJson() => {
    'expectedVersion': expectedVersion,
    'contactPersonName': _optional(contactPersonName),
    'phoneNumber': _optional(phoneNumber),
    'secondaryPhoneNumber': _optional(secondaryPhoneNumber),
    'email': _optional(email)?.toLowerCase(),
    'address': _optional(address),
    'city': _optional(city),
    'notes': _optional(notes),
    'isActive': isActive,
    'deactivationReason': _optional(deactivationReason),
  };
}

final class SupplierPaymentAccountInput {
  const SupplierPaymentAccountInput({
    required this.accountType,
    required this.accountLabel,
    required this.currencyCode,
    this.accountHolderName,
    this.bankName,
    this.bankBranchName,
    this.accountNumber,
    this.iban,
    this.walletProvider,
    this.walletNumber,
    this.notes,
  });
  final String accountType;
  final String accountLabel;
  final String? accountHolderName;
  final String? bankName;
  final String? bankBranchName;
  final String? accountNumber;
  final String? iban;
  final String? walletProvider;
  final String? walletNumber;
  final String currencyCode;
  final String? notes;
  String get payloadFingerprint => jsonEncode(toJson());
  Map<String, Object?> toJson() => {
    'accountType': accountType,
    'accountLabel': accountLabel.trim(),
    'accountHolderName': _optional(accountHolderName),
    'bankName': _optional(bankName),
    'bankBranchName': _optional(bankBranchName),
    'accountNumber': _optional(accountNumber),
    'iban': _optional(iban),
    'walletProvider': _optional(walletProvider),
    'walletNumber': _optional(walletNumber),
    'currencyCode': currencyCode.trim().toUpperCase(),
    'notes': _optional(notes),
  };
}

final class SupplierInvoiceItemInput {
  const SupplierInvoiceItemInput({
    required this.itemName,
    required this.quantity,
    required this.unitCode,
    required this.unitPrice,
    required this.discountAmount,
    required this.taxAmount,
    this.itemCode,
    this.itemDescription,
    this.customUnitName,
    this.notes,
  });
  final String itemName;
  final String? itemCode;
  final String? itemDescription;
  final String quantity;
  final String unitCode;
  final String? customUnitName;
  final String unitPrice;
  final String discountAmount;
  final String taxAmount;
  final String? notes;

  BigInt? get totalMinorUnits {
    final quantityUnits = DecimalQuantity.toThousandths(quantity);
    final price = DecimalMoney.toMinorUnits(unitPrice);
    final discount = DecimalMoney.toMinorUnits(discountAmount);
    final tax = DecimalMoney.toMinorUnits(taxAmount);
    if (quantityUnits == null ||
        price == null ||
        discount == null ||
        tax == null) {
      return null;
    }
    final product = quantityUnits * price;
    var subtotal = product ~/ BigInt.from(1000);
    final remainder = product.remainder(BigInt.from(1000));
    if (remainder > BigInt.from(500) ||
        remainder == BigInt.from(500) && subtotal.isOdd) {
      subtotal += BigInt.one;
    }
    final result = subtotal - discount + tax;
    return result > BigInt.zero ? result : null;
  }
}

final class SupplierInvoiceCreateInput {
  const SupplierInvoiceCreateInput({
    required this.supplierId,
    required this.expenseCategoryId,
    required this.invoiceDate,
    required this.dueDate,
    required this.amount,
    required this.currencyCode,
    required this.description,
    this.projectId,
    this.invoiceNumber,
    this.notes,
    this.items = const [],
  });
  final String supplierId;
  final String expenseCategoryId;
  final String? projectId;
  final DateTime invoiceDate;
  final DateTime dueDate;
  final String amount;
  final String currencyCode;
  final String? invoiceNumber;
  final String description;
  final String? notes;
  final List<SupplierInvoiceItemInput> items;

  bool get itemsMatchAmount {
    if (items.isEmpty) return true;
    final expected = DecimalMoney.toMinorUnits(amount);
    if (expected == null) return false;
    var total = BigInt.zero;
    for (final item in items) {
      final value = item.totalMinorUnits;
      if (value == null) return false;
      total += value;
    }
    return total == expected;
  }

  String get payloadFingerprint => toJsonBody();
  String toJsonBody() {
    final map = <String, Object?>{
      'supplierId': supplierId,
      'expenseCategoryId': expenseCategoryId,
      'projectId': _optional(projectId),
      'invoiceDate': _date(invoiceDate),
      'dueDate': _date(dueDate),
      'amount': amount,
      'currencyCode': currencyCode.trim().toUpperCase(),
      'invoiceNumber': _optional(invoiceNumber),
      'description': description.trim(),
      'notes': _optional(notes),
      'items': [
        for (final item in items)
          {
            'itemName': item.itemName.trim(),
            'itemCode': _optional(item.itemCode),
            'itemDescription': _optional(item.itemDescription),
            'quantity': item.quantity,
            'unitCode': item.unitCode,
            'customUnitName': _optional(item.customUnitName),
            'unitPrice': item.unitPrice,
            'discountAmount': item.discountAmount,
            'taxAmount': item.taxAmount,
            'notes': _optional(item.notes),
          },
      ],
    };
    return _numericJsonDeep(map, const {
      'amount',
      'quantity',
      'unitPrice',
      'discountAmount',
      'taxAmount',
    });
  }
}

final class SupplierPaymentDebtAllocationInput {
  const SupplierPaymentDebtAllocationInput({
    required this.supplierDebtId,
    required this.amount,
    this.notes,
  });
  final String supplierDebtId;
  final String amount;
  final String? notes;
}

final class SupplierPaymentFundingAllocationInput {
  const SupplierPaymentFundingAllocationInput({
    required this.fundingSourceId,
    required this.amount,
    this.notes,
  });
  final String fundingSourceId;
  final String amount;
  final String? notes;
}

final class SupplierPaymentCreateInput {
  const SupplierPaymentCreateInput({
    required this.supplierId,
    required this.paymentDate,
    required this.paymentAmount,
    required this.currencyCode,
    required this.paymentMethod,
    required this.debtAllocations,
    required this.fundingAllocations,
    this.supplierPaymentAccountId,
    this.payerBankName,
    this.referenceNumber,
    this.proofFileUrl,
    this.description,
    this.notes,
  });
  final String supplierId;
  final String? supplierPaymentAccountId;
  final DateTime paymentDate;
  final String paymentAmount;
  final String currencyCode;
  final String paymentMethod;
  final String? payerBankName;
  final String? referenceNumber;
  final String? proofFileUrl;
  final String? description;
  final String? notes;
  final List<SupplierPaymentDebtAllocationInput> debtAllocations;
  final List<SupplierPaymentFundingAllocationInput> fundingAllocations;

  bool get allocationsMatch =>
      _unique(debtAllocations.map((value) => value.supplierDebtId)) &&
      _unique(fundingAllocations.map((value) => value.fundingSourceId)) &&
      DecimalMoney.sumEquals(
        debtAllocations.map((value) => value.amount),
        paymentAmount,
      ) &&
      DecimalMoney.sumEquals(
        fundingAllocations.map((value) => value.amount),
        paymentAmount,
      );

  String get payloadFingerprint => toJsonBody();
  String toJsonBody() => _numericJsonDeep(
    {
      'supplierId': supplierId,
      'supplierPaymentAccountId': _optional(supplierPaymentAccountId),
      'paymentDate': _date(paymentDate),
      'paymentAmount': paymentAmount,
      'currencyCode': currencyCode.trim().toUpperCase(),
      'paymentMethod': paymentMethod,
      'payerBankName': _optional(payerBankName),
      'referenceNumber': _optional(referenceNumber),
      'proofFileUrl': _optional(proofFileUrl),
      'description': _optional(description),
      'notes': _optional(notes),
      'debtAllocations': [
        for (final item in debtAllocations)
          {
            'supplierDebtId': item.supplierDebtId,
            'amount': item.amount,
            'notes': _optional(item.notes),
          },
      ],
      'fundingAllocations': [
        for (final item in fundingAllocations)
          {
            'fundingSourceId': item.fundingSourceId,
            'amount': item.amount,
            'notes': _optional(item.notes),
          },
      ],
    },
    const {'paymentAmount', 'amount'},
  );
}

final class SupplierPaymentReviewInput {
  const SupplierPaymentReviewInput({
    required this.expectedVersion,
    this.rejectionReason,
  });
  final int expectedVersion;
  final String? rejectionReason;
  String get payloadFingerprint => jsonEncode(toJson());
  Map<String, Object?> toJson() => {
    'expectedVersion': expectedVersion,
    'rejectionReason': _optional(rejectionReason),
  };
}

final class SupplierCreditCreateInput {
  const SupplierCreditCreateInput({
    required this.supplierId,
    required this.creditNoteDate,
    required this.amount,
    required this.currencyCode,
    required this.reasonType,
    required this.description,
    this.supplierReferenceNumber,
    this.notes,
  });
  final String supplierId;
  final DateTime creditNoteDate;
  final String amount;
  final String currencyCode;
  final String? supplierReferenceNumber;
  final String reasonType;
  final String description;
  final String? notes;
  String get payloadFingerprint => toJsonBody();
  String toJsonBody() => _numericJson(
    {
      'supplierId': supplierId,
      'creditNoteDate': _date(creditNoteDate),
      'amount': amount,
      'currencyCode': currencyCode.trim().toUpperCase(),
      'supplierReferenceNumber': _optional(supplierReferenceNumber),
      'reasonType': reasonType,
      'description': description.trim(),
      'notes': _optional(notes),
    },
    const {'amount'},
  );
}

final class SupplierCreditApproveInput {
  const SupplierCreditApproveInput({required this.expectedVersion});
  final int expectedVersion;
  String get payloadFingerprint => jsonEncode(toJson());
  Map<String, Object?> toJson() => {'expectedVersion': expectedVersion};
}

final class SupplierCreditAllocationInput {
  const SupplierCreditAllocationInput({
    required this.supplierDebtId,
    required this.amount,
    this.notes,
  });
  final String supplierDebtId;
  final String amount;
  final String? notes;
}

final class SupplierCreditApplyInput {
  const SupplierCreditApplyInput({
    required this.expectedVersion,
    required this.allocations,
  });
  final int expectedVersion;
  final List<SupplierCreditAllocationInput> allocations;
  bool totalWithin(String available) {
    final maximum = DecimalMoney.toMinorUnits(available);
    if (maximum == null ||
        !_unique(allocations.map((value) => value.supplierDebtId))) {
      return false;
    }
    var total = BigInt.zero;
    for (final item in allocations) {
      final value = DecimalMoney.toMinorUnits(item.amount);
      if (value == null || value <= BigInt.zero) return false;
      total += value;
    }
    return total <= maximum;
  }

  String get payloadFingerprint => toJsonBody();
  String toJsonBody() => _numericJsonDeep(
    {
      'expectedVersion': expectedVersion,
      'allocations': [
        for (final item in allocations)
          {
            'supplierDebtId': item.supplierDebtId,
            'amount': item.amount,
            'notes': _optional(item.notes),
          },
      ],
    },
    const {'amount'},
  );
}

bool _unique(Iterable<String> values) {
  final list = values.toList(growable: false);
  return list.isNotEmpty && list.toSet().length == list.length;
}

String _numericJson(Map<String, Object?> map, Set<String> numericKeys) =>
    _numericJsonDeep(map, numericKeys);

String _numericJsonDeep(Map<String, Object?> map, Set<String> numericKeys) {
  var index = 0;
  final replacements = <String, String>{};
  Object? replace(Object? value, String? key) {
    if (key != null && numericKeys.contains(key) && value is String) {
      final marker = '__AHDAH_SUPPLIER_NUMBER_${index++}__';
      replacements[marker] = value;
      return marker;
    }
    if (value is Map<String, Object?>) {
      return value.map(
        (entryKey, entryValue) =>
            MapEntry(entryKey, replace(entryValue, entryKey)),
      );
    }
    if (value is List) return value.map((item) => replace(item, null)).toList();
    return value;
  }

  var encoded = jsonEncode(replace(map, null));
  for (final entry in replacements.entries) {
    encoded = encoded.replaceFirst('"${entry.key}"', entry.value);
  }
  return encoded;
}

String _date(DateTime value) =>
    '${value.year.toString().padLeft(4, '0')}-'
    '${value.month.toString().padLeft(2, '0')}-'
    '${value.day.toString().padLeft(2, '0')}';
String? _optional(String? value) {
  final normalized = value?.trim();
  return normalized == null || normalized.isEmpty ? null : normalized;
}
