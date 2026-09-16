# Project status

## Current phase

**Settlement Database-First schema and cutover design complete — awaiting design approval.** Approved D01–D16 Policy v1 remains unchanged. Schema execution and application implementation remain pending; financial closure stays disabled.

## Date, workspace and branch

- Date: `2026-09-16`.
- Workspace: `C:/dev/Ahdah`.
- Branch: `design/settlement-schema-cutover`, created from `f962d6119281906b5144b80fe1ffdac7e4525d9e` on `docs/settlement-policy-decisions`.
- Existing history preserved. No merge or PR.
- Pre-existing `frontend/ahdah_app/devtools_options.yaml` remains unread, untouched and untracked.

## Delivered scope

- [Schema and cutover design](docs/SETTLEMENT_SCHEMA_CUTOVER_DESIGN.md): 26 proposed new tables, five proposed existing-table changes, 48 current/future writer and boundary entries, 24 specified acceptance fixtures, detailed financial-control/custody/certification/reopen/evidence/owner/effect/credit/policy structures, constraint responsibilities, canonical lock ordering and legacy rollout/rollback.
- Catalog metadata inspected read-only using existing secure configuration: PostgreSQL 18.4, `ahdah_db`, `ahdah`, 91 tables, transaction read-only on. No business/customer rows or sequence values queried; no credentials printed or retained. Appendix A records 49 relevant table signatures from the full inspection.
- Compared all 91 generated table/column sets and inspected scalar nullability, numeric precision, generated values, keys, relationships, defaults and index coverage. No missing table/column/PK/FK name or scalar nullability/decimal-generation discrepancy found. Expected omissions: live CHECKs/triggers and 22 expression indexes are not represented in generated mappings; zero/false CLR defaults are often omitted. Partial-unique singular navigations require set-based evaluation.
- Confirmed no project designation in custody, no aggregate financial revision, no universal project write gate or typed project closure history. The advance closure computed readiness omits separate debt/claim amounts; metadata-only attachments do not prove bytes or paper custody.
- README, settlement contract and policy document link the new artifact. No business policy reinterpreted; no deployed constraint directly contradicts the approved baseline.

## Design decisions

- One project financial-control row per project, with Open/InSettlement/Settled/FinanciallyClosed state, material revision and concurrency version, same-project active cycle/certificate pointers and attribution status.
- One nullable-project custody position per balance/project, including a unique explicit unassigned position. Current available/reserved totals reconcile exactly to authoritative balances; immutable movements are historical, not additional spendable money. Deferred aggregate triggers plus transaction logic are proposed; CHECKs do not validate cross-row sums.
- Immutable detailed snapshots, workflow events, closure certificates and reopen records. Completed closure projects operational FinanciallyClosed; Cancelled stays Cancelled. Reopening starts a new cycle, preserves history and advances material revision.
- Exact per-currency owner schedules retain qualified undisputed contractual retention as disclosed outstanding. Unknown/disputed/unclassified positions cannot certify closure. No cross-currency netting or implicit write-off.
- Versioned company/category digital and paper rules, separate verified binary evidence and physical-original events, evidenced scoped exceptions. Credit intent becomes actual allocation without double counting; typed return/difference effects prevent duplicate posting.
- Every material writer gates all affected projects before financial rows and increments each project once per logical command. Sorted project/balance locks, full authoritative approval/closure evaluation, Serializable certification/reopening and idempotent full-transaction retries are proposed. Existing writers must be upgraded before activation.
- Legacy cutover uses a drained financial-write boundary, holder declarations, reviewed opening slices/reservations, scoped uncertainty and unchanged historical ledgers. Unrelated reviewed projects are not blocked by unrelated company custody.

## Verification and scope boundary

- `git diff --check` and `git diff --cached --check`: passed. Complete documentation changes reviewed; local links, table structure and T01–T26/W01–W48/F01–F24 counts checked. Staged scope is exactly the five authorized Markdown files; temporary inspection files removed.
- No DDL, SQL migration, PostgreSQL mutation, EF scaffold or generated-model change; no C#, Dart or ARB modifications.
- No application builds/tests run for this documentation-only milestone. The 24 fixtures are specifications, not executed tests.
- Historical results only: 242 Flutter tests, clean analysis/formatting, Web release and Android debug builds in the prior Flutter milestone; 356 backend tests in the earlier backend foundation. These were not rerun here.
- No app launch, financial command, iOS verification, merge or PR. The requested documentation branch is to be committed and pushed after final checks.

## Remaining inputs and risks

Company/category evidence choices, actual contract denomination/retention terms, reviewed legacy attribution and financing classifications, durable binary storage and cutover declarations are still needed. No production/customer-row correctness was asserted. Deferred triggers, source/effect budgets, canonical snapshots, generated mappings and all writer/race paths require staging proof. Closure cannot activate while an old writer or direct DML bypass remains. Privileged maintenance requires controlled suspension and reconciliation; post-posting rollback cannot drop history or restore only part of the financial state.

## Exact next task

After explicit approval of [the schema design](docs/SETTLEMENT_SCHEMA_CUTOVER_DESIGN.md), prepare reviewable Database-First forward/rollback DDL and metadata verification/cutover rehearsal scripts for T01–T26 and the five listed existing-table alterations. Do not execute those scripts, mutate any database, scaffold models or implement commands in that preparation task. Present the artifacts and staging execution/rollback checklist for separate execution authorization.
