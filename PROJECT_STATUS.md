# Project status

## Project

Ahdah — عُهدة

## Current phase

PostgreSQL Database-First Integration

## Date

2026-07-23

## Completed work

- Added repository-local `dotnet-ef` 10.0.10.
- Added `Microsoft.EntityFrameworkCore.Design` 10.0.10 and `Npgsql.EntityFrameworkCore.PostgreSQL` 10.0.3 with development-only Design metadata.
- Reverse-engineered only the existing PostgreSQL 18 `ahdah_db` database's `ahdah` schema through configuration by name.
- Isolated the generated context at `backend/src/Ahdah.Infrastructure/Persistence/Generated/Context/AhdahDbContext.cs` and 91 generated entity files under `Persistence/Generated/Entities`.
- Registered `AhdahDbContext` manually through `AddAhdahPersistence`, targeting PostgreSQL 18.0 without sensitive-data logging, retry behavior, migrations, or database initialization.
- Added offline model-mapping and source-safety integration tests.

## Verification results

- Generated model: 91 distinct mapped tables; every mapped schema is `ahdah`.
- User Secret configuration key: resolved successfully without displaying its value.
- `dotnet tool restore` and `dotnet restore backend/Ahdah.sln`: passed.
- `dotnet build backend/Ahdah.sln`: passed with 0 warnings and 0 errors.
- `dotnet test backend/Ahdah.sln`: passed; 5 total tests, 5 passed, 0 failed, 0 skipped.
- EF design-time `dbcontext info`: passed; Npgsql provider, `ahdah_db`, and PostgreSQL 18.0 options were resolved.
- Database mutation status: none; no mutation command was executed.
- Migrations status: none.

## Known warnings

- Npgsql scaffolding cannot represent 22 PostgreSQL expression indexes in the EF model. The indexes remain in PostgreSQL and must not be recreated through migrations.
- The `report_delivery_attempts(company_id, report_delivery_id, background_job_id)` relationship targets a unique index whose `background_job_id` column is nullable; EF warned that principal properties for this relationship must be non-nullable. The generated mapping requires review before business use of that relationship.
- Database integration provides persistence structure only; authentication, authorization, tenant enforcement behavior, and business features remain unimplemented.

## Exact recommended next task

Implement the Identity and Company Access foundation using the existing app_users, companies, invitations, and join_requests tables without changing the database schema.
