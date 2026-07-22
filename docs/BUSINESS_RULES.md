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

## Rules pending implementation review

- Company onboarding, suspension, and tenant membership lifecycle
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
