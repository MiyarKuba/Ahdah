# Ahdah — عُهدة

Ahdah is a multi-tenant platform for financial custody and construction operations. It is intended to help companies coordinate projects, custody balances, transfers, expenses, supplier obligations, worker claims, documents, audit trails, notifications, and scheduled reporting.

This repository is currently in **Flutter Project Settlement Readiness**. Project Details opens the read-only `/projects/:projectId/settlement` screen, consuming the completed backend foundation with Arabic/English labels, exact currency-separated amounts, role-limited findings, and explicit evaluation gaps. Financial finalization and closure writes remain deferred.

`GET /api/v1/projects/{projectId}/settlement` returns blocker references, category totals, evaluation gaps and distinct settlement/closure impediments. `canSettle` and `canClose` are nullable: false means blocked; null means indeterminate. Zero known blockers does not establish readiness because advance/project attribution and project finalization policy are undefined. See [docs/SETTLEMENT.md](docs/SETTLEMENT.md) for the authoritative contract and limitations.

Manager, Deputy, Accountant, and assigned Supervisor can view settlement according to backend access; Worker and unknown roles cannot. `NotVisible` categories show no counts, records, or amounts. `NotAttributable` means unavailable attribution, never a zero balance. Blockers link only to existing permitted detail screens by record type and ID. Category/currency totals are never combined, and no finalize/close controls exist. Current verification is recorded in [PROJECT_STATUS.md](PROJECT_STATUS.md).

## Technology stack

- Client: Flutter stable and Dart, targeting Android, iOS, and Web
- API: controller-based ASP.NET Core Web API on .NET 10 and C#
- Architecture: modular monolith with explicit internal boundaries
- Database: PostgreSQL 18 (`ahdah_db`, schema `ahdah`), Entity Framework Core, and Npgsql using a database-first workflow

The 91-table generated persistence model is isolated under Infrastructure. No credential or migration is stored in the repository.

## Repository structure

```text
backend/
  src/
    Ahdah.Api/             HTTP host and controllers
    Ahdah.Application/     Use cases, contracts, validation, orchestration
    Ahdah.Domain/          Domain rules and models
    Ahdah.Infrastructure/  Technical implementations and integrations
  tests/
    Ahdah.UnitTests/
    Ahdah.IntegrationTests/
frontend/
  ahdah_app/               Flutter client for Android, iOS, and Web
database/
  schema/                  Reserved reviewed database-first artifacts
  documentation/           Reserved database documentation
docs/                      Product and engineering guidance
```

## Local prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Flutter stable with its bundled Dart SDK
- A supported browser for Flutter Web
- Android Studio and an Android SDK for Android development
- macOS with Xcode and CocoaPods for iOS build and testing

For local database tooling or API startup, configure `ConnectionStrings:AhdahDatabase` and `Authentication:Jwt:SigningKey` in the API project's .NET User Secrets. Never store either value in repository files. Production must supply secrets through environment-based secret configuration.

Non-secret JWT defaults are `Ahdah.Api` for the issuer, `Ahdah.Clients` for the audience, and 15 minutes for access-token lifetime. Startup fails clearly if the signing key is absent or shorter than 32 UTF-8 bytes.

## Build the backend

From the repository root:

```powershell
dotnet restore backend\Ahdah.sln
dotnet build backend\Ahdah.sln
```

The development-only OpenAPI document is exposed by the API host. The database-independent health route remains anonymous at `GET /api/system/health`.

Identity Phase 1 exposes:

- `POST /api/v1/auth/register-company`
- `POST /api/v1/auth/login`
- `GET /api/v1/auth/me`

The access phase additionally exposes:

- Manager-only `POST /api/v1/invitations`
- Manager-only `GET /api/v1/invitations`
- Public `POST /api/v1/invitations/accept`
- Manager-only `POST /api/v1/invitations/{invitationId}/cancel`
- Public `POST /api/v1/join-requests`
- Manager-only `GET /api/v1/join-requests`
- Manager-only `POST /api/v1/join-requests/{joinRequestId}/approve`
- Manager-only `POST /api/v1/join-requests/{joinRequestId}/reject`

The company-structure phase additionally exposes:

- Manager/Deputy `GET /api/v1/company/members`
- Manager/Deputy `GET /api/v1/company/members/{memberId}`
- Role-filtered `GET /api/v1/projects`
- Role-filtered `GET /api/v1/projects/{projectId}`
- Manager-only `POST /api/v1/projects`
- Manager-only `PATCH /api/v1/projects/{projectId}`
- Manager-only `PUT /api/v1/projects/{projectId}/supervisor`
- Schema-limited `GET /api/v1/projects/{projectId}/members`

Advances Foundation — Backend Phase 1 additionally exposes:

- Role-filtered `GET /api/v1/advances`
- Role-filtered `GET /api/v1/advances/{advanceId}`
- Role-filtered `GET /api/v1/advances/{advanceId}/movements`
- Manager-only `GET /api/v1/advance-funding-sources`
- Manager-only `POST /api/v1/advances`
- Manager/Deputy `POST /api/v1/advances/{advanceId}/distributions`
- Holder `POST /api/v1/advances/{advanceId}/returns`
- Recipient `POST /api/v1/advance-transfers/{transferId}/confirm`
- Recipient `POST /api/v1/advance-transfers/{transferId}/reject` for internal transfers and returns
- Current-user `GET /api/v1/advance-balances/me`
- Manager/Deputy/Accountant `GET /api/v1/advance-balances/users/{userId}`

Every financial command requires an `Idempotency-Key` header. Creation allocates only existing same-company `Available`/`PartiallyUsed` funding sources and funds the fixed advance amount atomically. The schema has no direct advance/project relationship, so project association and filtering are omitted. Settlement and closure writes remain deferred because their existing tables depend on expense, document, debt, claim, resolution, and review workflows. See [docs/ADVANCES.md](docs/ADVANCES.md).

Expenses Foundation — Backend Phase 1 additionally exposes:

- `GET`/Manager-only `POST /api/v1/expense-categories`
- Role-filtered `GET /api/v1/expenses`
- Role-filtered `GET /api/v1/expenses/{expenseId}`
- Manager/Deputy/Supervisor/Worker `POST /api/v1/expenses`
- `GET /api/v1/expenses/{expenseId}/allocations`
- `GET`/authorized metadata-only `POST /api/v1/expenses/{expenseId}/attachments`
- `GET /api/v1/expenses/{expenseId}/history`
- Manager/Deputy/Accountant `POST /api/v1/expenses/{expenseId}/approve`
- Manager/Deputy/Accountant `POST /api/v1/expenses/{expenseId}/reject`
- Role-filtered `GET /api/v1/reimbursements`
- Role-filtered `GET /api/v1/reimbursements/{reimbursementId}`

Expense creation supports exact `AdvanceBalance` and `PersonalFunds` payment modes. It uses one optional direct project because the schema has no expense project-split table. Supplier credit, original-paper custody, binary upload, claim payment, and final settlement are intentionally not published. Financial and lifecycle commands require `Idempotency-Key`. See [docs/EXPENSES.md](docs/EXPENSES.md).

Suppliers and Supplier Debt Foundation — Backend Phase 1 additionally exposes tenant/role-filtered supplier directory and statement reads, masked supplier payment accounts, supplier-credit invoices backed by `expenses` plus `supplier_debts`, optional `expense_items`, partial/full supplier payments with debt and funding allocations, payment confirmation/rejection, read-only refund history, and approved credit-note allocation. Financial commands reuse `Idempotency-Key`, explicit transactions, row locks, computed debt balances, immutable ledgers, and concurrency tokens. A role-safe read-only `/api/v1/supplier-payments/funding-sources` selector exposes only sources the authenticated payment recorder may use. There is no standalone supplier-invoice table and no supplier-payment relationship to advance balances. See [docs/SUPPLIERS.md](docs/SUPPLIERS.md).

Manager and Deputy see all company projects; Accountant sees basic project structure without contract value; Supervisor sees only actively assigned projects; Worker sees no projects because the schema has no worker/project relationship. Project-member results contain active supervisor assignments only. Manager alone receives contract value and may set it during creation. Later contract-value changes are deferred to the existing `project_contract_changes` approval/history model, and `Completed`, `FinanciallyClosed`, and `Cancelled` transitions remain deferred.

Invitation creation uses the validated non-secret `Access:Invitations:LifetimeHours` option, which is 168 hours (7 days) in development. It stores only a SHA-256 hash and returns the URL-safe raw token once to the manager for out-of-band delivery. Raw tokens are never stored or logged.

Invitation acceptance locks the pending invitation and uses only its company, phone, and role. `Supervisor` and `Worker` become active and receive authentication; `Deputy` and `Accountant` remain `PendingApproval` with identity status `Pending` and receive no token.

Public join submission atomically creates a `PendingApproval` `app_user` and linked `Pending` `join_request`; the password hash exists only on `app_users`. Manager approval activates `Supervisor`/`Worker`, while unverified `Deputy`/`Accountant` remain pending identity verification. Rejection marks both the request and linked user rejected. Because phone is globally unique, reapplication or company transfer requires a future approved account-recovery workflow.

Registration requires the existing schema's company code and E.164 manager phone format. It creates the company and its initial `Manager` in one PostgreSQL transaction and returns an access token. Login is phone-only because `app_users.phone_number` is globally unique, while email uniqueness is tenant-scoped.

Refresh tokens and server-side logout are not exposed. The existing `user_devices` table stores device and push-notification metadata but has no refresh-token hash, expiration, rotation, or revocation fields. Clients must discard the short-lived access token to log out until an approved schema decision provides secure session storage.

## Authenticated Flutter shell and access administration

Authenticated routes use one responsive `go_router` shell: `/home`, `/access/invitations`, `/access/join-requests`, and `/account`. Mobile uses a Material 3 `NavigationBar`; layouts at 840 logical pixels and wider use `NavigationRail`. The authoritative role from `GET /api/v1/auth/me` drives centralized `RoleCapabilities`: only `Manager` sees or may route to invitation and join-request administration, while known non-manager and unknown future roles receive the safe Home/Account shell. Backend 403 responses remain authoritative.

Manager invitation administration lists exact page-based API results, filters by verified lifecycle values, creates invitations with phone and a non-manager role, and cancels only pending invitations after confirmation. The raw creation token is held only in the one-time dialog call stack, is copied only by explicit user action, is never persisted or routed, and is discarded when the dialog closes. SMS/email delivery remains intentionally unimplemented.

Manager join-request administration lists and filters paginated requests, then approves a manager-selected non-manager role or rejects with the backend-required reason. Approval surfaces the exact `ApprovedAndActivated` or `ApprovedPendingIdentityVerification` outcome. Deputy and Accountant decisions explain identity verification; no access token or internal tenant/user identifier is displayed or sent.

The shared account page displays safe authoritative current-user, company, role, account, and identity-verification status data, supports Arabic/English switching, and performs local logout by deleting the local access token. A minimal backend DTO correction adds `identityVerificationStatus` to the existing safe user summary; it introduces no endpoint, database, or persistence-model change.

## Flutter projects and company directory

The authenticated shell now adds stable project routes at `/projects`, `/projects/new`, `/projects/:projectId`, `/projects/:projectId/edit`, `/projects/:projectId/supervisor`, and `/projects/:projectId/members`, plus read-only directory routes at `/company/members` and `/company/members/:memberId`. Central `RoleCapabilities` drives both navigation and router guards: Worker and unknown roles receive no project access, project writes are Manager-only, the directory is Manager/Deputy-only, and project-supervisor summaries follow the backend matrix.

Project and member lists use server-side status/role/search filters, page size 20, stable ID deduplication, refresh, recoverable load-more errors, and responsive cards. Searches are capped at 100 characters and one-character searches are never sent. Supervisor empty states explicitly say that only assigned projects appear.

Manager creation uses the backend's atomic `newOwner` flow because no safe owner-directory endpoint exists; Flutter never exposes a raw owner UUID. Contract value is validated and transported as decimal text emitted as an exact JSON number without binary floating-point conversion, and is rendered only when Manager capability allows it. Metadata updates send only changed supported fields with the authoritative `expectedVersion`, expose only `Active` and `Paused`, and never edit contract value. Supervisor replacement selects only server-filtered active exact Supervisors, requires confirmation when replacing, and explains that assignment history is preserved.

The project-members route is labelled as active supervision assignments, not workers or complete project staff. The company directory and member detail are read-only and contain no role, status, deletion, or identity-verification mutations. Both features are localized in Arabic RTL and English LTR and share the same mobile/tablet/Web implementation.

## Flutter advances and user balances

The authenticated shell exposes `/advances`, `/advances/new`, `/advances/:advanceId`, `/advances/:advanceId/movements`, `/advances/:advanceId/distribute`, `/advances/:advanceId/return`, `/advance-balances`, and `/advance-balances/users/:userId`. Central `RoleCapabilities` supplies broad navigation and route guards; loaded advance status, current holder balance, expected transfer recipient, and the API remain authoritative for record actions.

Advance and movement lists use server pagination, exact status/reference/user filters, stable IDs, refresh, and recoverable load-more failures. Supervisor and Worker empty states explain personal participation visibility. Personal and selected-user balance pages render authoritative server rows per advance/currency and never reconstruct balances from movements or total different currencies. Manager and Deputy select authorized users through the safe company directory. Accountant retains API read capability but has no raw-ID UI because Accountant directory discovery is not available.

Manager creation offers only active Deputies and existing usable funding sources. Decimal text is validated as positive `NUMERIC(18,2)`, converted to integer minor units only for exact allocation comparisons, and emitted as JSON numeric tokens without `double`. Manager distribution offers active Deputies; Deputy distribution offers active Supervisors/Workers. Returns expose no destination selector because the server derives the confirmed upstream recipient. Pending movement records identify the expected recipient for confirmation and allow rejection only for `InternalTransfer` and `BalanceReturn`, never initial `AdvanceDelivery`.

Every financial command generates 32 cryptographically secure random bytes with `Random.secure`, sends the URL-safe ephemeral value in `Idempotency-Key`, and keeps it only in an auto-disposed controller. There is no automatic write retry. Network/timeouts produce an explicit uncertain state whose “Retry same operation” reuses the exact payload/key; success, definitive rejection, cancellation, payload replacement, or controller disposal clears it. Keys are never displayed, logged, or persisted.

Automated Flutter tests use fake repositories and controlled Dio adapters only. They do not invoke a real financial endpoint or connect to PostgreSQL.

## Flutter expenses and reimbursements

The authenticated shell now includes Expenses for Manager, Deputy, Accountant, Supervisor, and Worker. Stable guarded routes are `/expenses`, `/expenses/new`, `/expenses/:expenseId`, `/expenses/:expenseId/history`, `/expenses/:expenseId/documents`, `/expenses/:expenseId/review`, `/expense-categories`, `/expense-categories/new`, `/reimbursements`, and `/reimbursements/:reimbursementId`. Unknown roles receive no expense access; Accountant cannot create; only Manager creates categories; only Manager/Deputy/Accountant reach review.

Expense lists use server-side exact status/payment-mode/reference filters, page 1/page size 20 pagination, stable deduplication, refresh, and preserved data on load-more failure. Detail renders only the safe API projection, including one optional direct project, allocations, metadata-only documents, optional item rows, personal claim, reviewer information, and audit history. Document POST is intentionally not exposed because its current contract requires a storage-generated application-relative path and SHA-256 value while no binary storage provider exists.

Creation offers only `AdvanceBalance` and `PersonalFunds`. Project selection follows API-visible projects and category scope, with no Worker project selector. Multiple caller-owned authoritative balances may be allocated; each amount is exact decimal text and the total is checked in integer minor units. The balance DTO now exposes its existing safe `userAdvanceBalanceId`, the minimum backend contract correction required to submit an allocation. `PersonalFunds` creates an unpaid claim and exposes no payment action.

Expense creation, approval, and rejection reuse the existing 32-byte ephemeral financial operation key behavior. Timeout/network outcomes remain uncertain and allow only explicit same-payload retry with the same in-memory key. No automatic write retry, persistence, display, or logging of keys exists. Arabic remains default RTL and English remains LTR.

This phase passes `flutter analyze`, 137 Flutter tests, a Web release build, and an Android debug APK build. The Android application was not launched. iOS build/signing verification remains pending macOS and Xcode.

## Flutter suppliers and payables

The authenticated shell adds `/suppliers` plus guarded supplier detail/edit/account/statement routes and separate invoice, debt, payment, credit-note, and refund routes. Manager has full supplier administration; Deputy has read access and invoice creation; Accountant has financial reads and invoice/payment/credit/account creation; Supervisor receives assigned-project supplier/debt visibility only; Worker and unknown roles receive no supplier destination. Router guards and page actions share centralized `RoleCapabilities`, while backend authorization remains authoritative.

Lists use API pagination and stable-ID deduplication. Payment creation supports exact multi-debt and multi-funding allocation, verified bank/wallet account selection, and proof metadata without claiming binary upload. Invoice quantity uses exact three-decimal arithmetic and monetary requests use exact two-decimal arithmetic; neither is converted through binary floating point. Statements and balances remain separated by currency.

Invoice, payment, confirmation/rejection, credit-note creation/approval/allocation, and supplier account commands use ephemeral cryptographically secure `Idempotency-Key` values. Writes are never automatically retried. Ambiguous network/timeouts expose only an explicit same-payload retry with the same in-memory key; changed payloads receive a new key. Refunds are read-only, and the UI does not invent settlement, reversal, verification, or advance-balance funding workflows.

Automated tests use fakes and controlled network adapters and do not invoke live financial endpoints or PostgreSQL. Web and Android build commands remain below; iOS source compatibility is maintained, while compilation and signing require macOS/Xcode.

This phase passes strict Dart formatting, `flutter analyze`, 189 Flutter tests, a Web release build (including the Wasm compatibility dry run), and an Android debug APK build. The Android application was not launched.

## Run backend tests

```powershell
dotnet test backend\Ahdah.sln
```

## Run Flutter on Web

From `frontend\ahdah_app`:

```powershell
C:\dev\flutter\bin\flutter.bat pub get
C:\dev\flutter\bin\flutter.bat run -d chrome --web-port 5173 --dart-define=API_BASE_URL=http://localhost:5231
```

For a release build:

```powershell
C:\dev\flutter\bin\flutter.bat build web --release --dart-define=API_BASE_URL=http://localhost:5231
```

`API_BASE_URL` is required and has no silent production default. Deployed builds must use HTTPS. Web access tokens are deliberately memory-only, so a browser refresh requires a new login.

## Run Flutter on Android

Start a device or emulator yourself, then run these commands from `frontend\ahdah_app`:

```powershell
flutter doctor
flutter devices
C:\dev\flutter\bin\flutter.bat run -d <android-device-id> --dart-define=API_BASE_URL=http://10.0.2.2:5231
```

The emulator reaches the local HTTP API through `10.0.2.2`. A physical Android device may use `adb reverse tcp:5231 tcp:5231` with `API_BASE_URL=http://127.0.0.1:5231`, or the development computer's LAN address without committing that address. Local cleartext support exists only in Android's debug source set; release networking remains HTTPS-oriented. The repository does not launch or manage an emulator automatically.

The Android debug APK build has passed with Gradle 9.1.0:

```powershell
C:\dev\flutter\bin\flutter.bat build apk --debug --dart-define=API_BASE_URL=http://10.0.2.2:5231
```

The first build downloaded Gradle 9.1.0 and installed the required NDK, Android SDK platforms, and CMake. The resulting APK is `frontend/ahdah_app/build/app/outputs/flutter-apk/app-debug.apk`. This verifies the build artifact only; the app has not been run on an emulator or physical device.

## iOS development

The iOS project permits local-network development through the narrow ATS `NSAllowsLocalNetworking` setting rather than disabling ATS globally. Use `localhost:5231` for the simulator or the development computer's LAN address for a physical device. iOS remains unbuilt on Windows; builds, signing, and simulator/device verification require macOS, Xcode, and the relevant Apple setup.

## Database-First workflow

Restore the repository-local EF tool with `dotnet tool restore`. Generated files under `backend/src/Ahdah.Infrastructure/Persistence/Generated` must not be manually edited. Database changes and re-scaffolding require explicit approval and review. Migrations, `EnsureCreated`, `EnsureDeleted`, and automatic schema updates are prohibited.

See [PROJECT_STATUS.md](PROJECT_STATUS.md), [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md), and [docs/FLUTTER_DEVELOPMENT.md](docs/FLUTTER_DEVELOPMENT.md) for verified status, architecture, security decisions, routes, and run commands. Generated persistence files remain infrastructure-only and are never returned through API contracts. SMS/email invitation delivery, member mutations, project closure/contract-change workflows, and business/financial dashboards remain unimplemented.
