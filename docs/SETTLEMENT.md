# Settlement and closure foundation — Phase 1

## Discovery (before implementation)

Baseline: `115cc5a544f18cbc11213f1e080ab9599128cc72`. Generated Database-First entities, mappings, existing financial services, and business documentation were inspected. Read-only PostgreSQL catalog inspection confirmed lifecycle CHECK constraints on 2026-09-15. No database objects or generated files are changed.

| Area | Authoritative representation and consequence |
|---|---|
| Project | `projects.status`: Active, Paused, Completed, FinanciallyClosed, Cancelled. Completed and FinanciallyClosed require `completed_at` and `actual_end_date`. No project settlement row or financial-closure actor/date exists. Advance closure is a different lifecycle. |
| Assignments | Active tenant-scoped `project_supervisors` grants assigned-project visibility. No Worker assignment exists. |
| Advances | `advances` has no project ID. `user_advance_balances` stores available/reserved and received/restored/expensed/transferred/returned/adjustment totals. Its balance equation is authoritative. Expense funding lineage does not assign a whole advance or its remaining balance to a project. |
| Distributions/returns | `money_transfers` and `transfer_advance_allocations` represent delivery, internal transfer, and balance return. Draft, PendingConfirmation and CorrectionRequired are unfinished, but there is no project attribution. |
| Advance settlement | `advance_settlements` links a balance and versioned financial snapshot; difference and difference type are computed. Draft/PendingReview/CorrectionRequired/Approved/Rejected/Cancelled/Reversed. Resolutions have Draft/PendingApproval/Approved/Rejected/Cancelled/Reversed and funding-source/claim references. Neither has a project relation. |
| Advance closure | `advance_closures` snapshots advance version, balances, pending operations, unresolved issues, differences, debts and claims. Computed reconciliation requires all balances settled and zero pending/issues/available/reserved/difference. Approval requires ReadyToClose; administrative closure also requires authorization evidence. This cannot be applied to a project by renaming it. |
| Expenses | Direct optional project, exact lifecycle, category, payment mode, amount and currency. Items compute line totals; they do not introduce a separate settlement lifecycle. Rejected/Cancelled/Reversed are final excluded states. |
| Documents | Category `requires_receipt` and active company Always/Threshold/Never document mode already govern approval. Non-rejected Receipt/Invoice satisfies receipt policy; non-rejected metadata satisfies general document policy. Multiple rows must be queried through the table set because the generated navigation is singular. Original-paper custody and proof of actual uploaded bytes are unavailable. |
| Reimbursements | `personal_claims` has a direct optional project. Computed outstanding = claim + adjustment - paid - reduction - write-off. Open/PartiallySettled are unresolved; Settled/Cancelled/Reversed are final. Payment allocation links permit project-scoped pending claim-payment detection. |
| Supplier debt | Direct optional project and unique invoice expense. Computed outstanding = debt + adjustment - paid - credit - write-off. Pending payment allocations reserve payment capacity but do not reduce outstanding. Approved credit effects are already in the computed amount. |
| Payments/credit | No project on payment/credit headers: use tenant debt allocations. Draft/PendingApproval operations are unfinished. An approved credit's unused amount is not a project liability. Unallocated credit intent cannot be assigned to a project by supplier identity. |
| Returns/refunds | Expense returns inherit project from expense. Draft/PendingApproval/Approved/Rejected/Cancelled/Reversed. Refunds link returns and one funding source: PendingVerification/Confirmed/Cancelled/Reversed. Approved return effects need reconciliation; no complete effect-link contract exists for every resolution type (particularly credit notes/Other). |
| Manager funding | `manager_contributions` links a funding source and manager, not a project. No dedicated manager-settlement table exists. A `personal_claims(source_type='ManagerContribution')` row establishes a claim; only its direct project attribution is used. Contribution amount alone never becomes a repayment blocker. |
| Other funding | Funding sources, source allocations and cashbox entries carry availability and lineage, not project custody ownership. Owner-payment project allocations describe funding origin. Owner refunds and contract changes have direct project links and PendingApproval states. Owner refunds have no currency column; no currency is invented. |
| History/control | Balance, funding, supplier-debt and personal-claim ledgers are authority/history, not additional liabilities. Audit logs/idempotency records are not summed or exposed. Financial entities have version fields; custom concurrency tokens are outside generated files. Settings contain approval/tolerance controls but do not authorize silent write-offs or define project closure. |

## Design decisions

The read model must distinguish known blockers from incomplete assessment. There is no approved rule assigning advances to projects or defining a project settlement transition. A clean list alone cannot certify settlement. `canSettle` and `canClose` therefore use nullable booleans: false for a known impediment, null when full readiness cannot be established. Phase 1 never returns true for whole-project authorization. It separately reports whether the evaluated, visible financial categories have blockers. This is deliberate uncertainty, not a fabricated financial rule.

Supervisor receives assigned-project expense/document/debt blockers only; categories requiring broader financial rights remain explicitly not evaluated. Other people's claims, payments, credits and refunds must not leak through counts, amounts, IDs, or readiness derived from hidden records. Worker/unknown roles are forbidden. All visibility is enforced on the server.

## Endpoint and response contract

`GET /api/v1/projects/{projectId}/settlement` uses the existing Projects route and Problem Details conventions. No body, company ID, actor ID or idempotency key is accepted. The existing `SupplierViewer` outer policy supplies the exact financial-reader role allow-list; the service independently revalidates active membership, role and project assignment.

Response fields:

- `projectId`, `projectStatus`, `projectVersion`, `evaluatedAt`: identity, lifecycle and UTC evaluation time. The project version is not a version for the entire financial snapshot.
- `visibilityScope`: `CompanyFinancial` or `AssignedProjectLimited`.
- `hasKnownFinancialBlockers`, `totalBlockerCount`: visible financial reason count, not a complete project obligation count and not a unique-record count across categories.
- `canSettle`, `canClose`: nullable booleans. `false` means a known impediment; `null` means undetermined. These are always serialized, including nulls. Neither authorizes a command and neither returns true in Phase 1.
- `settlementReadiness`, `closureReadiness`: `Blocked` or `Indeterminate`.
- `categories`: category, evaluation status (`Evaluated`, `NotVisible`, `NotAttributable`), amount meaning, reason count, currency totals, and blocker references. Empty `NotVisible` data means unassessed, not clear.
- Blockers have stable `code`, `recordType`, `recordId`, `status`, optional `currencyCode`/`amount`, and optional `resourcePath` to an existing API. References with no published read route retain their type/ID; no fictitious route is emitted. Expense returns link their parent expense.
- `evaluationGaps`: stable category/code pairs explaining missing attribution, policy, visibility, settings, or return-effect verification.
- `settlementImpediments` and `closureImpediments`: lifecycle/known-blocker reason codes; they are separate from financial reason counts and monetary totals.

English display text is not the business contract. Flutter should localize codes, render explicit unknown/not-visible states, and keep unavailable actions disabled. A null must never be parsed as true or as zero blockers.

### Readiness decision table

| Situation | CanSettle | CanClose | Reason |
|---|---|---|---|
| Any visible financial blocker | false | false | KnownFinancialBlockers |
| Active or Paused, no visible blockers | null | false | ProjectNotCompleted for closure; settlement policy/attribution unknown |
| Completed, no visible blockers | null | null | Project settlement/closure policies undefined; attribution gaps remain |
| Cancelled | false | false | ProjectCancelled; existing exposure is still reported |
| FinanciallyClosed | false | false | ProjectAlreadyFinanciallyClosed; the historical status does not suppress current exposure |
| Unknown project status | false | false | UnknownProjectStatus |

Completed/FinanciallyClosed missing completion date/time additionally produces `ProjectCompletionEvidenceUnavailable`. `hasKnownFinancialBlockers=false` can coexist with Indeterminate readiness. No tolerance, implied repayment, automatic write-off, or project transition is invented from a company-setting name.

## Implemented financial categories

| Category | Normal blocker codes | Authority / amount meaning |
|---|---|---|
| Expenses | ExpenseDraft, ExpensePendingReview, ExpenseCorrectionRequired | Project expense header amount awaiting a final review state |
| ExpenseDocuments | MissingExpenseDocument | Existing category receipt and company metadata rules; no monetary total |
| Reimbursements | UnresolvedReimbursement | Open/PartiallySettled project claims except ManagerContribution; computed outstanding |
| ManagerContributions | UnsettledManagerContributionClaim | Only directly project-linked ManagerContribution claims; computed outstanding; excluded from Reimbursements to avoid duplication |
| SupplierDebt | OutstandingSupplierDebt | Open/PartiallySettled project debt; computed outstanding |
| SupplierPayments | PendingSupplierPayment | Draft/PendingApproval payment with debt allocations to this project; sum only those allocations |
| SupplierCredits | PendingSupplierCredit | Draft/PendingApproval credit with actual debt allocations to this project; sum only those allocations |
| ReimbursementPayments | PendingReimbursementPayment | Draft/PendingApproval claim payments; project allocation sums |
| ExpenseReturns | PendingExpenseReturn | Draft/PendingApproval returns through the project expense; return amount |
| SupplierRefunds | PendingSupplierRefund | PendingVerification refunds through return and project expense; gross refund amount |
| OwnerOperations | PendingOwnerRefund, PendingProjectContractChange | Direct project references, Manager only, no amounts/currency exposed |

Advance balances, pending distributions/returns and advance settlement/closure records are **NotAttributable**, not zero-valued project blockers. The corresponding `ProjectAdvanceAttributionUnavailable` gap is unconditional, so it cannot disclose hidden advance existence. Funding origin and spending lineage do not establish remaining project custody.

Unknown states produce `UnknownExpenseStatus`, `UnknownSupplierDebtStatus`, `UnknownClaimStatus`, `UnknownSupplierPaymentStatus`, `UnknownSupplierCreditStatus`, `UnknownClaimPaymentStatus`, `UnknownExpenseReturnStatus`, or `UnknownSupplierRefundStatus` and a `FinancialRecordRequiresReview` gap. Null computed claim/debt amounts produce `OutstandingAmountUnavailable`, never a silently substituted zero.

### Final statuses, reservations and double counting

- Expenses: Approved ends review; Rejected/Cancelled/Reversed are excluded from both review and document blockers. Approved expenses still require the same current metadata evidence used by approval.
- Claims and debts: Settled/Cancelled/Reversed are excluded. Cancelling an untouched record can leave its original computed nominal amount positive, so amounts alone cannot establish exposure.
- Supplier/claim payments: Confirmed/Rejected/Cancelled/Reversed are excluded. Pending allocations are operations, not debt reductions. Confirmed payments, approved credits and write-offs are already reflected in stored/computed liability authority; their allocations/ledgers are not subtracted again.
- Credits: Approved/Rejected/Cancelled/Reversed are excluded. Approved unused credit is not a blocker. An unallocated pending note cannot be assigned to a project merely because its supplier has invoices there. `UnallocatedCreditProjectIntentUnavailable` records this limitation. Current credit writes allocate only after approval; pending allocated notes are principally relevant to existing/imported data.
- Returns: Approved/Rejected/Cancelled/Reversed are final review states. Approved does not prove every resolution's financial effect. An approved project return adds `ApprovedReturnEffectsRequireReconciliation`; this is an evaluation gap, not an invented outstanding amount. Refunds are assessed independently by their own lifecycle. Confirmed/Cancelled/Reversed refunds are excluded.
- One record/reason is counted once in each category. A pending expense with a missing document legitimately has two reasons, but its amount is present only in Expenses. A pending expense and its open claim/debt are separate records requiring different resolutions; no combined monetary total is published.

## Currency and access

All amounts use `decimal` backed by existing NUMERIC mappings. Totals are grouped by exact currency code **within a category**. There is no total across categories or currencies: pending operations, return amounts and outstanding liabilities overlap and must not be added together. Project payment totals never include another project's allocation. Owner refunds have no currency column, so their references carry no amount rather than guessing a default currency.

| Role | Settlement access |
|---|---|
| Manager | Tenant project financial categories plus pending owner-operation references |
| Deputy / Accountant | Tenant project financial categories; OwnerOperations explicitly NotVisible |
| Supervisor | Active assigned-project Expenses, ExpenseDocuments and SupplierDebt only; other financial categories explicitly NotVisible, no hidden counts/IDs/amounts |
| Worker / unknown | 403 |
| Missing/stale membership or inactive company | 401 |
| Missing/cross-company project, unassigned Supervisor project | Same safe 404 |

Every query root and tenant-owned subquery includes `company_id`. A server-derived project ID scopes expenses/debts/claims, and tenant-scoped allocation/return relationships scope dependent records. Client query fields cannot replace authenticated tenant/actor authority.

## Read consistency and performance

The service opens one explicit PostgreSQL `RepeatableRead` transaction and marks it `READ ONLY` before membership/assignment/category queries. This avoids mixing pre-payment and post-payment category states during concurrent financial writes. No row is tracked, updated, audited, or persisted merely to compute readiness; no replay or idempotency row is created. The result describes one committed snapshot and is not a lock or permission for a later command.

Queries select only project-scoped unfinished records or missing-document findings. Relationship existence checks and project payment/credit allocation sums execute in SQL. Category totals and stable ordering are calculated from these projected blocker records in the central summarizer. The number of queries is fixed per role, with no per-record query loop or cache. The response currently returns all matching project blockers; very large projects may need paged blocker detail with full SQL totals in a future version.

## Verification (2026-09-15)

- Full `dotnet restore`, `dotnet build`, and `dotnet test` completed successfully from `backend`.
- Build: 0 warnings, 0 errors. Tests: **356 passed**, 0 failed, 0 skipped (148 unit, 208 integration).
- This phase adds **96 tests**: 77 real-service/query cases, 13 HTTP/OpenAPI cases and 6 application-rule cases.
- Coverage includes no known blockers/indeterminate readiness, unapportionable advance balances, expense draft/review/correction and final states, documentation thresholds and multiple rows, claim/debt generated balances, pending payment allocation scope, allocated/unallocated credit lifecycle, returns/refunds, manager claims, mixed currencies/exact decimals, role/assignment/tenant boundaries, stale callers, missing projects, safe contracts and absent write routes.
- EF Core InMemory 10.0.10 is used only for tests of the actual service/query implementation. Fixtures supply computed columns as authoritative values and do not simulate database constraint enforcement. A multi-document fixture uses separate save contexts to avoid the generated singular navigation's tracking limitation.
- All ten category queries translated through Npgsql and also executed against PostgreSQL inside a read-only RepeatableRead transaction using a synthetic tenant. The complete service returned Success for an existing eligible project. These were read-only smoke checks, not a concurrent-write stress test or financial mutation test.
- `git diff --check` passed. Generated Database-First files and PostgreSQL objects are unchanged. Temporary inspection tooling was removed. Flutter/Android/iOS were not rebuilt or runtime-tested in this backend-only phase.

## Flutter consumer (2026-09-16)

Project Details now opens the named `projectSettlement` route at `/projects/:projectId/settlement`. Handwritten immutable settlement models preserve every DTO field, including nullable booleans and optional blocker amount/currency/API reference. An API repository uses the existing authenticated financial GET path, protecting decimal tokens before JSON decoding. Rendering uses exact two-decimal text with separate category/currency groups and no combined payable total.

The dedicated capability permits Manager/Deputy/Accountant/Supervisor and denies Worker/unknown roles. Backend assignment and tenant checks remain authoritative; a forbidden or missing/cross-company project renders a localized failure. Supervisor receives no company-financial detail links. `NotVisible` categories render only their label and access explanation, ignoring unexpected nested counts, totals, records, and links. `NotAttributable` explains unavailable attribution rather than zero. Unknown future values are localized as requiring review.

Settlement and closure have separate Blocked/Indeterminate messages and impediments. Null is never coerced; zero known blockers never produces a ready/clear state. Evaluation gaps appear before category details. Known findings expand into currency-separated totals, localized reasons/types/statuses, safe record IDs, and permitted detail links. `Expense`, `PersonalClaim`, `SupplierDebt`, `SupplierPayment`, and `SupplierCreditNote` map to existing named Flutter routes; unsupported record types have no link. The API `resourcePath` is retained as contract data but is never routed.

English/LTR and Arabic/RTL use the same responsive widgets. The screen supports initial loading, explicit and pull refresh, safe retries, and centralized 401 session expiry. Failed refresh removes stale financial data. No backend rule, schema, generated persistence model, or financial command changed in this consumer milestone. Full Flutter verification is recorded in [../PROJECT_STATUS.md](../PROJECT_STATUS.md); the backend verification above is historical and was not rerun for this Flutter-only change.

## Deferred scope and next task

The documentation-only [settlement policy decision review](SETTLEMENT_POLICY_DECISIONS.md) now evaluates advance attribution, machine-testable certification predicates, snapshots, permissions, post-close restrictions, cancellation, reopening and concurrency. Its D01–D16 matrix requires explicit product-owner decisions. Recommendations do not change this endpoint's current false/null semantics or authorize implementation.

The review confirms two implementation prerequisites: project version does not cover child financial mutations, and current expense/supplier-invoice creation checks project existence without enforcing a financial-closure lock. A future close command must re-evaluate authoritative data in its transaction and use a financial-write guard shared by every relevant writer. Locking the project only in the close command is insufficient. See the decision document's evidence register and concurrency section for source references and the proposed protocol.

No generated persistence model, PostgreSQL object or closure/settlement write endpoint changes. Original-paper custody, actual file storage verification, advance/project attribution, unallocated credit intent, complete approved-return reconciliation, financial finalization, and irreversible closure remain deferred. There is no dedicated manager-settlement table and no permission to assume every contribution must be repaid.

**Recommended next Codex task after D01–D16 approval:** Record the selected policies and produce a detailed Database-First schema and cutover design. Verify current PostgreSQL catalog metadata read-only; specify tenant/currency/conservation constraints, certification evidence and every writer's lock/revision obligations; prepare migration/reconciliation acceptance fixtures. Do not execute DDL, re-scaffold or implement commands before separate approval of the concrete schema artifact and cutover plan.
