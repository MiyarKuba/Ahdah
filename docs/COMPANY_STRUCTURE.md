# Company structure — projects and member visibility

## Database findings

The PostgreSQL catalog and generated Database-First model were inspected read-only. Relevant mapped tables are:

- `companies`: tenant and active-company boundary.
- `app_users`: company member, exact role/status/identity state, and project actor/supervisor target.
- `project_owners`: mandatory owner/client record with tenant-composite project relationship.
- `projects`: construction project/site metadata and required contract value.
- `project_supervisors`: supervisor assignment history.
- `project_contract_changes`: deferred reviewed contract-value change workflow. Exact types are `Increase`, `Decrease`, and `Correction`; exact statuses are `PendingApproval`, `Approved`, `Rejected`, and `Cancelled`. A partial unique index allows one pending change per project.

Other project-name tables (`project_owner_payments`, owner allocation/refund tables) are financial and unused in this phase. There is no generic project-member, worker-project, user-project assignment, separate site/location, or client table.

`projects` uses UUID primary key `project_id` and unique `(company_id, project_id)`. Required fields are company, owner, name, site address, `contract_value NUMERIC(18,2)`, contract date, start date, status, creator, version, and timestamps. Optional fields are coordinates (both or neither), contact phone, expected/actual end dates, description, notes, completion fields, and cancellation fields. Exact statuses are `Active`, `Paused`, `Completed`, `FinanciallyClosed`, and `Cancelled`.

`project_owners` uses UUID primary key and unique `(company_id, project_owner_id)`. Phone is unique within a company, and non-null email is case-insensitively unique within a company. Name/phone/company/creator/version/timestamps are required. `projects` references the owner through `(company_id, project_owner_id)`.

`project_supervisors` uses a UUID assignment key and tenant-composite foreign keys to project, supervisor, assigner, and optional remover. Active rows have no removal fields; inactive rows require remover, time, and non-blank reason. The schema permits multiple active supervisors as long as the same supervisor is not actively duplicated on one project. API reads therefore use `assignedSupervisors`; the focused replacement route normalizes a managed project to the selected active supervisor while preserving every ended row.

## Visibility matrix

| Role | Company directory | Project list/detail | Contract value | Project members | Mutations |
|---|---|---|---|---|---|
| Manager | All members | All tenant projects | Included | Active supervisors | Create, metadata PATCH, replace supervisor |
| Deputy | All members, read-only | All tenant projects | Omitted | Active supervisors | None |
| Accountant | Denied pending approved directory rule | All tenant projects | Omitted | Denied pending approved allocation rule | None |
| Supervisor | No full directory | Actively assigned projects only | Omitted | Active supervisors on assigned projects | None |
| Worker | No full directory | None; no schema relationship | Omitted | None | None |
| Unknown | Denied | Denied | Omitted | Denied | None |

Authorization policies are an outer gate. Every persistence query still filters `company_id`, and supervisor visibility is expressed in SQL through active assignment predicates before count and pagination.

## API behavior

Company directory:

- `GET /api/v1/company/members`
- `GET /api/v1/company/members/{memberId}`

The list supports exact role/status filters, bounded prefix search, page starting at 1, default page size 20, and maximum 100. List results omit phone/email; detail adds safe contact fields. Security, password, token, lockout, approval, identity-evidence, status-reason, and version internals are never returned.

Projects:

- `GET /api/v1/projects`
- `GET /api/v1/projects/{projectId}`
- `POST /api/v1/projects`
- `PATCH /api/v1/projects/{projectId}`
- `PUT /api/v1/projects/{projectId}/supervisor`
- `GET /api/v1/projects/{projectId}/members`

Project lists use the shared page contract, exact status filtering, bounded project-name/site-address prefix search, newest-first stable ordering, and visible-record total counts. Tenant and assignment filters run in the database. Unavailable or unauthorized tenant-scoped records return a non-enumerating 404.

Manager creation supplies project metadata and required positive two-decimal contract value. It supplies exactly one existing owner ID or a complete new owner. Existing owners and optional supervisors are verified using company and resource ID together. New owner, project, and optional supervisor assignment are committed in one transaction. New status is always `Active`.

PATCH requires `expectedVersion` and permits project name, site address, contact phone, contract/start/expected-end dates, owner replacement, description, notes, and `Active`/`Paused` status. Optional nullable metadata uses null as “unchanged” in this phase; clearing a value is not exposed. Coordinates, actual/completion/cancellation fields, contract value, tenant/creator/timestamp fields, and arbitrary version changes are not patchable.

Supervisor replacement requires `supervisorUserId` and `expectedVersion`. The target must be an active same-company exact `Supervisor`. The service locks the tenant project, ends other active assignments with removal history, adds the new active row if needed, increments project version, and commits atomically. Unassignment is omitted because no approved product behavior requires a supervisor-less transition endpoint.

Project members are paged active supervisor assignments only. No project-member write endpoint exists.

## Flutter consumption

Flutter consumes the published contracts without backend or database changes. Handwritten project/member models use exact camelCase fields, parse UUIDs as opaque strings, parse timestamps defensively, and keep contract value nullable decimal text. The API client uses the existing authenticated Dio interceptor and Problem Details mapping; no request accepts or sends `company_id`.

Stable authenticated routes cover project list/create/detail/edit/supervisor/active-supervision pages and member list/detail pages. `RoleCapabilities` applies the documented matrix to navigation, route guards, Manager actions, project-supervisor actions, and contract-value presentation. Worker and unknown roles have no project destination; Accountant has no project-supervisor or directory destination; Company Members is Manager/Deputy-only.

Project/member lists use server-side exact filters and bounded search, page size 20, stable deduplication, refresh, pagination, stale-result suppression, and recoverable load-more failure. Supervisor empty messaging explains assigned-only visibility. Mobile uses cards and pull-to-refresh; tablet/Web use the same constrained responsive pages with persistent navigation.

Manager creation uses atomic `newOwner` because no safe owner directory is published; it never exposes a raw owner UUID. The optional supervisor picker requests only active exact Supervisors and defensively excludes other results. Updates load the authoritative project, send only supported changed fields with `expectedVersion`, expose only `Active`/`Paused`, and provide a 409 reload flow. Supervisor replacement also uses `expectedVersion`, confirmation, and explicit history-preservation wording.

Project member UI says “Project supervisors” and “active supervision assignments,” never workers or full staff. The company directory is read-only and list items do not invent contact fields omitted by the API. Contract value renders only for Manager capability and is neither calculated nor logged. Arabic/English status, role, identity, navigation, forms, validation, empty, error, and conflict states are localized.

## Deferred behavior

Direct contract-value changes are deferred to the existing `project_contract_changes` review/history workflow. The physical schema permits multiple historical change rows but the generated Project navigation is singular, so that mapping limitation requires explicit review in the future contract-change phase. `Completed`, `FinanciallyClosed`, and `Cancelled` transitions are deferred because they require additional lifecycle fields and, for financial closure, future settlement/debt/invoice/fund/transfer rules. Member role changes, status changes, deletion, identity verification, project participant writes, all financial modules, and Flutter UI are outside this phase.

This Flutter phase changes no PostgreSQL schema object, generated EF file, migration, backend contract, or financial workflow. Existing-owner selection remains deferred until a safe owner-directory API exists. Member mutations, project completion/cancellation/financial closure, contract-value changes, and all financial modules remain deferred.
