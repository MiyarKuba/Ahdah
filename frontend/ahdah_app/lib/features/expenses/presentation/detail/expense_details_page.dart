import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/routing/app_routes.dart';
import '../../../../core/localization/app_date_time_formatter.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/domain/role_capabilities.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../../../advances/presentation/widgets/advance_widgets.dart';
import '../../../session/presentation/session_controller.dart';
import '../controllers/expense_controllers.dart';
import '../widgets/expense_labels.dart';
import '../widgets/expense_widgets.dart';

final class ExpenseDetailsPage extends ConsumerStatefulWidget {
  const ExpenseDetailsPage({required this.expenseId, super.key});
  final String expenseId;

  @override
  ConsumerState<ExpenseDetailsPage> createState() => _ExpenseDetailsPageState();
}

class _ExpenseDetailsPageState extends ConsumerState<ExpenseDetailsPage> {
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
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(expenseDetailsControllerProvider(widget.expenseId));
    final capabilities = RoleCapabilities.forRole(
      ref.watch(sessionControllerProvider).current!.role,
    );
    if (state.phase == ExpenseDetailsPhase.initial ||
        state.phase == ExpenseDetailsPhase.loading && state.details == null) {
      return const Center(child: CircularProgressIndicator());
    }
    if (state.phase == ExpenseDetailsPhase.failure && state.details == null) {
      return AccessFailureState(
        message: localizedError(l10n, state.error!),
        onRetry: ref
            .read(expenseDetailsControllerProvider(widget.expenseId).notifier)
            .load,
      );
    }
    final details = state.details!;
    final expense = details.expense;
    final canReview =
        capabilities.canReviewExpenses && expense.status == 'PendingReview';
    return RefreshIndicator(
      onRefresh: ref
          .read(expenseDetailsControllerProvider(widget.expenseId).notifier)
          .load,
      child: ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        padding: const EdgeInsets.all(16),
        children: [
          Row(
            children: [
              Expanded(
                child: Text(
                  l10n.expenseDetails,
                  style: Theme.of(context).textTheme.headlineMedium,
                ),
              ),
              ExpenseStatusChip(status: expense.status),
            ],
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
                  const SizedBox(height: 6),
                  MoneyText(
                    amount: expense.totalAmount,
                    currency: expense.currencyCode,
                    style: Theme.of(context).textTheme.headlineMedium,
                  ),
                  const Divider(height: 24),
                  _Detail(label: l10n.description, value: expense.description),
                  _Detail(
                    label: l10n.category,
                    value: expense.category.categoryName,
                  ),
                  _Detail(
                    label: l10n.paymentMode,
                    value: expensePaymentModeLabel(l10n, expense.paymentMode),
                  ),
                  _Detail(
                    label: l10n.incurredBy,
                    value: expense.incurredBy.fullName,
                  ),
                  _Detail(
                    label: l10n.submittedBy,
                    value: expense.submittedBy.fullName,
                  ),
                  if (expense.project != null)
                    _Detail(
                      label: l10n.projectOptional,
                      value: expense.project!.projectName,
                    ),
                  if (expense.expenseDate != null)
                    _Detail(
                      label: l10n.expenseDate,
                      value: AppDateTimeFormatter.formatDate(
                        context,
                        expense.expenseDate,
                      ),
                    ),
                  if (expense.receiptNumber != null)
                    _Detail(
                      label: l10n.receiptNumber,
                      value: expense.receiptNumber!,
                    ),
                  if (expense.invoiceNumber != null)
                    _Detail(
                      label: l10n.invoiceNumber,
                      value: expense.invoiceNumber!,
                    ),
                  if (details.merchantName != null)
                    _Detail(
                      label: l10n.merchantNameOptional,
                      value: details.merchantName!,
                    ),
                  if (details.expenseLocation != null)
                    _Detail(
                      label: l10n.expenseLocationOptional,
                      value: details.expenseLocation!,
                    ),
                  if (details.notes != null)
                    _Detail(label: l10n.notesOptional, value: details.notes!),
                  if (details.reviewedBy != null)
                    _Detail(
                      label: l10n.reviewedBy,
                      value: details.reviewedBy!.fullName,
                    ),
                  if (details.rejectionReason != null)
                    _Detail(
                      label: l10n.rejectionReason,
                      value: details.rejectionReason!,
                    ),
                ],
              ),
            ),
          ),
          if (details.advanceAllocations.isNotEmpty)
            _Section(
              title: l10n.advanceAllocations,
              children: [
                for (final allocation in details.advanceAllocations)
                  ListTile(
                    title: Text(allocation.advanceNumber),
                    trailing: MoneyText(
                      amount: allocation.allocatedAmount,
                      currency: expense.currencyCode,
                    ),
                  ),
              ],
            ),
          if (details.items.isNotEmpty)
            _Section(
              title: l10n.expenseItems,
              children: [
                for (final item in details.items)
                  ListTile(
                    title: Text(item.itemName),
                    subtitle: Text('${item.quantity} ${item.unitCode}'),
                    trailing: MoneyText(
                      amount: item.totalAmount,
                      currency: expense.currencyCode,
                    ),
                  ),
              ],
            ),
          if (details.reimbursement != null)
            _Section(
              title: l10n.reimbursements,
              children: [
                ListTile(
                  onTap: () => context.go(
                    '${AppRoutes.reimbursementsPath}/'
                    '${details.reimbursement!.reimbursementId}',
                  ),
                  title: Text(details.reimbursement!.reimbursementNumber),
                  subtitle: Text(
                    reimbursementStatusLabel(
                      l10n,
                      details.reimbursement!.status,
                    ),
                  ),
                  trailing: const Icon(Icons.chevron_right),
                ),
              ],
            ),
          const SizedBox(height: 8),
          Wrap(
            spacing: 12,
            runSpacing: 12,
            children: [
              OutlinedButton.icon(
                onPressed: () => context.go(
                  '${AppRoutes.expensesPath}/${widget.expenseId}/documents',
                ),
                icon: const Icon(Icons.description_outlined),
                label: Text(l10n.documents),
              ),
              OutlinedButton.icon(
                onPressed: () => context.go(
                  '${AppRoutes.expensesPath}/${widget.expenseId}/history',
                ),
                icon: const Icon(Icons.history),
                label: Text(l10n.history),
              ),
              if (canReview)
                FilledButton.icon(
                  key: const Key('review-expense-action'),
                  onPressed: () => context.go(
                    '${AppRoutes.expensesPath}/${widget.expenseId}/review',
                  ),
                  icon: const Icon(Icons.fact_check_outlined),
                  label: Text(l10n.reviewExpense),
                ),
            ],
          ),
        ],
      ),
    );
  }
}

final class _Detail extends StatelessWidget {
  const _Detail({required this.label, required this.value});
  final String label;
  final String value;

  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.only(bottom: 8),
    child: Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        SizedBox(
          width: 150,
          child: Text(label, style: Theme.of(context).textTheme.labelLarge),
        ),
        Expanded(child: SelectableText(value)),
      ],
    ),
  );
}

final class _Section extends StatelessWidget {
  const _Section({required this.title, required this.children});
  final String title;
  final List<Widget> children;
  @override
  Widget build(BuildContext context) => Card(
    child: Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.all(16),
            child: Text(title, style: Theme.of(context).textTheme.titleMedium),
          ),
          ...children,
        ],
      ),
    ),
  );
}
