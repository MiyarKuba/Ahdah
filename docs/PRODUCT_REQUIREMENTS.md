# Product requirements

## Product summary

Ahdah — عُهدة is a multi-tenant financial custody and construction operations platform. Its purpose is to give construction companies a reliable, auditable view of project funding, custody, spending, liabilities, settlements, supporting documents, and operational responsibility.

## Goals

- Provide a company-isolated system of record for construction projects and financial custody operations.
- Make the flow of funds between companies, projects, and responsible people traceable.
- Support evidence-backed expenses, receipts, debts, reimbursements, and settlements.
- Preserve an audit trail suitable for financial and operational review.
- Deliver consistent workflows across Android, iOS, and Web.
- Enable notifications, scheduled processing, and report generation without weakening tenant or financial controls.

## Actors

Known actor groups include:

- Company administrators and authorized company users
- Project managers and deputies
- Accountants and financial reviewers
- Site supervisors
- Workers and claimants
- Users responsible for funding, custody, transfer, or settlement actions
- Operational staff responsible for documents, notifications, and reports
- Background and scheduled processes acting under explicit system and tenant context

Exact permissions, approval limits, separation-of-duty rules, and role mappings are pending review. No authorization model is implemented in the foundation phase.

## High-level scope

- Companies, users, and tenant membership
- Construction projects and assigned operational roles
- Funding sources
- Cash advances and custody balances
- Transfers between users and projects
- Project expenses and receipts
- Supplier debts and payments
- Worker reimbursement and personal claims
- Advance settlement and closure
- Documents and attachments
- Notifications
- Audit logs
- Background jobs
- Scheduled reports, report generation, and delivery

## Quality expectations

- Strict tenant isolation through `company_id`
- Transactional, auditable financial changes
- Decimal-safe financial values
- Clear and versionable API contracts
- Secure handling of credentials and personal or financial data
- Responsive, accessible client behavior across supported targets
- Observable server operations with correlation identifiers

## Out of scope for initialization

Business workflows, authentication, authorization, database access, entity scaffolding, financial calculations, background processing, deployment, and external-service integrations are intentionally excluded from this task.
