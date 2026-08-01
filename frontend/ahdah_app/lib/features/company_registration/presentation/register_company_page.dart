import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../app/providers.dart';
import '../../../core/errors/app_exception.dart';
import '../../../core/localization/error_labels.dart';
import '../../../core/validation/validators.dart';
import '../../../core/widgets/auth_shell.dart';
import '../../../core/widgets/form_widgets.dart';
import '../../../l10n/app_localizations.dart';
import '../../authentication/domain/identity_requests.dart';
import '../../session/presentation/session_controller.dart';

final class RegisterCompanyPage extends ConsumerStatefulWidget {
  const RegisterCompanyPage({super.key});

  @override
  ConsumerState<RegisterCompanyPage> createState() =>
      _RegisterCompanyPageState();
}

class _RegisterCompanyPageState extends ConsumerState<RegisterCompanyPage> {
  final _formKey = GlobalKey<FormState>();
  final _companyName = TextEditingController();
  final _companyCode = TextEditingController();
  final _managerName = TextEditingController();
  final _phone = TextEditingController();
  final _email = TextEditingController();
  final _password = TextEditingController();
  bool _submitting = false;
  AppException? _error;

  @override
  void dispose() {
    _companyName.dispose();
    _companyCode.dispose();
    _managerName.dispose();
    _phone.dispose();
    _email.dispose();
    _password.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (_submitting || !_formKey.currentState!.validate()) return;
    setState(() {
      _submitting = true;
      _error = null;
    });
    try {
      final result = await ref
          .read(authRepositoryProvider)
          .registerCompany(
            RegisterCompanyRequest(
              companyName: _companyName.text,
              companyCode: _companyCode.text,
              managerFullName: _managerName.text,
              managerPhone: _phone.text,
              managerEmail: _email.text,
              password: _password.text,
            ),
          );
      _password.clear();
      await ref
          .read(sessionControllerProvider.notifier)
          .establish(result.authentication);
    } on AppException catch (error) {
      _password.clear();
      if (mounted) setState(() => _error = error);
    } finally {
      if (mounted) setState(() => _submitting = false);
    }
  }

  String? _serverError(String field, AppLocalizations l10n) =>
      _error?.fieldErrors.values.containsKey(field) == true
      ? l10n.validationError
      : null;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return AuthShell(
      title: l10n.createCompany,
      subtitle: l10n.companyCodeHint,
      child: AutofillGroup(
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              if (_error != null) ...[
                AppMessageBanner(
                  message: localizedError(l10n, _error!),
                  isError: true,
                ),
                const SizedBox(height: 16),
              ],
              TextFormField(
                key: const Key('company-name-field'),
                controller: _companyName,
                enabled: !_submitting,
                textInputAction: TextInputAction.next,
                autofillHints: const [AutofillHints.organizationName],
                decoration: InputDecoration(
                  labelText: l10n.companyName,
                  errorText: _serverError('companyName', l10n),
                ),
                validator: (value) => Validators.name(
                  value,
                  l10n.requiredField,
                  l10n.invalidCompanyName,
                ),
              ),
              const SizedBox(height: 16),
              TextFormField(
                key: const Key('company-code-field'),
                controller: _companyCode,
                enabled: !_submitting,
                textCapitalization: TextCapitalization.characters,
                textDirection: TextDirection.ltr,
                textInputAction: TextInputAction.next,
                inputFormatters: const [UpperCaseTextFormatter()],
                decoration: InputDecoration(
                  labelText: l10n.companyCode,
                  helperText: l10n.companyCodeHint,
                  errorText: _serverError('companyCode', l10n),
                ),
                validator: (value) => Validators.companyCode(
                  value,
                  l10n.requiredField,
                  l10n.invalidCompanyCode,
                ),
              ),
              const SizedBox(height: 16),
              TextFormField(
                key: const Key('manager-name-field'),
                controller: _managerName,
                enabled: !_submitting,
                textInputAction: TextInputAction.next,
                autofillHints: const [AutofillHints.name],
                decoration: InputDecoration(
                  labelText: l10n.managerFullName,
                  errorText: _serverError('managerFullName', l10n),
                ),
                validator: (value) => Validators.name(
                  value,
                  l10n.requiredField,
                  l10n.invalidFullName,
                ),
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _phone,
                enabled: !_submitting,
                keyboardType: TextInputType.phone,
                textDirection: TextDirection.ltr,
                textInputAction: TextInputAction.next,
                autofillHints: const [AutofillHints.telephoneNumber],
                decoration: InputDecoration(
                  labelText: l10n.managerPhone,
                  helperText: l10n.phoneHint,
                  errorText: _serverError('managerPhone', l10n),
                ),
                validator: (value) => Validators.phone(
                  value,
                  l10n.requiredField,
                  l10n.invalidPhone,
                ),
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _email,
                enabled: !_submitting,
                keyboardType: TextInputType.emailAddress,
                textDirection: TextDirection.ltr,
                textInputAction: TextInputAction.next,
                autofillHints: const [AutofillHints.email],
                decoration: InputDecoration(
                  labelText: l10n.emailOptional,
                  errorText: _serverError('managerEmail', l10n),
                ),
                validator: (value) =>
                    Validators.email(value, l10n.invalidEmail),
              ),
              const SizedBox(height: 16),
              PasswordFormField(
                controller: _password,
                autofillHints: const [AutofillHints.newPassword],
                errorText: _serverError('password', l10n),
                validator: (value) => Validators.onboardingPassword(
                  value,
                  l10n.requiredField,
                  l10n.passwordMinOnboarding,
                ),
                onFieldSubmitted: (_) => _submit(),
              ),
              const SizedBox(height: 24),
              SubmitButton(
                label: l10n.createCompany,
                loading: _submitting,
                onPressed: _submit,
              ),
            ],
          ),
        ),
      ),
    );
  }
}
