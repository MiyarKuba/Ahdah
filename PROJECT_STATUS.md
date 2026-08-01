# Project status

## Project

Ahdah — عُهدة

## Current phase

Company Structure — Projects and Member Visibility

## Date

2026-08-01

## Actual schema findings

- `projects` is the construction project/site table. It requires tenant, owner, project name, site address, positive `contract_value NUMERIC(18,2)`, contract/start dates, status, creator, `version_number`, and timestamps.
- Exact project statuses are `Active`, `Paused`, `Completed`, `FinanciallyClosed`, and `Cancelled`. Completion and cancellation statuses require companion fields.
- `project_owners` is the required client/owner record, linked by tenant-composite `(company_id, project_owner_id)`. Phone is tenant-unique; non-null email is tenant-scoped case-insensitive unique.
- `project_supervisors` is a history table. Active rows have no removal data; inactive rows require remover, time, and reason. It has no version field.
- `project_contract_changes` is a separate reason/review/history model with types `Increase`/`Decrease`/`Correction`, statuses `PendingApproval`/`Approved`/`Rejected`/`Cancelled`, and one pending row per project. It is not implemented in this non-financial phase; its singular generated navigation also needs future mapping review before history queries.
- No generic project-member, worker-project, user-project assignment, client, or separate site table exists. Project members can only mean active supervisor assignments.

Tables used by runtime services: `companies`, `app_users`, `project_owners`, `projects`, and `project_supervisors`. `project_contract_changes` was inspected but is not written.

## Endpoints implemented

- Manager/Deputy `GET /api/v1/company/members`
- Manager/Deputy `GET /api/v1/company/members/{memberId}`
- Role/assignment-filtered `GET /api/v1/projects`
- Role/assignment-filtered `GET /api/v1/projects/{projectId}`
- Manager-only `POST /api/v1/projects`
- Manager-only `PATCH /api/v1/projects/{projectId}`
- Manager-only `PUT /api/v1/projects/{projectId}/supervisor`
- Schema-limited `GET /api/v1/projects/{projectId}/members`

Omitted endpoints: member mutation/verification routes; project-member assignment writes; supervisor unassignment; project completion, financial closure, and cancellation; contract-value change workflow; owner CRUD. Reasons are missing approved behavior, absent schema relationship, required history/financial workflow, or out-of-phase scope.

## Visibility and security

| Role | Directory | Projects | Contract value | Project members | Writes |
|---|---|---|---|---|---|
| Manager | All | All | Included | Active supervisors | Create/update/replace supervisor |
| Deputy | All | All | Omitted | Active supervisors | None |
| Accountant | Denied | All | Omitted | Denied | None |
| Supervisor | Denied | Active assignments only | Omitted | Assigned-project supervisors | None |
| Worker | Denied | None; schema has no link | Omitted | None | None |

`CompanyDirectoryViewer` and `ProjectViewer` policies were added. Existing `ManagerOnly` remains the mutation policy. Policies never replace tenant/record filtering.

Contract value is manager-only and JSON-omitted from non-manager responses. Manager must supply a positive, two-decimal value during creation. Direct changes are deferred because the existing contract-change history/review table must not be bypassed.

## Project behavior

Creation derives company and creator from validated current-user context, accepts exactly one existing owner ID or new owner contract, verifies optional supervisor by tenant/ID/role/status, starts `Active`, and uses one transaction for owner/project/assignment state. PostgreSQL `23505` maps to safe 409.

PATCH requires expected version and supports safe non-financial metadata plus only `Active`/`Paused` status. Terminal/cancellation/financial fields and contract value are excluded. Nullable optional metadata cannot be cleared in this contract; null means unchanged.

Supervisor replacement locks the tenant project, verifies expected version and an active exact Supervisor, non-destructively ends prior active assignments, adds the selected assignment if required, increments project version, and commits atomically. Project member reads return paginated active supervisors only.

## Pagination and filtering

All new lists reuse `items`, `page`, `pageSize`, `totalCount`, and `totalPages`; page starts at 1, default size is 20, maximum is 100. Member role/status and project status accept only exact verified values. Searches are trimmed, 2–100 characters, prefix-based, and parameterized. Tenant/visibility filters run before count and page selection.

## Packages and verification

- Package changes: none.
- Restore: `dotnet restore backend\Ahdah.sln` passed; all projects were up to date.
- Build: `dotnet build backend\Ahdah.sln` passed with 0 warnings and 0 errors.
- Tests: `dotnet test backend\Ahdah.sln` passed; 114 total (63 unit, 51 integration), 0 failed, 0 skipped.
- OpenAPI: development document builds in integration tests and exposes explicit DTOs/routes without generated entities or password/hash fields.
- PostgreSQL mutation status: none. Only a `BEGIN TRANSACTION READ ONLY` catalog inspection followed by `ROLLBACK` was executed; no real write endpoint was called.
- Generated EF status: unchanged.
- Migration status: none created or run. No `EnsureCreated`, `EnsureDeleted`, or `Database.Migrate` exists.
- Flutter status: unchanged; no Flutter command was run.

## Warnings and limitations

- Worker project visibility cannot be implemented until an approved genuine worker/project relationship exists.
- Accountant directory and project-member access remain denied because current business documentation does not approve them.
- The database permits multiple distinct active supervisors; API reads expose an array. The focused replacement route normalizes managed replacements to the selected active supervisor while retaining history.
- Owner list/management is not exposed; project creation can atomically create an owner or reuse a known active tenant owner ID.
- Contract change, completion, financial closure, cancellation, and all financial balances/workflows remain deferred.

## Exact recommended next task

Implement the Flutter projects/sites and company-member directory UI using the completed company-structure APIs, without implementing financial advances or expenses yet.
