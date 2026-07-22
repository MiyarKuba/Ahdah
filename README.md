# Ahdah — عُهدة

Ahdah is a multi-tenant platform for financial custody and construction operations. It is intended to help companies coordinate projects, custody balances, transfers, expenses, supplier obligations, worker claims, documents, audit trails, notifications, and scheduled reporting.

This repository is currently in the **Foundation / Initialization** phase. It contains buildable application shells and architecture guidance only; business features, authentication, and database integration have not started.

## Technology stack

- Client: Flutter stable and Dart, targeting Android, iOS, and Web
- API: controller-based ASP.NET Core Web API on .NET 10 and C#
- Architecture: modular monolith with explicit internal boundaries
- Database in a later phase: PostgreSQL 18, Entity Framework Core, and Npgsql using a database-first workflow

No database package, connection string, credential, entity, scaffold, or migration is part of this foundation.

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

PostgreSQL is not required for the foundation phase. Do not request or configure database credentials yet.

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

## Current phase

Foundation / Initialization. See [PROJECT_STATUS.md](PROJECT_STATUS.md) for verified status and [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) for the intended system shape.

**Database integration has not started.** The existing PostgreSQL database remains the source of truth and was not accessed or modified during initialization.
