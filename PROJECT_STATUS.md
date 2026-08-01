# Project status

## Project

Ahdah — عُهدة

## Current phase

Flutter Authenticated Shell and Access Administration — Implementation complete; Web release and Android debug APK builds verified

## Date

2026-08-01

## Platforms and packages

Targets are Android, iOS, and Web on Flutter 3.44.7 / Dart 3.12.2.

Direct packages remain unchanged:

- `dio` 5.11.0
- `flutter_riverpod` 2.6.1
- `go_router` 17.3.0
- `flutter_secure_storage` 10.3.1
- `shared_preferences` 2.5.5
- `flutter_localizations` from the Flutter SDK

No state-management, router, serialization/code-generation, database, Firebase, analytics, notification, sharing, or offline-write package was added.

## Authenticated shell architecture

- One nested `go_router` `ShellRoute` hosts stable authenticated routes `/home`, `/access/invitations`, `/access/join-requests`, and `/account` while preserving Web URLs and browser back behavior.
- Compact layouts use a Material 3 `NavigationBar`; widths at 840 logical pixels and above use a persistent `NavigationRail`. Feature pages are shared rather than duplicated by platform.
- The app bar displays safe company and user context. Content is SafeArea-protected, responsive, width-constrained, keyboard accessible, and compatible with Arabic RTL and English LTR.
- `RoleCapabilities` derives `canManageAccess` only from the authoritative role returned by `/api/v1/auth/me`. Manager receives Home, Invitations, Join Requests, and Account. All known non-manager roles and unknown future roles receive Home and Account.
- Router guards protect manager routes in addition to hiding destinations. Unauthenticated shell access redirects to onboarding; authenticated users cannot return to onboarding; non-manager manager-route attempts return to Home without loops. Backend 403 remains authoritative.

## Manager access administration

- A focused `AccessRepository` uses the existing Dio client, bearer interceptor, ProblemDetails mapping, exact camelCase contracts, and handwritten immutable models.
- Invitation and join-request list controllers independently model initial loading, refreshing, data/empty, loading more, and failure states. Filters reset to page 1; page size is 20; stable IDs deduplicate pages; loading-more failures preserve existing records.
- Write controllers are separate for invitation creation, invitation cancellation, join approval, and join rejection. They suppress duplicate submissions, do not retry writes automatically, and apply server results before refreshing lists.

### Invitations

- Manager invitation listing supports exact `Pending`, `Accepted`, `Expired`, and `Cancelled` filters, pull-to-refresh, Web refresh, retry, pagination, localized status chips, and local-time date/expiry presentation.
- Creation sends only `phoneNumber` and one of `Deputy`, `Accountant`, `Supervisor`, or `Worker`; E.164 validation mirrors the backend and `Manager` is never offered.
- The raw creation token is passed only to a one-time dialog-local result, is copied only on explicit action, is absent from list/provider/route/diagnostic/persistence state, and is discarded when the dialog closes. SMS/email delivery is not implemented.
- Only `Pending` invitations expose cancellation. Cancellation requires confirmation, leaves the record visible, and refreshes the server-backed list so `Cancelled` is displayed.

### Join requests

- Manager join-request listing supports exact `Pending`, `Approved`, `Rejected`, and `Cancelled` filters, refresh/retry, pagination, localized roles/statuses, and safe returned applicant/review fields.
- Approval treats requested role as informational and sends only manager-selected `assignedRole` plus supported optional `reviewNotes`. `Manager` is never offered. Sensitive `Deputy`/`Accountant` roles explain identity verification; `Supervisor`/`Worker` explain possible activation.
- Approval surfaces exact `ApprovedAndActivated` and `ApprovedPendingIdentityVerification` results without issuing/displaying a token or changing the manager session.
- Rejection requires the backend-supported 1–500 character reason, permits optional 1–1000 character review notes, requires confirmation, explains login impact, and retains the rejected record after refresh.

## Home, account, session, and localization

- Manager Home shows only safe name/company/role/account/identity state plus access shortcuts and an explicit statement that financial modules are deferred. No fake totals, balances, counts, or charts exist. Other roles receive a safe foundation overview.
- Account displays safe `/auth/me` name, company, localized role, user status, and identity-verification status. IDs, tokens, claims, credentials, and connection details are not displayed. Language switching and local logout are available.
- Authenticated access 401 responses centrally clear the local token and transition to login. A 403 does not clear the token. Network/timeouts remain retryable and preserve a potentially valid mobile token.
- Arabic remains the default with RTL; English is LTR. All new navigation, access workflow, status, dialog, error, pagination, and account strings use ARB/gen-l10n resources.

## Backend contract correction

Contract inspection found one blocking omission: the account requirement could not display authoritative identity state because the safe authentication `/auth/me` user summary omitted it. `identityVerificationStatus` was added to `UserSummary` and its existing mappings, with integration/OpenAPI coverage. No endpoint, authorization rule, business workflow, generated EF file, migration, or PostgreSQL structure/data was changed.

Verified access contracts remain:

- `page` starts at 1; `pageSize` defaults to 20 and has maximum 100; responses use `items`, `page`, `pageSize`, `totalCount`, and `totalPages` sorted newest-first.
- Invitation creation returns `{ invitation, token }`; token is returned only once. Cancellation returns the updated invitation summary.
- Join approval accepts `assignedRole` and nullable `reviewNotes`; rejection accepts required `reason` (maximum 500) and nullable `reviewNotes` (maximum 1000). Decisions return `outcome`, `userStatus`, and updated `request`.
- Manager operations never accept `company_id`; server tenant context remains JWT-derived and tenant-scoped.

## Verification results

- `dotnet restore backend\Ahdah.sln`: passed; all projects up to date.
- `dotnet build backend\Ahdah.sln`: passed with 0 warnings and 0 errors.
- `dotnet test backend\Ahdah.sln`: passed; 68 total, 68 passed, 0 failed, 0 skipped (40 unit, 28 integration).
- `flutter pub get`: passed; no direct dependency change.
- `dart format --output=none --set-exit-if-changed lib test`: passed; 65 files, 0 changes required.
- `flutter analyze`: passed with no issues.
- `flutter test`: passed; 65 total, 65 passed, 0 failed. Tests use fakes/controlled Dio adapters and call no real backend or database.
- Flutter Web release build with `API_BASE_URL=http://localhost:5231`: passed; output `frontend/ahdah_app/build/web`. The existing optional Cupertino-icons font warning was emitted; Material icons compiled and the Wasm dry run succeeded.
- Android debug APK with `API_BASE_URL=http://10.0.2.2:5231`: passed; output `frontend/ahdah_app/build/app/outputs/flutter-apk/app-debug.apk`. The app was not launched on an emulator or physical device.
- iOS source/configuration compatibility was inspected; compilation, signing, simulator, and device verification remain pending on macOS with Xcode.

## Safety and limitations

- No real manager write endpoint was invoked. No invitation was created/cancelled and no join request was approved/rejected against PostgreSQL.
- PostgreSQL mutation status: none; no database write endpoint or mutation command was executed.
- Database structure status: unchanged. No migration, `EnsureCreated`, `EnsureDeleted`, or `Database.Migrate` call was added or run.
- Generated EF files are unchanged.
- Web access tokens remain runtime-memory-only; refresh requires sign-in. Mobile tokens remain in platform secure storage. No refresh-token endpoint exists.
- Existing-member listing/role changes/suspension, identity verification, invitation delivery, projects/sites, financial workflows, suppliers, settlements, reports, notifications, and offline writes remain deferred.
- Android runtime behavior still needs device/emulator verification. iOS still requires macOS/Xcode verification.

## Exact recommended next task

Implement the core company structure foundation: projects/sites and company-member visibility, using the existing PostgreSQL schema and role permissions, before implementing financial advances and expenses.
