import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/routing/app_routes.dart';
import '../../../../core/localization/app_date_time_formatter.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../core/errors/app_exception.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../../../advances/presentation/widgets/advance_widgets.dart';
import '../../domain/expense_models.dart';
import '../controllers/expense_controllers.dart';
import '../widgets/expense_labels.dart';

final class ReimbursementsPage extends ConsumerStatefulWidget {
  const ReimbursementsPage({super.key});
  @override
  ConsumerState<ReimbursementsPage> createState() => _ReimbursementsPageState();
}

class _ReimbursementsPageState extends ConsumerState<ReimbursementsPage> {
  @override
  void initState() {
    super.initState();
    Future.microtask(
      () => ref.read(reimbursementListControllerProvider.notifier).load(),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(reimbursementListControllerProvider);
    return RefreshIndicator(
      onRefresh: ref.read(reimbursementListControllerProvider.notifier).load,
      child: ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        padding: const EdgeInsets.all(16),
        children: [
          Row(
            children: [
              Expanded(
                child: Text(
                  l10n.reimbursements,
                  style: Theme.of(context).textTheme.headlineMedium,
                ),
              ),
              IconButton(
                tooltip: l10n.refresh,
                onPressed: ref
                    .read(reimbursementListControllerProvider.notifier)
                    .load,
                icon: const Icon(Icons.refresh),
              ),
            ],
          ),
          const SizedBox(height: 12),
          DropdownButtonFormField<String?>(
            initialValue: state.status,
            decoration: InputDecoration(labelText: l10n.statusFilter),
            items: [
              DropdownMenuItem<String?>(
                value: null,
                child: Text(l10n.allStatuses),
              ),
              for (final status in reimbursementStatuses)
                DropdownMenuItem<String?>(
                  value: status,
                  child: Text(reimbursementStatusLabel(l10n, status)),
                ),
            ],
            onChanged: (value) => ref
                .read(reimbursementListControllerProvider.notifier)
                .setStatus(value),
          ),
          const SizedBox(height: 12),
          if (state.phase == ExpenseReadPhase.initial ||
              state.phase == ExpenseReadPhase.loading)
            const Padding(
              padding: EdgeInsets.all(32),
              child: Center(child: CircularProgressIndicator()),
            )
          else if (state.phase == ExpenseReadPhase.failure)
            AccessFailureState(
              message: localizedError(l10n, state.error!),
              onRetry: ref
                  .read(reimbursementListControllerProvider.notifier)
                  .load,
            )
          else if (state.items.isEmpty)
            AccessEmptyState(
              title: l10n.reimbursements,
              body: l10n.noReimbursements,
            )
          else
            for (final claim in state.items)
              _ReimbursementCard(
                claim: claim,
                onOpen: () => context.go(
                  '${AppRoutes.reimbursementsPath}/${claim.reimbursementId}',
                ),
              ),
          if (state.phase == ExpenseReadPhase.loadingMore)
            const Padding(
              padding: EdgeInsets.all(16),
              child: Center(child: CircularProgressIndicator()),
            )
          else if (state.hasMore || state.loadMoreError != null)
            TextButton.icon(
              onPressed: ref
                  .read(reimbursementListControllerProvider.notifier)
                  .loadMore,
              icon: const Icon(Icons.expand_more),
              label: Text(l10n.retryLoadingMore),
            ),
        ],
      ),
    );
  }
}

final class _ReimbursementCard extends StatelessWidget {
  const _ReimbursementCard({required this.claim, required this.onOpen});
  final ReimbursementSummary claim;
  final VoidCallback onOpen;
  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Card(
      child: ListTile(
        onTap: onOpen,
        title: Text(claim.reimbursementNumber),
        subtitle: Text(
          '${claim.expenseNumber} · ${claim.claimant.fullName}\n'
          '${reimbursementStatusLabel(l10n, claim.status)}',
        ),
        isThreeLine: true,
        trailing: MoneyText(
          amount: claim.outstandingAmount,
          currency: claim.currencyCode,
        ),
      ),
    );
  }
}

final class ReimbursementDetailsPage extends ConsumerWidget {
  const ReimbursementDetailsPage({required this.reimbursementId, super.key});
  final String reimbursementId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final result = ref.watch(
      reimbursementDetailsControllerProvider(reimbursementId),
    );
    return result.when(
      loading: () => const Center(child: CircularProgressIndicator()),
      error: (error, _) => AccessFailureState(
        message: error is AppException
            ? localizedError(l10n, error)
            : l10n.unknownValue,
        onRetry: () => ref.invalidate(
          reimbursementDetailsControllerProvider(reimbursementId),
        ),
      ),
      data: (claim) => ListView(
        padding: const EdgeInsets.all(16),
        children: [
          Text(
            l10n.reimbursementDetails,
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
                    claim.reimbursementNumber,
                    style: Theme.of(context).textTheme.titleLarge,
                  ),
                  MoneyText(
                    amount: claim.claimAmount,
                    currency: claim.currencyCode,
                    style: Theme.of(context).textTheme.headlineMedium,
                  ),
                  const SizedBox(height: 12),
                  Text(
                    '${l10n.status}: ${reimbursementStatusLabel(l10n, claim.status)}',
                  ),
                  Text(
                    '${l10n.outstandingAmount}: ${claim.outstandingAmount} ${claim.currencyCode}',
                  ),
                  Text('${l10n.claimant}: ${claim.claimant.fullName}'),
                  Text('${l10n.expenseReference}: ${claim.expenseNumber}'),
                  Text(
                    '${l10n.claimDate}: ${AppDateTimeFormatter.formatDate(context, claim.claimDate)}',
                  ),
                  if (claim.dueDate != null)
                    Text(
                      '${l10n.dueDate}: ${AppDateTimeFormatter.formatDate(context, claim.dueDate)}',
                    ),
                  const SizedBox(height: 12),
                  Text(l10n.claimUnpaid),
                ],
              ),
            ),
          ),
          FilledButton.icon(
            onPressed: () =>
                context.go('${AppRoutes.expensesPath}/${claim.expenseId}'),
            icon: const Icon(Icons.open_in_new),
            label: Text(l10n.expenseDetails),
          ),
        ],
      ),
    );
  }
}
