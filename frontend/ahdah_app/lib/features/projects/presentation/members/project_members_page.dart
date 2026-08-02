import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/localization/app_date_time_formatter.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../core/localization/value_labels.dart';
import '../../../../core/widgets/form_widgets.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../controllers/project_controllers.dart';

final class ProjectMembersPage extends ConsumerStatefulWidget {
  const ProjectMembersPage({required this.projectId, super.key});

  final String projectId;

  @override
  ConsumerState<ProjectMembersPage> createState() => _ProjectMembersPageState();
}

class _ProjectMembersPageState extends ConsumerState<ProjectMembersPage> {
  final _scroll = ScrollController();

  @override
  void initState() {
    super.initState();
    _scroll.addListener(() {
      if (_scroll.position.extentAfter < 280) {
        ref
            .read(projectMembersControllerProvider(widget.projectId).notifier)
            .loadMore();
      }
    });
    WidgetsBinding.instance.addPostFrameCallback((_) {
      ref
          .read(projectMembersControllerProvider(widget.projectId).notifier)
          .load();
    });
  }

  @override
  void dispose() {
    _scroll.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(projectMembersControllerProvider(widget.projectId));
    return Padding(
      padding: const EdgeInsets.all(24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          AppMessageBanner(message: l10n.activeSupervisionAssignmentsBody),
          const SizedBox(height: 16),
          Expanded(child: _body(l10n, state)),
        ],
      ),
    );
  }

  Widget _body(AppLocalizations l10n, ProjectMembersState state) {
    if (state.phase == ProjectMembersPhase.initial ||
        state.phase == ProjectMembersPhase.loading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (state.phase == ProjectMembersPhase.failure) {
      return AccessFailureState(
        message: localizedError(l10n, state.error!),
        onRetry: () => ref
            .read(projectMembersControllerProvider(widget.projectId).notifier)
            .load(),
      );
    }
    if (state.items.isEmpty) {
      return AccessEmptyState(
        title: l10n.projectMembersTitle,
        body: l10n.noSupervisor,
      );
    }
    return ListView.separated(
      controller: _scroll,
      itemCount: state.items.length + 1,
      separatorBuilder: (_, _) => const SizedBox(height: 12),
      itemBuilder: (context, index) {
        if (index == state.items.length) {
          if (state.phase == ProjectMembersPhase.loadingMore) {
            return const Center(child: CircularProgressIndicator());
          }
          if (state.loadMoreError != null) {
            return TextButton.icon(
              onPressed: () => ref
                  .read(
                    projectMembersControllerProvider(widget.projectId).notifier,
                  )
                  .loadMore(),
              icon: const Icon(Icons.refresh),
              label: Text(l10n.retryLoadingMore),
            );
          }
          return const SizedBox.shrink();
        }
        final member = state.items[index];
        return Card(
          child: Padding(
            padding: const EdgeInsets.all(18),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  member.fullName,
                  style: Theme.of(context).textTheme.titleMedium,
                ),
                AccessDetail(
                  label: l10n.role,
                  value: ValueLabels.role(l10n, member.role),
                ),
                AccessDetail(
                  label: l10n.accountStatus,
                  value: ValueLabels.userStatus(l10n, member.status),
                ),
                AccessDetail(
                  label: l10n.phoneNumber,
                  value: member.phoneNumber,
                ),
                if (member.email != null)
                  AccessDetail(label: l10n.email, value: member.email!),
                AccessDetail(
                  label: l10n.assignedAt,
                  value: AppDateTimeFormatter.format(
                    context,
                    member.assignedAtUtc,
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}
