import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../core/config/app_config.dart';
import '../core/storage/locale_controller.dart';
import '../features/authentication/presentation/configuration_error_page.dart';
import '../features/session/presentation/session_controller.dart';
import '../l10n/app_localizations.dart';
import 'routing/app_router.dart';
import 'theme/app_theme.dart';

final class AhdahApp extends ConsumerWidget {
  const AhdahApp({this.configurationError, super.key});

  final AppConfigException? configurationError;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final locale = ref.watch(localeControllerProvider);
    if (configurationError != null) {
      return MaterialApp(
        debugShowCheckedModeBanner: false,
        locale: locale,
        supportedLocales: AppLocalizations.supportedLocales,
        localizationsDelegates: const [
          AppLocalizations.delegate,
          GlobalMaterialLocalizations.delegate,
          GlobalWidgetsLocalizations.delegate,
          GlobalCupertinoLocalizations.delegate,
        ],
        onGenerateTitle: (context) => AppLocalizations.of(context).appName,
        theme: AppTheme.light(),
        home: const ConfigurationErrorPage(),
      );
    }

    return _SessionBootstrapper(
      child: MaterialApp.router(
        debugShowCheckedModeBanner: false,
        locale: locale,
        supportedLocales: AppLocalizations.supportedLocales,
        localizationsDelegates: const [
          AppLocalizations.delegate,
          GlobalMaterialLocalizations.delegate,
          GlobalWidgetsLocalizations.delegate,
          GlobalCupertinoLocalizations.delegate,
        ],
        onGenerateTitle: (context) => AppLocalizations.of(context).appName,
        theme: AppTheme.light(),
        routerConfig: ref.watch(appRouterProvider),
      ),
    );
  }
}

final class _SessionBootstrapper extends ConsumerStatefulWidget {
  const _SessionBootstrapper({required this.child});
  final Widget child;

  @override
  ConsumerState<_SessionBootstrapper> createState() =>
      _SessionBootstrapperState();
}

class _SessionBootstrapperState extends ConsumerState<_SessionBootstrapper> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      ref.read(sessionControllerProvider.notifier).bootstrap();
    });
  }

  @override
  Widget build(BuildContext context) => widget.child;
}
