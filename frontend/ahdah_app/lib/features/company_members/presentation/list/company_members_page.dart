import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/routing/app_routes.dart';
import '../../../../core/localization/app_date_time_formatter.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../core/localization/value_labels.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../../domain/company_member_list_state.dart';
import '../../domain/company_member_models.dart';
import '../controllers/company_member_controllers.dart';

final class CompanyMembersPage extends ConsumerStatefulWidget {
  const CompanyMembersPage({super.key});

  @override
  ConsumerState<CompanyMembersPage> createState() => _CompanyMembersPageState();
}

class _CompanyMembersPageState extends ConsumerState<CompanyMembersPage> {
  final _scroll = ScrollController();
  final _search = TextEditingController();

  @override
  void initState() {
    super.initState();
    _scroll.addListener(_onScroll);
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (ref.read(companyMemberListControllerProvider).phase ==
          CompanyMemberListPhase.initial) {
        ref.read(companyMemberListControllerProvider.notifier).load();
      }
    });
  }

  void _onScroll() {
    if (_scroll.position.extentAfter < 280) {
      ref.read(companyMemberListControllerProvider.notifier).loadMore();
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
    final state = ref.watch(companyMemberListControllerProvider);
    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 16, 20, 0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(
            l10n.directoryReadOnly,
            style: Theme.of(context).textTheme.bodyMedium,
          ),
          const SizedBox(height: 12),
          _Filters(search: _search, state: state),
          const SizedBox(height: 12),
          Expanded(child: _body(l10n, state)),
        ],
      ),
    );
  }

  Widget _body(AppLocalizations l10n, CompanyMemberListState state) {
    if (state.phase == CompanyMemberListPhase.initial ||
        state.phase == CompanyMemberListPhase.loading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (state.phase == CompanyMemberListPhase.failure && state.items.isEmpty) {
      return AccessFailureState(
        message: localizedError(l10n, state.error!),
        onRetry: () =>
            ref.read(companyMemberListControllerProvider.notifier).load(),
      );
    }
    if (state.items.isEmpty) {
      return AccessEmptyState(
        title: l10n.noMembersTitle,
        body: state.search.trim().length == 1
            ? l10n.searchMinimum
            : l10n.noMembersBody,
      );
    }
    return RefreshIndicator(
      onRefresh: () =>
          ref.read(companyMemberListControllerProvider.notifier).refresh(),
      child: LayoutBuilder(
        builder: (context, constraints) {
          final columns = constraints.maxWidth >= 850 ? 2 : 1;
          return GridView.builder(
            controller: _scroll,
            physics: const AlwaysScrollableScrollPhysics(),
            gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
              crossAxisCount: columns,
              crossAxisSpacing: 12,
              mainAxisSpacing: 12,
              mainAxisExtent: 190,
            ),
            itemCount: state.items.length + 1,
            itemBuilder: (context, index) {
              if (index == state.items.length) {
                if (state.phase == CompanyMemberListPhase.loadingMore) {
                  return const Center(child: CircularProgressIndicator());
                }
                if (state.loadMoreError != null) {
                  return Center(
                    child: TextButton.icon(
                      onPressed: () => ref
                          .read(companyMemberListControllerProvider.notifier)
                          .loadMore(),
                      icon: const Icon(Icons.refresh),
                      label: Text(l10n.retryLoadingMore),
                    ),
                  );
                }
                return const SizedBox.shrink();
              }
              return _MemberCard(member: state.items[index]);
            },
          );
        },
      ),
    );
  }
}

final class _Filters extends ConsumerWidget {
  const _Filters({required this.search, required this.state});

  final TextEditingController search;
  final CompanyMemberListState state;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final controller = ref.read(companyMemberListControllerProvider.notifier);
    return Wrap(
      spacing: 12,
      runSpacing: 12,
      crossAxisAlignment: WrapCrossAlignment.center,
      children: [
        SizedBox(
          width: 310,
          child: TextField(
            key: const Key('member-search-field'),
            controller: search,
            maxLength: 100,
            decoration: InputDecoration(
              labelText: l10n.searchMembers,
              counterText: '',
              prefixIcon: const Icon(Icons.search),
              suffixIcon: search.text.isEmpty
                  ? null
                  : IconButton(
                      tooltip: l10n.clearAction,
                      onPressed: () {
                        search.clear();
                        controller.setSearch('');
                      },
                      icon: const Icon(Icons.clear),
                    ),
            ),
            onChanged: (value) {
              controller.setSearch(value);
              (context as Element).markNeedsBuild();
            },
          ),
        ),
        SizedBox(
          width: 190,
          child: DropdownButtonFormField<String>(
            isExpanded: true,
            initialValue: state.role ?? '',
            decoration: InputDecoration(labelText: l10n.roleFilter),
            items: [
              DropdownMenuItem(value: '', child: Text(l10n.allRoles)),
              for (final role in const [
                'Manager',
                'Deputy',
                'Accountant',
                'Supervisor',
                'Worker',
              ])
                DropdownMenuItem(
                  value: role,
                  child: Text(ValueLabels.role(l10n, role)),
                ),
            ],
            onChanged: (value) => controller.setRole(
              value == null || value.isEmpty ? null : value,
            ),
          ),
        ),
        SizedBox(
          width: 210,
          child: DropdownButtonFormField<String>(
            isExpanded: true,
            initialValue: state.status ?? '',
            decoration: InputDecoration(labelText: l10n.statusFilter),
            items: [
              DropdownMenuItem(value: '', child: Text(l10n.allStatuses)),
              for (final status in const [
                'PendingApproval',
                'Active',
                'Suspended',
                'Inactive',
                'Rejected',
              ])
                DropdownMenuItem(
                  value: status,
                  child: Text(ValueLabels.userStatus(l10n, status)),
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
      ],
    );
  }
}

final class _MemberCard extends StatelessWidget {
  const _MemberCard({required this.member});

  final CompanyMemberSummary member;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Card(
      child: InkWell(
        borderRadius: BorderRadius.circular(12),
        onTap: () => context.goNamed(
          AppRoutes.companyMemberDetails,
          pathParameters: {'memberId': member.id},
        ),
        child: Padding(
          padding: const EdgeInsets.all(18),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                member.fullName,
                style: Theme.of(context).textTheme.titleMedium,
              ),
              const SizedBox(height: 8),
              AccessDetail(
                label: l10n.role,
                value: ValueLabels.role(l10n, member.role),
              ),
              AccessDetail(
                label: l10n.accountStatus,
                value: ValueLabels.userStatus(l10n, member.status),
              ),
              AccessDetail(
                label: l10n.createdAt,
                value: AppDateTimeFormatter.format(
                  context,
                  member.createdAtUtc,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
