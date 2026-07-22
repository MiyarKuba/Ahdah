# Ahdah agent rules

These instructions apply to every future automated or assisted task in this repository.

## Workspace safety

- Work only inside the current Ahdah workspace.
- Never modify another project or repository.
- Preserve Git history and unrelated user changes.
- Do not use destructive Git operations.
- Do not push to GitHub unless explicitly requested.
- Never commit secrets.
- Use environment variables or .NET user secrets for local secrets.

## Database safety

- Do not change database structure unless explicitly requested.
- The PostgreSQL database name is `ahdah_db`.
- The PostgreSQL schema is `ahdah`.
- The database currently contains 91 tables.
- Use Database-First when database integration begins.
- Never use EF Core `EnsureCreated`.
- Never run migrations automatically.
- Do not store or expose database credentials in Flutter.
- Flutter must never connect directly to PostgreSQL.

## Tenant and financial safety

- Every tenant-owned query and command must enforce `company_id`.
- Never use `float` or `double` for financial values.
- Use `decimal` in C# and `NUMERIC` in PostgreSQL.
- Keep financial operations transactional and auditable.
- Do not silently edit or delete financial ledger history.
- Do not change business rules without explicit approval.
- Report any ambiguity rather than inventing a financial rule.

## Architecture and platforms

- Respect the dependency direction documented in `docs/ARCHITECTURE.md`.
- Keep business policy out of API controllers and infrastructure details out of the domain.
- Android, iOS, and Web are first-class targets.
- Do not add Android-only behavior without an iOS-compatible path.
- Keep platform-specific code isolated.
- Do not couple Flutter contracts to persistence models.

## Quality and task completion

- Keep changes within the explicit task scope.
- Add tests proportional to each behavior change.
- Run applicable builds, static analysis, and tests after changes.
- Do not claim a check passed unless it actually completed successfully.
- Update `PROJECT_STATUS.md` after every completed task.
- Document material assumptions, unresolved risks, and recommended follow-up work.
