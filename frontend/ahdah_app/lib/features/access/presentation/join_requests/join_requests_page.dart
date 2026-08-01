import 'package:flutter/material.dart';
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

final class JoinRequestsPage extends ConsumerStatefulWidget {
  const JoinRequestsPage({super.key});

  @override
  ConsumerState<JoinRequestsPage> createState() => _JoinRequestsPageState();
}

class _JoinRequestsPageState extends ConsumerState<JoinRequestsPage> {
  final _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    _scrollController.addListener(_onScroll);
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (ref.read(joinRequestListControllerProvider).phase ==
          AccessListPhase.initial) {
        ref.read(joinRequestListControllerProvider.notifier).load();
      }
    });
  }

  void _onScroll() {
    if (_scrollController.position.extentAfter < 280) {
      ref.read(joinRequestListControllerProvider.notifier).loadMore();
    }
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  Future<void> _approve(JoinRequest request) async {
    final decision = await showDialog<JoinRequestDecision>(
      context: context,
      barrierDismissible: false,
      builder: (_) => _ApprovalDialog(request: request),
    );
    if (!mounted || decision == null) return;
    final l10n = AppLocalizations.of(context);
    final message = decision.outcome == 'ApprovedPendingIdentityVerification'
        ? l10n.approvedPendingIdentity
        : l10n.approvedAndActivated;
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(SnackBar(content: Text(message)));
    await ref.read(joinRequestListControllerProvider.notifier).refresh();
  }

  Future<void> _reject(JoinRequest request) async {
    final decision = await showDialog<JoinRequestDecision>(
      context: context,
      barrierDismissible: false,
      builder: (_) => _RejectionDialog(request: request),
    );
    if (!mounted || decision == null) return;
    final l10n = AppLocalizations.of(context);
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(SnackBar(content: Text(l10n.joinRequestRejected)));
    await ref.read(joinRequestListControllerProvider.notifier).refresh();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(joinRequestListControllerProvider);
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
                      'Approved',
                      'Rejected',
                      'Cancelled',
                    ])
                      DropdownMenuItem(
                        value: status,
                        child: Text(
                          ValueLabels.joinRequestStatus(l10n, status),
                        ),
                      ),
                  ],
                  onChanged: (value) => ref
                      .read(joinRequestListControllerProvider.notifier)
                      .setFilter(value == null || value.isEmpty ? null : value),
                ),
              ),
              IconButton(
                tooltip: l10n.refresh,
                onPressed: () => ref
                    .read(joinRequestListControllerProvider.notifier)
                    .refresh(),
                icon: const Icon(Icons.refresh),
              ),
            ],
          ),
          const SizedBox(height: 12),
          Expanded(child: _buildBody(l10n, state)),
        ],
      ),
    );
  }

  Widget _buildBody(AppLocalizations l10n, AccessListState<JoinRequest> state) {
    if (state.phase == AccessListPhase.loading ||
        state.phase == AccessListPhase.initial) {
      return const Center(child: CircularProgressIndicator());
    }
    if (state.phase == AccessListPhase.failure && state.items.isEmpty) {
      return AccessFailureState(
        message: localizedError(l10n, state.error!),
        onRetry: () =>
            ref.read(joinRequestListControllerProvider.notifier).load(),
      );
    }
    if (state.items.isEmpty) {
      return AccessEmptyState(
        title: l10n.noJoinRequestsTitle,
        body: l10n.noJoinRequestsBody,
      );
    }
    return RefreshIndicator(
      onRefresh: () =>
          ref.read(joinRequestListControllerProvider.notifier).refresh(),
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
                    .read(joinRequestListControllerProvider.notifier)
                    .loadMore(),
                icon: const Icon(Icons.refresh),
                label: Text(l10n.retryLoadingMore),
              );
            }
            return const SizedBox(height: 20);
          }
          return _JoinRequestCard(
            request: state.items[index],
            onApprove: _approve,
            onReject: _reject,
          );
        },
      ),
    );
  }
}

final class _JoinRequestCard extends StatelessWidget {
  const _JoinRequestCard({
    required this.request,
    required this.onApprove,
    required this.onReject,
  });

  final JoinRequest request;
  final ValueChanged<JoinRequest> onApprove;
  final ValueChanged<JoinRequest> onReject;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(18),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    request.fullName,
                    style: Theme.of(context).textTheme.titleMedium,
                  ),
                ),
                AccessStatusChip(
                  status: request.status,
                  kind: AccessStatusKind.joinRequest,
                ),
              ],
            ),
            AccessDetail(label: l10n.phoneNumber, value: request.phoneNumber),
            if (request.email != null)
              AccessDetail(label: l10n.email, value: request.email!),
            AccessDetail(
              label: l10n.requestedRole,
              value: ValueLabels.role(l10n, request.requestedRole),
            ),
            if (request.assignedRole != null)
              AccessDetail(
                label: l10n.assignedRole,
                value: ValueLabels.role(l10n, request.assignedRole!),
              ),
            AccessDetail(
              label: l10n.requestedAt,
              value: AppDateTimeFormatter.format(
                context,
                request.requestedAtUtc,
              ),
            ),
            if (request.requestMessage != null)
              AccessDetail(
                label: l10n.requestMessage,
                value: request.requestMessage!,
              ),
            if (request.reviewNotes != null)
              AccessDetail(
                label: l10n.reviewNotes,
                value: request.reviewNotes!,
              ),
            if (request.rejectionReason != null)
              AccessDetail(
                label: l10n.rejectionReason,
                value: request.rejectionReason!,
              ),
            if (request.reviewedAtUtc != null)
              AccessDetail(
                label: l10n.reviewedAt,
                value: AppDateTimeFormatter.format(
                  context,
                  request.reviewedAtUtc,
                ),
              ),
            if (request.isPending) ...[
              const SizedBox(height: 14),
              Wrap(
                spacing: 8,
                runSpacing: 8,
                children: [
                  FilledButton.icon(
                    onPressed: () => onApprove(request),
                    icon: const Icon(Icons.check),
                    label: Text(l10n.approve),
                  ),
                  OutlinedButton.icon(
                    onPressed: () => onReject(request),
                    icon: const Icon(Icons.close),
                    label: Text(l10n.reject),
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

final class _ApprovalDialog extends ConsumerStatefulWidget {
  const _ApprovalDialog({required this.request});
  final JoinRequest request;

  @override
  ConsumerState<_ApprovalDialog> createState() => _ApprovalDialogState();
}

class _ApprovalDialogState extends ConsumerState<_ApprovalDialog> {
  final _formKey = GlobalKey<FormState>();
  final _notes = TextEditingController();
  late String _role;

  @override
  void initState() {
    super.initState();
    _role = ValueLabels.assignableRoles.contains(widget.request.requestedRole)
        ? widget.request.requestedRole
        : 'Worker';
  }

  @override
  void dispose() {
    _notes.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    final l10n = AppLocalizations.of(context);
    final confirmed = await showAccessConfirmation(
      context,
      title: l10n.approveJoinRequestTitle,
      body: l10n.approveJoinRequestBody,
      confirmLabel: l10n.approve,
    );
    if (!confirmed || !mounted) return;
    final result = await ref
        .read(joinApprovalControllerProvider.notifier)
        .approve(
          widget.request.id,
          ApproveJoinRequestInput(
            assignedRole: _role,
            reviewNotes: _notes.text,
          ),
        );
    if (mounted && result != null) Navigator.pop(context, result);
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(joinApprovalControllerProvider);
    final sensitive = _role == 'Deputy' || _role == 'Accountant';
    return AlertDialog(
      title: Text(l10n.approveJoinRequestTitle),
      content: SizedBox(
        width: 480,
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              AccessDetail(
                label: l10n.requestedRole,
                value: ValueLabels.role(l10n, widget.request.requestedRole),
              ),
              const SizedBox(height: 14),
              DropdownButtonFormField<String>(
                isExpanded: true,
                key: const Key('approval-role-field'),
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
              const SizedBox(height: 10),
              Text(
                sensitive
                    ? l10n.sensitiveRoleApprovalNotice
                    : l10n.standardRoleApprovalNotice,
              ),
              const SizedBox(height: 14),
              TextFormField(
                controller: _notes,
                enabled: !state.isSubmitting,
                minLines: 2,
                maxLines: 4,
                decoration: InputDecoration(
                  labelText: l10n.reviewNotesOptional,
                ),
                validator: (value) =>
                    Validators.message(value, l10n.invalidMessage),
              ),
              if (state.error != null) ...[
                const SizedBox(height: 14),
                AppMessageBanner(
                  message: localizedError(l10n, state.error!),
                  isError: true,
                ),
              ],
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
          key: const Key('approval-submit'),
          onPressed: state.isSubmitting ? null : _submit,
          child: state.isSubmitting
              ? const SizedBox.square(
                  dimension: 20,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : Text(l10n.approve),
        ),
      ],
    );
  }
}

final class _RejectionDialog extends ConsumerStatefulWidget {
  const _RejectionDialog({required this.request});
  final JoinRequest request;

  @override
  ConsumerState<_RejectionDialog> createState() => _RejectionDialogState();
}

class _RejectionDialogState extends ConsumerState<_RejectionDialog> {
  final _formKey = GlobalKey<FormState>();
  final _reason = TextEditingController();
  final _notes = TextEditingController();

  @override
  void dispose() {
    _reason.dispose();
    _notes.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    final l10n = AppLocalizations.of(context);
    final confirmed = await showAccessConfirmation(
      context,
      title: l10n.rejectJoinRequestTitle,
      body: l10n.rejectJoinRequestBody,
      confirmLabel: l10n.reject,
    );
    if (!confirmed || !mounted) return;
    final result = await ref
        .read(joinRejectionControllerProvider.notifier)
        .reject(
          widget.request.id,
          RejectJoinRequestInput(
            reason: _reason.text,
            reviewNotes: _notes.text,
          ),
        );
    if (mounted && result != null) Navigator.pop(context, result);
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(joinRejectionControllerProvider);
    return AlertDialog(
      title: Text(l10n.rejectJoinRequestTitle),
      content: SizedBox(
        width: 480,
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text(l10n.rejectionLoginNotice),
              const SizedBox(height: 14),
              TextFormField(
                key: const Key('rejection-reason-field'),
                controller: _reason,
                enabled: !state.isSubmitting,
                autofocus: true,
                minLines: 2,
                maxLines: 4,
                decoration: InputDecoration(labelText: l10n.rejectionReason),
                validator: (value) => Validators.rejectionReason(
                  value,
                  l10n.requiredField,
                  l10n.invalidRejectionReason,
                ),
              ),
              const SizedBox(height: 14),
              TextFormField(
                controller: _notes,
                enabled: !state.isSubmitting,
                minLines: 2,
                maxLines: 4,
                decoration: InputDecoration(
                  labelText: l10n.reviewNotesOptional,
                ),
                validator: (value) =>
                    Validators.message(value, l10n.invalidMessage),
              ),
              if (state.error != null) ...[
                const SizedBox(height: 14),
                AppMessageBanner(
                  message: localizedError(l10n, state.error!),
                  isError: true,
                ),
              ],
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
          key: const Key('rejection-submit'),
          onPressed: state.isSubmitting ? null : _submit,
          child: state.isSubmitting
              ? const SizedBox.square(
                  dimension: 20,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : Text(l10n.reject),
        ),
      ],
    );
  }
}
