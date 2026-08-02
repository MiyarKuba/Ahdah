import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/routing/app_routes.dart';
import '../../../../core/localization/app_date_time_formatter.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../core/widgets/form_widgets.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../../../company_members/domain/company_member_models.dart';
import '../../domain/project_models.dart';
import '../../domain/project_requests.dart';
import '../controllers/project_controllers.dart';
import '../widgets/project_form_widgets.dart';

final class ProjectSupervisorPage extends ConsumerStatefulWidget {
  const ProjectSupervisorPage({required this.projectId, super.key});

  final String projectId;

  @override
  ConsumerState<ProjectSupervisorPage> createState() =>
      _ProjectSupervisorPageState();
}

class _ProjectSupervisorPageState extends ConsumerState<ProjectSupervisorPage> {
  CompanyMemberSummary? _selected;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      ref
          .read(projectDetailsControllerProvider(widget.projectId).notifier)
          .load();
    });
  }

  Future<void> _submit(ProjectDetails project) async {
    final l10n = AppLocalizations.of(context);
    final selected = _selected;
    if (selected == null || !selected.isEligibleSupervisor) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(l10n.chooseSupervisor)));
      return;
    }
    if (project.assignedSupervisors.any((item) => item.userId == selected.id)) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(l10n.validationError)));
      return;
    }
    if (project.assignedSupervisors.isNotEmpty) {
      final confirmed = await showAccessConfirmation(
        context,
        title: l10n.replaceSupervisorTitle,
        body: l10n.replaceSupervisorBody,
        confirmLabel: l10n.replaceSupervisor,
      );
      if (!confirmed || !mounted) return;
    }
    final result = await ref
        .read(supervisorAssignmentControllerProvider.notifier)
        .assign(
          project.id,
          SupervisorAssignmentInput(
            supervisorUserId: selected.id,
            expectedVersion: project.versionNumber,
          ),
        );
    if (!mounted || result == null) return;
    await ref.read(projectListControllerProvider.notifier).refresh();
    ref.invalidate(projectDetailsControllerProvider(project.id));
    if (!mounted) return;
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(SnackBar(content: Text(l10n.supervisorAssigned)));
    context.goNamed(
      AppRoutes.projectDetails,
      pathParameters: {'projectId': project.id},
    );
  }

  Future<void> _reload() async {
    ref.read(supervisorAssignmentControllerProvider.notifier).reset();
    await ref
        .read(projectDetailsControllerProvider(widget.projectId).notifier)
        .load();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final details = ref.watch(
      projectDetailsControllerProvider(widget.projectId),
    );
    final submit = ref.watch(supervisorAssignmentControllerProvider);
    if (details.phase == ProjectDetailsPhase.initial ||
        details.phase == ProjectDetailsPhase.loading &&
            details.project == null) {
      return const Center(child: CircularProgressIndicator());
    }
    if (details.phase == ProjectDetailsPhase.failure &&
        details.project == null) {
      return AccessFailureState(
        message: localizedError(l10n, details.error!),
        onRetry: _reload,
      );
    }
    final project = details.project!;
    return SingleChildScrollView(
      padding: const EdgeInsets.all(24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(
            project.assignedSupervisors.isEmpty
                ? l10n.assignSupervisor
                : l10n.replaceSupervisor,
            style: Theme.of(context).textTheme.headlineSmall,
          ),
          const SizedBox(height: 16),
          if (submit.error != null) ...[
            AppMessageBanner(
              message: submit.isConflict
                  ? l10n.projectChangedConflict
                  : localizedError(l10n, submit.error!),
              isError: true,
            ),
            if (submit.isConflict)
              Align(
                alignment: AlignmentDirectional.centerStart,
                child: TextButton.icon(
                  key: const Key('reload-supervisor-project-action'),
                  onPressed: _reload,
                  icon: const Icon(Icons.refresh),
                  label: Text(l10n.reloadProject),
                ),
              ),
            const SizedBox(height: 12),
          ],
          Card(
            child: Padding(
              padding: const EdgeInsets.all(22),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Text(
                    project.projectName,
                    style: Theme.of(context).textTheme.titleLarge,
                  ),
                  const SizedBox(height: 12),
                  if (project.assignedSupervisors.isEmpty)
                    Text(l10n.noSupervisor)
                  else ...[
                    Text(
                      l10n.assignedSupervisors,
                      style: Theme.of(context).textTheme.titleMedium,
                    ),
                    for (final supervisor in project.assignedSupervisors)
                      ListTile(
                        contentPadding: EdgeInsets.zero,
                        leading: const Icon(Icons.engineering_outlined),
                        title: Text(supervisor.fullName),
                        subtitle: Text(
                          AppDateTimeFormatter.format(
                            context,
                            supervisor.assignedAtUtc,
                          ),
                        ),
                      ),
                  ],
                  const Divider(height: 32),
                  SupervisorPickerField(
                    label: l10n.chooseSupervisor,
                    selected: _selected,
                    enabled: !submit.isSubmitting,
                    onChanged: (value) => setState(() => _selected = value),
                  ),
                  const SizedBox(height: 16),
                  AppMessageBanner(message: l10n.supervisorHistoryNotice),
                ],
              ),
            ),
          ),
          const SizedBox(height: 20),
          Align(
            alignment: AlignmentDirectional.centerEnd,
            child: SubmitButton(
              label: project.assignedSupervisors.isEmpty
                  ? l10n.assignSupervisor
                  : l10n.replaceSupervisor,
              loading: submit.isSubmitting,
              onPressed: () => _submit(project),
            ),
          ),
        ],
      ),
    );
  }
}
