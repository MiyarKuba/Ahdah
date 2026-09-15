# Project status

## Project

Ahdah — عُهدة

## Current phase

Settlement and Closure Foundation, Phase 1 — backend read-only readiness

## Date, workspace, and branch

- Date: `2026-09-15`.
- Workspace: `C:\dev\Ahdah`.
- Branch: `feat/settlement-closure`.
- Baseline: `115cc5a544f18cbc11213f1e080ab9599128cc72` (`feat: add Flutter suppliers and payables workflows`).
- The pre-existing settlement branch was safely fast-forwarded from `392cddb` to the verified baseline. Existing main and all prior history were preserved.
- Pre-existing untracked `frontend/ahdah_app/devtools_options.yaml` remains untouched and unstaged.

## Delivered scope

- `GET /api/v1/projects/{projectId}/settlement` with explicit safe DTOs, stable codes, record references, current project status/version, financial blocker counts, currency-separated category totals and evaluation gaps.
- Expense review/document, reimbursement, supplier debt/payment/credit, claim-payment, expense-return/refund, project-linked manager-contribution claim, and Manager-only pending owner-operation findings.
- Centralized readiness summary distinguishes operational completion, known financial blockers, missing evaluation, cancellation and already-closed status.
- One PostgreSQL RepeatableRead, READ ONLY transaction provides a consistent view of membership, assignments and category records. No schema, generated model, ledger or financial state writes.
- Existing expense document threshold policy extracted for shared use without changing approval behavior.
- Company-scoped access for Manager/Deputy/Accountant; Supervisor limited to assigned-project expense/document/debt findings; Worker and unknown roles denied. Other people's claims/payments are not exposed to Supervisor through IDs, amounts or counts.
- Backend tests exercise the real service and queries with isolated in-memory data, PostgreSQL SQL translation, HTTP authorization and OpenAPI. EF Core InMemory 10.0.10 is a test-only dependency.

## Material limitations and decisions

- Advances, user balances, distributions/returns and advance settlement/closure records have no authoritative project ownership relationship. Funding origin or expense use does not assign the remaining advance to a project.
- Manager contributions are not assumed repayable; only directly project-linked personal claims establish assessed obligations. No dedicated manager-settlement table exists.
- Project settlement and final closure transition policies are undefined. `canSettle`/`canClose` are false for known impediments and null when indeterminate; Phase 1 never certifies finalization. `hasKnownFinancialBlockers=false` is not full financial clearance.
- Pending payment allocations are operations, not reductions in computed outstanding debt. Category totals cannot be summed into a combined payable amount.
- Approved unallocated credits are not blockers. Unallocated pending credit intent and complete approved-return effects cannot be attributed/reconciled safely in every case.
- Expense metadata rules can be checked; original-paper custody and actual binary file verification cannot.
- No Flutter source or placeholder was added. No settlement/closure command, migration, generated-model edit, new PostgreSQL object, or financial-rule change was introduced.
- Full contract, schema findings, role matrix and follow-up details: [docs/SETTLEMENT.md](docs/SETTLEMENT.md).

## Verification

- Required `dotnet restore`: passed.
- Required `dotnet build`: passed, 0 warnings and 0 errors.
- Required `dotnet test`: passed, **356 total** (148 unit, 208 integration), 0 failed and 0 skipped.
- Added **96 tests**: 77 real-service/query cases, 13 HTTP/OpenAPI cases and 6 application-rule cases.
- PostgreSQL read-only verification: all 10 category queries executed successfully for a synthetic tenant, and the complete read-only service returned Success for an existing eligible project. No record identifiers, financial data or credentials were emitted. No PostgreSQL mutation occurred.
- Tests cover lifecycle exclusions, pending operations, exact decimals, mixed currencies, allocation scope, missing evidence, tenant boundaries, assigned/unassigned Supervisor access, Worker/unknown denial, stale membership and absent projects. In-memory fixtures do not validate PostgreSQL constraints or concurrent transactions; SQL translation and read-only PostgreSQL execution were checked separately. No concurrent-write stress test was performed.
- `git diff --check`: passed. Final diff was reviewed for scope; temporary inspection tooling was removed. Generated persistence and Flutter source are unchanged.

The multi-document fixture seeds separate units of work because the generated navigation is singular while the physical table permits multiple documents. The production reader queries the table directly.

Flutter, Android and iOS were not rebuilt or runtime-tested in this backend-only phase. The historical baseline verification was 260 backend tests and 189 Flutter tests; these are not claims of new platform verification.

## Exact recommended next task

Implement a read-only Flutter Project Settlement screen consuming the completed endpoint, with Arabic/English stable-code labels, exact decimal rendering, currency-separated category totals, permitted record navigation, and explicit Blocked/Indeterminate/NotVisible/NotAttributable states. Do not add finalize/close actions. Separately obtain approved project advance-attribution and lifecycle rules before designing any financial command.
