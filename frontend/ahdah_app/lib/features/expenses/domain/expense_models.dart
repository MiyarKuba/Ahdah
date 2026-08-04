import '../../advances/domain/advance_models.dart' show decimalText;

const expenseStatuses = <String>[
  'Draft',
  'PendingReview',
  'CorrectionRequired',
  'Approved',
  'Rejected',
  'Cancelled',
  'Reversed',
];

const expensePaymentModes = <String>['AdvanceBalance', 'PersonalFunds'];

const expenseCategoryGroups = <String>[
  'Materials',
  'Labor',
  'Subcontracting',
  'Transportation',
  'Equipment',
  'Fuel',
  'Services',
  'Administrative',
  'Utilities',
  'Permits',
  'Other',
];

const expenseCategoryScopes = <String>['ProjectOnly', 'CompanyOnly', 'Both'];

const expenseDocumentTypes = <String>[
  'Receipt',
  'Invoice',
  'Quotation',
  'DeliveryNote',
  'PaymentProof',
  'Contract',
  'PurchaseOrder',
  'Other',
];

const reimbursementStatuses = <String>[
  'Open',
  'PartiallySettled',
  'Settled',
  'Cancelled',
  'Reversed',
];

final class ExpenseUserSummary {
  const ExpenseUserSummary({
    required this.userId,
    required this.fullName,
    required this.role,
  });

  final String userId;
  final String fullName;
  final String role;

  factory ExpenseUserSummary.fromJson(Map<String, Object?> json) =>
      ExpenseUserSummary(
        userId: _string(json['userId']),
        fullName: _string(json['fullName']),
        role: _string(json['role']),
      );
}

final class ExpenseProjectSummary {
  const ExpenseProjectSummary({
    required this.projectId,
    required this.projectName,
  });

  final String projectId;
  final String projectName;

  factory ExpenseProjectSummary.fromJson(Map<String, Object?> json) =>
      ExpenseProjectSummary(
        projectId: _string(json['projectId']),
        projectName: _string(json['projectName']),
      );
}

final class ExpenseCategory {
  const ExpenseCategory({
    required this.expenseCategoryId,
    required this.categoryName,
    required this.categoryGroup,
    required this.expenseScope,
    required this.requiresSupplier,
    required this.requiresReceipt,
    required this.supportsQuantityDetails,
    required this.isActive,
    required this.displayOrder,
    required this.versionNumber,
    this.parentExpenseCategoryId,
    this.categoryCode,
    this.description,
  });

  final String expenseCategoryId;
  final String? parentExpenseCategoryId;
  final String? categoryCode;
  final String categoryName;
  final String categoryGroup;
  final String expenseScope;
  final String? description;
  final bool requiresSupplier;
  final bool requiresReceipt;
  final bool supportsQuantityDetails;
  final bool isActive;
  final int displayOrder;
  final int versionNumber;

  factory ExpenseCategory.fromJson(Map<String, Object?> json) =>
      ExpenseCategory(
        expenseCategoryId: _string(json['expenseCategoryId']),
        parentExpenseCategoryId: _optionalString(
          json['parentExpenseCategoryId'],
        ),
        categoryCode: _optionalString(json['categoryCode']),
        categoryName: _string(json['categoryName']),
        categoryGroup: _string(json['categoryGroup']),
        expenseScope: _string(json['expenseScope']),
        description: _optionalString(json['description']),
        requiresSupplier: json['requiresSupplier'] == true,
        requiresReceipt: json['requiresReceipt'] == true,
        supportsQuantityDetails: json['supportsQuantityDetails'] == true,
        isActive: json['isActive'] == true,
        displayOrder: _integer(json['displayOrder']),
        versionNumber: _integer(json['versionNumber']),
      );
}

final class ExpenseSummary {
  const ExpenseSummary({
    required this.expenseId,
    required this.expenseNumber,
    required this.totalAmount,
    required this.currencyCode,
    required this.paymentMode,
    required this.description,
    required this.status,
    required this.category,
    required this.incurredBy,
    required this.submittedBy,
    required this.hasReceiptDocument,
    required this.versionNumber,
    this.project,
    this.expenseDate,
    this.receiptNumber,
    this.invoiceNumber,
    this.reimbursementStatus,
    this.createdAtUtc,
    this.submittedAtUtc,
    this.reviewedAtUtc,
  });

  final String expenseId;
  final String expenseNumber;
  final DateTime? expenseDate;
  final String totalAmount;
  final String currencyCode;
  final String paymentMode;
  final String description;
  final String status;
  final ExpenseCategory category;
  final ExpenseProjectSummary? project;
  final ExpenseUserSummary incurredBy;
  final ExpenseUserSummary submittedBy;
  final String? receiptNumber;
  final String? invoiceNumber;
  final bool hasReceiptDocument;
  final String? reimbursementStatus;
  final int versionNumber;
  final DateTime? createdAtUtc;
  final DateTime? submittedAtUtc;
  final DateTime? reviewedAtUtc;

  factory ExpenseSummary.fromJson(Map<String, Object?> json) => ExpenseSummary(
    expenseId: _string(json['expenseId']),
    expenseNumber: _string(json['expenseNumber']),
    expenseDate: _dateOnly(json['expenseDate']),
    totalAmount: decimalText(json['totalAmount']),
    currencyCode: _string(json['currencyCode']),
    paymentMode: _string(json['paymentMode']),
    description: _string(json['description']),
    status: _string(json['status']),
    category: ExpenseCategory.fromJson(_map(json['category'])),
    project: json['project'] is Map
        ? ExpenseProjectSummary.fromJson(_map(json['project']))
        : null,
    incurredBy: ExpenseUserSummary.fromJson(_map(json['incurredBy'])),
    submittedBy: ExpenseUserSummary.fromJson(_map(json['submittedBy'])),
    receiptNumber: _optionalString(json['receiptNumber']),
    invoiceNumber: _optionalString(json['invoiceNumber']),
    hasReceiptDocument: json['hasReceiptDocument'] == true,
    reimbursementStatus: _optionalString(json['reimbursementStatus']),
    versionNumber: _integer(json['versionNumber']),
    createdAtUtc: _utcDate(json['createdAtUtc']),
    submittedAtUtc: _utcDate(json['submittedAtUtc']),
    reviewedAtUtc: _utcDate(json['reviewedAtUtc']),
  );
}

final class ExpenseAdvanceAllocation {
  const ExpenseAdvanceAllocation({
    required this.expenseAdvanceAllocationId,
    required this.advanceId,
    required this.advanceNumber,
    required this.allocatedAmount,
    this.createdAtUtc,
  });

  final String expenseAdvanceAllocationId;
  final String advanceId;
  final String advanceNumber;
  final String allocatedAmount;
  final DateTime? createdAtUtc;

  factory ExpenseAdvanceAllocation.fromJson(Map<String, Object?> json) =>
      ExpenseAdvanceAllocation(
        expenseAdvanceAllocationId: _string(json['expenseAdvanceAllocationId']),
        advanceId: _string(json['advanceId']),
        advanceNumber: _string(json['advanceNumber']),
        allocatedAmount: decimalText(json['allocatedAmount']),
        createdAtUtc: _utcDate(json['createdAtUtc']),
      );
}

final class ExpenseDocumentMetadata {
  const ExpenseDocumentMetadata({
    required this.expenseDocumentId,
    required this.documentType,
    required this.originalFileName,
    required this.mimeType,
    required this.fileSizeBytes,
    required this.captureSource,
    required this.isPrimary,
    required this.verificationStatus,
    required this.uploadedBy,
    required this.versionNumber,
    this.documentNumber,
    this.documentDate,
    this.issuerName,
    this.verifiedBy,
    this.createdAtUtc,
    this.verifiedAtUtc,
    this.rejectionReason,
    this.notes,
  });

  final String expenseDocumentId;
  final String documentType;
  final String? documentNumber;
  final DateTime? documentDate;
  final String? issuerName;
  final String originalFileName;
  final String mimeType;
  final int fileSizeBytes;
  final String captureSource;
  final bool isPrimary;
  final String verificationStatus;
  final ExpenseUserSummary uploadedBy;
  final ExpenseUserSummary? verifiedBy;
  final DateTime? createdAtUtc;
  final DateTime? verifiedAtUtc;
  final String? rejectionReason;
  final String? notes;
  final int versionNumber;

  factory ExpenseDocumentMetadata.fromJson(Map<String, Object?> json) =>
      ExpenseDocumentMetadata(
        expenseDocumentId: _string(json['expenseDocumentId']),
        documentType: _string(json['documentType']),
        documentNumber: _optionalString(json['documentNumber']),
        documentDate: _dateOnly(json['documentDate']),
        issuerName: _optionalString(json['issuerName']),
        originalFileName: _string(json['originalFileName']),
        mimeType: _string(json['mimeType']),
        fileSizeBytes: _integer(json['fileSizeBytes']),
        captureSource: _string(json['captureSource']),
        isPrimary: json['isPrimary'] == true,
        verificationStatus: _string(json['verificationStatus']),
        uploadedBy: ExpenseUserSummary.fromJson(_map(json['uploadedBy'])),
        verifiedBy: json['verifiedBy'] is Map
            ? ExpenseUserSummary.fromJson(_map(json['verifiedBy']))
            : null,
        createdAtUtc: _utcDate(json['createdAtUtc']),
        verifiedAtUtc: _utcDate(json['verifiedAtUtc']),
        rejectionReason: _optionalString(json['rejectionReason']),
        notes: _optionalString(json['notes']),
        versionNumber: _integer(json['versionNumber']),
      );
}

final class ExpenseItem {
  const ExpenseItem({
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

  factory ExpenseItem.fromJson(Map<String, Object?> json) => ExpenseItem(
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

final class ReimbursementSummary {
  const ReimbursementSummary({
    required this.reimbursementId,
    required this.reimbursementNumber,
    required this.expenseId,
    required this.expenseNumber,
    required this.claimant,
    required this.claimAmount,
    required this.outstandingAmount,
    required this.currencyCode,
    required this.description,
    required this.status,
    required this.versionNumber,
    this.project,
    this.claimDate,
    this.dueDate,
    this.createdAtUtc,
  });

  final String reimbursementId;
  final String reimbursementNumber;
  final String expenseId;
  final String expenseNumber;
  final ExpenseUserSummary claimant;
  final ExpenseProjectSummary? project;
  final DateTime? claimDate;
  final DateTime? dueDate;
  final String claimAmount;
  final String outstandingAmount;
  final String currencyCode;
  final String description;
  final String status;
  final int versionNumber;
  final DateTime? createdAtUtc;

  factory ReimbursementSummary.fromJson(Map<String, Object?> json) =>
      ReimbursementSummary(
        reimbursementId: _string(json['reimbursementId']),
        reimbursementNumber: _string(json['reimbursementNumber']),
        expenseId: _string(json['expenseId']),
        expenseNumber: _string(json['expenseNumber']),
        claimant: ExpenseUserSummary.fromJson(_map(json['claimant'])),
        project: json['project'] is Map
            ? ExpenseProjectSummary.fromJson(_map(json['project']))
            : null,
        claimDate: _dateOnly(json['claimDate']),
        dueDate: _dateOnly(json['dueDate']),
        claimAmount: decimalText(json['claimAmount']),
        outstandingAmount: decimalText(json['outstandingAmount']),
        currencyCode: _string(json['currencyCode']),
        description: _string(json['description']),
        status: _string(json['status']),
        versionNumber: _integer(json['versionNumber']),
        createdAtUtc: _utcDate(json['createdAtUtc']),
      );
}

final class ExpenseDetails {
  const ExpenseDetails({
    required this.expense,
    required this.subtotalAmount,
    required this.discountAmount,
    required this.taxAmount,
    required this.advanceAllocations,
    required this.documents,
    required this.items,
    this.merchantName,
    this.expenseLocation,
    this.notes,
    this.correctionReason,
    this.rejectionReason,
    this.reviewedBy,
    this.reimbursement,
  });

  final ExpenseSummary expense;
  final String subtotalAmount;
  final String discountAmount;
  final String taxAmount;
  final String? merchantName;
  final String? expenseLocation;
  final String? notes;
  final String? correctionReason;
  final String? rejectionReason;
  final ExpenseUserSummary? reviewedBy;
  final List<ExpenseAdvanceAllocation> advanceAllocations;
  final List<ExpenseDocumentMetadata> documents;
  final List<ExpenseItem> items;
  final ReimbursementSummary? reimbursement;

  factory ExpenseDetails.fromJson(Map<String, Object?> json) => ExpenseDetails(
    expense: ExpenseSummary.fromJson(_map(json['expense'])),
    subtotalAmount: decimalText(json['subtotalAmount']),
    discountAmount: decimalText(json['discountAmount']),
    taxAmount: decimalText(json['taxAmount']),
    merchantName: _optionalString(json['merchantName']),
    expenseLocation: _optionalString(json['expenseLocation']),
    notes: _optionalString(json['notes']),
    correctionReason: _optionalString(json['correctionReason']),
    rejectionReason: _optionalString(json['rejectionReason']),
    reviewedBy: json['reviewedBy'] is Map
        ? ExpenseUserSummary.fromJson(_map(json['reviewedBy']))
        : null,
    advanceAllocations: _items(
      json['advanceAllocations'],
      ExpenseAdvanceAllocation.fromJson,
    ),
    documents: _items(json['documents'], ExpenseDocumentMetadata.fromJson),
    items: _items(json['items'], ExpenseItem.fromJson),
    reimbursement: json['reimbursement'] is Map
        ? ReimbursementSummary.fromJson(_map(json['reimbursement']))
        : null,
  );
}

final class ExpenseHistoryEvent {
  const ExpenseHistoryEvent({
    required this.eventId,
    required this.eventName,
    required this.eventAction,
    required this.outcome,
    required this.description,
    this.actor,
    this.expenseVersionNumber,
    this.occurredAtUtc,
  });

  final String eventId;
  final String eventName;
  final String eventAction;
  final String outcome;
  final String description;
  final ExpenseUserSummary? actor;
  final int? expenseVersionNumber;
  final DateTime? occurredAtUtc;

  factory ExpenseHistoryEvent.fromJson(Map<String, Object?> json) =>
      ExpenseHistoryEvent(
        eventId: _string(json['eventId']),
        eventName: _string(json['eventName']),
        eventAction: _string(json['eventAction']),
        outcome: _string(json['outcome']),
        description: _string(json['description']),
        actor: json['actor'] is Map
            ? ExpenseUserSummary.fromJson(_map(json['actor']))
            : null,
        expenseVersionNumber: json['expenseVersionNumber'] == null
            ? null
            : _integer(json['expenseVersionNumber']),
        occurredAtUtc: _utcDate(json['occurredAtUtc']),
      );
}

final class ExpensePage extends _Page<ExpenseSummary> {
  const ExpensePage({
    required super.items,
    required super.page,
    required super.pageSize,
    required super.totalCount,
    required super.totalPages,
  });

  factory ExpensePage.fromJson(Map<String, Object?> json) => ExpensePage(
    items: _items(json['items'], ExpenseSummary.fromJson),
    page: _integer(json['page'], fallback: 1),
    pageSize: _integer(json['pageSize'], fallback: 20),
    totalCount: _integer(json['totalCount']),
    totalPages: _integer(json['totalPages']),
  );
}

final class ExpenseCategoryPage extends _Page<ExpenseCategory> {
  const ExpenseCategoryPage({
    required super.items,
    required super.page,
    required super.pageSize,
    required super.totalCount,
    required super.totalPages,
  });

  factory ExpenseCategoryPage.fromJson(Map<String, Object?> json) =>
      ExpenseCategoryPage(
        items: _items(json['items'], ExpenseCategory.fromJson),
        page: _integer(json['page'], fallback: 1),
        pageSize: _integer(json['pageSize'], fallback: 20),
        totalCount: _integer(json['totalCount']),
        totalPages: _integer(json['totalPages']),
      );
}

final class ExpenseAllocationPage extends _Page<ExpenseAdvanceAllocation> {
  const ExpenseAllocationPage({
    required super.items,
    required super.page,
    required super.pageSize,
    required super.totalCount,
    required super.totalPages,
  });

  factory ExpenseAllocationPage.fromJson(Map<String, Object?> json) =>
      ExpenseAllocationPage(
        items: _items(json['items'], ExpenseAdvanceAllocation.fromJson),
        page: _integer(json['page'], fallback: 1),
        pageSize: _integer(json['pageSize'], fallback: 20),
        totalCount: _integer(json['totalCount']),
        totalPages: _integer(json['totalPages']),
      );
}

final class ExpenseDocumentPage extends _Page<ExpenseDocumentMetadata> {
  const ExpenseDocumentPage({
    required super.items,
    required super.page,
    required super.pageSize,
    required super.totalCount,
    required super.totalPages,
  });

  factory ExpenseDocumentPage.fromJson(Map<String, Object?> json) =>
      ExpenseDocumentPage(
        items: _items(json['items'], ExpenseDocumentMetadata.fromJson),
        page: _integer(json['page'], fallback: 1),
        pageSize: _integer(json['pageSize'], fallback: 20),
        totalCount: _integer(json['totalCount']),
        totalPages: _integer(json['totalPages']),
      );
}

final class ExpenseHistoryPage extends _Page<ExpenseHistoryEvent> {
  const ExpenseHistoryPage({
    required super.items,
    required super.page,
    required super.pageSize,
    required super.totalCount,
    required super.totalPages,
  });

  factory ExpenseHistoryPage.fromJson(Map<String, Object?> json) =>
      ExpenseHistoryPage(
        items: _items(json['items'], ExpenseHistoryEvent.fromJson),
        page: _integer(json['page'], fallback: 1),
        pageSize: _integer(json['pageSize'], fallback: 20),
        totalCount: _integer(json['totalCount']),
        totalPages: _integer(json['totalPages']),
      );
}

final class ReimbursementPage extends _Page<ReimbursementSummary> {
  const ReimbursementPage({
    required super.items,
    required super.page,
    required super.pageSize,
    required super.totalCount,
    required super.totalPages,
  });

  factory ReimbursementPage.fromJson(Map<String, Object?> json) =>
      ReimbursementPage(
        items: _items(json['items'], ReimbursementSummary.fromJson),
        page: _integer(json['page'], fallback: 1),
        pageSize: _integer(json['pageSize'], fallback: 20),
        totalCount: _integer(json['totalCount']),
        totalPages: _integer(json['totalPages']),
      );
}

abstract base class _Page<T> {
  const _Page({
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
