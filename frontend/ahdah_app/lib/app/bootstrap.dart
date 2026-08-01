import 'package:shared_preferences/shared_preferences.dart';

import '../core/config/app_config.dart';

final class BootstrapResult {
  const BootstrapResult({
    required this.preferences,
    this.config,
    this.configurationError,
  });

  final SharedPreferences preferences;
  final AppConfig? config;
  final AppConfigException? configurationError;
}

Future<BootstrapResult> bootstrapAhdah() async {
  final preferences = await SharedPreferences.getInstance();
  try {
    return BootstrapResult(
      preferences: preferences,
      config: AppConfig.fromEnvironment(),
    );
  } on AppConfigException catch (error) {
    return BootstrapResult(preferences: preferences, configurationError: error);
  }
}
