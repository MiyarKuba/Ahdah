import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/localization/app_date_time_formatter.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../controllers/expense_controllers.dart';
import '../widgets/expense_labels.dart';

final class ExpenseHistoryPage extends ConsumerStatefulWidget {
  const ExpenseHistoryPage({required this.expenseId, super.key});
  final String expenseId;
  @override
  ConsumerState<ExpenseHistoryPage> createState() => _ExpenseHistoryPageState();
}

class _ExpenseHistoryPageState extends ConsumerState<ExpenseHistoryPage> {
  @override
  void initState() {
    super.initState();
    Future.microtask(
      () => ref
          .read(expenseHistoryControllerProvider(widget.expenseId).notifier)
          .load(),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(expenseHistoryControllerProvider(widget.expenseId));
    if (state.phase == ExpenseReadPhase.initial ||
        state.phase == ExpenseReadPhase.loading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (state.phase == ExpenseReadPhase.failure) {
      return AccessFailureState(
        message: localizedError(l10n, state.error!),
        onRetry: ref
            .read(expenseHistoryControllerProvider(widget.expenseId).notifier)
            .load,
      );
    }
    if (state.items.isEmpty) {
      return AccessEmptyState(
        title: l10n.expenseHistory,
        body: l10n.noExpenseHistory,
      );
    }
    return ListView.separated(
      padding: const EdgeInsets.all(16),
      itemCount: state.items.length + 2,
      separatorBuilder: (_, _) => const SizedBox(height: 8),
      itemBuilder: (context, index) {
        if (index == 0) {
          return Text(
            l10n.expenseHistory,
            style: Theme.of(context).textTheme.headlineMedium,
          );
        }
        if (index == state.items.length + 1) {
          if (state.phase == ExpenseReadPhase.loadingMore) {
            return const Center(child: CircularProgressIndicator());
          }
          if (state.hasMore || state.loadMoreError != null) {
            return TextButton.icon(
              onPressed: ref
                  .read(
                    expenseHistoryControllerProvider(widget.expenseId).notifier,
                  )
                  .loadMore,
              icon: const Icon(Icons.expand_more),
              label: Text(l10n.retryLoadingMore),
            );
          }
          return const SizedBox.shrink();
        }
        final event = state.items[index - 1];
        return Card(
          child: ListTile(
            leading: const Icon(Icons.history),
            title: Text(expenseHistoryEventLabel(l10n, event.eventName)),
            subtitle: Text(
              '${event.outcome == 'Success' ? l10n.historyOutcomeSuccess : l10n.unknownValue}\n'
              '${event.actor?.fullName ?? l10n.unknownValue} · '
              '${AppDateTimeFormatter.format(context, event.occurredAtUtc)}',
            ),
            isThreeLine: true,
          ),
        );
      },
    );
  }
}
