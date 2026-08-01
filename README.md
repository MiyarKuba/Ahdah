# Ahdah — عُهدة

Ahdah is a multi-tenant platform for financial custody and construction operations. It is intended to help companies coordinate projects, custody balances, transfers, expenses, supplier obligations, worker claims, documents, audit trails, notifications, and scheduled reporting.

This repository is currently in **Company Structure — Projects and Member Visibility**. The backend now exposes tenant-safe company-directory and construction-project APIs, including schema-backed owner creation/reuse and supervisor replacement history. Flutter project/member UI and all financial workflows remain deferred.

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

The emulator reaches the local HTTP API through `10.0.2.2`. A physical device uses the development computer's LAN address without committing that address. Local cleartext support exists only in Android's debug source set; release networking remains HTTPS-oriented. The repository does not launch or manage an emulator automatically.

The Android debug APK build has passed with Gradle 9.1.0:

```powershell
C:\dev\flutter\bin\flutter.bat build apk --debug --dart-define=API_BASE_URL=http://10.0.2.2:5231
```

The first build downloaded Gradle 9.1.0 and installed the required NDK, Android SDK platforms, and CMake. The resulting APK is `frontend/ahdah_app/build/app/outputs/flutter-apk/app-debug.apk`. This verifies the build artifact only; the app has not been run on an emulator or physical device.

## iOS development

The iOS project permits local-network development through the narrow ATS `NSAllowsLocalNetworking` setting rather than disabling ATS globally. Use `localhost:5231` for the simulator or the development computer's LAN address for a physical device. iOS remains unbuilt on Windows; builds, signing, and simulator/device verification require macOS, Xcode, and the relevant Apple setup.

## Database-First workflow

Restore the repository-local EF tool with `dotnet tool restore`. Generated files under `backend/src/Ahdah.Infrastructure/Persistence/Generated` must not be manually edited. Database changes and re-scaffolding require explicit approval and review. Migrations, `EnsureCreated`, `EnsureDeleted`, and automatic schema updates are prohibited.

See [PROJECT_STATUS.md](PROJECT_STATUS.md), [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md), and [docs/FLUTTER_DEVELOPMENT.md](docs/FLUTTER_DEVELOPMENT.md) for verified status, architecture, security decisions, routes, and run commands. Generated persistence files remain infrastructure-only and are never returned through API contracts. SMS/email invitation delivery, manager administration UI, and business dashboards remain unimplemented.
