import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/routing/app_routes.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../access/domain/role_capabilities.dart';
import '../../../access/presentation/widgets/access_widgets.dart';
import '../../../session/presentation/session_controller.dart';
import '../controllers/advance_controllers.dart';
import '../widgets/advance_widgets.dart';

final class PersonalAdvanceBalancesPage extends ConsumerStatefulWidget {
  const PersonalAdvanceBalancesPage({super.key});

  @override
  ConsumerState<PersonalAdvanceBalancesPage> createState() =>
      _PersonalAdvanceBalancesPageState();
}

class _PersonalAdvanceBalancesPageState
    extends ConsumerState<PersonalAdvanceBalancesPage> {
  @override
  void initState() {
    super.initState();
    Future.microtask(() {
      ref.read(personalAdvanceBalanceControllerProvider.notifier).load();
      final role = ref.read(sessionControllerProvider).current?.role;
      if (role == 'Manager' || role == 'Deputy') {
        ref.read(advanceMemberDirectoryControllerProvider.notifier).load();
      }
    });
  }

  Future<void> _selectUser() async {
    final l10n = AppLocalizations.of(context);
    final members = ref.read(advanceMemberDirectoryControllerProvider).items;
    String? selected;
    final userId = await showDialog<String>(
      context: context,
      builder: (context) => StatefulBuilder(
        builder: (context, setDialogState) => AlertDialog(
          title: Text(l10n.authorizedBalanceLookup),
          content: DropdownButtonFormField<String>(
            initialValue: selected,
            isExpanded: true,
            decoration: InputDecoration(labelText: l10n.selectUser),
            items: [
              for (final member in members)
                DropdownMenuItem(
                  value: member.id,
                  child: Text(member.fullName),
                ),
            ],
            onChanged: (value) => setDialogState(() => selected = value),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: Text(l10n.cancelAction),
            ),
            FilledButton(
              onPressed: selected == null
                  ? null
                  : () => Navigator.pop(context, selected),
              child: Text(l10n.viewDetails),
            ),
          ],
        ),
      ),
    );
    if (userId != null && mounted) {
      context.go('${AppRoutes.advanceBalancesPath}/users/$userId');
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(personalAdvanceBalanceControllerProvider);
    final session = ref.watch(sessionControllerProvider).current!;
    final capabilities = RoleCapabilities.forRole(session.role);
    final directory = ref.watch(advanceMemberDirectoryControllerProvider);

    return _BalanceScaffold(
      title: l10n.personalBalance,
      state: state,
      onRefresh: ref
          .read(personalAdvanceBalanceControllerProvider.notifier)
          .refresh,
      onLoadMore: ref
          .read(personalAdvanceBalanceControllerProvider.notifier)
          .loadMore,
      header: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(l10n.pendingReservationsNotice),
          const SizedBox(height: 8),
          if (capabilities.canSelectAuthorizedBalanceUser)
            FilledButton.tonalIcon(
              onPressed: directory.items.isEmpty ? null : _selectUser,
              icon: const Icon(Icons.manage_accounts_outlined),
              label: Text(l10n.authorizedBalanceLookup),
            )
          else if (capabilities.canViewAuthorizedBalances)
            Text(l10n.accountantBalanceLookupLimitation),
        ],
      ),
    );
  }
}

final class AuthorizedUserAdvanceBalancesPage extends ConsumerStatefulWidget {
  const AuthorizedUserAdvanceBalancesPage({required this.userId, super.key});

  final String userId;

  @override
  ConsumerState<AuthorizedUserAdvanceBalancesPage> createState() =>
      _AuthorizedUserAdvanceBalancesPageState();
}

class _AuthorizedUserAdvanceBalancesPageState
    extends ConsumerState<AuthorizedUserAdvanceBalancesPage> {
  @override
  void initState() {
    super.initState();
    Future.microtask(
      () => ref
          .read(
            authorizedAdvanceBalanceControllerProvider(widget.userId).notifier,
          )
          .load(),
    );
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(
      authorizedAdvanceBalanceControllerProvider(widget.userId),
    );
    return _BalanceScaffold(
      title: AppLocalizations.of(context).userBalances,
      state: state,
      onRefresh: ref
          .read(
            authorizedAdvanceBalanceControllerProvider(widget.userId).notifier,
          )
          .refresh,
      onLoadMore: ref
          .read(
            authorizedAdvanceBalanceControllerProvider(widget.userId).notifier,
          )
          .loadMore,
    );
  }
}

final class _BalanceScaffold extends StatelessWidget {
  const _BalanceScaffold({
    required this.title,
    required this.state,
    required this.onRefresh,
    required this.onLoadMore,
    this.header,
  });

  final String title;
  final AdvanceBalancesState state;
  final Future<void> Function() onRefresh;
  final Future<void> Function() onLoadMore;
  final Widget? header;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    if (state.phase == PagedReadPhase.initial ||
        state.phase == PagedReadPhase.loading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (state.phase == PagedReadPhase.failure) {
      return AccessFailureState(
        message: localizedError(l10n, state.error!),
        onRetry: onRefresh,
      );
    }
    return Scaffold(
      body: RefreshIndicator(
        onRefresh: onRefresh,
        child: ListView(
          physics: const AlwaysScrollableScrollPhysics(),
          padding: const EdgeInsets.all(16),
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    title,
                    style: Theme.of(context).textTheme.headlineMedium,
                  ),
                ),
                IconButton(
                  tooltip: l10n.refresh,
                  onPressed: onRefresh,
                  icon: const Icon(Icons.refresh),
                ),
              ],
            ),
            if (header != null) ...[const SizedBox(height: 12), header!],
            const SizedBox(height: 12),
            if (state.items.isEmpty)
              AccessEmptyState(
                title: l10n.noBalancesTitle,
                body: l10n.noBalancesBody,
              )
            else
              for (final balance in state.items)
                AdvanceBalanceCard(
                  balance: balance,
                  onOpen: () => context.go(
                    '${AppRoutes.advancesPath}/${balance.advanceId}',
                  ),
                ),
            if (state.phase == PagedReadPhase.loadingMore)
              const Center(
                child: Padding(
                  padding: EdgeInsets.all(16),
                  child: CircularProgressIndicator(),
                ),
              ),
            if (state.hasMore && state.phase != PagedReadPhase.loadingMore)
              TextButton(
                onPressed: onLoadMore,
                child: Text(l10n.retryLoadingMore),
              ),
            if (state.loadMoreError != null)
              Text(localizedError(l10n, state.loadMoreError!)),
          ],
        ),
      ),
    );
  }
}
