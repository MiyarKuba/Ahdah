# Ahdah — عُهدة

Ahdah is a multi-tenant platform for financial custody and construction operations. It is intended to help companies coordinate projects, custody balances, transfers, expenses, supplier obligations, worker claims, documents, audit trails, notifications, and scheduled reporting.

This repository is currently in **Identity and Company Access — Phase 1**. The backend now supports company bootstrap registration, initial-manager creation, phone/password login, short-lived JWT access tokens, tenant-aware current-user resolution, and core authorization policies on top of the existing Database-First model.

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

Registration requires the existing schema's company code and E.164 manager phone format. It creates the company and its initial `Manager` in one PostgreSQL transaction and returns an access token. Login is phone-only because `app_users.phone_number` is globally unique, while email uniqueness is tenant-scoped.

Refresh tokens and server-side logout are not exposed. The existing `user_devices` table stores device and push-notification metadata but has no refresh-token hash, expiration, rotation, or revocation fields. Clients must discard the short-lived access token to log out until an approved schema decision provides secure session storage.

## Run backend tests

```powershell
dotnet test backend\Ahdah.sln
```

## Run Flutter on Web

From `frontend\ahdah_app`:

```powershell
flutter pub get
flutter run -d chrome
```

For a release build:

```powershell
flutter build web
```

## Run Flutter on Android

Start a device or emulator yourself, then run these commands from `frontend\ahdah_app`:

```powershell
flutter doctor
flutter devices
flutter run -d <android-device-id>
```

The repository does not launch or manage an emulator automatically.

## iOS development

The iOS project is preserved under `frontend\ahdah_app\ios`, but iOS builds and tests require macOS, Xcode, and the relevant Apple signing setup. They cannot be performed on Windows.

## Database-First workflow

Restore the repository-local EF tool with `dotnet tool restore`. Generated files under `backend/src/Ahdah.Infrastructure/Persistence/Generated` must not be manually edited. Database changes and re-scaffolding require explicit approval and review. Migrations, `EnsureCreated`, `EnsureDeleted`, and automatic schema updates are prohibited.

See [PROJECT_STATUS.md](PROJECT_STATUS.md) for verified status and [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) for the system shape. Generated persistence files remain infrastructure-only and are never returned through API contracts. Invitations, join requests, and Flutter authentication UI are not implemented in this phase.
