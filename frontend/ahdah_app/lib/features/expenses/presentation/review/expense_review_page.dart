import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/routing/app_routes.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../advances/presentation/widgets/advance_widgets.dart';
import '../../domain/expense_requests.dart';
import '../controllers/expense_controllers.dart';
import '../widgets/expense_labels.dart';

final class ExpenseReviewPage extends ConsumerStatefulWidget {
  const ExpenseReviewPage({required this.expenseId, super.key});
  final String expenseId;
  @override
  ConsumerState<ExpenseReviewPage> createState() => _ExpenseReviewPageState();
}

class _ExpenseReviewPageState extends ConsumerState<ExpenseReviewPage> {
  final _reason = TextEditingController();
  @override
  void initState() {
    super.initState();
    Future.microtask(
      () => ref
          .read(expenseDetailsControllerProvider(widget.expenseId).notifier)
          .load(),
    );
  }

  @override
  void dispose() {
    _reason.dispose();
    super.dispose();
  }

  Future<bool> _confirm(String title, String body) async =>
      await showDialog<bool>(
        context: context,
        builder: (context) => AlertDialog(
          title: Text(title),
          content: Text(body),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(context, false),
              child: Text(MaterialLocalizations.of(context).cancelButtonLabel),
            ),
            FilledButton(
              onPressed: () => Navigator.pop(context, true),
              child: Text(MaterialLocalizations.of(context).okButtonLabel),
            ),
          ],
        ),
      ) ??
      false;

  Future<void> _approve(int version) async {
    final l10n = AppLocalizations.of(context);
    if (!await _confirm(l10n.approveExpense, l10n.approveExpenseNotice)) return;
    final result = await ref
        .read(expenseApprovalControllerProvider.notifier)
        .approve(
          widget.expenseId,
          ApproveExpenseInput(expectedVersion: version),
        );
    await _complete(result != null, l10n.expenseApproved);
  }

  Future<void> _reject(int version) async {
    final l10n = AppLocalizations.of(context);
    if (_reason.text.trim().isEmpty || _reason.text.trim().length > 1000) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(l10n.invalidRequiredText)));
      return;
    }
    if (!await _confirm(l10n.rejectExpense, l10n.rejectExpenseNotice)) return;
    final result = await ref
        .read(expenseRejectionControllerProvider.notifier)
        .reject(
          widget.expenseId,
          RejectExpenseInput(expectedVersion: version, reason: _reason.text),
        );
    await _complete(result != null, l10n.expenseRejected);
  }

  Future<void> _complete(bool success, String message) async {
    if (!success || !mounted) return;
    await ref.read(expenseListControllerProvider.notifier).refresh();
    await ref.read(reimbursementListControllerProvider.notifier).load();
    ref.invalidate(expenseHistoryControllerProvider(widget.expenseId));
    ref.invalidate(expenseDetailsControllerProvider(widget.expenseId));
    if (!mounted) return;
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(SnackBar(content: Text(message)));
    context.go('${AppRoutes.expensesPath}/${widget.expenseId}');
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final detailsState = ref.watch(
      expenseDetailsControllerProvider(widget.expenseId),
    );
    final approval = ref.watch(expenseApprovalControllerProvider);
    final rejection = ref.watch(expenseRejectionControllerProvider);
    final details = detailsState.details;
    if (details == null) {
      return const Center(child: CircularProgressIndicator());
    }
    final expense = details.expense;
    final busy = approval.isSubmitting || rejection.isSubmitting;
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Text(
          l10n.reviewExpense,
          style: Theme.of(context).textTheme.headlineMedium,
        ),
        const SizedBox(height: 12),
        Card(
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  expense.expenseNumber,
                  style: Theme.of(context).textTheme.titleLarge,
                ),
                MoneyText(
                  amount: expense.totalAmount,
                  currency: expense.currencyCode,
                ),
                Text('${l10n.incurredBy}: ${expense.incurredBy.fullName}'),
                Text(
                  '${l10n.paymentMode}: ${expensePaymentModeLabel(l10n, expense.paymentMode)}',
                ),
                if (expense.project != null)
                  Text(
                    '${l10n.projectOptional}: ${expense.project!.projectName}',
                  ),
                const SizedBox(height: 12),
                Text(l10n.approveExpenseNotice),
                Text(l10n.rejectExpenseNotice),
              ],
            ),
          ),
        ),
        const SizedBox(height: 12),
        TextField(
          controller: _reason,
          maxLength: 1000,
          maxLines: 3,
          enabled: !busy,
          decoration: InputDecoration(labelText: l10n.rejectionReason),
        ),
        if (approval.phase == ExpenseCommandPhase.failure)
          Text(
            localizedError(l10n, approval.error!),
            style: TextStyle(color: Theme.of(context).colorScheme.error),
          ),
        if (rejection.phase == ExpenseCommandPhase.failure)
          Text(
            localizedError(l10n, rejection.error!),
            style: TextStyle(color: Theme.of(context).colorScheme.error),
          ),
        if (approval.isUncertain || rejection.isUncertain)
          Card(
            color: Theme.of(context).colorScheme.errorContainer,
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(l10n.operationStatusUncertain),
                  Text(l10n.idempotentRetryExplanation),
                  TextButton(
                    onPressed: approval.isUncertain
                        ? ref
                              .read(expenseApprovalControllerProvider.notifier)
                              .retrySameOperation
                        : ref
                              .read(expenseRejectionControllerProvider.notifier)
                              .retrySameOperation,
                    child: Text(l10n.retrySameOperation),
                  ),
                ],
              ),
            ),
          ),
        const SizedBox(height: 12),
        if (expense.status == 'PendingReview')
          Wrap(
            spacing: 12,
            runSpacing: 12,
            children: [
              FilledButton.icon(
                key: const Key('approve-expense-action'),
                onPressed: busy ? null : () => _approve(expense.versionNumber),
                icon: const Icon(Icons.check),
                label: Text(l10n.approveExpense),
              ),
              OutlinedButton.icon(
                key: const Key('reject-expense-action'),
                onPressed: busy ? null : () => _reject(expense.versionNumber),
                icon: const Icon(Icons.close),
                label: Text(l10n.rejectExpense),
              ),
            ],
          ),
      ],
    );
  }
}
