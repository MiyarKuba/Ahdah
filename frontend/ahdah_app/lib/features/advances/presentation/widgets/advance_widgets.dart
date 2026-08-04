import 'package:flutter/material.dart';

import '../../../../core/localization/app_date_time_formatter.dart';
import '../../../../core/localization/value_labels.dart';
import '../../../../l10n/app_localizations.dart';
import '../../domain/advance_models.dart';
import '../controllers/advance_controllers.dart';

final class MoneyText extends StatelessWidget {
  const MoneyText({
    required this.amount,
    required this.currency,
    this.style,
    super.key,
  });

  final String amount;
  final String currency;
  final TextStyle? style;

  @override
  Widget build(BuildContext context) => Semantics(
    label: '$amount $currency',
    child: Directionality(
      textDirection: TextDirection.ltr,
      child: Text('$amount $currency', style: style),
    ),
  );
}

final class AdvanceStatusChip extends StatelessWidget {
  const AdvanceStatusChip({
    required this.status,
    this.transfer = false,
    super.key,
  });

  final String status;
  final bool transfer;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;
    final label = transfer
        ? ValueLabels.transferStatus(l10n, status)
        : ValueLabels.advanceStatus(l10n, status);
    final color = switch (status) {
      'Open' || 'Confirmed' || 'Recorded' => scheme.primaryContainer,
      'PendingConfirmation' ||
      'Draft' ||
      'CorrectionRequired' => scheme.tertiaryContainer,
      'Rejected' || 'Cancelled' || 'Reversed' => scheme.errorContainer,
      _ => scheme.surfaceContainerHighest,
    };
    return Chip(
      avatar: Icon(_icon, size: 17),
      label: Text(label),
      backgroundColor: color,
      side: BorderSide.none,
    );
  }

  IconData get _icon => switch (status) {
    'Open' || 'Confirmed' || 'Recorded' => Icons.verified_outlined,
    'PendingConfirmation' || 'Draft' || 'CorrectionRequired' => Icons.schedule,
    'Rejected' || 'Cancelled' || 'Reversed' => Icons.block,
    _ => Icons.info_outline,
  };
}

final class AdvanceSummaryCard extends StatelessWidget {
  const AdvanceSummaryCard({
    required this.advance,
    required this.onOpen,
    super.key,
  });

  final AdvanceSummary advance;
  final VoidCallback onOpen;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Card(
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: onOpen,
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Expanded(
                    child: Text(
                      advance.advanceNumber,
                      style: Theme.of(context).textTheme.titleMedium,
                    ),
                  ),
                  AdvanceStatusChip(status: advance.status),
                ],
              ),
              MoneyText(
                amount: advance.advanceAmount,
                currency: advance.currencyCode,
                style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                  fontWeight: FontWeight.w700,
                ),
              ),
              const SizedBox(height: 8),
              Wrap(
                spacing: 16,
                runSpacing: 8,
                children: [
                  _LabelValue(
                    label: l10n.availableAmount,
                    child: MoneyText(
                      amount: advance.availableAmount,
                      currency: advance.currencyCode,
                    ),
                  ),
                  _LabelValue(
                    label: l10n.reservedAmount,
                    child: MoneyText(
                      amount: advance.reservedAmount,
                      currency: advance.currencyCode,
                    ),
                  ),
                  _LabelValue(
                    label: l10n.recipient,
                    value: advance.recipient.fullName,
                  ),
                  _LabelValue(
                    label: l10n.issueDate,
                    value: AppDateTimeFormatter.formatDate(
                      context,
                      advance.issueDate,
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}

final class AdvanceBalanceCard extends StatelessWidget {
  const AdvanceBalanceCard({required this.balance, this.onOpen, super.key});

  final AdvanceBalanceSummary balance;
  final VoidCallback? onOpen;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final child = Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Expanded(
                child: Text(
                  balance.advanceNumber,
                  style: Theme.of(context).textTheme.titleMedium,
                ),
              ),
              Chip(
                label: Text(ValueLabels.balanceStatus(l10n, balance.status)),
              ),
            ],
          ),
          Text(balance.holder.fullName),
          const SizedBox(height: 8),
          MoneyText(
            amount: balance.availableAmount,
            currency: balance.currencyCode,
            style: Theme.of(
              context,
            ).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700),
          ),
          const SizedBox(height: 8),
          Wrap(
            spacing: 16,
            runSpacing: 8,
            children: [
              _LabelValue(
                label: l10n.reservedAmount,
                child: MoneyText(
                  amount: balance.reservedAmount,
                  currency: balance.currencyCode,
                ),
              ),
              _LabelValue(
                label: l10n.receivedAmount,
                child: MoneyText(
                  amount: balance.totalReceivedAmount,
                  currency: balance.currencyCode,
                ),
              ),
              _LabelValue(
                label: l10n.restoredAmount,
                child: MoneyText(
                  amount: balance.totalRestoredAmount,
                  currency: balance.currencyCode,
                ),
              ),
              _LabelValue(
                label: l10n.transferredAmount,
                child: MoneyText(
                  amount: balance.totalTransferredOutAmount,
                  currency: balance.currencyCode,
                ),
              ),
              _LabelValue(
                label: l10n.returnedAmount,
                child: MoneyText(
                  amount: balance.totalReturnedAmount,
                  currency: balance.currencyCode,
                ),
              ),
            ],
          ),
        ],
      ),
    );
    return Card(
      clipBehavior: Clip.antiAlias,
      child: onOpen == null ? child : InkWell(onTap: onOpen, child: child),
    );
  }
}

final class FinancialCommandNotice extends StatelessWidget {
  const FinancialCommandNotice({
    required this.phase,
    required this.onRetry,
    required this.onCancel,
    this.errorMessage,
    super.key,
  });

  final FinancialCommandPhase phase;
  final VoidCallback onRetry;
  final VoidCallback onCancel;
  final String? errorMessage;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    if (phase == FinancialCommandPhase.idle ||
        phase == FinancialCommandPhase.submitting ||
        phase == FinancialCommandPhase.success) {
      return const SizedBox.shrink();
    }
    final uncertain = phase == FinancialCommandPhase.uncertainSubmission;
    return Semantics(
      liveRegion: true,
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: uncertain
              ? Theme.of(context).colorScheme.tertiaryContainer
              : Theme.of(context).colorScheme.errorContainer,
          borderRadius: BorderRadius.circular(16),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              uncertain
                  ? l10n.operationStatusUncertain
                  : errorMessage ?? l10n.genericError,
              style: Theme.of(context).textTheme.titleMedium,
            ),
            if (uncertain) ...[
              const SizedBox(height: 8),
              Text(l10n.idempotentRetryExplanation),
              const SizedBox(height: 12),
              Wrap(
                spacing: 8,
                children: [
                  FilledButton.icon(
                    onPressed: onRetry,
                    icon: const Icon(Icons.replay),
                    label: Text(l10n.retrySameOperation),
                  ),
                  TextButton(
                    onPressed: onCancel,
                    child: Text(l10n.cancelUncertainOperation),
                  ),
                ],
              ),
            ],
          ],
        ),
      ),
    );
  }
}

final class _LabelValue extends StatelessWidget {
  const _LabelValue({required this.label, this.value, this.child});

  final String label;
  final String? value;
  final Widget? child;

  @override
  Widget build(BuildContext context) => Semantics(
    label: value == null ? label : '$label: $value',
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(label, style: Theme.of(context).textTheme.labelMedium),
        child ?? Text(value ?? ''),
      ],
    ),
  );
}
