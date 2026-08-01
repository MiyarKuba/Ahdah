import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'app/app.dart';
import 'app/bootstrap.dart';
import 'app/providers.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  final bootstrap = await bootstrapAhdah();

  runApp(
    ProviderScope(
      overrides: [
        sharedPreferencesProvider.overrideWithValue(bootstrap.preferences),
        if (bootstrap.config != null)
          appConfigProvider.overrideWithValue(bootstrap.config!),
      ],
      child: AhdahApp(configurationError: bootstrap.configurationError),
    ),
  );
}
