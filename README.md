# Ahdah — عُهدة

Ahdah is a multi-tenant platform for financial custody and construction operations. It is intended to help companies coordinate projects, custody balances, transfers, expenses, supplier obligations, worker claims, documents, audit trails, notifications, and scheduled reporting.

This repository is currently in the **PostgreSQL Database-First Integration** phase. The backend has secure EF Core/Npgsql structural integration; business features and authentication have not started.

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

For local database tooling or API startup, configure `ConnectionStrings:AhdahDatabase` in the API project's .NET User Secrets. Never store the value in repository files. Production must supply it through environment-based secret configuration.

## Build the backend

From the repository root:

```powershell
dotnet restore backend\Ahdah.sln
dotnet build backend\Ahdah.sln
```

The development-only OpenAPI document is exposed by the API host. The database-independent health route is `GET /api/system/health`.

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

See [PROJECT_STATUS.md](PROJECT_STATUS.md) for verified status and [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) for the system shape. Database integration is infrastructure only and is not business-feature implementation.
