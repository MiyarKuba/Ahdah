# Database rules

## Existing database

- Database: `ahdah_db`
- Schema: `ahdah`
- Database engine: PostgreSQL 18
- Current table count: 91
- Integration approach: database first
- Source of truth: the existing database

The database was designed and created before the application. EF Core Database-First integration now maps the existing schema without changing it. The expected mapped base-table count is 91.

## Permanent safeguards

- Do not use EF Core `EnsureCreated`.
- Do not create or run automatic migrations.
- Do not make destructive schema changes.
- Do not assume application models can redefine the existing schema.
- Review generated database-first mappings before accepting them.
- Separate generated persistence mappings from domain and application policy.
- Every tenant-owned query and command must enforce `company_id`.
- Financial values must use C# `decimal` and PostgreSQL `NUMERIC`; never use `float` or `double`.
- Financial ledger records must not be silently edited or deleted.
- Financial changes must be transactional and auditable.
- Connection strings, passwords, and other secrets must not be committed.
- Flutter must never receive database credentials or connect directly to PostgreSQL.

## Database-First integration

Generated context and entity files are isolated under `Ahdah.Infrastructure/Persistence/Generated` and must not be manually edited. Custom persistence extensions belong outside that directory. Re-scaffolding requires a dedicated branch, explicit approval, and review.

The API obtains `ConnectionStrings:AhdahDatabase` through ASP.NET Core configuration. Local development uses .NET User Secrets; production must use environment-based secret configuration. Credentials must never appear in repository files or generated source.

No application startup path may apply migrations, call `EnsureCreated`, or repair schema automatically.
