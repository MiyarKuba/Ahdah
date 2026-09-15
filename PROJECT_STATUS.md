# Project status

## Project

Ahdah — عُهدة

## Current phase

Flutter Suppliers and Payables

## Date, workspace, and branch

- Date: `2026-08-04`.
- Workspace: `C:\dev\Ahdah`.
- Branch: `feat/flutter-suppliers-payables`.
- Pre-existing untracked `frontend/ahdah_app/devtools_options.yaml` remains untouched and unstaged.

## Delivered scope

- Responsive, localized supplier directory with list/detail/create/edit/deactivate, masked payment-account list/create, and currency-separated statement views.
- Supplier-credit invoice and debt list/detail flows plus invoice creation with optional exact quantity/line calculations.
- Supplier payment list/detail/create, multi-debt and multi-funding allocation, verified account selection, and authorized confirmation/rejection.
- Credit-note list/detail/create/approve/allocation and read-only supplier refund history.
- API pagination, stable-ID deduplication, refresh/load-more states, defensive unknown-enum handling, and Arabic RTL/English LTR labels.
- Exact role/navigation/route/action matrix: Manager full supplier administration; Deputy reads plus invoice creation; Accountant financial reads and invoice/payment/credit/account creation; Supervisor assigned-project supplier/debt reads; Worker and unknown roles denied.
- Handwritten immutable Flutter API models and repository boundary; persistence models are not exposed to Flutter.

## Financial and security behavior

- Money stays as `NUMERIC(18,2)`-compatible decimal text and integer minor units; invoice quantity uses `NUMERIC(18,3)`-compatible integer thousandths. Financial calculation and serialization never use binary floating point.
- Multi-debt, multi-funding, and credit allocations require positive unique rows and exact totals. Balances/statements never combine currencies.
- Financial commands use 32 random bytes from `Random.secure` as an ephemeral URL-safe `Idempotency-Key`. There is no automatic write retry. Ambiguous timeout/network results allow only explicit identical-payload retry with the same in-memory key; a changed payload creates a new key.
- Tenant/company, actor/reviewer, lifecycle, balance, version destination, and ledger authority remain server-controlled. Flutter never connects directly to PostgreSQL.
- Payment proof remains metadata only. Raw account identifiers are never displayed; bank/wallet payment selection requires a server-verified masked supplier account.

## Minimal backend contract correction

Accountants may record supplier payments, but the prior funding-source selector was Manager-only. The phase adds the read-only `GET /api/v1/supplier-payments/funding-sources` endpoint under the existing supplier-payment recorder policy. It filters by `company_id`, usable lifecycle, positive availability, optional currency/payment method, and Manager-contribution ownership. It does not add an advance-balance relationship, new business rule, database object, migration, or generated EF change.

## Intentionally deferred

- Supplier refund creation/verification and its expense-return lineage.
- Payment-account verification/rejection/default/update/deactivation.
- Supplier reactivation/hard delete; debt adjustment/write-off/cancellation/reversal.
- Payment cancellation/reversal/correction and credit rejection/cancellation/reversal.
- Binary upload/OCR, invoice similarity, notifications, exchange rates, accounting export, and external bank/gateway integration.
- Advance-balance supplier funding and final settlement/closure.

## Verification

- Packages added or changed: none.
- Backend restore/build passed; build reported 0 warnings and 0 errors.
- Backend tests passed: 260 total (142 unit, 118 integration), 0 failed.
- Focused supplier Flutter tests passed: 26, 0 failed.
- Flutter dependency resolution passed; strict formatting passed with 143 files checked and 0 changes required after formatting.
- `flutter analyze` passed with no issues.
- Full Flutter tests passed: 189 total, 0 failed (including 26 focused supplier/payables tests).
- Flutter Web release build passed with `API_BASE_URL=http://localhost:5231`; its Wasm compatibility dry run also passed. The compiler emitted a non-fatal missing Cupertino icon-font warning, and the application source contains no `CupertinoIcons` usage.
- Android debug APK build passed with `API_BASE_URL=http://10.0.2.2:5231` at `frontend/ahdah_app/build/app/outputs/flutter-apk/app-debug.apk`.
- Android is build-verified only and was not launched. iOS compilation, signing, simulator, and device verification remain pending macOS/Xcode.
- Automated tests use fakes/controlled adapters. No live financial command or PostgreSQL mutation occurred.
- PostgreSQL schema changes: none. Generated Database-First changes: none. Migrations: none. `EnsureCreated`, `EnsureDeleted`, and `Database.Migrate`: not introduced.
- Staging, commit, and push: not performed.

## Verification commands

```powershell
cd C:\dev\Ahdah\frontend\ahdah_app
C:\dev\flutter\bin\flutter.bat pub get
C:\dev\flutter\bin\dart.bat format --output=none --set-exit-if-changed lib test
C:\dev\flutter\bin\flutter.bat analyze
C:\dev\flutter\bin\flutter.bat test
C:\dev\flutter\bin\flutter.bat build web --release --dart-define=API_BASE_URL=http://localhost:5231
C:\dev\flutter\bin\flutter.bat build apk --debug --dart-define=API_BASE_URL=http://10.0.2.2:5231
```

## Exact recommended next task

Design and implement the backend Settlement and Closure foundation using the existing advance settlement, expense confirmation, document completion, reimbursement resolution, supplier debt, manager-contribution settlement, and closure tables without modifying PostgreSQL.
