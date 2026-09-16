import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../app/routing/app_routes.dart';
import '../../../core/localization/app_date_time_formatter.dart';
import '../../../core/localization/error_labels.dart';
import '../../../core/localization/value_labels.dart';
import '../../../l10n/app_localizations.dart';
import '../../access/domain/role_capabilities.dart';
import '../../access/presentation/widgets/access_widgets.dart';
import '../../projects/presentation/controllers/project_controllers.dart';
import '../../session/presentation/session_controller.dart';
import '../domain/settlement_models.dart';
import 'controllers/settlement_controller.dart';
import 'settlement_labels.dart';
import 'settlement_navigation.dart';

final class ProjectSettlementPage extends ConsumerWidget {
  const ProjectSettlementPage({required this.projectId, super.key});
  final String projectId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final capabilities = RoleCapabilities.forRole(
      ref.watch(sessionControllerProvider).current?.role,
    );
    if (!capabilities.canViewProjectSettlement) {
      return Center(child: Text(l10n.permissionDenied));
    }
    final state = ref.watch(settlementControllerProvider(projectId));
    Future<void> refresh() =>
        ref.read(settlementControllerProvider(projectId).notifier).load();
    if (state.value == null) {
      if (state.error != null) {
        return AccessFailureState(
          message: localizedError(l10n, state.error!),
          onRetry: refresh,
        );
      }
      return const Center(child: CircularProgressIndicator());
    }
    final summary = state.value!;
    final project = ref
        .watch(projectDetailsControllerProvider(projectId))
        .project;
    return RefreshIndicator(
      onRefresh: refresh,
      child: Center(
        child: ConstrainedBox(
          constraints: const BoxConstraints(maxWidth: 1100),
          child: ListView(
            key: const Key('settlement-scroll'),
            physics: const AlwaysScrollableScrollPhysics(),
            padding: const EdgeInsets.all(20),
            children: [
              Wrap(
                alignment: WrapAlignment.spaceBetween,
                crossAxisAlignment: WrapCrossAlignment.center,
                spacing: 12,
                runSpacing: 8,
                children: [
                  Text(
                    l10n.settlementTitle,
                    style: Theme.of(context).textTheme.headlineSmall,
                  ),
                  Wrap(
                    spacing: 8,
                    runSpacing: 8,
                    children: [
                      TextButton.icon(
                        onPressed: () => context.goNamed(
                          AppRoutes.projectDetails,
                          pathParameters: {'projectId': projectId},
                        ),
                        icon: const Icon(Icons.arrow_back),
                        label: Text(l10n.settlementBack),
                      ),
                      OutlinedButton.icon(
                        key: const Key('settlement-refresh'),
                        onPressed: state.loading ? null : refresh,
                        icon: const Icon(Icons.refresh),
                        label: Text(l10n.settlementRefresh),
                      ),
                    ],
                  ),
                ],
              ),
              if (state.loading) const LinearProgressIndicator(),
              if (project != null)
                Text(
                  project.projectName,
                  style: Theme.of(context).textTheme.titleLarge,
                ),
              Text('${l10n.settlementProjectReference}: $projectId'),
              Text(ValueLabels.projectStatus(l10n, summary.projectStatus)),
              Text(
                '${l10n.settlementEvaluatedAt}: ${AppDateTimeFormatter.format(context, summary.evaluatedAt)}',
              ),
              Text(
                '${l10n.settlementVisibility}: ${SettlementLabels.code(l10n, summary.visibilityScope)}',
              ),
              const SizedBox(height: 16),
              LayoutBuilder(
                builder: (context, constraints) {
                  final settlement = _ReadinessCard(
                    title: l10n.settlementSettlement,
                    readiness: summary.settlementReadiness,
                    value: summary.canSettle,
                    unknown: l10n.settlementSettlementUnknown,
                    blocked: l10n.settlementSettlementBlocked,
                    impedimentsTitle: l10n.settlementSettlementImpediments,
                    impediments: summary.settlementImpediments,
                  );
                  final closure = _ReadinessCard(
                    title: l10n.settlementClosure,
                    readiness: summary.closureReadiness,
                    value: summary.canClose,
                    unknown: l10n.settlementClosureUnknown,
                    blocked: l10n.settlementClosureBlocked,
                    impedimentsTitle: l10n.settlementClosureImpediments,
                    impediments: summary.closureImpediments,
                  );
                  return constraints.maxWidth >= 760
                      ? Row(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Expanded(child: settlement),
                            const SizedBox(width: 12),
                            Expanded(child: closure),
                          ],
                        )
                      : Column(
                          crossAxisAlignment: CrossAxisAlignment.stretch,
                          children: [settlement, closure],
                        );
                },
              ),
              _Section(
                title:
                    '${l10n.settlementKnownBlockers}: ${summary.totalBlockerCount}',
                children: [
                  Text(l10n.settlementCountScope),
                  if (summary.totalBlockerCount == 0)
                    Text(l10n.settlementNoKnown),
                  Text(l10n.settlementCurrencies),
                ],
              ),
              _Section(
                title: l10n.settlementGaps,
                children: [
                  Text(l10n.settlementGapsHelp),
                  if (summary.evaluationGaps.isEmpty)
                    Text(l10n.settlementNoGaps),
                  for (final gap in summary.evaluationGaps)
                    Padding(
                      padding: const EdgeInsets.only(top: 12),
                      child: Text(
                        '${SettlementLabels.code(l10n, gap.category)}: ${SettlementLabels.code(l10n, gap.code)}',
                      ),
                    ),
                ],
              ),
              Padding(
                padding: const EdgeInsets.symmetric(vertical: 16),
                child: Text(
                  l10n.settlementCategories,
                  style: Theme.of(context).textTheme.titleLarge,
                ),
              ),
              if (summary.categories.isEmpty) Text(l10n.settlementNoCategories),
              for (final category in summary.categories)
                _CategoryCard(category: category, capabilities: capabilities),
            ],
          ),
        ),
      ),
    );
  }
}

final class _ReadinessCard extends StatelessWidget {
  const _ReadinessCard({
    required this.title,
    required this.readiness,
    required this.value,
    required this.unknown,
    required this.blocked,
    required this.impedimentsTitle,
    required this.impediments,
  });
  final String title, readiness, unknown, blocked, impedimentsTitle;
  final bool? value;
  final List<String> impediments;
  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;
    final recognized =
        (value == false && readiness == 'Blocked') ||
        (value == null && readiness == 'Indeterminate');
    return _Section(
      title: title,
      children: [
        Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Icon(
              readiness == 'Blocked'
                  ? Icons.block_outlined
                  : Icons.help_outline,
              color: readiness == 'Blocked' ? scheme.error : scheme.tertiary,
            ),
            const SizedBox(width: 8),
            Expanded(
              child: Text(
                recognized
                    ? SettlementLabels.code(l10n, readiness)
                    : l10n.settlementUnknown,
                style: Theme.of(context).textTheme.titleMedium,
              ),
            ),
          ],
        ),
        const SizedBox(height: 8),
        Text(
          value == null
              ? unknown
              : value == false
              ? blocked
              : l10n.settlementUnsupported,
        ),
        if (!recognized && value != true) Text(l10n.settlementUnsupported),
        const SizedBox(height: 16),
        Text(impedimentsTitle, style: Theme.of(context).textTheme.titleSmall),
        if (impediments.isEmpty) Text(l10n.settlementNoImpediments),
        for (final code in impediments)
          Padding(
            padding: const EdgeInsets.only(top: 8),
            child: Text(SettlementLabels.code(l10n, code)),
          ),
      ],
    );
  }
}

final class _CategoryCard extends StatelessWidget {
  const _CategoryCard({required this.category, required this.capabilities});
  final SettlementCategory category;
  final RoleCapabilities capabilities;
  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final restricted =
        capabilities.requiresAssignedProjects &&
        !const {
          'Expenses',
          'ExpenseDocuments',
          'SupplierDebt',
          'Advances',
        }.contains(category.category);
    final evaluation = restricted ? 'NotVisible' : category.evaluationStatus;
    if (evaluation != 'Evaluated') {
      return _Section(
        title: SettlementLabels.code(l10n, category.category),
        children: [
          Text(
            SettlementLabels.code(l10n, evaluation),
            style: Theme.of(context).textTheme.titleSmall,
          ),
          Text(switch (evaluation) {
            'NotVisible' => l10n.settlementHidden,
            'NotAttributable' => l10n.settlementUnattributable,
            _ => l10n.settlementUnsupported,
          }),
        ],
      );
    }
    return Card(
      child: ExpansionTile(
        key: Key('settlement-category-${category.category}'),
        title: Text(SettlementLabels.code(l10n, category.category)),
        subtitle: Text(
          '${SettlementLabels.code(l10n, evaluation)} · ${l10n.settlementKnownBlockers}: ${category.blockerCount}',
        ),
        expandedCrossAxisAlignment: CrossAxisAlignment.stretch,
        childrenPadding: const EdgeInsets.all(16),
        children: [
          if (category.blockerCount == 0)
            Text(l10n.settlementNoCategoryBlockers),
          if (category.totals.isNotEmpty) ...[
            Text(SettlementLabels.code(l10n, category.amountMeaning)),
            for (final total in category.totals)
              _Money(amount: total.amount, currency: total.currencyCode),
          ],
          for (final blocker in category.blockers)
            _BlockerRow(blocker: blocker, capabilities: capabilities),
        ],
      ),
    );
  }
}

final class _BlockerRow extends StatelessWidget {
  const _BlockerRow({required this.blocker, required this.capabilities});
  final SettlementBlocker blocker;
  final RoleCapabilities capabilities;
  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final destination = settlementDestination(blocker, capabilities);
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 12),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            SettlementLabels.code(l10n, blocker.code),
            style: Theme.of(context).textTheme.titleSmall,
          ),
          Text(
            '${SettlementLabels.code(l10n, blocker.recordType)} · ${SettlementLabels.status(l10n, blocker.status)}',
          ),
          Text('${l10n.settlementRecordReference}: ${blocker.recordId}'),
          if (blocker.amount != null)
            _Money(amount: blocker.amount!, currency: blocker.currencyCode),
          if (destination != null)
            TextButton.icon(
              onPressed: () => context.goNamed(
                destination.name,
                pathParameters: destination.parameters,
              ),
              icon: const Icon(Icons.open_in_new),
              label: Text(l10n.settlementOpenRecord),
            ),
          const Divider(),
        ],
      ),
    );
  }
}

final class _Money extends StatelessWidget {
  const _Money({required this.amount, this.currency});
  final String amount;
  final String? currency;
  @override
  Widget build(BuildContext context) {
    final code = currency != null && RegExp(r'^[A-Z]{3}$').hasMatch(currency!)
        ? currency!
        : AppLocalizations.of(context).unknownValue;
    return Text('$code $amount', textDirection: TextDirection.ltr);
  }
}

final class _Section extends StatelessWidget {
  const _Section({required this.title, required this.children});
  final String title;
  final List<Widget> children;
  @override
  Widget build(BuildContext context) => Card(
    child: Padding(
      padding: const EdgeInsets.all(20),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(title, style: Theme.of(context).textTheme.titleMedium),
          const SizedBox(height: 12),
          ...children,
        ],
      ),
    ),
  );
}
