import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/localization/app_date_time_formatter.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../core/localization/value_labels.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/domain/role_capabilities.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../../../session/presentation/session_controller.dart';
import '../../domain/advance_models.dart';
import '../../domain/advance_requests.dart';
import '../controllers/advance_controllers.dart';
import '../widgets/advance_widgets.dart';

final class AdvanceMovementsPage extends ConsumerStatefulWidget {
  const AdvanceMovementsPage({required this.advanceId, super.key});

  final String advanceId;

  @override
  ConsumerState<AdvanceMovementsPage> createState() =>
      _AdvanceMovementsPageState();
}

class _AdvanceMovementsPageState extends ConsumerState<AdvanceMovementsPage> {
  final _scroll = ScrollController();

  @override
  void initState() {
    super.initState();
    Future.microtask(
      () => ref
          .read(advanceMovementsControllerProvider(widget.advanceId).notifier)
          .load(),
    );
    _scroll.addListener(() {
      if (_scroll.position.extentAfter < 320) {
        ref
            .read(advanceMovementsControllerProvider(widget.advanceId).notifier)
            .loadMore();
      }
    });
  }

  @override
  void dispose() {
    _scroll.dispose();
    super.dispose();
  }

  Future<void> _refreshAffected() async {
    await Future.wait([
      ref
          .read(advanceMovementsControllerProvider(widget.advanceId).notifier)
          .load(),
      ref
          .read(advanceDetailsControllerProvider(widget.advanceId).notifier)
          .load(),
      ref.read(advanceListControllerProvider.notifier).refresh(),
      ref.read(personalAdvanceBalanceControllerProvider.notifier).refresh(),
    ]);
  }

  Future<void> _confirm(AdvanceMovement movement) async {
    final l10n = AppLocalizations.of(context);
    final confirmed = await showAccessConfirmation(
      context,
      title: l10n.confirmFinancialOperationTitle,
      body: '${l10n.confirmReceiptBody}\n\n${movement.amount}',
      confirmLabel: l10n.confirmReceipt,
    );
    if (!confirmed || !mounted) return;
    final result = await ref
        .read(advanceConfirmationControllerProvider.notifier)
        .confirm(movement.movementId);
    if (result != null) await _refreshAffected();
  }

  Future<void> _reject(AdvanceMovement movement) async {
    final l10n = AppLocalizations.of(context);
    final reason = TextEditingController();
    final formKey = GlobalKey<FormState>();
    final value = await showDialog<String>(
      context: context,
      builder: (context) => AlertDialog(
        title: Text(l10n.rejectTransfer),
        content: Form(
          key: formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(l10n.rejectTransferBody),
              const SizedBox(height: 12),
              TextFormField(
                controller: reason,
                autofocus: true,
                maxLength: 500,
                validator: (value) {
                  final normalized = value?.trim() ?? '';
                  return normalized.isEmpty ||
                          normalized.length > 500 ||
                          normalized.contains('\n') ||
                          normalized.contains('\r')
                      ? l10n.invalidRejectionReason
                      : null;
                },
                decoration: InputDecoration(labelText: l10n.rejectionReason),
              ),
            ],
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: Text(l10n.cancelAction),
          ),
          FilledButton(
            onPressed: () {
              if (formKey.currentState?.validate() ?? false) {
                Navigator.pop(context, reason.text.trim());
              }
            },
            child: Text(l10n.rejectTransfer),
          ),
        ],
      ),
    );
    reason.dispose();
    if (value == null || !mounted) return;
    final result = await ref
        .read(advanceRejectionControllerProvider.notifier)
        .reject(movement.movementId, RejectAdvanceTransferInput(reason: value));
    if (result != null) await _refreshAffected();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(
      advanceMovementsControllerProvider(widget.advanceId),
    );
    final session = ref.watch(sessionControllerProvider).current!;
    final capabilities = RoleCapabilities.forRole(session.role);
    final confirmation = ref.watch(advanceConfirmationControllerProvider);
    final rejection = ref.watch(advanceRejectionControllerProvider);
    final submitting = confirmation.isSubmitting || rejection.isSubmitting;

    if (state.phase == PagedReadPhase.initial ||
        state.phase == PagedReadPhase.loading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (state.phase == PagedReadPhase.failure) {
      return AccessFailureState(
        message: localizedError(l10n, state.error!),
        onRetry: ref
            .read(advanceMovementsControllerProvider(widget.advanceId).notifier)
            .load,
      );
    }

    return Scaffold(
      body: RefreshIndicator(
        onRefresh: _refreshAffected,
        child: ListView(
          controller: _scroll,
          physics: const AlwaysScrollableScrollPhysics(),
          padding: const EdgeInsets.all(16),
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    l10n.movementHistory,
                    style: Theme.of(context).textTheme.headlineMedium,
                  ),
                ),
                IconButton(
                  tooltip: l10n.refresh,
                  onPressed: _refreshAffected,
                  icon: const Icon(Icons.refresh),
                ),
              ],
            ),
            const SizedBox(height: 12),
            FinancialCommandNotice(
              phase: confirmation.phase,
              errorMessage: confirmation.error == null
                  ? null
                  : localizedError(l10n, confirmation.error!),
              onRetry: () async {
                final result = await ref
                    .read(advanceConfirmationControllerProvider.notifier)
                    .retrySameOperation();
                if (result != null) await _refreshAffected();
              },
              onCancel: ref
                  .read(advanceConfirmationControllerProvider.notifier)
                  .cancelPending,
            ),
            FinancialCommandNotice(
              phase: rejection.phase,
              errorMessage: rejection.error == null
                  ? null
                  : localizedError(l10n, rejection.error!),
              onRetry: () async {
                final result = await ref
                    .read(advanceRejectionControllerProvider.notifier)
                    .retrySameOperation();
                if (result != null) await _refreshAffected();
              },
              onCancel: ref
                  .read(advanceRejectionControllerProvider.notifier)
                  .cancelPending,
            ),
            if (state.items.isEmpty)
              AccessEmptyState(
                title: l10n.noMovementsTitle,
                body: l10n.noMovementsBody,
              )
            else
              for (final movement in state.items)
                _MovementCard(
                  movement: movement,
                  canConfirm:
                      !submitting &&
                      capabilities.canConfirmReceivedAdvance &&
                      movement.isTransfer &&
                      movement.status == 'PendingConfirmation' &&
                      movement.recipient?.userId == session.user.userId,
                  canReject:
                      !submitting &&
                      capabilities.canRejectReceivedTransfer &&
                      movement.canBeRejected &&
                      movement.status == 'PendingConfirmation' &&
                      movement.recipient?.userId == session.user.userId,
                  onConfirm: () => _confirm(movement),
                  onReject: () => _reject(movement),
                ),
            if (state.phase == PagedReadPhase.loadingMore)
              const Center(
                child: Padding(
                  padding: EdgeInsets.all(16),
                  child: CircularProgressIndicator(),
                ),
              ),
            if (state.loadMoreError != null)
              TextButton.icon(
                onPressed: ref
                    .read(
                      advanceMovementsControllerProvider(
                        widget.advanceId,
                      ).notifier,
                    )
                    .loadMore,
                icon: const Icon(Icons.refresh),
                label: Text(l10n.retryLoadingMore),
              ),
          ],
        ),
      ),
    );
  }
}

final class _MovementCard extends StatelessWidget {
  const _MovementCard({
    required this.movement,
    required this.canConfirm,
    required this.canReject,
    required this.onConfirm,
    required this.onReject,
  });

  final AdvanceMovement movement;
  final bool canConfirm;
  final bool canReject;
  final VoidCallback onConfirm;
  final VoidCallback onReject;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                const Icon(Icons.timeline),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(
                    ValueLabels.movementType(l10n, movement.operationType),
                    style: Theme.of(context).textTheme.titleMedium,
                  ),
                ),
                AdvanceStatusChip(status: movement.status, transfer: true),
              ],
            ),
            Directionality(
              textDirection: TextDirection.ltr,
              child: Text(
                movement.amount,
                style: Theme.of(context).textTheme.headlineSmall,
              ),
            ),
            Text(
              '${l10n.sender}: ${movement.sender?.fullName ?? movement.actor.fullName}',
            ),
            if (movement.recipient != null)
              Text('${l10n.recipient}: ${movement.recipient!.fullName}'),
            Text(AppDateTimeFormatter.format(context, movement.occurredAtUtc)),
            if (canConfirm || canReject) ...[
              const SizedBox(height: 12),
              Wrap(
                spacing: 8,
                children: [
                  if (canConfirm)
                    FilledButton(
                      key: Key('confirm-transfer-${movement.movementId}'),
                      onPressed: onConfirm,
                      child: Text(l10n.confirmReceipt),
                    ),
                  if (canReject)
                    OutlinedButton(
                      key: Key('reject-transfer-${movement.movementId}'),
                      onPressed: onReject,
                      child: Text(l10n.rejectTransfer),
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
