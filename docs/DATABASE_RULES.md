# Database rules

## Existing database

- Database: `ahdah_db`
- Schema: `ahdah`
- Database engine: PostgreSQL 18
- Current table count: 91
- Integration approach: database first
- Source of truth: the existing database

The database was designed and created before this application foundation. Initialization does not connect to it, inspect it, scaffold it, execute SQL, or alter it.

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

## Future integration expectations

A future, explicitly scoped task should add compatible EF Core and Npgsql packages, obtain credentials through a secure local mechanism, validate read-only connectivity, and scaffold only the `ahdah` schema without modifying the database. Generated output and schema assumptions must be reviewed before application use.

No application startup path may apply migrations, call `EnsureCreated`, or repair schema automatically.
