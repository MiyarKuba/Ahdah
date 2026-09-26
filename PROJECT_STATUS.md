# Project status

## Project financial write gate — Wave 2A — 2026-09-26

The implemented Wave 2A surface is the existing advance `InternalTransfer` and `BalanceReturn` lifecycle: initiation/reservation, confirmation, and rejection. Attributed balances now require explicit T02 custody-position allocations whose exact decimal amounts total the transfer. Each phase discovers the complete project set, locks T01 in UUID order, then locks balances and positions canonically. It posts immutable `ReserveTransfer`, `ConfirmTransfer`, or `RejectTransfer` T03/T04 history and T05 reservation state atomically with legacy balances and advances each affected project revision once per committed material command. Confirmation preserves each project or unassigned designation at the recipient, including a zero unassigned position required for a newly created recipient balance. Rejection restores the exact reserved slices. Missing allocations, missing/closed gates, insufficient custody, reservation mismatch, and failed commands leave no partial history or revision.

Inventory confirmed that expense returns, supplier refunds, owner-payment refunds, personal-claim adjustment/write-off/payment, and supplier-debt adjustment/write-off/reversal are not published writers. Their schema is read-only or future-only, so Wave 2A does not invent commands or post T22 financial-resolution effects. Flutter, generated EF files, and settlement DDL remain unchanged; enforcement, settlement approval, and financial closure remain disabled.

Verification used only PostgreSQL 18 at `127.0.0.1:55440`. Four environment-gated real PostgreSQL tests executed, including the new multi-project transfer/return race; all passed. `dotnet restore`, `dotnet build` with zero warnings/errors, and the full suite passed: 148 unit plus 214 integration tests, 362 total, zero failed or skipped. PostgreSQL exposed and prevented an initially missing recipient-balance key and the mandatory unassigned-position invariant before partial history could commit; both implementations were corrected. The disposable `ahdah_custody_app_test` database was dropped and absence verified. Production `ahdah_db` was not targeted, and `94_activate_enforcement.sql` was not run.

## Project financial write gate — Wave 1 — 2026-09-22

The existing expense and supplier writer surfaces now use `ProjectFinancialGate` in their authoritative PostgreSQL transactions. Covered commands are project expense create/approve/reject (including the expense-owned personal claim create/cancel and supplier-debt rejection effect), supplier invoice/debt create, supplier payment create/confirm/reject across one or more debt projects, and approved supplier-credit allocation. Missing T01 and `FinanciallyClosed` fail closed. Multi-project commands discover the complete project set in one query, lock T01 in database UUID order, revalidate locked debt lineage, and advance each distinct project exactly once after successful domain validation. Exact idempotent replay and failed commands do not advance revision.

Pending supplier payments remain excluded from `paid_amount` and computed debt outstanding while their allocations still advance project revision as a settlement-readiness change. Pending expense document metadata is nonmaterial; verification/revocation, evidence exceptions, and paper custody remain deferred. No existing public command supports expense reassignment/edit/correction/cancel/reverse, claim payment/adjustment/write-off, supplier debt adjustment/write-off, or payment reversal, so these remain explicit writer inventory rather than invented APIs.

Database-First generated files and settlement DDL are unchanged. Flutter is unchanged. Global enforcement, settlement approval, financial closure, reopening, and `canSettle=true` remain disabled. `financial_revision` is the current staleness authority for future snapshots.

Verification used only the repository-local PostgreSQL 18 endpoint `127.0.0.1:55440` and a disposable `ahdah_custody_app_test` clone. Both focused real-PostgreSQL Wave 1 tests passed and executed their database paths: the expense atomic/replay/concurrent-review test and the supplier multi-project payment/allocation/debt test. The first runs exposed two EF query-ordering translation failures, and fixture timestamps exposed two PostgreSQL verified-after-creation constraints; these were corrected before the final run. `dotnet restore`, `dotnet build` (zero warnings/errors), and the full `dotnet test` suite passed: 361 backend tests, zero failed or skipped. The disposable database was dropped and its absence verified. No command targeted production `ahdah_db`; `94_activate_enforcement.sql` was not run.

## Financial control and custody backend — 2026-09-21

The verified PostgreSQL settlement schema has been scaffolded Database-First from `127.0.0.1:55432/ahdah_settlement_staging` on `feat/financial-control-custody`. The generated model now maps 117 `ahdah` tables, including T01-T26 and the reviewed currency columns. Handwritten partial configuration marks mutable T01, T02, T03, and T05 `version_number` properties as concurrency tokens; PostgreSQL remains authoritative for deferred triggers, immutable history, expression indexes, and `UNIQUE NULLS NOT DISTINCT` behavior.

New projects create T01 in their project transaction with revision 1, state `Open`, cycle 1, and attribution `PendingReview`. `ProjectFinancialGate` provides tenant-scoped reads, PostgreSQL UUID-ordered `FOR UPDATE` locks, the `FinanciallyClosed` guard, and one revision/version/time update per logical project command. Missing legacy T01 fails closed and is not synthesized.

AdvanceDelivery confirmation now creates one unassigned T02 position, a Posted T03 Opening, and its immutable T04 receipt from the authoritative `BalanceCreated` entry in the same transaction. The custody API exposes tenant-safe balance and project reads plus Accountant/Deputy Designate request and Manager approval. Posting Designate locks T01, balance, and positions in canonical order; validates the original idempotent request and active Policy v1 binding; records Manager T12 evidence; posts paired conserving T04 effects; and increments the destination project revision once. Replays do not create effects or revisions. InternalTransfer and BalanceReturn fail closed when either affected holder already has T02 custody until designation-preserving writer integration is implemented.

Real PostgreSQL tests use disposable `ahdah_custody_app_test`, cloned from the validated final rehearsal schema because the older staging clone still held the pre-fix PL/pgSQL alias body. They cover atomic project/T01 creation, live Opening and replay, Designate roles/approval/replay/rollback, reads, exact conservation, missing T01, the closed-state guard, transfer/return compatibility, and concurrent oversubscription. Release, Reallocate, Correction, universal writer integration, settlement approval, closure, reopening, and global enforcement remain disabled/deferred. Flutter and production `ahdah_db` are unchanged.

## Staging validation gate — 2026-09-20

**SETTLEMENT PHYSICAL DDL VALIDATED FOR APPLICATION DEVELOPMENT.** All six isolated PostgreSQL 18 core areas pass on branch `test/settlement-staging-ddl`: catalog, runtime read-only privileges, custody conservation, packet sealing, live/legacy Opening plus Manager-authorized Designate, and exact pre-cutover rollback. The custody function uses descriptive locals/aliases and all 11 settlement functions compile with variable-conflict errors enabled. Rollback requires the captured pre-install schema-USAGE state and restored the isolated runtime signature exactly. See [the validation record](docs/SETTLEMENT_STAGING_DDL_VALIDATION.md). The existing `ahdah_db` service was not mutated and enforcement was not activated. This milestone does not authorize production rollout or financial closure.

## Current phase

**Project financial write gate Wave 1 implemented over the verified custody foundation.** Policy v1 meaning is unchanged. No settlement approval or financial closure capability has been enabled.

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

Implement Project Financial Write Gate Wave 2B: enroll contract and owner operations, manager-contribution project effects, T23/T24 credit intent/effects, verified evidence and paper custody, policy exceptions, and operational completion/cancellation financial-basis changes. When real expense-return/refund and claim/debt resolution writers are published, enroll their typed T22 effects as part of the same universal-writer proof. Do not activate T01 enforcement or begin settlement approval/financial closure until universal writer coverage is implemented and proven.
