import 'dart:ui';

import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../../app/providers.dart';

const _localePreferenceKey = 'ahdah.locale';

final localeControllerProvider =
    StateNotifierProvider<LocaleController, Locale>((ref) {
      return LocaleController(ref.watch(sharedPreferencesProvider));
    });

final class LocaleController extends StateNotifier<Locale> {
  LocaleController(this._preferences)
    : super(Locale(_preferences.getString(_localePreferenceKey) ?? 'ar'));

  final SharedPreferences _preferences;

  Future<void> setLocale(Locale locale) async {
    if (locale.languageCode != 'ar' && locale.languageCode != 'en') return;
    state = Locale(locale.languageCode);
    await _preferences.setString(_localePreferenceKey, locale.languageCode);
  }

  Future<void> toggle() =>
      setLocale(Locale(state.languageCode == 'ar' ? 'en' : 'ar'));
}
