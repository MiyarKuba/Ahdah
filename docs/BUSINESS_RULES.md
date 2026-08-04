# Business rules

This document records only known high-level rules. Detailed financial, approval, lifecycle, and exception rules are **pending implementation review** with product owners and the existing database design. Ambiguity must be reported; it must not be filled with an invented financial rule.

## Confirmed foundation rules

- Every tenant-owned operation is scoped to and enforced by `company_id` on the server.
- A client-provided company or record identifier does not establish authorization or ownership.
- Financial values use exact decimal representations: C# `decimal` and PostgreSQL `NUMERIC`, never `float` or `double`.
- Financial operations must be transactional and auditable.
- Ledger and financial-history records must not be silently edited or deleted.
- Supporting documents and receipts must be associated through reviewed, authorized workflows.
- Background jobs and scheduled reports must preserve explicit tenant context and authorization boundaries.
- The Flutter client cannot be the sole enforcement point for validation, permissions, or financial rules.

## Identity Phase 1 rules

- Company registration requires a unique uppercase alphanumeric `company_code` of 6–20 characters and an E.164 manager phone number matching the existing database checks.
- Registration creates exactly one company and one initial manager atomically. The client cannot choose company ID, user ID, role, status, timestamps, or version number.
- The bootstrap company and manager use exact stored values `Active` and `Manager`. Because the database prohibits an active manager unless identity status is `Verified`, the initial manager is recorded as `Verified` during owner bootstrap; phone and email are not represented as verified. This bootstrap assumption requires review before adding a separate external identity-proofing workflow.
- Exact supported user roles are `Manager`, `Deputy`, `Accountant`, `Supervisor`, and `Worker`.
- Exact user statuses are `PendingApproval`, `Active`, `Suspended`, `Inactive`, and `Rejected`.
- Exact company statuses are `PendingSetup`, `Active`, `Suspended`, and `Inactive`.
- Only `Active` users in `Active` companies may log in or use the current-user endpoint. Existing future lock timestamps are also honored without disclosing their cause.
- Phone is the only Phase 1 login identifier. It is globally unique. Email login is deferred because email uniqueness is company-scoped and no tenant-selection contract has been approved.
- Passwords are never stored or compared as plaintext. Framework password hashing and verification are mandatory, including explicit rehash handling.
- Tenant membership for authenticated requests comes only from the validated `company_id` JWT claim.
- Access tokens are short-lived. No refresh token or server revocation is available with the current schema; logout means client-side token discard.
- Invitations and join requests are handled only through the separately documented schema-supported access phase.

## Invitations and join requests

- Managers can create, list, and cancel only invitations belonging to the `company_id` in their validated JWT. Cancellation is a non-destructive `Pending` to `Cancelled` transition and stores the existing cancellation timestamp; the schema has no cancellation-actor field.
- Invitation roles are limited to `Deputy`, `Accountant`, `Supervisor`, and `Worker`. `Manager` can never be assigned through invitation or join-request workflows.
- Invitation lifecycle values are exactly `Pending`, `Accepted`, `Expired`, and `Cancelled`.
- A pending, unexpired invitation can be accepted once. Acceptance uses the invitation's company, phone, and role; the recipient cannot replace those values.
- Invitation lifetime is configurable and defaults to 168 hours (7 days) in development. The manager receives the raw token only once; delivery is out of band.
- Accepted `Supervisor` and `Worker` users become `Active` with identity `NotRequired` and receive authentication. `Deputy` and `Accountant` remain `PendingApproval` with identity `Pending` and receive no token. Acceptance never marks identity as verified.
- Raw invitation tokens are not stored or logged. Token lookup uses a deterministic SHA-256 hash of the URL-safe token.
- Join-request lifecycle values are exactly `Pending`, `Approved`, `Rejected`, and `Cancelled`; requested and assigned roles are limited to the same four non-manager roles.
- Public join submission creates a `PendingApproval` same-company `app_user` and a linked `Pending` request atomically. Its password hash is stored only on `app_users`; the request contains no password information.
- The approving manager chooses the final role; the applicant's requested role is provisional and informational only.
- Approval updates the linked existing user rather than creating another. `Supervisor`/`Worker` become active; `Deputy`/`Accountant` remain pending unless identity is already legitimately verified.
- Rejection marks the linked user `Rejected` without deleting it or erasing its password hash.
- Rejection requires a non-blank reason up to 500 characters because the existing database constraint requires one for `Rejected` records.
- Phone is globally unique, so the current schema supports one account/tenant membership per phone. Reapplication after rejection and company transfer require a future approved recovery workflow.
- Flutter presents requested roles as provisional, never offers `Manager`, and sends the exact invariant API role value rather than the localized label. Join submission does not authenticate the applicant. Invitation acceptance authenticates only when the API returns an access token; pending sensitive roles are shown as requiring identity verification without inventing a verification workflow.
- Manager access-administration UI may create/list/cancel invitations and list/approve/reject join requests only. It does not manage existing members, alter existing member roles, or perform identity verification.
- A manager-selected join approval role is final for the decision request. `Deputy` and `Accountant` outcomes explicitly communicate pending identity verification when returned by the server; `Supervisor` and `Worker` may activate according to the server outcome.
- The raw invitation token is displayed once after creation for explicit copying and approved out-of-band delivery. It is not stored in client state or local persistence, and closing the result intentionally makes it unrecoverable from the Flutter client.
- Client-side validation mirrors known request constraints for usability but never replaces API validation or authorization. Pending and status screens translate internal values into safe Arabic/English explanations and do not promise email or push updates.

## Company structure rules

- Manager and Deputy may read the tenant company directory. Accountant directory access remains unapproved; Supervisor and Worker have no full-directory access.
- Manager may list/view all tenant projects, create projects, update non-financial metadata, and replace the active supervisor set. Deputy is project read-only.
- Accountant may list/view tenant project identity and structure but never receives contract value and cannot view project members in this phase.
- Supervisor may list/view only projects proven by an active `project_supervisors` row and may view active supervisor members of those projects. Worker has no project visibility because the schema has no worker/project membership relationship.
- Project contract value is required, positive `NUMERIC(18,2)`. Manager alone supplies it during creation and receives it in responses. Direct changes are deferred to the schema's separate project-contract-change review/history workflow.
- A project requires a same-company project owner. Creation either verifies an existing active owner by tenant and owner ID or creates the supplied owner atomically; no automatic matching or merging occurs.
- A supervisor candidate must be an active same-company user with exact role `Supervisor`. Replacement preserves history by ending old active assignment rows rather than deleting them.
- New projects start as `Active`. Metadata updates may retain or transition between `Active` and `Paused`. `Completed`, `FinanciallyClosed`, and `Cancelled` transitions remain deferred.
- Project completion and financial closure do not imply or calculate advance settlement, supplier debt payment, invoice delivery, manager-fund settlement, or transfer/reallocation completion in this phase.
- Existing-member role/status mutations, identity verification, project-member writes, financial modules beyond the documented Advances Phase 1 foundation, and further Flutter company-structure changes remain deferred.

## Advances foundation rules

- Manager creates a top-level advance only for an active same-company exact `Deputy`. Company, creator, sender, status, timestamps, and balance values always come from authenticated/server state.
- Top-level funding allocates one or more existing same-company `Available` or `PartiallyUsed` funding sources. Allocations must share currency and total the fixed advance amount. New funding-source subtype creation and verification remain separate future workflows.
- Initial delivery, internal distribution, and unused-money return require recipient confirmation. Source availability moves to reserved before a pending operation and becomes used/transferred/returned only on confirmation.
- Manager may distribute money actually held to Deputy. Deputy may distribute money actually held to Supervisor or Worker. Accountant is read-only. Supervisor-to-Worker distribution is not approved in this phase.
- A holder may return unused money only to the single unambiguous upstream sender proven by confirmed transfer lineage. The client cannot select another destination.
- The recipient may reject a pending internal distribution or balance return, which releases the existing reservation without deleting history. Initial advance-delivery rejection is deferred because `advances` has no rejected state and its delivery is unique.
- `user_advance_balances` is authoritative. Every amount reservation or confirmation updates that row under lock and appends a corresponding immutable balance-ledger record in one transaction.
- All money uses positive two-decimal `NUMERIC(18,2)`/`decimal` values. Cross-currency allocations are rejected; exchange rates and conversion are not implemented.
- Every financial command requires tenant-scoped idempotency. A key cannot be reused by another operation, payload, or actor.
- Advance records have no direct project relationship. A source originating from a project-owner payment does not make a multi-source advance project-owned.
- An available balance of zero does not mean settled or closed. Expense allocation, receipts, supplier debt, claims, settlement resolution, closure, and FIFO settlement remain deferred.

## Expenses foundation rules

- Exact expense statuses are `Draft`, `PendingReview`, `CorrectionRequired`, `Approved`, `Rejected`, `Cancelled`, and `Reversed`. Phase 1 creates `PendingReview` and publishes only approve/reject transitions.
- Exact payment modes are `AdvanceBalance`, `SupplierCredit`, and `PersonalFunds`. Phase 1 creates the first and third only; supplier debt remains deferred.
- Expense creation is personal because incurred-by and submitter are authenticated state. No supervisor/worker hierarchy exists, so on-behalf-of creation is not inferred.
- A category scope of `ProjectOnly` requires the one direct project, `CompanyOnly` prohibits it, and `Both` permits either. Multiple-project splits are unsupported by the schema.
- Supervisor project association and visibility require an active assignment. Worker project association is unavailable because no worker/project relationship exists.
- Advance-backed creation locks each authoritative active holder balance, requires one currency and exact allocation total, moves available amount to reserved, and appends `ExpenseReserved`. Approval converts reserved to expensed with `ExpenseConfirmed`; rejection releases the reservation with `ExpenseReservationReleased`.
- Personal-funds creation atomically records one open expense-linked `personal_claim`. Rejection cancels only an untouched open claim. Payment, adjustment, write-off, and settlement remain separate workflows.
- Manager, Deputy, and Accountant may review pending expenses subject to company approval-separation settings. Rejection never deletes the expense or history.
- A category requiring a receipt cannot be approved without non-rejected Receipt/Invoice metadata. The schema has no no-receipt exemption state, so none is invented.
- Attachment Phase 1 records metadata only under active company size, MIME, and extension settings. It does not upload binary content, expose paths/hashes, or infer original-paper custody.
- Expense creation, attachment metadata, approval, and rejection require the existing tenant-scoped idempotency mechanism. Writes are transactional and never automatically retried.

## Rules pending implementation review

- Company onboarding after initial bootstrap, suspension, and tenant membership lifecycle
- Additional role delegation beyond the approved company-directory/project matrix
- Project worker membership and assignment writes
- Creation and verification of each funding-source subtype
- Advance cancellation/reissue and initial-delivery rejection
- Transfer correction, cancellation, and reversal workflows
- Supervisor-to-Worker distribution authority
- Expense correction, cancellation, reversal, document verification, no-receipt exception, and automatic-approval actor rules
- Supplier debt creation, aging, payment allocation, and closure rules
- Worker claim eligibility, evidence, approval, and reimbursement rules
- Advance settlement completeness and closure criteria
- Currency support, rounding, precision, exchange rates, and cross-currency behavior
- Financial period locking and correction procedures
- Attachment retention, classification, and access rules
- Notification triggers, recipients, escalation, and retry policy
- Audit retention and privileged-access review
- Report definitions, scheduling, delivery, and data-snapshot semantics

## Change control

No detailed rule is considered approved merely because it appears in an interface, a database column name, sample data, or developer assumption. Business-rule changes require explicit approval and corresponding updates to documentation, code, and tests.
