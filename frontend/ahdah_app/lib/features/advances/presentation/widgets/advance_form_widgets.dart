import 'package:flutter/material.dart';

import '../../../../l10n/app_localizations.dart';
import '../../domain/advance_list_state.dart';
import '../../domain/decimal_money.dart';

final class AdvanceTransferFormFields extends StatelessWidget {
  const AdvanceTransferFormFields({
    required this.amountController,
    required this.transferDate,
    required this.transferMethod,
    required this.onSelectDate,
    required this.onMethodChanged,
    required this.bankNameController,
    required this.referenceController,
    required this.descriptionController,
    required this.notesController,
    required this.currencyCode,
    this.availableAmount,
    super.key,
  });

  final TextEditingController amountController;
  final DateTime transferDate;
  final String transferMethod;
  final VoidCallback onSelectDate;
  final ValueChanged<String> onMethodChanged;
  final TextEditingController bankNameController;
  final TextEditingController referenceController;
  final TextEditingController descriptionController;
  final TextEditingController notesController;
  final String currencyCode;
  final String? availableAmount;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final needsBank =
        transferMethod == 'BankTransfer' || transferMethod == 'Cheque';
    final needsReference =
        needsBank ||
        transferMethod == 'Card' ||
        transferMethod == 'MobileWallet';
    final needsDescription = transferMethod == 'Other';
    return Column(
      children: [
        TextFormField(
          key: const Key('advance-amount-field'),
          controller: amountController,
          keyboardType: const TextInputType.numberWithOptions(decimal: true),
          textDirection: TextDirection.ltr,
          decoration: InputDecoration(
            labelText: '${l10n.amount} ($currencyCode)',
          ),
          validator: (value) {
            final canonical = DecimalMoney.canonicalize(value);
            if (canonical == null) return l10n.invalidMoney;
            if (availableAmount != null &&
                !DecimalMoney.lessThanOrEqual(canonical, availableAmount!)) {
              return l10n.insufficientAvailableBalance;
            }
            return null;
          },
        ),
        const SizedBox(height: 12),
        ListTile(
          contentPadding: EdgeInsets.zero,
          leading: const Icon(Icons.event_outlined),
          title: Text(l10n.transferDate),
          subtitle: Text(
            MaterialLocalizations.of(context).formatMediumDate(transferDate),
          ),
          trailing: TextButton(
            onPressed: onSelectDate,
            child: Text(l10n.selectDate),
          ),
        ),
        const SizedBox(height: 12),
        DropdownButtonFormField<String>(
          key: const Key('transfer-method-field'),
          initialValue: transferMethod,
          decoration: InputDecoration(labelText: l10n.paymentMethod),
          items: [
            for (final method in knownTransferMethods.where(
              (item) => item != 'BalanceTransfer',
            ))
              DropdownMenuItem(
                value: method,
                child: Text(_methodLabel(l10n, method)),
              ),
          ],
          onChanged: (value) {
            if (value != null) onMethodChanged(value);
          },
        ),
        if (needsBank) ...[
          const SizedBox(height: 12),
          TextFormField(
            controller: bankNameController,
            maxLength: 150,
            decoration: InputDecoration(labelText: l10n.bankName),
            validator: (value) =>
                (value?.trim().isEmpty ?? true) ? l10n.requiredField : null,
          ),
        ],
        if (needsReference) ...[
          const SizedBox(height: 12),
          TextFormField(
            controller: referenceController,
            maxLength: 150,
            decoration: InputDecoration(labelText: l10n.referenceNumber),
            validator: (value) =>
                (value?.trim().isEmpty ?? true) ? l10n.requiredField : null,
          ),
        ],
        if (needsDescription) ...[
          const SizedBox(height: 12),
          TextFormField(
            controller: descriptionController,
            maxLength: 1000,
            decoration: InputDecoration(labelText: l10n.description),
            validator: (value) =>
                (value?.trim().isEmpty ?? true) ? l10n.requiredField : null,
          ),
        ],
        const SizedBox(height: 12),
        TextFormField(
          controller: notesController,
          maxLength: 1000,
          maxLines: 3,
          decoration: InputDecoration(labelText: l10n.notesOptional),
          validator: (value) {
            final normalized = value?.trim() ?? '';
            return normalized.contains('\n') || normalized.contains('\r')
                ? l10n.invalidMessage
                : null;
          },
        ),
      ],
    );
  }
}

String _methodLabel(AppLocalizations l10n, String method) => switch (method) {
  'Cash' => l10n.paymentCash,
  'BankTransfer' => l10n.paymentBankTransfer,
  'Cheque' => l10n.paymentCheque,
  'Card' => l10n.paymentCard,
  'MobileWallet' => l10n.paymentMobileWallet,
  'Other' => l10n.other,
  _ => l10n.unknownValue,
};
