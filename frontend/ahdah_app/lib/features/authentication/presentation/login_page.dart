import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../app/providers.dart';
import '../../../app/routing/app_routes.dart';
import '../../../core/errors/app_exception.dart';
import '../../../core/localization/error_labels.dart';
import '../../../core/validation/validators.dart';
import '../../../core/widgets/auth_shell.dart';
import '../../../core/widgets/form_widgets.dart';
import '../../../l10n/app_localizations.dart';
import '../../session/presentation/session_controller.dart';
import '../domain/identity_requests.dart';

final class LoginPage extends ConsumerStatefulWidget {
  const LoginPage({super.key});

  @override
  ConsumerState<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends ConsumerState<LoginPage> {
  final _formKey = GlobalKey<FormState>();
  final _phone = TextEditingController();
  final _password = TextEditingController();
  bool _submitting = false;
  AppException? _error;

  @override
  void dispose() {
    _phone.dispose();
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
      final authentication = await ref
          .read(authRepositoryProvider)
          .login(
            LoginRequest(phoneNumber: _phone.text, password: _password.text),
          );
      await ref
          .read(sessionControllerProvider.notifier)
          .establish(authentication);
    } on AppException catch (error) {
      if (error.kind == AppExceptionKind.unauthorized) _password.clear();
      if (mounted) setState(() => _error = error);
    } finally {
      if (mounted) setState(() => _submitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final sessionExpired =
        ref.watch(sessionControllerProvider).status ==
        SessionStatus.sessionExpired;
    return AuthShell(
      title: l10n.login,
      subtitle: l10n.phoneHint,
      child: AutofillGroup(
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              if (sessionExpired) ...[
                AppMessageBanner(message: l10n.sessionExpired),
                const SizedBox(height: 16),
              ],
              if (_error != null) ...[
                AppMessageBanner(
                  message: localizedError(l10n, _error!),
                  isError: true,
                ),
                const SizedBox(height: 16),
              ],
              TextFormField(
                key: const Key('login-phone-field'),
                controller: _phone,
                enabled: !_submitting,
                keyboardType: TextInputType.phone,
                textDirection: TextDirection.ltr,
                textInputAction: TextInputAction.next,
                autofillHints: const [AutofillHints.telephoneNumber],
                decoration: InputDecoration(
                  labelText: l10n.phoneNumber,
                  helperText: l10n.phoneHint,
                ),
                validator: (value) => Validators.phone(
                  value,
                  l10n.requiredField,
                  l10n.invalidPhone,
                ),
              ),
              const SizedBox(height: 16),
              PasswordFormField(
                controller: _password,
                validator: (value) =>
                    Validators.loginPassword(value, l10n.passwordMinLogin),
                onFieldSubmitted: (_) => _submit(),
              ),
              const SizedBox(height: 24),
              SubmitButton(
                label: l10n.login,
                loading: _submitting,
                onPressed: _submit,
              ),
              const SizedBox(height: 20),
              TextButton(
                onPressed: _submitting
                    ? null
                    : () => context.goNamed(AppRoutes.registerCompany),
                child: Text(l10n.createCompany),
              ),
              TextButton(
                onPressed: _submitting
                    ? null
                    : () => context.goNamed(AppRoutes.joinRequest),
                child: Text(l10n.submitJoinRequest),
              ),
              TextButton(
                onPressed: _submitting
                    ? null
                    : () => context.goNamed(AppRoutes.acceptInvitation),
                child: Text(l10n.acceptInvitation),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
