import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/routing/app_routes.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../core/localization/value_labels.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/domain/role_capabilities.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../../../session/presentation/session_controller.dart';
import '../../domain/advance_list_state.dart';
import '../controllers/advance_controllers.dart';
import '../widgets/advance_widgets.dart';

final class AdvancesPage extends ConsumerStatefulWidget {
  const AdvancesPage({super.key});

  @override
  ConsumerState<AdvancesPage> createState() => _AdvancesPageState();
}

class _AdvancesPageState extends ConsumerState<AdvancesPage> {
  final _reference = TextEditingController();
  final _scroll = ScrollController();

  @override
  void initState() {
    super.initState();
    Future.microtask(() {
      ref.read(advanceListControllerProvider.notifier).load();
      final role = ref.read(sessionControllerProvider).current?.role;
      if (role == 'Manager' || role == 'Deputy') {
        ref.read(advanceMemberDirectoryControllerProvider.notifier).load();
      }
    });
    _scroll.addListener(_onScroll);
  }

  void _onScroll() {
    if (_scroll.position.extentAfter < 320) {
      ref.read(advanceListControllerProvider.notifier).loadMore();
    }
  }

  @override
  void dispose() {
    _reference.dispose();
    _scroll.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final list = ref.watch(advanceListControllerProvider);
    final session = ref.watch(sessionControllerProvider).current!;
    final capabilities = RoleCapabilities.forRole(session.role);
    final directory = ref.watch(advanceMemberDirectoryControllerProvider);
    final participantOnly =
        session.role == 'Supervisor' || session.role == 'Worker';

    return Scaffold(
      floatingActionButton: capabilities.canCreateTopLevelAdvance
          ? FloatingActionButton.extended(
              key: const Key('create-advance-action'),
              onPressed: () => context.go(AppRoutes.advanceCreatePath),
              icon: const Icon(Icons.add),
              label: Text(l10n.createAdvance),
            )
          : null,
      body: RefreshIndicator(
        onRefresh: ref.read(advanceListControllerProvider.notifier).refresh,
        child: CustomScrollView(
          controller: _scroll,
          physics: const AlwaysScrollableScrollPhysics(),
          slivers: [
            SliverToBoxAdapter(
              child: Padding(
                padding: const EdgeInsets.fromLTRB(16, 16, 16, 8),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        Expanded(
                          child: Text(
                            l10n.advancesTitle,
                            style: Theme.of(context).textTheme.headlineMedium,
                          ),
                        ),
                        IconButton(
                          tooltip: l10n.refresh,
                          onPressed: ref
                              .read(advanceListControllerProvider.notifier)
                              .refresh,
                          icon: const Icon(Icons.refresh),
                        ),
                        IconButton(
                          tooltip: l10n.personalBalance,
                          onPressed: () =>
                              context.go(AppRoutes.advanceBalancesPath),
                          icon: const Icon(
                            Icons.account_balance_wallet_outlined,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 12),
                    TextField(
                      controller: _reference,
                      maxLength: 50,
                      onChanged: ref
                          .read(advanceListControllerProvider.notifier)
                          .setReference,
                      decoration: InputDecoration(
                        labelText: l10n.searchAdvanceReference,
                        prefixIcon: const Icon(Icons.search),
                        counterText: '',
                      ),
                    ),
                    const SizedBox(height: 12),
                    Wrap(
                      spacing: 12,
                      runSpacing: 12,
                      children: [
                        SizedBox(
                          width: 260,
                          child: DropdownButtonFormField<String?>(
                            initialValue: list.status,
                            isExpanded: true,
                            decoration: InputDecoration(
                              labelText: l10n.statusFilter,
                            ),
                            items: [
                              DropdownMenuItem<String?>(
                                value: null,
                                child: Text(l10n.allStatuses),
                              ),
                              for (final status in knownAdvanceStatuses)
                                DropdownMenuItem<String?>(
                                  value: status,
                                  child: Text(
                                    ValueLabels.advanceStatus(l10n, status),
                                  ),
                                ),
                            ],
                            onChanged: ref
                                .read(advanceListControllerProvider.notifier)
                                .setStatus,
                          ),
                        ),
                        if (capabilities.canSelectAuthorizedBalanceUser &&
                            directory.items.isNotEmpty)
                          SizedBox(
                            width: 280,
                            child: DropdownButtonFormField<String?>(
                              initialValue: list.userId,
                              isExpanded: true,
                              decoration: InputDecoration(
                                labelText: l10n.selectUser,
                              ),
                              items: [
                                DropdownMenuItem<String?>(
                                  value: null,
                                  child: Text(l10n.allRoles),
                                ),
                                for (final member in directory.items)
                                  DropdownMenuItem<String?>(
                                    value: member.id,
                                    child: Text(member.fullName),
                                  ),
                              ],
                              onChanged: ref
                                  .read(advanceListControllerProvider.notifier)
                                  .setUser,
                            ),
                          ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
            if (list.phase == AdvanceListPhase.loading ||
                list.phase == AdvanceListPhase.initial)
              const SliverFillRemaining(
                child: Center(child: CircularProgressIndicator()),
              )
            else if (list.phase == AdvanceListPhase.failure)
              SliverFillRemaining(
                child: AccessFailureState(
                  message: localizedError(l10n, list.error!),
                  onRetry: ref
                      .read(advanceListControllerProvider.notifier)
                      .load,
                ),
              )
            else if (list.items.isEmpty)
              SliverFillRemaining(
                child: AccessEmptyState(
                  title: l10n.noAdvancesTitle,
                  body: participantOnly
                      ? l10n.participantAdvancesBody
                      : l10n.noAdvancesBody,
                ),
              )
            else
              SliverPadding(
                padding: const EdgeInsets.fromLTRB(16, 8, 16, 24),
                sliver: SliverList.separated(
                  itemCount: list.items.length,
                  separatorBuilder: (_, _) => const SizedBox(height: 8),
                  itemBuilder: (context, index) {
                    final advance = list.items[index];
                    return AdvanceSummaryCard(
                      advance: advance,
                      onOpen: () => context.go(
                        '${AppRoutes.advancesPath}/${advance.advanceId}',
                      ),
                    );
                  },
                ),
              ),
            if (list.phase == AdvanceListPhase.loadingMore)
              const SliverToBoxAdapter(
                child: Padding(
                  padding: EdgeInsets.all(16),
                  child: Center(child: CircularProgressIndicator()),
                ),
              ),
            if (list.loadMoreError != null)
              SliverToBoxAdapter(
                child: Center(
                  child: TextButton.icon(
                    onPressed: ref
                        .read(advanceListControllerProvider.notifier)
                        .loadMore,
                    icon: const Icon(Icons.refresh),
                    label: Text(l10n.retryLoadingMore),
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }
}
