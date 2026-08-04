import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/localization/app_date_time_formatter.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../controllers/expense_controllers.dart';
import '../widgets/expense_labels.dart';

final class ExpenseDocumentsPage extends ConsumerStatefulWidget {
  const ExpenseDocumentsPage({required this.expenseId, super.key});
  final String expenseId;
  @override
  ConsumerState<ExpenseDocumentsPage> createState() =>
      _ExpenseDocumentsPageState();
}

class _ExpenseDocumentsPageState extends ConsumerState<ExpenseDocumentsPage> {
  @override
  void initState() {
    super.initState();
    Future.microtask(
      () => ref
          .read(expenseDocumentControllerProvider(widget.expenseId).notifier)
          .load(),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(
      expenseDocumentControllerProvider(widget.expenseId),
    );
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Text(
          l10n.documentMetadata,
          style: Theme.of(context).textTheme.headlineMedium,
        ),
        const SizedBox(height: 12),
        Card(
          color: Theme.of(context).colorScheme.tertiaryContainer,
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Text(l10n.binaryUploadNotImplemented),
          ),
        ),
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
                .read(
                  expenseDocumentControllerProvider(widget.expenseId).notifier,
                )
                .load,
          )
        else if (state.items.isEmpty)
          AccessEmptyState(title: l10n.documents, body: l10n.noDocuments)
        else
          for (final document in state.items)
            Card(
              child: ListTile(
                leading: const Icon(Icons.description_outlined),
                title: Text(
                  expenseDocumentTypeLabel(l10n, document.documentType),
                ),
                subtitle: Text(
                  '${document.originalFileName}\n'
                  '${documentStatusLabel(l10n, document.verificationStatus)} · '
                  '${AppDateTimeFormatter.format(context, document.createdAtUtc)}',
                ),
                isThreeLine: true,
              ),
            ),
        if (state.phase == ExpenseReadPhase.loadingMore)
          const Padding(
            padding: EdgeInsets.all(16),
            child: Center(child: CircularProgressIndicator()),
          )
        else if (state.hasMore)
          TextButton.icon(
            onPressed: ref
                .read(
                  expenseDocumentControllerProvider(widget.expenseId).notifier,
                )
                .loadMore,
            icon: const Icon(Icons.expand_more),
            label: Text(l10n.retryLoadingMore),
          ),
        if (state.loadMoreError != null)
          TextButton.icon(
            onPressed: ref
                .read(
                  expenseDocumentControllerProvider(widget.expenseId).notifier,
                )
                .loadMore,
            icon: const Icon(Icons.refresh),
            label: Text(l10n.retryLoadingMore),
          ),
      ],
    );
  }
}
