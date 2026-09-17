# Project status

## Current phase

**Settlement DDL, verification and cutover-rehearsal artifacts prepared — review pending, execution not approved.** Policy v1 meaning is unchanged. No settlement command or financial closure capability has been enabled.

## Baselines and branch

- Date: 2026-09-17; workspace: `C:/dev/Ahdah`.
- Policy: `f962d6119281906b5144b80fe1ffdac7e4525d9e`.
- Reviewed schema design: `a8df2e8d48c110959c172a1f35835ee973b495aa`.
- Branch: `design/settlement-ddl-artifacts`, created directly from the reviewed design commit.
- Existing history preserved. `frontend/ahdah_app/devtools_options.yaml` remains unread, untouched and untracked.

## Delivered artifacts

- [database/settlement](database/settlement/README.md): 18 SQL files covering all 26 new tables, five reviewed existing-table changes, forward dependency order, least-privilege installation, metadata preflight/verification, authorized future business reconciliation, synthetic structural rehearsal, staged enforcement and pre-cutover empty-schema rollback.
- 420 new-table columns; 500 named table constraints (plus eleven deferred constraint-trigger entries, excluding implicit PostgreSQL NOT NULL entries); 219 indexes; 61 triggers; eleven new functions. Two deployed trigger functions reused without modification.
- [DDL review](docs/SETTLEMENT_DDL_REVIEW.md), [index workload register](database/settlement/INDEX_REVIEW.md), [24 fixture specifications](database/settlement/REHEARSAL.md) and [seven two-session race schedules](database/settlement/RACES.md).
- Schema installation, legacy population, invariant verification, enforcement activation and operational enablement are separate. New runtime tables remain read-only after installation. All applicable entries in the 48-writer inventory still require upgrades before closure.

## Catalog and static verification

- Recaptured catalog metadata in READ ONLY/ROLLBACK: PostgreSQL 18.4, ahdah_db/ahdah, 91 tables. No differences found in the 49 recorded relevant table signatures/keys/FKs/triggers.
- Metadata-only preflight executed successfully. It checks 7,966 metadata signature rows using fingerprint `d697e4fbc6699faed6076d576a814d38`, version/schema/functions/table count and proposed object-name conflicts. No customer/business rows or sequence values queried; no credentials printed or committed.
- Local pglast 8.4 SQL/native PL/pgSQL parsing completed without syntax errors. Its high-level trigger-AST JSON decoder has a reproducible wrapper error; native parser syntax acceptance is recorded separately from execution/binding validation.
- Static dependency, tenant FK column/unique-key coverage, identifier length, duplicate names, artifact inventories and documentation checks performed. Working/staged diff checks and final intended-file scope are verified before commit.
- No DDL, schema/data mutation, privilege change, fixture execution, invariant/business-row query, migration, EF scaffold, C#, Dart, ARB or generated-model change. No application builds/tests run. No safe disposable database was created or assumed; 24 fixtures and seven races are not execution passes.

## Resolved clarification and residual risks

The owner-approved physical-design correction resolves Opening authorization: confirmed AdvanceDelivery/BalanceCreated receipt or approved evidenced legacy Balance review establishes unassigned custody only. Separate Designate remains Manager-authorized and gated. Project Correction uses CorrectCustody project authority; company-only Correction uses existing approved balance-resolution or legacy-review authority. See the DDL review for source integrity and unchanged Policy v1.

Trigger behavior, database binding, privileges, packet sealing concurrency and all application writers need separately authorized staging proof. The SQL harness provides structural probes; future command-level fixture bindings must test the actual missing commands. The approved schema has no per-company enforcement activation flag, so companies may be prepared in phases but structural activation waits for all-company coverage. Runtime ownership/inherited privileged roles and unrestricted legacy writers remain blockers to closure activation.

## Exact next task

Review [SETTLEMENT_DDL_REVIEW.md](docs/SETTLEMENT_DDL_REVIEW.md) and the SQL artifacts. Verify the clarified source paths and operation scopes. After separate explicit staging-execution authorization, verify backup/restore and role separation, rerun metadata preflight, execute only the approved installation on the named isolated staging target, compare catalog/deparsed definitions and run authorized structural probes. Do not enable closure, populate real cutover data, scaffold EF or implement commands without their separately approved scope. Production execution remains separately gated.
