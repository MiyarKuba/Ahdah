# Project status

## Project

Ahdah — عُهدة

## Current phase

Flutter Project Settlement Readiness — read-only consumer of the completed backend contract

## Date, workspace, and branch

- Date: `2026-09-16`.
- Workspace: `C:\dev\Ahdah`.
- Branch: `feat/flutter-settlement-readiness`.
- Baseline: `b8b15bb37eedb3ae5863521578f7a61d9f5507c1` (`feat: add project settlement readiness foundation`).
- Created the feature branch directly from the verified backend commit; preserved all history and main.
- Pre-existing untracked `frontend/ahdah_app/devtools_options.yaml` remains untouched and unstaged.

## Delivered scope

- Project Details entry to named route `projectSettlement`, `/projects/:projectId/settlement`, within the existing authenticated shell.
- Feature-first `settlements` domain/data/presentation layers: immutable handwritten public-contract models, API repository/provider, and project-scoped Riverpod controller.
- Authenticated GET through the existing API client, without client company/actor/role authority. Exact nested decimal JSON tokens become two-decimal strings; categories and currencies are never aggregated.
- Dedicated capability: Manager/Deputy/Accountant permitted; Supervisor limited to server-authorized assigned projects and visible expense/document/debt findings; Worker and unknown roles denied. Backend authorization remains authoritative.
- Separate settlement and financial-closure readiness cards and impediments. Nullable booleans remain nullable; false is blocked, null is indeterminate, and unsupported future states do not certify clearance. Zero known blockers never means ready.
- Prominent evaluation gaps, visible blocker count, expandable evaluated categories, currency-separated amounts, and localized blocker records. `NotVisible` suppresses counts/totals/IDs/navigation; `NotAttributable` explains unavailable attribution without showing zero.
- Safe permitted navigation for expense, personal claim, supplier debt, supplier payment, and supplier credit note records. Unknown/unsupported types have no link; API `resourcePath` is never interpreted as a Flutter route.
- English/LTR and Arabic/RTL stable-code labels, safe unknown fallbacks, responsive layouts, refresh/loading/retry/error states, 401 session expiry, stale snapshot removal on failed refresh, and late-response disposal protection.
- Updated README and architecture, Flutter development, and settlement documentation. No finalize/close controls or backend changes.

## Verification

- `flutter pub get`: passed; existing dependency constraints retained.
- `dart format --output=none --set-exit-if-changed lib test`: passed, 154 files, 0 changes.
- `flutter analyze`: passed, no issues.
- `flutter test`: passed, **242 tests** (189 baseline + **53 added**), no failures.
- Added model/repository/controller, capability/routing/widget, and security/source tests. Coverage includes exact large nested decimal tokens, nullable flags, immutable collections, safe requests, all current emitted reason/gap labels, hidden/unattributable injected data, evaluated-empty categories, permitted real route navigation, unsupported records/states, narrow EN/AR layouts, loading/refresh, disposal, and 401/403/404/network handling.
- Web release build: passed with `API_BASE_URL=http://localhost:5231`; WebAssembly compatibility dry run succeeded. The build emitted a non-fatal Cupertino font-family warning; no Cupertino assets were introduced in this milestone.
- Android debug APK build: passed with `API_BASE_URL=http://10.0.2.2:5231`, output `frontend/ahdah_app/build/app/outputs/flutter-apk/app-debug.apk`.
- `git diff --check`: passed. Temporary implementation scripts were removed and the final milestone diff reviewed; only milestone files are included.
- Backend code and generated persistence models are unchanged. The prior backend milestone passed 356 tests; these were not rerun for this Flutter-only change.
- Tests use fake repositories and controlled HTTP adapters. No live API/database requests, app runtime launch, financial writes, or PostgreSQL mutation were performed in this milestone.

## Material limitations and deferred scope

- Advance/project attribution, final settlement eligibility, and financial-closure transition policy remain undefined. The backend remains the source of truth and cannot currently certify finalization.
- Snapshot results are informational, not a lock or permission for a later operation. All matching blockers are returned by the current endpoint; pagination for very large projects is a future contract concern.
- Only existing permitted detail routes are linked. Refund/payment types without an applicable Flutter detail route remain display-only. There is no assumption that manager contributions are repayable.
- Finalize/close commands, automatic lifecycle changes, supplier refund creation, reversal workflows, binary proof upload, OCR, notifications, currency conversion, and accounting exports remain separate milestones.
- iOS compilation/signing and simulator/device checks require macOS/Xcode; no iOS build or runtime verification is claimed. Web and Android builds do not replace live API or device testing.
- Contract and backend policy limitations: [docs/SETTLEMENT.md](docs/SETTLEMENT.md).

## Exact recommended next task

Draft a decision document for project advance attribution and settlement/financial-closure eligibility using the current schema and read-only contract. Identify unresolved business choices and required approvals; do not implement commands, schema changes, or financial rules before those decisions are approved.
