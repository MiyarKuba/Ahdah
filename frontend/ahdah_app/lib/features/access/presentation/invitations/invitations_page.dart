import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/localization/app_date_time_formatter.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../core/localization/value_labels.dart';
import '../../../../core/validation/validators.dart';
import '../../../../core/widgets/form_widgets.dart';
import '../../../../l10n/app_localizations.dart';
import '../../domain/access_list_state.dart';
import '../../domain/access_models.dart';
import '../controllers/access_list_controllers.dart';
import '../controllers/access_write_controllers.dart';
import '../widgets/access_widgets.dart';

final class InvitationsPage extends ConsumerStatefulWidget {
  const InvitationsPage({super.key});

  @override
  ConsumerState<InvitationsPage> createState() => _InvitationsPageState();
}

class _InvitationsPageState extends ConsumerState<InvitationsPage> {
  final _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    _scrollController.addListener(_onScroll);
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final controller = ref.read(invitationListControllerProvider.notifier);
      if (ref.read(invitationListControllerProvider).phase ==
          AccessListPhase.initial) {
        controller.load();
      }
    });
  }

  void _onScroll() {
    if (_scrollController.position.extentAfter < 280) {
      ref.read(invitationListControllerProvider.notifier).loadMore();
    }
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  Future<void> _createInvitation() async {
    CreatedInvitation? created = await showDialog<CreatedInvitation>(
      context: context,
      barrierDismissible: false,
      builder: (_) => const _CreateInvitationDialog(),
    );
    if (!mounted || created == null) return;
    await showDialog<void>(
      context: context,
      barrierDismissible: false,
      builder: (_) => _InvitationTokenDialog(result: created!),
    );
    created = null;
    if (mounted) {
      await ref.read(invitationListControllerProvider.notifier).refresh();
    }
  }

  Future<void> _cancel(Invitation invitation) async {
    final l10n = AppLocalizations.of(context);
    final confirmed = await showAccessConfirmation(
      context,
      title: l10n.cancelInvitationTitle,
      body: l10n.cancelInvitationBody,
      confirmLabel: l10n.cancelInvitation,
    );
    if (!confirmed || !mounted) return;
    final result = await ref
        .read(invitationCancellationControllerProvider.notifier)
        .cancel(invitation.id);
    if (!mounted) return;
    if (result != null) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(l10n.invitationCancelled)));
      await ref.read(invitationListControllerProvider.notifier).refresh();
    } else {
      final error = ref.read(invitationCancellationControllerProvider).error;
      if (error != null) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text(localizedError(l10n, error))));
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(invitationListControllerProvider);
    final cancellation = ref.watch(invitationCancellationControllerProvider);
    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 16, 20, 0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Wrap(
            alignment: WrapAlignment.spaceBetween,
            crossAxisAlignment: WrapCrossAlignment.center,
            spacing: 12,
            runSpacing: 12,
            children: [
              SizedBox(
                width: 230,
                child: DropdownButtonFormField<String>(
                  isExpanded: true,
                  initialValue: state.filter ?? '',
                  decoration: InputDecoration(labelText: l10n.statusFilter),
                  items: [
                    DropdownMenuItem(value: '', child: Text(l10n.allStatuses)),
                    for (final status in const [
                      'Pending',
                      'Accepted',
                      'Expired',
                      'Cancelled',
                    ])
                      DropdownMenuItem(
                        value: status,
                        child: Text(ValueLabels.invitationStatus(l10n, status)),
                      ),
                  ],
                  onChanged: (value) => ref
                      .read(invitationListControllerProvider.notifier)
                      .setFilter(value == null || value.isEmpty ? null : value),
                ),
              ),
              Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  IconButton(
                    tooltip: l10n.refresh,
                    onPressed: () => ref
                        .read(invitationListControllerProvider.notifier)
                        .refresh(),
                    icon: const Icon(Icons.refresh),
                  ),
                  const SizedBox(width: 8),
                  FilledButton.icon(
                    onPressed: _createInvitation,
                    icon: const Icon(Icons.add),
                    label: Text(l10n.createInvitation),
                  ),
                ],
              ),
            ],
          ),
          const SizedBox(height: 12),
          Expanded(child: _buildBody(l10n, state, cancellation.isSubmitting)),
        ],
      ),
    );
  }

  Widget _buildBody(
    AppLocalizations l10n,
    AccessListState<Invitation> state,
    bool cancelling,
  ) {
    if (state.phase == AccessListPhase.loading ||
        state.phase == AccessListPhase.initial) {
      return const Center(child: CircularProgressIndicator());
    }
    if (state.phase == AccessListPhase.failure && state.items.isEmpty) {
      return AccessFailureState(
        message: localizedError(l10n, state.error!),
        onRetry: () =>
            ref.read(invitationListControllerProvider.notifier).load(),
      );
    }
    if (state.items.isEmpty) {
      return AccessEmptyState(
        title: l10n.noInvitationsTitle,
        body: l10n.noInvitationsBody,
      );
    }
    return RefreshIndicator(
      onRefresh: () =>
          ref.read(invitationListControllerProvider.notifier).refresh(),
      child: ListView.separated(
        controller: _scrollController,
        physics: const AlwaysScrollableScrollPhysics(),
        itemCount: state.items.length + 1,
        separatorBuilder: (_, _) => const SizedBox(height: 12),
        itemBuilder: (context, index) {
          if (index == state.items.length) {
            if (state.phase == AccessListPhase.loadingMore) {
              return const Padding(
                padding: EdgeInsets.all(20),
                child: Center(child: CircularProgressIndicator()),
              );
            }
            if (state.loadMoreError != null) {
              return TextButton.icon(
                onPressed: () => ref
                    .read(invitationListControllerProvider.notifier)
                    .loadMore(),
                icon: const Icon(Icons.refresh),
                label: Text(l10n.retryLoadingMore),
              );
            }
            return const SizedBox(height: 20);
          }
          final invitation = state.items[index];
          return Card(
            child: Padding(
              padding: const EdgeInsets.all(18),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Expanded(
                        child: SelectableText(
                          invitation.invitedPhoneNumber,
                          textDirection: TextDirection.ltr,
                          style: Theme.of(context).textTheme.titleMedium,
                        ),
                      ),
                      AccessStatusChip(
                        status: invitation.status,
                        kind: AccessStatusKind.invitation,
                      ),
                    ],
                  ),
                  AccessDetail(
                    label: l10n.assignedRole,
                    value: ValueLabels.role(l10n, invitation.assignedRole),
                  ),
                  AccessDetail(
                    label: l10n.createdAt,
                    value: AppDateTimeFormatter.format(
                      context,
                      invitation.createdAtUtc,
                    ),
                  ),
                  AccessDetail(
                    label: l10n.expiresAt,
                    value: AppDateTimeFormatter.format(
                      context,
                      invitation.expiresAtUtc,
                    ),
                  ),
                  if (invitation.acceptedAtUtc != null)
                    AccessDetail(
                      label: l10n.acceptedAt,
                      value: AppDateTimeFormatter.format(
                        context,
                        invitation.acceptedAtUtc,
                      ),
                    ),
                  if (invitation.cancelledAtUtc != null)
                    AccessDetail(
                      label: l10n.cancelledAt,
                      value: AppDateTimeFormatter.format(
                        context,
                        invitation.cancelledAtUtc,
                      ),
                    ),
                  if (invitation.canCancel) ...[
                    const SizedBox(height: 12),
                    Align(
                      alignment: AlignmentDirectional.centerEnd,
                      child: TextButton.icon(
                        onPressed: cancelling
                            ? null
                            : () => _cancel(invitation),
                        icon: const Icon(Icons.cancel_outlined),
                        label: Text(l10n.cancelInvitation),
                      ),
                    ),
                  ],
                ],
              ),
            ),
          );
        },
      ),
    );
  }
}

final class _CreateInvitationDialog extends ConsumerStatefulWidget {
  const _CreateInvitationDialog();

  @override
  ConsumerState<_CreateInvitationDialog> createState() =>
      _CreateInvitationDialogState();
}

class _CreateInvitationDialogState
    extends ConsumerState<_CreateInvitationDialog> {
  final _formKey = GlobalKey<FormState>();
  final _phone = TextEditingController();
  String _role = 'Worker';

  @override
  void dispose() {
    _phone.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    final result = await ref
        .read(invitationCreationControllerProvider.notifier)
        .create(
          CreateInvitationInput(phoneNumber: _phone.text, assignedRole: _role),
        );
    if (mounted && result != null) Navigator.pop(context, result);
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(invitationCreationControllerProvider);
    return AlertDialog(
      title: Text(l10n.createInvitation),
      content: SizedBox(
        width: 440,
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              if (state.error != null) ...[
                AppMessageBanner(
                  message: localizedError(l10n, state.error!),
                  isError: true,
                ),
                const SizedBox(height: 16),
              ],
              TextFormField(
                key: const Key('invitation-phone-field'),
                controller: _phone,
                enabled: !state.isSubmitting,
                autofocus: true,
                textDirection: TextDirection.ltr,
                keyboardType: TextInputType.phone,
                decoration: InputDecoration(
                  labelText: l10n.phoneNumber,
                  helperText: l10n.phoneHint,
                ),
                validator: (value) => Validators.phone(
                  value,
                  l10n.requiredField,
                  l10n.invalidPhone,
                ),
              ),
              const SizedBox(height: 16),
              DropdownButtonFormField<String>(
                isExpanded: true,
                key: const Key('invitation-role-field'),
                initialValue: _role,
                decoration: InputDecoration(labelText: l10n.assignedRole),
                items: ValueLabels.assignableRoles
                    .map(
                      (role) => DropdownMenuItem(
                        value: role,
                        child: Text(ValueLabels.role(l10n, role)),
                      ),
                    )
                    .toList(growable: false),
                onChanged: state.isSubmitting
                    ? null
                    : (value) => setState(() => _role = value ?? _role),
              ),
            ],
          ),
        ),
      ),
      actions: [
        TextButton(
          onPressed: state.isSubmitting ? null : () => Navigator.pop(context),
          child: Text(l10n.cancelAction),
        ),
        FilledButton(
          key: const Key('create-invitation-submit'),
          onPressed: state.isSubmitting ? null : _submit,
          child: state.isSubmitting
              ? const SizedBox.square(
                  dimension: 20,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : Text(l10n.createInvitation),
        ),
      ],
    );
  }
}

final class _InvitationTokenDialog extends StatelessWidget {
  const _InvitationTokenDialog({required this.result});
  final CreatedInvitation result;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return AlertDialog(
      title: Text(l10n.invitationCreatedTitle),
      content: SizedBox(
        width: 520,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            AppMessageBanner(message: l10n.oneTimeTokenWarning),
            const SizedBox(height: 16),
            AccessDetail(
              label: l10n.phoneNumber,
              value: result.invitation.invitedPhoneNumber,
            ),
            AccessDetail(
              label: l10n.assignedRole,
              value: ValueLabels.role(l10n, result.invitation.assignedRole),
            ),
            AccessDetail(
              label: l10n.expiresAt,
              value: AppDateTimeFormatter.format(
                context,
                result.invitation.expiresAtUtc,
              ),
            ),
            const SizedBox(height: 16),
            Text(
              l10n.invitationToken,
              style: Theme.of(context).textTheme.labelLarge,
            ),
            const SizedBox(height: 6),
            DecoratedBox(
              decoration: BoxDecoration(
                color: Theme.of(context).colorScheme.surfaceContainerHighest,
                borderRadius: BorderRadius.circular(12),
              ),
              child: Padding(
                padding: const EdgeInsets.all(14),
                child: SelectableText(
                  result.token,
                  textDirection: TextDirection.ltr,
                ),
              ),
            ),
            const SizedBox(height: 8),
            Text(l10n.invitationDeliveryNotImplemented),
          ],
        ),
      ),
      actions: [
        TextButton.icon(
          key: const Key('copy-invitation-token'),
          onPressed: () async {
            await Clipboard.setData(ClipboardData(text: result.token));
            if (context.mounted) {
              ScaffoldMessenger.of(
                context,
              ).showSnackBar(SnackBar(content: Text(l10n.tokenCopied)));
            }
          },
          icon: const Icon(Icons.copy),
          label: Text(l10n.copyToken),
        ),
        FilledButton(
          autofocus: true,
          onPressed: () => Navigator.pop(context),
          child: Text(l10n.finishAction),
        ),
      ],
    );
  }
}
