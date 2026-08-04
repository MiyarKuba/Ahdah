import 'package:flutter/material.dart';

import '../../../../core/localization/app_date_time_formatter.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../advances/presentation/widgets/advance_widgets.dart';
import '../../domain/expense_models.dart';
import 'expense_labels.dart';

final class ExpenseStatusChip extends StatelessWidget {
  const ExpenseStatusChip({required this.status, super.key});
  final String status;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;
    final color = switch (status) {
      'Approved' => scheme.primaryContainer,
      'PendingReview' ||
      'CorrectionRequired' ||
      'Draft' => scheme.tertiaryContainer,
      'Rejected' || 'Cancelled' || 'Reversed' => scheme.errorContainer,
      _ => scheme.surfaceContainerHighest,
    };
    return Chip(
      avatar: Icon(
        status == 'Approved'
            ? Icons.verified_outlined
            : status == 'PendingReview'
            ? Icons.schedule
            : Icons.info_outline,
        size: 17,
      ),
      label: Text(expenseStatusLabel(l10n, status)),
      backgroundColor: color,
      side: BorderSide.none,
    );
  }
}

final class ExpenseSummaryCard extends StatelessWidget {
  const ExpenseSummaryCard({
    required this.expense,
    required this.onOpen,
    super.key,
  });
  final ExpenseSummary expense;
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
                      expense.expenseNumber,
                      style: Theme.of(context).textTheme.titleMedium,
                    ),
                  ),
                  ExpenseStatusChip(status: expense.status),
                ],
              ),
              MoneyText(
                amount: expense.totalAmount,
                currency: expense.currencyCode,
                style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                  fontWeight: FontWeight.w700,
                ),
              ),
              const SizedBox(height: 8),
              Text(
                expense.description,
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
              ),
              const SizedBox(height: 8),
              Wrap(
                spacing: 16,
                runSpacing: 6,
                children: [
                  Text('${l10n.category}: ${expense.category.categoryName}'),
                  Text(
                    '${l10n.paymentMode}: '
                    '${expensePaymentModeLabel(l10n, expense.paymentMode)}',
                  ),
                  if (expense.project != null)
                    Text(
                      '${l10n.projectOptional}: ${expense.project!.projectName}',
                    ),
                  if (expense.expenseDate != null)
                    Text(
                      '${l10n.expenseDate}: '
                      '${AppDateTimeFormatter.formatDate(context, expense.expenseDate!)}',
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
