import '../../advances/domain/advance_models.dart' show decimalText;

const supplierTypes = <String>[
  'GeneralSupplier',
  'MaterialsSupplier',
  'EquipmentSupplier',
  'EquipmentRental',
  'FuelSupplier',
  'Subcontractor',
  'TransportProvider',
  'MaintenanceProvider',
  'ServiceProvider',
  'Other',
];
const supplierTransactionModes = <String>[
  'CashOnly',
  'CreditOnly',
  'CashAndCredit',
];
const supplierPaymentMethods = <String>[
  'Cash',
  'BankTransfer',
  'Cheque',
  'Card',
  'MobileWallet',
  'Other',
];
const supplierAccountTypes = <String>[
  'BankAccount',
  'MobileWallet',
  'CashCollection',
  'Other',
];
const supplierDebtStatuses = <String>[
  'Open',
  'PartiallySettled',
  'Settled',
  'Cancelled',
  'Reversed',
];
const supplierPaymentStatuses = <String>[
  'Draft',
  'PendingApproval',
  'Confirmed',
  'Rejected',
  'Cancelled',
  'Reversed',
];
const supplierCreditStatuses = <String>[
  'Draft',
  'PendingApproval',
  'Approved',
  'Rejected',
  'Cancelled',
  'Reversed',
];
const supplierCreditReasons = <String>[
  'ReturnedGoods',
  'DamagedGoods',
  'PricingCorrection',
  'Overbilling',
  'AdditionalDiscount',
  'ServiceCompensation',
  'Other',
];
const supplierUnitCodes = <String>[
  'Piece',
  'Package',
  'Box',
  'Bag',
  'Kilogram',
  'Ton',
  'Meter',
  'SquareMeter',
  'CubicMeter',
  'Liter',
  'Hour',
  'Day',
  'Trip',
  'Service',
  'LumpSum',
  'Other',
];

final class SupplierSummary {
  const SupplierSummary({
    required this.supplierId,
    required this.supplierName,
    required this.supplierType,
    required this.defaultCurrencyCode,
    required this.transactionMode,
    required this.defaultPaymentTermsDays,
    required this.isActive,
    required this.versionNumber,
    this.supplierCode,
    this.contactPersonName,
    this.phoneNumber,
    this.email,
    this.city,
    this.creditLimit,
    this.preferredPaymentMethod,
    this.createdAtUtc,
    this.updatedAtUtc,
  });

  final String supplierId;
  final String? supplierCode;
  final String supplierName;
  final String supplierType;
  final String? contactPersonName;
  final String? phoneNumber;
  final String? email;
  final String? city;
  final String defaultCurrencyCode;
  final String transactionMode;
  final int defaultPaymentTermsDays;
  final String? creditLimit;
  final String? preferredPaymentMethod;
  final bool isActive;
  final int versionNumber;
  final DateTime? createdAtUtc;
  final DateTime? updatedAtUtc;

  factory SupplierSummary.fromJson(Map<String, Object?> json) =>
      SupplierSummary(
        supplierId: _string(json['supplierId']),
        supplierCode: _optionalString(json['supplierCode']),
        supplierName: _string(json['supplierName']),
        supplierType: _string(json['supplierType']),
        contactPersonName: _optionalString(json['contactPersonName']),
        phoneNumber: _optionalString(json['phoneNumber']),
        email: _optionalString(json['email']),
        city: _optionalString(json['city']),
        defaultCurrencyCode: _string(json['defaultCurrencyCode']),
        transactionMode: _string(json['transactionMode']),
        defaultPaymentTermsDays: _integer(json['defaultPaymentTermsDays']),
        creditLimit: _optionalDecimal(json['creditLimit']),
        preferredPaymentMethod: _optionalString(json['preferredPaymentMethod']),
        isActive: json['isActive'] == true,
        versionNumber: _integer(json['versionNumber']),
        createdAtUtc: _utcDate(json['createdAtUtc']),
        updatedAtUtc: _utcDate(json['updatedAtUtc']),
      );
}

final class SupplierBalanceSummary {
  const SupplierBalanceSummary({
    required this.currencyCode,
    required this.outstandingAmount,
    required this.openDebtCount,
  });
  final String currencyCode;
  final String outstandingAmount;
  final int openDebtCount;
  factory SupplierBalanceSummary.fromJson(Map<String, Object?> json) =>
      SupplierBalanceSummary(
        currencyCode: _string(json['currencyCode']),
        outstandingAmount: decimalText(json['outstandingAmount']),
        openDebtCount: _integer(json['openDebtCount']),
      );
}

final class SupplierDetails {
  const SupplierDetails({
    required this.supplier,
    required this.balances,
    this.secondaryPhoneNumber,
    this.commercialRegistrationNumber,
    this.taxRegistrationNumber,
    this.address,
    this.notes,
  });
  final SupplierSummary supplier;
  final String? secondaryPhoneNumber;
  final String? commercialRegistrationNumber;
  final String? taxRegistrationNumber;
  final String? address;
  final String? notes;
  final List<SupplierBalanceSummary> balances;
  factory SupplierDetails.fromJson(Map<String, Object?> json) =>
      SupplierDetails(
        supplier: SupplierSummary.fromJson(_map(json['supplier'])),
        secondaryPhoneNumber: _optionalString(json['secondaryPhoneNumber']),
        commercialRegistrationNumber: _optionalString(
          json['commercialRegistrationNumber'],
        ),
        taxRegistrationNumber: _optionalString(json['taxRegistrationNumber']),
        address: _optionalString(json['address']),
        notes: _optionalString(json['notes']),
        balances: _items(json['balances'], SupplierBalanceSummary.fromJson),
      );
}

final class SupplierPaymentAccount {
  const SupplierPaymentAccount({
    required this.supplierPaymentAccountId,
    required this.accountType,
    required this.accountLabel,
    required this.currencyCode,
    required this.isDefault,
    required this.verificationStatus,
    required this.isActive,
    required this.versionNumber,
    this.accountHolderName,
    this.bankName,
    this.bankBranchName,
    this.maskedAccountNumber,
    this.maskedIban,
    this.walletProvider,
    this.maskedWalletNumber,
    this.notes,
    this.createdAtUtc,
  });
  final String supplierPaymentAccountId;
  final String accountType;
  final String accountLabel;
  final String? accountHolderName;
  final String? bankName;
  final String? bankBranchName;
  final String? maskedAccountNumber;
  final String? maskedIban;
  final String? walletProvider;
  final String? maskedWalletNumber;
  final String currencyCode;
  final bool isDefault;
  final String verificationStatus;
  final bool isActive;
  final String? notes;
  final int versionNumber;
  final DateTime? createdAtUtc;
  factory SupplierPaymentAccount.fromJson(Map<String, Object?> json) =>
      SupplierPaymentAccount(
        supplierPaymentAccountId: _string(json['supplierPaymentAccountId']),
        accountType: _string(json['accountType']),
        accountLabel: _string(json['accountLabel']),
        accountHolderName: _optionalString(json['accountHolderName']),
        bankName: _optionalString(json['bankName']),
        bankBranchName: _optionalString(json['bankBranchName']),
        maskedAccountNumber: _optionalString(json['maskedAccountNumber']),
        maskedIban: _optionalString(json['maskedIban']),
        walletProvider: _optionalString(json['walletProvider']),
        maskedWalletNumber: _optionalString(json['maskedWalletNumber']),
        currencyCode: _string(json['currencyCode']),
        isDefault: json['isDefault'] == true,
        verificationStatus: _string(json['verificationStatus']),
        isActive: json['isActive'] == true,
        notes: _optionalString(json['notes']),
        versionNumber: _integer(json['versionNumber']),
        createdAtUtc: _utcDate(json['createdAtUtc']),
      );
}

final class SupplierProjectSummary {
  const SupplierProjectSummary({
    required this.projectId,
    required this.projectName,
  });
  final String projectId;
  final String projectName;
  factory SupplierProjectSummary.fromJson(Map<String, Object?> json) =>
      SupplierProjectSummary(
        projectId: _string(json['projectId']),
        projectName: _string(json['projectName']),
      );
}

final class SupplierInvoiceItem {
  const SupplierInvoiceItem({
    required this.expenseItemId,
    required this.lineNumber,
    required this.itemName,
    required this.quantity,
    required this.unitCode,
    required this.unitPrice,
    required this.subtotalAmount,
    required this.discountAmount,
    required this.taxAmount,
    required this.totalAmount,
    this.itemCode,
    this.itemDescription,
    this.customUnitName,
    this.notes,
  });
  final String expenseItemId;
  final int lineNumber;
  final String itemName;
  final String? itemCode;
  final String? itemDescription;
  final String quantity;
  final String unitCode;
  final String? customUnitName;
  final String unitPrice;
  final String subtotalAmount;
  final String discountAmount;
  final String taxAmount;
  final String totalAmount;
  final String? notes;
  factory SupplierInvoiceItem.fromJson(Map<String, Object?> json) =>
      SupplierInvoiceItem(
        expenseItemId: _string(json['expenseItemId']),
        lineNumber: _integer(json['lineNumber']),
        itemName: _string(json['itemName']),
        itemCode: _optionalString(json['itemCode']),
        itemDescription: _optionalString(json['itemDescription']),
        quantity: decimalText(json['quantity']),
        unitCode: _string(json['unitCode']),
        customUnitName: _optionalString(json['customUnitName']),
        unitPrice: decimalText(json['unitPrice']),
        subtotalAmount: decimalText(json['subtotalAmount']),
        discountAmount: decimalText(json['discountAmount']),
        taxAmount: decimalText(json['taxAmount']),
        totalAmount: decimalText(json['totalAmount']),
        notes: _optionalString(json['notes']),
      );
}

final class SupplierInvoiceSummary {
  const SupplierInvoiceSummary({
    required this.supplierDebtId,
    required this.debtNumber,
    required this.expenseId,
    required this.expenseNumber,
    required this.supplier,
    required this.amount,
    required this.paidAmount,
    required this.creditNoteAmount,
    required this.writtenOffAmount,
    required this.outstandingAmount,
    required this.currencyCode,
    required this.expenseStatus,
    required this.debtStatus,
    required this.description,
    required this.expenseVersionNumber,
    required this.debtVersionNumber,
    this.invoiceNumber,
    this.project,
    this.invoiceDate,
    this.dueDate,
    this.createdAtUtc,
  });
  final String supplierDebtId;
  final String debtNumber;
  final String expenseId;
  final String expenseNumber;
  final String? invoiceNumber;
  final SupplierSummary supplier;
  final SupplierProjectSummary? project;
  final DateTime? invoiceDate;
  final DateTime? dueDate;
  final String amount;
  final String paidAmount;
  final String creditNoteAmount;
  final String writtenOffAmount;
  final String outstandingAmount;
  final String currencyCode;
  final String expenseStatus;
  final String debtStatus;
  final String description;
  final int expenseVersionNumber;
  final int debtVersionNumber;
  final DateTime? createdAtUtc;
  factory SupplierInvoiceSummary.fromJson(Map<String, Object?> json) =>
      SupplierInvoiceSummary(
        supplierDebtId: _string(json['supplierDebtId']),
        debtNumber: _string(json['debtNumber']),
        expenseId: _string(json['expenseId']),
        expenseNumber: _string(json['expenseNumber']),
        invoiceNumber: _optionalString(json['invoiceNumber']),
        supplier: SupplierSummary.fromJson(_map(json['supplier'])),
        project: json['project'] is Map
            ? SupplierProjectSummary.fromJson(_map(json['project']))
            : null,
        invoiceDate: _dateOnly(json['invoiceDate']),
        dueDate: _dateOnly(json['dueDate']),
        amount: decimalText(json['amount']),
        paidAmount: decimalText(json['paidAmount']),
        creditNoteAmount: decimalText(json['creditNoteAmount']),
        writtenOffAmount: decimalText(json['writtenOffAmount']),
        outstandingAmount: decimalText(json['outstandingAmount']),
        currencyCode: _string(json['currencyCode']),
        expenseStatus: _string(json['expenseStatus']),
        debtStatus: _string(json['debtStatus']),
        description: _string(json['description']),
        expenseVersionNumber: _integer(json['expenseVersionNumber']),
        debtVersionNumber: _integer(json['debtVersionNumber']),
        createdAtUtc: _utcDate(json['createdAtUtc']),
      );
}

final class SupplierInvoiceDetails {
  const SupplierInvoiceDetails({
    required this.invoice,
    required this.adjustmentAmount,
    required this.items,
    this.notes,
  });
  final SupplierInvoiceSummary invoice;
  final String adjustmentAmount;
  final String? notes;
  final List<SupplierInvoiceItem> items;
  factory SupplierInvoiceDetails.fromJson(Map<String, Object?> json) =>
      SupplierInvoiceDetails(
        invoice: SupplierInvoiceSummary.fromJson(_map(json['invoice'])),
        adjustmentAmount: decimalText(json['adjustmentAmount']),
        notes: _optionalString(json['notes']),
        items: _items(json['items'], SupplierInvoiceItem.fromJson),
      );
}

final class SupplierUserSummary {
  const SupplierUserSummary({
    required this.userId,
    required this.fullName,
    required this.role,
  });
  final String userId;
  final String fullName;
  final String role;
  factory SupplierUserSummary.fromJson(Map<String, Object?> json) =>
      SupplierUserSummary(
        userId: _string(json['userId']),
        fullName: _string(json['fullName']),
        role: _string(json['role']),
      );
}

final class SupplierPaymentSummary {
  const SupplierPaymentSummary({
    required this.supplierPaymentId,
    required this.paymentNumber,
    required this.supplierId,
    required this.supplierName,
    required this.paymentAmount,
    required this.currencyCode,
    required this.paymentMethod,
    required this.hasProof,
    required this.status,
    required this.createdBy,
    required this.versionNumber,
    this.paymentDate,
    this.referenceNumber,
    this.confirmedBy,
    this.createdAtUtc,
    this.confirmedAtUtc,
  });
  final String supplierPaymentId;
  final String paymentNumber;
  final String supplierId;
  final String supplierName;
  final DateTime? paymentDate;
  final String paymentAmount;
  final String currencyCode;
  final String paymentMethod;
  final String? referenceNumber;
  final bool hasProof;
  final String status;
  final SupplierUserSummary createdBy;
  final SupplierUserSummary? confirmedBy;
  final int versionNumber;
  final DateTime? createdAtUtc;
  final DateTime? confirmedAtUtc;
  factory SupplierPaymentSummary.fromJson(Map<String, Object?> json) =>
      SupplierPaymentSummary(
        supplierPaymentId: _string(json['supplierPaymentId']),
        paymentNumber: _string(json['paymentNumber']),
        supplierId: _string(json['supplierId']),
        supplierName: _string(json['supplierName']),
        paymentDate: _dateOnly(json['paymentDate']),
        paymentAmount: decimalText(json['paymentAmount']),
        currencyCode: _string(json['currencyCode']),
        paymentMethod: _string(json['paymentMethod']),
        referenceNumber: _optionalString(json['referenceNumber']),
        hasProof: json['hasProof'] == true,
        status: _string(json['status']),
        createdBy: SupplierUserSummary.fromJson(_map(json['createdBy'])),
        confirmedBy: json['confirmedBy'] is Map
            ? SupplierUserSummary.fromJson(_map(json['confirmedBy']))
            : null,
        versionNumber: _integer(json['versionNumber']),
        createdAtUtc: _utcDate(json['createdAtUtc']),
        confirmedAtUtc: _utcDate(json['confirmedAtUtc']),
      );
}

final class SupplierPaymentDebtAllocation {
  const SupplierPaymentDebtAllocation({
    required this.supplierPaymentDebtAllocationId,
    required this.supplierDebtId,
    required this.debtNumber,
    required this.allocatedAmount,
    this.createdAtUtc,
  });
  final String supplierPaymentDebtAllocationId;
  final String supplierDebtId;
  final String debtNumber;
  final String allocatedAmount;
  final DateTime? createdAtUtc;
  factory SupplierPaymentDebtAllocation.fromJson(Map<String, Object?> json) =>
      SupplierPaymentDebtAllocation(
        supplierPaymentDebtAllocationId: _string(
          json['supplierPaymentDebtAllocationId'],
        ),
        supplierDebtId: _string(json['supplierDebtId']),
        debtNumber: _string(json['debtNumber']),
        allocatedAmount: decimalText(json['allocatedAmount']),
        createdAtUtc: _utcDate(json['createdAtUtc']),
      );
}

final class SupplierPaymentFundingAllocation {
  const SupplierPaymentFundingAllocation({
    required this.supplierPaymentFundingSourceId,
    required this.fundingSourceId,
    required this.sourceType,
    required this.allocatedAmount,
    this.createdAtUtc,
  });
  final String supplierPaymentFundingSourceId;
  final String fundingSourceId;
  final String sourceType;
  final String allocatedAmount;
  final DateTime? createdAtUtc;
  factory SupplierPaymentFundingAllocation.fromJson(
    Map<String, Object?> json,
  ) => SupplierPaymentFundingAllocation(
    supplierPaymentFundingSourceId: _string(
      json['supplierPaymentFundingSourceId'],
    ),
    fundingSourceId: _string(json['fundingSourceId']),
    sourceType: _string(json['sourceType']),
    allocatedAmount: decimalText(json['allocatedAmount']),
    createdAtUtc: _utcDate(json['createdAtUtc']),
  );
}

final class SupplierPaymentDetails {
  const SupplierPaymentDetails({
    required this.payment,
    required this.debtAllocations,
    required this.fundingSources,
    this.supplierPaymentAccountId,
    this.payerBankName,
    this.description,
    this.notes,
  });
  final SupplierPaymentSummary payment;
  final String? supplierPaymentAccountId;
  final String? payerBankName;
  final String? description;
  final String? notes;
  final List<SupplierPaymentDebtAllocation> debtAllocations;
  final List<SupplierPaymentFundingAllocation> fundingSources;
  factory SupplierPaymentDetails.fromJson(Map<String, Object?> json) =>
      SupplierPaymentDetails(
        payment: SupplierPaymentSummary.fromJson(_map(json['payment'])),
        supplierPaymentAccountId: _optionalString(
          json['supplierPaymentAccountId'],
        ),
        payerBankName: _optionalString(json['payerBankName']),
        description: _optionalString(json['description']),
        notes: _optionalString(json['notes']),
        debtAllocations: _items(
          json['debtAllocations'],
          SupplierPaymentDebtAllocation.fromJson,
        ),
        fundingSources: _items(
          json['fundingSources'],
          SupplierPaymentFundingAllocation.fromJson,
        ),
      );
}

final class SupplierFundingSource {
  const SupplierFundingSource({
    required this.fundingSourceId,
    required this.sourceType,
    required this.currencyCode,
    required this.availableAmount,
    required this.status,
    required this.paymentMethods,
    this.sourceDate,
  });
  final String fundingSourceId;
  final String sourceType;
  final DateTime? sourceDate;
  final String currencyCode;
  final String availableAmount;
  final String status;
  final List<String> paymentMethods;
  factory SupplierFundingSource.fromJson(Map<String, Object?> json) =>
      SupplierFundingSource(
        fundingSourceId: _string(json['fundingSourceId']),
        sourceType: _string(json['sourceType']),
        sourceDate: _dateOnly(json['sourceDate']),
        currencyCode: _string(json['currencyCode']),
        availableAmount: decimalText(json['availableAmount']),
        status: _string(json['status']),
        paymentMethods: _strings(json['paymentMethods']),
      );
}

final class SupplierCreditAllocation {
  const SupplierCreditAllocation({
    required this.supplierCreditNoteAllocationId,
    required this.supplierDebtId,
    required this.debtNumber,
    required this.allocatedAmount,
    this.createdAtUtc,
  });
  final String supplierCreditNoteAllocationId;
  final String supplierDebtId;
  final String debtNumber;
  final String allocatedAmount;
  final DateTime? createdAtUtc;
  factory SupplierCreditAllocation.fromJson(Map<String, Object?> json) =>
      SupplierCreditAllocation(
        supplierCreditNoteAllocationId: _string(
          json['supplierCreditNoteAllocationId'],
        ),
        supplierDebtId: _string(json['supplierDebtId']),
        debtNumber: _string(json['debtNumber']),
        allocatedAmount: decimalText(json['allocatedAmount']),
        createdAtUtc: _utcDate(json['createdAtUtc']),
      );
}

final class SupplierCreditNoteSummary {
  const SupplierCreditNoteSummary({
    required this.supplierCreditNoteId,
    required this.creditNoteNumber,
    required this.supplierId,
    required this.supplierName,
    required this.creditNoteAmount,
    required this.appliedAmount,
    required this.availableAmount,
    required this.currencyCode,
    required this.reasonType,
    required this.description,
    required this.status,
    required this.versionNumber,
    this.supplierReferenceNumber,
    this.creditNoteDate,
    this.createdAtUtc,
  });
  final String supplierCreditNoteId;
  final String creditNoteNumber;
  final String supplierId;
  final String supplierName;
  final String? supplierReferenceNumber;
  final DateTime? creditNoteDate;
  final String creditNoteAmount;
  final String appliedAmount;
  final String availableAmount;
  final String currencyCode;
  final String reasonType;
  final String description;
  final String status;
  final int versionNumber;
  final DateTime? createdAtUtc;
  factory SupplierCreditNoteSummary.fromJson(Map<String, Object?> json) =>
      SupplierCreditNoteSummary(
        supplierCreditNoteId: _string(json['supplierCreditNoteId']),
        creditNoteNumber: _string(json['creditNoteNumber']),
        supplierId: _string(json['supplierId']),
        supplierName: _string(json['supplierName']),
        supplierReferenceNumber: _optionalString(
          json['supplierReferenceNumber'],
        ),
        creditNoteDate: _dateOnly(json['creditNoteDate']),
        creditNoteAmount: decimalText(json['creditNoteAmount']),
        appliedAmount: decimalText(json['appliedAmount']),
        availableAmount: decimalText(json['availableAmount']),
        currencyCode: _string(json['currencyCode']),
        reasonType: _string(json['reasonType']),
        description: _string(json['description']),
        status: _string(json['status']),
        versionNumber: _integer(json['versionNumber']),
        createdAtUtc: _utcDate(json['createdAtUtc']),
      );
}

final class SupplierCreditNoteDetails {
  const SupplierCreditNoteDetails({
    required this.creditNote,
    required this.allocations,
    this.notes,
  });
  final SupplierCreditNoteSummary creditNote;
  final String? notes;
  final List<SupplierCreditAllocation> allocations;
  factory SupplierCreditNoteDetails.fromJson(
    Map<String, Object?> json,
  ) => SupplierCreditNoteDetails(
    creditNote: SupplierCreditNoteSummary.fromJson(_map(json['creditNote'])),
    notes: _optionalString(json['notes']),
    allocations: _items(json['allocations'], SupplierCreditAllocation.fromJson),
  );
}

final class SupplierRefundSummary {
  const SupplierRefundSummary({
    required this.supplierRefundId,
    required this.refundNumber,
    required this.supplierId,
    required this.supplierName,
    required this.expenseReturnId,
    required this.refundAmount,
    required this.feeAmount,
    required this.netReceivedAmount,
    required this.currencyCode,
    required this.refundMethod,
    required this.hasProof,
    required this.description,
    required this.status,
    required this.versionNumber,
    this.refundDate,
    this.supplierReferenceNumber,
    this.transactionReferenceNumber,
    this.createdAtUtc,
  });
  final String supplierRefundId;
  final String refundNumber;
  final String supplierId;
  final String supplierName;
  final String expenseReturnId;
  final DateTime? refundDate;
  final String refundAmount;
  final String feeAmount;
  final String netReceivedAmount;
  final String currencyCode;
  final String refundMethod;
  final String? supplierReferenceNumber;
  final String? transactionReferenceNumber;
  final bool hasProof;
  final String description;
  final String status;
  final int versionNumber;
  final DateTime? createdAtUtc;
  factory SupplierRefundSummary.fromJson(Map<String, Object?> json) =>
      SupplierRefundSummary(
        supplierRefundId: _string(json['supplierRefundId']),
        refundNumber: _string(json['refundNumber']),
        supplierId: _string(json['supplierId']),
        supplierName: _string(json['supplierName']),
        expenseReturnId: _string(json['expenseReturnId']),
        refundDate: _dateOnly(json['refundDate']),
        refundAmount: decimalText(json['refundAmount']),
        feeAmount: decimalText(json['feeAmount']),
        netReceivedAmount: decimalText(json['netReceivedAmount']),
        currencyCode: _string(json['currencyCode']),
        refundMethod: _string(json['refundMethod']),
        supplierReferenceNumber: _optionalString(
          json['supplierReferenceNumber'],
        ),
        transactionReferenceNumber: _optionalString(
          json['transactionReferenceNumber'],
        ),
        hasProof: json['hasProof'] == true,
        description: _string(json['description']),
        status: _string(json['status']),
        versionNumber: _integer(json['versionNumber']),
        createdAtUtc: _utcDate(json['createdAtUtc']),
      );
}

final class SupplierStatementEntry {
  const SupplierStatementEntry({
    required this.eventId,
    required this.eventType,
    required this.reference,
    required this.amount,
    required this.currencyCode,
    required this.status,
    required this.description,
    this.eventDate,
    this.occurredAtUtc,
  });
  final String eventId;
  final String eventType;
  final String reference;
  final DateTime? eventDate;
  final String amount;
  final String currencyCode;
  final String status;
  final String description;
  final DateTime? occurredAtUtc;
  factory SupplierStatementEntry.fromJson(Map<String, Object?> json) =>
      SupplierStatementEntry(
        eventId: _string(json['eventId']),
        eventType: _string(json['eventType']),
        reference: _string(json['reference']),
        eventDate: _dateOnly(json['eventDate']),
        amount: decimalText(json['amount']),
        currencyCode: _string(json['currencyCode']),
        status: _string(json['status']),
        description: _string(json['description']),
        occurredAtUtc: _utcDate(json['occurredAtUtc']),
      );
}

final class SupplierStatement {
  const SupplierStatement({
    required this.supplier,
    required this.balances,
    required this.entries,
  });
  final SupplierSummary supplier;
  final List<SupplierBalanceSummary> balances;
  final SupplierPage<SupplierStatementEntry> entries;
  factory SupplierStatement.fromJson(Map<String, Object?> json) =>
      SupplierStatement(
        supplier: SupplierSummary.fromJson(_map(json['supplier'])),
        balances: _items(json['balances'], SupplierBalanceSummary.fromJson),
        entries: SupplierPage.fromJson(
          _map(json['entries']),
          SupplierStatementEntry.fromJson,
        ),
      );
}

final class SupplierPage<T> {
  const SupplierPage({
    required this.items,
    required this.page,
    required this.pageSize,
    required this.totalCount,
    required this.totalPages,
  });
  final List<T> items;
  final int page;
  final int pageSize;
  final int totalCount;
  final int totalPages;
  bool get hasMore => page < totalPages;
  factory SupplierPage.fromJson(
    Map<String, Object?> json,
    T Function(Map<String, Object?>) parse,
  ) => SupplierPage(
    items: _items(json['items'], parse),
    page: _integer(json['page'], fallback: 1),
    pageSize: _integer(json['pageSize'], fallback: 20),
    totalCount: _integer(json['totalCount']),
    totalPages: _integer(json['totalPages']),
  );
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
String? _optionalDecimal(Object? value) =>
    value == null ? null : decimalText(value);
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
