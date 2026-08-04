abstract final class DecimalMoney {
  static final _pattern = RegExp(r'^(?:0|[1-9][0-9]{0,15})(?:\.[0-9]{1,2})?$');
  static final maximumMinorUnits = BigInt.parse('999999999999999999');

  static String? canonicalize(String? input, {bool allowZero = false}) {
    if (input == null) return null;
    var text = input.trim();
    const arabicDigits = '٠١٢٣٤٥٦٧٨٩';
    const easternDigits = '۰۱۲۳۴۵۶۷۸۹';
    for (var index = 0; index < 10; index++) {
      text = text
          .replaceAll(arabicDigits[index], '$index')
          .replaceAll(easternDigits[index], '$index');
    }
    text = text.replaceAll('٫', '.');
    if (!_pattern.hasMatch(text)) return null;
    final parts = text.split('.');
    final whole = parts.first.replaceFirst(RegExp(r'^0+(?=\d)'), '');
    final fraction = parts.length == 1 ? '00' : parts[1].padRight(2, '0');
    final normalized = '$whole.$fraction';
    final minor = toMinorUnits(normalized);
    if (minor == null || minor > maximumMinorUnits) return null;
    if (!allowZero && minor == BigInt.zero) return null;
    return normalized;
  }

  static BigInt? toMinorUnits(String input) {
    final match = RegExp(r'^(\d{1,16})\.(\d{2})$').firstMatch(input);
    if (match == null) return null;
    final whole = BigInt.tryParse(match.group(1)!);
    final fraction = BigInt.tryParse(match.group(2)!);
    if (whole == null || fraction == null) return null;
    return whole * BigInt.from(100) + fraction;
  }

  static bool sumEquals(Iterable<String> values, String expected) {
    final expectedMinor = toMinorUnits(expected);
    if (expectedMinor == null) return false;
    var total = BigInt.zero;
    for (final value in values) {
      final minor = toMinorUnits(value);
      if (minor == null) return false;
      total += minor;
      if (total > maximumMinorUnits) return false;
    }
    return total == expectedMinor;
  }

  static bool lessThanOrEqual(String value, String maximum) {
    final left = toMinorUnits(value);
    final right = toMinorUnits(maximum);
    return left != null && right != null && left <= right;
  }
}
