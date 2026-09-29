# Settlement staging DDL validation — safety gate record

## Universal enforcement rehearsal — 2026-09-27

An isolated clone-based rehearsal on repository-local PostgreSQL 18 at `127.0.0.1:55440` passed. Activation rejected missing T01, missing T02, T02 total drift, T05 reservation drift, and uncertified `FinanciallyClosed`. Renaming the second trigger forced a late activation error; PostgreSQL rolled back the first enable. Valid activation enabled all three reviewed triggers in roughly 114 ms with no observed deadlock.

The final fresh-install check restored the verified 91-table baseline rather than cloning an already installed settlement database. The current 01–10 DDL installed successfully, `90_verify_catalog.sql` confirmed all 117 tables and disabled enforcement state, 95 verified its exact matrix, and 94 enabled all three triggers in 81 ms. Nine restricted-runtime regression tests passed with zero failures or skips. Static parsing covered all 19 SQL files, 805 statements, 12 function definitions, and 26 anonymous blocks.

Application acceptance ran as `ahdah_staging_runtime`, verified by `current_user`, after applying the exact writer grants in `95_runtime_writer_privileges.sql`. Direct ALTER/disable, TRUNCATE, immutable DELETE/UPDATE, schema CREATE, and unused T06 writes failed for insufficient privilege. Project/T01, balance/T02, and advance-currency bypass probes were rejected. All current real PostgreSQL writer/race paths passed. All `ahdah_enforcement_*` databases were removed; production `ahdah_db` was never targeted. The repository-local cluster was left in its pre-task running state.

This does not authorize production activation. No post-history destructive rollback was attempted; `99_rollback_pre_cutover.sql` remains pre-cutover only. A reviewed operational trigger-disable/forward-recovery procedure is still a production blocker.

**2026-09-20. Status: SETTLEMENT PHYSICAL DDL VALIDATED FOR APPLICATION DEVELOPMENT.** All six time-boxed core areas pass on isolated PostgreSQL 18 staging: catalog, runtime privileges, custody conservation, packet sealing, Opening/Designate authority, and exact pre-cutover rollback. This authorizes Database-First application development only. It does not authorize production rollout, enforcement activation, customer cutover, or financial closure. Reviewed baseline: `a482cba2bd1f83fcbd3217b2638d27237af2903a`; branch: `test/settlement-staging-ddl`.

## Isolated target and baseline proof

- Existing PostgreSQL stayed on port 5432 at `C:/Program Files/PostgreSQL/18/data`, system ID `7665018367053543140`. It was used only for forced read-only metadata and schema export; `ahdah_db` was not mutated.
- PostgreSQL 18.4 staging ran only on `127.0.0.1:55432` from `C:/dev/Ahdah/.tooling/settlement-staging/pgdata`, system ID `7686554636196223216`.
- Deployment owner `ahdah_staging_owner` and runtime `ahdah_staging_runtime` are separate login roles. The runtime is not superuser, cannot create databases or roles, has no BYPASSRLS, and is not a member of the owner role.
- Source fingerprint: `d697e4fbc6699faed6076d576a814d38`. Proved schema-restored fingerprint: `b649a5ab846cc272287afd0217f33bfd`.
- Both catalogs produced 7,966 signature rows. A focused comparison covered 3,027 CHECK, index and generated-column objects. All 450 differences (417 CHECKs, 32 indexes and one generated expression) were PostgreSQL representation-only differences; identifiers, constants, operators and non-expression catalog facts matched. No semantic or structural difference remained unproved.
- Full clean staging backup: custom format, created 2026-09-19 14:09:12 UTC, 1,393,443 bytes, SHA-256 `B6D51C2132BA20E51C82E903F14A734769CABE41C9244BB42429348E2C566635`. It restored independently into `ahdah_settlement_restore`, whose exact-name and fingerprint preflight passed.

All 18 executable SQL entry points now require an explicit `expected_database`. Preflight also requires the caller-selected lowercase MD5 `expected_catalog_fingerprint`; it does not choose a baseline automatically. The rehearsal additionally requires `ahdah_settlement_rehearsal_<suffix>`. These checks do not authorize production execution.

## Installer atomicity correction

### Root cause

The first installation reached `10_runtime_privileges.sql`, whose missing-schema-USAGE guard ran `\quit 3` from a file loaded with `\ir`. PostgreSQL 18 psql reported `\quit: extra argument "3" ignored`; ending the included file returned control to the parent runner. No SQL error had aborted the open transaction, so `install_reviewed.sql` reached `COMMIT` and psql exited 0. The failure mechanism was therefore neither transactional nor propagated to the top-level runner.

### Correction

- `install_reviewed.sql` owns the single `BEGIN`/`COMMIT`, enables `ON_ERROR_STOP` first, validates operator variables before `BEGIN`, and converts rejected top-level values into SQL exceptions and a nonzero exit.
- Modules 01–10 contain no psql conditional/quit failure paths and no transaction-control statements. Transactional prerequisites use SQL exceptions.
- `10_runtime_privileges.sql` validates role safety, grants `USAGE` on schema `ahdah`, revokes schema `CREATE`, grants `SELECT` on the 26 new tables, revokes write/table privileges and execution on the 11 protected functions, and verifies the resulting state. Missing preexisting schema `USAGE` is no longer a rejection condition.

### Late-failure rollback proof

A clean disposable baseline database was installed with a deliberately unsafe late runtime role that inherited owner authority. Module 10 raised its SQL exception after modules 01–09 had run; psql exited 3. Post-failure evidence showed 91 tables, zero T01–T26 tables, none of the five existing-table alterations, zero settlement functions, unchanged runtime privileges, and a passing baseline fingerprint. This proves late failure rolls back early modules and prevents parent `COMMIT`.

## Clean restore and successful installation

The partially committed staging database was dropped and recreated only inside the isolated cluster, then restored from the proved full backup. Before retry it had 91 tables, no settlement objects, none of the five reviewed alterations, and the exact restored fingerprint. The corrected install completed in 1,165 ms with exit 0 and committed 26 new tables, five reviewed alterations, 11 functions, and the reviewed constraints, indexes, triggers and privileges. `94_activate_enforcement.sql` was not run.

## Final DDL repair pass

The first valid Designate probe exposed a PL/pgSQL name-resolution defect: record variable `b` and SQL table alias `b` occurred in the same `settlement_custody_authority()` expression. PostgreSQL attempted to use the unassigned record variable while evaluating the reserved-designation predicate. The correction is mechanical: one-letter/local records were renamed to `operation_row`, `movement_row`, `balance_row`, `delivery_row`, `review_row`, `resolution_row`, and `authorization_row`; scalar IDs received descriptive names; overlapping SQL aliases were also made descriptive. Custody policy and predicates were unchanged.

All 11 settlement functions now declare `#variable_conflict error`. A static declared-variable/SQL-alias audit found zero collisions, and a clean forward install compiled all 11 functions with the directive present. No other function ambiguity required a semantic rewrite.

`99_rollback_pre_cutover.sql` now requires `runtime_role` and `runtime_schema_usage_preexisting=true|false` in addition to its existing approval and exact-database inputs. It rejects a missing or invalid value before mutation. At the end of the rollback transaction it grants or revokes only schema USAGE to reproduce the caller-recorded pre-install state. The isolated baseline recorded `runtime_schema_usage_preexisting=false`.

## Core validation register

| Area | Result | Evidence and limit |
|---|---|---|
| A. Catalog | **PASS** | `90_verify_catalog.sql` exited 0 on `ahdah_settlement_staging` with enforcement expected disabled. All reviewed object and privilege metadata checks passed. |
| B. Runtime privileges | **PASS** | As `ahdah_staging_runtime`, SELECT succeeded. INSERT, UPDATE, DELETE, TRUNCATE, protected-function execution and `ALTER TABLE ... DISABLE TRIGGER` each returned nonzero. Runtime has schema USAGE, no schema CREATE, SELECT on exactly 26 new tables, no owned tables, no owner membership, and safe role attributes. |
| C. Custody conservation | **PASS** | Transaction-scoped rehearsal accepted one unassigned slice and a conserving aggregate; rejected duplicate unassigned, negative amounts and aggregate drift; and used a savepoint to prove rejected changes left no partial financial state. The outer fixture transaction rolled back. |
| D. Settlement packet sealing | **PASS** | A children-first deferred packet completed, then an approved settlement was bound to it. Incomplete packet, late category insertion, snapshot update, snapshot delete and approved settlement rewrite were each rejected. The outer fixture transaction rolled back. |
| E. Opening / Designate | **PASS** | Live confirmed-delivery Opening and approved legacy-balance Opening each established exactly one unassigned position with no project authorization. Fabricated authorization and project-designated Opening were rejected. Valid Manager-authorized Designate succeeded with paired conserving movements and project revision updates; wrong project, action, and role links were rejected by their intended rules. Minimal Release and two-project Reallocate smoke paths also passed. The outer fixture transaction rolled back. These are structural rules, not application identity/authentication tests. |
| F. Pre-cutover rollback | **PASS** | On a separate clean installed database, an invalid privilege-state input exited 3 before mutation. The approved rollback with `runtime_schema_usage_preexisting=false` restored 91 tables, zero settlement tables/columns/functions/triggers, fingerprint `b649a5ab846cc272287afd0217f33bfd`, and runtime privilege signature `USAGE=false, CREATE=false, explicit table grants=0`, exactly matching the recorded pre-install signature. |

The rehearsal fixture was corrected to give its synthetic Manager the baseline-required pending identity-verification state. Packet probes were extended to cover incomplete construction, delete immutability and approved-history immutability. These are structural database checks, not the full F01–F24 application suite.

## Remaining scope and deferred work

No DDL blocker remains in the six-core validation scope.

Explicitly deferred until backend writers exist:

- closure versus the real expense writer;
- closure versus the real supplier writer;
- closure versus the real custody command;
- end-to-end settlement approval;
- Serializable application retry behavior; and
- the full 48-writer gate matrix.

No EF scaffolding, backend/application change, customer cutover, production reconciliation, enforcement activation, PR or merge occurred.
