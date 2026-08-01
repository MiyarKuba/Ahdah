import 'package:flutter/material.dart';

import '../../l10n/app_localizations.dart';

abstract final class AppDateTimeFormatter {
  static String format(BuildContext context, DateTime? utc) {
    if (utc == null) return AppLocalizations.of(context).unknownValue;
    final local = utc.toLocal();
    final material = MaterialLocalizations.of(context);
    return '${material.formatMediumDate(local)} · '
        '${material.formatTimeOfDay(TimeOfDay.fromDateTime(local))}';
  }
}
