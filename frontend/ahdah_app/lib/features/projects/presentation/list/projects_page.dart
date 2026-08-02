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
import '../../domain/project_list_state.dart';
import '../../domain/project_models.dart';
import '../controllers/project_controllers.dart';
import '../widgets/project_widgets.dart';

final class ProjectsPage extends ConsumerStatefulWidget {
  const ProjectsPage({super.key});

  @override
  ConsumerState<ProjectsPage> createState() => _ProjectsPageState();
}

class _ProjectsPageState extends ConsumerState<ProjectsPage> {
  final _scroll = ScrollController();
  final _search = TextEditingController();

  @override
  void initState() {
    super.initState();
    _scroll.addListener(_onScroll);
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (ref.read(projectListControllerProvider).phase ==
          ProjectListPhase.initial) {
        ref.read(projectListControllerProvider.notifier).load();
      }
    });
  }

  void _onScroll() {
    if (_scroll.position.extentAfter < 280) {
      ref.read(projectListControllerProvider.notifier).loadMore();
    }
  }

  @override
  void dispose() {
    _scroll.dispose();
    _search.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(projectListControllerProvider);
    final role = ref.watch(sessionControllerProvider).current!.role;
    final capabilities = RoleCapabilities.forRole(role);
    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 16, 20, 0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          _ProjectFilters(
            search: _search,
            state: state,
            canCreate: capabilities.canManageProjects,
          ),
          const SizedBox(height: 12),
          Expanded(child: _body(l10n, state, capabilities)),
        ],
      ),
    );
  }

  Widget _body(
    AppLocalizations l10n,
    ProjectListState state,
    RoleCapabilities capabilities,
  ) {
    if (state.phase == ProjectListPhase.initial ||
        state.phase == ProjectListPhase.loading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (state.phase == ProjectListPhase.failure && state.items.isEmpty) {
      return AccessFailureState(
        message: localizedError(l10n, state.error!),
        onRetry: () => ref.read(projectListControllerProvider.notifier).load(),
      );
    }
    if (state.items.isEmpty) {
      return AccessEmptyState(
        title: l10n.noProjectsTitle,
        body: state.search.trim().length == 1
            ? l10n.searchMinimum
            : capabilities.requiresAssignedProjects
            ? l10n.noAssignedProjectsBody
            : l10n.noProjectsBody,
      );
    }
    return RefreshIndicator(
      onRefresh: () =>
          ref.read(projectListControllerProvider.notifier).refresh(),
      child: LayoutBuilder(
        builder: (context, constraints) {
          final wide = constraints.maxWidth >= 900;
          return ListView.separated(
            controller: _scroll,
            physics: const AlwaysScrollableScrollPhysics(),
            itemCount: state.items.length + 1,
            separatorBuilder: (_, _) => const SizedBox(height: 12),
            itemBuilder: (context, index) {
              if (index == state.items.length) {
                if (state.phase == ProjectListPhase.loadingMore) {
                  return const Padding(
                    padding: EdgeInsets.all(20),
                    child: Center(child: CircularProgressIndicator()),
                  );
                }
                if (state.loadMoreError != null) {
                  return TextButton.icon(
                    onPressed: () => ref
                        .read(projectListControllerProvider.notifier)
                        .loadMore(),
                    icon: const Icon(Icons.refresh),
                    label: Text(l10n.retryLoadingMore),
                  );
                }
                return const SizedBox(height: 20);
              }
              return _ProjectCard(
                project: state.items[index],
                showContractValue: capabilities.canViewContractValue,
                wide: wide,
              );
            },
          );
        },
      ),
    );
  }
}

final class _ProjectFilters extends ConsumerWidget {
  const _ProjectFilters({
    required this.search,
    required this.state,
    required this.canCreate,
  });

  final TextEditingController search;
  final ProjectListState state;
  final bool canCreate;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final controller = ref.read(projectListControllerProvider.notifier);
    return Wrap(
      spacing: 12,
      runSpacing: 12,
      crossAxisAlignment: WrapCrossAlignment.center,
      children: [
        SizedBox(
          width: 330,
          child: TextField(
            key: const Key('project-search-field'),
            controller: search,
            maxLength: 100,
            decoration: InputDecoration(
              labelText: l10n.searchProjects,
              counterText: '',
              prefixIcon: const Icon(Icons.search),
            ),
            onChanged: controller.setSearch,
          ),
        ),
        SizedBox(
          width: 220,
          child: DropdownButtonFormField<String>(
            isExpanded: true,
            initialValue: state.status ?? '',
            decoration: InputDecoration(labelText: l10n.statusFilter),
            items: [
              DropdownMenuItem(value: '', child: Text(l10n.allStatuses)),
              for (final status in const [
                'Active',
                'Paused',
                'Completed',
                'FinanciallyClosed',
                'Cancelled',
              ])
                DropdownMenuItem(
                  value: status,
                  child: Text(ValueLabels.projectStatus(l10n, status)),
                ),
            ],
            onChanged: (value) => controller.setStatus(
              value == null || value.isEmpty ? null : value,
            ),
          ),
        ),
        IconButton(
          tooltip: l10n.refresh,
          onPressed: controller.refresh,
          icon: const Icon(Icons.refresh),
        ),
        if (canCreate)
          FilledButton.icon(
            key: const Key('create-project-action'),
            onPressed: () => context.goNamed(AppRoutes.projectCreate),
            icon: const Icon(Icons.add),
            label: Text(l10n.createProject),
          ),
      ],
    );
  }
}

final class _ProjectCard extends StatelessWidget {
  const _ProjectCard({
    required this.project,
    required this.showContractValue,
    required this.wide,
  });

  final ProjectDetails project;
  final bool showContractValue;
  final bool wide;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final supervisorNames = project.assignedSupervisors
        .map((item) => item.fullName)
        .where((name) => name.isNotEmpty)
        .join(', ');
    return Card(
      child: InkWell(
        borderRadius: BorderRadius.circular(12),
        onTap: () => context.goNamed(
          AppRoutes.projectDetails,
          pathParameters: {'projectId': project.id},
        ),
        child: Padding(
          padding: const EdgeInsets.all(18),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(
                      project.projectName,
                      style: Theme.of(context).textTheme.titleLarge,
                    ),
                  ),
                  ProjectStatusChip(status: project.status),
                ],
              ),
              const SizedBox(height: 8),
              Wrap(
                spacing: wide ? 36 : 8,
                runSpacing: 4,
                children: [
                  AccessDetail(
                    label: l10n.ownerClient,
                    value: project.owner.ownerName,
                  ),
                  AccessDetail(
                    label: l10n.siteAddress,
                    value: project.siteAddress,
                  ),
                  AccessDetail(
                    label: l10n.startDate,
                    value: AppDateTimeFormatter.formatDate(
                      context,
                      project.startDate,
                    ),
                  ),
                  AccessDetail(
                    label: l10n.assignedSupervisor,
                    value: supervisorNames.isEmpty
                        ? l10n.noSupervisor
                        : supervisorNames,
                  ),
                  if (showContractValue && project.contractValue != null)
                    AccessDetail(
                      key: const Key('project-contract-value'),
                      label: l10n.contractValue,
                      value: formatLyd(project.contractValue!),
                    ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
