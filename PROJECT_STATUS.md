# Project status

## Project

Ahdah — عُهدة

## Current phase

Flutter Company Structure — Projects and Member Directory

## Date

2026-08-01

## Workspace and branch

- Workspace confirmed before edits: `C:\dev\Ahdah`.
- Branch confirmed before edits: `feat/flutter-company-structure`.
- The working tree was clean at the initial safety gate.

## Backend contracts consumed

- Manager/Deputy `GET /api/v1/company/members` and `GET /api/v1/company/members/{memberId}`.
- Role-filtered `GET /api/v1/projects` and `GET /api/v1/projects/{projectId}`.
- Manager-only `POST /api/v1/projects`, `PATCH /api/v1/projects/{projectId}`, and `PUT /api/v1/projects/{projectId}/supervisor`.
- Capability/record-filtered `GET /api/v1/projects/{projectId}/members`.
- Exact `items`, `page`, `pageSize`, `totalCount`, and `totalPages` pagination metadata.
- Exact member list/detail, project/owner/supervisor/member, create/update/assignment, validation, Problem Details, and camelCase JSON contracts were inspected from controllers, Application contracts/models, validation attributes, OpenAPI setup/coverage, and existing integration tests. No generated EF entity is consumed.

## Flutter architecture

- Added feature-first `features/projects/{domain,data,presentation}` with handwritten immutable models, exact request inputs, an API repository, focused Riverpod controllers, and responsive list/detail/create/edit/supervisor/member pages.
- Added parallel `features/company_members` list/detail architecture with distinct safe list/detail models and a read-only repository.
- Extended the existing Dio client, bearer interceptor, Problem Details handling, session-expiry flow, localization mappings, shared widgets, Riverpod, and `go_router`; no package was added or changed.
- Contract values remain nullable decimal text. Manager creation validates the text and emits it as an exact JSON number without a `double` conversion. No financial arithmetic exists.

## Routes and guards

- `/projects`
- `/projects/new`
- `/projects/:projectId`
- `/projects/:projectId/edit`
- `/projects/:projectId/supervisor`
- `/projects/:projectId/members`
- `/company/members`
- `/company/members/:memberId`

Central `RoleCapabilities` drives both navigation and router redirects. Worker and unknown roles cannot route to projects. Non-Managers cannot route to create/edit/supervisor pages. Accountant cannot route to project members. Only Manager and Deputy can route to the company directory. Unauthenticated access still follows the authoritative session redirect, and backend 403 responses remain authoritative without clearing valid tokens.

## Role visibility

| Role | Navigation and reads | Project writes | Project supervisors | Contract value | Directory |
|---|---|---|---|---|---|
| Manager | All tenant projects | Create, safe update, replace supervisor | Yes | Yes | Read-only |
| Deputy | All tenant projects | None | Yes | No | Read-only |
| Accountant | All tenant projects | None | No | No | No |
| Supervisor | Active assignments only | None | Assigned-project summaries | No | No |
| Worker | No project relationship | None | No | No | No |
| Unknown | Safe Home/Account minimum | None | No | No | No |

## Project list and detail

- Initial loading, mobile pull-to-refresh, explicit refresh, localized empty/retry, server status filter/search, page size 20, stable deduplication, load-more recovery, and stale-response suppression are implemented.
- One-character searches are never sent; search is capped at 100 characters and debounced without another package.
- Supervisor empty messaging says only assigned projects appear. No page sweep or totals are calculated.
- Detail displays only actual project, owner, date, status, contact, description/note, supervisor, and useful timestamp fields. It exposes only capability-approved actions.
- A non-Manager never renders contract value, including when a fake response incorrectly contains it.

## Manager project creation and owner behavior

- The form uses actual accepted fields: project/site/contact, positive two-decimal contract value, contract/start/expected-end dates, description/notes, complete `newOwner`, and optional supervisor.
- Initial status is not selectable and therefore remains backend-fixed `Active`. No terminal/final status action exists.
- No safe owner-list endpoint exists, so normal users receive the atomic new-owner flow. No raw existing-owner UUID input or invented owner list exists. Existing-owner selection remains a documented API limitation.
- Duplicate submit is prevented; writes are not retried automatically. Success refreshes the list and routes to authoritative detail.

## Manager metadata update

- Authoritative detail is loaded before editing and supplies `expectedVersion`.
- Only changed supported metadata is sent. Owner is read-only, contract value is absent, nullable clearing is not invented, and only `Active`/`Paused` are selectable.
- HTTP 409 remains explicit and provides a reload action; it is never automatically retried.

## Supervisor assignment and project supervisor view

- The selector requests `role=Supervisor`, `status=Active`, bounded search, and page size 20, then defensively filters the page again. No arbitrary user ID entry is shown.
- Current active supervisors are displayed. Replacement requires confirmation and states that the server ends assignments without deleting history. Unassignment is absent.
- The members route is titled Project supervisors and explains that it represents active supervision assignments only, not workers or complete project staff. It is paginated and read-only.

## Company member directory

- Manager and Deputy receive server-filtered role/status/search, page size 20, refresh, stable deduplication, retry, load-more recovery, localized empty states, responsive cards, and safe detail navigation.
- List presentation does not invent phone/email fields omitted by the list DTO. Detail uses the exact endpoint for safe contact and timestamps.
- List and detail are explicitly read-only. There are no role, status, deletion, activation/suspension, or identity-verification actions.

## Error, session, localization, and accessibility behavior

- Existing AppException/Problem Details mappings cover validation, 401 expiry, 403 permission denial without token deletion, 404 unavailable records, 409 stale conflicts, server errors, timeout, and network retry.
- Lists preserve loaded data after load-more failure. Create/update/supervisor writes prevent duplicates and do not auto-retry.
- Arabic RTL and English LTR resources cover navigation, filters, statuses, forms, validation, empty/error/conflict states, project terminology, active-supervision wording, and read-only member presentation. Unknown values use a safe fallback.
- Pages use SafeArea through the authenticated shell, constrained responsive layouts, keyboard-safe scrolling, text-labelled status chips, tooltips, semantic loading controls, and practical Material touch/focus behavior.

## Flutter verification

- Package changes: none. `flutter pub get` passed; 11 newer incompatible package versions were informational only.
- Format: `dart format --output=none --set-exit-if-changed lib test` passed; 90 files, 0 changed.
- Analyze: `flutter analyze` passed with no issues.
- Tests: `flutter test` passed; 104 total, 0 failed.
- Tests use fake repositories/token stores and controlled Dio adapters; no real project/member API or PostgreSQL workflow was called.
- Web: release build passed with `API_BASE_URL=http://localhost:5231`; output is `frontend/ahdah_app/build/web`. The build reported a non-fatal missing Cupertino-icons font warning while Material icons were present, and the Wasm dry run succeeded.
- Android: debug APK build passed with `API_BASE_URL=http://10.0.2.2:5231`; output is `frontend/ahdah_app/build/app/outputs/flutter-apk/app-debug.apk`. The app was not launched on an emulator or device.
- iOS: shared Dart/source compatibility and narrow `NSAllowsLocalNetworking` ATS configuration were inspected. No iOS build was attempted on Windows. The repository currently has no checked-in `ios/Podfile`; macOS/Xcode/CocoaPods generation/resolution, signing, simulator, and device verification remain pending.

## Backend, database, and generated-file status

- Backend source changes: none.
- Backend restore/build/tests: not rerun because backend files did not change. The prior verified baseline remains 114 passing backend tests, but this task makes no new backend-test claim.
- PostgreSQL mutation: none during this task. No real API write, database command, catalog query, migration, or schema operation was executed.
- Generated EF files: unchanged.
- Migrations: none created or run. No `EnsureCreated`, `EnsureDeleted`, or `Database.Migrate` was introduced.
- Financial modules: none added.

## Warnings and limitations

- Existing-owner selection needs a future safe owner-directory API; creation currently uses only the supported atomic new-owner UX.
- Worker project visibility cannot exist until an approved real worker/project relationship exists.
- Project members remain active supervisor assignments only because the schema has no generic project-member relationship.
- Direct contract-value changes, completion, cancellation, and financial closure remain deferred.
- Member role/status mutations and identity verification remain deferred.
- The Web build's Cupertino-icons font warning is non-fatal and no Cupertino icon dependency was added in this task.
- iOS verification requires macOS and Xcode; the missing checked-in Podfile should be reviewed/generated there before the first iOS build.

## Exact recommended next task

Design and implement the backend Advances foundation using the existing advances, advance funding, distribution, settlement, return, balance, and approval tables without modifying the PostgreSQL schema.
