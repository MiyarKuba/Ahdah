# Project status

## Project

Ahdah — عُهدة

## Current phase

Settlement Policy v1 Approved — detailed schema/cutover design and implementation pending

## Date, workspace, and branch

- Date: `2026-09-16`.
- Workspace: `C:\dev\Ahdah`.
- Branch: `docs/settlement-policy-decisions`.
- Baseline: `eb5fbadb6387557dc87cbba6b114c3d3374f4d82` (`feat: add Flutter project settlement readiness`).
- Documentation branch created directly from the verified baseline. Existing history and main preserved.
- Pre-existing `frontend/ahdah_app/devtools_options.yaml` remains untouched and untracked.

## Delivered scope

- [docs/SETTLEMENT_POLICY_DECISIONS.md](docs/SETTLEMENT_POLICY_DECISIONS.md): 17-section product/accounting/schema/authorization/lifecycle review, source evidence register, five advance-attribution options, concrete shared-custody example, transfer/return comparisons, eligibility predicates, snapshot alternatives, role matrix, post-close operation rules, reopening, cancelled-project run-off, currency rules, schema classifications, and phased rollout.
- D01–D16 decisions now recorded as approved policy v1, with D01 release clarification, D06 configurable paper policy, D08 qualifying contractual retention and D11 practical independent-approval rule. No policy is implemented by this documentation update.
- Approval-record update follows review commit `6b72dfea875b56ce027a3534aaef204283e50954`; the original recommendations remain in Git history.
- Relevant README phase wording and settlement-contract documentation updated. No application, generated model, migration, SQL or PostgreSQL change.

## Confirmed current-system findings

- Funding source, custody holder, project intent, recorded project expenditure and unspent custody are distinct. Existing advances, user balances, transfers and advance settlement/closure entities do not establish project ownership of remaining funds.
- Expenses/claims/debts have project relationships; shared payments/credits use allocations. Existing category totals overlap and must never be combined; currencies remain separate.
- Expense and supplier-invoice creation currently validate project existence without a comprehensive FinanciallyClosed write guard. A status-only close command would not safely lock the project.
- Project version is not an aggregate financial revision. Per-record versions and a read-only RepeatableRead GET cannot authorize a future certification; that command must re-evaluate complete authoritative state inside its write transaction.
- Project status/completion fields, generic audits and advance-level closure records do not preserve an adequate project certification with immutable evidence, actor, reason, timestamp, policy and financial revision.
- Existing document metadata does not prove verified binary evidence or original-paper custody; approved-return effects, credit intent, legacy attribution and final owner/currency rules have unresolved gaps.

## Approved policy baseline

- Explicit project allocations within user/advance custody, including an explicit company-unassigned component and immutable movement history. Designation release does not erase company custody or repay a liability.
- Separate physical completion from financial reconciliation. Approved settlement records a complete reconciliation at a particular financial revision/policy; closure independently certifies the current settlement and locks ordinary financial activity.
- Accountant/Deputy prepare/submit; independent Manager approves/closes; Supervisor acknowledges physical completion only. Current read scope remains limited; no default self-approval exception.
- Paper custody is configurable (Required / Not required / Required for selected categories); explicit evidence-backed exceptions are allowed, and missing policy never means Not required.
- Qualifying undisputed contractual owner retention can remain outside immediate cash settlement only with known amount/currency, contractual basis, due/release condition and explicit classification. Unknown/disputed/unclassified owner positions block final closure. Retention remains disclosed outstanding, never silently zeroed.
- Accountant/Deputy prepare allocation/reallocation/release; Manager approves. Release requires reconciled holder custody, no project reservation or unresolved difference, and recorded reason/approval.
- A later explicitly configured delegated approver supports small companies; no silent self-approval or permanent two-Manager requirement. Beneficiary conflicts require another authorized reviewer.
- Persist detailed snapshots and certification/reopening history. Deliver controlled Manager reopening with a new cycle before closure activation; return normal projects to Completed, preserve Cancelled for run-off cases.
- Require all financial writers to participate in a shared project gate, authoritative closed-state check and financial-revision protocol. Support only narrow audited administrative corrections and append-only supplemental evidence after closure.
- Necessary schema proposals cover custody slices/effects, financial-control state, settlement snapshots, closure/reopening evidence, credit intent, return/difference effects, owner denomination/reconciliation and policy/cutover evidence. Recommended/optional additions and rejected shortcuts are classified in the decision document.

## Verification and review boundary

- `git diff --check`: passed for the documentation milestone. Status/stat and documentation content reviewed; all 17 sections are present, local evidence links resolve, and table column counts are consistent.
- Repository implementation and generated Database-First mappings were inspected. No live database connection or catalog query was performed in this review. Earlier catalog findings are labeled historical; deployed constraints/triggers must be checked before concrete schema work.
- No application code changed. Backend/Flutter builds and tests were intentionally not rerun for documentation-only changes.
- Historical baseline only: 242 Flutter tests, clean analysis/formatting, Web release and Android debug builds passed in the preceding milestone; 356 backend tests passed in the backend foundation. These are not new verification claims.
- No app runtime launch, iOS verification, financial operation, database mutation, merge or PR is part of this milestone.

## Remaining decisions and risks

D01–D16 have been approved as clarified/amended in the decision document. Remaining work is detailed schema/cutover design, current catalog verification, explicit tenant/category/contract configuration, and implementation proof. No financial rule or missing configuration value may be inferred. Closure remains disabled until D16’s attribution, resolution workflows, universal guards, valid true-readiness evaluation, history, reopening and race tests are complete. No DDL, generated model refresh or application behavior is authorized by this documentation record.

## Exact recommended next task

Use approved policy v1 (including D01/D06/D08/D11 clarifications) to produce the detailed Database-First schema and cutover design for custody, certification, evidence and concurrency. Verify current PostgreSQL catalog metadata read-only, specify constraints and every writer's lock/revision obligations, and prepare migration/reconciliation acceptance fixtures. Do not execute DDL, re-scaffold or implement commands until the concrete schema artifact and cutover plan receive separate approval.
