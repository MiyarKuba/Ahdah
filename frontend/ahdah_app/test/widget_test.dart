import 'package:ahdah_app/app/app.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  testWidgets('renders the Ahdah bootstrap screen', (tester) async {
    await tester.pumpWidget(const AhdahApp());

    expect(find.text('عُهدة'), findsOneWidget);
    expect(find.text('Ahdah'), findsOneWidget);
    expect(
      find.text('Construction custody and expense management'),
      findsOneWidget,
    );

    final arabicTitle = tester.widget<Text>(
      find.byKey(const Key('arabic-title')),
    );

    expect(arabicTitle.textDirection, TextDirection.rtl);
  });
}
