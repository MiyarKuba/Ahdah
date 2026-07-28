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

## Rules pending implementation review

- Company onboarding after initial bootstrap, suspension, and tenant membership lifecycle
- Role capabilities for managers, deputies, accountants, supervisors, workers, and administrators
- Project responsibility assignment and delegation
- Funding-source eligibility and availability
- Custody issue, balance, transfer, and return rules
- Transfer initiation, acceptance, rejection, reversal, and approval rules
- Expense classification, receipt requirements, review, rejection, and correction rules
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
