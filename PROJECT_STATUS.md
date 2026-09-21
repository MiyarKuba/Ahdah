# Project status

## Financial control and custody backend — 2026-09-21

The verified PostgreSQL settlement schema has been scaffolded Database-First from `127.0.0.1:55432/ahdah_settlement_staging` on `feat/financial-control-custody`. The generated model now maps 117 `ahdah` tables, including T01-T26 and the reviewed currency columns. Handwritten partial configuration marks mutable T01, T02, T03, and T05 `version_number` properties as concurrency tokens; PostgreSQL remains authoritative for deferred triggers, immutable history, expression indexes, and `UNIQUE NULLS NOT DISTINCT` behavior.

New projects create T01 in their project transaction with revision 1, state `Open`, cycle 1, and attribution `PendingReview`. `ProjectFinancialGate` provides tenant-scoped reads, PostgreSQL UUID-ordered `FOR UPDATE` locks, the `FinanciallyClosed` guard, and one revision/version/time update per logical project command. Missing legacy T01 fails closed and is not synthesized.

AdvanceDelivery confirmation now creates one unassigned T02 position, a Posted T03 Opening, and its immutable T04 receipt from the authoritative `BalanceCreated` entry in the same transaction. The custody API exposes tenant-safe balance and project reads plus Accountant/Deputy Designate request and Manager approval. Posting Designate locks T01, balance, and positions in canonical order; validates the original idempotent request and active Policy v1 binding; records Manager T12 evidence; posts paired conserving T04 effects; and increments the destination project revision once. Replays do not create effects or revisions. InternalTransfer and BalanceReturn fail closed when either affected holder already has T02 custody until designation-preserving writer integration is implemented.

Real PostgreSQL tests use disposable `ahdah_custody_app_test`, cloned from the validated final rehearsal schema because the older staging clone still held the pre-fix PL/pgSQL alias body. They cover atomic project/T01 creation, live Opening and replay, Designate roles/approval/replay/rollback, reads, exact conservation, missing T01, the closed-state guard, transfer/return compatibility, and concurrent oversubscription. Release, Reallocate, Correction, universal writer integration, settlement approval, closure, reopening, and global enforcement remain disabled/deferred. Flutter and production `ahdah_db` are unchanged.

## Staging validation gate — 2026-09-20

**SETTLEMENT PHYSICAL DDL VALIDATED FOR APPLICATION DEVELOPMENT.** All six isolated PostgreSQL 18 core areas pass on branch `test/settlement-staging-ddl`: catalog, runtime read-only privileges, custody conservation, packet sealing, live/legacy Opening plus Manager-authorized Designate, and exact pre-cutover rollback. The custody function uses descriptive locals/aliases and all 11 settlement functions compile with variable-conflict errors enabled. Rollback requires the captured pre-install schema-USAGE state and restored the isolated runtime signature exactly. See [the validation record](docs/SETTLEMENT_STAGING_DDL_VALIDATION.md). The existing `ahdah_db` service was not mutated and enforcement was not activated. This milestone does not authorize production rollout or financial closure.

## Current phase

**Project financial control and custody attribution backend foundation implemented over the verified DDL.** Policy v1 meaning is unchanged. No settlement approval or financial closure capability has been enabled.

## Baselines and branch

- Date: 2026-09-17; workspace: `C:/dev/Ahdah`.
- Policy: `f962d6119281906b5144b80fe1ffdac7e4525d9e`.
- Reviewed schema design: `a8df2e8d48c110959c172a1f35835ee973b495aa`.
- Reviewed design branch: `design/settlement-ddl-artifacts`; validated DDL branch: `test/settlement-staging-ddl`, based on commit `a482cba2bd1f83fcbd3217b2638d27237af2903a`.
- Existing history preserved. `frontend/ahdah_app/devtools_options.yaml` remains unread, untouched and untracked.

## Delivered artifacts

- [database/settlement](database/settlement/README.md): 18 SQL files covering all 26 new tables, five reviewed existing-table changes, forward dependency order, least-privilege installation, metadata preflight/verification, authorized future business reconciliation, synthetic structural rehearsal, staged enforcement and pre-cutover empty-schema rollback.
- 420 new-table columns; 500 named table constraints (plus eleven deferred constraint-trigger entries, excluding implicit PostgreSQL NOT NULL entries); 219 indexes; 61 triggers; eleven new functions. Two deployed trigger functions reused without modification.
- [DDL review](docs/SETTLEMENT_DDL_REVIEW.md), [index workload register](database/settlement/INDEX_REVIEW.md), [24 fixture specifications](database/settlement/REHEARSAL.md) and [seven two-session race schedules](database/settlement/RACES.md).
- Schema installation, legacy population, invariant verification, enforcement activation and operational enablement are separate. New runtime tables remain read-only after installation. All applicable entries in the 48-writer inventory still require upgrades before closure.

## Earlier DDL-design catalog and static verification

- Recaptured catalog metadata in READ ONLY/ROLLBACK: PostgreSQL 18.4, ahdah_db/ahdah, 91 tables. No differences found in the 49 recorded relevant table signatures/keys/FKs/triggers.
- Metadata-only preflight executed successfully. It checks 7,966 metadata signature rows using fingerprint `d697e4fbc6699faed6076d576a814d38`, version/schema/functions/table count and proposed object-name conflicts. No customer/business rows or sequence values queried; no credentials printed or committed.
- Local pglast 8.4 SQL/native PL/pgSQL parsing completed without syntax errors. Its high-level trigger-AST JSON decoder has a reproducible wrapper error; native parser syntax acceptance is recorded separately from execution/binding validation.
- Static dependency, tenant FK column/unique-key coverage, identifier length, duplicate names, artifact inventories and documentation checks performed. Working/staged diff checks and final intended-file scope are verified before commit.
- At that earlier DDL-design milestone, no DDL, schema/data mutation, privilege change, fixture execution, invariant/business-row query, migration, EF scaffold, C#, Dart, ARB, or generated-model change was made. No application builds/tests ran, and no safe disposable database was created or assumed; its 24 fixtures and seven races were specifications rather than execution passes. The current implementation and PostgreSQL execution results are recorded above.

## Resolved clarification and residual risks

The owner-approved physical-design correction resolves Opening authorization: confirmed AdvanceDelivery/BalanceCreated receipt or approved evidenced legacy Balance review establishes unassigned custody only. Separate Designate remains Manager-authorized and gated. Project Correction uses CorrectCustody project authority; company-only Correction uses existing approved balance-resolution or legacy-review authority. See the DDL review for source integrity and unchanged Policy v1.

Trigger behavior, database binding, privileges, packet sealing concurrency and all application writers need separately authorized staging proof. The SQL harness provides structural probes; future command-level fixture bindings must test the actual missing commands. The approved schema has no per-company enforcement activation flag, so companies may be prepared in phases but structural activation waits for all-company coverage. Runtime ownership/inherited privileged roles and unrestricted legacy writers remain blockers to closure activation.

## Exact next task

Upgrade the remaining project-affecting financial writers to the shared T01 gate and financial-revision protocol, starting with expenses, claims/reimbursements, supplier debt/invoices, and their payment/allocation paths. Do not begin settlement approval or financial closure until universal writer coverage is complete.
