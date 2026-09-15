import '../../../../core/localization/value_labels.dart';
import '../../../../l10n/app_localizations.dart';

abstract final class SupplierLabels {
  static String type(AppLocalizations l10n, String value) => switch (value) {
    'GeneralSupplier' => l10n.supplierTypeGeneral,
    'MaterialsSupplier' => l10n.supplierTypeMaterials,
    'EquipmentSupplier' => l10n.supplierTypeEquipment,
    'EquipmentRental' => l10n.supplierTypeEquipmentRental,
    'FuelSupplier' => l10n.supplierTypeFuel,
    'Subcontractor' => l10n.supplierTypeSubcontractor,
    'TransportProvider' => l10n.supplierTypeTransport,
    'MaintenanceProvider' => l10n.supplierTypeMaintenance,
    'ServiceProvider' => l10n.supplierTypeService,
    'Other' => l10n.other,
    _ => l10n.unknownValue,
  };
  static String transactionMode(AppLocalizations l10n, String value) =>
      switch (value) {
        'CashOnly' => l10n.transactionCashOnly,
        'CreditOnly' => l10n.transactionCreditOnly,
        'CashAndCredit' => l10n.transactionCashAndCredit,
        _ => l10n.unknownValue,
      };
  static String accountType(AppLocalizations l10n, String value) =>
      switch (value) {
        'BankAccount' => l10n.accountTypeBank,
        'MobileWallet' => l10n.accountTypeWallet,
        'CashCollection' => l10n.accountTypeCashCollection,
        'Other' => l10n.other,
        _ => l10n.unknownValue,
      };
  static String status(AppLocalizations l10n, String value) => switch (value) {
    'Draft' => l10n.advanceStatusDraft,
    'PendingReview' => l10n.expenseStatusPendingReview,
    'CorrectionRequired' => l10n.expenseStatusCorrectionRequired,
    'PendingApproval' || 'PendingVerification' => l10n.statusPendingApproval,
    'Approved' || 'Confirmed' || 'Verified' => l10n.confirmed,
    'Open' => l10n.advanceStatusOpen,
    'PartiallySettled' => l10n.claimStatusPartiallySettled,
    'Settled' => l10n.claimStatusSettled,
    'Rejected' => l10n.rejected,
    'Cancelled' => l10n.advanceStatusCancelled,
    'Reversed' => l10n.advanceStatusReversed,
    'Available' ||
    'PartiallyUsed' ||
    'FullyUsed' => ValueLabels.fundingStatus(l10n, value),
    _ => l10n.unknownValue,
  };
  static String reason(AppLocalizations l10n, String value) => switch (value) {
    'ReturnedGoods' => l10n.creditReasonReturnedGoods,
    'DamagedGoods' => l10n.creditReasonDamagedGoods,
    'PricingCorrection' => l10n.creditReasonPricingCorrection,
    'Overbilling' => l10n.creditReasonOverbilling,
    'AdditionalDiscount' => l10n.creditReasonAdditionalDiscount,
    'ServiceCompensation' => l10n.creditReasonServiceCompensation,
    'Other' => l10n.other,
    _ => l10n.unknownValue,
  };
  static String unit(AppLocalizations l10n, String value) => switch (value) {
    'Piece' => l10n.unitPiece,
    'Package' => l10n.unitPackage,
    'Box' => l10n.unitBox,
    'Bag' => l10n.unitBag,
    'Kilogram' => l10n.unitKilogram,
    'Ton' => l10n.unitTon,
    'Meter' => l10n.unitMeter,
    'SquareMeter' => l10n.unitSquareMeter,
    'CubicMeter' => l10n.unitCubicMeter,
    'Liter' => l10n.unitLiter,
    'Hour' => l10n.unitHour,
    'Day' => l10n.unitDay,
    'Trip' => l10n.unitTrip,
    'Service' => l10n.unitService,
    'LumpSum' => l10n.unitLumpSum,
    'Other' => l10n.other,
    _ => l10n.unknownValue,
  };
}
