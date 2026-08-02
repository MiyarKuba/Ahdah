import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/routing/app_routes.dart';
import '../../../../core/localization/app_date_time_formatter.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../core/localization/value_labels.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/domain/role_capabilities.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../../../session/presentation/session_controller.dart';
import '../../domain/project_models.dart';
import '../controllers/project_controllers.dart';
import '../widgets/project_widgets.dart';

final class ProjectDetailsPage extends ConsumerStatefulWidget {
  const ProjectDetailsPage({required this.projectId, super.key});

  final String projectId;

  @override
  ConsumerState<ProjectDetailsPage> createState() => _ProjectDetailsPageState();
}

class _ProjectDetailsPageState extends ConsumerState<ProjectDetailsPage> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      ref
          .read(projectDetailsControllerProvider(widget.projectId).notifier)
          .load();
    });
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(projectDetailsControllerProvider(widget.projectId));
    if (state.phase == ProjectDetailsPhase.initial ||
        state.phase == ProjectDetailsPhase.loading && state.project == null) {
      return const Center(child: CircularProgressIndicator());
    }
    if (state.phase == ProjectDetailsPhase.failure && state.project == null) {
      return AccessFailureState(
        message: localizedError(l10n, state.error!),
        onRetry: () => ref
            .read(projectDetailsControllerProvider(widget.projectId).notifier)
            .load(),
      );
    }
    final project = state.project!;
    final capabilities = RoleCapabilities.forRole(
      ref.watch(sessionControllerProvider).current!.role,
    );
    return RefreshIndicator(
      onRefresh: () => ref
          .read(projectDetailsControllerProvider(widget.projectId).notifier)
          .load(),
      child: SingleChildScrollView(
        physics: const AlwaysScrollableScrollPhysics(),
        padding: const EdgeInsets.all(24),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Wrap(
              alignment: WrapAlignment.spaceBetween,
              crossAxisAlignment: WrapCrossAlignment.center,
              spacing: 12,
              runSpacing: 12,
              children: [
                Text(
                  project.projectName,
                  style: Theme.of(context).textTheme.headlineSmall,
                ),
                Wrap(
                  spacing: 8,
                  runSpacing: 8,
                  children: [
                    if (capabilities.canViewProjectMembers)
                      OutlinedButton.icon(
                        onPressed: () => context.goNamed(
                          AppRoutes.projectMembers,
                          pathParameters: {'projectId': project.id},
                        ),
                        icon: const Icon(Icons.supervisor_account_outlined),
                        label: Text(l10n.projectMembersTitle),
                      ),
                    if (capabilities.canManageProjects) ...[
                      OutlinedButton.icon(
                        key: const Key('edit-project-action'),
                        onPressed: () => context.goNamed(
                          AppRoutes.projectEdit,
                          pathParameters: {'projectId': project.id},
                        ),
                        icon: const Icon(Icons.edit_outlined),
                        label: Text(l10n.editProject),
                      ),
                      FilledButton.icon(
                        key: const Key('assign-supervisor-action'),
                        onPressed: () => context.goNamed(
                          AppRoutes.projectSupervisor,
                          pathParameters: {'projectId': project.id},
                        ),
                        icon: const Icon(Icons.person_add_alt_1),
                        label: Text(
                          project.assignedSupervisors.isEmpty
                              ? l10n.assignSupervisor
                              : l10n.replaceSupervisor,
                        ),
                      ),
                    ],
                  ],
                ),
              ],
            ),
            if (state.phase == ProjectDetailsPhase.loading)
              const LinearProgressIndicator(),
            const SizedBox(height: 16),
            _ProjectOverview(
              project: project,
              showContractValue: capabilities.canViewContractValue,
            ),
            const SizedBox(height: 16),
            _OwnerCard(owner: project.owner),
            const SizedBox(height: 16),
            _SupervisorsCard(supervisors: project.assignedSupervisors),
          ],
        ),
      ),
    );
  }
}

final class _ProjectOverview extends StatelessWidget {
  const _ProjectOverview({
    required this.project,
    required this.showContractValue,
  });

  final ProjectDetails project;
  final bool showContractValue;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(22),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            ProjectStatusChip(status: project.status),
            AccessDetail(label: l10n.siteAddress, value: project.siteAddress),
            if (project.contactPhoneNumber != null)
              AccessDetail(
                label: l10n.phoneNumber,
                value: project.contactPhoneNumber!,
              ),
            AccessDetail(
              label: l10n.contractDate,
              value: AppDateTimeFormatter.formatDate(
                context,
                project.contractDate,
              ),
            ),
            AccessDetail(
              label: l10n.startDate,
              value: AppDateTimeFormatter.formatDate(
                context,
                project.startDate,
              ),
            ),
            if (project.expectedEndDate != null)
              AccessDetail(
                label: l10n.expectedEndDate,
                value: AppDateTimeFormatter.formatDate(
                  context,
                  project.expectedEndDate,
                ),
              ),
            if (project.actualEndDate != null)
              AccessDetail(
                label: l10n.actualEndDate,
                value: AppDateTimeFormatter.formatDate(
                  context,
                  project.actualEndDate,
                ),
              ),
            if (showContractValue && project.contractValue != null)
              AccessDetail(
                key: const Key('project-contract-value'),
                label: l10n.contractValueLyd,
                value: formatLyd(project.contractValue!),
              ),
            if (project.description != null)
              AccessDetail(
                label: l10n.descriptionOptional,
                value: project.description!,
              ),
            if (project.notes != null)
              AccessDetail(label: l10n.notesOptional, value: project.notes!),
            AccessDetail(
              label: l10n.createdAt,
              value: AppDateTimeFormatter.format(context, project.createdAtUtc),
            ),
            AccessDetail(
              label: l10n.updatedAt,
              value: AppDateTimeFormatter.format(context, project.updatedAtUtc),
            ),
          ],
        ),
      ),
    );
  }
}

final class _OwnerCard extends StatelessWidget {
  const _OwnerCard({required this.owner});

  final ProjectOwnerSummary owner;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(22),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              l10n.ownerClient,
              style: Theme.of(context).textTheme.titleMedium,
            ),
            AccessDetail(label: l10n.ownerName, value: owner.ownerName),
            AccessDetail(label: l10n.phoneNumber, value: owner.phoneNumber),
            if (owner.email != null)
              AccessDetail(label: l10n.email, value: owner.email!),
            if (owner.address != null)
              AccessDetail(label: l10n.addressOptional, value: owner.address!),
          ],
        ),
      ),
    );
  }
}

final class _SupervisorsCard extends StatelessWidget {
  const _SupervisorsCard({required this.supervisors});

  final List<ProjectSupervisorSummary> supervisors;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(22),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              l10n.assignedSupervisors,
              style: Theme.of(context).textTheme.titleMedium,
            ),
            if (supervisors.isEmpty)
              Padding(
                padding: const EdgeInsets.only(top: 10),
                child: Text(l10n.noSupervisor),
              )
            else
              for (final supervisor in supervisors)
                ListTile(
                  contentPadding: EdgeInsets.zero,
                  leading: const Icon(Icons.engineering_outlined),
                  title: Text(supervisor.fullName),
                  subtitle: Text(
                    '${ValueLabels.userStatus(l10n, supervisor.status)} · '
                    '${AppDateTimeFormatter.format(context, supervisor.assignedAtUtc)}',
                  ),
                ),
          ],
        ),
      ),
    );
  }
}
