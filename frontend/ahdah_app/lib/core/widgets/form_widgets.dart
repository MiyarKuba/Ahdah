import 'package:flutter/material.dart';

import '../../l10n/app_localizations.dart';

final class PasswordFormField extends StatefulWidget {
  const PasswordFormField({
    required this.controller,
    required this.validator,
    this.onFieldSubmitted,
    this.autofillHints = const [AutofillHints.password],
    this.errorText,
    super.key,
  });

  final TextEditingController controller;
  final FormFieldValidator<String> validator;
  final ValueChanged<String>? onFieldSubmitted;
  final Iterable<String> autofillHints;
  final String? errorText;

  @override
  State<PasswordFormField> createState() => _PasswordFormFieldState();
}

class _PasswordFormFieldState extends State<PasswordFormField> {
  bool _obscure = true;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return TextFormField(
      key: const Key('password-field'),
      controller: widget.controller,
      obscureText: _obscure,
      validator: widget.validator,
      autofillHints: widget.autofillHints,
      textInputAction: TextInputAction.done,
      onFieldSubmitted: widget.onFieldSubmitted,
      decoration: InputDecoration(
        labelText: l10n.password,
        errorText: widget.errorText,
        suffixIcon: IconButton(
          tooltip: _obscure ? l10n.showPassword : l10n.hidePassword,
          onPressed: () => setState(() => _obscure = !_obscure),
          icon: Icon(_obscure ? Icons.visibility : Icons.visibility_off),
        ),
      ),
    );
  }
}

final class SubmitButton extends StatelessWidget {
  const SubmitButton({
    required this.label,
    required this.loading,
    required this.onPressed,
    super.key,
  });

  final String label;
  final bool loading;
  final VoidCallback onPressed;

  @override
  Widget build(BuildContext context) => Semantics(
    liveRegion: loading,
    label: loading ? AppLocalizations.of(context).loading : label,
    child: FilledButton(
      onPressed: loading ? null : onPressed,
      child: loading
          ? const SizedBox.square(
              dimension: 22,
              child: CircularProgressIndicator(strokeWidth: 2),
            )
          : Text(label),
    ),
  );
}

final class AppMessageBanner extends StatelessWidget {
  const AppMessageBanner({
    required this.message,
    this.isError = false,
    super.key,
  });
  final String message;
  final bool isError;

  @override
  Widget build(BuildContext context) => Semantics(
    liveRegion: true,
    child: Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: isError
            ? Theme.of(context).colorScheme.errorContainer
            : Theme.of(context).colorScheme.secondaryContainer,
        borderRadius: BorderRadius.circular(12),
      ),
      child: Text(message),
    ),
  );
}
