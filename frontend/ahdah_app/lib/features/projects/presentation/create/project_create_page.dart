import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/routing/app_routes.dart';
import '../../../../core/localization/error_labels.dart';
import '../../../../core/validation/validators.dart';
import '../../../../core/widgets/form_widgets.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../company_members/domain/company_member_models.dart';
import '../../domain/project_requests.dart';
import '../controllers/project_controllers.dart';
import '../widgets/project_form_widgets.dart';

final class ProjectCreatePage extends ConsumerStatefulWidget {
  const ProjectCreatePage({super.key});

  @override
  ConsumerState<ProjectCreatePage> createState() => _ProjectCreatePageState();
}

class _ProjectCreatePageState extends ConsumerState<ProjectCreatePage> {
  final _formKey = GlobalKey<FormState>();
  final _projectName = TextEditingController();
  final _siteAddress = TextEditingController();
  final _contactPhone = TextEditingController();
  final _contractValue = TextEditingController();
  final _description = TextEditingController();
  final _notes = TextEditingController();
  final _ownerName = TextEditingController();
  final _ownerPhone = TextEditingController();
  final _ownerEmail = TextEditingController();
  final _ownerAddress = TextEditingController();
  final _ownerNotes = TextEditingController();
  DateTime? _contractDate;
  DateTime? _startDate;
  DateTime? _expectedEndDate;
  CompanyMemberSummary? _supervisor;

  @override
  void dispose() {
    for (final controller in [
      _projectName,
      _siteAddress,
      _contactPhone,
      _contractValue,
      _description,
      _notes,
      _ownerName,
      _ownerPhone,
      _ownerEmail,
      _ownerAddress,
      _ownerNotes,
    ]) {
      controller.dispose();
    }
    super.dispose();
  }

  Future<void> _submit() async {
    final l10n = AppLocalizations.of(context);
    if (!_formKey.currentState!.validate()) return;
    if (_expectedEndDate != null &&
        _startDate != null &&
        _expectedEndDate!.isBefore(_startDate!)) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(l10n.invalidDateOrder)));
      return;
    }
    final result = await ref
        .read(projectCreateControllerProvider.notifier)
        .create(
          ProjectCreateInput(
            projectName: _projectName.text,
            siteAddress: _siteAddress.text,
            contactPhoneNumber: _contactPhone.text,
            contractValue: _contractValue.text,
            contractDate: _contractDate!,
            startDate: _startDate!,
            expectedEndDate: _expectedEndDate,
            description: _description.text,
            notes: _notes.text,
            newOwner: NewProjectOwnerInput(
              ownerName: _ownerName.text,
              phoneNumber: _ownerPhone.text,
              email: _ownerEmail.text,
              address: _ownerAddress.text,
              notes: _ownerNotes.text,
            ),
            supervisorUserId: _supervisor?.id,
          ),
        );
    if (!mounted || result == null) return;
    await ref.read(projectListControllerProvider.notifier).refresh();
    if (!mounted) return;
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(SnackBar(content: Text(l10n.projectCreated)));
    context.goNamed(
      AppRoutes.projectDetails,
      pathParameters: {'projectId': result.id},
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final submit = ref.watch(projectCreateControllerProvider);
    return SingleChildScrollView(
      padding: const EdgeInsets.all(24),
      keyboardDismissBehavior: ScrollViewKeyboardDismissBehavior.onDrag,
      child: Form(
        key: _formKey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(
              l10n.createProject,
              style: Theme.of(context).textTheme.headlineSmall,
            ),
            const SizedBox(height: 16),
            if (submit.error != null) ...[
              AppMessageBanner(
                message: localizedError(l10n, submit.error!),
                isError: true,
              ),
              const SizedBox(height: 16),
            ],
            _Section(
              title: l10n.projectDetails,
              children: [
                TextFormField(
                  key: const Key('project-name-field'),
                  controller: _projectName,
                  enabled: !submit.isSubmitting,
                  maxLength: 200,
                  decoration: InputDecoration(labelText: l10n.projectName),
                  validator: (value) => Validators.boundedText(
                    value,
                    200,
                    l10n.requiredField,
                    l10n.invalidRequiredText,
                  ),
                ),
                TextFormField(
                  key: const Key('site-address-field'),
                  controller: _siteAddress,
                  enabled: !submit.isSubmitting,
                  maxLength: 500,
                  decoration: InputDecoration(labelText: l10n.siteAddress),
                  validator: (value) => Validators.boundedText(
                    value,
                    500,
                    l10n.requiredField,
                    l10n.invalidRequiredText,
                  ),
                ),
                TextFormField(
                  controller: _contactPhone,
                  enabled: !submit.isSubmitting,
                  maxLength: 20,
                  textDirection: TextDirection.ltr,
                  keyboardType: TextInputType.phone,
                  decoration: InputDecoration(
                    labelText: l10n.contactPhoneOptional,
                  ),
                  validator: (value) => value == null || value.trim().isEmpty
                      ? null
                      : Validators.phone(
                          value,
                          l10n.requiredField,
                          l10n.invalidPhone,
                        ),
                ),
                TextFormField(
                  key: const Key('contract-value-field'),
                  controller: _contractValue,
                  enabled: !submit.isSubmitting,
                  textDirection: TextDirection.ltr,
                  keyboardType: const TextInputType.numberWithOptions(
                    decimal: true,
                  ),
                  inputFormatters: [
                    FilteringTextInputFormatter.allow(RegExp(r'[0-9.]')),
                    LengthLimitingTextInputFormatter(19),
                  ],
                  decoration: InputDecoration(labelText: l10n.contractValueLyd),
                  validator: (value) => Validators.contractValue(
                    value,
                    l10n.invalidContractValue,
                  ),
                ),
                _DateRow(
                  contractDate: _contractDate,
                  startDate: _startDate,
                  expectedEndDate: _expectedEndDate,
                  enabled: !submit.isSubmitting,
                  onContractDate: (value) =>
                      setState(() => _contractDate = value),
                  onStartDate: (value) => setState(() => _startDate = value),
                  onExpectedEndDate: (value) =>
                      setState(() => _expectedEndDate = value),
                ),
                TextFormField(
                  controller: _description,
                  enabled: !submit.isSubmitting,
                  minLines: 2,
                  maxLines: 5,
                  maxLength: 1500,
                  decoration: InputDecoration(
                    labelText: l10n.descriptionOptional,
                  ),
                  validator: (value) => Validators.optionalBoundedText(
                    value,
                    1500,
                    l10n.invalidRequiredText,
                  ),
                ),
                TextFormField(
                  controller: _notes,
                  enabled: !submit.isSubmitting,
                  minLines: 2,
                  maxLines: 4,
                  maxLength: 1000,
                  decoration: InputDecoration(labelText: l10n.notesOptional),
                  validator: (value) => Validators.optionalBoundedText(
                    value,
                    1000,
                    l10n.invalidRequiredText,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            _Section(
              title: l10n.newOwnerTitle,
              message: l10n.existingOwnerUnavailable,
              children: [
                TextFormField(
                  key: const Key('owner-name-field'),
                  controller: _ownerName,
                  enabled: !submit.isSubmitting,
                  maxLength: 200,
                  decoration: InputDecoration(labelText: l10n.ownerName),
                  validator: (value) => Validators.boundedText(
                    value,
                    200,
                    l10n.requiredField,
                    l10n.invalidRequiredText,
                  ),
                ),
                TextFormField(
                  key: const Key('owner-phone-field'),
                  controller: _ownerPhone,
                  enabled: !submit.isSubmitting,
                  maxLength: 20,
                  textDirection: TextDirection.ltr,
                  keyboardType: TextInputType.phone,
                  decoration: InputDecoration(labelText: l10n.phoneNumber),
                  validator: (value) => Validators.phone(
                    value,
                    l10n.requiredField,
                    l10n.invalidPhone,
                  ),
                ),
                TextFormField(
                  controller: _ownerEmail,
                  enabled: !submit.isSubmitting,
                  maxLength: 254,
                  keyboardType: TextInputType.emailAddress,
                  decoration: InputDecoration(labelText: l10n.emailOptional),
                  validator: (value) =>
                      Validators.email(value, l10n.invalidEmail),
                ),
                TextFormField(
                  controller: _ownerAddress,
                  enabled: !submit.isSubmitting,
                  maxLength: 500,
                  decoration: InputDecoration(labelText: l10n.addressOptional),
                  validator: (value) => Validators.optionalBoundedText(
                    value,
                    500,
                    l10n.invalidRequiredText,
                  ),
                ),
                TextFormField(
                  controller: _ownerNotes,
                  enabled: !submit.isSubmitting,
                  maxLength: 1000,
                  minLines: 2,
                  maxLines: 4,
                  decoration: InputDecoration(labelText: l10n.notesOptional),
                  validator: (value) => Validators.optionalBoundedText(
                    value,
                    1000,
                    l10n.invalidRequiredText,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            _Section(
              title: l10n.optionalSupervisor,
              children: [
                SupervisorPickerField(
                  label: l10n.optionalSupervisor,
                  selected: _supervisor,
                  enabled: !submit.isSubmitting,
                  onChanged: (value) => setState(() => _supervisor = value),
                ),
              ],
            ),
            const SizedBox(height: 24),
            Align(
              alignment: AlignmentDirectional.centerEnd,
              child: SubmitButton(
                label: l10n.createProject,
                loading: submit.isSubmitting,
                onPressed: _submit,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

final class _Section extends StatelessWidget {
  const _Section({required this.title, required this.children, this.message});

  final String title;
  final String? message;
  final List<Widget> children;

  @override
  Widget build(BuildContext context) => Card(
    child: Padding(
      padding: const EdgeInsets.all(22),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(title, style: Theme.of(context).textTheme.titleLarge),
          if (message != null) ...[const SizedBox(height: 8), Text(message!)],
          const SizedBox(height: 16),
          for (var index = 0; index < children.length; index++) ...[
            children[index],
            if (index != children.length - 1) const SizedBox(height: 12),
          ],
        ],
      ),
    ),
  );
}

final class _DateRow extends StatelessWidget {
  const _DateRow({
    required this.contractDate,
    required this.startDate,
    required this.expectedEndDate,
    required this.enabled,
    required this.onContractDate,
    required this.onStartDate,
    required this.onExpectedEndDate,
  });

  final DateTime? contractDate;
  final DateTime? startDate;
  final DateTime? expectedEndDate;
  final bool enabled;
  final ValueChanged<DateTime?> onContractDate;
  final ValueChanged<DateTime?> onStartDate;
  final ValueChanged<DateTime?> onExpectedEndDate;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return LayoutBuilder(
      builder: (context, constraints) {
        final width = constraints.maxWidth >= 760
            ? (constraints.maxWidth - 24) / 3
            : constraints.maxWidth;
        return Wrap(
          spacing: 12,
          runSpacing: 12,
          children: [
            SizedBox(
              width: width,
              child: ProjectDateField(
                label: l10n.contractDate,
                value: contractDate,
                required: true,
                enabled: enabled,
                onChanged: onContractDate,
              ),
            ),
            SizedBox(
              width: width,
              child: ProjectDateField(
                label: l10n.startDate,
                value: startDate,
                required: true,
                enabled: enabled,
                onChanged: onStartDate,
              ),
            ),
            SizedBox(
              width: width,
              child: ProjectDateField(
                label: l10n.expectedEndDate,
                value: expectedEndDate,
                enabled: enabled,
                onChanged: onExpectedEndDate,
              ),
            ),
          ],
        );
      },
    );
  }
}
