# Settlement DDL preparation artifacts

**2026-09-17 — PREPARATION ONLY. NOTHING HERE HAS BEEN APPLIED.** Policy baseline `f962d6119281906b5144b80fe1ffdac7e4525d9e`; reviewed physical design `a8df2e8d48c110959c172a1f35835ee973b495aa`. See [design authority](../../docs/SETTLEMENT_SCHEMA_CUTOVER_DESIGN.md) and [DDL review](../../docs/SETTLEMENT_DDL_REVIEW.md).

## Inventory and execution dependencies

18 SQL files; 26 proposed tables; five altered existing tables; 420 new-table columns; 500 named table constraints (26 PK, 64 UNIQUE, 162 FK, 248 CHECK including three existing-table currency checks); 219 indexes (90 constraint-backed, 129 standalone); 61 triggers including eleven deferred constraint triggers; eleven new validation/sealing functions. Reuse two existing functions without modification. PostgreSQL 18 also creates NOT NULL catalog constraints; those implicit entries are excluded from 500. The eleven constraint-trigger catalog entries are additional to the named table-constraint count. [Every index workload](INDEX_REVIEW.md) is listed.

| File | Classification / objects | Future dependency and boundary |
|---|---|---|
| 00_preflight.sql | Metadata only | Before installation; exact baseline fingerprint; read-only, no business queries. |
| 01_core_financial_control.sql | CORE CONTROL / T01 | Gate authority before upgraded writers; foreign references completed in 09. |
| 02_custody.sql | CUSTODY / T02–T05 | Requires existing balance/advance roots and T01; approvals/evidence/legacy links resolve after other modules. |
| 03_settlement_certification.sql | CERTIFICATION / T06–T13 | Requires T01, policy/evidence roots; circular snapshot FKs installed deferred in 09. |
| 04_policy_evidence.sql | EVIDENCE/POLICY / T14–T18 | Required before certifying any packet; does not copy mutable settings or seed policies. |
| 05_owner_reconciliation.sql | OWNER RECONCILIATION / T19–T21 | Project, policy, evidence, independent approval. |
| 06_resolution_credit_intent.sql | RESOLUTION SUPPORT / T22–T24 | Existing return/debt/credit/ledger roots plus custody and authorization. |
| 07_legacy_cutover_schema.sql | LEGACY CUTOVER / T25–T26 | Records reviewed openings and affected scope; no inferred backfill. |
| 08_existing_table_alterations.sql | CORE CONTROL / OWNER RECONCILIATION | Three nullable denomination columns/checks only; the remaining two table changes are trigger attachments in 09. |
| 09_indexes_constraints_triggers.sql | All seven classes | Run after 01–08. Adds all FKs/indexes/functions/triggers. Existing-table enforcement triggers start disabled. |
| 10_runtime_privileges.sql | CORE CONTROL | Requires reviewed nonowner runtime role; grants SELECT only on new tables. No closure authority granted. |
| install_reviewed.sql | Future installation driver | Explicit approval flag, preflight, then 01–10 in one transaction; no rehearsal/cutover/activation. |
| 90_verify_catalog.sql | Metadata only | 117-table installed schema, columns, keys/FKs/index keys/null semantics/triggers/privileges; manual definition comparison required. |
| 91_verify_invariants.sql | LEGACY CUTOVER | Authorized business-row reconciliation for one company; counts violations, fails without repairing. |
| 92_cutover_precheck.sql | LEGACY CUTOVER | Authorized one-company baseline inventory by currency/status; never executed during preparation. |
| 93_cutover_rehearsal.sql | Fixture-only structural rehearsal | Empty dedicated database name, synthetic rows, rollback; full command-level fixtures in REHEARSAL.md. |
| 94_activate_enforcement.sql | CORE CONTROL / CUSTODY | Separately authorized all-company cutover checkpoint, reviewed openings and upgraded writers; enables existing-table triggers only. |
| 99_rollback_pre_cutover.sql | Pre-cutover rollback | Empty new tables, no denomination backfill, enforcement still disabled, no dependent writers; fails otherwise. |

DDL preparation covers every reviewed object. Operational enablement is phased: custody, policy/evidence, owner/effect/credit workflows and certification need not launch on one day. Modules are a dependency decomposition, **not independently deployable fragments** with incomplete FKs. The supplied driver installs the complete structural set atomically; a different phased DDL deployment must receive a dependency-complete execution plan first. T01 must exist before writers are upgraded; T02–T05 establish attribution; T06–T13 certification; T14–T18 evidence/policy; T19–T24 owner/effects/intent; T25–T26 cutover.

## Catalog baseline and preflight

Read-only metadata recaptured 2026-09-17: PostgreSQL 18.4, `ahdah_db`, `ahdah`, 91 tables. Compared 49 recorded table signatures, PK/UQ/FK definitions and triggers with the design appendix: no differences found. Existing immutable function always rejects UPDATE/DELETE; existing timestamp function sets updated_at. Neither is replaced.

Preflight checks database/schema/PostgreSQL 18 compatibility, built-in gen_random_uuid/sha256 (no new extension required), 91-table count, new-object name conflicts and **7,966 metadata signature rows** covering all baseline columns/types/nullability/defaults/generated/identity, constraints, indexes, triggers and schema functions. Sorted MD5 fingerprint: `d697e4fbc6699faed6076d576a814d38`. MD5 is a drift detector, not financial authentication. A difference fails rather than adapting DDL. The metadata-only preflight itself was executed successfully inside READ ONLY/ROLLBACK; no other artifact was executed against PostgreSQL.

## Installation, population, enforcement, feature enablement

1. Separate review/approval authorizes future staging installation. Verify backup/restore and runtime/deployment role separation. `psql -X` with ON_ERROR_STOP, supplied runtime_role and explicit true approval flag; use the driver only after authorization.
2. Driver runs metadata preflight, then one transaction for 01–10. Table/index creation is on empty new tables; no CONCURRENTLY index builds. Three nullable currency additions preserve existing rows with no currency default. Existing financial history and baseline constraints/indexes remain intact. Lock timeout fails rather than waits indefinitely. Stage timing/lock impact must be measured before production.
3. **No automatic backfill:** independently authorize holder declarations, source digests, control rows, position/reservation openings, policy/configuration approvals and reviewed denomination updates. Null currencies remain explicit gaps; NOT NULL is not included and needs a later reviewed proposal. All new rows must be entered through reviewed cutover/application procedures.
4. Existing projects/balances are not immediately broken: `trg_settlement_project_control`, `trg_settlement_balance_conservation` and `trg_settlement_advance_currency` are installed DISABLED. New-table integrity/sealing triggers are active. Missing controls never imply Open in an upgraded writer.
5. Run 90 metadata review and 91/92 separately authorized business reconciliation. Hold the maintenance boundary. 94 activates the three existing-table triggers only after all-company control/custody coverage and version-boundary review. The approved design has no per-company activation marker; this is deliberately a global structural activation checkpoint. Do not overload policy binding or infer activation from arbitrary row existence. Per-company feature rollout follows later.
6. **Closure cannot be enabled after schema installation alone. All 48 inventoried writer/action entries must be reviewed and all material writers upgraded to take T01 and the revision protocol. Financial closure remains disabled until then**, and until complete evaluator, independent authorization, reopening and real race tests pass. Installation privileges deny new runtime writes. Later command write grants require separate review; no privileged generic direct-DML integration is authorized here.

## Conservation backstop

Deferred row constraint triggers cover INSERT/UPDATE/DELETE on positions, reservations and (after activation) authoritative balances. They derive affected old/new balance IDs, order them, acquire each authoritative balance FOR UPDATE, then validate:

- sum(position.available)=balance.available;
- sum(position.reserved)=balance.reserved;
- each position.reserved=sum(Reserved reservation.amount);
- exactly one unassigned position for each affected surviving balance.

They never modify money, repair totals or create balancing entries. Existing balance CHECK still enforces cumulative received/restored versus expensed/transferred/returned/adjusted/current money. Posting lineage, allocation budgets and movement pairing remain application transaction duties. A reservation references an immutable position identity. All writers must lock their **entire** affected project/balance set in canonical order before DML. A deferred row trigger sees only its old/new parents; it cannot discover a transaction's future statements or retroactively prevent an out-of-order lock. Multi-row writes remain atomic; noncompliant writers may deadlock/abort, never get repaired silently. Full-transaction retries are required.

## Packet construction and immutability

Preallocate snapshot UUID; create Draft cycle; insert category rows before snapshot header with deferred FK; insert header; bind submission pointer; force constraints/commit. Header insertion seals. New category insertion checks the project gate and rejects an existing header. Deferred packet checks require nonempty categories, a valid header, and same insertion `xmin` for header/categories to reject concurrent attachment. This is a transient construction check, not durable business identity. Build the whole packet in one write subtransaction (a savepoint can wrap the whole packet). No impossible mandatory FK cycle exists: Draft cycle snapshot pointer is nullable; references become complete before commit.

Immutable triggers protect snapshots/categories/events/certificates/reopenings/authorization/policies/paper/evidence/effects/applications/movements. Conditional freeze triggers protect Posted operations, Approved cycles/owner schedules/reviews; parent-locking guards forbid child INSERT/UPDATE/DELETE after freezing. Identity/reference updates on mutable projections are restricted. Approved cycle supersession/invalidation remains event-derived. T01 pointer plus deferred same-project/revision checks governs active closure; Cancelled projection remains Cancelled. T11 records reopening without deleting old certification. Database checks do not prove Manager membership, policy completeness or beneficiary independence: those remain upgraded-command duties.

## Runtime privileges and residual authority

Runtime_role must already exist, be non-superuser/nonowner, and not inherit ownership of new tables. 10 revokes all new-table privileges from PUBLIC and that role, then grants SELECT only. It revokes PUBLIC/runtime execution on the eleven new trigger functions, never changes the two baseline functions, and requires existing schema USAGE. No role/password/secret is created or printed. Trigger functions use SECURITY INVOKER and fixed search_path; no SECURITY DEFINER escape hatch is supplied. 90 rejects inherited write/TRIGGER/TRUNCATE rights or ownership on new tables.

Owners/superusers and members able to SET ROLE to an owner can bypass ordinary privilege enforcement; use separate nonlogin ownership/deployment administration and audited maintenance. A runtime account that already owns baseline tables remains a residual bypass risk; changing existing ownership/whole-schema grants is outside the five reviewed alterations. Resolve before closure, never claim a REVOKE overcomes ownership. Existing baseline writers are not enrolled by this SQL. No schema-installation flag acts as a security boundary.

## Rollback

**POST-CUTOVER ROLLBACK IS NOT DROP TABLE.** 99 is deliberately stricter than merely no certificates: it requires all 26 new tables empty, all three new currency columns null, the three existing-table enforcement triggers still disabled, and an externally attested no-writer-dependency boundary. It locks the full set before checking. It drops only named new triggers/FKs/columns/tables/functions, no CASCADE; an unexpected dependency aborts. Original tables, functions, indexes, sequences, roles, ledgers and grants are preserved. If policy/review/opening rows already exist, this automated rollback refuses and requires a reviewed preservation/recovery plan. After any posted financial history, use forward correction or a coordinated full restore with post-boundary transaction accounting.

## Validation limits and next review

SQL and PL/pgSQL were parsed locally with pglast 8.4/libpg_query; no DDL was executed and no disposable database was created. The wrapper's trigger-AST JSON decoder has a reproducible error even for a trivial trigger; native parse_plpgsql_json completed parsing, so no claim is made about its JSON AST. Catalog reference/unique-key coverage, duplicate names, 63-byte limits, FK dependency ordering and artifact scope were checked. Parser acceptance is not PostgreSQL relation/type binding, trigger execution, privilege enforcement or race proof. 90 verifies structural metadata but CHECK expressions, partial predicates and function bodies still require its explicit deparsed-definition review.

## Approved Opening and Correction clarification — 2026-09-17

The inherited T03 rule required project-scoped T12 authorization for every Posted Opening, although company-unassigned custody has no project. The owner resolved this inconsistency: **Opening establishes unassigned custody; separate Designate assigns it to a project.** This corrects physical design, not approved Policy v1. T12.project_id remains mandatory.

| Operation | Authority and source | Scope and effects |
|---|---|---|
| Opening, live | Confirmed existing `money_transfers` AdvanceDelivery, linked advance, recipient confirmation and `balance_ledger_entries` BalanceCreated/Advance receipt | T03 money_transfer_id set, legacy review null, authorization_decision_id null; one unassigned T02, received/available equal delivery, no reserved amount; no project revision. |
| Opening, legacy | Approved T25 Balance review with evidence, source balance/version and retained boundary digest | T03 legacy_attribution_review_id set, delivery/auth null; current available/reserved enters unassigned T02; no replayed gross receipt or existing balance-ledger posting. A zero balance requires its unassigned position but no all-zero T04 row. |
| Designate | Allowed Manager T12 action `Designate`; affected project; reviewed legacy source when reserved boundary attribution is needed | Separate immutable paired unassigned debit/project credit; net available/reserved zero per balance; project gate and financial revision +1. |
| Release | Allowed Manager T12 action `Release` | Paired project debit/unassigned credit; no reserved movement; D01 reconciled holder, no project reservation or unresolved difference still required. |
| Reallocate | Allowed Manager T12 action `Reallocate` for both projects | Paired project effects, canonical gates, both revisions +1; no reserved or cross-currency move. |
| Correction, project-affecting | Allowed Manager T12 action `CorrectCustody` for every affected project, linked original Posted operation | Independent appropriate project authority; gates/revision +1; closed project must reopen; compensating immutable effects. |
| Correction, company-only | Approved existing advance_settlement_resolutions linked through advance_settlements to the target balance/currency, or approved evidenced T25 Balance review; original Posted operation | authorization_decision_id null; only the authoritative source balance's unassigned position; no invented project. Approved source is the authority, not a generic approval flag. |

Source inspection: `AdvanceService.ConfirmTransferAsync` dispatches AdvanceDelivery to `ConfirmDeliveryAsync`, which verifies recipient/advance, consumes funding allocations, creates the balance and BalanceCreated ledger referencing **Advance**, then records confirmation. InternalTransfer and BalanceReturn instead use `ConfirmAllocatedTransferAsync`: retain `ConfirmTransfer` movements and existing designation, even for a newly created recipient balance. They cannot be relabeled Opening.

DDL amendments: T03 distinguishes live/legacy Opening source paths and requires null project authorization; a typed nullable advance_settlement_resolution_id supports existing company-correction authority. T12 adds CorrectCustody. T03/T04 deferred authority triggers reject project Opening, validate receipt/review lineage and paired disposition effects, and require matching Manager action/scope. Two partial unique indexes prevent repeated posted Opening per source; a balance lock also checks duplicate opening across source paths. The new resolution FK has tenant-leading support. Posted headers/movements stay frozen. No sixth baseline table alteration or company-scoped T12 was introduced.

All writers must still validate actor membership/independence, approved source budgets and unapplied residual, original correction lineage, whole-command idempotency, evidence authenticity, exact currency and revision +1 under the universal gate. Source approval alone does not authorize reuse of an already consumed resolution. SQL structural checks do not replace those application duties. All five existing-table changes, T02 uniqueness, conservation, immutable history and closure controls remain intact.

Legacy sequence: preserve declarations, source/boundary versions/digests and project impacts in T25/T26; approved Opening initializes **unassigned** current custody; separate Manager-approved Designate operations establish evidenced project slices. For reserved boundary custody, construct final T05 reservations with the original allocation lineage in the same cutover transaction and force conservation before commit; never reparent an existing live reservation. Indeterminate review cannot authorize Posted Opening; provisional conserving positions remain explicitly unapproved cutover work until reviewed, and cannot certify closure.

Next task is artifact review, then separately authorized isolated staging installation, metadata/definition comparison and structural rehearsal. Production cutover, application commands and scaffolding require their own approved scope.
